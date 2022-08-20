using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Csv;
using TableTool.Editor;
using UnityEditor;
using UnityEngine;

namespace DataTable.Editor
{
    public class DataTableWriter
    {
        private static readonly StringBuilder Sb = new StringBuilder();

        public static void WriteTable(string csvFilePath, string destFolder)
        {
            try
            {
                var tableName = Path.GetFileNameWithoutExtension(csvFilePath);
                var csv = File.ReadAllText(csvFilePath);
                if (!string.IsNullOrEmpty(csv))
                {
                    var lines = CsvReader.ReadFromText(csv).ToList();
                    var names = lines[0];
                    var types = lines[1];
                    var headers = names.Headers;
                    if (!names[0].Equals("Id"))
                    {
                        Debug.LogError("第一列必须是Id列，去改表！！！！！！！");
                        return;
                    }

                    var list = new List<DataTableHeaderItem>();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var header = headers[i];
                        if (header.StartsWith("#"))
                            continue;

                        list.Add(new DataTableHeaderItem()
                        {
                            name = names[i],
                            type = types[i],
                            des = headers[i],
                        });
                    }

                    var tableStr = WriteTableScript(tableName, list);
                    var parserStr = WriteTableParserScript(tableName, list);
                    var tableFolder = destFolder + "/Table";
                    var parserFolder = destFolder + "/Parser";
                    var loaderStr = WriteDataTableBuild(tableFolder);
                    if (!Directory.Exists(tableFolder))
                    {
                        Directory.CreateDirectory(tableFolder);
                    }

                    if (!Directory.Exists(parserFolder))
                    {
                        Directory.CreateDirectory(parserFolder);
                    }

                    File.WriteAllText($"{tableFolder}/{tableName}DataTable.cs", tableStr, Encoding.UTF8);
                    File.WriteAllText($"{parserFolder}/{tableName}DataTable.Parser.cs", parserStr, Encoding.UTF8);
                    File.WriteAllText($"{destFolder}/DataTableBuilder.cs", loaderStr, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string WriteTableScript(string tableName, List<DataTableHeaderItem> list)
        {
            Sb.Clear();
            Sb.AppendLine("using DataTable.Runtime;");
            Sb.AppendLine("");
            Sb.AppendLine("/// <summary>");
            Sb.AppendLine("/// 禁止手动修改脚本");
            Sb.AppendLine("/// </summary>");
            Sb.AppendLine($"public class {tableName}DataTable : BaseDataTable");
            Sb.AppendLine("{");
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                switch (item.type)
                {
                    case "int":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public int {item.name};");
                        break;
                    case "float":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public float {item.name};");
                        break;
                    case "bool":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public bool {item.name};");
                        break;
                    case "long":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public long {item.name};");
                        break;
                    case "string":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public string {item.name};");
                        break;
                    case "intArray":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public int[] {item.name};");
                        break;
                    case "floatArray":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public float[] {item.name};");
                        break;
                    case "boolArray":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public bool[] {item.name};");
                        break;
                    case "longArray":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public long[] {item.name};");
                        break;
                    case "stringArray":
                        Sb.AppendLine($"    /// <summary> {item.des} </summary>");
                        Sb.AppendLine($"    public string[] {item.name};");
                        break;
                    default:
                        Debug.LogError($"表：{tableName}.csv；未知类型：{item.type}，请检查配置第3行！！！！！！！");
                        return string.Empty;
                }
            }

            Sb.AppendLine("    public override object GetId() => Id;");
            Sb.AppendLine("}");
            return Sb.ToString();
        }

        private static string WriteTableParserScript(string tableName, List<DataTableHeaderItem> list)
        {
            Sb.Clear();
            Sb.AppendLine("using System.Collections.Generic;");
            Sb.AppendLine("using DataTable.Runtime;");
            Sb.AppendLine("");
            Sb.AppendLine("/// <summary>");
            Sb.AppendLine("/// 禁止手动修改脚本");
            Sb.AppendLine("/// </summary>");
            Sb.AppendLine($"public class {tableName}DataTableParser : ITableParser<{tableName}DataTable>");
            Sb.AppendLine("{");
            Sb.AppendLine($"    public string TableName() => \"{tableName}\";");
            Sb.AppendLine(
                $"    public void Parse({tableName}DataTable table, Dictionary<string, string> filedMap)");
            Sb.AppendLine("    {");
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                switch (item.type)
                {
                    case "int":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToInt(filedMap[\"{item.name}\"]);");
                        break;
                    case "float":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToFloat(filedMap[\"{item.name}\"]);");
                        break;
                    case "bool":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToBool(filedMap[\"{item.name}\"]);");
                        break;
                    case "long":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToLong(filedMap[\"{item.name}\"]);");
                        break;
                    case "string":
                        Sb.AppendLine($"        table.{item.name} = filedMap[\"{item.name}\"];");
                        break;
                    case "intArray":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToInts(filedMap[\"{item.name}\"]);");
                        break;
                    case "floatArray":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToFloats(filedMap[\"{item.name}\"]);");
                        break;
                    case "boolArray":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToBools(filedMap[\"{item.name}\"]);");
                        break;
                    case "longArray":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToLongs(filedMap[\"{item.name}\"]);");
                        break;
                    case "stringArray":
                        Sb.AppendLine($"        table.{item.name} = ParserUtil.ToStrings(filedMap[\"{item.name}\"]);");
                        break;
                    default:
                        Debug.LogError($"表：{tableName}.csv；未知类型：{item.type}，请检查配置第3行！！！！！！！");
                        return string.Empty;
                }
            }

            Sb.AppendLine("    }");
            Sb.AppendLine("}");
            return Sb.ToString();
        }

        static string WriteDataTableBuild(string tableFolder)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(tableFolder);
            var list= new List<string>();
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                if (fileInfo.Name.EndsWith(".cs"))
                {
                    var key = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    list.Add(key);
                }
            }
            Sb.Clear();
            Sb.AppendLine("using System;");
            Sb.AppendLine("using Container.Impl;");
            Sb.AppendLine("using DataTable.Runtime;");
            Sb.AppendLine("");
            Sb.AppendLine("// Don't editor it");
            Sb.AppendLine("public class DataTableBuilder");
            Sb.AppendLine("{");
            Sb.AppendLine("    public static void Build(Func<string, string> loadTextHandle)");
            Sb.AppendLine("    {");
            Sb.AppendLine("        DataTableHelper.LoadTextAssetHandle = loadTextHandle;");
            foreach (var key in list)
            {
                Sb.AppendLine($"        IocContainer.Bind(typeof(IDataTableModule<{key}>)).To(typeof(DataTableModule<{key}, {key}Parser>));");
                Sb.AppendLine($"        IocContainer.Resolve<IDataTableModule<{key}>>().Load();");
            }
            Sb.AppendLine("    }");
            Sb.AppendLine("}");

            return Sb.ToString();
        }
    }
}
