using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternSaveRegistersToMemory: InstructionPattern
{
    public PatternSaveRegistersToMemory() : base("SRM", "SaveRegistersToMemory") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xF0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0x55;

        return BitConverter.ToUInt16([lo, hi]);
    }
}