using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternShiftLeftRegister : InstructionPattern
{
    public PatternShiftLeftRegister() : base("SHL", "ShiftLeftRegister") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        return CanDecodeOneRegister(opcode, end: 0xE, out line, halfByteEnd: true);
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        
        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = parsedLine.Operands is [_, { OperandType: OperandType.Register }]
            ? (byte) ((byte)(parsedLine.Operands[1].Register << 4) + 0xE)
            : (byte) 0x0E;

        return BitConverter.ToUInt16([lo, hi]);
    }
}