using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace KyleBushCompiler
{
    class Scanner
    {
        enum State
        {
            START,
            INTEGER_START,
            INTEGER_ACCEPT,
            FLOATING_POINT_START,
            FLOATING_POINT_SCI_NOTATION,
            FLOATING_POINT_SCI_NOTATION_SIGN,
            FLOATING_POINT_SCI_NOTATION_DIGIT,
            FLOATING_POINT_FRACTIONAL_DIGIT,
            FLOATING_POINT_ACCEPT,
            IDENTIFIER_START,
            IDENTIFIER_ACCEPT,
            STRING_START,
            STRING_ACCEPT,
            COMMENT_2_START,
            COMMENT_2_BODY,
            COMMENT_2_CLOSE,
            COMMENT_1_BODY,
            ONE_OR_TWO_CHAR_TOKEN_ACCEPT,
            UNDEFINED
        }

        private State CurrentState;
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
            CurrentLine = FileText[CurrentLineIndex];
        }

        /// <summary>
        /// Uses DFA to select the next available Token
        /// </summary>
        /// <param name="echoOn"> selects whether input lines are echoed when read</param>
        public void GetNextToken(bool echoOn)
        {
            CurrentState = State.START;
            EchoOn = echoOn;
            NextToken = "";
            TokenFound = false;

            GetNextChar();

            while (!EndOfFile && !TokenFound)
            {
                // Check for single character comment identifier
                if (CurrentChar == '{')
                {
                    CommentStyleOne();
                }
                // Check for 2 character comment identifier
                else if (CurrentChar == '(' && LookAhead() == '*')
                {
                    CommentStyleTwo();
                }
                // Check for one or two char tokens
                else if (IsOneOrTwoCharTokenStart(CurrentChar))
                {
                    GetOneOrTwoCharToken(CurrentChar);

                }
                // Check if NUMERIC CONSTANT either INTEGER or FLOATING_POINT
                else if (IsDigit(CurrentChar))
                {
                    GetNumericToken();
                }
                // Check if IDENTIFIER
                else if (IsLetter(CurrentChar))
                {
                    GetIdentifierToken();
                }
                // Check if STRING
                else if (CurrentChar == '"')
                {
                    GetStringToken();
                }
                else
                {
                    AcceptToken(UNDEFINED, State.UNDEFINED);
                }
            }

            if (EndOfFile)
            {
                CheckForEndOfFileErrors();
            }
        }

        private void CheckForEndOfFileErrors()
        {
            switch (CurrentState)
            {
                case State.COMMENT_1_BODY:
                case State.COMMENT_2_BODY:
                    Console.WriteLine("WARNING: End of file found before comment terminated");
                    break;
                case State.STRING_START:
                    Console.WriteLine("WARNING: Unterminated string found");
                    break;
            }
        }

        private void GetStringToken()
        {
            CurrentState = State.STRING_START;
            GetNextChar();
            while (!EndOfFile || CurrentChar != '"')
            {
                AddCharToNextToken();
                GetNextChar();
            }
            AcceptToken(STRING, State.STRING_ACCEPT);
            AddTokenToSymbolTable();
        }

        private void GetIdentifierToken()
        {
            CurrentState = State.IDENTIFIER_START;
            while (!EndOfFile && !IsWhitespace(CurrentChar) && IsLetter(CurrentChar) || IsDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '$')
            {
                AddCharToNextToken();
                GetNextChar();
            }
            AcceptToken(GetIdentifierCode(), State.IDENTIFIER_ACCEPT);
            AddTokenToSymbolTable();
        }


        private void GetNumericToken()
        {
            CurrentState = State.INTEGER_START;
            while (!EndOfFile && IsDigit(CurrentChar))
            {
                AddCharToNextToken();
                GetNextChar();
            }
            if (CurrentChar == '.')
            {
                GenerateFloatingPointToken();
            }
            else
            {
                AcceptToken(INTEGER, State.INTEGER_ACCEPT);
            }
        }

        private void GenerateFloatingPointToken()
        {
            CurrentState = State.FLOATING_POINT_START;
            GetNextChar();
            if (IsDigit(CurrentChar))
            {
                while (!EndOfFile && IsDigit(CurrentChar))
                {
                    AddCharToNextToken();
                    GetNextChar();
                }
                if (CurrentChar == 'E')
                {
                    GenerateFloatingPointScientificNotationToken();
                }
            }
            else if (CurrentChar == 'E')
            {
                GenerateFloatingPointScientificNotationToken();
            }
            else
            {
                AcceptToken(TokenCode, State.FLOATING_POINT_ACCEPT);
            }
        }

        private void GenerateFloatingPointScientificNotationToken()
        {
            CurrentState = State.FLOATING_POINT_SCI_NOTATION;
            GetNextChar();
            if (CurrentChar == '-' || CurrentChar == '+')
            {
                CurrentState = State.FLOATING_POINT_SCI_NOTATION_SIGN;
                GetNextChar();
                if (IsDigit(CurrentChar))
                {
                    CurrentState = State.FLOATING_POINT_SCI_NOTATION_DIGIT;
                    while (!EndOfFile && IsDigit(CurrentChar))
                    {
                        AddCharToNextToken();
                        GetNextChar();
                    }
                    AcceptToken(TokenCode, State.FLOATING_POINT_ACCEPT);
                }
                else
                {
                    Console.WriteLine("ERROR: Expected at least one digit.");
                }
            }
            else
            {
                Console.WriteLine("ERROR: Expected + or - character.");
            }
        }

        private void AcceptToken(int tokenCode, State state)
        {
            TokenFound = true;
            CurrentState = state;
            TokenCode = tokenCode;
            TruncateTokenIfTooLong();
        }

        private void CommentStyleTwo()
        {
            CurrentState = State.COMMENT_2_BODY;
            GetNextChar();
            GetNextChar();

            while (!EndOfFile && CurrentChar != '*' && LookAhead() != ')')
            {
                GetNextChar();
            };

            GetNextChar();
            GetNextChar();

            CurrentState = State.START;   
        }

        /// <summary>
        /// The token is a comment so all characters are ignored until a '}' is found to close the comment
        /// </summary>
        private void CommentStyleOne()
        {
            CurrentState = State.COMMENT_1_BODY;
            while (!EndOfFile && CurrentChar != '}')
            {
                GetNextChar();
            };
        }

        private void TruncateTokenIfTooLong()
        {
            if (NextToken.Length > MAXLENGTH)
            {
                Console.WriteLine("Token length exceeds " + MAXLENGTH + ". Token has been truncated.");
                NextToken = NextToken.Substring(0, MAXLENGTH);
            } 
        }

        private bool IsOneOrTwoCharTokenStart(char c)
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
                case ':':
                case '>':
                case '<':
                    return true;
                default:
                    return false;
            }
        }

        private void GetOneOrTwoCharToken(char c)
        {
            switch (c)
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
            }
            AcceptToken(ReserveTable.LookupName(NextToken), State.ONE_OR_TWO_CHAR_TOKEN_ACCEPT);
        }

        private char LookAhead()
        {
            char lookAhead = ' ';
            if (CurrentCharIndex < CurrentLine.Length)
            {
                lookAhead = CurrentLine[CurrentCharIndex];
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
            int code = ReserveTable.LookupName(NextToken.ToUpper());
            if (code == -1)
            {
                return IDENTIFIER;
            }

            return code;
        }

        private void AddCharToNextToken()
        {
            NextToken += CurrentChar;
        }

        private void GetNextLine()
        {
            do
            {
                CurrentLine = FileText[CurrentLineIndex];
                CurrentLineIndex++;
                if (EchoOn)
                {
                    Console.WriteLine(CurrentLine);
                }
            } while (String.IsNullOrWhiteSpace(CurrentLine) || String.IsNullOrEmpty(CurrentLine));      
        }

        private void GetNextChar()
        {
            if (IsEndOfFile())
            {
                EndOfFile = true;
                return;
            }
            if (IsEndOfLine())
            {
                GetNextLine();
                CurrentCharIndex = 0;
            }

            CurrentChar = CurrentLine[CurrentCharIndex];
            CurrentCharIndex++;

            SkipBlanks();
        }

        private void SkipBlanks()
        {
            while (!EndOfFile && IsWhitespace(CurrentChar))
            {
                CurrentCharIndex++;
                GetNextChar();
            }
        }

        private bool IsEndOfFile()
        {
            return (CurrentLineIndex == FileText.Length && CurrentCharIndex == CurrentLine.Length);
        }

        private bool IsEndOfLine()
        {
            return (CurrentLine.Length == 0 || CurrentCharIndex == CurrentLine.Length);
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
