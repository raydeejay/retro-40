\ map-editor.fs

17 ficl-vocabulary map-editor-voc
also map-editor-voc definitions

\ variables and initialisation/shutdown
0 VALUE image
0 VALUE hud?

VARIABLE cursorx
VARIABLE cursory
VARIABLE sprite#
2VARIABLE last-coords

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
: paint-tile  ( x y -- )    sprite# @  -ROT  m! ;
: clear-tile  ( x y -- )            0  -ROT  m! ;
: draw-area?   ( x y -- f )   0 64 WITHIN  SWAP   0 64 WITHIN  AND ;
: sprite-area?  ( x y -- f )  48 56 WITHIN  SWAP  80 88 WITHIN  AND ;
: +sprite           ( -- )    sprite# @ 1+ 256 MOD  sprite# ! ;
: -sprite           ( -- )    sprite# @ 1- 256 MOD  sprite# ! ;

: toggle-hud  ( -- )  hud? NOT TO hud? ;

: update-mouse  ( -- )
  MOUSEB @ CASE
    1 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  paint-tile  ENDOF
    4 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  clear-tile  ENDOF
  ENDCASE
;

: palette-area?  ( x y -- f )  72 104 WITHIN  SWAP   0 32 WITHIN  AND ;
: clicked-sprite  ( x y -- u )  72 - 8 / 4 *  SWAP 8 / +  ;

: update-mouse-hud  ( -- )
  MOUSEX @  MOUSEY @
  MOUSEB @ CASE
    1 OF  2DUP palette-area? IF  2DUP clicked-sprite sprite# !  THEN  ENDOF
  ENDCASE
  last-coords 2!
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

: mapheet
  ( bg @ ) 3 96 0 32 8 * 16 8 * rect
  16 0 DO
    32 0 DO
      96 I 8 * +  J 8 * I +    J 8 * I +  spr
    LOOP
  LOOP ;

: sspr  ( scale u x y -- )
  { scale u x y }
  8 0 DO
    8 0 DO
      I J u SP@  I scale * x +  J scale * y +  scale  scale  rect
    LOOP
  LOOP
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

: zoomed-display   8 sprite# @  0  0 sspr ;
: map-display      map ;
: cursor-display   14  8 cursorx @ *  8 cursory @ *  8 8 rect ;
: sprite-display    sprite# @ 80 48 8 8 rect ;

: <draw>
  0 cls
\  zoomed-display
  map-display
\  sprite-display
  \  cursor-display
  hud? IF
    0 0 72 32 32 rect
    palette-display
  THEN
;

: <init>  0 cls  1 sprite# ! ;


\ install the software
PREVIOUS DEFINITIONS

ALSO map-editor-voc

INSTALL map-editor

PREVIOUS
