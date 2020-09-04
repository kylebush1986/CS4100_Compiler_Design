using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    class QuadTable
    {
        public List<Quad> QuadTableData { get; set; }

        public QuadTable()
        {
          
        }

        /// <summary>
        /// Create a new, empty QuadTable ready for data to be added, with the specified number of rows(size).
        /// </summary>
        public void Initialize(int size) //size and other parameters as needed
        {

        }

        /// <summary>
        /// Returns the int index of the next open slot in the QuadTable.
        /// </summary>
        /// <returns>int index of the next open slot in the QuadTable</returns>
        public int NextQuad()
        {
            int index = 0;
            return index;
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

        }

        /// <summary>
        /// Returns the data for the opcode and three operands located at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Quad GetQuad(int index)
        {
            Quad quad = new Quad();
            return quad;
        }

        /// <summary>
        /// Changes the contents of the existing quad at index.Used only when backfilling jump addresses later, during code generation, and very important
        /// </summary>
        /// <param name="index"></param>
        /// <param name="opcode"></param>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <param name="op3"></param>
        public void SetQuad(int index, int opcode, int op1, int op2, int op3)
        {

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
            string mnemonic = "";
            return mnemonic;
        }

        /// <summary>
        /// Prints the currently used contents of the Quad table in neat tabular format
        /// </summary>
        public void PrintQuadTable()
        {

        }
    }
}
