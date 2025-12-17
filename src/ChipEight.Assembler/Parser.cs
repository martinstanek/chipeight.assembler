using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
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
            operands.Add(new Operand(lineNumber, tokens[t]));
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
    public Operand(int lineNumber, string token)
    {
        OperandType = OperandType.LabelReference;
        Register = 0;
        Number = 0;
        Label = string.Empty;
        
        Parse(lineNumber, token);   
    }

    private void Parse(int lineNumber, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SyntaxException(lineNumber);
        }

        if (ushort.TryParse(token, NumberStyles.Integer, null, out var deciNumber))
        {
            OperandType = OperandType.Number;
            Number = deciNumber;
            
            return;
        }

        if (ushort.TryParse(token.Replace("0x", ""), NumberStyles.HexNumber, null, out var hexNumber))
        {
            OperandType = OperandType.Number;
            Number = hexNumber;

            return;
        }
        
        if (ushort.TryParse(token.Replace("b", ""), NumberStyles.BinaryNumber, null, out var binNumber))
        {
            OperandType = OperandType.Number;
            Number = binNumber;

            return;
        }

        if (token.Length == 2 && token.ToUpper()[0] == 'V')
        {
            var indexChar = token[1];
            if (!byte.TryParse($"{indexChar}", NumberStyles.HexNumber, null, out var registerIndex))
            {
                throw new SyntaxException(lineNumber);
            }

            OperandType = OperandType.Register;
            Register = registerIndex;
            
            return;
        }

        OperandType = OperandType.LabelReference;
        Label = token;
    }

    public OperandType OperandType { get; private set; }

    public byte Register { get; private set; }

    public ushort Number { get; private set; }

    public string Label { get; private set; }
}

public enum LineType
{
    Label,
    Instruction
}

public enum OperandType
{
    LabelReference,
    Register,
    Number
}