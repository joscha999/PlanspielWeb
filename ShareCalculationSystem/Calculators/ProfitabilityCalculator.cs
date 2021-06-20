using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareCalculationSystem.Calculators {
	public class ProfitabilityCalculator : ShareCalculator {
		private const double PositiveAdjustment = 1.1d;

		public override int CalculationPeriod => 28;

		public override double Calculate(IEnumerable<SaveDataModel> data) {
			var currentProfit = data.First().Profit;
			var currentCV = data.First().CompanyValue;
			var changeProfit = 0d;
			var changeCV = 0d;

			var currLoanAmount = data.First().LoansList?.Sum(l => l.LoanAmount) ?? 0;
			if (currLoanAmount == 0)
				currLoanAmount = 20_000_000;

			foreach (var sd in data.Skip(1)) {
				var diff = sd.Profit - currentProfit;
				if (diff > 0)
					diff *= PositiveAdjustment;

				changeProfit += diff;
				currentProfit = sd.Profit;

				changeCV += sd.CompanyValue - currentCV;
				currentCV = sd.CompanyValue;

				var newLoanAmount = sd.LoansList?.Sum(l => l.LoanAmount) ?? 0;
				if (newLoanAmount == 0)
					newLoanAmount = 20_000_000;

				if (newLoanAmount != currLoanAmount) {
					changeProfit -= newLoanAmount - currLoanAmount;
					currentProfit -= newLoanAmount - currLoanAmount;
					currLoanAmount = newLoanAmount;
				}
			}

			return (changeProfit + changeCV) / 10_000_000;
		}
	}
}