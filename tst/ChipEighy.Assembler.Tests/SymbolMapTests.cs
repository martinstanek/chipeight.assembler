using ChipEight.Assembler;
using Shouldly;
using Xunit;

namespace ChipEighy.Assembler.Tests;

public sealed class SymbolMapTests
{
    [Fact]
    public void SymbolMap_InputIsValid_MapIsCreated()
    {
        var asm = """
                  main:
                      CALL sub
                      CLR
                  
                  sub:
                      RTN
                  """;
        
        var tokens = Tokenizer.FromFile(asm);
        var lines = Parser.Parse(tokens);
        var symbolMap = SymbolMap.FromParsedLines(lines);
        
        symbolMap.GetLabelAddress(0, "main").ShouldBe((ushort) 0x200);
        symbolMap.GetLabelAddress(0, "sub").ShouldBe((ushort) 0x204);
    }
}