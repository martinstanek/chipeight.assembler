namespace ChipEight.Assembler;

public sealed class PatternValueToI : InstructionPattern
{
    public PatternValueToI(SymbolMap symbolMap) : base("VI", "ValueToI", symbolMap) { }
    
    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIf(index: 0, OperandType.Register, parsedLine);
        
        var address = parsedLine.Operands[0].OperandType == OperandType.Number
            ? parsedLine.Operands[0].Number
            : Map.GetLabelAddress(parsedLine.LineNumber, parsedLine.Operands[0].Label);

        return (ushort) (0xA000 + address);
    }
}