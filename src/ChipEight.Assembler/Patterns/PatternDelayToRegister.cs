using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternDelayToRegister : InstructionPattern
{
    public PatternDelayToRegister() : base("DLR", "DelayToRegister") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        return CanDecodeOneRegister(opcode, end: 0x07, out line);
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xF0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0x07;

        return BitConverter.ToUInt16([lo, hi]);
    }
}