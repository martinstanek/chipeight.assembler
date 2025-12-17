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
        var binary = new Encoder().Build().Encode(symbolMap, lines);

        return binary;
    }
}

public class SymbolMap
{
    public static SymbolMap FromParsedLines(ImmutableArray<ParsedLine> lines)
    {
        return new SymbolMap();
    }
}

public class Encoder
{
    private readonly Dictionary<string, InstructionPattern> _patterns = new();

    public Encoder Build()
    {
        _patterns.Add("CLR", new PatternClear());
        _patterns.Add("RTN", new PatternReturn());
        _patterns.Add("VRG", new PatternValueToRegister());
        
        return this;
    }
    
    public byte[] Encode(SymbolMap symbolMap, ImmutableArray<ParsedLine> lines)
    {
        var binary = new List<byte>();

        foreach (var parsedLine in lines)
        {
            if (parsedLine.LineType == LineType.Instruction)
            {
                if (!_patterns.TryGetValue(parsedLine.Instruction, out var pattern))
                {
                    throw new SyntaxException(parsedLine.LineNumber);
                }

                var opcode = pattern.Encode(parsedLine.LineNumber, parsedLine.Operands);
                var bytes = BitConverter.GetBytes(opcode);
                
                bytes.Reverse();
                
                binary.AddRange(bytes);
            }
        }

        return binary.ToArray();
    }
}

public abstract class InstructionPattern
{
    protected InstructionPattern(string mnemonic, string keyword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mnemonic);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyword);
        
        Mnemonic = mnemonic;
        Keyword = keyword;
    }

    public abstract ushort Encode(int lineNumber, ImmutableArray<Operand> operands);

    public string Mnemonic { get; private set; }

    public string Keyword { get; private set; }
}

public sealed class PatternClear : InstructionPattern
{
    public PatternClear() : base("CLR", "Clear") { }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        if (operands.Length != 0)
        {
            throw new SyntaxException(lineNumber);
        }

        return 0x00E0;
    }
}

public sealed class PatternReturn : InstructionPattern
{
    public PatternReturn() : base("RTN", "Return") { }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        if (operands.Length != 0)
        {
            throw new SyntaxException(lineNumber);
        }

        return 0x00EE;
    }
}

public sealed class PatternValueToRegister : InstructionPattern
{
    public PatternValueToRegister() : base("VRG", "ValueToRegister") {  }
    
    public override ushort Encode(int lineNumber, ImmutableArray<Operand> operands)
    {
        if (operands.Length != 2)
        {
            throw new SyntaxException(lineNumber);
        }

        if (operands[0].OperandType != OperandType.Register)
        {
            throw new SyntaxException(lineNumber);
        }

        if (operands[1].OperandType != OperandType.Number)
        {
            throw new SyntaxException(lineNumber);
        }
        
        var bytes = new[] { (byte) operands[1].Number, (byte) (0x60 + operands[0].Register) };
        var opcode =  BitConverter.ToUInt16(bytes);

        return opcode;
    }
} 