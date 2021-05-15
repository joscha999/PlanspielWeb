using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DapperTypeHandlers {
	public class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset> {
		public override DateTimeOffset Parse(object value) {
			if (value == null || value is not string str)
				return new DateTimeOffset(0, TimeSpan.Zero);

			if (DateTimeOffset.TryParse(str, out var dto))
				return dto;

			return new DateTimeOffset(0, TimeSpan.Zero);
		}

		public override void SetValue(IDbDataParameter parameter, DateTimeOffset value) {
			parameter.Value = value.ToString();
		}
	}
}