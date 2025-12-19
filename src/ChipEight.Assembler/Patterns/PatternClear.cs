namespace ChipEight.Assembler.Patterns;

public sealed class PatternClear : InstructionPattern
{
    public PatternClear() : base("CLR", "Clear") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        throw new System.NotImplementedException();
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(count: 0, parsedLine);

        return 0x00E0;
    }
}