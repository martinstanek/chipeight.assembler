using System;
using System.Diagnostics;
using System.IO;

namespace ChipEight.Assembler.Host;

public static class Program
{
    private const string DefaultChipSymbolFileExtension = "ch8sym";
    private const string DefaultChip8BinaryExtension = "ch8";
    private const string DefaultChip8AssemblyExtension = "ch8asm";
    
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No input file provided");
            return;
        }

        if (args[0].EndsWith(DefaultChip8AssemblyExtension))
        {
            TryAssembly(args[0]);
            return;
        }

        if (args[0].EndsWith(DefaultChip8BinaryExtension))
        {
            TryDisassembly(args[0]);
            return;
        }

        Console.WriteLine("Unknown file provided");
    }

    private static void TryAssembly(string input)
    {
        var stopWatch = new Stopwatch();
        
        stopWatch.Start();

        try
        {
            var file = input;
            var fileName = Path.GetFileNameWithoutExtension(file);
            var filePath = Path.GetDirectoryName(file) ?? "/";
            var outputBinary = Path.Combine(filePath, $"{fileName}.{DefaultChip8BinaryExtension}");
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
        finally
        {
            stopWatch.Stop();
        }
        
        Console.WriteLine($"Compiled: {stopWatch.Elapsed.TotalMilliseconds}ms");
    }
    
    private static void TryDisassembly(string input)
    {
        var stopWatch = new Stopwatch();
        
        stopWatch.Start();

        try
        {
            var file = input;
            var fileName = Path.GetFileNameWithoutExtension(file);
            var filePath = Path.GetDirectoryName(file) ?? "/";
            var outputAssembly = Path.Combine(filePath, $"{fileName}.{DefaultChip8AssemblyExtension}");
            var binary = File.ReadAllBytes(file);
            var assembly = Compiler.Disassemble(binary);

            File.WriteAllText(outputAssembly, assembly);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        finally
        {
            stopWatch.Stop();
        }
        
        Console.WriteLine($"Decompiled: {stopWatch.Elapsed.TotalMilliseconds}ms");
    }
}