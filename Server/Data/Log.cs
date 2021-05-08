namespace Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    [Table("log")]
    public partial class Log
    {
        public int id { get; set; }

        public int id_employee { get; set; }

        public DateTime datetime { get; set; }

        public int operation { get; set; }

        [StringLength(50)]
        public string name_process { get; set; }

        public int time { get; set; }
        [JsonIgnore]
        public virtual Employee employee { get; set; }
        internal static string ToJsonList(List<Log> logs)
        {
            return JsonSerializer.Serialize<List<Log>>(logs);
        }
    }
}
