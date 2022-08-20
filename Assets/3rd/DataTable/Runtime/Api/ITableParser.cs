using System.Collections.Generic;

namespace DataTable.Runtime
{
    public interface ITableParser<T> where T : BaseDataTable
    {
        string TableName();
        void Parse(T dataTable, Dictionary<string, string> filedMap);
    }
}