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
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x00E0);
    }
    
    [Fact]
    public void PatternReturn_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["RTN"]);
        var pattern = new PatternReturn();
        var opcode = pattern.Encode(parsedLine);
        
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
        var opcode = pattern.Encode(parsedLine);
        
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
        var opcode = pattern.Encode(parsedLine);
        
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
        var opcode = pattern.Encode(parsedLine);
        
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
        var opcode = pattern.Encode(parsedLine);
        
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
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x100A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void PatternSkipIfEqual_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["SKE", "V2", value]);
        var pattern = new PatternSkipIfEqual();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x320A);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void PatternSkipIfNotEqual_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["SKNE", "V2", value]);
        var pattern = new PatternSkipIfNotEqual();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x420A);
    }
    
    [Fact]
    public void PatternSkipIfRegistersEqual_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SKRE", "V4", "VF"]);
        var pattern = new PatternSkipIfRegistersEqual();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x54F0);
    }
    
    [Fact]
    public void PatternMoveRegisterValues_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["MRV", "V4", "VF"]);
        var pattern = new PatternMoveRegisterValues();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F0);
    }
    
    [Fact]
    public void PatternBitwiseOr_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["OR", "V4", "VF"]);
        var pattern = new PatternBitwiseOr();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F1);
    }
    
    [Fact]
    public void PatternBitwiseAnd_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["AND", "V4", "VF"]);
        var pattern = new PatternBitwiseAnd();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F2);
    }
    
    [Fact]
    public void PatternBitwiseXor_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["XOR", "V4", "VF"]);
        var pattern = new PatternBitwiseXor();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F3);
    }
    
    [Fact]
    public void PatternSubtractRegisters_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SUB", "V4", "VF"]);
        var pattern = new PatternSubtractRegisters();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F5);
    }
    
    [Fact]
    public void PatternSumRegisters_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SUM", "V4", "VF"]);
        var pattern = new PatternSumRegisters();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F4);
    }
    
    [Fact]
    public void PatternShiftRightRegister_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SHR", "V4", "VF"]);
        var pattern = new PatternShiftRightRegister();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F6);
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void PatternAddValueToRegister_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["ADD", "V2", value]);
        var pattern = new PatternAddValueToRegister();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x720A);
    }
}