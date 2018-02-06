using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentSelector
{
    class ItemComparer : IEqualityComparer<Item>  
    {
        public bool Equals(Item x, Item y)
        {
            return x.text == y.text;
        }

        public int GetHashCode(Item obj)
        {
            if (obj == null) { return 0; } else { return obj.ToString().GetHashCode(); }  
        }
    }
}
