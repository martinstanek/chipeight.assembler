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
    [InlineData(new [] { "subroutine:" }, "", "subroutine", LineType.Label, 0)]
    public void ParsedLine_InputIsValid_ExpectedTypeAndOperandCount(string[] tokens, string mnemonic, string label, LineType lineType, int operands)
    {
        var line = new ParsedLine(0, tokens.ToImmutableArray());
        
        line.Operands.Length.ShouldBe(operands);
        line.Instruction.ShouldBe(mnemonic);
        line.Label.ShouldBe(label);
        line.LineType.ShouldBe(lineType);
    }
}