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



        public string PredicateToString(Expression<Func<T, bool>> predicate)
        {

            var ForeachSorgu = Types.GetAllTypes(predicate);
            int i = 0;
            string str = " WHERE";
            foreach (var Query in ForeachSorgu.Data)
            {
                if (Query.Type.Type == typeof(DateTime))
                {
                    var date = Convert.ToDateTime(Query.Type.Value);
                    if (Oracle)
                    {
                        if (date.Hour >= 1)
                            Query.Type.Value = "TO_DATE('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY/MM/DD HH:MI:SS')";
                        else
                            Query.Type.Value += "TO_DATE('" + date.ToString("yyyy-MM-dd") + "', 'YYYY/MM/DD')";
                    }
                    else
                    {
                        if (date.Second >= 1)
                            Query.Type.Value = "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        else
                            Query.Type.Value += "'" + date.ToString("yyyy-MM-dd") + "'";
                    }
                }
                switch (Query.EqualType)
                {
                    case "Equal":
                        str += " " + Query.Type.Name + " = " + Query.Type.Value;
                        break;
                    case "NotEqual":
                        str += " " + Query.Type.Name + " != " + Query.Type.Value;
                        break;
                    case "GreaterThan":
                        str += " " + Query.Type.Name + " > " + Query.Type.Value;
                        break;
                    case "GreaterThanOrEqual":
                        str += " " + Query.Type.Name + " >= " + Query.Type.Value;
                        break;
                    case "LessThan":
                        str += " " + Query.Type.Name + " < " + Query.Type.Value;
                        break;
                    case "LessThanOrEqual":
                        str += " " + Query.Type.Name + " <= " + Query.Type.Value;
                        break;
                    case "Contains":
                        str += " " + Query.Type.Name + " LIKE %" + Query.Type.Value + "%";
                        break;
                    case "StartsWith":
                        str += " " + Query.Type.Name + " LIKE " + Query.Type.Value + "%";
                        break;
                    case "EndsWith":
                        str += " " + Query.Type.Name + " LIKE % " + Query.Type.Value;
                        break;
                }
                if (i <= ForeachSorgu.Data.Count - 2)
                {
                    switch (ForeachSorgu.AndOrOr[i])
                    {
                        case "And":
                            str += " AND";
                            break;
                        case "AndAlso":
                            str += " AND";
                            break;
                        case "Or":
                            str += " OR";
                            break;
                        case "OrElse":
                            str += " OR";
                            break;
                    }
                }


                i++;
            }

            return str;
        }

        public COASqlData<T> GenerateDeleteQuery(Expression<Func<T, bool>> where)
        {
            var wherestring = PredicateToString(where);

            string str = $"DELETE FROM {TableName}{wherestring}";
            return new COASqlData<T>() { SqlQuery = str, Lenght = str.Length, TableName = TableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, WhereQuery = wherestring };
        }
        COAGetAllTypes Types = new COAGetAllTypes();
        public COASqlData<T> GenerateSelectQuery(Expression<Func<T, object>> select = null)
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

            return new COASqlData<T>() { SqlQuery = str, Lenght = str.Length, TableName = TableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, SelectedColumns = SelectList };
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

            properties.ForEach(prop => { if (prop != PrimaryKeyName) insertQuery.Append($"@{prop},"); });

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
                    updateQuery.Append($"{property}=@{property},");
                    SelectedList.Add(property);
                }

            });

            updateQuery.Remove(updateQuery.Length - 1, 1);
            var wherestring = PredicateToString(where);
            updateQuery.Append(wherestring);
            return new COASqlUpdateData<T>() { SqlQuery = updateQuery.ToString(), Lenght = updateQuery.ToString().Length, SelectedColumns=SelectedList, TableName = TableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, WhereQuery = wherestring };
        }

        //public string Update(Expression<Func<T, bool>> Where, T Value)
        //{
        //    string str = "";

        //    TypeInfo typee = Value.GetType().GetTypeInfo();

        //    IEnumerable<PropertyInfo> PropertiesList = typee.DeclaredProperties;
        //    str = "UPDATE " + TableName + " SET ";
        //    var lastitem = PropertiesList.LastOrDefault();
        //    var ForeachSorgu = GetAllTypes(Where);
        //    int i = 0;
        //    foreach (var item in PropertiesList)
        //    {
        //        if (item.Name != PrimaryKeyName)
        //        {
        //            var getvalue = item.GetValue(Value, null);

        //            if (item.PropertyType == typeof(string))
        //            {

        //                if (item != lastitem)
        //                    str += " " + item.Name + " = '" + getvalue + "',";
        //                else
        //                    str += " " + item.Name + " = '" + getvalue + "'";


        //            }
        //            else if (item.PropertyType == typeof(DateTime))
        //            {
        //                if (!Oracle)
        //                {
        //                    if (item != lastitem)
        //                        str += " " + item.Name + " = " + Convert.ToDateTime(getvalue).ToString("yyyy-MM-dd HH:mm:ss") + ",";
        //                    else
        //                        str += " " + item.Name + " = " + Convert.ToDateTime(getvalue).ToString("yyyy-MM-dd HH:mm:ss");
        //                }
        //                else
        //                {
        //                    var date = Convert.ToDateTime(getvalue);
        //                    if (date.Hour >= 1)
        //                    {
        //                        if (item != lastitem)
        //                            str += " " + item.Name + " = TO_DATE('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY/MM/DD HH:MI:SS'),";
        //                        else
        //                            str += " " + item.Name + " = TO_DATE('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY/MM/DD HH:MI:SS')";
        //                    }
        //                    else
        //                    {
        //                        if (item != lastitem)
        //                            str += " " + item.Name + " = TO_DATE('" + date.ToString("yyyy-MM-dd") + "', 'YYYY/MM/DD'),";
        //                        else
        //                            str += " " + item.Name + " = TO_DATE('" + date.ToString("yyyy-MM-dd") + "', 'YYYY/MM/DD')";
        //                    }

        //                }

        //            }
        //            else
        //            {
        //                if (item != lastitem)
        //                    str += " " + item.Name + " = " + getvalue + ",";
        //                else
        //                    str += " " + item.Name + " = " + getvalue;
        //            }
        //        }

        //    }
        //    str += " WHERE";
        //    foreach (var Query in ForeachSorgu.Data)
        //    {
        //        if (Query.Type.Type == typeof(DateTime))
        //        {
        //            if (Oracle)
        //            {
        //                var date = Convert.ToDateTime(Query.Type.Value);
        //                if (date.Hour >= 1)
        //                {

        //                    Query.Type.Value = "TO_DATE('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY/MM/DD HH:MI:SS')";
        //                }
        //                else
        //                {
        //                    Query.Type.Value += "TO_DATE('" + date.ToString("yyyy-MM-dd") + "', 'YYYY/MM/DD')";
        //                }
        //            }
        //        }
        //        switch (Query.EqualType)
        //        {
        //            case "Equal":

        //                str += " " + Query.Type.Name + " = " + Query.Type.Value;

        //                break;
        //            case "NotEqual":

        //                str += " " + Query.Type.Name + " != " + Query.Type.Value;

        //                break;
        //            case "GreaterThan":

        //                str += " " + Query.Type.Name + " > " + Query.Type.Value;

        //                break;
        //            case "GreaterThanOrEqual":

        //                str += " " + Query.Type.Name + " >= " + Query.Type.Value;

        //                break;
        //            case "LessThan":

        //                str += " " + Query.Type.Name + " < " + Query.Type.Value;

        //                break;
        //            case "LessThanOrEqual":

        //                str += " " + Query.Type.Name + " <= " + Query.Type.Value;

        //                break;
        //            case "Contains":
        //                str += " " + Query.Type.Name + " LIKE %" + Query.Type.Value + "%";
        //                break;
        //            case "StartsWith":
        //                str += " " + Query.Type.Name + " LIKE " + Query.Type.Value + "%";
        //                break;
        //            case "EndsWith":
        //                str += " " + Query.Type.Name + " LIKE % " + Query.Type.Value;
        //                break;
        //        }
        //        if (i <= ForeachSorgu.Data.Count - 2 && ForeachSorgu.AndOrOr.Count > 0)
        //        {
        //            switch (ForeachSorgu.AndOrOr[i])
        //            {
        //                case "And":
        //                    str += " AND";
        //                    break;
        //                case "AndAlso":
        //                    str += " AND";
        //                    break;
        //                case "Or":
        //                    str += " OR";
        //                    break;
        //                case "OrElse":
        //                    str += " OR";
        //                    break;
        //            }
        //        }


        //        i++;
        //    }
        //    return str;
        //}

    }
    public static class Extensionn
    {
        static COAGetAllTypes Types = new COAGetAllTypes();
        public static COASqlData<T> OrderBy<T>(this COASqlData<T> data, Expression<Func<T, object>> select)
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
        public static COASqlData<T> OrderByDescing<T>(this COASqlData<T> data, Expression<Func<T, object>> select)
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
        public static COASqlData<T> Where<T>(this COASqlData<T> data, Expression<Func<T, bool>> Where)
        {
            var ForeachSorgu = Types.GetAllTypes(Where);
            int i = 0;
            string str = " WHERE";
            foreach (var Query in ForeachSorgu.Data)
            {
                if (Query.Type.Type == typeof(DateTime))
                {
                    var date = Convert.ToDateTime(Query.Type.Value);
                    if (data.Oracle)
                    {
                        if (date.Hour >= 1)
                            Query.Type.Value = "TO_DATE('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY/MM/DD HH:MI:SS')";
                        else
                            Query.Type.Value += "TO_DATE('" + date.ToString("yyyy-MM-dd") + "', 'YYYY/MM/DD')";
                    }
                    else
                    {
                        if (date.Second >= 1)
                            Query.Type.Value = "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        else
                            Query.Type.Value += "'" + date.ToString("yyyy-MM-dd") + "'";
                    }

                }
                switch (Query.EqualType)
                {
                    case "Equal":
                        str += " " + Query.Type.Name + " = " + Query.Type.Value;
                        break;
                    case "NotEqual":
                        str += " " + Query.Type.Name + " != " + Query.Type.Value;
                        break;
                    case "GreaterThan":
                        str += " " + Query.Type.Name + " > " + Query.Type.Value;
                        break;
                    case "GreaterThanOrEqual":
                        str += " " + Query.Type.Name + " >= " + Query.Type.Value;
                        break;
                    case "LessThan":
                        str += " " + Query.Type.Name + " < " + Query.Type.Value;
                        break;
                    case "LessThanOrEqual":
                        str += " " + Query.Type.Name + " <= " + Query.Type.Value;
                        break;
                    case "Contains":
                        str += " " + Query.Type.Name + " LIKE %" + Query.Type.Value + "%";
                        break;
                    case "StartsWith":
                        str += " " + Query.Type.Name + " LIKE " + Query.Type.Value + "%";
                        break;
                    case "EndsWith":
                        str += " " + Query.Type.Name + " LIKE % " + Query.Type.Value;
                        break;
                }
                if (i <= ForeachSorgu.Data.Count - 2)
                {
                    switch (ForeachSorgu.AndOrOr[i])
                    {
                        case "And":
                            str += " AND";
                            break;
                        case "AndAlso":
                            str += " AND";
                            break;
                        case "Or":
                            str += " OR";
                            break;
                        case "OrElse":
                            str += " OR";
                            break;
                    }
                }


                i++;
            }


            data.SqlQuery += str;
            data.WhereQuery = str;
            data.Lenght = data.SqlQuery.Length;
            return data;
        }
        public static COASqlData<T> Skip<T>(this COASqlData<T> data, int skip, int take = 0)
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
            var str = $"with dummyTable as (select ROW_NUMBER() over({Ordrquery}) as RowNumber,* from {data.TableName}{data.WhereQuery}) select top({skip}) {SelectedColumns} from dummyTable";
            if (take > 0)
                str += $" WHERE RowNumber > ({take})";

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
                     updateQuery.Append($"{colums}=@{colums},");
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
                Columns =new List<string>(data.SelectedColumns);
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
                            updateQuery.Append($"{colums}=@{colums},");
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
