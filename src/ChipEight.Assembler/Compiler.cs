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
        
        return this;
    }
    
    public byte[] Encode(SymbolMap symbolMap, ImmutableArray<ParsedLine> lines)
    {
        var binary = new List<byte>();

        foreach (var parsedLine in lines)
        {
            if (parsedLine.LineType == LineType.Instruction)
            {
                var opcode = _patterns[parsedLine.Instruction].Encode(parsedLine.LineNumber, parsedLine.Operands);
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

public class PatternClear : InstructionPattern
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