using Planspiel.Models;
using ShareCalculationSystem.Calculators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareCalculationSystem {
    public class ShareSystem {
        public List<ShareCalculator> ShareCalculators { get; } = new List<ShareCalculator>();
        public int MaxRequiredData { get; }
        public Func<int, long, int, IEnumerable<SaveDataModel>> DataCallback { get; }

        public double DefaultShareValue { get; set; } = 100;

        public ShareSystem(Func<int, long, int, IEnumerable<SaveDataModel>> dataCallback) {
			//TODO
			//PolutionHitCalculator :: -pollution | 14 days | trend
			//MachineDowntimeHitCalculator :: -downtime | 14 days | trend
			//ProfitabilityCalculator :: +companyValue +profit | 30 days | trend, check against each other
			//ProductionInvestmentCalculator :: +building +companyValue | 30 days | trend
			//ResearchBonusCalculator :: +newResearch | 2 days | trend
			//RegionInvestmentCalculator :: +region | 2 days | trend

			//RiskCalculator :: allData | 180 | spikes
			//DemandCalculator :: -remainingDemand | 60 days | trend

			//ShareCalculators.Add(new TestCalculator());

			ShareCalculators.Add(new PolutionHitCalculator());
			ShareCalculators.Add(new MachineDowntimeHitCalculator());
			ShareCalculators.Add(new ProfitabilityCalculator());
			ShareCalculators.Add(new ProductionInvestmentCalculator());
			ShareCalculators.Add(new RegionInvestmentCalculator());

            DataCallback = dataCallback;
            MaxRequiredData = ShareCalculators.Max(sc => sc.CalculationPeriod);
        }

        public double Calculate(SaveDataModel current) {
            //get data
            var data = DataCallback(current.UnixDays, current.SteamID, MaxRequiredData).OrderBy(d => d.UnixDays);

            //calculate for all non calced
            var dataCount = data.Count();
            if (data == null || dataCount == 0)
                return DefaultShareValue;

            //calculate current
            var shareDiff = 0.0d;

            foreach (var calculator in ShareCalculators) {
                var start = Math.Max(0, dataCount - calculator.CalculationPeriod);
                shareDiff += calculator.Calculate(data.Skip(start).Take(calculator.CalculationPeriod));
            }

            return Math.Max(0, data.Last().ShareValue + shareDiff);
        }
    }
}