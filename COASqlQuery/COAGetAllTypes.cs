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
        public QueryEntityList GetAllTypes<T, U>(Expression<Func<T, U>> expression)
        {

            var v = new ReferanceAllTypes(typeof(T));
            v.Visit(expression);
            QueryEntitiy QueryItem = null;
            var listt = new QueryEntityList();
            for (int i = 0; i < v.Ad.Count; i++)
            {
                try
                {
                    QueryItem = new QueryEntitiy()
                    {
                        EqualType = v.EqualType[i],
                        Type = new TypeAndValue()
                        {
                            Name = v.Ad[i],
                            Value = v.Valuee[i].ToString(),
                            Type = v.type[i]
                        }
                    };
                }
                catch (Exception)
                {
                }

                listt.Data.Add(QueryItem);
            }
            listt.Ad = v.Ad;
            listt.AndOrOr = v.AndAlsoOrOr;
            return listt;
        }

        public List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }


        public string PredicateToString<T>(Expression<Func<T, bool>> predicate,bool Oracle)
        {

            var ForeachSorgu = GetAllTypes(predicate);
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
                        str += " " + Query.Type.Name + " LIKE '%" + TurnChar(Query.Type.Value) + "%'";
                        break;
                    case "StartsWith":
                        str += " " + Query.Type.Name + " LIKE '" + TurnChar(Query.Type.Value) + "%'";
                        break;
                    case "EndsWith":
                        str += " " + Query.Type.Name + " LIKE '% " + TurnChar(Query.Type.Value) + "'";
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
        private string TurnChar(string str)
        {
            if (str.Substring(0, 1) == "'")
            {
                str = str.Remove(str.Length - 1, 1);
                str = str.Remove(0, 1);
            }
            return str;
        }
    }
}
