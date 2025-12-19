using ChipEight.Assembler;
using Shouldly;
using Xunit;

namespace ChipEighy.Assembler.Tests;

public class SplitterTests
{
    [Fact]
    public void FromBinary_Splits()
    {
        var binary = new byte[]
        {
            0x22, 0x06,
            0x60, 0x01,
            0x60, 0x0A,
            0x00, 0xEE
        };

        var opcodes = Splitter.FromBinary(binary);
        
        opcodes.Length.ShouldBe(4);
        opcodes[0].ShouldBe((ushort) 0x2206);
        opcodes[3].ShouldBe((ushort) 0x00EE);
    }
}