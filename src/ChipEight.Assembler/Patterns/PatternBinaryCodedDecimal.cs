using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternBinaryCodedDecimal: InstructionPattern
{
    public PatternBinaryCodedDecimal() : base("BCD", "BinaryCodedDecimal") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        throw new NotImplementedException();
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xF0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0x33;

        return BitConverter.ToUInt16([lo, hi]);
    }
}