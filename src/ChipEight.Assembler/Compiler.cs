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

        var opcode = pattern.Encode(parsedLine.LineNumber, parsedLine.Operands);
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

    public abstract ushort Encode(int lineNumber, ImmutableArray<Operand> operands);

    protected void ThrowIfNot(byte index, int lineNumber, OperandType type, ImmutableArray<Operand> operands)
    {
        if (operands[index].OperandType != type)
        {
            throw new SyntaxException(lineNumber);
        }
    }
    
    protected void ThrowIf(byte index, int lineNumber, OperandType type, ImmutableArray<Operand> operands)
    {
        if (operands[index].OperandType == type)
        {
            throw new SyntaxException(lineNumber);
        }
    }
    
    protected void ThrowIfNot(byte count, int lineNumber, ImmutableArray<Operand> operands)
    {
        if (operands.Length != count)
        {
            throw new SyntaxException(lineNumber);
        }
    }

    protected SymbolMap Map { get; private set; }
    
    public string Mnemonic { get; private set; }

    public string Keyword { get; private set; }
}

public sealed class PatternClear : InstructionPattern
{
    public PatternClear() : base("CLR", "Clear") { }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        ThrowIfNot(count: 0, lineNumber, operands);

        return 0x00E0;
    }
}

public sealed class PatternReturn : InstructionPattern
{
    public PatternReturn() : base("RTN", "Return") { }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        ThrowIfNot(count: 0, lineNumber, operands);

        return 0x00EE;
    }
}

public sealed class PatternCall : InstructionPattern
{
    public PatternCall(SymbolMap symbolMap) : base("CALL", "Call", symbolMap) { }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        ThrowIfNot(count: 1, lineNumber, operands);
        ThrowIf(index: 0, lineNumber, OperandType.Register, operands);

        var address = operands[0].OperandType == OperandType.Number
            ? operands[0].Number
            : Map.GetLabelAddress(lineNumber, operands[0].Label);

        return (ushort)(0x2000 + address);
    }
}

public sealed class PatternValueToRegister : InstructionPattern
{
    public PatternValueToRegister() : base("VRG", "ValueToRegister") {  }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        ThrowIfNot(count: 2, lineNumber, operands);
        ThrowIfNot(index: 0, lineNumber, OperandType.Register, operands);
        ThrowIfNot(index: 1, lineNumber, OperandType.Number, operands);
        
        var hi = (byte) (0x60 + operands[0].Register);
        var lo = (byte) operands[1].Number;
        
        return  BitConverter.ToUInt16([lo, hi]);
    }
}

public sealed class PatternValueToI : InstructionPattern
{
    public PatternValueToI(SymbolMap symbolMap) : base("VI", "ValueToI", symbolMap) { }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        ThrowIfNot(count: 1, lineNumber, operands);
        ThrowIf(index: 0, lineNumber, OperandType.Register, operands);
        
        var address = operands[0].OperandType == OperandType.Number
            ? operands[0].Number
            : Map.GetLabelAddress(lineNumber, operands[0].Label);

        return (ushort) (0xA000 + address);
    }
}

public sealed class PatternJump : InstructionPattern
{
    public PatternJump(SymbolMap symbolMap) : base("JMP", "Jump", symbolMap) { }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        ThrowIfNot(count: 1, lineNumber, operands);
        ThrowIf(index: 0, lineNumber, OperandType.Register, operands);

        var address = operands[0].OperandType == OperandType.Number
            ? operands[0].Number
            : Map.GetLabelAddress(lineNumber, operands[0].Label);

        return (ushort) (0x1000 + address);
    }
}

public sealed class PatternDrawSprite : InstructionPattern
{
    public PatternDrawSprite() : base("DRW", "DrawSprite") { }

    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        ThrowIfNot(count: 3, lineNumber, operands);
        ThrowIfNot(index: 0, lineNumber, OperandType.Register, operands);
        ThrowIfNot(index: 1, lineNumber, OperandType.Register, operands);
        ThrowIfNot(index: 2, lineNumber, OperandType.Number, operands);

        var hi = (byte) (0xD0 + operands[0].Register);
        var lo = (byte) ((operands[1].Register << 4) + (byte) operands[2].Number);

        return BitConverter.ToUInt16([lo, hi]);
    }
}