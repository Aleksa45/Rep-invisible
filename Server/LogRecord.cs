using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server
{
    class LogRecord
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("id_emp")]
        public int IdEmp { get; set; }
        [JsonPropertyName("datetime")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("operation")]
        public int Operation { get; set; }
        [JsonPropertyName("name_process")]
        public string NameProcess { get; set; }

        public static LogRecord[] FromJsonArray(string json)
        {
            return JsonSerializer.Deserialize<LogRecord[]>(json);
        }

        internal static string ToJsonList(List<LogRecord> logs)
        {
            return JsonSerializer.Serialize(logs);
        }
    }
}
