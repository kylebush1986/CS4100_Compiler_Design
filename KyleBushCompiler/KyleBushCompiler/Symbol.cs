using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace KyleBushCompiler
{
    enum DataType
    {
        Integer,
        Double,
        String
    }

    enum SymbolKind
    {
        Label,
        Variable,
        Constant
    }
    class Symbol
    {
        public string Name { get; set; }
        public int Kind { get; set; }
        public DataType DataType { get; set; }
     
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public double DoubleValue { get; set; }

        public Symbol(string name, int kind, DataType dataType, int value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            IntValue = value;
        }

        public Symbol(string name, int kind, DataType dataType, double value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            DoubleValue = value;
        }

        public Symbol(string name, int kind, DataType dataType, string value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            StringValue = value;
        }
    }
}
