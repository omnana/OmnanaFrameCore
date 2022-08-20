using System;
using System.Threading.Tasks;

namespace DataTable.Runtime
{
    public static class DataTableHelper
    {
        public static Func<string, string> LoadTextAssetHandle { get; set; }
        public static Task<Func<string, string>> LoadAsyncTextAssetHandle { get; set; }
    }
}