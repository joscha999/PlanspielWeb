using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareCalculationSystem.Calculators {
	public class RegionInvestmentCalculator : ShareCalculator {
		public override int CalculationPeriod => 2;

		public override double Calculate(IEnumerable<SaveDataModel> data) {
			var current = data.First().RegionCount;
			var change = 0d;

			foreach (var sd in data.Skip(1)) {
				change += sd.RegionCount - current;
				current = sd.RegionCount;
			}

			return change;
		}
	}
}