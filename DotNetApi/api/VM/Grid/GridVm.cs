using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace VM.Grid
{
    public static class GridVm
    {
        public static string FormatResult2(IList list, long numberOfRows, int pageIndex, long totalRecords)
        {
            var result = new GridResults();
            var rows = new List<GridRow>();
            foreach (dynamic entry in list)
            {
                var row = new GridRow
                              {
                                  id = entry.GetType().GetProperties()[0].GetValue(entry, null),
                                  cell = new string[entry.GetType().GetProperties().Length]
                              };

                for (var i = 0; i < row.cell.Count(); i++)
                {
                    row.cell[i] = entry.GetType().GetProperties()[i].GetValue(entry, null).ToString();
                }
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            result.page = pageIndex;
            if (numberOfRows != 0)
            {
                result.total = (totalRecords + numberOfRows - 1) / numberOfRows;
            }
            else
            {
                result.total = 1;
            }
            result.records = totalRecords;
            return new JavaScriptSerializer().Serialize(result);
        }

        public static GridResults FormatResult(IList list, int numberOfRows, int pageIndex, long totalRecords)
        {
            var result = new GridResults();
            var rows = new List<GridRow>();
            foreach (dynamic entry in list)
            {
                var row = new GridRow
                              {
                                  id = entry.GetType().GetProperties()[0].GetValue(entry, null),
                                  cell = new string[entry.GetType().GetProperties().Length]
                              };

                for (var i = 0; i < row.cell.Count(); i++)
                {
                    row.cell[i] = entry.GetType().GetProperties()[i].GetValue(entry, null).ToString();
                }
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            result.page = pageIndex;
            if (numberOfRows != 0)
            {
                result.total = (totalRecords + numberOfRows - 1) / numberOfRows;
            }
            else
            {
                result.total = 1;
            }
            result.records = totalRecords;
            return result;
        }
    }
    public struct GridResults
    {
        public int page;
        public long total;
        public long records;
        public GridRow[] rows;
    }
    public struct GridRow
    {
        public long id;
        public string[] cell;
    }
}