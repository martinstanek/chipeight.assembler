using System;

namespace ChipEight.Assembler;

public sealed class PatternSkipIfEqual : InstructionPattern
{
    public PatternSkipIfEqual() : base("SKE", "SkipIfEqual") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Number, parsedLine);

        var hi = (byte) (0x30 + parsedLine.Operands[0].Register);
        var lo = (byte) parsedLine.Operands[1].Number;

        return BitConverter.ToUInt16([lo, hi]);
    }
}