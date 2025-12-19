using System;

namespace ChipEight.Assembler;

public static class Compiler
{
    public static byte[] Assemble(string asm, out string symbolMap)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(asm);

        var tokens = Tokenizer.FromFile(asm);
        var lines = Parser.Parse(tokens);
        var symbols = SymbolMap.Collect(lines);
        var binary = Encoder.Build(symbols).Encode(lines);

        symbolMap = symbols.ToString();

        return binary;
    }
    
    public static byte[] Assemble(string asm)
    {
        return Assemble(asm, out _);
    }
}