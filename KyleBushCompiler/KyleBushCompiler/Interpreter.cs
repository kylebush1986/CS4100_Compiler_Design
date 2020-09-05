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
                            System.Environment.Exit(0);
                            break;
                        case "DIV":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            value = op1 / op2;
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, value);
                            ProgramCounter++;
                            break;
                        case "MUL":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            value = op1 * op2;
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, value);
                            ProgramCounter++;
                            break;
                        case "SUB":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            value = op1 - op2;
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, value);
                            ProgramCounter++;
                            break;
                        case "ADD":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            op2 = symbolTable.GetSymbol(CurrentQuad.Op2).IntValue;
                            value = op1 + op2;
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, value);
                            ProgramCounter++;
                            break;
                        case "MOV":
                            op1 = symbolTable.GetSymbol(CurrentQuad.Op1).IntValue;
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, op1);
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
