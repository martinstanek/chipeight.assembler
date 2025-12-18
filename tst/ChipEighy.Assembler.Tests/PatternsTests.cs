using System.Collections.Generic;
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
    [InlineData("label")]
    public void PatternCall_InputIsValid_Encodes(string value)
    {
        var symbolMap = new SymbolMap(new Dictionary<string, ushort> { { "label", 0x00A } });
        var parsedLine = new ParsedLine(0, ["CALL", value]);
        var pattern = new PatternCall(symbolMap);
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0x200A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    [InlineData("label")]
    public void PatternValueToI_InputIsValid_Encodes(string value)
    {
        var symbolMap = new SymbolMap(new Dictionary<string, ushort> { { "label", 0x00A } });
        var parsedLine = new ParsedLine(0, ["VI", value]);
        var pattern = new PatternValueToI(symbolMap);
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0xA00A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void PatternDrawSprite_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["DRW", "V1", "V2", value]);
        var pattern = new PatternDrawSprite();
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0xD12A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    [InlineData("label")]
    public void PatternJump_InputIsValid_Encodes(string value)
    {
        var symbolMap = new SymbolMap(new Dictionary<string, ushort> { { "label", 0x00A } });
        var parsedLine = new ParsedLine(0, ["JMP", value]);
        var pattern = new PatternJump(symbolMap);
        var opcode = pattern.Encode(0, parsedLine.Operands);
        
        opcode.ShouldBe((ushort) 0x100A);
    }
}