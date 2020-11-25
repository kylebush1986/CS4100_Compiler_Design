using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace KyleBushCompiler
{
    /// <summary>
    /// Used to specify data type of a symbol
    /// </summary>
    public enum DataType
    {
        Integer,
        Double,
        String,
        Invalid
    }

    /// <summary>
    /// Used to specify the kind of a symbol
    /// </summary>
    public enum SymbolKind
    {
        Label,
        Variable,
        Constant,
        ProgName
    }

    public class Symbol
    {
        public string Name { get; set; }
        public SymbolKind Kind { get; set; }
        public DataType DataType { get; set; }

        private int _intValue;
        private string _stringValue;
        private double _doubleValue;

        /// <summary>
        /// Contructor to initialize a Symbol containing an integer value.
        /// </summary>
        /// <param name="name">String name of symbol</param>
        /// <param name="kind">Defines the kind of the symbol</param>
        /// <param name="dataType">Defines the data type of the symbol</param>
        /// <param name="value">The integer value of the symbol</param>
        public Symbol(string name, SymbolKind kind, DataType dataType, int value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            _intValue = value;
        }

        /// <summary>
        /// Contructor to initialize a Symbol containing a double value.
        /// </summary>
        /// <param name="name">String name of symbol</param>
        /// <param name="kind">Defines the kind of the symbol</param>
        /// <param name="dataType">Defines the data type of the symbol</param>
        /// <param name="value">The double value of the symbol</param>
        public Symbol(string name, SymbolKind kind, DataType dataType, double value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            _doubleValue = value;
        }

        /// <summary>
        /// Contructor to initialize a Symbol containing a string value.
        /// </summary>
        /// <param name="name">String name of symbol</param>
        /// <param name="kind">Defines the kind of the symbol</param>
        /// <param name="dataType">Defines the data type of the symbol</param>
        /// <param name="value">The string value of the symbol</param>
        public Symbol(string name, SymbolKind kind, DataType dataType, string value)
        {
            Name = name;
            Kind = kind;
            DataType = dataType;
            _stringValue = value;
        }

        /// <summary>
        /// Sets a Symbol with an integer value.
        /// </summary>
        /// <param name="value">The integer value of the symbol</param>
        public void SetValue(int value)
        {
            _intValue = value;
        }

        /// <summary>
        /// Sets a Symbol with a string value.
        /// </summary>
        /// <param name="value">The string value of the symbol</param>
        public void SetValue(string value)
        {
            _stringValue = value;
        }

        /// <summary>
        /// Sets a Symbol with a double value.
        /// </summary>
        /// <param name="value">The double value of the symbol</param>
        public void SetValue(double value)
        {
            _doubleValue = value;
        }

        /// <summary>
        /// Checks the DataType of the Symbol and returns the appropriate value.
        /// </summary>
        /// <returns>int, string, or double depending on the DataType property.</returns>
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
