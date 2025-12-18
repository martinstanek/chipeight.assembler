using ChipEight.Assembler;
using ChipEight.Machine;
using Shouldly;
using Xunit;

namespace ChipEighy.Assembler.Tests;

public class InstructionsTests
{
    [Fact]
    public void Chip_Clear()
    {
        var asm = """
                  CLR 
                  """;

        var chip = new Chip();
        var binary = Compiler.Assemble(asm);

        chip.Load(binary);
        chip.Run(cycles: 1);
        chip.Registers.Pc.ShouldBe((ushort) 0x202);
    }
    
    [Fact]
    public void Chip_ValueToRegister()
    {
        var asm = """
                  VRG V2 10
                  """;

        var chip = new Chip();
        var binary = Compiler.Assemble(asm);

        chip.Load(binary);
        chip.Run(cycles: 1);
        chip.Registers.V[2].ShouldBe((byte) 10);
    }
    
    [Fact]
    public void Chip_Jump()
    {
        var asm = """
                  JMP 0x206  
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 1);
        
        chip.Registers.Pc.ShouldBe((ushort) 0x206);
    }
    
    [Fact]
    public void Chip_RandomToRegister()
    {
        var asm = """
                  VRG V0 1
                  RND V0 1
                  """;

        var chip = new Chip();
        var binary = Compiler.Assemble(asm);

        chip.Load(binary);
        chip.Run(cycles: 2);
        
        chip.Registers.V[0x0].ShouldNotBe((byte) 0x81);
    }
    
    [Fact]
    public void Chip_Call()
    {
        var asm = """
                  CALL 0x206
                   VRG V0 1
                   VRG V0 10
                   RTN
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);

        chip.Load(binary);
        chip.Run(cycles: 4);
        
        chip.Registers.Pc.ShouldBe((ushort) 0x206);
    }
    
    [Fact]
    public void Chip_SkipIfKey()
    {
        var asm = """
                   VRG V1 2
                  SKEY V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Keypad.Keys[0x2] = true;
        chip.Load(binary);
        chip.Run(cycles: 2);
        
        chip.Registers.Pc.ShouldBe((ushort) 0x206);
    }
    
    [Fact]
    public void Chip_SkipIfNotKey()
    {
        var asm = """
                   VRG V1 2
                  SNKEY V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Keypad.Keys[0x2] = false;
        chip.Load(binary);
        chip.Run(cycles: 2);
        
        chip.Registers.Pc.ShouldBe((ushort) 0x206);
    }
    
    [Fact]
    public void Chip_CallAndReturn()
    {
        var asm = """
                  CALL 0x206
                   VRG V2 1
                   VRG V2 2
                   VRG V2 10
                   RTN
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);

        chip.Load(binary);
        chip.Run(cycles: 6);
        
        chip.Registers.Pc.ShouldBe((ushort) 0x208);
        chip.Registers.Sp.ShouldBe((byte) 0);
        chip.Opcode.ShouldNotBeNull().ShouldBe((ushort) 0x620A );
    }

    [Fact]
    public void Chip_SkipIfEqual()
    {
        var asm = """
                  VRG V1 5
                  SKE V1 5
                  VRG V1 1
                  VRG V1 2
                  """;

        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0x1].ShouldBe((byte) 0x02);
    }
    
    [Fact]
    public void Chip_SkipIfNotEqual()
    {
        var asm = """
                   VRG V1 5
                  SKNE V1 4
                   VRG V1 1
                   VRG V1 2
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[1].ShouldBe((byte) 0x02);
    }

    [Fact]
    public void Chip_SkipIfRegistersEqual()
    {
        var asm = """
                   VRG V0 5
                   VRG V1 5 
                  SKRE V0 V1
                   VRG V0 1
                   VRG V0 2
                  """;

        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 4);
        
        chip.Registers.V[0].ShouldBe((byte) 0x02);
    }
    
    [Fact]
    public void Chip_SkipIfRegistersNotEqual()
    {
        var asm = """
                  VRG V0 5
                  VRG V1 3
                  SKRNE V0 V1
                  VRG V0 1
                  VRG V0 2
                  """;
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 4);
        
        chip.Registers.V[0].ShouldBe((byte) 0x02);
    }

    [Fact]
    public void Chip_RegistersMove()
    {
        var asm = """
                  VRG V0 5
                  VRG V1 3
                  MRV V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0x03);
    }
    
    [Fact]
    public void Chip_RegistersOr()
    {
        var asm = """
                  VRG V0 0
                  VRG V1 0xFF
                   OR V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0xFF);
    }
    
    [Fact]
    public void Chip_RegistersAnd()
    {
        var asm = """
                  VRG V0 0xFF
                  VRG V1 0xF0
                  AND V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0xF0);
    }
    
    [Fact]
    public void Chip_RegistersXor()
    {
        var asm = """
                  VRG V0 0xF0
                  VRG V1 0xF0
                  XOR V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0x00);
    }
    
    [Fact]
    public void Chip_RegistersAdd_NotOverFlow()
    {
        var asm = """
                  VRG V0 1
                  VRG V1 1
                  SUM V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0x02);
        chip.Registers.V[0xF].ShouldBe((byte) 0x0);
    }
    
    [Fact]
    public void Chip_RegistersAdd_WithOverFlow()
    {
        var asm = """
                  VRG V0 0xFF
                  VRG V1 0x02
                  SUM V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0x01);
        chip.Registers.V[0xF].ShouldBe((byte) 0x1);
    }
    
    [Fact]
    public void Chip_RegistersSubtract_NotBorrow()
    {
        var asm = """
                  VRG V0 2
                  VRG V1 1
                  SUB V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0x1);
        chip.Registers.V[0xF].ShouldBe((byte) 0x1);
    }
    
    [Fact]
    public void Chip_RegistersSubtract_WithBorrow()
    {
        var asm = """
                  VRG V0 2
                  VRG V1 1
                  SUB V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
       
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0x1);
        chip.Registers.V[0xF].ShouldBe((byte) 0x1);
    }
   
    [Fact]
    public void Chip_RegistersSubtractReverse_NotBorrow()
    {
        var asm = """
                  VRG V0 1
                  VRG V1 2
                  SUB V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0xFF);
        chip.Registers.V[0xF].ShouldBe((byte) 0x0);
    }
    
    [Fact]
    public void Chip_RegistersSubtractReverse_WithBorrow()
    {
        var asm = """
                  VRG V0 2
                  VRG V1 1
                  SUBR V0 V1
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);

        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0xFF);
        chip.Registers.V[0xF].ShouldBe((byte) 0x0);
    }

    [Fact]
    public void Chip_RegistersShiftRight()
    {
        var asm = """
                  VRG V0 2
                  SHR V0
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 2);
        
        chip.Registers.V[0].ShouldBe((byte) 0x01);
        chip.Registers.V[0xF].ShouldBe((byte) 0x0);
    }
    
    [Fact]
    public void Chip_RegistersShiftLeft()
    {
        var asm = """
                  VRG V0 1
                  SHL V0
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 2);
        
        chip.Registers.V[0].ShouldBe((byte) 0x02);
        chip.Registers.V[0xF].ShouldBe((byte) 0x0);
    }
    
    [Fact]
    public void Chip_AddRegisterToI()
    {
        var asm = """
                   VRG V0 1
                    VI 1
                  ADDI V0 
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0x01);
        chip.Registers.I.ShouldBe((byte) 0x02);
    }
    
    [Fact]
    public void Chip_BCD()
    {
        var asm = """
                  VRG V0 0xFE
                   VI 0x250
                  BCD V0
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 3);
        
        chip.Registers.V[0].ShouldBe((byte) 0xFE);
        chip.Registers.I.ShouldBe((ushort) 0x250);
        chip.Memory.Raw[chip.Registers.I].ShouldBe((byte) 2);
        chip.Memory.Raw[chip.Registers.I + 1].ShouldBe((byte) 5);
        chip.Memory.Raw[chip.Registers.I + 2].ShouldBe((byte) 4);
    }
    
    [Fact]
    public void Chip_StoreRegistersToMemory()
    {
        var asm = """
                  VRG V0 0xFE
                  VRG V1 0xEF
                  VRG V2 0xFF
                   VI 0x250
                  SRM V3
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 5);
        
        chip.Registers.V[0].ShouldBe((byte) 0xFE);
        chip.Registers.V[1].ShouldBe((byte) 0xEF);
        chip.Registers.V[2].ShouldBe((byte) 0xFF);
        chip.Registers.I.ShouldBe((ushort) 0x250);
        chip.Memory.Raw[chip.Registers.I].ShouldBe((byte) 0xFE);
        chip.Memory.Raw[chip.Registers.I + 1].ShouldBe((byte) 0xEF);
        chip.Memory.Raw[chip.Registers.I + 2].ShouldBe((byte) 0xFF);
    }
    
    [Fact]
    public void Chip_LoadRegistersFromMemory()
    {
        var asm = """
                  VRG V0 0xFE
                  VRG V1 0xEF
                  VRG V2 0xFF
                   VI 0x250
                  SRM V3
                  VRG V0 0
                  VRG V1 0
                  VRG V2 0
                  LRM V3
                  """;
        
        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 9);
        
        chip.Registers.V[0].ShouldBe((byte) 0xFE);
        chip.Registers.V[1].ShouldBe((byte) 0xEF);
        chip.Registers.V[2].ShouldBe((byte) 0xFF);
        chip.Registers.I.ShouldBe((ushort) 0x250);
        chip.Memory.Raw[chip.Registers.I].ShouldBe((byte) 0xFE);
        chip.Memory.Raw[chip.Registers.I + 1].ShouldBe((byte) 0xEF);
        chip.Memory.Raw[chip.Registers.I + 2].ShouldBe((byte) 0xFF);
    }
    
    [Fact]
    public void Chip_DrawSprite()
    {
        var asm = """
                    VRG V2 0 
                    VRG V3 0 
                     VI 0x20A 
                    DRW V2 V3 6
                    JMP 0x208
                        00100000b 01110000b
                        01110000b 11111000b
                        11011000b 10001000b
                  """;
        
        var chip = new Chip().WithRemoteDisplay("http://127.0.0.1:8090");
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 6);
        chip.Memory.Raw[0x20A].ShouldBe((byte) 0x20);
        chip.Memory.Raw[0x20A + 1].ShouldBe((byte) 0x70);
        chip.Memory.Raw[0x20A + 2].ShouldBe((byte) 0x70);
        chip.Memory.Raw[0x20A + 3].ShouldBe((byte) 0xF8);
        chip.Memory.Raw[0x20A + 4].ShouldBe((byte) 0xD8);
        chip.Memory.Raw[0x20A + 5].ShouldBe((byte) 0x88);
    }
    
    [Fact]
    public void Chip_DrawSprite_WithLabels()
    {
        var asm = """
                  main:
                    VRG V2 0 
                    VRG V3 0 
                     VI data 
                    DRW V2 V3 6
                    JMP continue
                   
                  data: 
                        00100000b 01110000b
                        01110000b 11111000b
                        11011000b 10001000b
                        
                  continue:
                    CLR    
                  """;

        var chip = new Chip();
        var binary = Compiler.Assemble(asm);
        
        chip.Load(binary);
        chip.Run(cycles: 6);
        chip.Memory.Raw[0x20A].ShouldBe((byte) 0x20);
        chip.Memory.Raw[0x20A + 1].ShouldBe((byte) 0x70);
        chip.Memory.Raw[0x20A + 2].ShouldBe((byte) 0x70);
        chip.Memory.Raw[0x20A + 3].ShouldBe((byte) 0xF8);
        chip.Memory.Raw[0x20A + 4].ShouldBe((byte) 0xD8);
        chip.Memory.Raw[0x20A + 5].ShouldBe((byte) 0x88);
    }
}