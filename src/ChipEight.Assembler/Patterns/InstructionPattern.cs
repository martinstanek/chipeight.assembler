using System;
using ChipEight.Assembler.Exceptions;

namespace ChipEight.Assembler.Patterns;

public abstract class InstructionPattern
{
    protected InstructionPattern(string mnemonic, string keyword) : this(mnemonic, keyword, SymbolMap.Empty) { }

    protected InstructionPattern(string mnemonic, string keyword, SymbolMap symbolMap)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mnemonic);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyword);

        Map = symbolMap;
        Mnemonic = mnemonic;
        Keyword = keyword;
    }

    public ushort Encode(ParsedLine parsedLine)
    {
        ThrowIfNotMnemonic(parsedLine);

        return EncodeLine(parsedLine);
    }

    public abstract bool CanDecode(ushort opcode, out string line);
    
    protected abstract ushort EncodeLine(ParsedLine parsedLine);

    protected static void ThrowIfNot(byte index, OperandType type, ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length < index + 1)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }

        if (parsedLine.Operands[index].OperandType != type)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }

        if (type == OperandType.Register && parsedLine.Operands[index].Register > 0xF)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }
    
    protected static void ThrowIf(byte index, OperandType type, ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length < index + 1)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
        
        if (parsedLine.Operands[index].OperandType == type)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }
    
    protected static void ThrowIfNot(byte count, ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length != count)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }

    protected bool CanDecodeTwoRegisters(ushort opcode, byte end, out string line)
    {
        line = string.Empty;
        var reg1 = (byte) ((opcode & 0x0F00) >> 8);
        var reg2 = (byte) ((opcode & 0x00F0) >> 4);
        var ending = (byte) opcode & 0x000F;

        if (reg1 > 0xF || reg2 > 0xF || ending != end)
        {
            return false;
        }

        line = $"{Mnemonic} V{reg1:X} V{reg2:X}";

        return true;
    }

    protected bool CanDecodeOneRegister(ushort opcode, byte end, out string line, bool halfByteEnd = false)
    {
        line = string.Empty;
        var reg = (byte) ((opcode & 0x0F00) >> 8);
        var ending = halfByteEnd 
            ? (byte) opcode & 0x000F
            : (byte) opcode & 0x00FF;

        if (ending != end|| reg > 0xF)
        {
            return false;
        }

        line = $"{Mnemonic} V{reg:X}";

        return true;
    }

    protected bool CanDecodeOneRegisterAndValue(ushort opcode, out string line)
    {
        line = string.Empty;
        var reg = (byte) ((opcode & 0x0F00) >> 8);
        var val = (byte) opcode & 0x00FF;

        if (reg > 0xF)
        {
            return false;
        }

        line = $"{Mnemonic} V{reg:X} {val}";

        return true;
    }

    protected bool CanDecodeTwoRegistersAndValue(ushort opcode, out string line)
    {
        line = string.Empty;
        var reg1 = (byte) ((opcode & 0x0F00) >> 8);
        var reg2 = (byte) ((opcode & 0x00F0) >> 4);
        var val = (byte) opcode & 0x000F;

        if (reg1 > 0xF || reg2 > 0xF)
        {
            return false;
        }

        line = $"{Mnemonic} V{reg1:X} V{reg2:X} {val}";

        return true;
    }

    protected bool CanDecodeValue(ushort opcode, out string line)
    {
        line = string.Empty;
        var val = opcode & 0x0FFF;

        if (val > 0xFFF)
        {
            return false;
        }

        line = $"{Mnemonic} {val}";

        return true;
    }

    protected bool CanDecodeEnding(ushort opcode, byte end, out string line)
    {
        line = string.Empty;
        var ending = (byte) opcode & 0x00FF;

        if (ending != end)
        {
            return false;
        }

        line = Mnemonic;

        return true;
    }

    private void ThrowIfNotMnemonic(ParsedLine parsedLine)
    {
        if (parsedLine.LineType != LineType.Instruction 
            || !parsedLine.Instruction.Equals(Mnemonic, StringComparison.OrdinalIgnoreCase))
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }
    
    public string Mnemonic { get; }

    public string Keyword { get; private set; }
    
    public byte Group { get; }
    
    protected SymbolMap Map { get; }
}