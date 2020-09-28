using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace KyleBushCompiler
{
    class Scanner
    {
        public string NextToken { get; set; }
        public int TokenCode { get; set; }
        public bool EndOfFile { get; set; }
        public string[] FileText { get; set; }
        public string CurrentLine { get; set; }
        public char CurrentChar { get; set; }
        public int CurrentLineIndex { get; set; }
        public int CurrentCharIndex { get; set; }
        public bool TokenFound { get; set; }
        public bool EchoOn { get; set; }

        public void Initialize(string[] fileText)
        {
            EndOfFile = false;
            EchoOn = false;
            FileText = fileText;
            CurrentLineIndex = 0;
            CurrentCharIndex = 0;
            CurrentLine = FileText[CurrentLineIndex];
            CurrentChar = CurrentLine[CurrentCharIndex];
        }

        /// <summary>
        /// Uses DFA to select the next available Token
        /// </summary>
        /// <param name="echoOn"> selects whether input lines are echoed when read</param>
        public void GetNextToken(bool echoOn)
        {
            EchoOn = echoOn;
            NextToken = "";
            TokenFound = false;
            GetNextLine();
            while (!EndOfFile && !TokenFound)
            {
                GetNextChar();
                // Check for single character comment identifier
                if (CurrentChar == '{')
                {
                    GetNextChar();
                    while (CurrentChar != '}') 
                    {
                        GetNextChar();
                    };
                }
                // Check for 2 character comment identifier
                else if (CurrentChar == '(')
                {
                    GetNextChar();
                    if (CurrentChar == '*')
                    {
                        GetNextChar();
                        while (CurrentChar != '*') 
                        {
                            GetNextChar();
                        };
                        GetNextChar();
                        if (CurrentChar != ')')
                        {
                            Console.WriteLine("Invalid Character - Expected ')' to close comment.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Character - Expected '*' to begin comment.");
                    }
                }
                else if (char.IsDigit(CurrentChar))
                {
                    TokenFound = true;
                }
                else if (char.IsLetter(CurrentChar))
                {
                    while (!char.IsWhiteSpace(CurrentChar) || char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '$')
                    {
                        NextToken += CurrentChar;
                        GetNextChar();
                    }
                    TokenCode = 50;
                    TokenFound = true;
                }
                else if (CurrentChar == '"')
                {

                }
                else
                {

                }
            }
        }

        private void GetNextLine()
        {
            if (CurrentLineIndex <= FileText.Length)
            {
                if (CurrentCharIndex >= CurrentLine.Length || CurrentLineIndex == 0)
                {
                    CurrentLine = FileText[CurrentLineIndex];
                    CurrentLineIndex++;

                    if (EchoOn)
                    {
                        Console.WriteLine(CurrentLine);
                    }
                }
            }
            else
            {
                EndOfFile = true;
            }   
        }

        public void GetNextChar()
        {
            if (CurrentCharIndex < CurrentLine.Length)
            {
                CurrentChar = CurrentLine[CurrentCharIndex];
                CurrentCharIndex++;
            }
            else
            {
                GetNextLine();
                CurrentCharIndex = 0;
                CurrentChar = CurrentLine[CurrentCharIndex];
                CurrentCharIndex++;
            }

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
