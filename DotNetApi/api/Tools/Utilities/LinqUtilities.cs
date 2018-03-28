using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace Tools.Utilities
{
    public static class LinqUtilities
    {
        public static DataTable ConvertTo<T>(List<T> lst)
        {
            //CREATE DATATABLE STRUCTURE
            var tbl = CreateTable(lst);
            //var entType = typeof(T);
            //var properties = TypeDescriptor.GetProperties(entType);

            //GET THE LIST ITEM AND ADD INTO THE LIST
            //foreach (var item in lst)
            //{
            //    var row = tbl.NewRow();
            //    foreach (PropertyDescriptor prop in properties)
            //    {
            //        row[prop.Name] = prop.GetValue(item);
            //    }
            //    tbl.Rows.Add(row);
            //}

            return tbl;
        }

        public static DataTable CreateTable<T>(List<T> lst)
        {
            //var entType = typeof(T);
            ////SET THE DATATABLE NAME AS CLASS NAME
            //var tbl = new DataTable();
            ////GET THE PROPERTY LIST
            //var properties = TypeDescriptor.GetProperties(entType);
            //foreach (PropertyDescriptor prop in properties)
            //{
            //    //ADD PROPERTY AS COLUMN
            //    var columnType = prop.PropertyType.Name == "Nullable`1" ? typeof(String) : prop.PropertyType;
            //    tbl.Columns.Add(prop.Name, columnType);
            //}
            var infos = lst.First().GetType().GetProperties();
            var table = new DataTable("Export");
            foreach (var info in infos)
            {
                var column = new DataColumn { Caption = info.Name, ColumnName = info.Name };
                if (info.PropertyType.Name == "Nullable`1")
                {
                    column.DataType = info.PropertyType.GetGenericArguments()[0];
                    column.AllowDBNull = true;
                }
                else
                {
                    column.DataType = info.PropertyType;
                }
                table.Columns.Add(column);
            }

            foreach (var record in lst)
            {
                var row = table.NewRow();
                for (var i = 0; i < table.Columns.Count; i++)
                    row[i] = infos[i].GetValue(record, null)==null? DBNull.Value: infos[i].GetValue(record, null);

                table.Rows.Add(row);
            }

            return table;
        }

        public static void PropertyCopy(ref object source, ref object target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();
            var propMap = GetMatchingProperties(sourceType, targetType);
            for (var i = 0; i <= propMap.Count - 1; i++)
            {
                var prop = propMap[i];
                var sourceValue = prop.SourceProperty.GetValue(source, null);
                if (!((ColumnAttribute)prop.SourceProperty.GetCustomAttributes(true)[0]).IsPrimaryKey)
                {
                    prop.TargetProperty.SetValue(target, sourceValue, null);
                }
            }
        }

        private static List<PropertyMap> GetMatchingProperties(Type sourceType, Type targetType)
        {
            var objName = sourceType.Name;
            var sourceProperties = sourceType.GetProperties();
            var targetProperties = targetType.GetProperties();

            var props = (from s in sourceProperties
                         from t in targetProperties
                         where s.Name == t.Name &&
                               s.CanRead &&
                               t.CanWrite &&
                               s.PropertyType.IsPublic &&
                               t.PropertyType.IsPublic &&
                               s.PropertyType.Name != objName
                         select new PropertyMap(s, t)).ToList();

            return props;
        }

        public static DataSet ToDataSet<T>(List<T> items)
        {
            var ds = new DataSet();
            ds.Tables.Add(ToDataTable(items));
            return ds;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}

public class PropertyMap
{
    private PropertyInfo _sourceProperty;
    private PropertyInfo _targetProperty;

    public PropertyInfo SourceProperty { get; set; }
    public PropertyInfo TargetProperty { get; set; }

    public PropertyMap(PropertyInfo sourceProperty, PropertyInfo targetProperty)
    {
        _sourceProperty = sourceProperty;
        _targetProperty = targetProperty;
    }
}
