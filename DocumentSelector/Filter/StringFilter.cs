using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentSelector.Filter
{
    /// <summary>
    /// aaaa+bbbb+ddd,aaaa+dd+cc
    /// </summary>
    class StringFilter : IFilter
    {


        public bool match(string str, string filter)
        {
            if (str == null || str.Trim().Length == 0)
            {
                return false;
            }
            if (filter == null || filter.Trim().Length == 0)
            {
                return false;
            }
            return this.matchOr(str, filter);
        }

        private bool matchOr(string str, string filter)
        {
            string[] filters = filter.Split(',');
            bool result = false;
            foreach (var f in filters)
            {
                result = matchAnd(str, f);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        private bool matchAnd(string str, string filter)
        {
            string[] filters = filter.Split('+');
            bool[] temp = new bool[filters.Length];
            for (int i = 0; i < filters.Length; i++)
            {
                if (str.IndexOf(filters[i]) > -1)
                {
                    temp[i] = true;
                }
                else
                {
                    temp[i] = false;
                }
            }

            foreach (var item in temp)
            {
                if (item == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
