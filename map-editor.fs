\ map-editor.fs

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
0 VALUE hud?

VARIABLE sprite#

VARIABLE mapx
VARIABLE mapy


\ editor
: +sprite  ( -- )   sprite# @ 1+ 256 MOD  sprite# ! ;
: -sprite  ( -- )   sprite# @ 1- 256 MOD  sprite# ! ;
: +mapx    ( -- )   mapx @ 1+ 256 32 - MOD  mapx ! ;
: -mapx    ( -- )   mapx @ 1- 256 32 - MOD  0 MAX mapx ! ;
: +mapy    ( -- )   mapy @ 1+ 192 24 - MOD  mapy ! ;
: -mapy    ( -- )   mapy @ 1- 192 24 - MOD  0 MAX mapy ! ;

: toggle-hud  ( -- )  hud? NOT TO hud? ;

: paint-tile  ( x y -- )    sprite# @  -ROT  m! ;
: clear-tile  ( x y -- )            0  -ROT  m! ;

: update-mouse  ( -- )
  MOUSEB @ CASE
    1 OF  MOUSEX @ 8 / mapx @ +  MOUSEY @ 8 / mapy @ +  paint-tile  ENDOF
    4 OF  MOUSEX @ 8 / mapx @ +  MOUSEY @ 8 / mapy @ +  clear-tile  ENDOF
  ENDCASE
;

: palette-area?  ( x y -- f )  72 72 12 8 * + WITHIN  SWAP   0 16 8 * WITHIN  AND ;
: clicked-sprite  ( x y -- u )  72 - 8 / 16 *  SWAP 8 / +  ;

: update-mouse-hud  ( -- )
  MOUSEB @ CASE
    1 OF  MOUSEX @  MOUSEY @ palette-area? IF  MOUSEX @  MOUSEY @  clicked-sprite sprite# !  THEN  ENDOF
  ENDCASE
;

: update-keys  ( -- )
  SCANCODE_W  just-pressed?  IF  -mapy  THEN
  SCANCODE_S  just-pressed?  IF  +mapy  THEN
  SCANCODE_A  just-pressed?  IF  -mapx  THEN
  SCANCODE_D  just-pressed?  IF  +mapx  THEN
  SCANCODE_H  just-pressed?  IF  toggle-hud  THEN
  SCANCODE_M  just-pressed?  IF  +sprite  THEN
  SCANCODE_N  just-pressed?  IF  -sprite  THEN
  SCANCODE_F1 just-pressed?  IF  s" default.map" save-map THEN
  SCANCODE_F2 just-pressed?  IF  s" default.map" load-map THEN
  SCANCODE_Q  just-pressed?  IF  retro-40  THEN
;

: <update>  ( -- )
  hud? IF  update-mouse-hud  ELSE  update-mouse  THEN
  update-keys
;

: palette-display
  16 0 DO
    16 0 DO
      0 I 8 * +  72 J 8 * +
      J 16 * I +  spr
      J 16 * I +  sprite# @ = IF  15  0 I 8 * +  72 J 8 * +  8 8 rectb THEN
    LOOP
  LOOP
;

: map-display      mapx @ mapy @ map ;

: <draw>
  0 cls
  map-display
  hud? IF
    0 0 64 40 48 rect
    palette-display
  THEN
;

: <init>  0 cls  1 sprite# ! ;


\ install the software
PREVIOUS DEFINITIONS

ALSO map-editor-voc

INSTALL map-editor

PREVIOUS
