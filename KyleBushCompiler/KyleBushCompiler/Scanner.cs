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
        private const int IDENTIFIER = 50;
        private const int INTEGER = 51;
        private const int FLOATING_POINT = 52;
        private const int STRING = 53;
        private const int UNDEFINED = 99;

        public string NextToken { get; set; }
        public int TokenCode { get; set; }
        public SymbolTable SymbolTable { get; private set; }
        public ReserveTable ReserveTable { get; private set; }
        public bool EndOfFile { get; set; }
        public string[] FileText { get; set; }
        public string CurrentLine { get; set; }
        public char CurrentChar { get; set; }
        public int CurrentLineIndex { get; set; }
        public int CurrentCharIndex { get; set; }
        public bool TokenFound { get; set; }
        public bool EchoOn { get; set; }

        public void Initialize(string[] fileText, SymbolTable symbolTable, ReserveTable reserveTable)
        {
            SymbolTable = symbolTable;
            ReserveTable = reserveTable;
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
            while (!EndOfFile && !TokenFound)
            {
                SkipBlanks();
                // Check for single character comment identifier
                if (CurrentChar == '{')
                {
                    GetNextChar();
                    while (CurrentChar != '}') 
                    {
                        GetNextChar();
                    };
                    GetNextChar();
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
                        if (CurrentChar == ')')
                        {
                            GetNextChar();
                        }
                        else
                        {
                            Console.WriteLine("Invalid Character - Expected ')' to close comment.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Character - Expected '*' to begin comment.");
                    }
                }
                // Check if NUMERIC CONSTANT either INTEGER or FLOATING_POINT
                else if (IsDigit(CurrentChar))
                {
                    TokenFound = true;
                }
                // Check if IDENTIFIER
                else if (IsLetter(CurrentChar))
                {
                    while (!IsWhitespace(CurrentChar) && IsLetter(CurrentChar) || IsDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '$')
                    {
                        AddCharToNextToken();
                    }
                    TokenCode = GetIdentifierCode();
                    if (TokenCode == IDENTIFIER)
                    {
                        AddTokenToSymbolTable();
                    }
                    TokenFound = true;
                }
                // Check if STRING
                else if (CurrentChar == '"')
                {
                    GetNextChar();
                    while (CurrentChar != '"')
                    {
                        AddCharToNextToken();
                    }
                    TokenCode = STRING;
                    AddTokenToSymbolTable();
                    TokenFound = true;
                    GetNextChar();
                }
                else
                {
                    GetNextChar();
                }
            }
        }

        //private bool IsOtherIdentifier(char currentChar)
        //{
        //    ReserveTable
        //}

        /// <summary>
        /// Checks if the token is already in the symbol table.
        /// If it is not then it is added, otherwise it does nothing.
        /// </summary>
        private void AddTokenToSymbolTable()
        {
            int symbolIndex = SymbolTable.LookupSymbol(NextToken);
            if (symbolIndex == -1)
            {
                switch (TokenCode)
                {
                    case IDENTIFIER:
                        SymbolTable.AddSymbol(NextToken, SymbolKind.Variable, 0);
                        break;
                    case INTEGER:
                        SymbolTable.AddSymbol(NextToken, SymbolKind.Constant, Int32.Parse(NextToken));
                        break;
                    case FLOATING_POINT:
                        SymbolTable.AddSymbol(NextToken, SymbolKind.Constant, Double.Parse(NextToken));
                        break;
                    case STRING:
                        SymbolTable.AddSymbol(NextToken, SymbolKind.Constant, NextToken);
                        break;
                }
            }
        }

        private int GetIdentifierCode()
        {
            int code = ReserveTable.LookupName(NextToken);
            if (code == -1)
            {
                return IDENTIFIER;
            }

            return code;
        }

        private void AddCharToNextToken()
        {
            NextToken += CurrentChar;
            GetNextChar();
        }

        private void GetNextLine()
        {
            if (CurrentLineIndex <= FileText.Length)
            {
                if (CurrentCharIndex >= CurrentLine.Length || CurrentLineIndex == 0)
                {
                    do
                    {
                        CurrentLine = FileText[CurrentLineIndex];
                        CurrentLineIndex++;
                    } while (CurrentLine.Length == 0);
                             

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
            CurrentCharIndex++;

            if (CurrentCharIndex < CurrentLine.Length && CurrentLine.Length > 0)
            {
                CurrentChar = CurrentLine[CurrentCharIndex];
            }
            else
            {
                GetNextLine();
                if (!EndOfFile)
                {
                    CurrentCharIndex = 0;
                    CurrentChar = CurrentLine[CurrentCharIndex];
                }
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
            while (IsWhitespace(CurrentChar))
            {
                GetNextChar();
            }
        }

        public bool IsLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        public bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        public bool IsWhitespace(char c)
        {
            return char.IsWhiteSpace(c);
        }
    }
}
