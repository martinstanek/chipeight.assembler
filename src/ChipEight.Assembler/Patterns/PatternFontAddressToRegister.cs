using System;

namespace ChipEight.Assembler;

public sealed class PatternFontAddressToRegister: InstructionPattern
{
    public PatternFontAddressToRegister() : base("FRA", "FontAddressToRegister") { }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);

        var hi = (byte) (0xF0 + parsedLine.Operands[0].Register);
        var lo = (byte) 0x29;

        return BitConverter.ToUInt16([lo, hi]);
    }
}