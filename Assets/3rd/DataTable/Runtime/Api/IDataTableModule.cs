using System;
using System.Collections.Generic;

namespace DataTable.Runtime
{
    public interface IDataTableModule<T> where T : BaseDataTable
    {
        void Load();
        List<T> TableList { get; set; }
        T GetTableById(int id);
        T GetTableByIndex(int index);
        T First(Func<T, bool> func);
        List<T> GetTableList(Func<T, bool> func);
    }
}
