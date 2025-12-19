using System;
using System.Collections.Immutable;
using System.Linq;

namespace ChipEight.Assembler;

public static class Splitter
{
    public static ImmutableArray<ushort> FromBinary(byte[] binary)
    {
        var chunks = binary.Chunk(2);
        var opcodes = chunks.Select(s => BitConverter.ToUInt16(s.Reverse().ToArray()));

        return opcodes.ToImmutableArray();
    }
}