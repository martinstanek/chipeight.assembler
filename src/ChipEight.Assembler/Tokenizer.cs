using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ChipEight.Assembler;

public static class Tokenizer
{
    public static ImmutableArray<ImmutableArray<string>> FromFile(string asm)
    {
        var tokenLines = new List<ImmutableArray<string>>();

        foreach (var line in asm.Split(Environment.NewLine))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.TrimStart().StartsWith(Language.Comment))
            {
                continue;
            }

            var tokens = FromLine(line);

            tokenLines.Add(tokens);
        }

        return tokenLines.ToImmutableArray();
    }

    public static ImmutableArray<string> FromLine(string line)
    {
        var split = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        return split.Select(s => s.Trim()).ToImmutableArray();
    }
}