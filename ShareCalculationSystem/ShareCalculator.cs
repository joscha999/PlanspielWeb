using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareCalculationSystem {
    public abstract class ShareCalculator {
        /// <summary>
        /// How many days of data in the past this Calculator uses.
        /// </summary>
        public abstract int CalculationPeriod { get; }

        public abstract double Calculate(IEnumerable<SaveDataModel> data);
    }
}