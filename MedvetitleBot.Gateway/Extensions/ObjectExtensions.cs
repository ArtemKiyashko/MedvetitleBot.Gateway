using Newtonsoft.Json;

namespace MedvetitleBot.Gateway.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object update) => update is null ? default : JsonConvert.SerializeObject(update);
    }
}

