using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternSkipIfNotKey : InstructionPattern
{
    public PatternSkipIfNotKey() : base("SNKEY", "SkipIfNotKey") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xE0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0xA1;

        return BitConverter.ToUInt16([lo, hi]);
    }
}