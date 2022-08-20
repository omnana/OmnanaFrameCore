using System;
using System.Collections.Generic;
using Csv;

namespace DataTable.Runtime
{
    public static class ParserUtil
    {
        public static int ToInt(string str)
        {
            return int.Parse(str);
        }
        
        public static float ToFloat(string str)
        {
            return float.Parse(str);
        }
        
        public static bool ToBool(string str)
        {
            return int.Parse(str) == 1;
        }
        
        public static long ToLong(string str)
        {
            return long.Parse(str);
        }

        public static string[] ToStrings(string str)
        {
            var array = str.Split('|');
            var result = new string[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                result[index] = array[index];
            }

            return result;
        }

        public static int[] ToInts(string str)
        {
            var array = str.Split('|');
            var result = new int[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                result[index] = int.Parse(array[index]);
            }

            return result;
        }
        
        public static float[] ToFloats(string str)
        {
            var array = str.Split('|');
            var result = new float[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                result[index] = float.Parse(array[index]);
            }

            return result;
        }
        
        public static bool[] ToBools(string str)
        {
            var array = str.Split('|');
            var result = new bool[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                result[index] = bool.Parse(array[index]);
            }

            return result;
        }
        
        public static long[] ToLongs(string str)
        {
            var array = str.Split('|');
            var result = new long[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                result[index] = long.Parse(array[index]);
            }

            return result;
        }

        private static readonly Dictionary<string, string> FieldMap = new Dictionary<string, string>();
        private static readonly List<string> FieldNames = new List<string>();

        public static List<T> Parser<T>(ITableParser<T> parser, string csv) where T : BaseDataTable
        {
            var result = new List<T>();
            var idx = 0;
            FieldNames.Clear();
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                if (idx == 0) // 变量名
                {
                    foreach (var t in line.Values)
                    {
                        FieldNames.Add(t);
                    }
                }
                else if (idx >= 2)
                {
                    FieldMap.Clear();
                    var table = Activator.CreateInstance<T>();
                    for (int i = 0, cnt = line.Values.Length; i < cnt; i++)
                    {
                        var field = FieldNames[i];
                        FieldMap.Add(field, line.Values[i]);
                    }

                    parser.Parse(table, FieldMap);
                    result.Add(table);
                }

                idx++;
            }

            return result;
        }
    }
}
