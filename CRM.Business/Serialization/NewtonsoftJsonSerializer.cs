using System.IO;
using Newtonsoft.Json;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace CRM.Business.Serialization
{
    public class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        private readonly JsonSerializer _serializer;

        public NewtonsoftJsonSerializer(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public string ContentType
        {
            get => "application/json";
            set { }
        }

        public string Serialize(object obj)
        {
            using var stringWriter = new StringWriter();
            using var jsonTextWriter = new JsonTextWriter(stringWriter);
            _serializer.Serialize(jsonTextWriter, obj);

            return stringWriter.ToString();
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            var content = response.Content;

            using var stringReader = new StringReader(content);
            using var jsonTextReader = new JsonTextReader(stringReader);
            return _serializer.Deserialize<T>(jsonTextReader);
        }

        public static NewtonsoftJsonSerializer Default =>
            new(new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
    }
}