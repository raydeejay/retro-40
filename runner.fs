17 ficl-vocabulary runner-voc
also runner-voc definitions

0 DEFINE-SFX jump.wav
1 DEFINE-SFX stomp.wav
2 DEFINE-SFX coin.wav
3 DEFINE-SFX death.wav

: solid?  ( tile# -- f ) 32 >= ;

: intersect?  ( x0 y0 x1 y1 -- f )
  ROT ( y1 y0 )
  2DUP < IF SWAP THEN
  - 0 8 WITHIN ( x0 x1 f )
  -ROT ( f x0 x1 )
  2DUP < IF SWAP THEN
  - 0 8 WITHIN ( f f )
  AND
;

0.1e FCONSTANT gravity

0 VALUE tick
0 VALUE offx
0 VALUE offy
0 VALUE coins

VARIABLE x
VARIABLE y
VARIABLE dx
VARIABLE dy
VARIABLE grounded?
VARIABLE jump-velocity
VARIABLE facing-left

\ --------------------------------------------------
\ entities
\ --------------------------------------------------
8 CELLS CONSTANT /entity
CREATE entities 256 /entity * ALLOT
0 VALUE #entities

1 CONSTANT type-enemy
2 CONSTANT type-coin

: active@    ( entity# -- f ) /entity * entities + @ ;
: active!    ( f entity# -- ) /entity * entities + ! ;
: x@         ( entity# -- f ) /entity * entities + 1 CELLS+ F@ ;
: x!         ( f entity# -- ) /entity * entities + 1 CELLS+ F! ;
: y@         ( entity# -- f ) /entity * entities + 2 CELLS+ F@ ;
: y!         ( f entity# -- ) /entity * entities + 2 CELLS+ F! ;
: dx@        ( entity# -- f ) /entity * entities + 3 CELLS+ F@ ;
: dx!        ( f entity# -- ) /entity * entities + 3 CELLS+ F! ;
: dy@        ( entity# -- f ) /entity * entities + 4 CELLS+ F@ ;
: dy!        ( f entity# -- ) /entity * entities + 4 CELLS+ F! ;
: faceleft@  ( entity# -- f ) /entity * entities + 5 CELLS+ @ ;
: faceleft!  ( f entity# -- ) /entity * entities + 5 CELLS+ ! ;
: frame@     ( entity# -- f ) /entity * entities + 6 CELLS+ @ ;
: frame!     ( f entity# -- ) /entity * entities + 6 CELLS+ ! ;
: grounded@  ( entity# -- f ) /entity * entities + 7 CELLS+ @ ;
: grounded!  ( f entity# -- ) /entity * entities + 7 CELLS+ ! ;


\ --------------------------------------------------
\ enemies
\ --------------------------------------------------

: walk-enemy  ( entity# -- )  >R 0.3e R@ faceleft@ IF FNEGATE THEN  R@ x@ F+ R> x! ;

: accumulate-gravity-enemy  ( entity# -- )  { e } gravity e dy@ F0> IF 2e F* THEN e dy@ F+ e dy! ;
: fall-enemy                ( entity# -- )  { e } e dy@ e y@ F+ e y! ;

: destination-down-enemy  ( entity# -- )
  { e }
  FALSE e grounded!
  e x@ F>S  4 +  8 /
  e y@ F>S  8 +  8 /
  M@
;

: ?hit-floor-enemy  ( entity# -- )
  { e }
  e dy@ 0e F>=  e destination-down-enemy solid?  AND
  IF
    e y@ 8e F/ F>S 8 * S>F e y!
    0e e dy!
    TRUE e grounded!
  THEN
;

: ?bounce-enemy  ( entity# -- )
  { e }
  e faceleft@ IF
    e x@ F>S 8 / e y@ F>S 8 / M2@ 1 = IF FALSE e faceleft! THEN
  ELSE
    e x@ F>S 8 / 1+ e y@ F>S 8 / M2@ 1 = IF TRUE e faceleft! THEN
  THEN
;

: ?collide-enemy  ( entity# -- )
  { e }
  x F@ F>S  y F@ F>S
  e x@ F>S  e y@ F>S
  intersect?  dy F@ 0e F>  AND IF
    FALSE e active!
    -3.1e dy F!
    1 sfx
  THEN
;

: ?collide-coin  ( entity# -- )
  { e }
  x F@ F>S  y F@ F>S
  e x@ F>S  e y@ F>S
  intersect?  IF
    FALSE e active!
    coins 1+ TO coins
    2 sfx
  THEN
;

: update-entity ( entity# -- )
  { e }
  e active@ CASE
    type-enemy OF
      e ?bounce-enemy
      e walk-enemy
      e accumulate-gravity-enemy
      e fall-enemy
      e ?hit-floor-enemy
      e ?collide-enemy
      e frame@ 1+ 4 8 * MOD  e frame!
    ENDOF
    type-coin OF
      e ?collide-coin
    ENDOF
  ENDCASE
;

: ?update-enemies  ( -- ) #entities 0 ?DO  I update-entity  LOOP ;

: draw-enemies  ( -- )
  #entities 0 ?DO
    I active@ IF
      0 colorkey !
      I faceleft@ NOT IF 1 ELSE 0 THEN flip !
      I x@ F>S offx - I y@ F>S offy -
      I active@ CASE
        type-enemy OF  I frame@ 8 / 24 +  spr  ENDOF
        type-coin  OF  20  spr  ENDOF
      ENDCASE
      0 flip !
      -1 colorkey !
    THEN
  LOOP
;

\ --------------------------------------------------
\ player movement
\ --------------------------------------------------
\ moving sideways
: walk  ( -- )
  0e dx F!
  SCANCODE_A pressed? IF  tick 1+ 32 MOD TO tick  -1.3e dx F!  TRUE  facing-left ! THEN
  SCANCODE_D pressed? IF  tick 1+ 32 MOD TO tick   1.3e dx F!  FALSE facing-left ! THEN
;

: destination-h  ( -- )
  x F@ F>S  dx F@ F0> IF 7 ELSE 0 THEN +  8 /
  y F@ F>S  7 +  8 /
  m@
;

VARIABLE startx
: move       ( -- )  x F@ startx F!  dx F@ x F+! ;
: ?pushback  ( -- )  destination-h solid? IF  startx F@ x F!  THEN ;

\ moving downwards (gravity doubles when falling, for feeling)
: accumulate-gravity  ( -- )  gravity dy F@ F0> IF 2e F* THEN dy F+! ;
: fall                ( -- )  dy F@ y F+! ;

: destination-down  ( -- )
  FALSE grounded? !
  x F@ F>S  4 +  8 /
  y F@ F>S  8 +  8 /
  m@
;

: ?hit-floor  ( -- )
  dy F@ 0e F>=  destination-down solid?  AND
  IF
    y F@ 8e F/ F>S 8 * S>F y F!
    0e dy F!
    TRUE grounded? !
  THEN
;

\ moving upwards
: ?jump  ( -- )
  SCANCODE_SPACE just-pressed?  grounded? @  AND
  IF  dy F@ jump-velocity F@ F-  dy F!  0 sfx  THEN
;

: destination-up  ( -- )
  x F@ F>S  4 +  8 /
  y F@ F>S  8 /
  m@
;

: ?hit-ceiling  ( -- )
  dy F@ 0e F<=  destination-up solid?  AND
  IF
    y F@ 8e F+ 8e F/ F>S 8 * S>F y F!
    0e dy F!
  THEN
;

\ --------------------------------------------------
\ initialization
\ --------------------------------------------------
: init-player  ( -- )
  16e  x F!
  16e  y F!
  0e dx F!
  0e dy F!
  0 grounded? !
  3.1e jump-velocity F!
  0 TO coins
;


\ hooks
: <init>  ( -- )
  s" runner.spr" load-sprites
  s" runner.map" load-map
  init-player
;

\ --------------------------------------------------
\ game stuff
\ --------------------------------------------------
: ?exit  ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;

: calculate-offsets  ( -- )
  \ determine horizontal offset
  x F@ F>S W 2/ -
  DUP  0< IF DROP 0 THEN
  DUP  W 8 * W - > IF DROP W 8 * W - THEN
  TO offx
  \ determine vertical offset ( you may want to place the line other than in the middle )
  y F@ F>S H 2/ -
  DUP  0< IF DROP 0 THEN
  DUP  H 8 * H - > IF DROP H 8 * H - THEN
  TO offy
;

: <update>  ( -- )
  ?jump
  walk
  move
  ?pushback
  accumulate-gravity
  fall
  ?hit-floor
  ?hit-ceiling
  ?exit
  ?update-enemies
;

: spawn  ( -- )
  offy 8 / DUP 25 UNDER+ DO
    offx 8 / DUP 33 UNDER+ DO
      I J M2@ 2 = IF
        #entities 256 < IF
          type-enemy #entities active!
          I 8 * S>F #entities x!  J 8 * S>F #entities y!
          0 I J M2!
          #entities 1+ TO #entities
        THEN
      THEN
      I J M2@ 20 = IF
        #entities 256 < IF
          type-coin #entities active!
          I 8 * S>F #entities x!  J 8 * S>F #entities y!
          0 I J M2!
          #entities 1+ TO #entities
        THEN
      THEN
    LOOP
  LOOP
;

: draw-map  ( -- )  offx 8 /  offy 8 /  33 25  offx 8 MOD NEGATE offy 8 MOD NEGATE  map* ;

: draw-player  ( -- )
  0 colorkey !
  facing-left @ IF 1 ELSE 0 THEN flip !
  x F@ F>S offx -  y F@ F>S offy -  tick 8 / 8 + spr
  0 flip !
  -1 colorkey !
  0 0 at-xy s" COINS: " ?PUTS coins .
;

: <draw>  ( -- )
  0 cls
  \ apply offset
  calculate-offsets
  draw-map
  spawn
  draw-player
  draw-enemies
;

\ --------------------------------------------------
\ install the software
\ --------------------------------------------------
PREVIOUS DEFINITIONS

ALSO runner-voc

INSTALL runner

PREVIOUS
