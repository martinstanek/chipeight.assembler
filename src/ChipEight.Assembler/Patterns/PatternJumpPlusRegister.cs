namespace ChipEight.Assembler.Patterns;

public sealed class PatternJumpPlusRegister : InstructionPattern
{
    public PatternJumpPlusRegister(SymbolMap symbolMap) : base("JMPR", "JumpPlusRegister", symbolMap) { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        throw new System.NotImplementedException();
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIf(index: 0, OperandType.Register, parsedLine);

        var address = parsedLine.Operands[0].OperandType == OperandType.Number
            ? parsedLine.Operands[0].Number
            : Map.GetLabelAddress(parsedLine.LineNumber, parsedLine.Operands[0].Label);

        return (ushort) (0xB000 + address);
    }
}