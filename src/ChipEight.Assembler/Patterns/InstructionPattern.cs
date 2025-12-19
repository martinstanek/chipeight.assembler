using System;
using ChipEight.Assembler.Exceptions;

namespace ChipEight.Assembler;

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

    public virtual ushort Encode(ParsedLine parsedLine)
    {
        ThrowIfNotMnemonic(parsedLine);

        return EncodeLine(parsedLine);
    }

    protected abstract ushort EncodeLine(ParsedLine parsedLine);

    protected void ThrowIfNotMnemonic(ParsedLine parsedLine)
    {
        if (parsedLine.LineType != LineType.Instruction 
            || !parsedLine.Instruction.Equals(Mnemonic, StringComparison.OrdinalIgnoreCase))
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }

    protected void ThrowIfNot(byte index, OperandType type, ParsedLine parsedLine)
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
    
    protected void ThrowIf(byte index, OperandType type, ParsedLine parsedLine)
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
    
    protected void ThrowIfNot(byte count, ParsedLine parsedLine)
    {
        if (parsedLine.Operands.Length != count)
        {
            throw new SyntaxException(parsedLine.LineNumber);
        }
    }

    protected SymbolMap Map { get; }
    
    public string Mnemonic { get; }

    public string Keyword { get; private set; }
}