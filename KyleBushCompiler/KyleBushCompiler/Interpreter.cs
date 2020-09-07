using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KyleBushCompiler
{
    class Interpreter
    {
        public int ProgramCounter { get; set; }
        public Quad CurrentQuad { get; set; }

        public void PrintTrace(string opcode, int op1, int op2, int op3)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {opcode} {op1}, {op2}, {op3}");
        }

        public void PrintTrace(string opcode, int op1, int op2)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {opcode} {op1}, {op2}");
        }

        public void PrintTrace(string opcode, int op)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {opcode} {op}");
        }

        public void PrintTrace(string opcode)
        {
            Console.WriteLine($"PC = {ProgramCounter}: {opcode}");
        }

        public void InterpretQuads(QuadTable quadTable, SymbolTable symbolTable, bool TraceOn)
        {
            int value;
            int op1;
            int op2;
            int op3;
            ProgramCounter = 0;
            while (ProgramCounter < quadTable.QuadTableData.Count)
            {
                CurrentQuad = quadTable.GetQuad(ProgramCounter);
                string CurrentQuadOpcode = quadTable.GetMnemonic(CurrentQuad.OpCode);
                if (true) //CurrentQuadOpcode)
                {
                    switch (CurrentQuadOpcode)
                    {
                        case "STOP":
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode);
                            }
                            ProgramCounter = quadTable.QuadTableData.Count;
                            break;
                        case "DIV":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            op3 = CurrentQuad.Op3;
                            value = op1 / op2;
                            symbolTable.UpdateSymbol(op3, (int)SymbolKind.Variable, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode, op1, op2, op3);
                            }
                            ProgramCounter++;
                            break;
                        case "MUL":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            value = op1 * op2;
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, (int)SymbolKind.Variable, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode, op1, op2, op3);
                            }
                            ProgramCounter++;
                            break;
                        case "SUB":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            value = op1 - op2;
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, (int)SymbolKind.Variable, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode, op1, op2, op3);
                            }
                            ProgramCounter++;
                            break;
                        case "ADD":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            value = op1 + op2;
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, (int)SymbolKind.Variable, value);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode, op1, op2, op3);
                            }
                            ProgramCounter++;
                            break;
                        case "MOV":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op3 = CurrentQuad.Op3;
                            symbolTable.UpdateSymbol(op3, (int)SymbolKind.Variable, op1);
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode, CurrentQuad.Op1, op3);
                            }
                            ProgramCounter++;
                            break;
                        case "STI":
                            break;
                        case "LDI":
                            break;
                        case "BNZ":
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].IntValue != 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            } 
                            else
                            {
                                ProgramCounter++;
                            }
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode, CurrentQuad.Op3);
                            }
                            break;
                        case "BNP":
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].IntValue <= 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        case "BNN":
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].IntValue >= 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        case "BZ":
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].IntValue == 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        case "BP":
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].IntValue > 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        case "BN":
                            if (symbolTable.SymbolTableData[CurrentQuad.Op1].IntValue < 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        case "BR":
                            ProgramCounter = CurrentQuad.Op3;
                            break;
                        case "BINDR":
                            ProgramCounter = symbolTable.SymbolTableData[CurrentQuad.Op3].IntValue;
                            break;
                        case "PRINT":
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuadOpcode, CurrentQuad.Op1);
                            }
                            Console.WriteLine($"{ symbolTable.SymbolTableData[CurrentQuad.Op1].Name} {symbolTable.SymbolTableData[CurrentQuad.Op1].IntValue}");
                            ProgramCounter++;
                            break;
                        default:
                            Console.WriteLine($"Invalid Opcode {CurrentQuadOpcode}");
                            break;
                    }
                }
            }
        }
    }
}
