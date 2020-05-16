using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb {
    //https://www.blakepell.com/asp-net-core-storing-objects-in-tempdata
    public static class TempDataExtensions {
        /// <summary>
        /// Puts an object into the TempData by first serializing it to JSON.
        /// </summary>
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Gets an object from the TempData by deserializing it from JSON.
        /// </summary>
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class {
            tempData.TryGetValue(key, out object o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static bool TryGet<T>(this ITempDataDictionary tempData, string key, out T t) where T : class {
            t = tempData.Get<T>(key);
            return t != null;
        }
    }
}