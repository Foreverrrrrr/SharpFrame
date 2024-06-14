using Newtonsoft.Json;
using System;
using System.IO;

namespace SharpFrame.Structure.Debugging
{
    public class AxisConfigurationFile
    {
        public static void Get_Local_Profile<T>(string table, ref T t) where T : class
        {
            DirectoryInfo root = new DirectoryInfo(System.Environment.CurrentDirectory + @"\Configuration\");
            FileInfo[] files = root.GetFiles();
            if (Array.Exists(files, x => x.Name == table + ".json"))
            {
                string destinationFile = System.Environment.CurrentDirectory + @"\Configuration\" + table + ".json";
                string jsonStr = File.ReadAllText(destinationFile);
                T deserializeResult = JsonConvert.DeserializeObject<T>(jsonStr);
                t = deserializeResult;
            }
        }
    }
}
