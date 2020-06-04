using System.Collections.Generic;

namespace SpiritTime.Frontend.Services.TableServices
{
    public interface ITableService<T> where T : class
    {
        bool IsSortedAscending { get; set; }
        string CurrentSortColumn { get; set; }
        List<T> Objects { get; set; }

        void SortTable(string columnName);
        string GetSortStyle(string columnName);
    }
}
