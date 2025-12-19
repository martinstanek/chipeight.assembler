using System;

namespace ChipEight.Assembler;

public static class Compiler
{
    public static byte[] Assemble(string asm, out string symbols)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(asm);

        var tokens = Tokenizer.FromFile(asm);
        var lines = Parser.Parse(tokens);
        var symbolMap = SymbolMap.FromParsedLines(lines);
        var binary = Encoder.Build(symbolMap).Encode(lines);

        symbols = symbolMap.ToString();

        return binary;
    }
    
    public static byte[] Assemble(string asm)
    {
        return Assemble(asm, out _);
    }
}