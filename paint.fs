\ paint.fs

17 ficl-vocabulary paint-voc
also paint-voc definitions

\ variables and initialisation/shutdown
0 VALUE image
0 VALUE hud?

VARIABLE color#
2VARIABLE last-coords

: <init>    ( -- )
  W H * ALLOCATE ABORT" Error allocating image buffer!" TO image
  image W H * ERASE
  FALSE TO hud?
;

: <shutdown>  ( -- )  image FREE ABORT" Error freeing image buffer!" ;
: ?exit       ( -- )  SCANCODE_Q pressed? IF  <shutdown>  retro-40  THEN ;


\ controls
: +color   ( -- )   color# @ 1+  16 MOD        color# ! ;
: -color   ( -- )   color# @ 1-  16 MOD 0 MAX  color# ! ;

: paint-pixel  ( x y -- )  color# @  -ROT  last-coords 2@  2SWAP line ;
: clear-pixel  ( x y -- )         0  -ROT  last-coords 2@  2SWAP line ;

0 VALUE stack
0 VALUE stack-start
0 VALUE stack-end

: alloc-stack   ( -- )  W H * 2* ALLOCATE ABORT" Error allocating floodfill stack" TO stack ;
: empty-stack   ( -- )  stack TO stack-start  stack TO stack-end ;
: free-stack    ( -- )  stack FREE ABORT" Error freeing floodfill stack" ;

: append   ( x y -- )  stack-end    C!  stack-end 1+ C!  stack-end 2+ TO stack-end ;
: fetch    ( -- x y )  stack-end 1- C@  stack-end 2- C@  stack-end 2- TO stack-end ;

\ this can probably be factored and better optimised :-)
: flood-fill  ( x y c c'-- )
  { x y c c' | above below }
  c c' = IF EXIT THEN
  x y P@ c <> IF EXIT THEN
  alloc-stack
  empty-stack
  x y append
  BEGIN
    stack-end stack-start >
  WHILE
    fetch  TO y  TO x
    BEGIN  x 0 >=  x y P@ c =  AND  WHILE  x 1- TO x  REPEAT
    x 1+ TO x
    0 TO above  0 TO below
    BEGIN
      x W <  x y P@ c =  AND
    WHILE
      c' x y P!
      above 0=  y 0>  x y 1- p@ c =  AND AND IF
        x y 1- append
        1 TO above
      ELSE
        above 0<>  y 0>  x y 1- p@ c =  AND AND IF THEN
        0 TO above
      THEN

      below 0=  y H 1- <  x y 1+ p@ c =  AND AND IF
        x y 1+ append
        1 TO below
      ELSE
        below 0<>  y 0>  x y 1+ p@ c =  AND AND IF THEN
        0 TO below
      THEN
      x 1+ TO x
    REPEAT
  REPEAT
  free-stack
;

: toggle-hud  ( -- )  VRAM image hud?  DUP NOT TO hud?  IF SWAP THEN  W H * MOVE ;

: update-mouse  ( -- )
  MOUSEX @  MOUSEY @
  MOUSEB @ CASE
    1 OF  2DUP  paint-pixel  ENDOF
    4 OF  2DUP  clear-pixel  ENDOF
  ENDCASE
  last-coords 2!
;

\ hud
: palette-area?  ( x y -- f )  72 104 WITHIN  SWAP   0 32 WITHIN  AND ;
: clicked-color  ( x y -- u )  72 - 8 / 4 *  SWAP 8 / +  ;

: update-mouse-hud  ( -- )
  MOUSEX @  MOUSEY @
  MOUSEB @ CASE
    1 OF  2DUP palette-area? IF  2DUP clicked-color color# !  THEN  ENDOF
  ENDCASE
  last-coords 2!
;


: update-keys  ( -- )
  SCANCODE_F just-pressed?  IF  MOUSEX @ MOUSEY @ 2DUP P@ color# @ flood-fill  THEN
  SCANCODE_H just-pressed?  IF  toggle-hud  THEN
  SCANCODE_C just-pressed?  IF  -color      THEN
  SCANCODE_V just-pressed?  IF  +color      THEN
  SCANCODE_Q just-pressed?  IF   ?exit      THEN
;

: <update>  ( -- )
  hud? IF  update-mouse-hud  ELSE  update-mouse  THEN
  update-keys
;

: palette-display
  4 0 DO
    4 0 DO
      I 4 * J +  DUP
      0 J 8 * +  72 I 8 * +  8 8 rect
      color# @ = IF  15  0 J 8 * +  72 I 8 * +  8 8 rectb  THEN
    LOOP
  LOOP
;

: <draw>    ( -- )
  hud? IF
    s" HELLO HUD!" 15 50 50 PUTS
    palette-display
  THEN
;

\ install the software
PREVIOUS DEFINITIONS

ALSO paint-voc

INSTALL paint

PREVIOUS
