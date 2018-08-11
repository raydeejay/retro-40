\ gravity simulation thingie
\ ported from https://github.com/incinirate/Riko4/blob/master/scripts/home/demos/grav.lua


\ some support code to be added to library
: >ints    ( F: a b -- a b )  F>S F>S SWAP ;
: fwithin  ( f: u a b -- f )  ROT F>S -ROT WITHIN ;


\ create a new vocabulary and push it to the top of the search order
17 FICL-VOCABULARY gravity-voc
ALSO gravity-voc

\ put the following definitions into the topmost vocabulary
DEFINITIONS

\ an additional framebuffer to store the trails
CREATE underlay W H * ALLOT

: on-field?  ( x y -- f )  0 H WITHIN  SWAP  0 W WITHIN  AND ;

: xy>upixel  ( f: x y -- u )  ( >INTS ) W * + ;

: u!  ( u x y -- )  2DUP on-field?  IF  xy>upixel underlay + C!  ELSE DROP 2DROP THEN ;
: u@  ( x y -- f )  2DUP on-field?  IF  xy>upixel underlay + C@  ELSE 2DROP FALSE THEN ;


\ particles container, field
200 CONSTANT ^particles
0 VALUE #particles

: particles    ( u -- u' )    5 CELLS * ;

CREATE field ^particles particles ALLOT
: th-particle  ( u -- addr )  particles field + ;


\ particle API
: +particle  ( c y x -- )
  field #particles particles + ( addr ) >R
     S>F R@ F!
     S>F R@ 1 CELLS+ F!
     S>F R@ 2 CELLS+ F!
  0e R@ 3 CELLS+ F!
  0e R> 4 CELLS+ F!
  #particles 1+ TO #particles
;

: -particle  ( u -- )
  { u }
  u 1+ th-particle  u th-particle  #particles u - 1- particles  MOVE
  #particles 1- TO #particles
;

: ?+particle  ( b x y -- )  #particles ^particles < IF  +particle  THEN ;
: ?-particle      ( u -- )  DUP 0 #particles WITHIN IF  -particle  ELSE DROP  THEN ;


\ static objects
CREATE objects 6 CELLS ALLOT
  \     80 ,  H 2/ ,  -18 ,
  \ W 80 - ,  H 2/ ,    6 ,

: th-object  ( u -- addr )  3 CELLS * objects + ;

\ input
: update-mouse  ( -- )
  MOUSEB @ CASE
    1 OF  -2 MOUSEY @ MOUSEX @ ?+particle  ENDOF \ negative charge
    4 OF   2 MOUSEY @ MOUSEX @ ?+particle  ENDOF \ positive charge
  ENDCASE
;

: update-keys  ( -- )  SCANCODE_Q  just-pressed?  IF  retro-40  THEN ;

\ simulation
: force+  ( part# -- )
  th-particle DUP
              DUP 3 CELLS+ F@ F+!
        CELL+ DUP 3 CELLS+ F@ F+!
;

: apply-object  ( obj# part# -- f )
  { obj# part# | f:dx f:dy f:norm }
  \ calculate delta
  obj# th-object       F@  part# th-particle       F@  F- TO dx
  obj# th-object CELL+ F@  part# th-particle CELL+ F@  F- TO dy
  
  \ calculate normal ( square root of the sum of the squares )
  dx FDUP F*  dy FDUP F*  F+ FSQRT TO norm
  
  \ kill when too far or too close
  norm  obj# th-object 2 CELLS+ F@ FABS  F<
  norm 500e F>
  OR IF  part# ?-particle  FALSE EXIT  THEN

  \ calculate force delta
  part# th-particle 2 CELLS+ F@   obj# th-object 2 CELLS+ F@  F* 0.1e F*  norm F/

  \ apply force delta to force
  FDUP dx norm F/ F*   part# th-particle 3 CELLS+ F+!
       dy norm F/ F*   part# th-particle 4 CELLS+ F+!

  \ signal that the particle is still alive
  TRUE
;

: cycle  ( -- )
  { | alive }
  0 #particles ?DO
    I force+
    0e I th-particle 3 CELLS+ F!
    0e I th-particle 4 CELLS+ F!
    TRUE TO alive
    2 0 DO
      \ accumulate force from objects
      I J apply-object  alive AND TO alive
    LOOP
    \ only update when the particle is still alive
    alive IF
      \ cache old coords
      \ apply force
      I th-particle 3 CELLS+ F@  I th-particle        F+!
      I th-particle 4 CELLS+ F@  I th-particle CELL+  F+!
      
      \ plot pixel IF the coords have changed
      \   pick color from the charge of the particle
      I th-particle 2 CELLS+ F@ F0> IF 4 ELSE 5 THEN
      \ draw
      I th-particle F@ F>S  I th-particle CELL+ F@ F>S  u!
    THEN
  -1 +LOOP
;

: calculate-forces  ( -- )   10 0 DO cycle LOOP ;


: <update>     ( -- )  update-mouse update-keys calculate-forces ;


\ rendering
: draw-underlay   ( -- )   underlay VRAM  W H *  MOVE ;

: draw-objects    ( -- )
  2 0 ?DO
    15 I th-object F@ F>S   I th-object CELL+ F@ F>S   I th-object 2 CELLS+ F@ F>S  ABS circ
  LOOP
;

: draw-particles  ( -- )
  #particles 0 ?DO
    1  I th-particle F@ F>S   I th-particle CELL+ F@ F>S   I th-particle 2 CELLS+ F@ F>S ABS  circ
  LOOP
;

: <draw>       ( -- )  0 cls draw-underlay draw-objects draw-particles ;


\ init
: <init>       ( -- )
  underlay W H * ERASE
  objects
      80 S>F DUP F! CELL+
    H 2/ S>F DUP F! CELL+
     -18 S>F DUP F! CELL+
  W 80 - S>F DUP F! CELL+
    H 2/ S>F DUP F! CELL+
       7 S>F F!
;


\ done adding definitions to the vocabulary
PREVIOUS DEFINITIONS

\ install the code in the general wordlist
ALSO gravity-voc
INSTALL gravity

\ restore the previous search order
PREVIOUS
