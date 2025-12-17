using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ChipEight.Assembler;

public sealed class Compiler
{
    public static byte[] Assemble(string asm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(asm);

        var tokens = Tokenizer.FromFile(asm);
        var lines = Parser.Parse(tokens);
        var symbolMap = SymbolMap.FromParsedLines(lines);
        var binary = Encoder.Encode(symbolMap, lines);

        return binary;
    }
}

public class Parser
{
    public static ImmutableArray<ParsedLine> Parse(ImmutableArray<ImmutableArray<string>> tokens)
    {
        var lines = new List<ParsedLine>();

        foreach (var lineTokens in tokens)
        {
            var parsedLine = FromLine(lineTokens);
            
            lines.Add(parsedLine);
        }

        return lines.ToImmutableArray();
    }

    private static ParsedLine FromLine(ImmutableArray<string> tokens)
    {
        return new ParsedLine();
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
    public static byte[] Encode(SymbolMap symbolMap, ImmutableArray<ParsedLine> lines)
    {
        return [];
    }
}

public class Tokenizer
{
    public static ImmutableArray<ImmutableArray<string>> FromFile(string asm)
    {
        var tokenLines = new List<ImmutableArray<string>>();

        foreach (var line in asm.Split(Environment.NewLine))
        {
            var tokens = FromLine(line);

            tokenLines.Add(tokens);
        }

        return tokenLines.ToImmutableArray();
    }

    public static ImmutableArray<string> FromLine(string line)
    {
        return [];
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

    public abstract ushort Encode(params Operand[] operands);

    public string Mnemonic { get; private set; }

    public string Keyword { get; private set; }
}

public class PatternClear : InstructionPattern
{
    public PatternClear() : base("CLR", "Clear") { }
    
    public override ushort Encode(params Operand[] operands)
    {
        return 0x00E0;
    }
}

public class Operand
{
    public Operand(string token)
    {
        OperandType = OperandType.Comment;
    }
    
    public OperandType OperandType { get; private set; }
}

public class ParsedLine
{
    public ParsedLine(params string[] tokens)
    {
        Instruction = string.Empty;
        Operands = [];
        LineType = LineType.Comment;
    }

    public string Instruction { get; private set; }

    public ImmutableArray<Operand> Operands { get; private set; }
    
    public LineType LineType { get; private set; }
}

public enum LineType
{
    Comment,
    Label,
    Instruction
}

public enum OperandType
{
    Comment,
    Label,
    Register,
    Literal,
    Address
}