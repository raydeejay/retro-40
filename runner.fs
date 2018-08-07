\ uses sprites 8-15

17 ficl-vocabulary runner-voc
also runner-voc definitions

0 DEFINE-SFX jump.wav

: solid?  ( tile# -- f ) DUP 1 = SWAP 2 = OR ;

0.1e FCONSTANT gravity

VARIABLE x
VARIABLE y
VARIABLE dx
VARIABLE dy
VARIABLE grounded?
VARIABLE jump-velocity
VARIABLE facing-left
0 VALUE tick
0 VALUE offx
0 VALUE offy

\ moving sideways
: walk  ( -- )
  0e dx F!
  SCANCODE_A pressed? IF  tick 1+ 32 MOD TO tick  -1e dx F!  TRUE  facing-left ! THEN
  SCANCODE_D pressed? IF  tick 1+ 32 MOD TO tick   1e dx F!  FALSE facing-left ! THEN
;
: destination-h  ( -- )
  x F@ F>S  dx F@ F0> IF 7 ELSE 0 THEN +  8 /
  y F@ F>S  7 +  8 /
  m@
;

VARIABLE startx
: move       ( -- )  x F@ startx F!  dx F@ x F+! ;
: ?pushback  ( -- )  destination-h solid? IF  startx F@ x F!  THEN ;

\ moving downwards
: accumulate-gravity  ( -- )  gravity dy F+! ;
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
  SCANCODE_W just-pressed?  SCANCODE_SPACE just-pressed?  OR
  grounded? @  AND
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

\ initialization
: init-player  ( -- )
   16e  x F!
   16e  y F!
    0e dx F!
    0e dy F!
     0 grounded? !
  3.8e jump-velocity F!
;


\ hooks
: <init>  ( -- )
  s" runner.spr" load-sprites
  s" default.map" load-map
  init-player
;

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
;

: <draw>  ( -- )
  0 cls
  \ apply offset
  calculate-offsets
  offx 8 /  offy 8 /  33 25  offx 8 MOD NEGATE offy 8 MOD NEGATE  map*
  facing-left @ IF 1 ELSE 0 THEN flip !
  0 colorkey !
  \ apply offset
  x F@ F>S offx -  y F@ F>S offy -  tick 8 / 8 + spr
  0 flip !    -1 colorkey !
  s" v1.0 2016 - @matthughson" 15 0 0 PUTS
;

\ install the software
PREVIOUS DEFINITIONS

ALSO runner-voc

INSTALL runner

PREVIOUS
