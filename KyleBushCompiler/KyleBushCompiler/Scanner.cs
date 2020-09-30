using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private const int MAXLENGTH = 30;

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
            GetNextLine();
            CurrentChar = CurrentLine[CurrentCharIndex];
        }

        /// <summary>
        /// Uses DFA to select the next available Token
        /// </summary>
        /// <param name="echoOn"> selects whether input lines are echoed when read</param>
        public void GetNextToken(bool echoOn)
        {
            SkipBlanks();
            EchoOn = echoOn;
            NextToken = "";
            TokenFound = false;
            while (!EndOfFile && !TokenFound)
            {
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
                    if (LookAhead() == '*')
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
                    // Found single character token '('
                    else
                    {
                        NextToken += CurrentChar;
                        TokenFound = true;
                    }
                }
                // Check if NUMERIC CONSTANT either INTEGER or FLOATING_POINT
                else if (IsDigit(CurrentChar))
                {
                    TokenCode = INTEGER;
                    while (IsDigit(CurrentChar))
                    {
                        NextToken += CurrentChar;
                        GetNextChar();
                    }
                    if (CurrentChar == '.')
                    {
                        TokenCode = FLOATING_POINT;
                        GetNextChar();
                        if (IsDigit(CurrentChar))
                        {
                            while (IsDigit(CurrentChar))
                            {
                                NextToken += CurrentChar;
                                GetNextChar();
                            }
                        } 
                        else if (CurrentChar == 'E')
                        {

                        }
                    }
                    TruncateToken();
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
                        TruncateToken();
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
                    TruncateToken();
                    AddTokenToSymbolTable();
                    TokenFound = true;
                    GetNextChar();
                }
                else if (IsOtherToken(CurrentChar))
                {
                    TokenFound = true;
                    TokenCode = ReserveTable.LookupName(NextToken);
                    GetNextChar();
                }
                else
                {
                    NextToken += CurrentChar;
                    TokenCode = UNDEFINED;
                    TokenFound = true;
                    GetNextChar();
                }
            }
        }

        private void TruncateToken()
        {
            if (NextToken.Length > MAXLENGTH)
            {
                return NextToken.Substring(0, MAXLENGTH);
            } 
        }

        private bool IsOtherToken(char c)
        {
            switch(c)
            {
                case '/':
                case '*':
                case '+':
                case '-':
                case '(':
                case ')':
                case ';':
                case '=':
                case ',':
                case '[':
                case ']':
                case '.':
                    NextToken += CurrentChar;
                    break;
                case ':':
                    if (LookAhead() == '=')
                    {
                        NextToken += CurrentChar;
                        GetNextChar();
                        NextToken += CurrentChar;
                    }
                    else
                    {
                        NextToken += CurrentChar;
                    }
                    break;
                case '>':
                    if (LookAhead() == '=')
                    {
                        NextToken += CurrentChar;
                        GetNextChar();
                        NextToken += CurrentChar;
                    }
                    else
                    {
                        NextToken += CurrentChar;
                    }
                    break;
                case '<':
                    if (LookAhead() == '=' || LookAhead() == '>')
                    {
                        NextToken += CurrentChar;
                        GetNextChar();
                        NextToken += CurrentChar;
                    }
                    else
                    {
                        NextToken += CurrentChar;
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }

        private char LookAhead()
        {
            char lookAhead = ' ';
            if (CurrentCharIndex + 1 < CurrentLine.Length)
            {
                lookAhead = CurrentLine[CurrentCharIndex + 1];
            }
            return lookAhead;
        }

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
            if (CurrentLineIndex < FileText.Length)
            {
                if (CurrentLineIndex == 0 || CurrentCharIndex >= CurrentLine.Length)
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

        private void GetNextChar()
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


        private void SkipBlanks()
        {
            while (IsWhitespace(CurrentChar))
            {
                GetNextChar();
            }
        }

        private bool IsLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        private bool IsWhitespace(char c)
        {
            return char.IsWhiteSpace(c);
        }
    }
}
