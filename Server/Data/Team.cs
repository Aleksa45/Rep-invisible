namespace Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Text.Json;

    public partial class Team
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Team()
        {
            employees = new HashSet<Employee>();
        }

        [Key]
        public int id_teams { get; set; }

        [Required]
        [StringLength(50)]
        public string team_name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> employees { get; set; }
        internal static string ToJsonList(List<Team> teams)
        {
            return JsonSerializer.Serialize<List<Team>>(teams);
        }
    }
}
