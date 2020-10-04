using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories {
	public static class ShareRepository {
		//TODO: make this shit configurable via web interface
		public static double CalculateSharePrice(SaveData data) {
			var profitInc = data.Profit / 100_000;
			var companyValueBase = data.CompanyValue / 1_000_000 * 3;
			var machineDowntimeDec = (1 - data.MachineUptime) * 100;
			var loanPaybackMulti = data.AbleToPayLoansBack ? 0.9d : 1d;
			var pollutionDec = data.AveragePollution * 1_000;

			var final = (companyValueBase
					+ profitInc
					- machineDowntimeDec
					- pollutionDec)
					* loanPaybackMulti;

			return final > 0 ? final : 0;
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