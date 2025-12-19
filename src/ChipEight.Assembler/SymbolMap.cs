using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using ChipEight.Assembler.Exceptions;

namespace ChipEight.Assembler;

public class SymbolMap
{
    private const ushort StartAddress = 0x200;
    
    private readonly IReadOnlyDictionary<string, ushort> _table;

    public SymbolMap(IReadOnlyDictionary<string, ushort> table)
    {
        _table = table;
    }
    
    public static SymbolMap FromParsedLines(ImmutableArray<ParsedLine> lines)
    {
        var map = new Dictionary<string, ushort>();
        var address = StartAddress;

        foreach (var parsedLine in lines)
        {
            if (parsedLine.LineType == LineType.Label)
            {
                map[parsedLine.Label] = address;
                continue;
            }

            address += 2;
        }
        
        return new SymbolMap(map);
    }

    public static SymbolMap Empty => new(ImmutableDictionary<string, ushort>.Empty);

    public ushort GetLabelAddress(int lineNumber, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        if (!_table.TryGetValue(label, out var address))
        {
            throw new SyntaxException(lineNumber);
        }

        return address;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        foreach (var map in _table)
        {
            sb.AppendLine($"{map.Key}:{map.Value.ToString("X")}");
        }

        return sb.ToString();
    }
}