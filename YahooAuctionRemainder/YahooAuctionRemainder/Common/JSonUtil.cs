using System;
using Newtonsoft.Json;

namespace YahooAuctionRemainder.Common
{
    public class JSonUtil
    {
        public static string SerializeToJson(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return json;
        }

        public static T DeserializeFromJson<T>(string jsonObj)
        {
            var result = JsonConvert.DeserializeObject<T>(jsonObj);
            return result;
        }
    }
}
