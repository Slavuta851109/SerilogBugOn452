using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LoggingAdapter.Logs.Serializers
{
    public static class ParametersSerializer
    {
        public static string Serialize(IEnumerable<object> parameters)
        {
            return parameters != null ? string.Join(",", parameters.Select(SerializeObject)) : null;
        }

        private static string SerializeObject(object requiredObject)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var serializedObject = requiredObject == null
                ? null
                : JsonConvert.SerializeObject(requiredObject, settings);
            return serializedObject;
        }
    }
}
