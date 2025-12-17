using System;

namespace ChipEight.Assembler.Exceptions;

public sealed class SyntaxException : Exception
{
    public SyntaxException(int lineNumber) : base(message: $"Syntax error. Line {lineNumber}") { }
    
    public SyntaxException(string message) : base(message: message) { }
}