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

        #region Quad OpCode Constants

        private const int STOP = 0;
        private const int DIV = 1;
        private const int MUL = 2;
        private const int SUB = 3;
        private const int ADD = 4;
        private const int MOV = 5;
        private const int STI = 6;
        private const int LDI = 7;
        private const int BNZ = 8;
        private const int BNP = 9;
        private const int BNN = 10;
        private const int BZ = 11;
        private const int BP = 12;
        private const int BN = 13;
        private const int BR = 14;
        private const int BINDR = 15;
        private const int PRINT = 16;

        #endregion

        #region Symbol Table Constants
        private const int Minus1Index = 0;
        private const int Plus1Index = 1;

        #endregion

        #region Properties
        public bool TraceOn { get; set; }
        public bool IsError { get; set; }
        public bool ErrorOcurred { get; set; }
        private bool PrintError { get; set; }
        private LexicalAnalyzer Scanner { get; set; }
        private ReserveTable TokenCodes { get; set; }
        private QuadTable Quads { get; set; }
        private bool ScannerEchoOn { get; set; }
        private bool Verbose { get; set; }
        private List<string> DeclaredVariables { get; set; }
        private List<string> DeclaredLabels { get; set; }
        private string ProgramName { get; set; }
        private int _tempNumber = 1;

        #endregion

        public SyntaxAnalyzer(LexicalAnalyzer scanner, ReserveTable tokenCodes, bool scannerEchoOn, QuadTable quads)
        {
            Scanner = scanner;
            ScannerEchoOn = scannerEchoOn;
            TokenCodes = tokenCodes;
            Quads = quads;
            DeclaredLabels = new List<string>();
            DeclaredVariables = new List<string>();
            PrintError = true;
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

            Scanner.PastDeclarationSection = false;

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
                        UnexpectedTokenError("PERIOD");
                    }
                }
                else
                {
                    UnexpectedTokenError("SEMICOLON");
                }
            }
            else
            {
                UnexpectedTokenError("UNIT");
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
                Scanner.PastDeclarationSection = false;
                LabelDeclaration();
            }

            while (Scanner.TokenCode == VAR && !IsError)
            {
                Scanner.PastDeclarationSection = false;
                VariableDecSec();
            }

            Scanner.PastDeclarationSection = true;

            BlockBody();

            // Error handling and resyncing
            while (IsError == true && !Scanner.EndOfFile)
            {
                Resync();
                IsError = false;
                PrintError = true;
                while (IsError == false && !Scanner.EndOfFile)
                {
                    Statement();

                    if (Scanner.TokenCode == END)
                    {
                        GetNextToken();
                        if (Scanner.TokenCode == PERIOD)
                            GetNextToken();
                        else
                            UnexpectedTokenError("PERIOD");
                    }
                    else if (Scanner.TokenCode == SEMICOLON)
                        GetNextToken();
                    else
                        UnexpectedTokenError("END or SEMICOLON");
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
                Statement();
                while (Scanner.TokenCode == SEMICOLON && !IsError)
                {
                    GetNextToken();
                    Statement();
                }

                if (Scanner.TokenCode == END)
                    GetNextToken();
                else
                    UnexpectedTokenError("END or SEMICOLON");
            }
            else
                UnexpectedTokenError("BEGIN");

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
                if (Scanner.TokenCode == IDENTIFIER)
                {
                    if (isNotPreviouslyDeclaredIdentifier(SymbolKind.Label))
                    {
                        Identifier();

                        while (Scanner.TokenCode == COMMA && !IsError)
                        {
                            GetNextToken();
                            if (Scanner.TokenCode == IDENTIFIER)
                            {
                                if (isNotPreviouslyDeclaredIdentifier(SymbolKind.Label))
                                {
                                    Identifier();
                                }
                            }
                        }

                        if (Scanner.TokenCode == SEMICOLON)
                            GetNextToken();
                        else
                            UnexpectedTokenError("SEMICOLON");
                    }
                }
            }
            else
                UnexpectedTokenError("LABEL");

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
                VariableDeclaration();
            }
            else
                UnexpectedTokenError("VAR");

            Debug(false, "VariableDecSec()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <variable-declaration> -> {<identifier> {$COMMA <identifier>}* $COLON <type> $SEMICOLON}+
        /// </summary>
        /// <returns></returns>
        private int VariableDeclaration()
        {
            List<string> variables = new List<string>();
            string type = "";

            if (IsError)
                return -1;

            Debug(true, "VariableDeclaration()");

            if (DeclaredLabels.Contains(Scanner.NextToken))
            {
                RedeclaredIdentifierError("LABEL", "VARIABLE");
            }
            else if (ProgramName == Scanner.NextToken)
            {
                RedeclaredIdentifierError("ProgramName", "VARIABLE");
            }
            else
            {
                do
                {
                    variables.Add(Scanner.NextToken);
                    DeclaredVariables.Add(Scanner.NextToken.ToUpper());
                    Variable();
                    while (Scanner.TokenCode == COMMA && !IsError)
                    {
                        GetNextToken();
                        variables.Add(Scanner.NextToken);
                        DeclaredVariables.Add(Scanner.NextToken.ToUpper());
                        Variable();
                    }
                    if (Scanner.TokenCode == COLON)
                    {
                        GetNextToken();
                        type = Scanner.NextToken;
                        Type();
                        if (Scanner.TokenCode == SEMICOLON)
                        {
                            GetNextToken();
                        }
                        else
                        {
                            UnexpectedTokenError("SEMICOLON");
                        }

                        SetVariableType(variables, type);
                    }
                    else
                    {
                        UnexpectedTokenError("COMMA");
                    }
                } while (Scanner.TokenCode == IDENTIFIER && !IsError);
            }
            

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

            int left, right, branchTarget, branchQuad, saveTop, limit, patchElse, temp;

            while (IsLabel() && !IsError)
            {
                int x = Label();
                if (Scanner.TokenCode == COLON)
                    GetNextToken();
            }
            if (isVariable())
            {
                left = Variable();
                if (Scanner.TokenCode == ASSIGN)
                {
                    GetNextToken();
                    if (IsSimpleExpression())
                    {
                        right = SimpleExpression();
                        Quads.AddQuad(MOV, right, 0, left);
                    }
                    else if (Scanner.TokenCode == STRINGTYPE)
                    {
                        right = StringConst();
                    }
                    else
                        UnexpectedTokenError("SIMPLE EXPRESSION or STRING");
                }
                else
                    UnexpectedTokenError("ASSIGN");
            }
            else if (Scanner.TokenCode == BEGIN)
            {
                BlockBody();
            }
            else if (Scanner.TokenCode == IF)
            {
                GetNextToken();
                branchQuad = RelExpression();
                if (Scanner.TokenCode == THEN)
                {
                    GetNextToken();
                    Statement();

                    if (Scanner.TokenCode == ELSE)
                    {
                        GetNextToken();

                        patchElse = Quads.NextQuad();
                        Quads.AddQuad(BR, 0, 0, 0);
                        Quads.SetQuadOp3(branchQuad, Quads.NextQuad());

                        Statement();

                        Quads.SetQuadOp3(patchElse, Quads.NextQuad());
                    }
                    else
                    {
                        Quads.SetQuadOp3(branchQuad, Quads.NextQuad());
                    }
                }
                else
                    UnexpectedTokenError("THEN");
            }
            else if (Scanner.TokenCode == WHILE)
            {
                GetNextToken();

                saveTop = Quads.NextQuad();
                branchQuad = RelExpression();

                if (Scanner.TokenCode == DO)
                {
                    GetNextToken();
                    Statement();
                    Quads.AddQuad(BR, 0, 0, saveTop);
                    Quads.SetQuadOp3(branchQuad, Quads.NextQuad());
                }
                else
                    UnexpectedTokenError("DO");
            }
            else if (Scanner.TokenCode == REPEAT)
            {
                GetNextToken();

                branchTarget = Quads.NextQuad();
                Statement();
                if (Scanner.TokenCode == UNTIL)
                {
                    GetNextToken();
                    branchQuad = RelExpression();
                    Quads.SetQuadOp3(branchQuad, branchTarget);
                }
                else
                    UnexpectedTokenError("UNTIL");
            }
            else if (Scanner.TokenCode == FOR)
            {
                GetNextToken();
                
                right = Variable();
                if (Scanner.TokenCode == ASSIGN)
                {
                    GetNextToken();
                    left = SimpleExpression();
                    Quads.AddQuad(MOV, left, 0, right); // Save the value of the expression in the variable.
                    saveTop = Quads.NextQuad();
                    if (Scanner.TokenCode == TO)
                    {
                        GetNextToken();
                        limit = SimpleExpression();
                        temp = GenSymbol();
                        Quads.AddQuad(SUB, right, limit, temp);
                        branchQuad = Quads.NextQuad();
                        Quads.AddQuad(BP, temp, 0, 0);

                        if (Scanner.TokenCode == DO)
                        {
                            GetNextToken();
                            Statement();

                            Quads.AddQuad(ADD, right, Plus1Index, right);
                            Quads.AddQuad(BR, 0, 0, saveTop);
                            Quads.SetQuadOp3(branchQuad, Quads.NextQuad());
                        }
                        else
                            UnexpectedTokenError("DO");
                    }
                    else
                        UnexpectedTokenError("TO");
                }
                else
                    UnexpectedTokenError("ASSIGN");
            }
            else if (Scanner.TokenCode == GOTO)
            {
                GetNextToken();
                right = Label();
                Quads.AddQuad(BR, 0, 0, right);
            }
            else if (Scanner.TokenCode == WRITELN)
            {
                GetNextToken();
                if (Scanner.TokenCode == LPAR)
                {
                    GetNextToken();
                    if (IsSimpleExpression())
                    {
                        left = SimpleExpression();
                        Quads.AddQuad(PRINT, left, 0, 0);
                        if (Scanner.TokenCode == RPAR)
                            GetNextToken();
                        else
                            UnexpectedTokenError("RPAR");
                    }
                    else if (Scanner.TokenCode == IDENTIFIER)
                    {
                        left = Identifier();
                        Quads.AddQuad(PRINT, left, 0, 0);
                        if (Scanner.TokenCode == RPAR)
                            GetNextToken();
                        else
                            UnexpectedTokenError("RPAR");
                    }
                    else if (Scanner.TokenCode == STRINGTYPE)
                    {
                        left = StringConst();
                        Quads.AddQuad(PRINT, left, 0, 0);
                        if (Scanner.TokenCode == RPAR)
                            GetNextToken();
                        else
                            UnexpectedTokenError("RPAR");
                    }
                    else
                        UnexpectedTokenError("SimpleExpression or IDENTIFIER or STRINGTYPE");
                }
                else
                    UnexpectedTokenError("LPAR");
            }
            else
                UnexpectedTokenError("Statement Token");

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
            UpdateSymbolKind(SymbolKind.ProgName);
            ProgramName = Scanner.NextToken;
            Identifier();
            Debug(false, "ProgIdentifier()");
            return -1;
        }

        /// <summary>
        /// Implements CFG Rule: <variable> -> <identifier> [$LEFT_BRACKET <simple expression> $RIGHT_BRACKET]
        /// </summary>
        /// <returns>The index of the variable in the symbol table.</returns>
        private int Variable()
        {
            if (IsError)
                return -1;

            Debug(true, "Variable()");

            int varIndex = -1;

            int index = Scanner.SymbolTable.LookupSymbol(Scanner.NextToken);
            if (index != -1)
            {
                Symbol symbol = Scanner.SymbolTable.GetSymbol(index);
                if (symbol.Kind == SymbolKind.Variable)
                {
                    if (DeclaredLabels.Contains(Scanner.NextToken.ToUpper()))
                    {
                        DeclarationWarning(SymbolKind.Variable, SymbolKind.Label);
                        DeclaredVariables.Add(Scanner.NextToken.ToUpper());
                    }
                    else if (!DeclaredVariables.Contains(Scanner.NextToken.ToUpper()))
                    {
                        UndeclaredWarning();
                        DeclaredVariables.Add(Scanner.NextToken.ToUpper());
                    }
                    
                }
                else
                    DeclarationWarning(SymbolKind.Variable, symbol.Kind);

                varIndex = Identifier();


                if (Scanner.TokenCode == LEFT_BRACKET)
                {
                    GetNextToken();
                    SimpleExpression();
                    if (Scanner.TokenCode == RIGHT_BRACKET)
                        GetNextToken();
                    else
                        UnexpectedTokenError("RIGHT_BRACKET");
                }
            }

            Debug(false, "Variable()");
            return varIndex;
        }

        /// <summary>
        /// Implements CFG Rule: <label> -> <identifier>
        /// </summary>
        /// <returns></returns>
        private int Label()
        {
            if (IsError)
                return -1;

            int result = -1;

            Debug(true, "Label()");
            // Checks that the indentifier has been declared as type label 
            if (IsLabel())
                result = Identifier();
            else
                UnexpectedTokenError("LABEL");
                
            Debug(false, "Label()");
            return result;
        }

        /// <summary>
        /// Implements CFG Rule: <relexpression> -> <simple expression> <relop> <simple expression>
        /// </summary>
        /// <returns></returns>
        private int RelExpression()
        {
            if (IsError)
                return -1;

            int left, right, saveRelop, result, temp;

            Debug(true, "Label()");

            left = SimpleExpression(); 
            saveRelop = RelOp();
            right = SimpleExpression();

            temp = GenSymbol();
            Quads.AddQuad(SUB, left, right, temp); // Compare left and right operands
            result = Quads.NextQuad(); // Index where branch will be
            Quads.AddQuad(RelopToOpcode(saveRelop), temp, 0, 0); // Op3 will be set later

            Debug(false, "Label()");
            return result;
        }

        /// <summary>
        /// Implements CFG Rule: <relop> -> $EQ | $LSS | $GTR | $NEQ | $LEQ | $GEQ
        /// </summary>
        /// <returns></returns>
        private int RelOp()
        {
            if (IsError)
                return -1;

            int opCode = -1;

            Debug(true, "Label()");
            switch (Scanner.TokenCode)
            {
                case EQUAL:
                    opCode = EQUAL;
                    GetNextToken();
                    break;
                case LESS_THAN:
                    opCode = LESS_THAN;
                    GetNextToken();
                    break;
                case GREATER_THAN:
                    opCode = GREATER_THAN;
                    GetNextToken();
                    break;
                case LESS_THAN_OR_EQUAL:
                    opCode = LESS_THAN_OR_EQUAL;
                    GetNextToken();
                    break;
                case GREATER_THAN_OR_EQUAL:
                    opCode = GREATER_THAN_OR_EQUAL;
                    GetNextToken();
                    break;
                case NOT_EQUAL:
                    opCode = NOT_EQUAL;
                    GetNextToken();
                    break;
                default:
                    UnexpectedTokenError("Relational Operator");
                    break;
            }
            Debug(false, "Label()");
            return opCode;
        }

        /// <summary>
        /// Implements CFG Rule: <simple expression> -> [<sign>] <term> {<addop> <term>}*
        /// </summary>
        /// <returns>The result of the expression.</returns>
        private int SimpleExpression()
        {
            if (IsError)
                return -1;

            Debug(true, "SimpleExpression()");

            int left, right, temp, opcode;
            int signVal = 0;

            if (isSign())
            {
                signVal = Sign();
            }

            left = Term();

            if (signVal == -1)
            {
                Quads.AddQuad(MULTIPLY, left, Minus1Index, left);
            }

            while (isAddOp() && !IsError)
            {
                opcode = AddOp();
                right = Term();
                temp = GenSymbol();
                Quads.AddQuad(opcode, left, right, temp);
                left = temp;
            }

            Debug(false, "SimpleExpression()");
            return left;
        }

        /// <summary>
        /// Creates a temp symbol or modifies it in the symbol table.
        /// </summary>
        /// <returns></returns>
        private int GenSymbol()
        {
            int index = Scanner.SymbolTable.AddSymbol("#temp" + _tempNumber.ToString(), SymbolKind.Variable, 0);
            _tempNumber++;
            return index;
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

            int result = -1;

            if (Scanner.TokenCode == PLUS)
            {
                result = ADD;
                GetNextToken();
            }
            else if (Scanner.TokenCode == MINUS)
            {
                result = SUB;
                GetNextToken();
            }
            else
                UnexpectedTokenError("PLUS or MINUS");
            Debug(false, "AddOp()");
            return result;
        }



        /// <summary>
        /// Implements CFG Rule: <sign> -> $PLUS | $MINUS
        /// </summary>
        /// <returns>0 if ERROR, 1 if PLUS, -1 if MINUS</returns>
        private int Sign()
        {
            if (IsError)
                return 0;

            Debug(true, "Sign()");

            int result = 0;

            if (Scanner.TokenCode == PLUS)
            {
                result = 1;
                GetNextToken();
            }
            else if (Scanner.TokenCode == MINUS)
            {
                result = -1;
                GetNextToken();
            }
            else
                UnexpectedTokenError("PLUS or MINUS");
            Debug(false, "Sign()");
            return result;
        }


        /// <summary>
        /// Implements CFG Rule: <term> -> <factor> {<mulop> <factor> }*
        /// </summary>
        /// <returns></returns>
        private int Term()
        {
            if (IsError)
                return -1;
            int left, right, opCode, temp;

            Debug(true, "Term()");
            left = Factor();

            while (isMulOp() && !IsError)
            {
                opCode = MulOp();
                right = Factor();
                temp = GenSymbol();
                try
                {
                    Quads.AddQuad(opCode, left, right, temp);
                }
                catch (DivideByZeroException e)
                {
                    Console.WriteLine(e.Message);
                }
                left = temp;
            }

            Debug(false, "Term()");
            return left;
        }

        /// <summary>
        /// Implements CFG Rule: <mulop> -> $MULTIPLY | $DIVIDE
        /// </summary>
        /// <returns>The op code for multiply or divide.</returns>
        private int MulOp()
        {
            if (IsError)
                return -1;

            int result = -1;

            Debug(true, "MulOp()");

            if (Scanner.TokenCode == MULTIPLY)
            {
                result = MUL;
                GetNextToken();
            }
            else if (Scanner.TokenCode == DIVIDE)
            {
                result = DIV;
                GetNextToken();
            }
            else
                UnexpectedTokenError("MULTIPLY or DIVIDE");

            Debug(false, "MulOp()");
            return result;
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

            int index = 0;

            if (isUnsignedConstant())
            {
                index = UnsignedConstant();
            }
            else if (isVariable())
            {
                index = Variable();
            }
            else if (Scanner.TokenCode == LPAR)
            {
                GetNextToken();
                index = SimpleExpression();
                if (Scanner.TokenCode == RPAR)
                    GetNextToken();
                else
                    UnexpectedTokenError("RPAR");
            }
            else
                UnexpectedTokenError("UNSIGNED CONSTANT or VARIABLE or LPAR");

            Debug(false, "Factor()");
            return index;
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
                                    UnexpectedTokenError("INTEGER");
                            }
                            else
                                UnexpectedTokenError("OF");
                        }
                        else
                            UnexpectedTokenError("RIGHT_BRACKET");
                    }
                    else
                        UnexpectedTokenError("INTTYPE");
                }
                else
                    UnexpectedTokenError("LEFT_BRACKET");
            }
            else
                UnexpectedTokenError("Simple Type or ARRAY");

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

            if (Scanner.TokenCode == INTEGER || Scanner.TokenCode == REAL || Scanner.TokenCode == STRING)
                GetNextToken();
            else
                UnexpectedTokenError("INTEGER or FLOAT or STRING");

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
            int sign = 1;
            int index;

            Debug(true, "Constant()");

            if (isSign())
            {
                sign = Sign();
            }
            index = UnsignedConstant();

            if (sign == -1)
            {
                Quads.AddQuad(MUL, index, Minus1Index, index);
            }

            Debug(false, "Constant()");
            return index;
        }

        /// <summary>
        /// Implements CFG Rule: <unsigned constant>-> <unsigned number>
        /// </summary>
        /// <returns></returns>
        private int UnsignedConstant()
        {
            if (IsError)
                return -1;

            int index = -1;

            Debug(true, "UnsignedConstant()");
            index = UnsignedNumber();
            Debug(false, "UnsignedConstant()");
            return index;
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

            int index = -1;

            if (Scanner.TokenCode == FLOAT || Scanner.TokenCode == INTTYPE)
            {
                index = Scanner.SymbolTable.LookupSymbol(Scanner.NextToken);
                GetNextToken();
            }
            else
                UnexpectedTokenError("FLOAT or INTTYPE");

            Debug(false, "UnsignedNumber()");
            return index;
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

            int index = -1;

            if (Scanner.TokenCode == IDENTIFIER)
            {
                index = Scanner.SymbolTable.LookupSymbol(Scanner.NextToken);
                GetNextToken();
            }
            else
                UnexpectedTokenError("IDENTIFIER");

            Debug(false, "Identifier()");
            return index;
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

            int index = -1;

            if (Scanner.TokenCode == STRINGTYPE)
            {
                index = Scanner.SymbolTable.LookupSymbol(Scanner.NextToken);
                GetNextToken();
            }
            else
                UnexpectedTokenError("STRINGYPE");

            Debug(false, "StringConst()");
            return index;
        }

        #endregion

        #region Errors and Warnings

        /// <summary>
        /// Prints an error with the expected token type and the actual token found.
        /// </summary>
        /// <param name="expectedToken">The expected token type.</param>
        private void UnexpectedTokenError(string expectedToken)
        {
            IsError = true;
            ErrorOcurred = true;

            if (PrintError)
            {
                Console.WriteLine("\n********** Error **********");
                Console.WriteLine("Line #{0}: {1}", Scanner.CurrentLineIndex, Scanner.CurrentLine);
                Console.WriteLine("ERROR: {0} expected, but {1} found.", expectedToken, Scanner.NextToken);
                Console.WriteLine("***************************\n");
            }

            PrintError = false;
        }

        /// <summary>
        /// Prints a warning message when an identifier is detected that was undeclared.
        /// </summary>
        private void UndeclaredWarning()
        {
            Console.WriteLine("\n********** Warning **********");
            Console.WriteLine("Line #{0}: {1}", Scanner.CurrentLineIndex, Scanner.CurrentLine);
            Console.WriteLine("WARNING: {0} undeclared.", Scanner.NextToken);
            Console.WriteLine("*****************************\n");
        }

        /// <summary>
        /// Prints a warning message when an identifier is used as a different Kind than what it was declared.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="found"></param>
        private void DeclarationWarning(SymbolKind expected, SymbolKind found)
        {
            Console.WriteLine("\n********** Warning **********");
            Console.WriteLine("Line #{0}: {1}", Scanner.CurrentLineIndex, Scanner.CurrentLine);
            Console.WriteLine("WARNING: {0} declared as expected, but used as {1}.", Scanner.NextToken, expected, found);
            Console.WriteLine("*****************************\n");
        }

        /// <summary>
        /// Displays 
        /// </summary>
        /// <param name="used"></param>
        /// <param name="declared"></param>
        private void RedeclaredIdentifierError(string used, string declared)
        {
            IsError = true;
            ErrorOcurred = true;

            Console.WriteLine("\n********** Error **********");
            Console.WriteLine("Line #{0}: {1}", Scanner.CurrentLineIndex, Scanner.CurrentLine);
            Console.WriteLine("WARNING: {0} used, but {1} declared.", used, declared);
            Console.WriteLine("***************************\n");
        }

        #endregion

        #region Type Testing

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
            {
                int index = Scanner.SymbolTable.LookupSymbol(Scanner.NextToken);
                if (index != -1)
                {
                    Symbol symbol = Scanner.SymbolTable.GetSymbol(index);
                    if (symbol.Kind == SymbolKind.Variable)
                        return true;
                }
            }
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
            switch (Scanner.TokenCode)
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
                else
                {
                    Console.WriteLine("Error: The current token is not in the symbol table.");
                }
            }
            return false;
        }

        #endregion

        #region Utility Methods

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
        /// Updates the 'kind' of the current token in the symbol table.
        /// </summary>
        private void UpdateSymbolKind(SymbolKind kind)
        {
            int tokenIndex = Scanner.SymbolTable.LookupSymbol(Scanner.NextToken);
            if (tokenIndex != -1)
            {
                Scanner.SymbolTable.UpdateSymbol(tokenIndex, kind, 0);
            }
            else
            {
                Console.WriteLine("Symbol not found in symbol table.");
            }
        }

        /// <summary>
        /// Sets the DataType and the default value of all the provided variables in the symbol table.
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="type"></param>
        private void SetVariableType(List<string> variables, string type)
        {
            int index;
            Symbol symbol;
            DataType dataType = ConvertDataType(type);
            int defaultIntValue = 0;
            double defaultRealValue = 1.1;
            string defaultStringValue = "string"; 

            foreach (string variable in variables)
            {
                index = Scanner.SymbolTable.LookupSymbol(variable);
                if (index != -1)
                {
                    symbol = Scanner.SymbolTable.GetSymbol(index);
                    if (dataType != DataType.Invalid)
                        symbol.DataType = dataType;
                    else
                        Console.WriteLine("Could not set value due to invalid data type.");

                    if (dataType == DataType.Integer)
                        symbol.SetValue(defaultIntValue);
                    else if (dataType == DataType.Double)
                        symbol.SetValue(defaultRealValue);
                    else if (dataType == DataType.String)
                        symbol.SetValue(defaultStringValue);
                    else
                        Console.WriteLine("Could not set value dues to invalid data type.");
                }
            }
        }

        /// <summary>
        /// Converts a string representation of a 'type' to an instance of the enum DataType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private DataType ConvertDataType(string type)
        {
            DataType dataType = DataType.Invalid;

            switch (type.ToUpper())
            {
                case "INTEGER":
                    dataType = DataType.Integer;
                    break;
                case "REAL":
                    dataType = DataType.Double;
                    break;
                case "STRING":
                    dataType = DataType.String;
                    break;
                default:
                    Console.WriteLine("Error: Invalid Data Type.");
                    break;
            }

            return dataType;
        }

        /// <summary>
        /// Determines if the identifier has been previously declared as a different type.
        /// </summary>
        /// <param name="identifierType"></param>
        /// <returns></returns>
        private bool isNotPreviouslyDeclaredIdentifier(SymbolKind kind)
        {
            string declaredAs = "";
            string identifierType = "";
            List<string> identifierList = new List<string>();

            switch (kind)
            {
                case SymbolKind.Label:
                    identifierList = DeclaredVariables;
                    declaredAs = "VARIABLE";
                    identifierType = "LABEL";
                    break;
                case SymbolKind.Variable:
                    identifierList = DeclaredLabels;
                    declaredAs = "LABEL";
                    identifierType = "VARIABLE";
                    break;
                default:
                    Console.WriteLine("Invalid Identifier Type");
                    return false;
            }

            if (identifierList.Contains(Scanner.NextToken))
            {
                RedeclaredIdentifierError(identifierType, declaredAs);
                return false;
            }
            else if (Scanner.NextToken == ProgramName)
            {
                RedeclaredIdentifierError(identifierType, "ProgramName");
                return false;
            }
            else
            {
                AddToDeclaredIdentifiers(kind);
                return true;
            }
        }

        /// <summary>
        /// Adds the identifier to the appropriate declated identifier list.
        /// </summary>
        /// <param name="kind"></param>
        private void AddToDeclaredIdentifiers(SymbolKind kind)
        {
            UpdateSymbolKind(kind);
            if (kind == SymbolKind.Variable)
            {
                DeclaredVariables.Add(Scanner.NextToken.ToUpper());
            } else if (kind == SymbolKind.Label)
            {
                DeclaredLabels.Add(Scanner.NextToken.ToUpper());
            }
            else
                Console.WriteLine("This kind of symbol does not need to be added to declared identifiers.");
        }

        /// <summary>
        /// Converts a given relational operator into its corresponding
        /// FALSE BRANCH opcode in order to facilitate Quad creation
        /// </summary>
        /// <param name="relop"></param>
        /// <returns>corresponding FALSE BRANCH opcode</returns>
        private int RelopToOpcode(int relop)
        {
            int result;

            switch (relop)
            {
                case EQUAL:
                    result = BNZ;
                    break;
                case NOT_EQUAL:
                    result = BZ;
                    break;
                case LESS_THAN:
                    result = BNN;
                    break;
                case GREATER_THAN:
                    result = BNP;
                    break;
                case LESS_THAN_OR_EQUAL:
                    result = BP;
                    break;
                case GREATER_THAN_OR_EQUAL:
                    result = BN;
                    break;
                default:
                    result = -1;
                    break;
            }

            return result;
        }

        #endregion
    }
}
