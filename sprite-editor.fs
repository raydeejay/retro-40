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
: >sprite        ( u -- addr )  DUP 16 / 16 * /sprite *  SWAP  16 MOD 8 * +  SRAM + ;
: copy-sprite  ( src dst -- )
  >sprite SWAP >sprite SWAP
  8 0 DO
    2DUP
    8 MOVE
    16 8 * + SWAP 16 8 * + SWAP
  LOOP
  2DROP
;


\ screen areas
: draw-area?     ( x y -- f )   8  72 WITHIN  SWAP   8  72 WITHIN  AND ;
: clicked-pixel   ( x y -- u )  8 - 8 /  SWAP  8 - 8 /  SWAP  ;

: palette-area?  ( x y -- f )  72 104 WITHIN  SWAP   0  32 WITHIN  AND ;
: clicked-color   ( x y -- u )  72 - 8 / 4 *  SWAP 8 / +  ;

: sheet-area?    ( x y -- f )   8 136 WITHIN  SWAP  88 216 WITHIN  AND ;
: clicked-sprite  ( x y -- u )  8 - 8 / 16 *  SWAP 88 - 8 / +  ;

\ actions
: paint-pixel  ( x y -- )  color# @  -ROT  sprite# @ sp! ;
: clear-pixel  ( x y -- )         0  -ROT  sprite# @ sp! ;
: +color   ( -- )   color# @ 1+  16 MOD        color# ! ;
: -color   ( -- )   color# @ 1-  16 MOD 0 MAX  color# ! ;
: +sprite  ( -- )  sprite# @ 1+ 256 MOD       sprite# ! ;
: -sprite  ( -- )  sprite# @ 1- 256 MOD 0 MAX sprite# ! ;


\ input handler
: update-mouse  ( -- )
  MOUSEX @ MOUSEY @ draw-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @  MOUSEY @  clicked-pixel paint-pixel  ENDOF
      4 OF  MOUSEX @  MOUSEY @  clicked-pixel clear-pixel  ENDOF
    ENDCASE
  THEN
  MOUSEX @ MOUSEY @ palette-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @ MOUSEY @ clicked-color color# ! ENDOF
    ENDCASE
  THEN
  MOUSEX @ MOUSEY @ sheet-area? IF
    MOUSEB @ CASE
      1 OF  MOUSEX @ MOUSEY @ clicked-sprite sprite# ! ENDOF
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



: zoomed-display
  6 7 7 66 66 rectb
  8 scale !
  8 8 sprite# @ spr
  1 scale !
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

: spritesheet
  3  87 7  16 8 * 2 +  16 8 * 2 +  rectb
  88 8  16 16  0 spr*
;

\ : help  ( -- )
\   S" WASD  - move"         15 0 128 puts
\   S"  SPC  - paint"        15 0 136 puts
\   S"  C V  - cycle color"  15 0 144 puts
\   S"  N M  - cycle sprite" 15 0 152 puts
\   S" F1 F2 - save/load"    15 0 160 puts
\   S"    Q  - quit"         15 0 168 puts
\ ;

\ : color-display    ( -- )  color# @ 80 48 8 8 rect ;
\ : preview-display  ( -- )  80 32 sprite# @ spr ;
: zoomed-cursor    ( -- )  14  8 cursorx @ * 8 +          8 cursory @ * 8 +       8 8 rectb ;
: sprite-cursor    ( -- )  14  8 sprite# @ 16 MOD * 88 +  8 sprite# @ 16 / * 8 +  8 8 rectb ;
: cursor-display   ( -- )  zoomed-cursor sprite-cursor ;

: <draw>
  zoomed-display
  palette-display
  spritesheet
\   color-display
\  preview-display
  cursor-display
\  help
;

: <init>  0 cls  1 color# ! ;

\ : update  <update> <draw> ;

INSTALL sprite-editor
