using System.Collections.Generic;
using ChipEight.Assembler;
using ChipEight.Assembler.Patterns;
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
    public void PatternClear_InputIsValid_Decodes()
    {
        var pattern = new PatternClear();
        var canDecode = pattern.CanDecode(0x00E0, out var line);
        
        canDecode.ShouldBeTrue();
        line.ShouldBe("CLR");
    }
    
    [Fact]
    public void PatternReturn_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["RTN"]);
        var pattern = new PatternReturn();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x00EE);
    }
    
    [Fact]
    public void PatternReturn_InputIsValid_Decodes()
    {
        var pattern = new PatternReturn();
        pattern.CanDecode(0x00EE, out var line).ShouldBeTrue();
        
        line.ShouldBe("RTN");
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
    
    [Fact]
    public void PatternValueToRegister_InputIsValid_Decodes()
    {
        var pattern = new PatternValueToRegister();
        pattern.CanDecode(0x6F0A, out var line).ShouldBeTrue();
        
        line.ShouldBe("VRG VF 10");
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
    
    [Fact]
    public void PatternCall_InputIsValid_Decodes()
    {
        var pattern = new PatternCall(SymbolMap.Empty);
        var canDecode = pattern.CanDecode(0x200A, out var line);
        
        canDecode.ShouldBeTrue();
        line.ShouldBe("CALL 10");
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
    
    [Fact]
    public void PatternValueToI_InputIsValid_Decodes()
    {
        var pattern = new PatternValueToI(SymbolMap.Empty);
        pattern.CanDecode(0xA00A, out var line).ShouldBeTrue();
        
        line.ShouldBe("VI 10");
    }
    
    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    [InlineData("label")]
    public void PatternJumpPlusRegister_InputIsValid_Encodes(string value)
    {
        var symbolMap = new SymbolMap(new Dictionary<string, ushort> { { "label", 0x00A } });
        var parsedLine = new ParsedLine(0, ["JMPR", value]);
        var pattern = new PatternJumpPlusRegister(symbolMap);
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0xB00A);
    }
    
    [Fact]
    public void PatternJumpPlusRegister_InputIsValid_Decodes()
    {
        var pattern = new PatternJumpPlusRegister(SymbolMap.Empty);
        pattern.CanDecode(0xB00A, out var line).ShouldBeTrue();
        
        line.ShouldBe("JMPR 10");
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
    
    [Fact]
    public void PatternDrawSprite_InputIsValid_Decodes()
    {
        var pattern = new PatternDrawSprite();
        pattern.CanDecode(0xD12A, out var line).ShouldBeTrue();
        
        line.ShouldBe("DRW V1 V2 10");
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
    
    [Fact]
    public void PatternJump_InputIsValid_Decodes()
    {
        var pattern = new PatternJump(SymbolMap.Empty);
        pattern.CanDecode(0x100A, out var line).ShouldBeTrue();
        
        line.ShouldBe("JMP 10");
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
    
    [Fact]
    public void PatternSkipIfEqual_InputIsValid_Decodes()
    {
        var pattern = new PatternSkipIfEqual();
        pattern.CanDecode(0x320A, out var line).ShouldBeTrue();
        
        line.ShouldBe("SKE V2 10");
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
    public void PatternSkipIfNotEqual_InputIsValid_Decodes()
    {
        var pattern = new PatternSkipIfNotEqual();
        pattern.CanDecode(0x420A, out var line).ShouldBeTrue();
        
        line.ShouldBe("SKNE V2 10");
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
    public void PatternSkipIfRegistersEqual_InputIsValid_Decodes()
    {
        var pattern = new PatternSkipIfRegistersEqual();
        pattern.CanDecode(0x54F0, out var line).ShouldBeTrue();
        
        line.ShouldBe("SKRE V4 VF");
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
    public void PatternMoveRegisterValues_InputIsValid_Decodes()
    {
        var pattern = new PatternMoveRegisterValues();
        pattern.CanDecode(0x84F0, out var line).ShouldBeTrue();
        
        line.ShouldBe("MRV V4 VF");
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
    public void PatternBitwiseOr_Decodes()
    {
        var pattern = new PatternBitwiseOr();
        var canDecode = pattern.CanDecode(0x84F1, out var line);
        
        canDecode.ShouldBeTrue();
        line.ShouldBe("OR V4 VF");
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
    public void PatternBitwiseAnd_Decodes()
    {
        var pattern = new PatternBitwiseAnd();
        var canDecode = pattern.CanDecode(0x84F2, out var line);
        
        canDecode.ShouldBeTrue();
        line.ShouldBe("AND V4 VF");
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
    public void PatternSubtractRegisters_InputIsValid_Decodes()
    {
        var pattern = new PatternSubtractRegisters();
        pattern.CanDecode(0x84F5, out var line).ShouldBeTrue();

        line.ShouldBe("SUB V4 VF");
    }
    
    [Fact]
    public void PatternSubtractRegistersReverse_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SUBR", "V4", "VF"]);
        var pattern = new PatternSubtractRegistersReverse();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F7);
    }
    
    [Fact]
    public void PatternSubtractRegistersReverse_InputIsValid_Decodes()
    {
        var pattern = new PatternSubtractRegistersReverse();
        pattern.CanDecode(0x84F7, out var line).ShouldBeTrue();

        line.ShouldBe("SUBR V4 VF");
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
    public void PatternSumRegisters_InputIsValid_Decodes()
    {
        var pattern = new PatternSumRegisters();
        pattern.CanDecode(0x84F4, out var line).ShouldBeTrue();
        
        line.ShouldBe("SUM V4 VF");
    }
    
    [Fact]
    public void PatternSkipIfRegistersNotEqual_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SKRNE", "V4", "VF"]);
        var pattern = new PatternSkipIfRegistersNotEqual();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x94F0);
    }
    
    [Fact]
    public void PatternSkipIfRegistersNotEqual_InputIsValid_Decodes()
    {
        var pattern = new PatternSkipIfRegistersNotEqual();
        pattern.CanDecode(0x94F0, out var line).ShouldBeTrue();
        
        line.ShouldBe("SKRNE V4 VF");
    }
    
    [Fact]
    public void PatternShiftRightRegister_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SHR", "V4", "VF"]);
        var pattern = new PatternShiftRightRegister();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84F6);
    }
    
    [Fact]
    public void PatternShiftRightRegister_InputIsValid_Decodes()
    {
        var pattern = new PatternShiftRightRegister();
        pattern.CanDecode(0x84F6, out var line).ShouldBeTrue();
        
        line.ShouldBe("SHR V4");
    }
    
    [Fact]
    public void PatternShiftRightRegister_IgnoredOperandNotUsed_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SHR", "V4"]);
        var pattern = new PatternShiftRightRegister();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x8406);
    }

    [Fact]
    public void PatternAddRegisterToI_IgnoredOperandNotUsed_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["ADDI", "V4"]);
        var pattern = new PatternAddRegisterToI();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF41E);
    }
    
    [Fact]
    public void PatternAddRegisterToI_Decodes()
    {
        var decoded = new PatternAddRegisterToI().CanDecode(0xF41E, out var line);
        
        decoded.ShouldBe(true);
        line.ShouldBe("ADDI V4");
    }
    
    [Fact]
    public void PatternShiftLeftRegister_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SHL", "V4", "VF"]);
        var pattern = new PatternShiftLeftRegister();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x84FE);
    }
    
    [Fact]
    public void PatternShiftLeftRegister_InputIsValid_Decodes()
    {
        var pattern = new PatternShiftLeftRegister();
        pattern.CanDecode(0x84FE, out var line).ShouldBeTrue();
        
        line.ShouldBe("SHL V4");
    }
    
    [Fact]
    public void PatternShiftLeftRegister_IgnoredOperandNotUsed_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SHL", "V4"]);
        var pattern = new PatternShiftLeftRegister();
        var opcode = pattern.Encode(parsedLine);
        
        opcode.ShouldBe((ushort) 0x840E);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("0xA")]
    [InlineData("00001010b")]
    public void PatternRandom_InputIsValid_Encodes(string value)
    {
        var parsedLine = new ParsedLine(0, ["RND", "V4", value]);
        var pattern = new PatternRandom();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xC40A);
    }
    
    [Fact]
    public void PatternRandom_InputIsValid_Decodes()
    {
        var pattern = new PatternRandom();
        pattern.CanDecode(0xC40A, out var line).ShouldBeTrue();

        line.ShouldBe("RND V4 10");
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
    
    [Fact]
    public void PatternAddValueToRegister_Decodes()
    {
        var pattern = new PatternAddValueToRegister();
        var canDecode = pattern.CanDecode(0x720A, out var line);
        
        canDecode.ShouldBe(true);
        line.ShouldBe("ADD V2 10");
    }

    [Fact]
    public void PatternSkipIfKey_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SKEY", "V4"]);
        var pattern = new PatternSkipIfKey();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xE49E);
    }
    
    [Fact]
    public void PatternSkipIfKey_InputIsValid_Decodes()
    {
        var pattern = new PatternSkipIfKey();
        pattern.CanDecode(0xE49E, out var line).ShouldBeTrue();

        line.ShouldBe("SKEY V4");
    }

    [Fact]
    public void PatternSkipIfNotKey_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SNKEY", "V4"]);
        var pattern = new PatternSkipIfNotKey();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xE4A1);
    }

    [Fact]
    public void PatternSkipIfNotKey_InputIsValid_Decodes()
    {
        var pattern = new PatternSkipIfNotKey();
        pattern.CanDecode(0xE4A1, out var line).ShouldBeTrue();

        line.ShouldBe("SNKEY V4");
    }

    [Fact]
    public void PatternDelayToRegister_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["DLR", "V4"]);
        var pattern = new PatternDelayToRegister();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF407);
    }
    
    [Fact]
    public void PatternDelayToRegister_Decodes()
    {
        var pattern = new PatternDelayToRegister();
        pattern.CanDecode(0xF407, out var line).ShouldBeTrue();

        line.ShouldBe("DLR V4");
    }

    [Fact]
    public void PatternWaitForKey_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["WKEY", "V4"]);
        var pattern = new PatternWaitForKey();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF40A);
    }
    
    [Fact]
    public void PatternWaitForKey_InputIsValid_Decodes()
    {
        var pattern = new PatternWaitForKey();
        pattern.CanDecode(0xF40A, out var line).ShouldBeTrue();

        line.ShouldBe("WKEY V4");
    }

    [Fact]
    public void PatternDelay_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["DLY", "V4"]);
        var pattern = new PatternDelay();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF415);
    }
    
    [Fact]
    public void PatternDelay_InputIsValid_Decodes()
    {
        var pattern = new PatternDelay();
        pattern.CanDecode(0xF415, out var line);
        
        line.ShouldBe("DLY V4");
    }

    [Fact]
    public void PatternBuzzer_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["BUZZ", "V4"]);
        var pattern = new PatternBuzzer();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF418);
    }
    
    [Fact]
    public void PatternBuzzer_InputIsValid_Decodes()
    {
        var pattern = new PatternBuzzer();
        var canDecode = pattern.CanDecode(0xF418, out var line);
        
        canDecode.ShouldBeTrue();
        line.ShouldBe("BUZZ V4");
    }

    [Fact]
    public void PatternFontAddressToRegister_InputIsValid_Encodes()
    {
        var pattern = new PatternFontAddressToRegister();
        pattern.CanDecode(0xF429, out var line).ShouldBeTrue();

        line.ShouldBe("FRA V4");
    }

    [Fact]
    public void PatternBinaryCodedDecimal_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["BCD", "V4"]);
        var pattern = new PatternBinaryCodedDecimal();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF433);
    }
    
    [Fact]
    public void PatternBinaryCodedDecimal_Decodes()
    {
        var pattern = new PatternBinaryCodedDecimal();
        var canDecode = pattern.CanDecode(0xF433, out var line);

        canDecode.ShouldBeTrue();
        line.ShouldBe("BCD V4");
    }

    [Fact]
    public void PatternSaveRegistersToMemory_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["SRM", "V4"]);
        var pattern = new PatternSaveRegistersToMemory();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF455);
    }
    
    [Fact]
    public void PatternSaveRegistersToMemory_InputIsValid_Decodes()
    {
        var pattern = new PatternSaveRegistersToMemory();
        pattern.CanDecode(0xF455, out var line).ShouldBeTrue();

        line.ShouldBe("SRM V4");
    }

    [Fact]
    public void PatternLoadRegistersFromMemory_InputIsValid_Encodes()
    {
        var parsedLine = new ParsedLine(0, ["LRM", "V4"]);
        var pattern = new PatternLoadRegistersFromMemory();
        var opcode = pattern.Encode(parsedLine);

        opcode.ShouldBe((ushort) 0xF465);
    }
    
    [Fact]
    public void PatternLoadRegistersFromMemory_InputIsValid_Decodes()
    {
        var pattern = new PatternLoadRegistersFromMemory();
        pattern.CanDecode(0xF465, out var line).ShouldBeTrue();
        
        line.ShouldBe("LRM V4");
    }
}