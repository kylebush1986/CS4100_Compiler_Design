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
            ReserveTable languageReserveWords = new ReserveTable();
            languageReserveWords.Add("STOP", 0);
            languageReserveWords.Add("DIV", 1);
            languageReserveWords.Add("MUL", 2);
            languageReserveWords.Add("SUB", 3);
            languageReserveWords.Add("ADD", 4);
            languageReserveWords.Add("MOV", 5);
            languageReserveWords.Add("STI", 6);
            languageReserveWords.Add("LDI", 7);
            languageReserveWords.Add("BNZ", 8);
            languageReserveWords.Add("BNP", 9);
            languageReserveWords.Add("BNN", 10);
            languageReserveWords.Add("BZ", 11);
            languageReserveWords.Add("BP", 12);
            languageReserveWords.Add("BN", 13);
            languageReserveWords.Add("BR", 14);
            languageReserveWords.Add("BINDR", 15);
            languageReserveWords.Add("PRINT", 16);

            string inputFilePath = @"C:\projects\CS4100_Compiler_Design\TestInput\program.txt";

            //Psuedocode for main program
            SymbolTable symbolTable = new SymbolTable();
            ReserveTable reserveTable = new ReserveTable();
            Scanner scanner = new Scanner();

            InitializeInputFile(inputFilePath);

            
            bool echoOn = true;

            while (!scanner.EndOfFile)
            {
                scanner.GetNextToken(echoOn);
                PrintToken(scanner.NextToken, scanner.TokenCode);
            }
            symbolTable.PrintSymbolTable();
            // Terminate();
        }

        static void InitializeStructures()
        {
            SymbolTable symbolTable = new SymbolTable();
            ReserveTable reserveTable = new ReserveTable();
        }

        static void InitializeInputFile(string filePath)
        {
            string[] fileText = File.ReadAllLines(filePath);

            foreach (string line in fileText)
            {
                Console.WriteLine(line);
            }
        }

        static void PrintToken(string nextToken, int tokenCode)
        {

        }
    }
}
