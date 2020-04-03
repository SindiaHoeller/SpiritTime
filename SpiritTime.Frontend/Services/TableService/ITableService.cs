using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services.TableService
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
