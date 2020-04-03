using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services.TableService
{
    public class TableService<T> : ITableService<T> where T : class
    {
        public bool IsSortedAscending { get; set; }
        public string CurrentSortColumn { get; set; }
        public List<T> Objects { get; set; }

        public void SortTable(string columnName)
        {
            //Sorting against a column that is not currently sorted against.
            if (columnName != CurrentSortColumn)
            {
                //We need to force order by ascending on the new column
                //This line uses reflection and will probably perform inefficiently in a production environment.
                Objects = Objects.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                CurrentSortColumn = columnName;
                IsSortedAscending = true;

            }
            else //Sorting against same column but in different direction
            {
                if (IsSortedAscending)
                {
                    Objects = Objects.OrderByDescending(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                }
                else
                {
                    Objects = Objects.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
                }

                //Toggle this boolean
                IsSortedAscending = !IsSortedAscending;
            }
        }
        public string GetSortStyle(string columnName)
        {
            if (CurrentSortColumn != columnName)
            {
                return "oi-sort-ascending";
            }
            if (IsSortedAscending)
            {
                return "oi-caret-bottom";
            }
            else
            {
                return "oi-caret-top";
            }
        }
    }
}
