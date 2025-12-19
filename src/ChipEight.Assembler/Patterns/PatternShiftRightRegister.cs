using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternShiftRightRegister : InstructionPattern
{
    public PatternShiftRightRegister() : base("SHR", "ShiftRightRegister") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        throw new NotImplementedException();
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        
        var hi = (byte) (0x80 + parsedLine.Operands[0].Register);
        var lo = parsedLine.Operands is [_, { OperandType: OperandType.Register }]
            ? (byte) ((byte)(parsedLine.Operands[1].Register << 4) + 6)
            : (byte) 0x06;

        return BitConverter.ToUInt16([lo, hi]);
    }
}