# Xmass 2005 test ROM

main:
   CLR
  CALL text
  CALL year 
  CALL song
   JMP end

text:
   CLR
   VRG V1 0
   VRG V2 0
    VI mchar
  CALL drawchar
    VI echar
  CALL drawnextchar
    VI rchar
  CALL drawnextchar
  CALL drawnextchar
    VI ychar
  CALL drawnextchar
   ADD V1 5
    VI xchar 
  CALL drawnextchar
    VI mchar
  CALL drawnextchar
    VI achar
  CALL drawnextchar
    VI schar
  CALL drawnextchar
  CALL drawnextchar
   RTN

year:
  VRG V1 17
  VRG V2 8
  VRG VA 2
 CALL letter
  VRG VA 0
 CALL drawnextletter 
 CALL drawnextletter 
  VRG VA 5
 CALL drawnextletter
   VI exclamationchar
 CALL drawnextchar 
  RTN 

letter:
  FRA VA
  DRW V1 V2 5
  RTN

drawnextletter:
   ADD V1 6
  CALL letter
   RTN

drawnextchar:
   ADD V1 6
  CALL drawchar
   RTN

drawchar:
   DRW V1 V2 5
   RTN  

song:
    VRG V2 60
    CALL play
    VRG V2 15
    CALL pause

    VRG V2 30
    CALL play
    VRG V2 15
    CALL pause

    VRG V2 90
    CALL play
    VRG V2 30
    CALL pause

    VRG V2 30
    CALL play
    VRG V2 15
    CALL pause

    VRG V2 60
    CALL play
    VRG V2 15
    CALL pause

    VRG V2 30
    CALL play
    VRG V2 15
    CALL pause

    VRG V2 90
    CALL play
    VRG V2 30
    CALL pause
    
    RTN

play:
    DLY V2
   BUZZ V2
wait:
    DLR V2      
    SKE V2 0
    JMP wait
    RTN

pause:
    DLY V2
waitpause:
    DLR V2
    SKE V2 0
    JMP waitpause
    RTN      

end:
  JMP end

ychar:
  10001000b 10001000b
  11111000b 00001000b
  00111000b 00000000b

mchar:
  10001000b 11011000b
  10101000b 10001000b
  10001000b 00000000b

echar:
  11111000b 10000000b   
  11100000b 10000000b 
  11111000b 00000000b

rchar:
  11110000b 10001000b
  10010000b 11100000b   
  10001000b 00000000b

xchar:
  10001000b 01010000b
  00100000b 01010000b   
  10001000b 00000000b  

achar:
  00100000b 01010000b 
  11111000b 10001000b     
  10001000b 00000000b 

schar:
  11111000b 10000000b 
  11111000b 00001000b     
  11111000b 00000000b

exclamationchar:
  00100000b 00100000b
  00100000b 00000000b     
  00100000b 00000000b