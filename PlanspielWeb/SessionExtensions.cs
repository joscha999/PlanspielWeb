using DAL.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb {
	public static class SessionExtensions {
		public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings();

		static SessionExtensions() {
			Settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		}

		public static void Put<T>(this ISession session, string key, T data) where T : class
			=> session.SetString(key, JsonConvert.SerializeObject(data, Settings));

		public static bool TryGet<T>(this ISession session, string key, out T data) where T : class {
			var str = session.GetString(key);
			if (string.IsNullOrEmpty(str)) {
				data = default;
				return false;
			}

			data = JsonConvert.DeserializeObject<T>(str);
			return true;
		}
	}
}