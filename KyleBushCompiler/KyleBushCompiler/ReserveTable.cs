using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    class ReserveTable
    {
        public List<OpCode> ReserveTableData { get; set; }

        /// <summary>
        /// Constructor, as needed.
        /// </summary>
        public void Initialize()
        {
            Add("STOP", 0);
            Add("DIV", 1);
            Add("MUL", 2);
            Add("SUB", 3);
            Add("ADD", 4);
            ReserveTableData.Add(new OpCode { Name = "MOV", Code = 5 });
            ReserveTableData.Add(new OpCode { Name = "STI", Code = 6 });
            ReserveTableData.Add(new OpCode { Name = "LDI", Code = 7 });

        }

        /// <summary>
        /// Returns the index of the row where the data was place, just adds to end of list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns>index of the row where the data was placed</returns>
        public int Add(string name, int code)
        {
            OpCode opCode = new OpCode(name, code);
            ReserveTableData.Add(opCode);
            return ReserveTableData.Count - 1;
        }

        /// <summary>
        /// Returns the code associated with name if name is in the table, else returns -1
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int LookupName(string name)
        {
            int code = 0;
            return code;
        }

        /// <summary>
        /// Returns the associated name if code is there, else an empty string
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string LookupCode(int code)
        {
            string name = "";
            return name;
        }

        /// <summary>
        /// Prints the currently used contents of the Reserve table in neat tabular format
        /// </summary>
        public void PrintReserveTable()
        {

        }
    }
}
