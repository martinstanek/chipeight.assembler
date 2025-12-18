using System;
using System.Diagnostics;
using System.IO;

namespace ChipEight.Assembler.Host;

public static class Program
{
    private const string DefaultChipSymbolFileExtension = "ch8sym";
    private const string DefaultChip8BinaryExtension = "ch8";
    
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No input file provided");
            return;
        }

        var stopWatch = new Stopwatch();
        
        stopWatch.Start();

        try
        {
            var file = args[0];
            var fileName = Path.GetFileNameWithoutExtension(file);
            var filePath = Path.GetDirectoryName(file) ?? "/";
            var outputBinary = Path.Combine(filePath, $"{fileName}.{DefaultChip8BinaryExtension}" );
            var outputSymbolic = Path.Combine(filePath, $"{fileName}.{DefaultChipSymbolFileExtension}");
            var asm = File.ReadAllText(file);
            var binary = Compiler.Assemble(asm, out var symbols);
            
            File.WriteAllBytes(outputBinary, binary);
            File.WriteAllText(outputSymbolic, symbols);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        
        stopWatch.Stop();
        
        Console.WriteLine($"Compiled: {stopWatch.Elapsed.TotalMilliseconds}ms");
    }
}