using System;
using System.Reflection.Emit;

namespace KyleBushCompiler
{
    class Program
    {

        static void Main(string[] args)
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

            SymbolTable symbolTable = new SymbolTable();
            symbolTable.AddSymbol("n", (int)SymbolKind.Variable, 0);
            symbolTable.AddSymbol("prod", (int)SymbolKind.Variable, 0);
            symbolTable.AddSymbol("count", (int)SymbolKind.Variable, 0);
            symbolTable.AddSymbol("fact", (int)SymbolKind.Variable, 0);
            symbolTable.AddSymbol("5", (int)SymbolKind.Constant, 5);
            symbolTable.AddSymbol("1", (int)SymbolKind.Constant, 1);
            symbolTable.AddSymbol("temp", (int)SymbolKind.Variable, 0);
            symbolTable.PrintSymbolTable();

            Interpreter interpreter = new Interpreter();
            interpreter.InterpretQuads(quadTable, symbolTable, false);
        }
    }
}
