using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    class Quad
    {
        public int OpCode { get; set; }
        public int Op1 { get; set; }
        public int Op2 { get; set; }
        public int Op3 { get; set; }

        public Quad(int opcode, int op1, int op2, int op3)
        {
            OpCode = opcode;
            Op1 = op1;
            Op2 = op2;
            Op3 = op3;
        }
    }
}
