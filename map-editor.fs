VARIABLE cursorx
VARIABLE cursory
VARIABLE sprite#

: #map 32 24 * ;

\ I/O
: save-map  ( addr u -- )
  R/W BIN CREATE-FILE ABORT" Error opening map file"
  DUP  MRAM  #map /sprite *  ROT  WRITE-FILE ABORT" Error writing map"
  CLOSE-FILE ABORT" Error closing map file" ;

: load-map  ( addr u -- )
  R/O BIN OPEN-FILE ABORT" Error opening map file"
  DUP  MRAM  #map /sprite *  ROT  READ-FILE ABORT" Error reading map" DROP
  CLOSE-FILE ABORT" Error closing map file" ;

\ editor
: paint-tile  ( x y -- )    sprite# @  -ROT  m! ;
: clear-tile  ( x y -- )            0  -ROT  m! ;
: draw-area?   ( x y -- f )   0 64 WITHIN  SWAP   0 64 WITHIN  AND ;
: sprite-area?  ( x y -- f )  48 56 WITHIN  SWAP  80 88 WITHIN  AND ;
: +sprite           ( -- )    sprite# @ 1+ 256 MOD  sprite# ! ;
: -sprite           ( -- )    sprite# @ 1- 256 MOD  sprite# ! ;
                    
: update-mouse  ( -- )
    MOUSEB @ CASE
      1 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  paint-tile  ENDOF
      4 OF  MOUSEX @ 8 /  MOUSEY @ 8 /  clear-tile  ENDOF
    ENDCASE
;

: update-keys  ( -- )
  SCANCODE_W     just-pressed?  IF  cursory @ 1- 8 MOD  DUP 0< IF DROP 7 THEN  cursory ! THEN
  SCANCODE_S     just-pressed?  IF  cursory @ 1+ 8 MOD  DUP 0< IF DROP 7 THEN  cursory ! THEN
  SCANCODE_A     just-pressed?  IF  cursorx @ 1- 8 MOD  DUP 0< IF DROP 7 THEN  cursorx ! THEN
  SCANCODE_D     just-pressed?  IF  cursorx @ 1+ 8 MOD  DUP 0< IF DROP 7 THEN  cursorx ! THEN
  SCANCODE_C     just-pressed?  IF  +sprite  THEN
  SCANCODE_V     just-pressed?  IF  -sprite  THEN
  SCANCODE_N     just-pressed?  IF  sprite# @ 1- #map MOD  sprite#  ! THEN
  SCANCODE_M     just-pressed?  IF  sprite# @ 1+ #map MOD  sprite#  ! THEN
  SCANCODE_F1    just-pressed?  IF  s" default.map" save-map THEN
  SCANCODE_F2    just-pressed?  IF  s" default.map" load-map THEN
  SCANCODE_F4    just-pressed?  IF  map 64 dump CR THEN
  SCANCODE_SPACE just-pressed?  IF  cursorx @ cursory @ paint-tile THEN
  SCANCODE_Q     just-pressed?  IF  retro-40  THEN
;

: <update>  ( -- )  update-mouse update-keys ;

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
      J 4 * I +
      0 J 8 * +  72 I 8 * +
      8 8 rect
    LOOP
  LOOP
;

: zoomed-display   8 sprite# @  0  0 sspr ;
: map-display      map ;
: cursor-display   14  8 cursorx @ *  8 cursory @ *  8 8 rect ;
: sprite-display    sprite# @ 80 48 8 8 rect ;

: <draw>
\  palette-display
\  zoomed-display
  map-display
\  sprite-display
\  cursor-display
;

: <init>  0 cls  1 sprite# ! ;

INSTALL map-editor
