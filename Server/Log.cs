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
    
    public partial class Log
    {
        public int id { get; set; }
        public int id_employee { get; set; }
        public System.DateTime datetime { get; set; }
        public int operation { get; set; }
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