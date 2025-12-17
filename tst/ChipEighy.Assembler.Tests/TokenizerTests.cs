using ChipEight.Assembler;
using Shouldly;
using Xunit;

namespace ChipEighy.Assembler.Tests;

public class TokenizerTests
{
    [Theory]
    [InlineData("CLR", 1)]
    [InlineData("JMP 544", 2)]
    [InlineData("JMP    544 ", 2)]
    public void FromLine_InputIsValid_ReturnsTokens(string asm, int expectedTokens)
    {
        var tokens = Tokenizer.FromLine(asm);
        
        tokens.Length.ShouldBe(expectedTokens);
    }
    
    [Theory]
    [InlineData("CLR", new[] { "CLR" })]
    [InlineData("JMP 544", new[] { "JMP", "544" })]
    [InlineData("JMP    544 ", new[] { "JMP", "544" })]
    public void FromLine_InputIsValid_ReturnsExpectedTokens(string asm, string[] expectedTokens)
    {
        var tokens = Tokenizer.FromLine(asm);
        
        tokens.ShouldBe(expectedTokens);
    }

    [Fact]
    public void FromFile_InputIsValid_ReturnsTokens()
    {
        var asm = """
                  label:
                    CLR
                    JMP 544
                    MRV V3 V4
                    JMP label
                  """;

        var tokens = Tokenizer.FromFile(asm);
        
        tokens.Length.ShouldBe(5);
        tokens[0].ShouldBe(new [] { "label:" });
        tokens[1].ShouldBe(new [] { "CLR" });
        tokens[2].ShouldBe(new [] { "JMP", "544" });
        tokens[3].ShouldBe(new [] { "MRV", "V3", "V4" });
        tokens[4].ShouldBe(new [] { "JMP", "label" });
    }
}