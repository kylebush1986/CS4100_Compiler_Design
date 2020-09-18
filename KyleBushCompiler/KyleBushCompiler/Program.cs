using System;
using System.Reflection.Emit;

namespace KyleBushCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter interpreter = new Interpreter();

            // Build assets for factorial algorithm and run it without trace in interpreter
            QuadTable factQuadTable = BuildQuadsForFactorial();
            SymbolTable factSymbolTable = BuildSymbolTableForFactorial();
            interpreter.InterpretQuads(factQuadTable, factSymbolTable, false);

            // Build assets for summation algorithm and run it with trace in interpreter
            QuadTable factQuadTableTrace = BuildQuadsForFactorial();
            SymbolTable factSymbolTableTrace = BuildSymbolTableForFactorial();
            interpreter.InterpretQuads(factQuadTableTrace, factSymbolTableTrace, true);

            // Build assets for summation algorithm and run it without trace in interpreter
            QuadTable sumQuadTable = BuildQuadsForSummation();
            SymbolTable sumSymbolTable = BuildSymbolTableForSummation();
            interpreter.InterpretQuads(sumQuadTable, sumSymbolTable, false);

            // Build assets for summation algorithm and run it with trace in interpreter
            QuadTable sumQuadTableTrace = BuildQuadsForSummation();
            SymbolTable sumSymbolTableTrace = BuildSymbolTableForSummation();
            interpreter.InterpretQuads(sumQuadTableTrace, sumSymbolTableTrace, true);
        }

        /// <summary>
        /// Builds and prints the hard coded Reserve Table and Quad Table to run the factorial algorithm. 
        /// </summary>
        /// <returns>The Quad Table</returns>
        static QuadTable BuildQuadsForFactorial()
        {
            ReserveTable reserveTable = new ReserveTable();
            reserveTable.PrintReserveTable();

            QuadTable quadTable = new QuadTable(reserveTable);
            quadTable.AddQuad(5, 4, 0, 0);
            quadTable.AddQuad(5, 5, 0, 1);
            quadTable.AddQuad(5, 5, 0, 2);
            quadTable.AddQuad(3, 0, 2, 6);
            quadTable.AddQuad(13, 6, 0, 8);
            quadTable.AddQuad(2, 1, 2, 1);
            quadTable.AddQuad(4, 2, 5, 2);
            quadTable.AddQuad(14, 0, 0, 3);
            quadTable.AddQuad(5, 1, 0, 3);
            quadTable.AddQuad(16, 3, 0, 0);
            quadTable.AddQuad(0, 0, 0, 0);
            quadTable.PrintQuadTable();

            return quadTable;
        }

        /// <summary>
        /// Builds and prints the hard coded Reserve Table and Quad Table to run the summation algorithm. 
        /// </summary>
        /// <returns>The Quad Table</returns>
        static QuadTable BuildQuadsForSummation()
        {
            ReserveTable reserveTable = new ReserveTable();
            reserveTable.PrintReserveTable();

            QuadTable quadTable = new QuadTable(reserveTable);
            quadTable.AddQuad(5, 4, 0, 0);
            quadTable.AddQuad(5, 5, 0, 7);
            quadTable.AddQuad(5, 5, 0, 2);
            quadTable.AddQuad(3, 0, 2, 6);
            quadTable.AddQuad(13, 6, 0, 8);
            quadTable.AddQuad(4, 1, 2, 1);
            quadTable.AddQuad(4, 2, 5, 2);
            quadTable.AddQuad(14, 0, 0, 3);
            quadTable.AddQuad(5, 1, 0, 3);
            quadTable.AddQuad(16, 3, 0, 0);
            quadTable.AddQuad(0, 0, 0, 0);
            quadTable.PrintQuadTable();

            return quadTable;
        }

        /// <summary>
        /// Builds and prints the hard coded Symbol Table for the factorial algorithm.
        /// </summary>
        /// <returns>The Symbol Table</returns>
        static SymbolTable BuildSymbolTableForFactorial()
        {
            SymbolTable symbolTable = new SymbolTable();
            symbolTable.AddSymbol("n", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("prod", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("count", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("fact", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("10", SymbolKind.Constant, 10);
            symbolTable.AddSymbol("1", SymbolKind.Constant, 1);
            symbolTable.AddSymbol("temp", SymbolKind.Variable, 0);
            symbolTable.PrintSymbolTable();

            return symbolTable;
        }

        /// <summary>
        /// Builds and prints the hard coded Symbol Table for the summation algorithm.
        /// </summary>
        /// <returns>The Symbol Table</returns>
        static SymbolTable BuildSymbolTableForSummation()
        {
            SymbolTable symbolTable = new SymbolTable();
            symbolTable.AddSymbol("n", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("sum", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("count", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("summation", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("10", SymbolKind.Constant, 10);
            symbolTable.AddSymbol("1", SymbolKind.Constant, 1);
            symbolTable.AddSymbol("temp", SymbolKind.Variable, 0);
            symbolTable.AddSymbol("0", SymbolKind.Variable, 0);
            symbolTable.PrintSymbolTable();

            return symbolTable;
        }
    }
}
