using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentSelector.Filter
{
    interface IFilter
    {
        bool match(string str, string filter);
    }
}
