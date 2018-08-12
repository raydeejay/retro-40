\ raycasting example

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
0 VALUE tick

\ floats
VARIABLE rayDirX
VARIABLE rayDirY

\ ints
VARIABLE mapX
VARIABLE mapY

: calc-position-and-direction
  { | f:cameraX }
  column @ 2* S>F   W S>F  F/  1e f-  TO cameraX
  cameraX planeX F@ F*   dirX F@ F+  rayDirX F!
  cameraX planeY F@ F*   dirY F@ F+  rayDirY F!
  \ the axis are transposed in every M@ call, because in the original
  \ code the map data was transposed...  should probably fix the
  \ formulas and names
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
VARIABLE side

: calc-step-and-sideDist
  rayDirY F@ FDUP F* rayDirX F@ FDUP F* F/ 1e F+ FSQRT   deltaDistX F!
  rayDirX F@ FDUP F* rayDirY F@ FDUP F* F/ 1e F+ FSQRT   deltaDistY F!
  rayDirX F@ F0< IF
    -1 stepX !
    posX F@          mapX @ S>F    F- deltaDistX F@ F* sideDistX F!
  ELSE
    1 stepX !
    mapX @ 1+ S>F    posX F@       F- deltaDistX F@ F* sideDistX F!
  THEN
  rayDirY F@ F0< IF
    -1 stepY !
    posY F@          mapY @ S>F    F- deltaDistY F@ F* sideDistY F!
  ELSE
    1 stepY !
    mapY @ S>F 1e F+ posY F@       F- deltaDistY F@ F* sideDistY F!
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
    mapY @ mapX @ M@
  UNTIL
;

\ Calculate distance projected on camera direction
\ (Euclidean distance will give fisheye effect!)
: calc-distance-projected
  side @ IF
    1 stepY @ - 2/  mapY @ +  S>F posY F@ F-  rayDirY F@ F/ perpWallDist F!
  ELSE
    1 stepX @ - 2/  mapX @ +  S>F posX F@ F-  rayDirX F@ F/ perpWallDist F!
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
VARIABLE texX

\ float
VARIABLE wallX

: calc-wallX
  perpWallDist F@  side @ IF
    rayDirX F@ F*  posX F@ F+
  ELSE
    rayDirY F@ F*  posY F@ F+
  THEN   wallX F!
  wallX F@ F>S NEGATE S>F  wallX F+!
;

: calc-texX
  wallX F@ 8e F* F>S texX !
  \ note: not sure of what these two lines are supposed to do... adjust the colour?
  \ update: they prevent mirroring :-)
  side @  0= rayDirX F@ F0> AND IF  8 texX @ - 1- texX !  THEN
  side @ 1 = rayDirY F@ F0< AND IF  8 texX @ - 1- texX !  THEN
;

\ store  texX lineHeight drawStart drawEnd texNum
CREATE hbuf W 6 CELLS * ALLOT
hbuf VARIABLE >hbuf

: draw-line
  { | d texY color texnum }
  mapY @ mapX @ M@ TO texnum
  drawEnd @ drawStart @ ?DO
    I 256 * H 128 * - lineHeight @ 128 * + TO D
    8 d * lineHeight @ /  256 /  TO texY
    texX @  texY  texnum
    DUP 34 = IF  tick 25 / +  THEN
    ( x y sprite ) SP@  TO color
    color column @ I P!
  LOOP
;

: draw-from-hbuf
  6 0 DO  >hbuf @ I CELLS +  @  LOOP
  { column texX lineHeight drawStart drawEnd texNum | texY color }

  drawEnd drawStart ?DO
    I 256 * H 128 * - lineHeight 128 * +
    8 * lineHeight /  256 /  TO texY
    texX  texY  texNum
    DUP 34 = IF  tick 25 / +  THEN
    ( x y sprite ) SP@  TO color
    color column I P!
  LOOP
  6 CELLS >hbuf +!
;

: add-to-hbuf
  column @  texX @  lineHeight @  drawStart @  drawEnd @  mapY @ mapX @ M@
  0 5 DO  >hbuf @ I CELLS +  ! -1 +LOOP
  6 CELLS >hbuf +!
;

: calc-3d
  hbuf >hbuf !
  W 0 DO
    I column !
    calc-position-and-direction
    1e rayDirX F@ F/ FABS  deltaDistX F!
    1e rayDirY F@ F/ FABS  deltaDistY F!
    0 side !
    calc-step-and-sideDist
    perform-DDA
    calc-distance-projected
    calc-lowest-highest
    calc-wallX
    calc-texX
    \ draw-line
    \ add data to a buffer, to be rendered later
    add-to-hbuf
  LOOP ;

: draw-3d
  hbuf >hbuf !
  W 0 DO
    I column !
    draw-from-hbuf
    \ render entities from near to far
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
  0 TO tick
;

: ?exit  ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;

0.05e FCONSTANT moveSpeed
0.03e FCONSTANT rotSpeed

: move-forward
  posY F@ F>S    posX F@ dirX F@ moveSpeed F* F+ F>S  M@ 0= IF
    dirX F@ moveSpeed F*  posX F+!
  THEN
  posY F@ dirY F@ moveSpeed F* F+ F>S  posX F@ F>S    M@ 0= IF
    dirY F@ moveSpeed F*  posY F+!
  THEN
;

: move-backward
  posY F@ F>S  posX F@ dirX F@ moveSpeed F* F- F>S   M@ 0= IF
    dirX F@ moveSpeed F*  FNEGATE posX F+!
  THEN
  posY F@ dirY F@ moveSpeed F* F- F>S    posX F@ F>S  M@ 0= IF
    dirY F@ moveSpeed F*  FNEGATE posY F+!
  THEN
;

: turn-left
  { | f:rcos f:rsin }
  rotSpeed FDUP  FSIN TO rsin  FCOS to rcos
  dirX F@ FDUP     rcos F*    dirY F@ rsin F*  F-  dirX F!
                   rsin F*    dirY F@ rcos F*  F+  dirY F!
  planeX F@ FDUP   rcos F*  planeY F@ rsin F*  F-  planeX F!
                   rsin F*  planeY F@ rcos F*  F+  planeY F!
;

: turn-right
  { | f:rcos f:rsin }
  rotSpeed FNEGATE FDUP  FSIN TO rsin  FCOS to rcos
  dirX F@ FDUP     rcos F*    dirY F@ rsin F*  F-  dirX F!
                   rsin F*    dirY F@ rcos F*  F+  dirY F!
  planeX F@ FDUP   rcos F*  planeY F@ rsin F*  F-  planeX F!
                   rsin F*  planeY F@ rcos F*  F+  planeY F!
;

: ?move  ( -- )
  SCANCODE_W pressed? IF  move-forward   THEN
  SCANCODE_S pressed? IF  move-backward  THEN
;

: ?turn  ( -- )
  SCANCODE_A pressed? IF  turn-left   THEN
  SCANCODE_D pressed? IF  turn-right  THEN
;

: animate   ( -- )  tick 1+ 100 MOD TO tick ;

: <update>  ( -- )  ?move  ?turn   animate calc-3d  ?exit ;

: ceiling   ( -- )  12  0 0     W H 2/  rect ;
: floor     ( -- )   2  0 H 2/  W H 2/  rect ;

: <draw>    ( -- )  0 cls  ceiling  floor  draw-3d ;

\ install the software
PREVIOUS DEFINITIONS

ALSO raycast-voc

INSTALL raycast

PREVIOUS
