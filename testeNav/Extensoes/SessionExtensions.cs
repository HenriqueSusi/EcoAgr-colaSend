using Newtonsoft.Json;

namespace testeNav.Extensoes
{
    public static class SessionExtensions
    {
        // Serializa um objeto e o armazena na sessão
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));  // Serializa o objeto como JSON
        }

        // Obtém um objeto da sessão e o desserializa de JSON para um objeto do tipo T
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);  // Recupera o JSON armazenado
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
