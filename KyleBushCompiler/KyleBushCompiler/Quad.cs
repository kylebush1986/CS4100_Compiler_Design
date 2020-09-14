using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    /// <summary>
    /// Contains the data for a single Quad Code
    /// </summary>
    public class Quad
    {
        public int OpCode { get; set; }
        public int Op1 { get; set; }
        public int Op2 { get; set; }
        public int Op3 { get; set; }

        /// <summary>
        /// Constructor which creates a new Quad Code
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <param name="op3"></param>
        public Quad(int opcode, int op1, int op2, int op3)
        {
            OpCode = opcode;
            Op1 = op1;
            Op2 = op2;
            Op3 = op3;
        }
    }
}
