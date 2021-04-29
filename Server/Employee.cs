//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Server
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.logs = new HashSet<Log>();
        }
    
        public int id { get; set; }
        public string user_name { get; set; }
        public string full_name { get; set; }
        public int id_position { get; set; }
        public double rate { get; set; }
        public Nullable<int> id_teams { get; set; }
        [JsonIgnore]
        public virtual Position position { get; set; }
        [JsonIgnore]
        public virtual Team team { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Log> logs { get; set; }

    }
}
