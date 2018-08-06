VARIABLE cursorx
VARIABLE cursory
VARIABLE color#
VARIABLE sprite#

: sprites SRAM ;
: /sprite 8 8 * ;
: #sprites 32 16 * 2* ;

\ I/O
: save-sprites  ( addr u -- )
  R/W BIN CREATE-FILE ABORT" Error opening sprite file"
  DUP  sprites  #sprites /sprite *  ROT  WRITE-FILE ABORT" Error writing sprites"
  CLOSE-FILE ABORT" Error closing sprite file" ;

: load-sprites  ( addr u -- )
  R/O BIN OPEN-FILE ABORT" Error opening sprite file"
  DUP  sprites  #sprites /sprite *  ROT  READ-FILE ABORT" Error reading sprites" DROP
  CLOSE-FILE ABORT" Error closing sprite file" ;


\ sprite commands
: >sprite        ( u -- addr )  /sprite * SRAM + ;
: copy-sprite  ( a b -- )       >sprite SWAP >sprite SWAP /sprite MOVE ;


\ editor
: paint-pixel  ( x y -- )  color# @  -ROT  sprite# @ sp! ;
: clear-pixel  ( x y -- )         0  -ROT  sprite# @ sp! ;

: draw-area?     ( x y -- f )   0  64 WITHIN  SWAP   0 64 WITHIN  AND ;
: color-area?    ( x y -- f )  48  56 WITHIN  SWAP  80 88 WITHIN  AND ;
: palette-area?  ( x y -- f )  72 104 WITHIN  SWAP   0 32 WITHIN  AND ;

: clicked-color  ( x y -- u )  72 - 8 / 4 *  SWAP 8 / +  ;

: +color   ( -- )   color# @ 1+  16 MOD        color# ! ;
: -color   ( -- )   color# @ 1-  16 MOD 0 MAX  color# ! ;
: +sprite  ( -- )  sprite# @ 1+ 256 MOD       sprite# ! ;
: -sprite  ( -- )  sprite# @ 1- 256 MOD 0 MAX sprite# ! ;
                    
: update-mouse  ( -- )
  MOUSEX @ MOUSEY @ draw-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  paint-pixel  ENDOF
      4 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  clear-pixel  ENDOF
    ENDCASE
  THEN
  MOUSEX @ MOUSEY @ color-area? IF
    MOUSEB @ CASE
      1 OF  +color  ENDOF
      4 OF  -color  ENDOF
    ENDCASE
  THEN
  MOUSEX @ MOUSEY @ palette-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @ MOUSEY @ clicked-color color# ! ENDOF
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
  SCANCODE_N     just-pressed?  IF  -sprite  THEN
  SCANCODE_M     just-pressed?  IF  +sprite  THEN
  SCANCODE_F1    just-pressed?  IF  s" default.spr" save-sprites THEN
  SCANCODE_F2    just-pressed?  IF  s" default.spr" load-sprites THEN
  SCANCODE_F4    just-pressed?  IF  sprites 64 dump CR THEN
  SCANCODE_SPACE just-pressed?  IF  cursorx @ cursory @ paint-pixel THEN
  SCANCODE_Q     just-pressed?  IF  retro-40  THEN
;

: <update>  ( -- )  update-mouse update-keys ;

: help  ( -- )
  S" WASD  - move"         15 0 128 puts
  S"  SPC  - paint"        15 0 136 puts
  S"  C V  - cycle color"  15 0 144 puts
  S"  N M  - cycle sprite" 15 0 152 puts
  S" F1 F2 - save/load"    15 0 160 puts
  S"    Q  - quit"         15 0 168 puts
;

: spritesheet
  ( bg @ ) 3 96 0 32 8 * 16 8 * rect
  96 0  16 32  0 spr*
;

: palette-display
  4 0 DO
    4 0 DO
      I 4 * J +
      0 J 8 * +  72 I 8 * +
      8 8 rect
    LOOP
  LOOP
;

: zoomed-display   6 0 0 65 65 rectb  8 scale !  0 0 sprite# @ spr  1 scale ! ;
: preview-display  80 32 sprite# @ spr ;

: cursor-display
  14  8 cursorx @ *       8 cursory @ *  8 8 rectb
  14  8 sprite# @ 16 MOD * 96 +  8 sprite# @ 16 / *  8 8 rectb
;

: color-display
  color# @ 80 48 8 8 rect
  color# @ = IF  15  0 J 8 * +  72 I 8 * +  8 8 rectb  THEN
;

: <draw>
  palette-display
  zoomed-display
  preview-display
  color-display
  spritesheet
  cursor-display
  help ;

: <init>  0 cls  1 color# ! ;

\ : update  <update> <draw> ;

INSTALL sprite-editor
