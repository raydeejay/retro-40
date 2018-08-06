s" Retro-40 ROM Booting" TYPE CR

\ ------------------------------------------------------------
\ COMPATIBILITY AND CONVENIENCE
\ ------------------------------------------------------------

: <=   ( a b -- f )  2DUP < -ROT = OR ;
: >=   ( a b -- f )  2DUP > -ROT = OR ;

: F>=  ( F: a b -- ) F2DUP F= F> OR ;
: F<=  ( F: a b -- ) F2DUP F= F< OR ;

: NOT    ( u -- f )  0= IF -1 ELSE 0 THEN ;
: 3DUP  ( a b c -- a b c a b c )  3 PICK  3 PICK  3 PICK ;

\ Chuck Moore
: UNDER+  ( a b c -- a+c b )  ROT + SWAP ;

\ more stack-related operators
: PLUCK  ( an+1 an an-1 .. a1 n -- an1+ an-1 .. a1 )  ROLL DROP ;
: PIVOT  ( a b c -- c b a )  SWAP ROT ;

: RND      ( u -- u' )  RANDOM SWAP MOD ;

: CELLS+  ( addr u -- addr' )  CELLS + ;
: CELLS-  ( addr u -- addr' )  CELLS - ;



\ ------------------------------------------------------------
\ LINES
\ ------------------------------------------------------------

VARIABLE ww
VARIABLE hh
VARIABLE dx1
VARIABLE dy1
VARIABLE dx2
VARIABLE dy2
VARIABLE longest
VARIABLE shortest
VARIABLE numerator

: init
  0 ww !
  0 hh !
  0 dx1 !
  0 dy1 !
  0 dx2 !
  0 dy2 !
  0 longest !
  0 shortest !
  0 numerator !
;

: sethw   ( x y x2 y2 -- )  ROT - hh !  SWAP - ww ! ;
: setdx1  ( -- )  ww @ 0< IF  -1 dx1 !  ELSE  ww @ 0> IF  1 dx1 !  THEN THEN ;
: setdx2  ( -- )  ww @ 0< IF  -1 dx2 !  ELSE  ww @ 0> IF  1 dx2 !  THEN THEN ;
: setdy1  ( -- )  hh @ 0< IF  -1 dy1 !  ELSE  hh @ 0> IF  1 dy1 !  THEN THEN ;
: setdy2  ( -- )  hh @ 0< IF  -1 dy2 !  ELSE  hh @ 0> IF  1 dy2 !  THEN THEN ;
: setnumerator  ( -- )  longest @ 1 RSHIFT numerator ! ;

: setsl   ( -- )
  ww @ ABS longest  !
  hh @ ABS shortest !
  longest @ shortest @ <= IF
    hh @ ABS longest  !
    ww @ ABS shortest !
    setdy2
    0 dx2 !
  THEN ;

: line  ( color x y x2 y2 -- )
  { color x y x2 y2 }
  init
  x y x2 y2 sethw
  setdx1
  setdx2
  setdy1
  setsl
  setnumerator
  longest @ 1+ 0 ?DO
    color x y p!
    shortest @ numerator +!
    numerator @ longest @ >= IF
      longest @ NEGATE numerator +!
      dx1 @ x + TO x
      dy1 @ y + TO y
    ELSE
      dx2 @ x + TO x
      dy2 @ y + TO y
    THEN
  LOOP ;

\ ------------------------------------------------------------
\ RECTANGLES
\ ------------------------------------------------------------
: rect { color x y w h | xw -- }
  x w + TO xw
  h 0 ?DO  color  x  y i +  xw 1-  OVER  line  LOOP
;

: rectb  ( color x y w h -- )
  { color x y w h | yh xw -- }
  x w + 1- TO xw
  y h + 1- TO yh
     xw x ?DO  color I y  p!                 LOOP
     xw x ?DO  color I yh p!                 LOOP
  yh 1+ y ?DO  color x I  p!  color xw I p!  LOOP
;

\ ------------------------------------------------------------
\ CIRCLES
\ ------------------------------------------------------------
: circ  ( color x0 y0 radius -- )
  { color x0 y0 radius | x y -- }
  radius  radius NEGATE  ?DO
    radius  radius NEGATE  ?DO
      I I * J J * +  radius radius * radius +  < IF
        color x0 I + y0 J + p! 
      THEN
    LOOP    
  LOOP ;

: circb  ( color x0 y0 radius -- )
  { color x0 y0 radius -- }
  radius  radius NEGATE  ?DO
    radius  radius NEGATE  ?DO
      I I * J J * +  radius radius * radius -  >
      I I * J J * +  radius radius * radius +  <
      AND IF
        color x0 I + y0 J + p! 
      THEN
    LOOP    
  LOOP ;


\ ------------------------------------------------------------
\ RECTS DEMO
\ ------------------------------------------------------------
0 VALUE x
0 VALUE y
0 VALUE dx 
0 VALUE dy
0 VALUE col 

: <init>
  W 2/ TO x
  H 2/ TO y
  6 TO dx
  4 TO dy
  4 TO col
;

: bounce-rect
  dx x + TO x
  dy y + TO y
  x 0<  x W 6 - >  OR IF
    dx NEGATE TO dx
    col 1+ 15 mod 1 MAX  TO col
  THEN
  y 0<  y H 6 - >  OR IF
    dy NEGATE TO dy
    col 1+ 16 mod 1 MAX  TO col
  THEN
  col x y 6 6 rect
;

: rp!     ( -- )  RANDOM 15 MOD 1+  RANDOM W MOD  RANDOM H MOD  P! ;
: rl!     ( -- )  RANDOM 15 MOD 1+  RANDOM W MOD  RANDOM H MOD  RANDOM W MOD     RANDOM H MOD     line ;
: rr!     ( -- )  RANDOM 15 MOD 1+  RANDOM W MOD  RANDOM H MOD  RANDOM W MOD 2/  RANDOM H MOD 2/  rect ;
: rc!     ( -- )  RANDOM 15 MOD 1+  RANDOM W MOD  RANDOM H MOD  RANDOM H MOD 2/  circ ;

\ : update  ( -- )  10 0 do  rp! rl! rr! rc! bounce-rect loop  ;

\ define a sprite
1 3 3 1 sp!
2 4 3 1 sp!
3 3 4 1 sp!
4 4 4 1 sp!

\ regular keyboard input, with repeat and all
: input   ( -- )
  SCANCODE_A pressed? IF  x 1- TO x  THEN
  SCANCODE_D pressed? IF  x 1+ TO x  THEN
  SCANCODE_W pressed? IF  y 1- TO y  THEN
  SCANCODE_S pressed? IF  y 1+ TO y  THEN
;

\ : update    ( -- )  input  x y 1 spr ;

\ run cycle machinery
\ define DEFER and IS (and WHAT'S perhaps?)
\ for the moment values will do...
: noop ;

' noop VALUE update-fn
' noop VALUE draw-fn
' noop VALUE init-fn

: init    init-fn execute ;
: update  update-fn execute  draw-fn execute ;

: install  ( "name" -- )
  CREATE \ \ check for missing functions?
  c" <update>" FIND NOT ABORT" <UPDATE> not found" ,
  c" <draw>"   FIND NOT ABORT" <DRAW> not found"   ,
  c" <init>"   FIND NOT ABORT" <INIT> not found"   ,
  DOES>
  DUP @ TO update-fn CELL+
  DUP @ TO draw-fn   CELL+
      @ TO init-fn
  init
;

\ load some code
include font.fs
include console.fs
include sprite-editor.fs
include snake.fs

\ run the console program
S" Retro-40 Initialised" ?puts CR CR
retro-40
