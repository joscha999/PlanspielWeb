using DAL.Models;
using Planspiel.Models;
using ShareCalculationSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DAL.Repositories {
	public class ShareRepository {
		public static ShareRepository Instance { get; private set; }

        private readonly SaveDataRepository SaveDataRepository;

		private readonly List<int> CalculationQueue = new();

		private readonly Thread CalculatorThread;

        public ShareSystem ShareSystem { get; }

		public int CalculationQueueCount => CalculationQueue.Count;

		public ShareRepository(SaveDataRepository saveDataRepository) {
            //NOTE: this is hacky, should really be using asp.nets dependency container
            Instance = this;

            ShareSystem = new ShareSystem(GetData);
            SaveDataRepository = saveDataRepository;

			CalculatorThread = new Thread(Work);
			CalculatorThread.Start();
        }

		private IEnumerable<SaveDataModel> GetData(int unixStartDay, long steamID, int period) {
			var prior = SaveDataRepository.GetPrior(unixStartDay, steamID);

			if (prior != null)
				yield return prior.ToModel();

			foreach (var sd in SaveDataRepository.GetPeriod(unixStartDay, unixStartDay - period, steamID)) {
				if (sd.UnixDays == prior.UnixDays)
					continue;

				yield return sd.ToModel();
			}
		}

		public double Calculate(SaveData data) => ShareSystem.Calculate(data.ToModel());

		public void RequestCalculation(SaveData sd, bool prio = false) {
			if (sd.Id == null)
				return;

			if (CalculationQueue.Contains(sd.Id.Value))
				return;

			if (prio) {
				CalculationQueue.Insert(0, sd.Id.Value);
			} else {
				CalculationQueue.Add(sd.Id.Value);
			}
		}

		private void Work() {
			//TODO: update to use cancellation token when stop is requested
			while (true) {
				if (CalculationQueue.Count == 0) {
					Thread.Sleep(5);
					continue;
				}

				int id = -1;
				lock (CalculationQueue) {
					id = CalculationQueue[0];
					CalculationQueue.RemoveAt(0);
				}
				var sd = SaveDataRepository.GetById(id);

				//check if we have a prior day that wasn't calc'd yet
				var prior = SaveDataRepository.GetPrior(sd.UnixDays, sd.SteamID);
				if (prior == null || prior._shareValue >= 0) {
					sd.ShareValue = (double)Calculate(sd);
					SaveDataRepository.Update(sd);
				} else {
					lock (CalculationQueue) {
						CalculationQueue.Remove(sd.Id.Value);
						CalculationQueue.Remove(prior.Id.Value);

						//insert these back at 0
						RequestCalculation(sd, true);
						RequestCalculation(prior, true);
					}
				}
			}
		}

        //new system:
        //[ShareSystem], [ShareCalculator]
        //calculator calc's one specific aspect
        //only works on the difference on data + weighs it
        //data is added once per day, not per month
        //calculator is configured via WI

        //Must have:
        //PolutionHitCalculator :: -pollution | 14 days | trend
        //MachineDowntimeHitCalculator :: -downtime | 14 days | trend vs. current
        //ProfitabilityCalculator :: +companyValue +profit | 30 days | trend, check against each other
        //DemandCalculator :: -remainingDemand | 60 days | trend
        //ProductionInvestmentCalculator :: +building +companyValue | 30 days | trend
        //ResearchBonusCalculator :: +newResearch | 2 days | trend
        //RegionInvestmentCalculator :: +region | 2 days | trend
        //RiskCalculator :: allData | 180 | spikes

        //Nice to have:
        //TotalRateOfReturnCalculator :: (Profit + LoanInterest) / Money
        //MarketLiquidityCalculator :: ?
    }
}