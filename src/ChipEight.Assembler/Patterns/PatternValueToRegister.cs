using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternValueToRegister : InstructionPattern
{
    public PatternValueToRegister() : base("VRG", "ValueToRegister") {  }

    public override bool CanDecode(ushort opcode, out string line)
    {
        return CanDecodeOneRegisterAndValue(opcode, out line);
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Number, parsedLine);
        
        var hi = (byte) (0x60 + parsedLine.Operands[0].Register);
        var lo = (byte) parsedLine.Operands[1].Number;
        
        return  BitConverter.ToUInt16([lo, hi]);
    }
}