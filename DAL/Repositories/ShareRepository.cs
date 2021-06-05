using DAL.Models;
using Planspiel.Models;
using ShareCalculationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories {
	public class ShareRepository {
        public static ShareRepository Instance { get; private set; }

        private readonly SaveDataRepository SaveDataRepository;

		private readonly Stack<SaveData> CalculationStack = new Stack<SaveData>();

        public ShareSystem ShareSystem { get; }

        public ShareRepository(SaveDataRepository saveDataRepository) {
            //NOTE: this is hacky, should really be using asp.nets dependency container
            Instance = this;

            ShareSystem = new ShareSystem(GetData);
            SaveDataRepository = saveDataRepository;
        }

        private IEnumerable<SaveDataModel> GetData(Date startDate, int period) {
			//var dbData = SaveDataRepository.GetPeriod(startDate, startDate.AddDays(-period));

			//foreach (var d in dbData) {
			//    if (d.Date.UnixDays == startDate.UnixDays)
			//        continue;

			//    if (d.InternalShareValue == -1) {
			//        d.ShareValue = ShareSystem.Calculate(d.ToModel());
			//        SaveDataRepository.Update(d);
			//    }
			//}

			//return dbData.Select(d => d.ToModel());
			return null;
        }

        public double Calculate(SaveData data) => ShareSystem.Calculate(data.ToModel());

		public void RequestCalculation(SaveData sd, int depth = 0) {
			if (CalculationStack.Contains(sd))
				return;

			CalculationStack.Push(sd);

			var previousDay = SaveDataRepository.GetForDate(sd.UnixDays - 1);
			if (previousDay != null)
				RequestCalculation(previousDay);
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