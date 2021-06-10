using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareCalculationSystem.Calculators {
	public class ProductionInvestmentCalculator : ShareCalculator {
		public override int CalculationPeriod => 30;

		public override double Calculate(IEnumerable<SaveDataModel> data) {
			var currentBuildingCount = data.First().BuildingCount;
			var currentCV = data.First().CompanyValue;

			var changeBuildingCount = 0d;
			var changeCV = 0d;

			foreach (var sd in data.Skip(1)) {
				changeBuildingCount += sd.BuildingCount - currentBuildingCount;
				currentBuildingCount = sd.BuildingCount;

				changeCV += sd.CompanyValue - currentCV;
				currentCV = sd.CompanyValue;
			}

			if (changeBuildingCount <= 0)
				return 0;

			return changeCV / 10_000_000;
		}
	}
}