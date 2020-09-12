using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KyleBushCompiler
{
    class Interpreter
    {
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
            ProgramCounter = 0;
            while (ProgramCounter < QuadTable.NextQuad())
            {
                CurrentQuad = QuadTable.GetQuad(ProgramCounter);
                if (QuadTable.ReserveTable.isValidOpCode(CurrentQuad.OpCode))
                {
                    switch (CurrentQuad.OpCode)
                    {
                        // STOP
                        // Terminate program
                        case 0:
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode);
                            }
                            ProgramCounter = QuadTable.NextQuad();
                            break;
                        // DIV
                        // Compute op1 / op2, place result into op3
                        case 1:
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, 
                                (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() / symbolTable.GetSymbol(CurrentQuad.Op2).GetValue()));
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, CurrentQuad.Op2, CurrentQuad.Op3);
                            }
                            ProgramCounter++;
                            break;
                        // MUL
                        // Compute op1 * op2, place result into op3
                        case 2:
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, 
                                (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() * symbolTable.GetSymbol(CurrentQuad.Op2).GetValue()));
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, CurrentQuad.Op2, CurrentQuad.Op3);
                            }
                            ProgramCounter++;
                            break;
                        // SUB
                        // Compute op1 - op2, place result into op3
                        case 3:
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable,
                                (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() - symbolTable.GetSymbol(CurrentQuad.Op2).GetValue()));
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, CurrentQuad.Op2, CurrentQuad.Op3);
                            }
                            ProgramCounter++;
                            break;
                        // ADD
                        // Compute op1 + op2, place result into op3
                        case 4:
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable,
                                (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() + symbolTable.GetSymbol(CurrentQuad.Op2).GetValue()));
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, CurrentQuad.Op2, CurrentQuad.Op3);
                            }
                            ProgramCounter++;
                            break;
                        // MOV
                        // Assign the value in op1 into op3 (op2 is ignored here)
                        case 5:
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, symbolTable.GetSymbol(CurrentQuad.Op1).GetValue());
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1, CurrentQuad.Op3);
                            }
                            ProgramCounter++;
                            break;
                        // STI
                        // Store indexed - Assign the value in op1 into op2 + offset op3
                        case 6:
                            symbolTable.UpdateSymbol((CurrentQuad.Op2 + CurrentQuad.Op3), (int)SymbolKind.Variable, symbolTable.GetSymbol(CurrentQuad.Op1).GetValue());
                            break;
                        // LDI
                        // Load indexed- Assign the value in op1 + offset op2, into op3
                        case 7:
                            symbolTable.UpdateSymbol(CurrentQuad.Op3, (int)SymbolKind.Variable, symbolTable.GetSymbol(CurrentQuad.Op1 + CurrentQuad.Op2).GetValue());
                            break;
                        // BNZ
                        // Branch Not Zero; if op1 value <> 0, set program counter to op3
                        case 8:
                            if (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() != 0)
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
                        // Branch Not Positive; if op1 value <= 0, set program counter to op3
                        case 9:
                            if (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() <= 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BNN
                        // Branch Not Negative; if op1 value >= 0, set program counter to op3
                        case 10:
                            if (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() >= 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BZ
                        // Branch Zero; if op1 value = 0, set program counter to op3
                        case 11:
                            if (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() == 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BP
                        // Branch Positive; if op1 value > 0, set program counter to op3
                        case 12:
                            if (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() > 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BN
                        // Branch Negative; if op1 value < 0, set program counter to op3
                        case 13:
                            if (symbolTable.GetSymbol(CurrentQuad.Op1).GetValue() < 0)
                            {
                                ProgramCounter = CurrentQuad.Op3;
                            }
                            else
                            {
                                ProgramCounter++;
                            }
                            break;
                        // BR
                        // Branch (unconditional); set program counter to op3
                        case 14:
                            ProgramCounter = CurrentQuad.Op3;
                            break;
                        // BINDR
                        // Branch (unconditional); set program counter to op3 value contents (indirect)
                        case 15:
                            ProgramCounter = symbolTable.GetSymbol(CurrentQuad.Op3).GetValue();
                            break;
                        // PRINT
                        // Write symbol table name and value of op 1
                        case 16:
                            if (TraceOn)
                            {
                                PrintTrace(CurrentQuad.OpCode, CurrentQuad.Op1);
                            }
                            Console.WriteLine($"{ symbolTable.GetSymbol(CurrentQuad.Op1).Name} = {symbolTable.GetSymbol(CurrentQuad.Op1).GetValue()}");
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
