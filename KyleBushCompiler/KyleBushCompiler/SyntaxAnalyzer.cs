using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KyleBushCompiler
{
    class SyntaxAnalyzer
    {
        #region Token Constants
        private const int GOTO = 0;
        private const int INTEGER = 1;
        private const int TO = 2;
        private const int DO = 3;
        private const int IF = 4;
        private const int THEN = 5;
        private const int ELSE = 6;
        private const int FOR = 7;
        private const int OF = 8;
        private const int WRITELN = 9;
        private const int READLN = 10;
        private const int BEGIN = 11;
        private const int END = 12;
        private const int VAR = 13;
        private const int WHILE = 14;
        private const int UNIT = 15;
        private const int LABEL = 16;
        private const int REPEAT = 17;
        private const int UNTIL = 18;
        private const int PROCEDURE = 19;
        private const int DOWNTO = 20;
        private const int FUNCTION = 21;
        private const int RETURN = 22;
        private const int REAL = 23;
        private const int STRING = 24;
        private const int ARRAY = 25;
        private const int DIVIDE = 30;
        private const int MULTIPLY = 31;
        private const int PLUS = 32;
        private const int MINUS = 33;
        private const int LPAR = 34;
        private const int RPAR = 35;
        private const int SEMICOLON = 36;
        private const int ASSIGN = 37;
        private const int GREATER_THAN = 38;
        private const int LESS_THAN = 39;
        private const int GREATER_THAN_OR_EQUAL = 40;
        private const int LESS_THAN_OR_EQUAL = 41;
        private const int EQUAL = 42;
        private const int NOT_EQUAL = 43;
        private const int COMMA = 44;
        private const int LEFT_BRACKET = 45;
        private const int RIGHT_BRACKET = 46;
        private const int COLON = 47;
        private const int PERIOD = 48;
        private const int IDENTIFIER = 50;
        private const int INTTYPE = 51;
        private const int FLOAT = 52;
        private const int STRINGTYPE = 53;
        private const int UNDEFINED = 99;
        #endregion

        #region Properties
        public bool TraceOn { get; set; }
        public bool IsError { get; set; }
        public bool ErrorOcurred { get; set; }
        private bool PrintError { get; set; }
        private LexicalAnalyzer Scanner { get; set; }
        private ReserveTable TokenCodes { get; set; }
        private bool ScannerEchoOn { get; set; }
        private bool Verbose { get; set; }

        #endregion

        public SyntaxAnalyzer(LexicalAnalyzer scanner, ReserveTable tokenCodes, bool scannerEchoOn)
        {
            Scanner = scanner;
            ScannerEchoOn = scannerEchoOn;
            TokenCodes = tokenCodes;
        }

        #region CFG Methods

        /// <summary>
        /// Implements CFG Rule: <program> -> $UNIT <prog-identifier> $SEMICOLON <block> $PERIOD
        /// </summary>
        /// <returns></returns>
        public int Program()
        {
            if (IsError)
                return -1;

            Debug(true, "Program()");

            if (Scanner.TokenCode == UNIT)
            {
                GetNextToken();
                int x = ProgIdentifier();
                if (Scanner.TokenCode == SEMICOLON)
                {
                    GetNextToken();
                    x = Block();
                    if (Scanner.TokenCode == PERIOD)
                    {
                        GetNextToken();
                    }
                    else
                    {
                        Error("PERIOD");
                    }
                }
                else
                {
                    Error("SEMICOLON");
                }
            }
            else
            {
                Error("UNIT");
            }

            Debug(false, "Program()");
            return -1;
        }


        /// <summary>
        /// Implements CFG Rule: <block> -> [<label-declaration>] {<variable-dec-sec>}* <block-body>
        /// Also contains main error handling logic.
        /// </summary>
        /// <returns></returns>
        private int Block()
        {
            if (IsError)
                return -1;

            Debug(true, "Block()");
            if (Scanner.TokenCode == LABEL)
            {
                GetNextToken();
                LabelDeclaration();
            }

            while (Scanner.TokenCode == VAR && !IsError)
            {
                GetNextToken();
                VariableDecSec();
            }

            BlockBody();

            // Error handling and resyncing
            while (IsError == true && !Scanner.EndOfFile)
            {
                Resync();
                IsError = false;
                while (IsError == false && !Scanner.EndOfFile)
                {
                    Statement();
                }
            }

            Debug(false, "Block()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <block-body> -> $BEGIN <statement> {$SEMICOLON <statement>}* $END
        /// </summary>
        /// <returns></returns>
        private int BlockBody()
        {
            if (IsError)
                return -1;

            Debug(true, "BlockBody()");
            if (Scanner.TokenCode == BEGIN)
            {
                GetNextToken();
                int x = Statement();
                while (Scanner.TokenCode == SEMICOLON && !IsError)
                {
                    GetNextToken();
                    x = Statement();
                }

                if (Scanner.TokenCode == END)
                    GetNextToken();
                else
                    Error("END");
            }
            else
                Error("BEGIN");

            Debug(false, "BlockBody()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <label-declaration> -> $LABEL <identifier> {$COMMA <identifier>}* $SEMICOLON
        /// </summary>
        /// <returns></returns>
        private int LabelDeclaration()
        {
            if (IsError)
                return -1;

            Debug(true, "LabelDeclaration()");
            if (Scanner.TokenCode == LABEL)
            {
                GetNextToken();
                int x = Identifier();
                while (Scanner.TokenCode == COMMA && !IsError)
                {
                    GetNextToken();
                    x = Identifier();
                }

                if (Scanner.TokenCode == SEMICOLON)
                    GetNextToken();
                else
                    Error("SEMICOLON");
            }
            else
                Error("LABEL");

            Debug(false, "LabelDeclaration()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <variable-dec-sec> -> $VAR <variable-declaration>
        /// </summary>
        /// <returns></returns>
        private int VariableDecSec()
        {
            if (IsError)
                return -1;

            Debug(true, "VariableDecSec()");
            if (Scanner.TokenCode == VAR)
            {
                GetNextToken();
                int x = VariableDeclaration();
            }
            else
                Error("VAR");

            Debug(false, "VariableDecSec()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <variable-declaration> -> {<identifier> {$COMMA <identifier>}* $COLON <type> $SEMICOLON}+
        /// </summary>
        /// <returns></returns>
        private int VariableDeclaration()
        {
            if (IsError)
                return -1;

            Debug(true, "VariableDeclaration()");
            do
            {
                int x = Identifier();
                while (Scanner.TokenCode == COMMA && !IsError)
                {
                    GetNextToken();
                    x = Identifier();
                }
                if (Scanner.TokenCode == COLON)
                {
                    GetNextToken();
                    x = Type();
                    if (Scanner.TokenCode == SEMICOLON)
                    {
                        GetNextToken();
                    }
                    else
                    {
                        Error("SEMICOLON");
                    }
                }
                else
                {
                    Error("COMMA");
                }
            } while (Scanner.TokenCode == IDENTIFIER && !IsError);

            Debug(false, "VariableDeclaration()");
            return -1;
        }


        /// <summary>
        /// Implements CFG Rule: <statement>-> {<label> $COLON]}*
        ///                                    [
        ///                                        <variable> $ASSIGN (<simple expression> | <string literal>) |
        ///                                        <block-body> |
        ///                                        $IF <relexpression> $THEN <statement> [$ELSE <statement>] |
        ///                                        $WHILE <relexpression> $DO <statement> |
        ///                                        $REPEAT <statement> $UNTIL <relexpression> |
        ///                                        $FOR <variable> $ASSIGN <simple expression> $TO <simple expression> $DO <statement> |
        ///                                        $GOTO <label> |
        ///                                        $WRITELN $LPAR (<simple expression> | <identifier> | <stringconst>) $RPAR
        ///                                    ]+
        /// </summary>
        /// <returns></returns>
        private int Statement()
        {
            if (IsError)
                return -1;

            Debug(true, "Statement()");
            while (IsLabel() && !IsError)
            {
                int x = Label();
                if (Scanner.TokenCode == COLON)
                    GetNextToken();
            }
            if (Scanner.TokenCode == IDENTIFIER)
            {
                GetNextToken();
                Variable();
                if (Scanner.TokenCode == ASSIGN)
                {
                    GetNextToken();
                    if (IsSimpleExpression())
                        SimpleExpression();
                    else if (Scanner.TokenCode == STRINGTYPE)
                        StringConst();
                    else
                        Error("SIMPLE EXPRESSION or STRING");
                }
                else if (Scanner.TokenCode == BEGIN)
                {
                    BlockBody();
                }
                else if (Scanner.TokenCode == IF)
                {
                    GetNextToken();
                    RelExpression();
                    if (Scanner.TokenCode == THEN)
                    {
                        GetNextToken();
                        Statement();
                        if (Scanner.TokenCode == ELSE)
                        {
                            GetNextToken();
                            Statement();
                        }
                    }
                    else
                        Error("THEN");
                }
                else if (Scanner.TokenCode == WHILE)
                {
                    GetNextToken();
                    RelExpression();
                    if (Scanner.TokenCode == DO)
                    {
                        GetNextToken();
                        Statement();
                    }
                    else
                        Error("DO");
                }
                else if (Scanner.TokenCode == REPEAT)
                {
                    GetNextToken();
                    Statement();
                    if (Scanner.TokenCode == UNTIL)
                    {
                        GetNextToken();
                        RelExpression();
                    }
                    else
                        Error("UNTIL");
                }
                else if (Scanner.TokenCode == FOR)
                {
                    GetNextToken();
                    Variable();
                    if (Scanner.TokenCode == ASSIGN)
                    {
                        GetNextToken();
                        SimpleExpression();
                        if (Scanner.TokenCode == TO)
                        {
                            GetNextToken();
                            SimpleExpression();
                            if (Scanner.TokenCode == DO)
                            {
                                GetNextToken();
                                Statement();
                            }
                            else
                                Error("DO");
                        }
                        else
                            Error("TO");
                    }
                    else
                        Error("ASSIGN");
                }
                else if (Scanner.TokenCode == GOTO)
                {
                    GetNextToken();
                    Label();
                }
                else if (Scanner.TokenCode == WRITELN)
                {
                    GetNextToken();
                    if (Scanner.TokenCode == LPAR)
                    {
                        GetNextToken();
                        if (IsSimpleExpression())
                        {
                            SimpleExpression();
                            if (Scanner.TokenCode == RPAR)
                                GetNextToken();
                            else
                                Error("RPAR");
                        }
                        else if (Scanner.TokenCode == IDENTIFIER)
                        {
                            Identifier();
                            if (Scanner.TokenCode == RPAR)
                                GetNextToken();
                            else
                                Error("RPAR");
                        }
                        else if (Scanner.TokenCode == STRINGTYPE)
                        {
                            StringConst();
                            if (Scanner.TokenCode == RPAR)
                                GetNextToken();
                            else
                                Error("RPAR");
                        }
                        else
                            Error("SimpleExpression or IDENTIFIER or STRINGTYPE");
                    }
                    else
                        Error("LPAR");
                }
                else
                    Error("Statement Token");
            }

            Debug(false, "Statement()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <prog-identifier> -> <identifier>
        /// </summary>
        /// <returns></returns>
        private int ProgIdentifier()
        {
            if (IsError)
                return -1;

            Debug(true, "ProgIdentifier()");
            Identifier();
            Debug(false, "ProgIdentifier()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <variable> -> <identifier> [$LEFT_BRACKET <simple expression> $RIGHT_BRACKET]
        /// </summary>
        /// <returns></returns>
        private int Variable()
        {
            if (IsError)
                return -1;

            Debug(true, "Variable()");
            Identifier();

            if (Scanner.TokenCode == LEFT_BRACKET)
            {
                GetNextToken();
                SimpleExpression();
                if (Scanner.TokenCode == RIGHT_BRACKET)
                    GetNextToken();
                else
                    Error("RIGHT_BRACKET");
            }

            Debug(false, "Variable()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <label> -> <identifier>
        /// </summary>
        /// <returns></returns>
        private int Label()
        {
            if (IsError)
                return -1;

            Debug(true, "Label()");
            // Must check that the indentifier has been declared as type label 
            if (IsLabel())
                Identifier();
            else
                Error("LABEL");
                
            Debug(false, "Label()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <relexpression> -> <simple expression> <relop> <simple expression>
        /// </summary>
        /// <returns></returns>
        private int RelExpression()
        {
            if (IsError)
                return -1;

            Debug(true, "Label()");
            SimpleExpression();
            RelOp();
            SimpleExpression();
            Debug(false, "Label()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <relop> -> $EQ | $LSS | $GTR | $NEQ | $LEQ | $GEQ
        /// </summary>
        /// <returns></returns>
        private int RelOp()
        {
            if (IsError)
                return -1;

            Debug(true, "Label()");
            switch (Scanner.TokenCode)
            {
                case EQUAL:
                case LESS_THAN:
                case GREATER_THAN:
                case LESS_THAN_OR_EQUAL:
                case GREATER_THAN_OR_EQUAL:
                case NOT_EQUAL:
                    GetNextToken();
                    break;
                default:
                    Error("Relational Operator");
                    break;
            }
            Debug(false, "Label()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <simple expression> -> [<sign>] <term> {<addop> <term>}*
        /// </summary>
        /// <returns></returns>
        private int SimpleExpression()
        {
            if (IsError)
                return -1;

            Debug(true, "SimpleExpression()");

            int x;

            if (isSign())
            {
                x = Sign();
            }

            x = Term();

            while (isAddOp() && !IsError)
            {
                x = AddOp();
                x = Term();
            }

            Debug(false, "SimpleExpression()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <addop> -> $PLUS | $MINUS
        /// </summary>
        /// <returns></returns>
        private int AddOp()
        {
            if (IsError)
                return -1;

            Debug(true, "AddOp()");
            if (Scanner.TokenCode == PLUS || Scanner.TokenCode == MINUS)
                GetNextToken();
            else
                Error("PLUS or MINUS");
            Debug(false, "AddOp()");
            return -1;
        }



        /// <summary>
        /// Implements CFG Rule: <sign> -> $PLUS | $MINUS
        /// </summary>
        /// <returns></returns>
        private int Sign()
        {
            if (IsError)
                return -1;

            Debug(true, "Sign()");
            if (Scanner.TokenCode == PLUS)
                GetNextToken();
            else if (Scanner.TokenCode == MINUS)
                GetNextToken();
            else
                Error("PLUS or MINUS");
            Debug(false, "Sign()");
            return -1;
        }


        /// <summary>
        /// Implements CFG Rule: <term> -> <factor> {<mulop> <factor> }*
        /// </summary>
        /// <returns></returns>
        private int Term()
        {
            if (IsError)
                return -1;

            Debug(true, "Term()");
            int x = Factor();

            while (isMulOp() && !IsError)
            {
                x = MulOp();
                x = Factor();
            }

            Debug(false, "Term()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <mulop> -> $MULTIPLY | $DIVIDE
        /// </summary>
        /// <returns></returns>
        private int MulOp()
        {
            if (IsError)
                return -1;

            Debug(true, "MulOp()");

            if (Scanner.TokenCode == MULTIPLY || Scanner.TokenCode == DIVIDE)
                GetNextToken();
            else
                Error("MULTIPLY or DIVIDE");

            Debug(false, "MulOp()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <factor> -> <unsigned constant> | <variable> | $LPAR <simple expression> $RPAR
        /// </summary>
        /// <returns></returns>
        private int Factor()
        {
            if (IsError)
                return -1;

            Debug(true, "Factor()");

            int x;

            if (isUnsignedConstant())
            {
                x = UnsignedConstant();
            }
            else if (isVariable())
            {
                Variable();
            }
            else if (Scanner.TokenCode == LPAR)
            {
                GetNextToken();
                SimpleExpression();
                if (Scanner.TokenCode == RPAR)
                    GetNextToken();
                else
                    Error("RPAR");
            }
            else
                Error("UNSIGNED CONSTANT or VARIABLE or LPAR");

            Debug(false, "Factor()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <type> -> <simple type> | $ARRAY $LBRACK $INTTYPE $RBRACK $OF $INTEGER
        /// </summary>
        /// <returns></returns>
        private int Type()
        {
            if (IsError)
                return -1;

            Debug(true, "Type()");
            if (IsSimpleType())
                SimpleType();
            else if (Scanner.TokenCode == ARRAY)
            {
                GetNextToken();
                if (Scanner.TokenCode == LEFT_BRACKET)
                {
                    GetNextToken();
                    if (Scanner.TokenCode == INTTYPE)
                    {
                        GetNextToken();
                        if (Scanner.TokenCode == RIGHT_BRACKET)
                        {
                            GetNextToken();
                            if (Scanner.TokenCode == OF)
                            {
                                GetNextToken();
                                if (Scanner.TokenCode == INTEGER)
                                {
                                    GetNextToken();
                                }
                                else
                                    Error("INTEGER");
                            }
                            else
                                Error("OF");
                        }
                        else
                            Error("RIGHT_BRACKET");
                    }
                    else
                        Error("INTTYPE");
                }
                else
                    Error("LEFT_BRACKET");
            }
            else
                Error("Simple Type or ARRAY");

            Debug(false, "Type()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <simple type> -> $INTEGER | $FLOAT | $STRING
        /// </summary>
        /// <returns></returns>
        private int SimpleType()
        {
            if (IsError)
                return -1;

            Debug(true, "SimpleType()");

            if (Scanner.TokenCode == INTEGER || Scanner.TokenCode == FLOAT || Scanner.TokenCode == STRING)
                GetNextToken();
            else
                Error("INTEGER or FLOAT or STRING");

            Debug(false, "SimpleType()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <constant> -> [<sign>] <unsigned constant>
        /// </summary>
        /// <returns></returns>
        private int Constant()
        {
            if (IsError)
                return -1;

            Debug(true, "Constant()");

            if (isSign())
                Sign();
            UnsignedConstant();

            Debug(false, "Constant()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <unsigned constant>-> <unsigned number>
        /// </summary>
        /// <returns></returns>
        private int UnsignedConstant()
        {
            if (IsError)
                return -1;

            Debug(true, "UnsignedConstant()");
            UnsignedNumber();
            Debug(false, "UnsignedConstant()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <unsigned number>-> $FLOAT | $INTTYPE
        /// </summary>
        /// <returns></returns>
        private int UnsignedNumber()
        {
            if (IsError)
                return -1;

            Debug(true, "UnsignedNumber()");

            if (Scanner.TokenCode == FLOAT || Scanner.TokenCode == INTTYPE)
                GetNextToken();
            else
                Error("FLOAT or INTTYPE");

            Debug(false, "UnsignedNumber()");
            return -1;
        }

        

        /// <summary>
        /// Implements CFG Rule: <identifier> -> $IDENTIFIER
        /// </summary>
        /// <returns></returns>
        private int Identifier()
        {
            if (IsError)
                return -1;

            Debug(true, "Identifier()");

            if (Scanner.TokenCode == IDENTIFIER)
                GetNextToken();
            else
                Error("IDENTIFIER");

            Debug(false, "Identifier()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <stringconst> -> $STRINGTYPE
        /// </summary>
        /// <returns></returns>
        private int StringConst()
        {
            if (IsError)
                return -1;

            Debug(true, "StringConst()");

            if (Scanner.TokenCode == STRINGTYPE)
                GetNextToken();
            else
                Error("STRINGYPE");

            Debug(false, "StringConst()");
            return -1;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Prints an error with the expected token type and the actual token found.
        /// </summary>
        /// <param name="expectedToken">The expected token type.</param>
        private void Error(string expectedToken)
        {
            IsError = true;
            Console.WriteLine("Line #{0}: {1}", Scanner.CurrentLineIndex + 1, Scanner.CurrentLine);
            Console.WriteLine("ERROR: {0} expected, but {1} found.", expectedToken, Scanner.NextToken);
        }
        

        /// <summary>
        /// Prints the method that is being entered or exited if TraceOn is set to true
        /// </summary>
        /// <param name="entering"></param>
        /// <param name="name"></param>
        private void Debug(bool entering, string name)
        {
            if (TraceOn)
            {
                if (entering)
                    Console.WriteLine("ENTERING " + name);
                else
                    Console.WriteLine("EXITING " + name);
            }
        }

        /// <summary>
        /// Gets the next token and prints the token lexeme and mneumonic if Trace is on.
        /// </summary>
        private void GetNextToken()
        {
            Scanner.GetNextToken(ScannerEchoOn);
            if (TraceOn)
                Console.WriteLine("Lexeme: {0} Mnemonic: {1}", Scanner.NextToken, TokenCodes.LookupCode(Scanner.TokenCode));
        }

        /// <summary>
        /// After an error occurs this finds the begining of the next statement.
        /// </summary>
        private void Resync()
        {
            while(!IsStatementStart() && !Scanner.EndOfFile)
            {
                GetNextToken();
            }
        }

        /// <summary>
        /// Checks if the next token is a Sign token.
        /// </summary>
        /// <returns></returns>
        private bool isSign()
        {
            if (Scanner.TokenCode == PLUS || Scanner.TokenCode == MINUS)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the next token is an AddOp token.
        /// </summary>
        /// <returns></returns>
        private bool isAddOp()
        {
            if (Scanner.TokenCode == PLUS || Scanner.TokenCode == MINUS)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the next token is a MulOp token.
        /// </summary>
        /// <returns></returns>
        private bool isMulOp()
        {
            if (Scanner.TokenCode == MULTIPLY || Scanner.TokenCode == DIVIDE)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the next token is a Variable
        /// </summary>
        /// <returns></returns>
        private bool isVariable()
        {
            if (Scanner.TokenCode == IDENTIFIER)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the next token is an Unsigned Constant
        /// </summary>
        /// <returns></returns>
        private bool isUnsignedConstant()
        {
            if (Scanner.TokenCode == FLOAT || Scanner.TokenCode == INTTYPE)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines if the current token could be the start of a statement.
        /// </summary>
        /// <returns>True if the token could start a statement, False if not.</returns>
        private bool IsStatementStart()
        {
            switch(Scanner.TokenCode)
            {
                case IDENTIFIER:
                case BEGIN:
                case IF:
                case WHILE:
                case REPEAT:
                case FOR:
                case GOTO:
                case WRITELN:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if the current token is the start of a simple expression.
        /// </summary>
        /// <returns></returns>
        private bool IsSimpleExpression()
        {
            if (isSign() || isUnsignedConstant() || isVariable() || Scanner.TokenCode == LPAR)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines if the current token is a simple type keyword.
        /// </summary>
        /// <returns></returns>
        private bool IsSimpleType()
        {
            switch (Scanner.TokenCode)
            {
                case INTEGER:
                case REAL:
                case STRING:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if the current token is a label.
        /// </summary>
        /// <returns></returns>
        private bool IsLabel()
        {
            if (Scanner.TokenCode == IDENTIFIER)
            {
                int labelIndex = Scanner.SymbolTable.LookupSymbol(Scanner.NextToken);
                if (labelIndex != -1)
                {
                    Symbol labelSymbol = Scanner.SymbolTable.GetSymbol(labelIndex);

                    if (labelSymbol.Kind == SymbolKind.Label)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
