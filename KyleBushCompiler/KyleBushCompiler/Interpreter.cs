using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KyleBushCompiler
{
    class Interpreter
    {
        private const int LABEL = 1;
        private const int VARIABLE = 2;
        private const int CONSTANT = 3;
        private int ProgramCounter { get; set; }
        private Quad CurrentQuad { get; set; }
        public QuadTable QuadTable { get; set; }

        public void PrintTrace(int opcode, int op1, int op2, int op3)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {QuadTable.GetMnemonic(opcode)} {op1}, {op2}, {op3}");
        }

        public void PrintTrace(int opcode, int op1, int op2)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {QuadTable.GetMnemonic(opcode)} {op1}, {op2}");
        }

        public void PrintTrace(int opcode, int op)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {QuadTable.GetMnemonic(opcode)} {op}");
        }

        public void PrintTrace(int opcode)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {QuadTable.GetMnemonic(opcode)}");
        }

        public void InterpretQuads(QuadTable quadTable, SymbolTable symbolTable, bool TraceOn)
        {
            QuadTable = quadTable;
            int value;
            Symbol op1;
            Symbol op2;
            int op3;
            ProgramCounter = 0;
            while (ProgramCounter < QuadTable.NextQuad())
            {
                CurrentQuad = QuadTable.GetQuad(ProgramCounter);
                if (QuadTable.ReserveTable.isValidOpCode(CurrentQuad.OpCode))
                {
                    switch (CurrentQuad.OpCode)
                    {
                        // STOP
                        case 0:
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode);
                            }
                            ProgramCounter = QuadTable.NextQuad();
                            break;
                        // DIV
                        case 1:
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1);
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2);
                            value = op1.GetValue() / op2.GetValue();
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, VARIABLE, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, CurrentQuad.Op2, CurrentQuad.Op3);
                            }
                            ProgramCounter++;
                            break;
                        // MUL
                        case 2:
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).GetValue();
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).GetValue();
                            value = op1 * op2;
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, VARIABLE, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, CurrentQuad.Op2, CurrentQuad.Op3);
                            }
                            ProgramCounter++;
                            break;
                        // SUB
                        case 3:
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).GetValue();
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).GetValue();
                            value = op1 - op2;
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, VARIABLE, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, op1, op2, op3);
                            }
                            ProgramCounter++;
                            break;
                        // ADD
                        case 4:
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).GetValue();
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).GetValue();
                            value = op1 + op2;
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, VARIABLE, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, op1, op2, op3);
                            }
                            ProgramCounter++;
                            break;
                        // MOV
                        case 5:
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).GetValue();
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, VARIABLE, op1);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, op3);
                            }
                            ProgramCounter++;
                            break;
                        // STI
                        case 6:
                            break;
                        // LDI
                        case 7:
                            break;
                        // BNZ
                        case 8:
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].GetValue() != 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            } 
                            else
                            {
                                ProgramCounter++;
                            }
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op3);
                            }
                            break;
                        // BNP
                        case 9:
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].GetValue() <= 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BNN
                        case 10:
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].GetValue() >= 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BZ
                        case 11:
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].GetValue() == 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BP
                        case 12:
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].GetValue() > 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BN
                        case 13:
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].GetValue() < 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BR
                        case 14:
                            ProgramCounter = CurrentQuad.Op3;
                            break;
                        // BINDR
                        case 15:
                            ProgramCounter = symbolTable.SymbolTableData[CurrentQuad.Op3].GetValue();
                            break;
                        // PRINT
                        case 16:
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1);
                            }
                            Console.WriteLine($"{ symbolTable.SymbolTableData[CurrentQuad.Op1].Name} {symbolTable.SymbolTableData[CurrentQuad.Op1].GetValue()}");
                            ProgramCounter++;
                            break;
                        default:
                            Console.WriteLine($"Invalid Opcode {CurrentQuad.OpCode}");
                            break;
                    }
                }
            }
        }
    }
}
