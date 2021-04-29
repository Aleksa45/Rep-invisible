using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StelsManager
{
    class DataContainer
    {
        public static DataContainer Instance = new DataContainer();

        public User[] Users;
        public Team[] Teams;

        public static Dictionary<string, string[]> GroupProcess = new Dictionary<string, string[]>()
        {
            { "Браузер",new string[]{ "msedge","firefox","chrome","opera"} },
            { "Проводник",new string[]{ "explorer"} },
            { "IDE",new string[]{ "devenv", "Teams" } },
            { "Office",new string[]{ "WINWORD", "EXCEL" } },
            { "SQL MS",new string[]{ "Ssms" } },
            { "1C",new string[]{ "1cv8ct" } },
        };

        public string GetKeyRecordGroup(Log logRecord, out bool isInstall)
        {
            isInstall = false;
            string key = logRecord.NameProcess;

            foreach (string keyD in GroupProcess.Keys)
            {
                if (GroupProcess[keyD].Contains(key))
                {
                    isInstall = true;
                    key = keyD;
                    break;
                }
            }

            return key;
        }

    }
}
