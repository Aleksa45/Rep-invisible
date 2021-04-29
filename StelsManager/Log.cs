using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StelsManager
{
    class Log
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("id_employee")]
        public int IdEmp { get; set; }
        [JsonPropertyName("datetime")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("operation")]
        public Operation Operation { get; set; }
        [JsonPropertyName("name_process")]
        public string NameProcess { get; set; }
        [JsonPropertyName("time")]
        public int time { get; set; }

        public static Log[] FromJsonArray(string json)
        {
            return JsonSerializer.Deserialize<Log[]>(json);
        }
    }
}
