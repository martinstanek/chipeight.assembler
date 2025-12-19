using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ChipEight.Assembler.Exceptions;

namespace ChipEight.Assembler;

public class Encoder
{
    private readonly Dictionary<string, InstructionPattern> _patterns = new();
    
    public Encoder Build(SymbolMap symbolMap)
    {
        _patterns.Add("CLR", new PatternClear());
        _patterns.Add("RTN", new PatternReturn());
        _patterns.Add("DRW", new PatternDrawSprite());
        _patterns.Add("VRG", new PatternValueToRegister());
        _patterns.Add("CALL", new PatternCall(symbolMap));
        _patterns.Add("JMP", new PatternJump(symbolMap));
        _patterns.Add("VI", new PatternValueToI(symbolMap));
        _patterns.Add("SKE", new PatternSkipIfEqual());
        _patterns.Add("SKNE", new PatternSkipIfNotEqual());
        _patterns.Add("SKRE", new PatternSkipIfRegistersEqual());
        _patterns.Add("ADD", new PatternAddValueToRegister());
        _patterns.Add("MRV", new PatternMoveRegisterValues());
        _patterns.Add("OR", new PatternBitwiseOr());
        _patterns.Add("AND", new PatternBitwiseAnd());
        _patterns.Add("XOR", new PatternBitwiseXor());
        _patterns.Add("SUB", new PatternSubtractRegisters());
        _patterns.Add("SUM", new PatternSumRegisters());
        _patterns.Add("SHR", new PatternShiftRightRegister());
        _patterns.Add("SHL", new PatternShiftLeftRegister());
        _patterns.Add("SKRNE", new PatternSkipIfRegistersNotEqual());
        _patterns.Add("SUBR", new PatternSubtractRegistersReverse());
        _patterns.Add("JMPR", new PatternJumpPlusRegister(symbolMap));
        _patterns.Add("ADDI", new PatternAddRegisterToI());
        _patterns.Add("RND", new PatternRandom());
        _patterns.Add("SKEY", new PatternSkipIfKey());
        _patterns.Add("SNKEY", new PatternSkipIfNotKey());
        _patterns.Add("DLR", new PatternDelayToRegister());
        _patterns.Add("WKEY", new PatternWaitForKey());
        _patterns.Add("DLY", new PatternDelay());
        _patterns.Add("BUZZ", new PatternBuzzer());
        _patterns.Add("FRA", new PatternFontAddressToRegister());
        _patterns.Add("BCD", new PatternBinaryCodedDecimal());
        _patterns.Add("SRM", new PatternSaveRegistersToMemory());
        _patterns.Add("LRM", new PatternLoadRegistersFromMemory());

        return this;
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

    private byte[] EncodeData(ParsedLine parsedLine)
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