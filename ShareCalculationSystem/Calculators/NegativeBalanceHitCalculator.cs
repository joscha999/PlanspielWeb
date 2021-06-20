using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareCalculationSystem.Calculators {
	public class NegativeBalanceHitCalculator : ShareCalculator {
		public override int CalculationPeriod => 14;

		public override double Calculate(IEnumerable<SaveDataModel> data) {
			var debt = 0d;

			foreach (var sd in data)
				debt += sd.Balance;

			debt /= 14;
			if (debt >= 0)
				return 0;

			return debt / 1_000_000_000;
		}
	}
}