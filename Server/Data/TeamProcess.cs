using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    [Table("team_process")]

    public class TeamProcess
    {
        [Key]
        public int id { get; set; }
        public int id_team { get; set; }
        public int id_process { get; set; }
        public virtual Team team { get; set; }
        public virtual Process process { get; set; }

    }
}
