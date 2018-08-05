\ screen
8 CONSTANT scale
W scale / CONSTANT width
H scale / CONSTANT height

: get-color ( x y -- c )  SWAP scale * SWAP scale * p@ ;

: draw  ( color x y -- )
  SWAP scale * SWAP scale *
  scale 0 ?DO scale 0 ?DO  3DUP I UNDER+ J + p!  LOOP LOOP
  2DROP DROP ;

: draw-white  ( x y -- )  15 -ROT draw ;
: draw-black  ( x y -- )   0 -ROT draw ;
: draw-walls-h    ( -- )
  width  0 DO  I 0 draw-white  I  height 1 -  draw-white  LOOP ;
: draw-walls-v    ( -- )
  height 0 DO  0 I draw-white  width 1 -  I  draw-white  LOOP ;
: draw-walls      ( -- )  draw-walls-h  draw-walls-v ;


\ snake
3 CONSTANT /snake
VARIABLE snake-body 500 /snake * CELLS ALLOT

: snake-x-head  ( -- )  snake-body ;
: snake-y-head  ( -- )  snake-body CELL+ ;
: snake-c-head  ( -- )  snake-body 2 CELLS + ;

: snake-x  ( offset -- address )  /snake * CELLS snake-body + ;
: snake-y  ( offset -- address )  /snake * CELLS snake-body CELL+ + ;
: snake-c  ( offset -- address )  /snake * CELLS snake-body 2 CELLS + + ;

VARIABLE direction
VARIABLE length
VARIABLE tick

0 CONSTANT left
1 CONSTANT up
2 CONSTANT right
3 CONSTANT down

: initialize-snake  ( -- )
  4 length !
  length @ 1+ 0 DO
    15 I snake-c !
    12 I - I snake-x !
    12 I snake-y !
  LOOP
  right direction ! ;

: grow-snake  ( -- )  1 length +! ;

: draw-snake  ( -- )
  length @ 0 DO  I snake-c @  I snake-x @ I  snake-y @  draw  LOOP
  length @ snake-x @
  length @ snake-y @
  draw-black ;

: check-collision ( -- f )  snake-x-head @ snake-y-head @ get-color 0<> ;

\ apple

VARIABLE apple-x
VARIABLE apple-y
VARIABLE apple-c

: set-apple         ( -- )        apple-y ! apple-x ! apple-c ! ;
: initialize-apple  ( -- )        15 10 10 set-apple ;
: draw-apple        ( -- )        apple-c @ apple-x @ apple-y @ draw ;
: random-color      ( -- c )      15 RND 1+ ;
: random-position   ( -- x y )    width 4 - RND 2 +  height 4 - RND 2 + ;
: random-apple      ( -- c x y )  random-color random-position ;



: move-apple  ( -- )
  apple-x @ apple-y @ draw-black
  random-apple set-apple ;

: check-apple  ( -- )
  snake-x-head @ apple-x @ =
  snake-y-head @ apple-y @ =
  AND IF
    apple-c @ snake-c-head !
    move-apple
    grow-snake
  THEN ;


\ updating
: update-up     ( -- )  -1 snake-y-head +! ;
: update-down   ( -- )   1 snake-y-head +! ;
: update-left   ( -- )  -1 snake-x-head +! ;
: update-right  ( -- )   1 snake-x-head +! ;

: update-snake-head  ( -- )
  direction @ CASE
    up    OF update-up    ENDOF
    down  OF update-down  ENDOF
    left  OF update-left  ENDOF
    right OF update-right ENDOF
  ENDCASE ;

\ Update each segment of the snake forward by one
: update-snake-tail  ( -- )
  0 length @ DO
    I snake-x @  I 1+ snake-x  !
    I snake-y @  I 1+ snake-y  !
    I snake-c @  I 1+ snake-c  !
  -1 +LOOP ;

: update-snake-tail  ( -- )
  snake-body  snake-body /snake CELLS +  length @ /snake CELLS *  MOVE ;

\ movement
: is-horizontal  ( -- )  direction @ DUP  left = SWAP  right =  OR ;
: is-vertical    ( -- )  direction @ DUP    up = SWAP   down =  OR ;
: turn-up        ( -- )  is-horizontal IF     up direction !  THEN ;
: turn-left      ( -- )  is-vertical   IF   left direction !  THEN ;
: turn-down      ( -- )  is-horizontal IF   down direction !  THEN ;
: turn-right     ( -- )  is-vertical   IF  right direction !  THEN ;

: change-direction  ( -- )
  SCANCODE_A pressed? tick @ 0= AND IF turn-left  EXIT THEN
  SCANCODE_W pressed? tick @ 0= AND IF turn-up    EXIT THEN
  SCANCODE_D pressed? tick @ 0= AND IF turn-right EXIT THEN
  SCANCODE_S pressed? tick @ 0= AND IF turn-down  EXIT THEN
  SCANCODE_Q just-pressed? IF retro-40   EXIT THEN
;

: check-input  ( -- )  change-direction ;

\ initialisation
: <init>  ( -- )
  0 cls  draw-walls  initialize-snake  initialize-apple ;

\ entry point
: <update>
  check-input
  tick @ 0= IF
    3 tick !
    update-snake-tail
    update-snake-head
    check-apple
    check-collision IF retro-40 THEN
  ELSE
    -1 tick +!
  THEN
;

: <draw>
  draw-snake
  draw-apple
 ;

INSTALL snake
