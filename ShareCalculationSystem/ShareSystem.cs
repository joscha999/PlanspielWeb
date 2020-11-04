using Planspiel.Models;
using ShareCalculationSystem.Calculators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareCalculationSystem {
    public class ShareSystem {
        public List<ShareCalculator> ShareCalculators { get; } = new List<ShareCalculator>();
        public int MaxRequiredData { get; }
        public Func<Date, int, IEnumerable<SaveDataModel>> DataCallback { get; }

        public double DefaultShareValue { get; set; } = 100;

        public ShareSystem(Func<Date, int, IEnumerable<SaveDataModel>> dataCallback) {
            //TODO: add share calcs
            ShareCalculators.Add(new TestCalculator());

            DataCallback = dataCallback;
            MaxRequiredData = ShareCalculators.Max(sc => sc.CalculationPeriod);
        }

        public double Calculate(SaveDataModel current) {
            //get data
            var data = DataCallback(current.Date, MaxRequiredData);

            //calculate for all non calced
            var dataCount = data.Count();
            if (data == null || dataCount <= 1)
                return DefaultShareValue;

            //calculate current
            var shareDiff = 0.0d;

            foreach (var calculator in ShareCalculators) {
                var start = Math.Max(0, dataCount - calculator.CalculationPeriod);
                shareDiff += calculator.Calculate(data.Skip(start).Take(calculator.CalculationPeriod));
            }

            return Math.Max(0, data.Skip(dataCount - 2).Take(1).Single().ShareValue + shareDiff);
        }
    }
}