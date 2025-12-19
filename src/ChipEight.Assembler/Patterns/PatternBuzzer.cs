using System;

namespace ChipEight.Assembler;

public sealed class PatternBuzzer: InstructionPattern
{
    public PatternBuzzer() : base("BUZZ", "Buzzer") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xF0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0x18;

        return BitConverter.ToUInt16([lo, hi]);
    }
}