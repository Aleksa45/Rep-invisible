namespace Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Text.Json;

    public partial class User
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string user_name { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string full_name { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(80)]
        public string position { get; set; }

        [Key]
        [Column(Order = 4)]
        public double rate { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int hour { get; set; }

        internal static string ToJsonList(List<User> users)
        {
            return JsonSerializer.Serialize(users);
        }
    }
}
