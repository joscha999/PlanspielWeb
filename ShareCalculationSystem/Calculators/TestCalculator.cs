using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareCalculationSystem.Calculators {
    public class TestCalculator : ShareCalculator {
        public override int CalculationPeriod => 14;

        public override double Calculate(IEnumerable<SaveDataModel> data) => 1;
    }
}