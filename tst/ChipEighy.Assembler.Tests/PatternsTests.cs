using ChipEight.Assembler;
using Shouldly;
using Xunit;

namespace ChipEighy.Assembler.Tests;

public sealed class PatternsTests
{
    [Fact]
    public void PatternClear_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["CLR"]);
        var pattern = new PatternClear();
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0x00E0);
    }
    
    [Fact]
    public void PatternReturn_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["RTN"]);
        var pattern = new PatternReturn();
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0x00EE);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void PatternValueToRegister_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["VRG", "VF", value]);
        var pattern = new PatternValueToRegister();
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0x6F0A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void PatternCall_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["CALL", value]);
        var pattern = new PatternCall();
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0x200A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void ValueToI_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["VI", value]);
        var pattern = new PatternValueToI();
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0xA00A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void DrawSprite_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["DRWS", "V1", "V2", value]);
        var pattern = new PatternDrawSprite();
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0xD12A);
    }
}