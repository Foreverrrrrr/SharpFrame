using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpFrame.Common
{
    public class ExcelTool
    {
        public struct PropertyStructure
        {
            public string TyName { get; set; }
            public object TyValue { get; set; }
            public Type TypeVert { get; set; }
        }

        public static void WriteExcel<T>(string path, T testdata, int automatic_deletion = 180) where T : class
        {
            if (automatic_deletion > 0)
                CleanFile(path, automatic_deletion);
            if (testdata != null)
            {
                string excelsavepath = path + "\\" + DateTime.Now.ToString("yyyy-MM");
                if (!Directory.Exists(excelsavepath))
                    Directory.CreateDirectory(excelsavepath);
                Type datatype = typeof(T);
                PropertyInfo[] properties = datatype.GetProperties();
                System.Collections.Generic.List<PropertyStructure> structures = new System.Collections.Generic.List<PropertyStructure>();
                foreach (PropertyInfo property in properties)
                {
                    DescriptionAttribute descriptionAttribute = (DescriptionAttribute)property.GetCustomAttribute(typeof(DescriptionAttribute));
                    PropertyStructure propertyStructure = new PropertyStructure();
                    propertyStructure.TyName = descriptionAttribute?.Description ?? property.Name;
                    propertyStructure.TyValue = property.GetValue(testdata);
                    propertyStructure.TypeVert = property.PropertyType;
                    structures.Add(propertyStructure);
                }
                string processName = "EXCEL";
                Process[] processes = Process.GetProcessesByName(processName);
                foreach (Process process in processes)
                {
                    //string vat = process.MainWindowTitle.Replace("- Excel", "");
                    //if (vat == DateTime.Now.ToString("dd") + ".xlsx")
                    process.Kill();
                }
                using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(excelsavepath + "\\" + DateTime.Now.ToString("dd") + ".xlsx")))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Count != 0 ? package.Workbook.Worksheets[1] : package.Workbook.Worksheets.Add("Form1");//是否存在工作表，不存在创建工作表
                    int rowCount = worksheet.Dimension != null ? worksheet.Dimension.End.Row : 0;//获取数据最后行
                    if (rowCount == 0)
                    {
                        for (int i = 0; i < structures.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = structures[i].TyName;
                            worksheet.Cells[1, i + 1].Style.Font.Name = "微软雅黑";
                        }
                        rowCount++;
                    }
                    for (int i = 0; i < structures.Count; i++)
                    {
                        if (structures[i].TypeVert == typeof(int))
                            worksheet.Cells[rowCount + 1, i + 1].Value = Convert.ToInt32(structures[i].TyValue);
                        else if (structures[i].TypeVert == typeof(double))
                            worksheet.Cells[rowCount + 1, i + 1].Value = Convert.ToDouble(structures[i].TyValue);
                        else if (structures[i].TypeVert == typeof(float))
                            worksheet.Cells[rowCount + 1, i + 1].Value = Convert.ToSingle(structures[i].TyValue);
                        else
                            worksheet.Cells[rowCount + 1, i + 1].Value = structures[i].TyValue.ToString();
                    }
                    package.SaveAs(new System.IO.FileInfo(excelsavepath + "\\" + DateTime.Now.ToString("dd") + ".xlsx"));
                }
            }
        }

        public static void WriteExcel<T>(string path, string excelname, ObservableCollection<T> data) where T : class
        {
            if (data != null && data.Count > 0)
            {
                if (Directory.Exists(path))
                {
                    string processName = "EXCEL";
                    Process[] processes = Process.GetProcessesByName(processName);
                    foreach (Process process in processes)
                    {
                        process.Kill();
                    }
                    using (ExcelPackage package = new ExcelPackage(new FileInfo(Path.Combine(path, excelname + ".xlsx"))))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Count != 0 ? package.Workbook.Worksheets[1] : package.Workbook.Worksheets.Add("Sheet1");

                        int rowCount = worksheet.Dimension != null ? worksheet.Dimension.End.Row : 0;

                        if (rowCount == 0)
                        {
                            Type dataType = typeof(T);
                            PropertyInfo[] properties = dataType.GetProperties();

                            for (int i = 0; i < properties.Length; i++)
                            {
                                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)properties[i].GetCustomAttribute(typeof(DescriptionAttribute));
                                string propertyName = descriptionAttribute?.Description ?? properties[i].Name;
                                worksheet.Cells[1, i + 1].Value = propertyName;
                                worksheet.Cells[1, i + 1].Style.Font.Name = "微软雅黑";
                            }

                            rowCount++;
                        }

                        foreach (T item in data)
                        {
                            Type dataType = typeof(T);
                            PropertyInfo[] properties = dataType.GetProperties();

                            for (int i = 0; i < properties.Length; i++)
                            {
                                worksheet.Cells[rowCount + 1, i + 1].Value = properties[i].GetValue(item)?.ToString();
                            }

                            rowCount++;
                        }

                        package.Save();
                    }
                }
            }
        }

        /// <summary>
        /// 文件定时删除
        /// </summary>
        /// <param name="path"></param>
        public static void CleanFile(string path, int time, bool boolmes = false)
        {
            string pathcl = path;
            DirectoryInfo dir = new DirectoryInfo(pathcl);
            if (boolmes)
            {
                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-time))
                        file.Delete();
                }
            }
            else
            {
                var files = dir.GetDirectories();
                foreach (var file in files)
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-time))
                        file.Delete(true);
                }
            }
        }
    }
}

