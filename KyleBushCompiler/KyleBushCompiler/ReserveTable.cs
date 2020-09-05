using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KyleBushCompiler
{
    class ReserveTable
    {
        public List<OpCode> ReserveTableData { get; set; }

        public ReserveTable()
        {
            Initialize();
        }

        /// <summary>
        /// Constructor, as needed.
        /// </summary>
        public void Initialize()
        {
            ReserveTableData = new List<OpCode>();

            Add("STOP", 0);
            Add("DIV", 1);
            Add("MUL", 2);
            Add("SUB", 3);
            Add("ADD", 4);
            Add("MOV", 5);
            Add("STI", 6);
            Add("LDI", 7);
            Add("BNZ", 8);
            Add("BNP", 9);
            Add("BNN", 10);
            Add("BZ", 11);
            Add("BP", 12);
            Add("BN", 13);
            Add("BR", 14);
            Add("BINDR", 15);
            Add("PRINT", 16);
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
            OpCode opCode = ReserveTableData.FirstOrDefault(x => x.Name == name);
            if (opCode == null)
            {
                return -1;
            }
            return opCode.Code;
        }

        /// <summary>
        /// Returns the associated name if code is there, else an empty string
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string LookupCode(int code)
        {
            OpCode opCode = ReserveTableData.FirstOrDefault(x => x.Code == code);
            if (opCode == null)
            {
                return "";
            }
            return opCode.Name;
        }

        /// <summary>
        /// Prints the currently used contents of the Reserve table in neat tabular format
        /// </summary>
        public void PrintReserveTable()
        {
            Console.WriteLine("RESERVE TABLE");
            Console.WriteLine("-----------------");
            Console.WriteLine($"|{ "Name", -7 } | { "Code", 5 }|");
            Console.WriteLine("-----------------");
            foreach (var code in ReserveTableData)
            {
                Console.WriteLine($"|{ code.Name, -7 } | { code.Code, 5 }|");
            }
            Console.WriteLine("-----------------");
        }
    }
}
