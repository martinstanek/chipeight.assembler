using System;

namespace ChipEight.Assembler;

public sealed class PatternBitwiseOr : InstructionPattern
{
    public PatternBitwiseOr() : base("OR", "BitwiseOr") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) ((byte) (parsedLine.Operands[1].Register << 4) + 1);

        return BitConverter.ToUInt16([lo, hi]);
    }
}