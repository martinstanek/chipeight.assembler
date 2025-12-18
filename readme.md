## The Chip8 Dis/Assembler

*... I'm not good in machine code, so let's try write an assembler .. work in progress*


```asm 
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
```