namespace Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Text.Json.Serialization;

    [Table("employee")]
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            logs = new HashSet<Log>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string user_name { get; set; }

        [Required]
        [StringLength(50)]
        public string full_name { get; set; }

        public int id_position { get; set; }

        public double rate { get; set; }

        public int? id_teams { get; set; }
        [JsonIgnore]
        public virtual Position position { get; set; }
        [JsonIgnore]
        public virtual Team team { get; set; }
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Log> logs { get; set; }
    }
}
