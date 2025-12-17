using System;

namespace ChipEight.Assembler;

public sealed class Compiler
{
    public static byte[] Assemble(string asm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(asm);
        
        return [0x00, 0xE0];
    }
}