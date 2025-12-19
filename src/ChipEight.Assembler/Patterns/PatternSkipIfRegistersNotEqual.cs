using System;

namespace ChipEight.Assembler.Patterns;

public sealed class PatternSkipIfRegistersNotEqual : InstructionPattern
{
    public PatternSkipIfRegistersNotEqual() : base("SKRNE", "SkipIfRegistersNotEqual") { }

    public override bool CanDecode(ushort opcode, out string line)
    {
        return CanDecodeTwoRegisters(opcode, end: 0x0, out line);
    }

    protected override ushort EncodeLine(ParsedLine parsedLine)
    {
        ThrowIfNot(index: 0, OperandType.Register, parsedLine);
        ThrowIfNot(index: 1, OperandType.Register, parsedLine);

        var hi = (byte) (0x90 + parsedLine.Operands[0].Register);
        var lo = ((byte)(parsedLine.Operands[1].Register << 4));

        return BitConverter.ToUInt16([lo, hi]);
    }
}