using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KyleBushCompiler
{
    class QuadTable
    {
        public List<Quad> QuadTableData { get; set; }
        public ReserveTable ReserveTable { get; set; }

        public QuadTable(ReserveTable reserveTable)
        {
            ReserveTable = reserveTable;
            Initialize();
        }

        /// <summary>
        /// Create a new, empty QuadTable ready for data to be added, with the specified number of rows(size).
        /// </summary>
        public void Initialize() //size and other parameters as needed
        {
            QuadTableData = new List<Quad>();
        }

        /// <summary>
        /// Returns the int index of the next open slot in the QuadTable.
        /// </summary>
        /// <returns>int index of the next open slot in the QuadTable</returns>
        public int NextQuad()
        {
            return QuadTableData.Count;
        }

        /// <summary>
        /// Expands the active length of the quad table by adding a new row at the NextQuad slot, 
        /// with the parameters sent as the new contents, and increments the NextQuad counter to the next available(empty) index.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <param name="op3"></param>
        public void AddQuad(int opcode, int op1, int op2, int op3)
        {
            QuadTableData.Add(new Quad(opcode, op1, op2, op3));
        }

        /// <summary>
        /// Returns the data for the opcode and three operands located at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Quad GetQuad(int index)
        {
            return QuadTableData[index];
        }

        /// <summary>
        /// Changes the contents of the existing quad at index. Used only when backfilling jump addresses later, during code generation, and very important
        /// </summary>
        /// <param name="index"></param>
        /// <param name="opcode"></param>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <param name="op3"></param>
        public void SetQuad(int index, int opcode, int op1, int op2, int op3)
        {
            QuadTableData[index] = new Quad(opcode, op1, op2, op3);
        }

        /// <summary>
        /// Returns the mnemonic string (‘ADD’, ‘PRINT’, etc.) associated with the opcode parameter.
        /// Used during interpreter ‘TRACE’ mode to print out the stored opcodes in readable format.
        /// Use the ReserveTable ADT to implement this.
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public string GetMnemonic(int opcode)
        {
            return ReserveTable.LookupCode(opcode);
        }

        /// <summary>
        /// Prints the currently used contents of the Quad table in neat tabular format
        /// </summary>
        public void PrintQuadTable()
        {
            Console.WriteLine("QUAD TABLE");
            Console.WriteLine("---------------------------");
            Console.WriteLine($"|{ "Opcode",-7 }|{ "Op1",5 }|{ "Op2",5 }|{ "Op3",5 }|");
            Console.WriteLine("---------------------------");
            foreach (var quad in QuadTableData)
            {
                Console.WriteLine($"|{ GetMnemonic(quad.OpCode),-7 }|{ quad.Op1,5 }|{ quad.Op2,5 }|{ quad.Op3,5 }|");
            }
            Console.WriteLine("---------------------------");
        }
    }
}
