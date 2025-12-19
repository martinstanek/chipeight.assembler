namespace ChipEight.Assembler.Patterns;

public sealed class PatternReturn : InstructionPattern
{
    public PatternReturn() : base("RTN", "Return") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        throw new System.NotImplementedException();
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(count: 0, parsedLine);

        return 0x00EE;
    }
}