using DAL.Models;
using Planspiel.Models;
using ShareCalculationSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repositories {
	public class ShareRepository {
		private const bool UseCachedCalculation = true;

		public static ShareRepository Instance { get; private set; }

        private readonly SaveDataRepository SaveDataRepository;

		private readonly Dictionary<long, List<int>> CalculationQueue = new();
		private readonly Dictionary<long, List<SaveData>> Cache = new();
		private readonly Dictionary<long, List<SaveData>> WriteQueue = new();

		private readonly List<Thread> CalculatorThreads = new();

		private readonly Dictionary<Thread, bool> ReadPrio = new();

        public ShareSystem ShareSystem { get; }

		public int CalculationQueueCount => CalculationQueue.Sum(kvp => kvp.Value.Count);

		public double GetCached(SaveData sd) {
			if (Cache.TryGetValue(sd.SteamID, out var list) && list.Count > 0)
				return list.Last().ShareValue;

			RequestCalculation(sd);
			return -1;
		}

		public List<SaveData> GetCachedForTeam(long steamID) {
			if (Cache.TryGetValue(steamID, out var list) && list.Count > 0)
				return list;

			return new();
		}

		public ShareRepository(SaveDataRepository saveDataRepository) {
            //NOTE: this is hacky, should really be using asp.nets dependency container
            Instance = this;

            ShareSystem = new ShareSystem(GetData);
            SaveDataRepository = saveDataRepository;
        }

		private IEnumerable<SaveDataModel> GetData(int unixStartDay, long steamID, int period) {
			if (UseCachedCalculation) {
				return InternalGetDataCached(unixStartDay, steamID, period);
			} else {
				return InternalGetDataDB(unixStartDay, steamID, period);
			}
		}

		private IEnumerable<SaveDataModel> InternalGetDataDB(int unixStartDay, long steamID, int period) {
			var prior = SaveDataRepository.GetPrior(unixStartDay, steamID);

			if (prior != null)
				yield return prior.ToModel();

			foreach (var sd in SaveDataRepository.GetPeriod(unixStartDay, unixStartDay - period, steamID)) {
				if (sd.UnixDays == prior.UnixDays)
					continue;

				yield return sd.ToModel();
			}
		}

		private IEnumerable<SaveDataModel> InternalGetDataCached(int unixStartDay, long steamID, int period) {
			if (!Cache.ContainsKey(steamID))
				BuildCache(steamID);

			return Cache[steamID]
				.Where(sd => sd.UnixDays >= unixStartDay - period && sd.UnixDays < unixStartDay)
				.OrderByDescending(sd => sd.UnixDays).Select(sd => sd.ToModel());
		}

		private SaveData GetByID(int id, long steamID) {
			if (UseCachedCalculation) {
				if (!Cache.ContainsKey(steamID))
					BuildCache(steamID);

				return Cache[steamID].Find(sd => sd.Id == id);
			} else {
				return SaveDataRepository.GetById(id);
			}
		}

		private SaveData GetPrior(int unixDay, long steamID) {
			if (UseCachedCalculation) {
				if (!Cache.ContainsKey(steamID))
					BuildCache(steamID);

				return Cache[steamID].Where(sd => sd.UnixDays < unixDay)
					.OrderByDescending(sd => sd.UnixDays).FirstOrDefault();
			} else {
				return SaveDataRepository.GetPrior(unixDay, steamID);
			}
		}

		private void Write(SaveData sd) {
			if (UseCachedCalculation) {
				if (!WriteQueue.ContainsKey(sd.SteamID))
					WriteQueue.Add(sd.SteamID, new List<SaveData>());

				WriteQueue[sd.SteamID].Add(sd);
			} else {
				SaveDataRepository.Update(sd);
			}
		}

		private void BuildCache(long steamID)
			=> Cache.Add(steamID, SaveDataRepository.GetForTeam(steamID, int.MaxValue, false).ToList());

		public double Calculate(SaveData data) => ShareSystem.Calculate(data.ToModel());

		public void RequestCalculation(SaveData sd, bool prio = false) {
			if (sd.Id == null)
				return;

			if (!CalculationQueue.TryGetValue(sd.SteamID, out var list)) {
				list = new List<int>();
				CalculationQueue.Add(sd.SteamID, list);

				var t = new Thread(new ParameterizedThreadStart(Work));
				CalculatorThreads.Add(t);
				ReadPrio.Add(t, true);

				t.Priority = ThreadPriority.AboveNormal;
				t.Start(sd.SteamID);
			}

			if (list.Contains(sd.Id.Value))
				return;

			if (prio) {
				list.Insert(0, sd.Id.Value);
			} else {
				list.Add(sd.Id.Value);
			}
		}

		private void Work(object steamID) {
			//TODO: update to use cancellation token when stop is requested
			var queueRef = CalculationQueue[(long)steamID];

			while (true) {
				if (queueRef.Count == 0) {
					if (WriteQueue.ContainsKey((long)steamID) && WriteQueue[(long)steamID].Count > 0
						&& !ReadPrio.Any(kvp => kvp.Value)) {
						WriteWriteQueue(WriteQueue[(long)steamID]);
					}

					Thread.Sleep(5);
					continue;
				}

				int id = -1;
				lock (queueRef) {
					id = queueRef[0];
					queueRef.RemoveAt(0);
				}
				var sd = GetByID(id, (long)steamID);

				//check if we have a prior day that wasn't calc'd yet
				var prior = GetPrior(sd.UnixDays, sd.SteamID);
				if (prior == null || prior._shareValue >= 0) {
					sd.ShareValue = (double)Calculate(sd);
					Write(sd);
				} else {
					lock (queueRef) {
						queueRef.Remove(sd.Id.Value);
						queueRef.Remove(prior.Id.Value);

						//insert these back at 0
						RequestCalculation(sd, true);
						RequestCalculation(prior, true);
					}
				}
			}
		}

		private void WriteWriteQueue(List<SaveData> dataToWrite) {
			foreach (var sd in dataToWrite)
				SaveDataRepository.Update(sd);
		}
    }
}