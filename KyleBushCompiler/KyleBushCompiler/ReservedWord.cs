using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    /// <summary>
    /// Contains the string and integer representations of an OpCode.
    /// </summary>
    public class ReservedWord
    {
        public string Name { get; set; }
        public int Code { get; set; }

        /// <summary>
        /// Creates a new ReservedWord object.
        /// </summary>
        /// <param name="name">String name of reserved word</param>
        /// <param name="code">Integer code of reserved word</param>
        public ReservedWord(string name, int code)
        {
            Name = name;
            Code = code;
        }
    }
}
