using System.Text.Json.Serialization;

namespace bot.Models
{
    public abstract class Auditable
    {
        [JsonIgnore]
        public DateTimeOffset DateCreated { get; set; }
    }
}