using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using ChipEight.Assembler.Exceptions;

namespace ChipEight.Assembler;

public static class Parser
{
    public static ImmutableArray<ParsedLine> Parse(ImmutableArray<ImmutableArray<string>> tokens)
    {
        var lines = new List<ParsedLine>();

        for (var l = 0; l < tokens.Length; l++)
        {
            var lineTokens = tokens[l];
            var parsedLine = new ParsedLine(l, lineTokens);
            
            lines.Add(parsedLine);
        }

        return lines.ToImmutableArray();
    }
}

public sealed class ParsedLine
{
    public ParsedLine(int lineNumber, ImmutableArray<string> tokens)
    {
        Instruction = string.Empty;
        Label = string.Empty;
        Operands = [];
        LineType = LineType.Instruction;
        LineNumber = lineNumber;
        
        Parse(lineNumber, tokens);
    }

    private void Parse(int lineNumber, ImmutableArray<string> tokens)
    {
        if (tokens.Length == 0)
        {
            throw new SyntaxException(lineNumber);
        }

        if (tokens.Length == 1 && tokens[0].EndsWith(':'))
        {
            LineType = LineType.Label;
            Label = tokens[0].Replace(":", string.Empty);

            return;
        }

        Instruction = tokens[0];

        var operands = new List<Operand>();
        
        for (var t = 1; t < tokens.Length; t++)
        {
            operands.Add(new Operand(tokens[t]));
        }

        Operands = operands.ToImmutableArray();
    }

    public int LineNumber { get; private set; }

    public string Instruction { get; private set; }

    public string Label { get; private set; }

    public ImmutableArray<Operand> Operands { get; private set; }
    
    public LineType LineType { get; private set; }
}

public sealed class Operand
{
    public Operand(string token)
    {
        OperandType = OperandType.Label;
    }
    
    public OperandType OperandType { get; private set; }
}

public enum LineType
{
    Label,
    Instruction
}

public enum OperandType
{
    Label,
    Register,
    Literal,
    Address
}