using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    /// <summary>
    /// Contains the string and integer representations of an OpCode.
    /// </summary>
    public class OpCode
    {
        public string Name { get; set; }
        public int Code { get; set; }

        /// <summary>
        /// Creates a new OpCode object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        public OpCode(string name, int code)
        {
            Name = name;
            Code = code;
        }
    }
}
