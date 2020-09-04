using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    class OpCode
    {
        public string Name { get; set; }
        public int Code { get; set; }

        public OpCode(string name, int code)
        {
            Name = name;
            Code = code;
        }
    }
}
