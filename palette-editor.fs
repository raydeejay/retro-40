VARIABLE color#
VARIABLE palette#

: palettes MRAM ;
: /palette 16 3 * ;
: #palettes 16 ;

\ I/O
: save-palettes  ( addr u -- )
  R/W BIN CREATE-FILE ABORT" Error opening palette file"
  DUP  palettes  #palettes /palette *  ROT  WRITE-FILE ABORT" Error writing palettes"
  CLOSE-FILE ABORT" Error closing palette file" ;

: load-palettes  ( addr u -- )
  R/O BIN OPEN-FILE ABORT" Error opening palette file"
  DUP  palettes  #palettes /palette *  ROT  READ-FILE ABORT" Error reading palettes" DROP
  CLOSE-FILE ABORT" Error closing palette file" ;

17 FICL-VOCABULARY palette-editor-voc
ALSO palette-editor-voc DEFINITIONS


\ palette commands
: >palette        ( u -- addr )  DUP 16 / 16 * /palette *  SWAP  16 MOD 8 * +  SRAM + ;
: copy-palette  ( src dst -- )
  >palette SWAP >palette SWAP
  8 0 DO
    2DUP
    8 MOVE
    16 8 * + SWAP 16 8 * + SWAP
  LOOP
  2DROP
;


\ editor
: paint-pixel  ( x y -- )  color# @  -ROT  palette# @ sp! ;
: clear-pixel  ( x y -- )         0  -ROT  palette# @ sp! ;

: palette-area?  ( x y -- f )  72 104 WITHIN  SWAP   0 32 WITHIN  AND ;
: r-area?  ( x y -- f )   0  8 WITHIN  NIP ;
: g-area?  ( x y -- f )   8 16 WITHIN  NIP ;
: b-area?  ( x y -- f )  16 24 WITHIN  NIP ;

: update-r  ( u -- )
  color# @ palette# @ PAL@
  ROT DROP
  color# @ palette# @ pal!
;

: update-g  ( u -- )
  color# @ palette# @ PAL@
  NIP >R SWAP R>
  color# @ palette# @ pal!
;

: update-b  ( u -- )
  color# @ palette# @ PAL@
  DROP ROT
  color# @ palette# @ pal!
;

: clicked-pixel   ( x y -- u )  8 - 8 /  SWAP  8 - 8 /  SWAP  ;
: clicked-color   ( x y -- u )  72 - 8 / 4 *  SWAP 8 / +  ;
: clicked-palette  ( x y -- u )  8 - 8 / 16 *  SWAP 88 - 8 / +  ;

: +color   ( -- )   color# @ 1+  16 MOD        color# ! ;
: -color   ( -- )   color# @ 1-  16 MOD 0 MAX  color# ! ;
: +palette  ( -- )  palette# @ 1+ 256 MOD       palette# ! ;
: -palette  ( -- )  palette# @ 1- 256 MOD 0 MAX palette# ! ;

: update-mouse  ( -- )
  MOUSEX @ MOUSEY @ r-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @  update-r  ENDOF
    ENDCASE
  THEN
  MOUSEX @ MOUSEY @ g-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @  update-g  ENDOF
    ENDCASE
  THEN
  MOUSEX @ MOUSEY @ b-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @  update-b  ENDOF
    ENDCASE
  THEN
  MOUSEX @ MOUSEY @ palette-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @ MOUSEY @ clicked-color color# !  ENDOF
    ENDCASE
  THEN
;

: update-keys  ( -- )
  SCANCODE_W     just-pressed?  IF  cursory @ 1- 8 MOD  DUP 0< IF DROP 7 THEN  cursory ! THEN
  SCANCODE_S     just-pressed?  IF  cursory @ 1+ 8 MOD  DUP 0< IF DROP 7 THEN  cursory ! THEN
  SCANCODE_A     just-pressed?  IF  cursorx @ 1- 8 MOD  DUP 0< IF DROP 7 THEN  cursorx ! THEN
  SCANCODE_D     just-pressed?  IF  cursorx @ 1+ 8 MOD  DUP 0< IF DROP 7 THEN  cursorx ! THEN
  SCANCODE_C     just-pressed?  IF   -color  THEN
  SCANCODE_V     just-pressed?  IF   +color  THEN
  SCANCODE_N     just-pressed?  IF  -palette  THEN
  SCANCODE_M     just-pressed?  IF  +palette  THEN
  SCANCODE_F1    just-pressed?  IF  s" default.spr" save-palettes THEN
  SCANCODE_F2    just-pressed?  IF  s" default.spr" load-palettes THEN
  SCANCODE_F4    just-pressed?  IF  palettes 64 dump CR THEN
  SCANCODE_SPACE just-pressed?  IF  cursorx @ cursory @ paint-pixel THEN
  SCANCODE_Q     just-pressed?  IF  retro-40  THEN
;

: <update>  ( -- )  update-mouse update-keys ;

: zoomed-display
  0 0 0 256 24 rect
  15 0  0 256 8 rectb
  15 0  8 256 8 rectb
  15 0 16 256 8 rectb
  color# @ palette# @ pal@ { r g b }
  15 r  0 r  8 line
  15 g  8 g 16 line
  15 b 16 b 24 line
;

: numeric-display
  color# @ palette# @ pal@ { r g b }
  5 5 at-xy s" R: " type r .
  5 6 at-xy s" G: " type g .
  5 7 at-xy s" B: " type b .
;

: palette-display
  4 0 DO
    4 0 DO
      I 4 * J +  DUP
      0 J 8 * +  72 I 8 * +
      8 8 rect
      color# @ = IF  15  0 J 8 * +  72 I 8 * +  8 8 rectb  THEN
    LOOP
  LOOP
;

: <draw>
  zoomed-display
  numeric-display
  palette-display
;

: <init>  0 cls  1 color# ! ;


\ install the software
PREVIOUS DEFINITIONS

ALSO palette-editor-voc

INSTALL palette-editor

PREVIOUS
