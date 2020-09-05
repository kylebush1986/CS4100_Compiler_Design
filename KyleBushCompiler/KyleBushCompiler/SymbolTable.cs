using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KyleBushCompiler
{
    class SymbolTable
    {
        public List<SymbolData> SymbolTableData { get; set; }

        /// <summary>
        /// Adds symbol with given kind and value to the symbol table, automatically setting the correct data_type,
        /// and returns the index where the symbol was located. If the symbol is already in the table, 
        /// no change or verification is made, and this just returns the index where the symbol was found.
        /// </summary>
        /// <param name="symbol">The symbol to add to the symbol table</param>
        /// <param name="kind">The kind of symbol</param>
        /// <param name="value">The value associated with the given symbol</param>
        /// <returns>The index of the added symbol in the symbol table as an integer</returns>
        public int AddSymbol(string symbol, int kind, int value)
        {
            int index = 0;
            return index;
        }

        /// <summary>
        /// Adds symbol with given kind and value to the symbol table, automatically setting the correct data_type,
        /// and returns the index where the symbol was located. If the symbol is already in the table, 
        /// no change or verification is made, and this just returns the index where the symbol was found.
        /// </summary>
        /// <param name="symbol">The symbol to add to the symbol table</param>
        /// <param name="kind">The kind of symbol</param>
        /// <param name="value">The value associated with the given symbol</param>
        /// <returns>The index of the added symbol in the symbol table as an integer</returns>
        public int AddSymbol(string symbol, int kind, double value)
        {
            int index = 0;
            return index;
        }

        /// <summary>
        /// Adds symbol with given kind and value to the symbol table, automatically setting the correct data_type,
        /// and returns the index where the symbol was located. If the symbol is already in the table, 
        /// no change or verification is made, and this just returns the index where the symbol was found.
        /// </summary>
        /// <param name="symbol">The symbol to add to the symbol table</param>
        /// <param name="kind">The kind of symbol</param>
        /// <param name="value">The value associated with the given symbol</param>
        /// <returns>The index of the added symbol in the symbol table as an integer</returns>
        public int AddSymbol(string symbol, int kind, string value)
        {
            int index = 0;
            return index;
        }

        /// <summary>
        /// Returns the index where symbol is found, or -1 if not in the table
        /// </summary>
        /// <param name="symbol">The symbol to look for in the table.</param>
        /// <returns>The index of the symbol or -1 if not found</returns>
        public int LookupSymbol(string symbol)
        {
            int index = 0;
            return index;
        }

        /// <summary>
        /// Return kind, data type, and value fields stored at index
        /// </summary>
        /// <param name="index">The index of the symbol to return</param>
        /// <returns></returns>
        public SymbolData GetSymbol(int index)
        {
            SymbolData data = new SymbolData();
            return data;
        }

        /// <summary>
        /// Set appropriate fields at slot indicated by index
        /// </summary>
        /// <param name="index">The index of the symbol to update</param>
        /// <param name="kind">The kind of symbol</param>
        /// <param name="value">The value of the symbol</param>
        public void UpdateSymbol(int index, int kind, int value)
        {
            SymbolData Symbol = SymbolTableData[index];
            Symbol.Kind = kind;
        }

        /// <summary>
        /// Set appropriate fields at slot indicated by index
        /// </summary>
        /// <param name="index">The index of the symbol to update</param>
        /// <param name="kind">The kind of symbol</param>
        /// <param name="value">The value of the symbol</param>
        public void UpdateSymbol(int index, int kind, double value)
        {

        }

        /// <summary>
        /// Set appropriate fields at slot indicated by index
        /// </summary>
        /// <param name="index">The index of the symbol to update</param>
        /// <param name="kind">The kind of symbol</param>
        /// <param name="value">The value of the symbol</param>
        public void UpdateSymbol(int index, int kind, string value)
        {
            
        }
        

        /// <summary>
        /// Prints the utilized rows of the symbol table in neat tabular format, 
        /// showing only the value field which is active for that row
        /// </summary>
        public void PrintSymbolTable()
        {
            foreach (var symbol in SymbolTableData)
            {
                Console.WriteLine(symbol);
            }
        }
    }
}
