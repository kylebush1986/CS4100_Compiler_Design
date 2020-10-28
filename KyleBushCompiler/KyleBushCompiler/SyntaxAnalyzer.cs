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
                // TODO: Implement {$SEMICOLON <statement>}*

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
            // TODO: Implement CFG Rule
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
            if (Scanner.TokenCode == PLUS)
                Scanner.GetNextToken(ScannerEchoOn);
            else if (Scanner.TokenCode == MINUS)
                Scanner.GetNextToken(ScannerEchoOn);
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
                Scanner.GetNextToken(ScannerEchoOn);
            else if (Scanner.TokenCode == MINUS)
                Scanner.GetNextToken(ScannerEchoOn);
            else
                Error("PLUS or MINUS");
            Debug(false, "Sign()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: 
        /// </summary>
        /// <returns></returns>
        private int Term()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements CFG Rule: 
        /// </summary>
        /// <returns></returns>
        private int MulOp()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements CFG Rule: 
        /// </summary>
        /// <returns></returns>
        private int Factor()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements CFG Rule: 
        /// </summary>
        /// <returns></returns>
        private int UnsignedConstant()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements CFG Rule: 
        /// </summary>
        /// <returns></returns>
        private int UnsignedNumber()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements CFG Rule: 
        /// </summary>
        /// <returns></returns>
        private int Identifier()
        {
            throw new NotImplementedException();
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
