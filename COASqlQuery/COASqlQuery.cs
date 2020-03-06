using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace COASqlQuery
{
    public partial class COASqlQuery<T>
    {
        public bool Oracle { get; set; } = false;

        private string TableName { get; set; }
        //Defualt Val Row_ID
        public string PrimaryKeyName { get; set; } = "Row_ID";

        public COASqlQuery(string CustomTableName = null)
        {
            if (string.IsNullOrEmpty(CustomTableName))
                TableName = typeof(T).Name;
            else
                TableName = CustomTableName;
        }
        protected IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

        public string CreateTable()
        {
            var CreataTableQuery = new StringBuilder($"CREATE TABLE {TableName} (");
            var properties = Types.GenerateListOfProperties(GetProperties);
            properties.ForEach(prop => { if (prop != PrimaryKeyName) CreataTableQuery.Append($"{prop} NCHAR(50),"); });
            CreataTableQuery
                .Remove(CreataTableQuery.Length - 1, 1)
                .Append(");");
            return CreataTableQuery.ToString();
        }



        public COASqlData<T> GenerateDeleteQuery(Expression<Func<T, bool>> where)
        {
            var wherestring = Types.PredicateToString(where, Oracle);

            string str = $"DELETE FROM {TableName}{wherestring}";
            return new COASqlData<T>() { SqlQuery = str, Lenght = str.Length, TableName = TableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, WhereQuery = wherestring };
        }
        COAGetAllTypes Types = new COAGetAllTypes();
        public COASqlSelectData<T> GenerateSelectQuery(Expression<Func<T, object>> select = null)
        {

            var SelectList = new List<string>();
            string selectStr = "";
            if (select != null)
            {
                var ForeachSorgu = Types.GetAllTypes(select);
                ForeachSorgu.Ad.ForEach(row =>
                {
                    selectStr += row + ",";
                    SelectList.Add(row);
                });
                selectStr = selectStr.Remove(selectStr.Length - 1, 1);

            }
            else
                selectStr = "*";
            var str = $"SELECT {selectStr} FROM {TableName}";

            return new COASqlSelectData<T>() { SqlQuery = str, Lenght = str.Length, TableName = TableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, SelectedColumns = SelectList };
        }

        public COASqlData<T> GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {TableName} ");
            insertQuery.Append("(");
            var properties = Types.GenerateListOfProperties(GetProperties);
            properties.ForEach(prop => { if (prop != PrimaryKeyName) insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop =>
            {
                if (prop != PrimaryKeyName)
                {
                    if (!Oracle)
                        insertQuery.Append($"@{prop},");
                    else
                        insertQuery.Append($":{prop},");
                }

            });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return new COASqlData<T>() { SqlQuery = insertQuery.ToString(), Lenght = insertQuery.ToString().Length, TableName = TableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName };
        }
        public COASqlUpdateData<T> GenerateUpdateQuery(Expression<Func<T, bool>> where)
        {
            var SelectedList = new List<string>();
            var updateQuery = new StringBuilder($"UPDATE {TableName} SET ");
            var properties = Types.GenerateListOfProperties(GetProperties);

            properties.ForEach(property =>
            {
                if (!property.Equals(PrimaryKeyName))
                {
                    if (!Oracle)
                        updateQuery.Append($"{property}=@{property},");
                    else
                        updateQuery.Append($"{property}=:{property},");
                    SelectedList.Add(property);
                }

            });

            updateQuery.Remove(updateQuery.Length - 1, 1);
            var wherestring = Types.PredicateToString(where, Oracle);
            updateQuery.Append(wherestring);
            return new COASqlUpdateData<T>() { SqlQuery = updateQuery.ToString(), Lenght = updateQuery.ToString().Length, SelectedColumns = SelectedList, TableName = TableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, WhereQuery = wherestring };
        }

        
    }
    public static class Extensionn
    {
        static COAGetAllTypes Types = new COAGetAllTypes();
        public static COASqlSelectData<T> OrderBy<T>(this COASqlSelectData<T> data, Expression<Func<T, object>> select)
        {
            string selectStr = " ORDER BY ";
            var ForeachSorgu = Types.GetAllTypes(select);
            ForeachSorgu.Ad.ForEach(row => { selectStr += row + ","; });
            selectStr = selectStr.Remove(selectStr.Length - 1, 1);
            data.SqlQuery += selectStr + " ASC";
            data.Lenght = data.SqlQuery.Length;
            data.OrderQuery = selectStr + " ASC";
            return data;
        }
        public static COASqlSelectData<T> OrderByDescing<T>(this COASqlSelectData<T> data, Expression<Func<T, object>> select)
        {
            string selectStr = " ORDER BY ";
            var ForeachSorgu = Types.GetAllTypes(select);
            ForeachSorgu.Ad.ForEach(row => { selectStr += row + ","; });
            selectStr = selectStr.Remove(selectStr.Length - 1, 1);
            data.SqlQuery += selectStr + " DESC";
            data.Lenght = data.SqlQuery.Length;
            data.OrderQuery = selectStr + " DESC";
            return data;
        }
        public static COASqlSelectData<T> Where<T>(this COASqlSelectData<T> data, Expression<Func<T, bool>> Where)
        {
            var str = Types.PredicateToString(Where, data.Oracle);
            data.SqlQuery += str;
            data.WhereQuery = str;
            data.Lenght = data.SqlQuery.Length;
            return data;
        }
        public static COASqlSelectData<T> Skip<T>(this COASqlSelectData<T> data, int skip, int take)
        {
            string SelectedColumns = "";
            if (data.SelectedColumns.Count == 0 || data.SelectedColumns == null)
            {

                var properties = Types.GenerateListOfProperties(typeof(T).GetProperties());
                properties.ForEach(props => { SelectedColumns += props + ","; data.SelectedColumns.Add(props); });
            }
            else
            {
                data.SelectedColumns.ForEach(colums => { SelectedColumns += colums + ","; });
            }
            SelectedColumns = SelectedColumns.Remove(SelectedColumns.Length - 1, 1);
            var Ordrquery = "";

            if (string.IsNullOrEmpty(data.OrderQuery))
                Ordrquery = $"order by {data.PrimaryKeyName} asc";
            else
                Ordrquery = data.OrderQuery;
            var str = "";

            if (!data.Oracle)
            {
                str = $"with dummyTable as (select ROW_NUMBER() over({Ordrquery}) as RowNumber,* from {data.TableName}{data.WhereQuery}) select top({take}) {SelectedColumns} from dummyTable";
                if (take > 0)
                    str += $" WHERE RowNumber > ({skip})";
            }
            else
            {
                str = $"select {SelectedColumns} from(select a.*, rownum rnum from(select * from {data.TableName}{data.WhereQuery} {Ordrquery}) a where rownum <= {take+skip})where rnum > {skip}";
            }
           

            data.SqlQuery = str;
            data.Lenght = data.SqlQuery.Length;
            data.OrderQuery = Ordrquery;
            return data;
        }

        //UpdateExtensions
        public static COASqlUpdateData<T> SelectColums<T>(this COASqlUpdateData<T> data, Expression<Func<T, object>> select)
        {
            var SelectedColums = Types.GetAllTypes(select);
            var updateQuery = new StringBuilder($"UPDATE {data.TableName} SET ");
            data.SelectedColumns.Clear();
            SelectedColums.Ad.ForEach(colums =>
             {

                 if (!colums.Equals(data.PrimaryKeyName))
                 {
                     if (!data.Oracle)
                         updateQuery.Append($"{colums}=@{colums},");
                     else
                         updateQuery.Append($"{colums}=:{colums},");
                     data.SelectedColumns.Add(colums);
                 }

             });

            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(data.WhereQuery);
            data.SqlQuery = updateQuery.ToString();
            return data;
        }
        public static COASqlUpdateData<T> RemoveColums<T>(this COASqlUpdateData<T> data, Expression<Func<T, object>> select)
        {
            List<string> Columns = new List<string>();
            var SelectedColums = Types.GetAllTypes(select);
            if (data.SelectedColumns == null || data.SelectedColumns.Count == 0)
                Columns = Types.GenerateListOfProperties(typeof(T).GetProperties());
            else
                Columns = new List<string>(data.SelectedColumns);
            var updateQuery = new StringBuilder($"UPDATE {data.TableName} SET ");
            data.SelectedColumns.Clear();
            Columns.ForEach(colums =>
            {

                if (!colums.Equals(data.PrimaryKeyName))
                {
                    SelectedColums.Ad.ForEach(selcomuns =>
                    {
                        if (!colums.Equals(selcomuns))
                        {
                            if (!data.Oracle)
                                updateQuery.Append($"{colums}=@{colums},");
                            else
                                updateQuery.Append($"{colums}=:{colums},");
                            data.SelectedColumns.Add(colums);
                        }

                    });

                }

            });
            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(data.WhereQuery);
            data.SqlQuery = updateQuery.ToString();

            return data;
        }
    }

}
