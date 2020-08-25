using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace COASqlQuery
{
    public partial class COAGetAllTypes
    {
        public COAReferanceDataType GetAllTypes(Expression expression)
        {
            var v = new ReferanceAllTypes();
            v.Visit(expression);
            return v.Datas;
        }
      
        public List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }


        public string PredicateToString(Expression predicate, COADataBaseTypes databasetype,bool join = false)
        {

            var Query = GetAllTypes(predicate);
            string str = " WHERE";
            for (int i = 0; i < Query.Name.Count; i++)
            {

                if (Query.Type[i] == typeof(DateTime))
                {
                    var date = Convert.ToDateTime(Query.Value[i]);
                    if (databasetype == COADataBaseTypes.Oracle)
                    {
                        if (date.Hour >= 1)
                            Query.Value[i] = "TO_DATE('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY/MM/DD HH:MI:SS')";
                        else
                            Query.Value[i] += "TO_DATE('" + date.ToString("yyyy-MM-dd") + "', 'YYYY/MM/DD')";
                    }
                    else
                    {
                        if (date.Second >= 1)
                            Query.Value[i] = "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        else
                            Query.Value[i] += "'" + date.ToString("yyyy-MM-dd") + "'";
                    }
                }
                switch (Query.EqualType[i])
                {
                    case "Equal":
                        str += " " + Query.Name[i] + " = " + Query.Value[i];
                        break;
                    case "NotEqual":
                        str += " " + Query.Name[i] + " != " + Query.Value[i];
                        break;
                    case "GreaterThan":
                        str += " " + Query.Name[i] + " > " + Query.Value[i];
                        break;
                    case "GreaterThanOrEqual":
                        str += " " + Query.Name[i] + " >= " + Query.Value[i];
                        break;
                    case "LessThan":
                        str += " " + Query.Name[i] + " < " + Query.Value[i];
                        break;
                    case "LessThanOrEqual":
                        str += " " + Query.Name[i] + " <= " + Query.Value[i];
                        break;
                    case "Contains":
                        str += " " + Query.Name[i] + " LIKE '%" + TurnChar(Query.Value[i]) + "%'";
                        break;
                    case "StartsWith":
                        str += " " + Query.Name[i] + " LIKE '" + TurnChar(Query.Value[i]) + "%'";
                        break;
                    case "EndsWith":
                        str += " " + Query.Name[i] + " LIKE '% " + TurnChar(Query.Value[i]) + "'";
                        break;
                }
                if (i <= Query.Name.Count - 2)
                {
                    switch (Query.AndOr[i])
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
            }

            return str;
        }
        private string TurnChar(string str)
        {
            if (str.Substring(0, 1) == "'")
            {
                str = str.Remove(str.Length - 1, 1);
                str = str.Remove(0, 1);
            }
            return str;
        }
        public string JoinSqlGenerator<T1>(List<string> SelectColums)
        {
            var str = new StringBuilder("SELECT ");
            SelectColums.ForEach((x) => { str.Append($"{x},"); });
            str.Remove(str.Length - 1, 1);
            str.Append(" FROM " + typeof(T1).Name);
            return str.ToString();
        }
        public List<string> JoinBaseGenerator(COAReferanceDataType Column1Selected)
        {
            var list = new List<string>();

            for (int i = 0; i < Column1Selected.Name.Count; i++)
            {
                list.Add($"{Column1Selected.Class[i]}.{Column1Selected.Name[i]}");
            }

            return list;
        }

    }
}
