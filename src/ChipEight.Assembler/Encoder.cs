using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ChipEight.Assembler.Exceptions;

namespace ChipEight.Assembler;

public class Encoder
{
    private readonly Dictionary<string, InstructionPattern> _patterns = new();
    
    public static Encoder Build(SymbolMap symbolMap)
    {
        return new Encoder()
            .AddPattern("CLR", new PatternClear())
            .AddPattern("RTN", new PatternReturn())
            .AddPattern("DRW", new PatternDrawSprite())
            .AddPattern("VRG", new PatternValueToRegister())
            .AddPattern("CALL", new PatternCall(symbolMap))
            .AddPattern("JMP", new PatternJump(symbolMap))
            .AddPattern("VI", new PatternValueToI(symbolMap))
            .AddPattern("SKE", new PatternSkipIfEqual())
            .AddPattern("SKNE", new PatternSkipIfNotEqual())
            .AddPattern("SKRE", new PatternSkipIfRegistersEqual())
            .AddPattern("ADD", new PatternAddValueToRegister())
            .AddPattern("MRV", new PatternMoveRegisterValues())
            .AddPattern("OR", new PatternBitwiseOr())
            .AddPattern("AND", new PatternBitwiseAnd())
            .AddPattern("XOR", new PatternBitwiseXor())
            .AddPattern("SUB", new PatternSubtractRegisters())
            .AddPattern("SUM", new PatternSumRegisters())
            .AddPattern("SHR", new PatternShiftRightRegister())
            .AddPattern("SHL", new PatternShiftLeftRegister())
            .AddPattern("SKRNE", new PatternSkipIfRegistersNotEqual())
            .AddPattern("SUBR", new PatternSubtractRegistersReverse())
            .AddPattern("JMPR", new PatternJumpPlusRegister(symbolMap))
            .AddPattern("ADDI", new PatternAddRegisterToI())
            .AddPattern("RND", new PatternRandom())
            .AddPattern("SKEY", new PatternSkipIfKey())
            .AddPattern("SNKEY", new PatternSkipIfNotKey())
            .AddPattern("DLR", new PatternDelayToRegister())
            .AddPattern("WKEY", new PatternWaitForKey())
            .AddPattern("DLY", new PatternDelay())
            .AddPattern("BUZZ", new PatternBuzzer())
            .AddPattern("FRA", new PatternFontAddressToRegister())
            .AddPattern("BCD", new PatternBinaryCodedDecimal())
            .AddPattern("SRM", new PatternSaveRegistersToMemory())
            .AddPattern("LRM", new PatternLoadRegistersFromMemory());
    }
    
    public byte[] Encode(ImmutableArray<ParsedLine> lines)
    {
        var binary = new List<byte>();

        foreach (var parsedLine in lines)
        {
            if (parsedLine.LineType == LineType.Instruction)
            {
                var bytes = EncodeInstruction(parsedLine);

                binary.AddRange(bytes);
            }

            if (parsedLine.LineType == LineType.Data)
            {
                var bytes = EncodeData(parsedLine);
                
                binary.AddRange(bytes);
            }
        }

        return binary.ToArray();
    }

    private Encoder AddPattern(string mnemonic, InstructionPattern pattern)
    {
        _patterns[mnemonic] = pattern;

        return this;
    }

    private byte[] EncodeInstruction(ParsedLine parsedLine)
    {
        if (!_patterns.TryGetValue(parsedLine.Instruction, out var pattern))
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }

        var opcode = pattern.Encode(parsedLine);
        var bytes = BitConverter.GetBytes(opcode);
                
        bytes.Reverse();
        
        return bytes;
    }

    private static byte[] EncodeData(ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length is 0 or > 2)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }

        if (parsedLine.Operands.Length == 1)
        {
            var bytes = BitConverter.GetBytes(parsedLine.Operands[0].Number);
                
            bytes.Reverse();
        
            return bytes;
        }

        return [(byte)parsedLine.Operands[0].Number, (byte)parsedLine.Operands[1].Number];
    }
}