using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Directory = UnityEngine.Windows.Directory;

namespace TableTool.Editor
{
    public enum CellValueType
    {
        [Description("int")] Int,
        [Description("float")] Float,
        [Description("bool")] Bool,
        [Description("string")] String,
        [Description("long")] Long,
        [Description("intArray")] IntArray,
        [Description("floatArray")] FloatArray,
        [Description("boolArray")] BoolArray,
        [Description("stringArray")] StringArray,
        [Description("longArray")] LongArray,
    }
    
    public class DataTableHeaderItem
    {
        public string name;
        public string type;
        public string des;
    }

    public class DataTableUtil
    {
        public static List<string> GetCsvFiles(string csvFolder)
        {
            var list = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(csvFolder);
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                if(fileInfo.Name.Equals(".meta") || !fileInfo.Name.EndsWith(".csv")) 
                    continue;
                list.Add(fileInfo.FullName);
            }

            return list;
        }
    }
}