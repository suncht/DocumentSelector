using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentSelector
{
    class Item
    {
        public string text;
        public string value;

        public Item(string text, string value)
        {
            this.text = text;
            this.value = value;
        }

        public override string ToString()
        {
            return this.text;
        }
    }
}
