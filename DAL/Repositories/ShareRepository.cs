using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories {
	public static class ShareRepository {
		//TODO: make this shit configurable via web interface
		public static double CalculateSharePrice(SaveData data) {
			var profitInc = data.Profit / 100_000;
			var companyValueBase = data.CompanyValue / 1_000_000 * 3;
			var machineDowntimeDec = (1 - data.MachineUptime) * 100;
			var loanPaybackMulti = data.AbleToPayLoansBack ? 0.9d : 1d;
			var pollutionDec = data.AveragePollution * 1_000;

			var final = (companyValueBase
					+ profitInc
					- machineDowntimeDec
					- pollutionDec)
					* loanPaybackMulti;

			return final > 0 ? final : 0;
		}
	}
}