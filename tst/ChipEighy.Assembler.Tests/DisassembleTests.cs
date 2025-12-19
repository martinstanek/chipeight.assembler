using ChipEight.Assembler;
using Shouldly;
using Xunit;

namespace ChipEighy.Assembler.Tests;

public sealed class DisassembleTests
{
    [Fact]
    public void Disassemble_InputIsValud_ReturnsAssembly()
    {
        var binary = new byte[]
        {
            0xA0, 0xFF,
            0x22, 0x06,
            0x60, 0x01,
            0x60, 0x0A,
            0x00, 0xEE
        };

        var asm = Compiler.Disassemble(binary);
        
        asm.ShouldNotBeEmpty();
        asm.ShouldStartWith("VI 255");
    }
}