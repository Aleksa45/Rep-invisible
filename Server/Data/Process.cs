using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.Data
{
    [Table("process")]
    public class Process
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string os_name { get; set; }
        [JsonIgnore]
        public virtual ICollection<TeamProcess> team_processes { get; set; }

    }
}
