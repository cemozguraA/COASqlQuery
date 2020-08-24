using System;
using System.Collections.Generic;
using System.Text;

namespace COASqlQuery
{
    public interface ICOASqlJoinData<T>
    {
        void Add(COASqlJoinData<T> a);
    }
}
