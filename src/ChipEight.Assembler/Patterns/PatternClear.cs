namespace ChipEight.Assembler.Patterns;

public sealed class PatternClear : InstructionPattern
{
    public PatternClear() : base("CLR", "Clear") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        return CanDecodeEnding(opcode, end: 0xE0, out line);
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(count: 0, parsedLine);

        return 0x00E0;
    }
}