## The Chip8 Dis/Assembler

*... I'm not good in machine code, so let's try write an assembler*

The complete [mnemonics](https://github.com/martinstanek/chipeight.assembler/blob/develop/misc/assembler.md) and [xmassapp](https://github.com/martinstanek/chipeight.assembler/blob/develop/misc/xmass.asm) example.

### Usage

```shell
% ./chipeightasm ./xmass.ch8asm                      
Compiled: 11.0147ms
```

Toolchain (vscode extension)
![toolchain](https://github.com/martinstanek/chipeight.assembler/blob/develop/misc/vsc.png?raw=true)

Let's draw a space-ship as seen in Byte Magazine '78:
```asm 
 main:
    VRG V2 0 
    VRG V3 0 
     VI data 
    DRW V2 V3 6
    JMP end
                   
 data: 
    00100000b 01110000b
    01110000b 11111000b
    11011000b 10001000b
                        
 end:
    JMP end 
```

![magazine](https://github.com/martinstanek/chipeight.assembler/blob/develop/misc/magazine.png?raw=true)