using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternAddRegisterToI : InstructionPattern
{
    public PatternAddRegisterToI() : base("ADDI", "AddRegisterToI") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        line = string.Empty;
        var reg = (byte) ((opcode & 0x0F00) >> 8);
        var ending = (byte) opcode & 0x00FF;

        if (ending != 0x1E || reg > 0xF)
        {
            return false;
        }

        line = $"{Mnemonic} V{reg}";

        return true;
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(count: 1, parsedLine);
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xF0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0x1E;

        return BitConverter.ToUInt16([lo, hi]);
    }
}