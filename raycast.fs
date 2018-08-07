\ uses sprites 8-15

17 ficl-vocabulary raycast-voc
also raycast-voc definitions

0 DEFINE-SFX jump.wav

\ floats
VARIABLE posX
VARIABLE posY
VARIABLE dirX
VARIABLE dirY
VARIABLE planeX
VARIABLE planeY

\ int
VARIABLE column

\ floats
VARIABLE cameraX
VARIABLE rayDirX
VARIABLE rayDirY

\ ints
VARIABLE mapX
VARIABLE mapY

: calc-position-and-direction
  column @ 2* S>F   W S>F  F/  1e f-  cameraX F!
  cameraX F@ planeX F@ F*   dirX F@ F+  rayDirX F!
  cameraX F@ planeY F@ F*   dirY F@ F+  rayDirY F!
  posX F@ F>S mapX !
  posY F@ F>S mapY !
;

\ floats
VARIABLE sideDistX
VARIABLE sideDistY

\ floats
VARIABLE deltaDistX
VARIABLE deltaDistY
VARIABLE perpWallDist

\ ints
VARIABLE stepX
VARIABLE stepY

\ ints
VARIABLE hit
VARIABLE side


: calc-step-and-sideDist
  rayDirX F@ F0< IF
    -1 stepX !
    posX F@ mapX @ S>F F- deltaDistX F@ F* sideDistX F!
  ELSE
    1 stepX !
    mapX @ S>F 1e F+ posX F@ F- deltaDistX F@ F* sideDistX F!
  THEN
  rayDirY F@ F0< IF
    -1 stepY !
    posY F@ mapY @ S>F F- deltaDistY F@ F* sideDistY F!
  ELSE
    1 stepY !
    mapY @ S>F 1e F+ posY F@ F- deltaDistY F@ F* sideDistY F!
  THEN
;

: perform-DDA
  BEGIN
    sideDistX F@ sideDistY F@ F< IF
      deltaDistX F@ sideDistX F+!
      stepX @ mapX +!
      0 side !
    ELSE
      deltaDistY F@ sideDistY F+!
      stepY @ mapY +!
      1 side !
    THEN
    \ check if ray hits a wall
    mapX @ mapY @ M@
  UNTIL
;

\ Calculate distance projected on camera direction
\ (Euclidean distance will give fisheye effect!)
: calc-distance-projected
  side @ IF
    1 stepY @ - 2/  mapY @ +  S>F posY F@ F-  rayDirY F@ F/  perpWallDist F!
  ELSE
    1 stepX @ - 2/  mapX @ +  S>F posX F@ F-  rayDirX F@ F/  perpWallDist F!
  THEN
;

\ ints
VARIABLE lineHeight
VARIABLE drawStart
VARIABLE drawEnd

: calc-lowest-highest
  H S>F  perpWallDist F@ F/  F>S lineHeight !
  lineHeight @ NEGATE 2/  H 2/  +    0 MAX drawStart !
  lineHeight @ 2/         H 2/  + H 1- MIN drawEnd !
;

\ ints
VARIABLE lineColor

: calc-color
  mapX @ mapY @ M@  lineColor !
\  side @ IF lineColor @ 2/ lineColor ! THEN
;

: draw-line  lineColor @  column @  drawStart @  column @  drawEnd @  line ;

: draw3d
  W 0 DO
    I column !
    calc-position-and-direction
    1e rayDirX F@ F/ FABS  deltaDistX F!
    1e rayDirY F@ F/ FABS  deltaDistY F!
    FALSE hit !
    0 side !
    calc-step-and-sideDist
    perform-DDA
    calc-distance-projected
    calc-lowest-highest
    calc-color
    draw-line
  LOOP
;


: <init>
  s" default.spr" load-sprites
  s" default.map" load-map
     4e posX F!
     4e posY F!
    -1e dirX F!
     0e dirY F!
     0e planeX F!
  0.66e planeY F!
;

: ?exit  ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;

VARIABLE oldDirX
VARIABLE oldPlaneX

0.05e FCONSTANT moveSpeed
0.03e FCONSTANT rotSpeed

: move-forward
  posX F@ dirX F@ moveSpeed F* F+ F>S  posY F@ F>S  M@ 0= IF
    dirX F@ moveSpeed F*  posX F+!
  THEN
  posX F@ F>S  posY F@ dirY F@ moveSpeed F* F+ F>S  M@ 0= IF
    dirY F@ moveSpeed F*  posY F+!
  THEN
;

: move-backward
  posX F@ dirX F@ moveSpeed F* F- F>S  posY F@ F>S  M@ 0= IF
    dirX F@ moveSpeed F*  FNEGATE posX F+!
  THEN
  posX F@ F>S  posY F@ dirY F@ moveSpeed F* F- F>S  M@ 0= IF
    dirY F@ moveSpeed F*  FNEGATE posY F+!
  THEN
;

: turn-left
  dirX F@ oldDirX F!
  planeX F@ oldPlaneX F!
       dirX F@ rotSpeed FCOS F*    dirY F@ rotSpeed FSIN F*  F-  dirX F!
    oldDirX F@ rotSpeed FSIN F*    dirY F@ rotSpeed FCOS F*  F+  dirY F!
     planeX F@ rotSpeed FCOS F*  planeY F@ rotSpeed FSIN F*  F-  planeX F!
  oldPlaneX F@ rotSpeed FSIN F*  planeY F@ rotSpeed FCOS F*  F+  planeY F!
;

: turn-right
  dirX F@ oldDirX F!
  planeX F@ oldPlaneX F!
       dirX F@ rotSpeed FNEGATE FCOS F*    dirY F@ rotSpeed FNEGATE FSIN F*  F-  dirX F!
    oldDirX F@ rotSpeed FNEGATE FSIN F*    dirY F@ rotSpeed FNEGATE FCOS F*  F+  dirY F!
     planeX F@ rotSpeed FNEGATE FCOS F*  planeY F@ rotSpeed FNEGATE FSIN F*  F-  planeX F!
  oldPlaneX F@ rotSpeed FNEGATE FSIN F*  planeY F@ rotSpeed FNEGATE FCOS F*  F+  planeY F!
;

: ?move  ( -- )
  SCANCODE_W pressed? IF  move-forward   THEN
  SCANCODE_S pressed? IF  move-backward  THEN
;

: ?turn  ( -- )
  SCANCODE_A pressed? IF  turn-left   THEN
  SCANCODE_D pressed? IF  turn-right  THEN
;

: <update>  ( -- )  ?move  ?turn  ?exit ;
: <draw>    ( -- )  0 cls  draw3d ;

\ install the software
PREVIOUS DEFINITIONS

ALSO raycast-voc

INSTALL raycast

PREVIOUS



\ : solid?  ( tile# -- f ) DUP 1 = SWAP 2 = OR ;

\ 0.1e FCONSTANT gravity

\ VARIABLE x
\ VARIABLE y
\ VARIABLE dx
\ VARIABLE dy
\ VARIABLE grounded?
\ VARIABLE jump-velocity
\ VARIABLE facing-left
\ 0 VALUE tick
\ 0 VALUE offx
\ 0 VALUE offy

\ \ moving sideways
\ : walk  ( -- )
\   0e dx F!
\   SCANCODE_A pressed? IF  tick 1+ 32 MOD TO tick  -1e dx F!  TRUE  facing-left ! THEN
\   SCANCODE_D pressed? IF  tick 1+ 32 MOD TO tick   1e dx F!  FALSE facing-left ! THEN
\ ;
\ : destination-h  ( -- )
\   x F@ F>S  dx F@ F0> IF 7 ELSE 0 THEN +  8 /
\   y F@ F>S  7 +  8 /
\   m@
\ ;

\ VARIABLE startx
\ : move       ( -- )  x F@ startx F!  dx F@ x F+! ;
\ : ?pushback  ( -- )  destination-h solid? IF  startx F@ x F!  THEN ;

\ \ moving downwards
\ : accumulate-gravity  ( -- )  gravity dy F+! ;
\ : fall                ( -- )  dy F@ y F+! ;

\ : destination-down  ( -- )
\   FALSE grounded? !
\   x F@ F>S  4 +  8 /
\   y F@ F>S  8 +  8 /
\   m@
\ ;

\ : ?hit-floor  ( -- )
\   dy F@ 0e F>=  destination-down solid?  AND
\   IF
\     y F@ 8e F/ F>S 8 * S>F y F!
\     0e dy F!
\     TRUE grounded? !
\   THEN
\ ;

\ \ moving upwards
\ : ?jump  ( -- )
\   SCANCODE_W just-pressed?  SCANCODE_SPACE just-pressed?  OR
\   grounded? @  AND
\   IF  dy F@ jump-velocity F@ F-  dy F!  0 sfx  THEN
\ ;

\ : destination-up  ( -- )
\   x F@ F>S  4 +  8 /
\   y F@ F>S  8 /
\   m@
\ ;

\ : ?hit-ceiling  ( -- )
\   dy F@ 0e F<=  destination-up solid?  AND
\   IF
\     y F@ 8e F+ 8e F/ F>S 8 * S>F y F!
\     0e dy F!
\   THEN
\ ;


\ hooks

\ : calc-offsets  ( -- )
\   \ determine horizontal offset
\   x F@ F>S W 2/ -
\   DUP  0< IF DROP 0 THEN
\   DUP  W 8 * W - > IF DROP W 8 * W - THEN
\   TO offx
\   \ determine vertical offset ( you may want to place the line other than in the middle )
\   y F@ F>S H 2/ -
\   DUP  0< IF DROP 0 THEN
\   DUP  H 8 * H - > IF DROP H 8 * H - THEN
\   TO offy
\ ;

