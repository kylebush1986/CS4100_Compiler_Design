using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection.Emit;

namespace KyleBushCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            ReserveTable OpCodes = new ReserveTable();
            OpCodes.Add("STOP", 0);
            OpCodes.Add("DIV", 1);
            OpCodes.Add("MUL", 2);
            OpCodes.Add("SUB", 3);
            OpCodes.Add("ADD", 4);
            OpCodes.Add("MOV", 5);
            OpCodes.Add("STI", 6);
            OpCodes.Add("LDI", 7);
            OpCodes.Add("BNZ", 8);
            OpCodes.Add("BNP", 9);
            OpCodes.Add("BNN", 10);
            OpCodes.Add("BZ", 11);
            OpCodes.Add("BP", 12);
            OpCodes.Add("BN", 13);
            OpCodes.Add("BR", 14);
            OpCodes.Add("BINDR", 15);
            OpCodes.Add("PRINT", 16);

            ReserveTable tokenCodes = new ReserveTable();

            // Reserved Words
            tokenCodes.Add("GOTO", 0);
            tokenCodes.Add("INTEGER", 1);
            tokenCodes.Add("TO", 2);
            tokenCodes.Add("DO", 3);
            tokenCodes.Add("IF", 4);
            tokenCodes.Add("THEN", 5);
            tokenCodes.Add("ELSE", 6);
            tokenCodes.Add("FOR", 7);
            tokenCodes.Add("OF", 8);
            tokenCodes.Add("WRITELN", 9);
            tokenCodes.Add("READLN", 10);
            tokenCodes.Add("BEGIN", 11);
            tokenCodes.Add("END", 12);
            tokenCodes.Add("VAR", 13);
            tokenCodes.Add("WHILE", 14);
            tokenCodes.Add("UNIT", 15);
            tokenCodes.Add("LABEL", 16);
            tokenCodes.Add("REPREAT", 17);
            tokenCodes.Add("UNTIL", 18);
            tokenCodes.Add("PROCEDURE", 19);
            tokenCodes.Add("DOWNTO", 20);
            tokenCodes.Add("FUNCTION", 21);
            tokenCodes.Add("RETURN", 22);
            tokenCodes.Add("REAL", 23);
            tokenCodes.Add("STRING", 24);
            tokenCodes.Add("ARRAY", 25);

            // Other Tokens
            tokenCodes.Add("/", 30);
            tokenCodes.Add("*", 31);
            tokenCodes.Add("+", 32);
            tokenCodes.Add("-", 33);
            tokenCodes.Add("(", 34);
            tokenCodes.Add(")", 35);
            tokenCodes.Add(";", 36);
            tokenCodes.Add(":=", 37);
            tokenCodes.Add(">", 38);
            tokenCodes.Add("<", 39);
            tokenCodes.Add(">=", 40);
            tokenCodes.Add("<=", 41);
            tokenCodes.Add("=", 42);
            tokenCodes.Add("<>", 43);
            tokenCodes.Add(",", 44);
            tokenCodes.Add("[", 45);
            tokenCodes.Add("]", 46);
            tokenCodes.Add(":", 47);
            tokenCodes.Add(".", 48);

            // Identifiers
            tokenCodes.Add("IDENTIFIER", 50);

            // Numeric Constants
            tokenCodes.Add("INTEGER", 51);
            tokenCodes.Add("FLOATING-POINT", 52);

            // String
            tokenCodes.Add("STRING", 53);

            // My test file
            //string inputFilePath = @"C:\projects\CS4100_Compiler_Design\TestInput\program.txt";

            // Provided test file
            string inputFilePath = @"C:\projects\CS4100_Compiler_Design\TestInput\LexicalTestF20.txt";

            //Psuedocode for main program
            SymbolTable symbolTable = new SymbolTable();
            ReserveTable reserveTable = new ReserveTable();

            try
            {
                string[] fileText = InitializeInputFile(inputFilePath);
                Scanner scanner = new Scanner();
                scanner.Initialize(fileText, symbolTable, tokenCodes);
                bool echoOn = true;

                while (!scanner.EndOfFile)
                {
                    scanner.GetNextToken(echoOn);
                    PrintToken(scanner.NextToken, scanner.TokenCode, tokenCodes, symbolTable);
                }
                symbolTable.PrintSymbolTable();
                // Terminate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void InitializeStructures()
        {
            SymbolTable symbolTable = new SymbolTable();
            ReserveTable reserveTable = new ReserveTable();
        }

        static string[] InitializeInputFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        static void PrintToken(string nextToken, int tokenCode, ReserveTable tokenCodes, SymbolTable symbolTable)
        {
            string mneumonic = tokenCodes.LookupCode(tokenCode);

            if (tokenCode >= 50 && tokenCode <= 53)
            {
                Console.WriteLine($"Token: {nextToken}, Token Code: {tokenCode}, Mneumonic: {mneumonic}, Symbol Table Index: {symbolTable.LookupSymbol(nextToken)}");
            }
            else
            {
                Console.WriteLine($"Token: {nextToken}, Token Code: {tokenCode}, Mneumonic: {mneumonic}");
            }

            
        }
    }
}
