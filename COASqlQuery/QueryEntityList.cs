using System;
using System.Collections.Generic;
using System.Text;

namespace COASqlQuery
{
    public class QueryEntityList
    {
        public List<QueryEntitiy> Data { get; set; }
        public List<string> AndOrOr { get; set; }
        public List<string> Ad { get; set; }

        public QueryEntityList()
        {
            Data = new List<QueryEntitiy>();
        }

    }
}
