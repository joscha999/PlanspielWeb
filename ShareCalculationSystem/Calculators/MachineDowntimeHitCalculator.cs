using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareCalculationSystem.Calculators {
	public class MachineDowntimeHitCalculator : ShareCalculator {
		public override int CalculationPeriod => 14;

		public override double Calculate(IEnumerable<SaveDataModel> data) {
			var current = 1 - data.First().MachineUptime;
			var change = 0d;

			foreach (var sd in data.Skip(1)) {
				var diff = 1 - sd.MachineUptime - current;
				if (diff > 0)
					change += diff;

				current = 1 - sd.MachineUptime;
			}

			return change * -2;
		}
	}
}