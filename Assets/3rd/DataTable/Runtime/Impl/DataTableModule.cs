using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataTable.Runtime
{
    public class DataTableModule<T, TM> : IDataTableModule<T> where T : BaseDataTable where TM : ITableParser<T>
    {
        private Dictionary<object, T> _tableDic;
        public List<T> TableList { get; set; }

        private TM _tableParser;

        public void Load()
        {
            _tableDic = new Dictionary<object, T>();
            _tableParser = Activator.CreateInstance<TM>();
            if (DataTableHelper.LoadTextAssetHandle == null)
            {
                Debug.LogError($"请先注册DataTableHelper.LoadTextAssetHandle事件");
                return;
            }

            var csv = DataTableHelper.LoadTextAssetHandle?.Invoke(_tableParser.TableName());
            TableList = ParserUtil.Parser(_tableParser, csv);
            foreach (var data in TableList)
            {
                var id = data.GetId();
                _tableDic.Add(id, data);
            }
        }

        public T GetTableById(int id)
        {
            return _tableDic.ContainsKey(id) ? _tableDic[id] : default;
        }

        public T GetTableByIndex(int index)
        {
            if (TableList.Count <= index && index < 0)
            {
                return default;
            }

            return TableList[index];
        }

        public T First(Func<T, bool> func)
        {
            if (TableList == null)
                return default;

            foreach (var t in TableList)
            {
                if (func(t))
                {
                    return t;
                }
            }

            return default;
        }

        public List<T> GetTableList(Func<T, bool> func)
        {
            if (TableList == null)
                return null;

            var list = new List<T>();
            foreach (var t in TableList)
            {
                if (func(t))
                {
                    list.Add(t);
                }
            }

            return list;
        }
    }
}
