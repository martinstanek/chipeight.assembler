using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using ChipEight.Assembler.Exceptions;
using ChipEight.Assembler.Patterns;

namespace ChipEight.Assembler;

public class Encoder
{
    private readonly Dictionary<string, InstructionPattern> _patterns = new();
    private readonly Dictionary<byte, List<InstructionPattern>> _patternGroups = new();
    
    public static Encoder Build(SymbolMap symbolMap)
    {
        return new Encoder()
            .AddPattern("CLR", 0x0, new PatternClear())
            .AddPattern("RTN", 0x0, new PatternReturn())
            .AddPattern("DRW", 0xD, new PatternDrawSprite())
            .AddPattern("VRG", 0x6, new PatternValueToRegister())
            .AddPattern("CALL", 0x2, new PatternCall(symbolMap))
            .AddPattern("JMP", 0x1,  new PatternJump(symbolMap))
            .AddPattern("VI",  0xA, new PatternValueToI(symbolMap))
            .AddPattern("SKE", 0x3, new PatternSkipIfEqual())
            .AddPattern("SKNE", 0x4, new PatternSkipIfNotEqual())
            .AddPattern("SKRE", 0x5, new PatternSkipIfRegistersEqual())
            .AddPattern("ADD", 0x7, new PatternAddValueToRegister())
            .AddPattern("MRV", 0x8, new PatternMoveRegisterValues())
            .AddPattern("OR", 0x8, new PatternBitwiseOr())
            .AddPattern("AND", 0x8, new PatternBitwiseAnd())
            .AddPattern("XOR", 0x8, new PatternBitwiseXor())
            .AddPattern("SUB", 0x8, new PatternSubtractRegisters())
            .AddPattern("SUM", 0x8, new PatternSumRegisters())
            .AddPattern("SHR", 0x8, new PatternShiftRightRegister())
            .AddPattern("SHL", 0x8, new PatternShiftLeftRegister())
            .AddPattern("SKRNE", 0x9, new PatternSkipIfRegistersNotEqual())
            .AddPattern("SUBR", 0x8, new PatternSubtractRegistersReverse())
            .AddPattern("JMPR", 0xB, new PatternJumpPlusRegister(symbolMap))
            .AddPattern("ADDI", 0xF, new PatternAddRegisterToI())
            .AddPattern("RND", 0xC, new PatternRandom())
            .AddPattern("SKEY", 0xE, new PatternSkipIfKey())
            .AddPattern("SNKEY", 0xE, new PatternSkipIfNotKey())
            .AddPattern("DLR", 0xF,new PatternDelayToRegister())
            .AddPattern("WKEY", 0xF, new PatternWaitForKey())
            .AddPattern("DLY", 0xF, new PatternDelay())
            .AddPattern("BUZZ", 0xF, new PatternBuzzer())
            .AddPattern("FRA", 0xF, new PatternFontAddressToRegister())
            .AddPattern("BCD", 0xF, new PatternBinaryCodedDecimal())
            .AddPattern("SRM", 0xF, new PatternSaveRegistersToMemory())
            .AddPattern("LRM", 0xF, new PatternLoadRegistersFromMemory());
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

    public string Decode(ImmutableArray<ushort> opcodes)
    {
        var sb = new StringBuilder();

        foreach (var opcode in opcodes)
        {
            var opcodeHandled = false;
            var group = GetPatternGroup(opcode);
            
            foreach (var instructionPattern in group)
            {
                if (instructionPattern.CanDecode(opcode, out var line))
                {
                    sb.AppendLine(line);
                    opcodeHandled = true;
                    break;                    
                }
            }

            if (!opcodeHandled)
            {
                sb.AppendLine(DecodeData(opcode));
            }
        }

        return sb.ToString();
    }

    private Encoder AddPattern(string mnemonic, byte group, InstructionPattern pattern)
    {
        _patterns[mnemonic] = pattern;

        if (!_patternGroups.ContainsKey(group))
        {
            _patternGroups[group] = new List<InstructionPattern>();
        }

        _patternGroups[group].Add(pattern);

        return this;
    }

    private IReadOnlyCollection<InstructionPattern> GetPatternGroup(ushort opcode)
    {
        var group = (byte) ((opcode & 0xF000) >> 12);
        
        return _patternGroups[group];
    }

    private byte[] EncodeInstruction(ParsedLine parsedLine)
    {
        if (!_patterns.TryGetValue(parsedLine.Instruction, out var pattern))
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }

        var opcode = pattern.Encode(parsedLine);
        var bytes = BitConverter.GetBytes(opcode);
        
        return bytes.Reverse().ToArray();
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

    private static string DecodeData(ushort opcode)
    {
        return $"0x{opcode:X4}";
    }
}