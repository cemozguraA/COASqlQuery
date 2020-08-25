using System;
using System.Collections.Generic;
using System.Text;

namespace COASqlQuery
{
    public class COASqlData<T>
    {
        public string SqlQuery { get; set; }
        public int Lenght { get; set; }
        public string TableName { get; set; }
        public COADataBaseTypes DataBaseType { get; set; }
        public string PrimaryKeyName { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();
        public string OrderQuery { get; set; }
        public string WhereQuery { get; set; }
  

    }
}
