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

        public string TableName { get { return tableName; } }

        private string tableName;
        //Defualt Val Row_ID
        public string PrimaryKeyName { get; set; } = "Row_ID";

        public COASqlQuery(string CustomTableName = null)
        {
            if (string.IsNullOrEmpty(CustomTableName))
                tableName = typeof(T).Name;
            else
                tableName = CustomTableName;
        }
        protected IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

        public string CreateTable()
        {
            var CreataTableQuery = new StringBuilder($"CREATE TABLE {tableName} (");
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

            string str = $"DELETE FROM {tableName}{wherestring}";
            return new COASqlData<T>() { SqlQuery = str, Lenght = str.Length, TableName = tableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, WhereQuery = wherestring };
        }
        COAGetAllTypes Types = new COAGetAllTypes();
        public COASqlSelectData<T> GenerateSelectQuery(Expression<Func<T, object>> select = null)
        {

            var SelectList = new List<string>();
            string selectStr = "";
            if (select != null)
            {
                var ForeachSorgu = Types.GetAllTypes(select);
                ForeachSorgu.Name.ForEach(row =>
                {
                    selectStr += row + ",";
                    SelectList.Add(row);
                });
                selectStr = selectStr.Remove(selectStr.Length - 1, 1);

            }
            else
                selectStr = "*";
            var str = $"SELECT {selectStr} FROM {tableName}";

            return new COASqlSelectData<T>() { SqlQuery = str, Lenght = str.Length, TableName = tableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, SelectedColumns = SelectList };
        }

        public COASqlData<T> GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {tableName} ");
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

            return new COASqlData<T>() { SqlQuery = insertQuery.ToString(), Lenght = insertQuery.ToString().Length, TableName = tableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName };
        }
        public COASqlUpdateData<T> GenerateUpdateQuery(Expression<Func<T, bool>> where)
        {
            var SelectedList = new List<string>();
            var updateQuery = new StringBuilder($"UPDATE {tableName} SET ");
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
            return new COASqlUpdateData<T>() { SqlQuery = updateQuery.ToString(), Lenght = updateQuery.ToString().Length, SelectedColumns = SelectedList, TableName = tableName, Oracle = Oracle, PrimaryKeyName = PrimaryKeyName, WhereQuery = wherestring };
        }

        public COASqlJoinData<T, T1, T2> GenerateJoinQuery<T1, T2>(Expression<Func<T1, T2, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData,oracle:Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3> GenerateJoinQuery<T1, T2, T3>(Expression<Func<T1, T2, T3, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, oracle: Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4> GenerateJoinQuery<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, oracle: Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5> GenerateJoinQuery<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, oracle: Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6> GenerateJoinQuery<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, oracle: Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7> GenerateJoinQuery<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, oracle: Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8> GenerateJoinQuery<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> SetTables)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, oracle: Oracle));
            return RData;
        }

        public COASqlJoinData<T, T1, T2> GenerateLeftJoinQuery<T1, T2>(Expression<Func<T1, T2, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.LEFT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3> GenerateLeftJoinQuery<T1, T2, T3>(Expression<Func<T1, T2, T3, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.LEFT,Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4> GenerateLeftJoinQuery<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.LEFT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5> GenerateLeftJoinQuery<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.LEFT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6> GenerateLeftJoinQuery<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.LEFT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7> GenerateLeftJoinQuery<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.LEFT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8> GenerateLeftJoinQuery<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> SetTables)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.LEFT, Oracle));
            return RData;
        }



        public COASqlJoinData<T, T1, T2> GenerateRightJoinQuery<T1, T2>(Expression<Func<T1, T2, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.RIGHT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3> GenerateRightJoinQuery<T1, T2, T3>(Expression<Func<T1, T2, T3, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.RIGHT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4> GenerateRightJoinQuery<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.RIGHT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5> GenerateRightJoinQuery<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.RIGHT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6> GenerateRightJoinQuery<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.RIGHT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7> GenerateRightJoinQuery<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> SetTables = null)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData, COASqlJoinTypes.RIGHT, Oracle));
            return RData;
        }
        public COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8> GenerateRightJoinQuery<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> SetTables)
        {
            var RData = new COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8>();
            RData.Add(MainJoinGeneator<T1>(SetTables, RData,COASqlJoinTypes.RIGHT, Oracle));
            return RData;
        }

        private COASqlJoinData<T> MainJoinGeneator<T1>(Expression SetTables, object obj,COASqlJoinTypes type=COASqlJoinTypes.INNER,bool oracle=false)
        {
            var returnData = new COASqlJoinData<T>();
            returnData.Oracle = oracle;
            returnData.TableName = TableName;
            returnData.SetJoinType(type);
            var Column1Selected = Types.GetAllTypes(SetTables);
            var Selectedlist = new List<string>();
            if (Column1Selected.Name.Count == 0)
            {

                var Types = GetGenericTypeList(obj);

                for (int i = 1; i < Types.Length; i++)
                {
                    var Props = Types[i].GetProperties();
                    foreach (var prop in Props)
                    {
                        Selectedlist.Add($"{prop.ReflectedType.Name}.{prop.Name}");
                    }
                }
            }
            else
                Selectedlist = Types.JoinBaseGenerator(Column1Selected);

            returnData.SelectedColumns = Selectedlist;
            returnData.SqlQuery = Types.JoinSqlGenerator<T1>(returnData.SelectedColumns);
            return returnData;
        }
        private Type[] GetGenericTypeList(object aa)
        {
            return aa.GetType().GetGenericArguments();
        }

    }
    public static class Extensionn
    {
        static COAGetAllTypes Types = new COAGetAllTypes();
        /// <summary>
        /// Set OrderBy option
        /// </summary>
        public static COASqlSelectData<T> OrderBy<T>(this COASqlSelectData<T> data, Expression<Func<T, object>> select)
        {
            string selectStr = " ORDER BY ";
            var ForeachSorgu = Types.GetAllTypes(select);
            ForeachSorgu.Name.ForEach(row => { selectStr += row + ","; });
            selectStr = selectStr.Remove(selectStr.Length - 1, 1);
            data.SqlQuery += selectStr + " ASC";
            data.Lenght = data.SqlQuery.Length;
            data.OrderQuery = selectStr + " ASC";
            return data;
        }
        /// <summary>
        /// Set OrderByDescing option
        /// </summary>
        public static COASqlSelectData<T> OrderByDescing<T>(this COASqlSelectData<T> data, Expression<Func<T, object>> select)
        {
            string selectStr = " ORDER BY ";
            var ForeachSorgu = Types.GetAllTypes(select);
            ForeachSorgu.Name.ForEach(row => { selectStr += row + ","; });
            selectStr = selectStr.Remove(selectStr.Length - 1, 1);
            data.SqlQuery += selectStr + " DESC";
            data.Lenght = data.SqlQuery.Length;
            data.OrderQuery = selectStr + " DESC";
            return data;
        }
        /// <summary>
        /// This is search property for your datas
        /// </summary>
        public static COASqlSelectData<T> Where<T>(this COASqlSelectData<T> data, Expression<Func<T, bool>> Where)
        {
            var str = Types.PredicateToString(Where, data.Oracle);
            data.SqlQuery += str;
            data.WhereQuery = str;
            data.Lenght = data.SqlQuery.Length;
            return data;
        }
        /// <summary>
        /// Skip - Take your list in database
        /// </summary>
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
                str = $"select {SelectedColumns} from(select a.*, rownum rnum from(select * from {data.TableName}{data.WhereQuery} {Ordrquery}) a where rownum <= {take + skip})where rnum > {skip}";
            }


            data.SqlQuery = str;
            data.Lenght = data.SqlQuery.Length;
            data.OrderQuery = Ordrquery;
            return data;
        }
        private static List<string> JoinEqualGenerator(Expression EqualExp, COASqlJoinTypes type)
        {
            var Column1Selected = Types.GetAllTypes(EqualExp);
            var list = new List<string>();

            for (int i = 0; i <= Column1Selected.AndOr.Count; i++)
            {
                var left = i * 2;
                var right = (i * 2 + 1);
                switch (type)
                {
                    case COASqlJoinTypes.INNER:
                        list.Add($" INNER JOIN {Column1Selected.Class[left]} ON {Column1Selected.Class[left]}.{Column1Selected.Name[left]} = {Column1Selected.Class[right]}.{Column1Selected.Name[right]}");
                        break;
                    case COASqlJoinTypes.LEFT:
                        list.Add($" LEFT JOIN {Column1Selected.Class[left]} ON {Column1Selected.Class[left]}.{Column1Selected.Name[left]} = {Column1Selected.Class[right]}.{Column1Selected.Name[right]}");
                        break;
                    case COASqlJoinTypes.RIGHT:
                        list.Add($" RIGHT JOIN {Column1Selected.Class[left]} ON {Column1Selected.Class[left]}.{Column1Selected.Name[left]} = {Column1Selected.Class[right]}.{Column1Selected.Name[right]}");
                        break;
                    default:
                        break;
                }
             
            }

            return list;
        }

        //UpdateExtensions

        /// <summary>
        /// Select what you want columns
        /// </summary>
        public static COASqlUpdateData<T> SelectColums<T>(this COASqlUpdateData<T> data, Expression<Func<T, object>> select)
        {
            var SelectedColums = Types.GetAllTypes(select);
            var updateQuery = new StringBuilder($"UPDATE {data.TableName} SET ");
            data.SelectedColumns.Clear();
            SelectedColums.Name.ForEach(colums =>
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
        /// <summary>
        /// Remove what you want columns
        /// </summary>
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
                    SelectedColums.Name.ForEach(selcomuns =>
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
        //JoinExtensions
        public static COASqlEqualData<T, T1, T2> Equal<T, T1, T2>(this COASqlJoinData<T, T1, T2> data, Expression<Func<T1, T2, bool>> equal)
        {
            data.JoinQuery = JoinEqualGenerator(equal, data.JoinType);
            foreach (var item in data.JoinQuery)
            {
                data.SqlQuery += item;
            }
            var equaldata = new COASqlEqualData<T, T1, T2>();
            equaldata.Add(data);
            return equaldata;
        }
        public static COASqlEqualData<T, T1, T2, T3> Equal<T, T1, T2, T3>(this COASqlJoinData<T, T1, T2, T3> data, Expression<Func<T1, T2, T3, bool>> equal)
        {
            data.JoinQuery = JoinEqualGenerator(equal, data.JoinType);
            foreach (var item in data.JoinQuery)
            {
                data.SqlQuery += item;
            }
            var equaldata = new COASqlEqualData<T, T1, T2, T3>();
            equaldata.Add(data);
            return equaldata;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4> Equal<T, T1, T2, T3, T4>(this COASqlJoinData<T, T1, T2, T3, T4> data, Expression<Func<T1, T2, T3, T4, bool>> equal)
        {
            data.JoinQuery = JoinEqualGenerator(equal, data.JoinType);
            foreach (var item in data.JoinQuery)
            {
                data.SqlQuery += item;
            }
            var equaldata = new COASqlEqualData<T, T1, T2, T3, T4>();
            equaldata.Add(data);
            return equaldata;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5> Equal<T, T1, T2, T3, T4, T5>(this COASqlJoinData<T, T1, T2, T3, T4, T5> data, Expression<Func<T1, T2, T3, T4, T5, bool>> equal)
        {
            data.JoinQuery = JoinEqualGenerator(equal, data.JoinType);
            foreach (var item in data.JoinQuery)
            {
                data.SqlQuery += item;
            }
            var equaldata = new COASqlEqualData<T, T1, T2, T3, T4, T5>();
            equaldata.Add(data);
            return equaldata;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6> Equal<T, T1, T2, T3, T4, T5, T6>(this COASqlJoinData<T, T1, T2, T3, T4, T5, T6> data, Expression<Func<T1, T2, T3, T4, T5, T6, bool>> equal)
        {
            data.JoinQuery = JoinEqualGenerator(equal, data.JoinType);
            foreach (var item in data.JoinQuery)
            {
                data.SqlQuery += item;
            }
            var equaldata = new COASqlEqualData<T, T1, T2, T3, T4, T5, T6>();
            equaldata.Add(data);
            return equaldata;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> Equal<T, T1, T2, T3, T4, T5, T6, T7>(this COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> equal)
        {
            data.JoinQuery = JoinEqualGenerator(equal, data.JoinType);
            foreach (var item in data.JoinQuery)
            {
                data.SqlQuery += item;
            }
            var equaldata = new COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7>();
            equaldata.Add(data);
            return equaldata;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> Equal<T, T1, T2, T3, T4, T5, T6, T7, T8>(this COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> equal)
        {
            data.JoinQuery = JoinEqualGenerator(equal, data.JoinType);
            foreach (var item in data.JoinQuery)
            {
                data.SqlQuery += item;
            }
            var equaldata = new COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8>();
            equaldata.Add(data);
            return equaldata;
        }
        //JoinRemove
        private static List<string> RemoveColums(List<string> selectedlist, List<string> RemoveList)
        {
            for (int i = 0; i < selectedlist.Count; i++)
            {
                var item = selectedlist[i];
                var cntrl = RemoveList.Where(a => a == item).FirstOrDefault();
                if (!string.IsNullOrEmpty(cntrl))
                    selectedlist.Remove(cntrl);
            }
            return selectedlist;
        }
        private static COASqlJoinData<T> MainRemoveGeneator<T, T1>(COASqlJoinData<T> returnData, Expression SetTables)
        {
            var RemoveSelected = Types.GetAllTypes(SetTables);
            var Newlist = RemoveColums(returnData.SelectedColumns, Types.JoinBaseGenerator(RemoveSelected));
            returnData.SelectedColumns = Newlist;
            returnData.SqlQuery = Types.JoinSqlGenerator<T1>(returnData.SelectedColumns);
            return returnData;
        }
        public static COASqlJoinData<T, T1, T2> RemoveColums<T, T1, T2>(this COASqlJoinData<T, T1, T2> data, Expression<Func<T1, T2, bool[]>> removecolumn)
        {
            data.Add(MainRemoveGeneator<T, T1>(data, removecolumn));
            return data;
        }
        public static COASqlJoinData<T, T1, T2, T3> RemoveColums<T, T1, T2, T3>(this COASqlJoinData<T, T1, T2, T3> data, Expression<Func<T1, T2, T3, object[]>> removecolumn)
        {
            data.Add(MainRemoveGeneator<T, T1>(data, removecolumn));
            return data;
        }
        public static COASqlJoinData<T, T1, T2, T3, T4> RemoveColums<T, T1, T2, T3, T4>(this COASqlJoinData<T, T1, T2, T3, T4> data, Expression<Func<T1, T2, T3, T4, object[]>> removecolumn)
        {
            data.Add(MainRemoveGeneator<T, T1>(data, removecolumn));
            return data;
        }
        public static COASqlJoinData<T, T1, T2, T3, T4, T5> RemoveColums<T, T1, T2, T3, T4, T5>(this COASqlJoinData<T, T1, T2, T3, T4, T5> data, Expression<Func<T1, T2, T3, T4, T5, object[]>> removecolumn)
        {
            data.Add(MainRemoveGeneator<T, T1>(data, removecolumn));
            return data;
        }
        public static COASqlJoinData<T, T1, T2, T3, T4, T5, T6> RemoveColums<T, T1, T2, T3, T4, T5, T6>(this COASqlJoinData<T, T1, T2, T3, T4, T5, T6> data, Expression<Func<T1, T2, T3, T4, T5, T6, object[]>> removecolumn)
        {
            data.Add(MainRemoveGeneator<T, T1>(data, removecolumn));
            return data;
        }
        public static COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7> RemoveColums<T, T1, T2, T3, T4, T5, T6, T7>(this COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, object[]>> removecolumn)
        {
            data.Add(MainRemoveGeneator<T, T1>(data, removecolumn));
            return data;
        }
        public static COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8> RemoveColums<T, T1, T2, T3, T4, T5, T6, T7, T8>(this COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object[]>> removecolumn)
        {
            data.Add(MainRemoveGeneator<T, T1>(data, removecolumn));
            return data;
        }
        //joinWhere

        public static COASqlEqualData<T, T1, T2> Where<T, T1, T2>(this COASqlEqualData<T, T1, T2> data, Expression<Func<T1, T2, bool>> where)
        {
            var wherestr = Types.PredicateToString(where, data.Oracle, true);
            data.WhereQuery = wherestr;
            data.SqlQuery += wherestr;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3> Where<T, T1, T2, T3>(this COASqlEqualData<T, T1, T2, T3> data, Expression<Func<T1, T2, T3, bool>> where)
        {
            var wherestr = Types.PredicateToString(where, data.Oracle, true);
            data.WhereQuery = wherestr;
            data.SqlQuery += wherestr;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4> Where<T, T1, T2, T3, T4>(this COASqlEqualData<T, T1, T2, T3, T4> data, Expression<Func<T1, T2, T3, T4, bool>> where)
        {
            var wherestr = Types.PredicateToString(where, data.Oracle);
            data.WhereQuery = wherestr;
            data.SqlQuery += wherestr;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5> Where<T, T1, T2, T3, T4, T5>(this COASqlEqualData<T, T1, T2, T3, T4, T5> data, Expression<Func<T1, T2, T3, T4, T5, bool>> where)
        {
            var wherestr = Types.PredicateToString(where, data.Oracle, true);
            data.WhereQuery = wherestr;
            data.SqlQuery += wherestr;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6> Where<T, T1, T2, T3, T4, T5, T6>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6> data, Expression<Func<T1, T2, T3, T4, T5, T6, bool>> where)
        {
            var wherestr = Types.PredicateToString(where, data.Oracle, true);
            data.WhereQuery = wherestr;
            data.SqlQuery += wherestr;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> Where<T, T1, T2, T3, T4, T5, T6, T7>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> where)
        {
            var wherestr = Types.PredicateToString(where, data.Oracle, true);
            data.WhereQuery = wherestr;
            data.SqlQuery += wherestr;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> Where<T, T1, T2, T3, T4, T5, T6, T7, T8>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> where)
        {
            var wherestr = Types.PredicateToString(where, data.Oracle);
            data.WhereQuery = wherestr;
            data.SqlQuery += wherestr;
            return data;
        }
        //JoinOrderBy
        private static string OrderByString(Expression select, bool desc = false)
        {
            string selectStr = " ORDER BY ";
            var ForeachSorgu = Types.GetAllTypes(select);
            int i = 0;
            ForeachSorgu.Name.ForEach(row => { selectStr += ForeachSorgu.Class[i] + "." + row + ","; i++; });
            selectStr = selectStr.Remove(selectStr.Length - 1, 1);
            if (!desc)
                selectStr += " ASC";
            else
                selectStr += " DESC";
            return selectStr;
        }
        public static COASqlEqualData<T, T1, T2> OrderBy<T, T1, T2>(this COASqlEqualData<T, T1, T2> data, Expression<Func<T1, T2, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3> OrderBy<T, T1, T2, T3>(this COASqlEqualData<T, T1, T2, T3> data, Expression<Func<T1, T2, T3, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4> OrderBy<T, T1, T2, T3, T4>(this COASqlEqualData<T, T1, T2, T3, T4> data, Expression<Func<T1, T2, T3, T4, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5> OrderBy<T, T1, T2, T3, T4, T5>(this COASqlEqualData<T, T1, T2, T3, T4, T5> data, Expression<Func<T1, T2, T3, T4, T5, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6> OrderBy<T, T1, T2, T3, T4, T5, T6>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6> data, Expression<Func<T1, T2, T3, T4, T5, T6, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> OrderBy<T, T1, T2, T3, T4, T5, T6, T7>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> OrderBy<T, T1, T2, T3, T4, T5, T6, T7, T8>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        //JoinOrderByDesc
        public static COASqlEqualData<T, T1, T2> OrderByDesc<T, T1, T2>(this COASqlEqualData<T, T1, T2> data, Expression<Func<T1, T2, object>> order)
        {
            data.OrderQuery = OrderByString(order);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3> OrderByDesc<T, T1, T2, T3>(this COASqlEqualData<T, T1, T2, T3> data, Expression<Func<T1, T2, T3, object>> order)
        {
            data.OrderQuery = OrderByString(order, true);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4> OrderByDesc<T, T1, T2, T3, T4>(this COASqlEqualData<T, T1, T2, T3, T4> data, Expression<Func<T1, T2, T3, T4, object>> order)
        {
            data.OrderQuery = OrderByString(order, true);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5> OrderByDesc<T, T1, T2, T3, T4, T5>(this COASqlEqualData<T, T1, T2, T3, T4, T5> data, Expression<Func<T1, T2, T3, T4, T5, object>> order)
        {
            data.OrderQuery = OrderByString(order, true);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6> OrderByDesc<T, T1, T2, T3, T4, T5, T6>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6> data, Expression<Func<T1, T2, T3, T4, T5, T6, object>> order)
        {
            data.OrderQuery = OrderByString(order, true);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> OrderByDesc<T, T1, T2, T3, T4, T5, T6, T7>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> order)
        {
            data.OrderQuery = OrderByString(order, true);
            data.SqlQuery += data.OrderQuery;
            return data;
        }
        public static COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> OrderByDesc<T, T1, T2, T3, T4, T5, T6, T7, T8>(this COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> data, Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> order)
        {
            data.OrderQuery = OrderByString(order, true);
            data.SqlQuery += data.OrderQuery;
            return data;
        }

    }

}
