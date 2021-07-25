// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

// Put your code here.

(CHOOSE_PIXEL)
@KBD
D = M
@SET_WHITE_PIXEL
D ; JEQ
@SET_BLACK_PIXEL
0 ; JMP

(SET_WHITE_PIXEL)
@pixelColor
M = 0
@LOOP_START
0 ; JMP

(SET_BLACK_PIXEL)
@pixelColor
M = 0
M = M - 1
@LOOP_START
0 ; JMP

(LOOP_START)
@SCREEN
D = A
@8192
D = A
@screenLength
M = D
@i
M = 0
(LOOP)
    @i
    D = M
    @screenLength
    D = D - M
    @CHOOSE_PIXEL
    D; JEQ
    @SCREEN
    D = A
    @i
    A = D + M
    D = A
    @pixelAddress
    M = D
    @pixelColor
    D = M
    @pixelAddress
    A = M
    M = D
    @i
    M = M + 1
    @LOOP
    0;JMP