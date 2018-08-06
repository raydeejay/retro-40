\ map-editor.fs

17 ficl-vocabulary map-editor-voc
also map-editor-voc definitions

\ variables and initialisation/shutdown
0 VALUE image
0 VALUE hud?

VARIABLE sprite#

: #map W H * ;

\ I/O
: save-map  ( addr u -- )
  R/W BIN CREATE-FILE ABORT" Error opening map file"
  DUP  MRAM  #map  ROT  WRITE-FILE ABORT" Error writing map"
  CLOSE-FILE ABORT" Error closing map file" ;

: load-map  ( addr u -- )
  R/O BIN OPEN-FILE ABORT" Error opening map file"
  DUP  MRAM  #map  ROT  READ-FILE ABORT" Error reading map" DROP
  CLOSE-FILE ABORT" Error closing map file" ;

\ editor
: +sprite           ( -- )    sprite# @ 1+ 256 MOD  sprite# ! ;
: -sprite           ( -- )    sprite# @ 1- 256 MOD  sprite# ! ;

: toggle-hud  ( -- )  hud? NOT TO hud? ;

: paint-tile  ( x y -- )    sprite# @  -ROT  m! ;
: clear-tile  ( x y -- )            0  -ROT  m! ;

: update-mouse  ( -- )
  MOUSEB @ CASE
    1 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  paint-tile  ENDOF
    4 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  clear-tile  ENDOF
  ENDCASE
;

: palette-area?  ( x y -- f )  72 104 WITHIN  SWAP   0 32 WITHIN  AND ;
: clicked-sprite  ( x y -- u )  72 - 8 / 4 *  SWAP 8 / +  ;

: update-mouse-hud  ( -- )
  MOUSEB @ CASE
    1 OF  MOUSEX @  MOUSEY @ palette-area? IF  MOUSEX @  MOUSEY @  clicked-sprite sprite# !  THEN  ENDOF
  ENDCASE
;

: update-keys  ( -- )
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
  4 0 DO
    4 0 DO
      0 I 8 * +  72 J 8 * +
      J 4 * I + spr
      J 4 * I +  sprite# @ = IF  15  0 I 8 * +  72 J 8 * +  8 8 rectb THEN
    LOOP
  LOOP
;

: map-display      map ;

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
