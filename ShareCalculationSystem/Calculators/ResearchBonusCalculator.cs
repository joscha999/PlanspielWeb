﻿using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareCalculationSystem.Calculators {
	public class ResearchBonusCalculator : ShareCalculator {
		private const double Factor = 1 / 6;

		public override int CalculationPeriod => 2;

		public override double Calculate(IEnumerable<SaveDataModel> data) {
			var current = data.First().UnlockedResearchCount;
			var change = 0d;

			foreach (var sd in data.Skip(1)) {
				change += sd.UnlockedResearchCount - current;
				current = sd.UnlockedResearchCount;
			}

			return change * Factor;
		}
	}
}