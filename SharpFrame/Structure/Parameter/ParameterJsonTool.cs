using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace SharpFrame.Structure.Parameter
{
    public static class ParameterJsonTool
    {

        /// <summary>
        /// 测试测试保存文件夹
        /// </summary>
        public static string TestJsonFolder { get; set; } = "Test";

        /// <summary>
        /// 数据测试保存文件夹
        /// </summary>
        public static string DataJsonFolder { get; set; } = "Data";

        /// <summary>
        /// 创建Json格式参数
        /// </summary>
        /// <param name="table">参数名称</param>
        public static void NewJosn(string table)
        {
            string path = "";
            string basejson = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            basejson = "Base.json";
            if (Directory.Exists(path))
            {
                string sourceFile = path + "\\" + basejson;
                string destinationFile = path + "\\" + table + ".json";
                bool isrewrite = true; // true=覆盖已存在的同名文件
                System.IO.File.Copy(sourceFile, destinationFile, isrewrite);
            }
        }

        /// <summary>
        /// 创建Json格式参数
        /// </summary>
        /// <param name="table">参数名称</param>
        public static void NewJosn(string table, string copy)
        {
            string path = "";
            string basejson = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            basejson = copy + ".json";
            if (Directory.Exists(path))
            {
                string sourceFile = path + "\\" + basejson;
                string destinationFile = path + "\\" + table + ".json";
                bool isrewrite = true; // true=覆盖已存在的同名文件
                System.IO.File.Copy(sourceFile, destinationFile, isrewrite);
            }
        }


        /// <summary>
        /// 修改Json参数名称
        /// </summary>
        /// <param name="filename">原Json名称</param>
        /// <param name="new_filename">新Json名称</param>
        public static void ChangeJsonName(string filename, string new_filename)
        {
            string path = "";
            var t = GetJson();
            if (t.Count > 0)
            {
                if (t.Contains(filename))
                {
                    if (!t.Contains(new_filename))
                    {
                        path = System.Environment.CurrentDirectory + @"\Parameter";
                        string destinationFile = path + "\\" + filename + ".json";
                        string new_destinationFile = path + "\\" + new_filename + ".json";
                        System.IO.File.Move(destinationFile, new_destinationFile);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有Json格式参数名称
        /// </summary>
        /// <returns></returns>
        public static List<string> GetJson()
        {
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            if (Directory.Exists(path))
            {
                DirectoryInfo root = new DirectoryInfo(path + "\\");
                FileInfo[] files = root.GetFiles();
                List<string> strings = new List<string>();
                foreach (var item in files)
                {
                    var st = item.Name;
                    if (item.Name != "Base.json" && item.Extension == ".json")
                        strings.Add(st.Replace(item.Extension, ""));
                }
                return strings;
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 删除Json格式参数
        /// </summary>
        /// <param name="table">参数名称</param>
        public static void DeleteJosn(string table)
        {
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            string destinationFile = path + "\\" + table + ".json";
            System.IO.File.Delete(destinationFile);
        }

        /// <summary>
        /// 读取指定Json格式参数
        /// </summary>
        /// <typeparam name="T">Json反序列化类型</typeparam>
        /// <param name="table">参数名称</param>
        /// <param name="t">反序列化传递变量</param>
        /// <returns>读取结果</returns>
        public static bool ReadJson<T>(string table, ref T t) where T : class
        {
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            DirectoryInfo root = new DirectoryInfo(path + "\\");
            FileInfo[] files = root.GetFiles();
            if (Array.Exists(files, x => x.Name == table + ".json"))
            {
                string destinationFile = path + "\\" + table + ".json";
                string jsonStr = File.ReadAllText(destinationFile);
                T deserializeResult = JsonConvert.DeserializeObject<T>(jsonStr);
                t = deserializeResult;
                return true;
            }
            else
                return false;
        }

        public static ObservableCollection<T> ReadAllJson<T>(T t) where T : class
        {
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            DirectoryInfo root = new DirectoryInfo(path + "\\");
            FileInfo[] files = root.GetFiles();
            ObservableCollection<T> values = new ObservableCollection<T>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name != "Base.json")
                {
                    string destinationFile = path + "\\" + files[i].Name;
                    string jsonStr = File.ReadAllText(destinationFile);
                    T deserializeResult = JsonConvert.DeserializeObject<T>(jsonStr);
                    values.Add(deserializeResult);
                }
            }
            return values;
        }

        /// <summary>
        /// 写入Json参数
        /// </summary>
        /// <typeparam name="T">序列化类型</typeparam>
        /// <param name="table">参数名称</param>
        /// <param name="t">序列化变量</param>
        /// <returns>写入结果</returns>
        public static bool WriteJson<T>(string table, T t) where T : class
        {
            bool ret = false;
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            DirectoryInfo root = new DirectoryInfo(path + "\\");
            FileInfo[] files = root.GetFiles();
            if (Array.Exists(files, x => x.Name == table + ".json"))
            {
                string destinationFile = path + "\\" + table + ".json";
                string serializedResult = JToken.Parse(JsonConvert.SerializeObject(t)).ToString();
                File.WriteAllText(destinationFile, serializedResult, Encoding.UTF8);
                ret = true;
            }
            else
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 创建基础Base参数
        /// </summary>
        /// <typeparam name="T">序列化类型</typeparam>
        /// <param name="t">参数结构</param>
        public static void Set_NullJson<T>(T t) where T : class
        {
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string destinationFile = path + @"\Base.json";
            string serializedResult = JToken.Parse(JsonConvert.SerializeObject(t)).ToString();
            File.WriteAllText(destinationFile, serializedResult, Encoding.UTF8);
        }

        /// <summary>
        /// 获取系统参数的Value值
        /// </summary>
        /// <typeparam name="T">Value类型</typeparam>
        /// <param name="system">系统参数项</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T GetSystemValue<T>(SystemParameter system)
        {
            switch (system.ValueType.Name)
            {
                case "String":
                    return (T)Convert.ChangeType(system.Value, typeof(T));
                case "Boolean":
                    return (T)Convert.ChangeType(system.Value, typeof(T));
                case "Int32":
                    return (T)Convert.ChangeType(system.Value, typeof(T));
                case "Single":
                    return (T)Convert.ChangeType(system.Value, typeof(T));
                case "Double":
                    return (T)Convert.ChangeType(system.Value, typeof(T));
                default:
                    throw new ArgumentException($"Unsupported type: {system.Name}");
            }
        }

        public static ObservableCollection<T> ConvertOBse<T>(this List<T> source)
        {
            ObservableCollection<T> to = new ObservableCollection<T>(source);
            //或者source.ForEach(p => to.Add(p));
            return to;
        }
    }

}
