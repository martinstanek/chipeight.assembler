using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternBitwiseXor : InstructionPattern
{
    public PatternBitwiseXor() : base("XOR", "BitwiseXor") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        return CanDecodeTwoRegisters(opcode, end: 3, out line);
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = (byte) ((byte) (parsedLine.Operands[1].Register << 4) + 3);

        return BitConverter.ToUInt16([lo, hi]);
    }
}