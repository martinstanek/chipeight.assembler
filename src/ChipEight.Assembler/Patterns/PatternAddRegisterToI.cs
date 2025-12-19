using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternAddRegisterToI : InstructionPattern
{
    public PatternAddRegisterToI() : base("ADDI", "AddRegisterToI") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(count: 1, parsedLine);
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xF0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0x1E;

        return BitConverter.ToUInt16([lo, hi]);
    }
}