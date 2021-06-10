using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareCalculationSystem.Calculators {
	public class PolutionHitCalculator : ShareCalculator {
		public override int CalculationPeriod => 14;

		public override double Calculate(IEnumerable<SaveDataModel> data) {
			var current = data.First().AveragePollution;
			var change = 0d;

			foreach (var sd in data.Skip(1)) {
				var diff = sd.AveragePollution - current;
				if (diff > 0)
					change += diff;

				current = sd.AveragePollution;
			}

			return change * -30;
		}
	}
}