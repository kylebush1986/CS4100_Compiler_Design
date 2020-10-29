using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KyleBushCompiler
{
    class SyntaxAnalyzer
    {
        #region Properties
        public bool TraceOn { get; set; }
        public bool IsError { get; set; }
        private LexicalAnalyzer Scanner { get; set; }
        private bool ScannerEchoOn { get; set; }

        #endregion

        public SyntaxAnalyzer(LexicalAnalyzer scanner, bool scannerEchoOn)
        {
            Scanner = scanner;
            ScannerEchoOn = scannerEchoOn;
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

            Debug(true, "Program()")

            if (Scanner.TokenCode == UNIT)
            {
                Scanner.GetNextToken(ScannerEchoOn);
                int x = ProgIdentifier();
                if (Scanner.TokenCode == SEMICOLON)
                {
                    Scanner.GetNextToken(ScannerEchoOn);
                    x = Block();
                    if (Scanner.TokenCode == PERIOD)
                    {
                        Scanner.GetNextToken(ScannerEchoOn);
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
        /// Implements CFG Rule: <block> -> $BEGIN <statement> {$SEMICOLON <statement>}* $END
        /// </summary>
        /// <returns></returns>
        private int Block()
        {
            if (IsError)
                return -1;

            Debug(true, "Block()");
            if (Scanner.TokenCode == BEGIN)
            {
                Scanner.GetNextToken(ScannerEchoOn);
                int x = Statement();
                while (Scanner.TokenCode == SEMICOLON)
                {
                    Scanner.GetNextToken(ScannerEchoOn);
                    x = Statement();
                }

                if (Scanner.TokenCode == END)
                    Scanner.GetNextToken(ScannerEchoOn);
                else
                    Error("END");
            }
            else
                Error("BEGIN");

            Debug(false, "Block()");
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
        /// Implements CFG Rule: <statement> -> <variable> $COLON-EQUALS <simple expression>
        /// </summary>
        /// <returns></returns>
        private int Statement()
        {
            if (IsError)
                return -1;

            Debug(true, "Statement()");
            int x = Variable();
            if (Scanner.TokenCode == COLON-EQUALS)
            {
                Scanner.GetNextToken(ScannerEchoOn);
                x = SimpleExpression();
            }
            else
                Error("COLON-EQUALS")

            Debug(false, "Statement()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <variable> -> <identifier>
        /// </summary>
        /// <returns></returns>
        private int Variable()
        {
            if (IsError)
                return -1;

            Debug(true, "Variable()");
            Identifier();
            Debug(false, "Variable()");
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

            while (isAddOp())
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
                Scanner.GetNextToken(ScannerEchoOn);
            else
                Error("PLUS or MINUS");
            Debug(false, "AddOp()");
            return -1;
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
        /// Implements CFG Rule: <sign> -> $PLUS | $MINUS
        /// </summary>
        /// <returns></returns>
        private int Sign()
        {
            if (IsError)
                return -1;

            Debug(true, "Sign()");
            if (Scanner.TokenCode == PLUS)
                Scanner.GetNextToken(ScannerEchoOn);
            else if (Scanner.TokenCode == MINUS)
                Scanner.GetNextToken(ScannerEchoOn);
            else
                Error("PLUS or MINUS");
            Debug(false, "Sign()");
            return -1;
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
        /// Implements CFG Rule: <term> -> <factor> {<mulop> <factor> }*
        /// </summary>
        /// <returns></returns>
        private int Term()
        {
            if (IsError)
                return -1;

            Debug(true, "Term()");
            int x = Factor();

            while (isMulOp())
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
                Scanner.GetNextToken(ScannerEchoOn);
            else
                Error("MULTIPLY or DIVIDE");

            Debug(false, "MulOp()");
            return -1;
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
        /// Implements CFG Rule: <factor> -> <unsigned constant> | <variable> | $LPAR <simple expression> $RPAR
        /// </summary>
        /// <returns></returns>
        private int Factor()
        {
            if (IsError)
                return -1;

            Debug(true, "Factor()");

            int x;

            // TOOD: Implement isUnsignedConstant() and isVariable()
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
                Scanner.GetNextToken(ScannerEchoOn);
                SimpleExpression();
                if (Scanner.TokenCode == RPAR)
                    Scanner.GetNextToken(ScannerEchoOn);
                else
                    Error("RPAR");
            }
            else
                Error("UNSIGNED CONSTANT or VARIABLE or LPAR");

            Debug(false, "Factor()");
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
                Scanner.GetNextToken(ScannerEchoOn);
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
                Scanner.GetNextToken(ScannerEchoOn);
            else
                Error("IDENTIFIER");

            Debug(false, "Identifier()");
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
            Console.WriteLine("ERROR: {0} expected, but {1} found.", (expectedToken, Scanner.NextToken));
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

        #endregion
    }
}
