using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternDrawSprite : InstructionPattern
{
    public PatternDrawSprite() : base("DRW", "DrawSprite") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);
        ThrowIfNot(index: 2, OperandType.Number, parsedLine);

        var hi = (byte) (0xD0 + parsedLine.Operands[0].Register);
        var lo = (byte) ((parsedLine.Operands[1].Register << 4) + (byte) parsedLine.Operands[2].Number);

        return BitConverter.ToUInt16([lo, hi]);
    }
}