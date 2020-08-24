using System;
using System.Collections.Generic;
using System.Text;

namespace COASqlQuery
{

    public class COASqlJoinData<T> : COASqlData<T>, ICOASqlJoinData<T>
    {
        private COASqlJoinTypes joinType;
        public COASqlJoinTypes JoinType { get { return joinType; } }
        public List<string> JoinQuery { get; set; }
        public COASqlJoinData(COASqlJoinTypes types = COASqlJoinTypes.INNER)
        {
            joinType = types;
        }
        public void Add(COASqlJoinData<T> a)
        {
            this.JoinQuery = a.JoinQuery;
            this.SqlQuery = a.SqlQuery;
            this.Lenght = a.SqlQuery.Length;
            this.Oracle = a.Oracle;
            this.OrderQuery = a.OrderQuery;
            this.PrimaryKeyName = a.PrimaryKeyName;
            this.SelectedColumns = a.SelectedColumns;
            this.TableName = a.TableName;
            this.WhereQuery = a.WhereQuery;
            joinType = a.JoinType;
        }
        public void SetJoinType( COASqlJoinTypes type)
        {
            joinType = type;
        }
    }
    public class COASqlJoinData<T, T1, T2> : COASqlJoinData<T>
    {

    }
    public class COASqlJoinData<T, T1, T2, T3> : COASqlJoinData<T>
    {

    }
    public class COASqlJoinData<T, T1, T2, T3, T4> : COASqlJoinData<T>
    {

    }
    public class COASqlJoinData<T, T1, T2, T3, T4, T5> : COASqlJoinData<T>
    {

    }
    public class COASqlJoinData<T, T1, T2, T3, T4, T5, T6> : COASqlJoinData<T>
    {

    }
    public class COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7> : COASqlJoinData<T>
    {

    }
    public class COASqlJoinData<T, T1, T2, T3, T4, T5, T6, T7, T8> : COASqlJoinData<T>
    {

    }
    public class COASqlEqualData<T, T1, T2> : COASqlJoinData<T>
    {

    }
    public class COASqlEqualData<T, T1, T2, T3> : COASqlJoinData<T>
    {

    }
    public class COASqlEqualData<T, T1, T2, T3, T4> : COASqlJoinData<T>
    {

    }
    public class COASqlEqualData<T, T1, T2, T3, T4, T5> : COASqlJoinData<T>
    {

    }
    public class COASqlEqualData<T, T1, T2, T3, T4, T5, T6> : COASqlJoinData<T>
    {

    }
    public class COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7> : COASqlJoinData<T>
    {

    }
    public class COASqlEqualData<T, T1, T2, T3, T4, T5, T6, T7, T8> : COASqlJoinData<T>
    {

    }
}
