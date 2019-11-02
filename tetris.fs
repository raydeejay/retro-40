\ tetris-like game

17 ficl-vocabulary tetris-voc
also tetris-voc definitions

0 DEFINE-SFX connect.wav
1 DEFINE-SFX zap.wav

CREATE shapes
1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 1 C, 0 C,
1 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 0 C, 0 C,
0 C, 1 C, 0 C, 0 C,
0 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

0 C, 0 C, 1 C, 0 C,
1 C, 1 C, 1 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,


1 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,


1 C, 1 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 1 C, 0 C,
0 C, 0 C, 1 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

0 C, 1 C, 0 C, 0 C,
0 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 0 C, 0 C, 0 C,
1 C, 1 C, 1 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,


0 C, 1 C, 1 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 0 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

0 C, 1 C, 1 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 0 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,


1 C, 1 C, 0 C, 0 C,
0 C, 1 C, 1 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

0 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 0 C, 0 C,
0 C, 1 C, 1 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

0 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,


0 C, 1 C, 0 C, 0 C,
1 C, 1 C, 1 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 0 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 1 C, 1 C, 0 C,
0 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

0 C, 1 C, 0 C, 0 C,
1 C, 1 C, 0 C, 0 C,
0 C, 1 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,


1 C, 1 C, 1 C, 1 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,

1 C, 1 C, 1 C, 1 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,
0 C, 0 C, 0 C, 0 C,

1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,
1 C, 0 C, 0 C, 0 C,


: shape@  ( x y rot# piece# -- )  >R >R 4 * + R> 16 * + R> 64 * + shapes + C@ ;

: fits?   ( rot# piece# x y -- f )
  { r p x y }
  4 0 ?DO
    4 0 ?DO
      11 x + I +  y J +  m@ 0<>  I J r p shape@ 0<>  AND IF  FALSE EXIT  THEN
    LOOP
  LOOP
  TRUE
;


VARIABLE piece-x
VARIABLE piece-y
VARIABLE piece-rot
VARIABLE piece-type
VARIABLE next-piece

: move-left   ( -- )
  piece-x @ 1- 0 MAX
  DUP piece-y @ piece-rot @ piece-type @ 2SWAP fits? IF piece-x ! ELSE DROP THEN
;

: move-right  ( -- )
  piece-x @ 1+ 9 MIN
  DUP piece-y @ piece-rot @ piece-type @ 2SWAP fits? IF piece-x ! ELSE DROP THEN
;

: rotate-left   ( -- )
  piece-rot @ 4 + 1- 4 MOD DUP
  piece-type @ piece-x @ piece-y @ fits? IF piece-rot ! ELSE DROP THEN
;

: rotate-right  ( -- )
  piece-rot @ 4 + 1+ 4 MOD DUP
  piece-type @ piece-x @ piece-y @ fits? IF piece-rot ! ELSE DROP THEN
;

: advance  ( -- )  1 piece-y +! ;

: new-piece  ( -- )
  next-piece @ piece-type !
  7 RND next-piece !
  -4 piece-y !
  5 piece-x !
  0 piece-rot !
;

: clear-field  ( -- )  20 0 DO MRAM W I * + 11 + 10 ERASE LOOP ;


VARIABLE tick

: +tick  ( -- )  tick @ 1+ 20 MOD tick ! ;

: ?move  ( -- )
  SCANCODE_W just-pressed? IF  rotate-left   THEN
  SCANCODE_E just-pressed? IF  rotate-right  THEN
  SCANCODE_A just-pressed? IF  move-left   THEN
  SCANCODE_D just-pressed? IF  move-right  THEN
  SCANCODE_S pressed? IF  0 tick !  THEN
;

: full-line?  ( y -- f )
  { y }
  TRUE 10 0 DO I 11 + y m@ 0<> AND LOOP 
;

: clear-lines  ( -- )
  20 0 DO
    I full-line? IF
      0 I 1- DO
        MRAM W I * + 11 +  MRAM W I 1+ * + 11 +  10 MOVE
      -1 +LOOP
    THEN
  LOOP 
  1 sfx
;

: persist-piece  ( -- )
  4 0 ?DO
    4 0 ?DO
      I J piece-rot @ piece-type @ shape@ IF
        piece-type @ 16 +  11 piece-x @ + I +  piece-y @ J +  m!
      THEN
    LOOP
  LOOP
  0 sfx
;

: ?advance  ( -- )
  tick @ 0= IF
    piece-rot @ piece-type @ piece-x @ piece-y @ 1+ fits? IF
      advance
    ELSE
      persist-piece
      clear-lines
      new-piece
    THEN
  THEN
;

: ?exit  ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;

: <update>  ( -- )
  +tick
  ?move
  ?advance
  ?exit
;


: draw-next-piece  ( -- )
  4 0 DO
    4 0 DO
      I J 0 next-piece @ shape@ IF
        6 I + 8 *  10 J + 8 *  16 next-piece @ + spr
      THEN
    LOOP
  LOOP
;

: draw-piece  ( -- )
  4 0 DO
    4 0 DO
      I J piece-rot @ piece-type @ shape@ IF
        11 piece-x @ + I + 8 *  piece-y @ J + 8 *  16 piece-type @ + spr
      THEN
    LOOP
  LOOP
;

: <draw>  ( -- )
  0 cls
  0 0 map
  draw-next-piece
  draw-piece
;

: <init>  ( -- )
  0 tick !
  new-piece
  next-piece
  s" tetris.spr" load-sprites
  s" tetris.map" load-map
;



\ install the software
PREVIOUS DEFINITIONS

ALSO tetris-voc

INSTALL tetris

PREVIOUS
