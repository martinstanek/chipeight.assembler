using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ChipEight.Assembler.Exceptions;

namespace ChipEight.Assembler;

public sealed class Compiler
{
    public static byte[] Assemble(string asm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(asm);

        var tokens = Tokenizer.FromFile(asm);
        var lines = Parser.Parse(tokens);
        var symbolMap = SymbolMap.FromParsedLines(lines);
        var binary = new Encoder().Build(symbolMap).Encode(lines);

        return binary;
    }
}

public class SymbolMap
{
    private const ushort StartAddress = 0x200;
    
    private readonly IReadOnlyDictionary<string, ushort> _table;

    public SymbolMap(IReadOnlyDictionary<string, ushort> table)
    {
        _table = table;
    }
    
    public static SymbolMap FromParsedLines(ImmutableArray<ParsedLine> lines)
    {
        var map = new Dictionary<string, ushort>();
        var address = StartAddress;

        foreach (var parsedLine in lines)
        {
            if (parsedLine.LineType == LineType.Label)
            {
                map[parsedLine.Label] = address;
                continue;
            }

            address += 2;
        }
        
        return new SymbolMap(map);
    }

    public static SymbolMap Empty => new(ImmutableDictionary<string, ushort>.Empty);

    public ushort GetLabelAddress(int lineNumber, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        if (!_table.TryGetValue(label, out var address))
        {
            throw new SyntaxException(lineNumber);
        }

        return address;
    }
}

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

public abstract class InstructionPattern
{
    protected InstructionPattern(string mnemonic, string keyword) : this(mnemonic, keyword, SymbolMap.Empty) { }

    protected InstructionPattern(string mnemonic, string keyword, SymbolMap symbolMap)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mnemonic);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyword);

        Map = symbolMap;
        Mnemonic = mnemonic;
        Keyword = keyword;
    }

    public virtual ushort Encode(ParsedLine parsedLine)
    {
        ThrowIfNotMnemonic(parsedLine);

        return EncodeLine(parsedLine);
    }

    protected abstract ushort EncodeLine(ParsedLine parsedLine);

    protected void ThrowIfNotMnemonic(ParsedLine parsedLine)
    {
        if (parsedLine.LineType != LineType.Instruction 
            || !parsedLine.Instruction.Equals(Mnemonic, StringComparison.OrdinalIgnoreCase))
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }

    protected void ThrowIfNot(byte index, OperandType type, ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length < index + 1)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }

        if (parsedLine.Operands[index].OperandType != type)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }
    
    protected void ThrowIf(byte index, OperandType type, ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length < index + 1)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
        
        if (parsedLine.Operands[index].OperandType == type)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }
    
    protected void ThrowIfNot(byte count, ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length != count)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }

    protected SymbolMap Map { get; private set; }
    
    public string Mnemonic { get; private set; }

    public string Keyword { get; private set; }
}

public sealed class PatternClear : InstructionPattern
{
    public PatternClear() : base("CLR", "Clear") { }
    
    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(count: 0, parsedLine);

        return 0x00E0;
    }
}

public sealed class PatternReturn : InstructionPattern
{
    public PatternReturn() : base("RTN", "Return") { }
    
    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(count: 0, parsedLine);

        return 0x00EE;
    }
}

public sealed class PatternCall : InstructionPattern
{
    public PatternCall(SymbolMap symbolMap) : base("CALL", "Call", symbolMap) { }
    
    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIf(index: 0, OperandType.Register, parsedLine);

        var address = parsedLine.Operands[0].OperandType == OperandType.Number
            ? parsedLine.Operands[0].Number
            : Map.GetLabelAddress(parsedLine.LineNumber, parsedLine.Operands[0].Label);

        return (ushort)(0x2000 + address);
    }
}

public sealed class PatternValueToRegister : InstructionPattern
{
    public PatternValueToRegister() : base("VRG", "ValueToRegister") {  }
    
    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Number, parsedLine);
        
        var hi = (byte) (0x60 + parsedLine.Operands[0].Register);
        var lo = (byte) parsedLine.Operands[1].Number;
        
        return  BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternValueToI : InstructionPattern
{
    public PatternValueToI(SymbolMap symbolMap) : base("VI", "ValueToI", symbolMap) { }
    
    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIf(index: 0, OperandType.Register, parsedLine);
        
        var address = parsedLine.Operands[0].OperandType == OperandType.Number
            ? parsedLine.Operands[0].Number
            : Map.GetLabelAddress(parsedLine.LineNumber, parsedLine.Operands[0].Label);

        return (ushort) (0xA000 + address);
    }
}

public sealed class PatternJump : InstructionPattern
{
    public PatternJump(SymbolMap symbolMap) : base("JMP", "Jump", symbolMap) { }
    
    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIf(index: 0, OperandType.Register, parsedLine);

        var address = parsedLine.Operands[0].OperandType == OperandType.Number
            ? parsedLine.Operands[0].Number
            : Map.GetLabelAddress(parsedLine.LineNumber, parsedLine.Operands[0].Label);

        return (ushort) (0x1000 + address);
    }
}

public sealed class PatternDrawSprite : InstructionPattern
{
    public PatternDrawSprite() : base("DRW", "DrawSprite") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);
        ThrowIfNot(index: 2, OperandType.Number, parsedLine);

        var hi = (byte) (0xD0 + parsedLine.Operands[0].Register);
        var lo = (byte) ((parsedLine.Operands[1].Register << 4) + (byte) parsedLine.Operands[2].Number);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternSkipIfEqual : InstructionPattern
{
    public PatternSkipIfEqual() : base("SKE", "SkipIfEqual") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Number, parsedLine);

        var hi = (byte) (0x30 + parsedLine.Operands[0].Register);
        var lo = (byte) parsedLine.Operands[1].Number;

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternSkipIfNotEqual : InstructionPattern
{
    public PatternSkipIfNotEqual() : base("SKNE", "SkipIfNotEqual") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Number, parsedLine);

        var hi = (byte) (0x40 + parsedLine.Operands[0].Register);
        var lo = (byte) parsedLine.Operands[1].Number;

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternSkipIfRegistersEqual : InstructionPattern
{
    public PatternSkipIfRegistersEqual() : base("SKRE", "SkipIfRegistersEqual") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x50 + parsedLine.Operands[0].Register);
        var lo = (byte) (parsedLine.Operands[1].Register << 4);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternAddValueToRegister : InstructionPattern
{
    public PatternAddValueToRegister() : base("ADD", "AddValueToRegister") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Number, parsedLine);

        var hi = (byte) (0x70 + parsedLine.Operands[0].Register);
        var lo = (byte) parsedLine.Operands[1].Number;

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternMoveRegisterValues : InstructionPattern
{
    public PatternMoveRegisterValues() : base("MRV", "MoveRegisterValues") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) (parsedLine.Operands[1].Register << 4);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternBitwiseOr : InstructionPattern
{
    public PatternBitwiseOr() : base("OR", "BitwiseOr") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) ((byte) (parsedLine.Operands[1].Register << 4) + 1);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternBitwiseAnd : InstructionPattern
{
    public PatternBitwiseAnd() : base("AND", "BitwiseAnd") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) ((byte) (parsedLine.Operands[1].Register << 4) + 2);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternBitwiseXor : InstructionPattern
{
    public PatternBitwiseXor() : base("XOR", "BitwiseXor") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) ((byte) (parsedLine.Operands[1].Register << 4) + 3);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternSubtractRegisters : InstructionPattern
{
    public PatternSubtractRegisters() : base("SUB", "SubtractRegisters") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) ((byte) (parsedLine.Operands[1].Register << 4) + 5);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternSumRegisters : InstructionPattern
{
    public PatternSumRegisters() : base("SUM", "SumRegisters") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) ((byte) (parsedLine.Operands[1].Register << 4) + 4);

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternShiftRightRegister : InstructionPattern
{
    public PatternShiftRightRegister() : base("SHR", "ShiftRightRegister") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        
        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = parsedLine.Operands is [_, { OperandType: OperandType.Register }]
            ? (byte) ((byte)(parsedLine.Operands[1].Register << 4) + 6)
            : (byte) 0x06;

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternShiftLeftRegister : InstructionPattern
{
    public PatternShiftLeftRegister() : base("SHL", "ShiftLeftRegister") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        
        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = parsedLine.Operands is [_, { OperandType: OperandType.Register }]
            ? (byte) ((byte)(parsedLine.Operands[1].Register << 4) + 0xE)
            : (byte) 0x0E;

        return BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternSkipIfRegistersNotEqual : InstructionPattern
{
    public PatternSkipIfRegistersNotEqual() : base("SKRNE", "SkipIfRegistersNotEqual") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x90 + parsedLine.Operands[0].Register);
        var lo = ((byte)(parsedLine.Operands[1].Register << 4));

        return BitConverter.ToUInt16([lo, hi]);
    }
}
