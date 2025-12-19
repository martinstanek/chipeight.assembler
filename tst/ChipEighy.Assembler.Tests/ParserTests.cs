using System.Collections.Immutable;
using ChipEight.Assembler;
using Shouldly;
using Xunit;

namespace ChipEighy.Assembler.Tests;

public sealed class ParserTests
{
    [Theory]
    [InlineData(new [] { "CLR" }, "CLR", "", LineType.Instruction, 0)]
    [InlineData(new [] { "JMP", "0x300" }, "JMP", "", LineType.Instruction, 1)]
    [InlineData(new [] { "MRV", "V3", "V4" }, "MRV", "", LineType.Instruction, 2)]
    [InlineData(new [] { "ADD", "V3", "1" }, "ADD", "", LineType.Instruction, 2)]
    [InlineData(new [] { "subroutine:" }, "", "subroutine", LineType.Label, 0)]
    public void ParsedLine_InputIsValid_ExpectedTypeAndOperandCount(string[] tokens, string mnemonic, string label, LineType lineType, int operands)
    {
        var line = new ParsedLine(0, tokens.ToImmutableArray());
        
        line.Operands.Length.ShouldBe(operands);
        line.Instruction.ShouldBe(mnemonic);
        line.Label.ShouldBe(label);
        line.LineType.ShouldBe(lineType);
    }
    
    [Theory]
    [InlineData(new [] { "0xAA", "0xFF" }, 2, 170)]
    [InlineData(new [] { "0xAAFF" }, 1, 43775)]
    [InlineData(new [] { "10", "11" }, 2, 10)]
    [InlineData(new [] { "00001111b", "11110000b" }, 2, 15)]
    [InlineData(new [] { "00000000b"}, 1, 0)]
    public void ParsedLine_InputIsValidData_ExpectedTypeAndOperandCount(string[] tokens, int operands, ushort value)
    {
        var line = new ParsedLine(0, tokens.ToImmutableArray());
        
        line.Operands.Length.ShouldBe(operands);
        line.Operands[0].Number.ShouldBe(value);
        line.LineType.ShouldBe(LineType.Data);
    }
}