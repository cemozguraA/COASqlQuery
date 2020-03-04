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

    }
}
