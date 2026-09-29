using System.Text.Json;

namespace EGS_Framework.Core.JSON
{
    public static class Serializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string ToJson<T>(T data)
        {
            return JsonSerializer.Serialize(data, Options);
        }

        public static T FromJson<T>(string json, out bool success) where T : new()
        {
            success = false;

            if (string.IsNullOrEmpty(json)) return new T();

            try
            {
                var result = JsonSerializer.Deserialize<T>(json);

                if (result == null) return new T();

                success = true;
                return result;
            }
            catch
            {
                return new T();
            }
        }
    }
}