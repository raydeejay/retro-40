\ drmario-like game

17 ficl-vocabulary drmario-voc
also drmario-voc definitions

0 DEFINE-SFX connect.wav
1 DEFINE-SFX zap.wav

CREATE capsules
\ single color
HEX
1 C, 2 C, 0 C, 0 C,  0 C, 3 C, 0 C, 4 C,  0 C, 0 C, 1 C, 2 C,  3 C, 0 C, 4 C, 0 C,
6 C, 7 C, 0 C, 0 C,  0 C, 8 C, 0 C, 9 C,  0 C, 0 C, 6 C, 7 C,  8 C, 0 C, 9 C, 0 C,
B C, C C, 0 C, 0 C,  0 C, D C, 0 C, E C,  0 C, 0 C, B C, C C,  D C, 0 C, E C, 0 C,
\ mixed color, with duplicated pairs as the original
1 C, 7 C, 0 C, 0 C,  0 C, 3 C, 0 C, 9 C,  0 C, 0 C, 6 C, 2 C,  8 C, 0 C, 4 C, 0 C,
1 C, C C, 0 C, 0 C,  0 C, 3 C, 0 C, E C,  0 C, 0 C, B C, 2 C,  D C, 0 C, 4 C, 0 C,
6 C, C C, 0 C, 0 C,  0 C, 8 C, 0 C, E C,  0 C, 0 C, B C, 7 C,  D C, 0 C, 9 C, 0 C,
6 C, 2 C, 0 C, 0 C,  0 C, 8 C, 0 C, 4 C,  0 C, 0 C, 1 C, 7 C,  3 C, 0 C, 9 C, 0 C,
B C, 2 C, 0 C, 0 C,  0 C, D C, 0 C, 4 C,  0 C, 0 C, 1 C, C C,  3 C, 0 C, E C, 0 C,
B C, 7 C, 0 C, 0 C,  0 C, D C, 0 C, 9 C,  0 C, 0 C, 6 C, C C,  8 C, 0 C, E C, 0 C,
DECIMAL

: capsule@  ( x y rot# piece# -- )  >R >R 2 * + R> 4 * + R> 16 * + capsules + C@ ;

\ TODO: use this
: rotate-capsule  { a b c d -- c a d b }  c a d b ;

: fits?   ( rot# piece# x y -- f )
  { r p x y }
  2 0 ?DO
    2 0 ?DO
      11 x + I +  y J +  m@ 0<>  I J r p capsule@ 0<>  AND IF  FALSE EXIT  THEN
    LOOP
  LOOP
  TRUE
;

: single?      ( spr# -- f )  16 - DUP  1 16 WITHIN  SWAP  5 MOD  0=          AND ;
: vertical?    ( spr# -- f )  16 - DUP  1 16 WITHIN  SWAP  5 MOD  2 >         AND ;
: horizontal?  ( spr# -- f )  16 - DUP  1 16 WITHIN  SWAP  5 MOD  1 3 WITHIN  AND ;

: >facing  ( spr# -- dir )  DUP IF 16 - 1- 5 MOD 1+ ELSE 5 THEN ;
: >color   ( spr# -- c )    DUP IF 16 - 1- 5 / 1+ THEN ;

VARIABLE capsule-x
VARIABLE capsule-y
VARIABLE capsule-rot
VARIABLE capsule-type
VARIABLE next-capsule

: move-left   ( -- )
  capsule-x @ 1- 0 MAX
  DUP capsule-y @ capsule-rot @ capsule-type @ 2SWAP fits? IF capsule-x ! ELSE DROP THEN
;

: move-right  ( -- )
  capsule-x @ 1+ 9 MIN
  DUP capsule-y @ capsule-rot @ capsule-type @ 2SWAP fits? IF capsule-x ! ELSE DROP THEN
;

: rotate-left   ( -- )
  capsule-rot @ 4 + 1- 4 MOD DUP
  capsule-type @ capsule-x @ capsule-y @ fits? IF capsule-rot ! ELSE DROP THEN
;

: rotate-right  ( -- )
  capsule-rot @ 4 + 1+ 4 MOD DUP
  capsule-type @ capsule-x @ capsule-y @ fits? IF capsule-rot ! ELSE DROP THEN
;

: advance  ( -- )  1 capsule-y +! ;

: new-capsule  ( -- )
  next-capsule @ capsule-type !
  9 RND next-capsule !
  -4 capsule-y !
  5 capsule-x !
  0 capsule-rot !
;

: split  ( x' y -- )
  { x y }
  x y M@ >facing CASE
    1 OF x 1+ y    M@ 3 +   x 1+ y     M!  ENDOF
    2 OF x 1- y    M@ 4 +   x 1- y     M!  ENDOF
    3 OF x    y 1+ M@ 1 +   x    y 1+  M!  ENDOF
    4 OF x    y 1- M@ 2 +   x    y 1-  M!  ENDOF
  ENDCASE
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


: pull-single  ( x y -- f )
  { x y | pulled? }
  11 x + y M@
  BEGIN
    11 x + y 1+ M@ 0=
  WHILE
    TRUE TO pulled?
    DUP  11 x +  y 1+  M!
      0  11 x +  y     M!
    y 1+ TO y
  REPEAT DROP
  pulled?
;

: pull-right  ( -- )
  { x y | pulled? }
  11 x +    y M@
  11 x + 1+ y M@
  BEGIN
    11 x +    y 1+ M@ 0=
    11 x + 1+ y 1+ M@ 0=
    AND
  WHILE
    TRUE TO pulled?
    2DUP  11 x + 1+  y 1+  M!
          11 x +     y 1+  M!
       0  11 x +     y     M!
       0  11 x + 1+  y     M!
  REPEAT 2DROP
  pulled?
;

: pull-left  ( -- )
  { x y | pulled? }
  11 x +    y M@
  11 x + 1- y M@
  BEGIN
    11 x +    y 1+ M@ 0=
    11 x + 1- y 1+ M@ 0=
    AND
  WHILE
    TRUE TO pulled?
    2DUP  11 x + 1-  y 1+  M!
          11 x +     y 1+  M!
       0  11 x +     y     M!
       0  11 x + 1-  y     M!
  REPEAT 2DROP
  pulled?
;

: pull-capsule  ( x y -- f )
  { x y }
  11 x + y M@ >facing CASE
    1 OF  x y pull-right  ENDOF
    2 OF  x y pull-left   ENDOF
  ENDCASE
;

: pull-column  ( x -- f )
  { x | color pulled? }
  0 19 ?DO
    x 11 + I M@ TO color
    color single?     IF  x I pull-single  pulled? OR TO pulled?  THEN
    color vertical?   IF  x I pull-single  pulled? OR TO pulled?  THEN
    color horizontal? IF  x I pull-capsule pulled? OR TO pulled?  THEN
  -1 +LOOP
  pulled?
;

: gravity  ( -- f )  FALSE 10 0 DO I pull-column OR LOOP ;


: remove-cells-h  ( x y u -- )
  0 ?DO
    2DUP  I 11 + UNDER+
    2DUP  split
    0 -ROT M!
  LOOP 2DROP
  1 sfx
;

: clear-row  ( u -- )
  { y | count last-color color }
  11 0 DO
    11 I + y M@ >color TO color
    count 1+ TO count
    color last-color <> IF
      count 4 > last-color 0<> AND IF  I count -  y  count  remove-cells-h  THEN
      color TO last-color
      0 TO count
    THEN
  LOOP
;

: clear-rows  ( -- )  20 0 DO  I clear-row  LOOP ;

: remove-cells-v  ( x y u -- )
  0 ?DO
    2DUP  OVER 11 +  OVER I +
    2DUP  split
    0 -ROT M!
  LOOP 2DROP
  1 sfx
;

: clear-column  ( u -- )
  { x | count last-color color }
  21 0 DO
    11 x +  I  M@ >color TO color
    count 1+ TO count
    color last-color <> IF
      count 4 >  last-color 0<> AND IF  x  I count -  count  remove-cells-v  THEN
      color TO last-color
      0 TO count
    THEN
  LOOP
;

: clear-columns  ( -- )  10 0 DO  I clear-column  LOOP ;


: persist-capsule  ( -- )
  2 0 ?DO
    2 0 ?DO
      I J capsule-rot @ capsule-type @ capsule@ IF
        I J capsule-rot @ capsule-type @ capsule@ 16 +  11 capsule-x @ + I +  capsule-y @ J +  m!
      THEN
    LOOP
  LOOP
  0 sfx
;

: ?advance  ( -- )
  tick @ 0= IF
    capsule-rot @ capsule-type @ capsule-x @ capsule-y @ 1+ fits? IF
      advance
    ELSE
      persist-capsule
      clear-rows
      clear-columns
      BEGIN  gravity  WHILE  clear-rows clear-columns REPEAT
      new-capsule
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


: draw-next-capsule  ( -- )
  2 0 DO
    2 0 DO
      I J 0 next-capsule @ capsule@ IF
        6 I + 8 *  10 J + 8 *  I J 0 capsule-type @ capsule@ 16 + spr
      THEN
    LOOP
  LOOP
;

: draw-capsule  ( -- )
  2 0 DO
    2 0 DO
      I J capsule-rot @ capsule-type @ capsule@ IF
        11 capsule-x @ + I + 8 *  capsule-y @ J + 8 *  I J capsule-rot @ capsule-type @ capsule@ 16 + spr
      THEN
    LOOP
  LOOP
;

: <draw>  ( -- )
  0 cls
  0 0 map
  draw-next-capsule
  draw-capsule
;

: <init>  ( -- )
  0 tick !
  new-capsule
  next-capsule
  s" drmario.spr" load-sprites
  s" drmario.map" load-map
;



\ install the software
PREVIOUS DEFINITIONS

ALSO drmario-voc

INSTALL drmario

PREVIOUS
