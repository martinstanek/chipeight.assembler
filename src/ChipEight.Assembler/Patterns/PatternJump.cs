namespace ChipEight.Assembler.Patterns;

public sealed class PatternJump : InstructionPattern
{
    public PatternJump(SymbolMap symbolMap) : base("JMP", "Jump", symbolMap) { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        return CanDecodeValue(opcode, out line);
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIf(index: 0, OperandType.Register, parsedLine);

        var address = parsedLine.Operands[0].OperandType == OperandType.Number
            ? parsedLine.Operands[0].Number
            : Map.GetLabelAddress(parsedLine.LineNumber, parsedLine.Operands[0].Label);

        return (ushort) (0x1000 + address);
    }
}