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
            // My test file
            //string inputFilePath = @"C:\projects\CS4100_Compiler_Design\TestInput\program.txt";

            // My test file
            string inputFilePath = @"C:\projects\CS4100_Compiler_Design\TestInput\GetNextCharTest.txt";

            // Provided test file
            //string inputFilePath = @"C:\projects\CS4100_Compiler_Design\TestInput\LexicalTestF20.txt";

            //Psuedocode for main program
            ReserveTable tokenCodes = InitializeReserveWordTable();
            SymbolTable symbolTable = new SymbolTable();

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

        static ReserveTable InitializeReserveWordTable()
        {
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

            return tokenCodes;
        }

        static string[] InitializeInputFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        /// <summary>
        /// Prints the Lexeme, the token code, a table-looked-up 4-character mnemonic for that code,
        /// and for identifiers and literals added to the symbol table, the symbol table location index of the token.
        /// </summary>
        /// <param name="nextToken"></param>
        /// <param name="tokenCode"></param>
        /// <param name="tokenCodes"></param>
        /// <param name="symbolTable"></param>
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
