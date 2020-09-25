using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace KyleBushCompiler
{
    class Scanner
    {
        public string NextToken { get; set; }
        public int TokenCode { get; set; }

        public bool EndOfFile { get; set; }

        public void Initialize()
        {
            EndOfFile = false;
        }

        /// <summary>
        /// Uses DFA to select the next available Token
        /// </summary>
        /// <param name="echoOn"> selects whether input lines are echoed when read</param>
        public void GetNextToken(bool echoOn)
        {

        }

        /// <summary>
        ///  Prints in straight columns the lexeme, the token code, 
        ///  a table-looked-up 4-character mnemonic for that code and,
        ///  for identifiers and literals added to the SymbolTable,
        ///  the SymbolTable location index of the token.
        /// </summary>
        public void PrintToken()
        {
            // TODO
            Console.WriteLine("Lexeme, TokenCode, 4 Char Mnemonic, SymbolTable Index");
        }

        public char GetNextChar()
        {
            return ' ';
        }

        public void SkipBlanks()
        {

        }

        public bool IsLetter()
        {
            return true;
        }

        public bool IsDigit()
        {
            return true;
        }

        public bool IsWhitespace()
        {
            return true;
        }
    }
}
