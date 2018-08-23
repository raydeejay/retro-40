17 ficl-vocabulary runner-voc
also runner-voc definitions

0 DEFINE-SFX jump.wav
1 DEFINE-SFX zap.wav

: solid?  ( tile# -- f ) 32 >= ;

0.1e FCONSTANT gravity

0 VALUE tick
0 VALUE offx
0 VALUE offy

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

\ update

\ animate

\ draw

: intersect?  ( x0 y0 x1 y1 -- f )
  ROT ( y1 y0 )
  2DUP < IF SWAP THEN
  - 0 8 WITHIN ( x0 x1 f )
  -ROT ( f x0 x1 )
  2DUP < IF SWAP THEN
  - 0 8 WITHIN ( f f )
  AND
;


CREATE entities 128 ALLOT


\ --------------------------------------------------
\ enemies
\ --------------------------------------------------
VARIABLE enemy?
VARIABLE enemy-grounded?
VARIABLE enemy-x
VARIABLE enemy-y
VARIABLE enemy-dx
VARIABLE enemy-dy
VARIABLE enemy-faceleft
VARIABLE enemy-frame

: walk-enemy  ( -- )  0.3e enemy-faceleft @ IF FNEGATE THEN  enemy-x F+! ;

: accumulate-gravity-enemy  ( -- )  gravity enemy-dy F@ F0> IF 2e F* THEN enemy-dy F+! ;
: fall-enemy                ( -- )  enemy-dy F@ enemy-y F+! ;

: destination-down-enemy  ( -- )
  FALSE enemy-grounded? !
  enemy-x F@ F>S  4 +  8 /
  enemy-y F@ F>S  8 +  8 /
  m@
;

: ?hit-floor-enemy  ( -- )
  enemy-dy F@ 0e F>=  destination-down-enemy solid?  AND
  IF
    enemy-y F@ 8e F/ F>S 8 * S>F enemy-y F!
    0e enemy-dy F!
    TRUE enemy-grounded? !
  THEN
;

: ?bounce-enemy  ( -- )
  enemy-faceleft @ IF
    enemy-x F@ F>S 8 / enemy-y F@ F>S 8 / M@ 1 = IF FALSE enemy-faceleft ! THEN
  ELSE
    enemy-x F@ F>S 8 / 1+ enemy-y F@ F>S 8 / M@ 1 = IF TRUE enemy-faceleft ! THEN
  THEN
;

: ?collide-enemy  ( -- )
        x F@ F>S        y F@ F>S
  enemy-x F@ F>S  enemy-y F@ F>S
  intersect?  dy F@ 0e F>  AND IF
    FALSE enemy? !  1 sfx
  THEN
;

: ?update-enemies  ( -- )
  enemy? @ IF
    ?bounce-enemy
    walk-enemy
    accumulate-gravity-enemy
    fall-enemy
    ?hit-floor-enemy
    ?collide-enemy
    enemy-frame @ 1+ 4 8 * MOD  enemy-frame !
  THEN
;

: draw-enemies  ( -- )
  enemy? @ IF
    0 colorkey !
    enemy-faceleft @ NOT IF 1 ELSE 0 THEN flip !
    enemy-x F@ F>S offx - enemy-y F@ F>S offy -   enemy-frame @ 8 / 24 +  spr
    0 flip !
    -1 colorkey !
  THEN
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
     3.8e jump-velocity F!
     0 enemy? !
  TRUE enemy-faceleft !
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
      enemy? @ NOT IF
        I J M@ 2 = IF
          TRUE enemy? !
          I 8 * S>F enemy-x F!  J 8 * S>F enemy-y F!
          0 sfx
          0 I J M!
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
