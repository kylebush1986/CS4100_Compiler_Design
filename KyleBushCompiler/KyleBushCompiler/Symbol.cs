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

        private int _intValue;
        private string _stringValue;
        private double _doubleValue;

        public Symbol(string name, int kind, DataType dataType, int value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            _intValue = value;
        }

        public Symbol(string name, int kind, DataType dataType, double value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            _doubleValue = value;
        }

        public Symbol(string name, int kind, DataType dataType, string value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            _stringValue = value;
        }

        public void SetValue(int value)
        {
            _intValue = value;
        }

        public void SetValue(string value)
        {
            _stringValue = value;
        }

        public void SetValue(double value)
        {
            _doubleValue = value;
        }

        public dynamic GetValue()
        {
            if (DataType == DataType.Integer)
                return _intValue;
            else if (DataType == DataType.Double)
                return _doubleValue;
            else
                return _stringValue;
        }
    }
}
