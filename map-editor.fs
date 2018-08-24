\ map-editor.fs


\ TODO:
\ movement by half/one screen at a time
\ select blocks of sprites
\ hold space to move with the mouse
\ display screen number
\ scroll spritesheet

\ I/O
: #map W H * ;

: save-map  ( addr u -- )
  R/W BIN CREATE-FILE ABORT" Error opening map file"
  DUP  MRAM   #map  ROT  WRITE-FILE ABORT" Error writing map"
  DUP  MRAM2  #map  ROT  WRITE-FILE ABORT" Error writing map"
  CLOSE-FILE ABORT" Error closing map file"
;

: load-map  ( addr u -- )
  R/O BIN OPEN-FILE ABORT" Error opening map file"
  DUP  MRAM   #map  ROT  READ-FILE ABORT" Error reading map" DROP
  DUP  MRAM2  #map  ROT  READ-FILE ABORT" Error reading map" DROP
  CLOSE-FILE ABORT" Error closing map file"
;

17 ficl-vocabulary map-editor-voc
also map-editor-voc definitions

\ variables and initialisation/shutdown
0 VALUE image
1 VALUE foreground?
1 VALUE background?
0 VALUE edit-foreground?

VARIABLE sprite#
VARIABLE sprite-page#
VARIABLE mapx
VARIABLE mapy


\ editor
: +sprite-page  ( -- )   sprite-page# @ 1+ 256 MOD  sprite-page# ! ;
: -sprite-page  ( -- )   sprite-page# @ 1- 256 MOD  sprite-page# ! ;
: +mapx    ( -- )   mapx @ 1+ 256 32 - MOD  mapx ! ;
: -mapx    ( -- )   mapx @ 1- 256 32 - MOD  0 MAX mapx ! ;
: +mapy    ( -- )   mapy @ 1+ 192 24 - MOD  mapy ! ;
: -mapy    ( -- )   mapy @ 1- 192 24 - MOD  0 MAX mapy ! ;

: toggle-foreground  ( -- )  foreground? NOT TO foreground? ;
: toggle-background  ( -- )  background? NOT TO background? ;
: toggle-layer  ( -- )  edit-foreground? NOT TO edit-foreground? ;

: paint-tile  ( x y -- )    sprite# @  -ROT  edit-foreground? IF m2! ELSE m! THEN ;
: clear-tile  ( x y -- )            0  -ROT  edit-foreground? IF m2! ELSE m! THEN ;

: palette-area?   ( x y -- f )  139 139 6 8 * + WITHIN  SWAP   127 256 WITHIN  AND ;
: map-area?   ( x y -- f )        8 136 WITHIN  SWAP  0 256 WITHIN  AND ;
: clicked-sprite  ( x y -- u )  139 - 8 / 16 *  SWAP 127 - 8 / +  16 6 * sprite-page# @ * + ;

: update-mouse  ( -- )
  MOUSEX @  MOUSEY @ palette-area? IF
    MOUSEB @ CASE
      1 OF    MOUSEX @  MOUSEY @  clicked-sprite sprite# ! ENDOF
    ENDCASE
  THEN
  MOUSEX @  MOUSEY @ map-area? IF
    MOUSEB @ CASE
    1 OF  MOUSEX @ 8 / mapx @ +  MOUSEY @ 8 / 1- mapy @ +  paint-tile  ENDOF
    4 OF  MOUSEX @ 8 / mapx @ +  MOUSEY @ 8 / 1- mapy @ +  clear-tile  ENDOF
    ENDCASE
  THEN
  MOUSEB @ 64 = IF retro-40 then
;

: update-keys  ( -- )
  SCANCODE_W  just-pressed?  IF  SCANCODE_LSHIFT pressed? IF 32 0 DO -mapy LOOP ELSE -mapy  THEN THEN
  SCANCODE_S  just-pressed?  IF  SCANCODE_LSHIFT pressed? IF 24 0 DO +mapy LOOP ELSE +mapy  THEN THEN
  SCANCODE_A  just-pressed?  IF  SCANCODE_LSHIFT pressed? IF 32 0 DO -mapx LOOP ELSE -mapx  THEN THEN
  SCANCODE_D  just-pressed?  IF  SCANCODE_LSHIFT pressed? IF 24 0 DO +mapx LOOP ELSE +mapx  THEN THEN
  SCANCODE_M  just-pressed?  IF  +sprite-page  THEN
  SCANCODE_N  just-pressed?  IF  -sprite-page  THEN
  SCANCODE_F1 just-pressed?  IF  s" default.map" save-map THEN
  SCANCODE_F2 just-pressed?  IF  s" default.map" load-map THEN
  SCANCODE_B  just-pressed?  IF  toggle-background THEN
  SCANCODE_F  just-pressed?  IF  toggle-foreground THEN
  SCANCODE_L  just-pressed?  IF  toggle-layer THEN
  SCANCODE_Q  just-pressed?  IF  retro-40  THEN
;

: <update>  ( -- )  update-mouse update-keys ;

: palette-display
  6 0 DO
    16 0 DO
      127 I 8 * +  139 J 8 * +
      J 16 * I +  16 6 * sprite-page# @ * +   spr
      J 16 * I +  16 6 * sprite-page# @ * +   sprite# @ = IF  15  127 I 8 * +  139 J 8 * +  8 8 rectb THEN
    LOOP
  LOOP
;

: coords-display  ( -- )  1 0 at-xy s" X: " ?puts mapx @ . s" Y: " ?puts mapy @ . ;
: lines           ( -- )  15 0 7 255 7 line  15 0 135 255 135 line ;
: info-display    ( -- )
  1 24 at-xy s" DISP BG: " ?puts  background? IF s" YES " ELSE s" NO" THEN ?puts
  1 25 at-xy s" DISP FG: " ?puts  foreground? IF s" YES " ELSE s" NO" THEN ?puts
  1 26 at-xy s" EDITING: " ?puts  edit-foreground? IF s" FG " ELSE S" BG" THEN ?puts
;
: map-display     ( -- )
  background? IF mapx @ mapy @ 32 16 0 8 map* THEN
  foreground? IF 0 colorkey !  mapx @ mapy @ 32 16 0 8 map2*   -1 colorkey ! THEN
;

: <draw>
  0 cls
  coords-display
  map-display
  lines
  info-display
  palette-display
;

: <init>  0 cls  1 sprite# ! ;


\ install the software
PREVIOUS DEFINITIONS

ALSO map-editor-voc

INSTALL map-editor

PREVIOUS
