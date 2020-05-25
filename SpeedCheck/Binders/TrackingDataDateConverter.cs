using System;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace SpeedCheck
{
    public class TrackingDataDateConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonSerializer = new JsonSerializer
            {
                DateFormatString = "dd.MM.yyyy"
            };

            return jsonSerializer.Deserialize<Models.TrackingData>(reader);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(Models.TrackingData);
    }

}
