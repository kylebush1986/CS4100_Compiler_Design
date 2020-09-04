using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace KyleBushCompiler
{
    class SymbolData
    {
        public int Kind { get; set; }
        public String DataType { get; set; }
     
        public int Index { get; set; }
        private string stringValue;
        private int intValue;
        private double doubleValue;

        public void SetValue(string value)
        {
            stringValue = value;
        }

        public void SetValue(int value)
        {
            intValue = value;
        }

        public void SetValue(double value)
        {
            doubleValue = value;
        }

        public string GetValue()
        {
            return stringValue;
        }

        public int GetValue()
        {
            return intValue;
        }
    }
}
