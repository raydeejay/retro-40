\ micro.fs

\ this is a Forth/Ficl/Retro-40 port
\ of my S7/Scheme/lambda8 port
\ of Matt Hughson's (@mhughson) TIC-80 port
\ of his Pico-8 Micro Platformer Starter Kit tech demo

\ the original introduction, complete with
\ its header, follows:

\  title:  Micro-Platformer Starter Kit
\  author: Matt Hughson (@mhughson)
\  desc:   Platforming engine in ~100 lines of code.
\  script: lua

\  the goal of this cart is to
\  demonstrate a very basic platforming
\  engine in under 100 lines of *code*,
\  while still maintaining an organized
\  and documented game.
\
\  it isn't meant to be a demo of doing
\  as much as possible, in as little
\  code as possible.  the 100 line limit
\  is just meant to encourage people
\  that "hey, you can make a game' with
\  very little coding!"
\
\  this will hopefully give new users a
\  simple and easy to understand
\  starting point for their own
\  platforming games.
\
\  NOTE: collision routine is based on
\  mario bros 2 and mckids, where we use
\  collision points rather than a box.
\  this has some interesting bugs but if
\  it was good enough for miyamoto, its
\  good enough for me!

CREATE mapbytes
1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 2 C, 2 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 2 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 2 C, 2 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 3 C, 2 C, 2 C, 3 C, 3 C, 3 C, 3 C, 3 C, 2 C, 2 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 2 C, 1 C, 1 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 2 C, 3 C, 3 C, 3 C, 2 C, 2 C, 2 C, 2 C, 2 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 2 C, 1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 2 C, 1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 3 C, 2 C, 2 C, 2 C, 2 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 1 C, 3 C, 2 C, 3 C, 3 C, 3 C, 3 C, 3 C, 1 C, 1 C, 1 C, 1 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 2 C, 3 C, 3 C, 2 C, 1 C, 1 C, 3 C, 1 C, 3 C, 2 C, 3 C, 3 C, 3 C, 1 C, 1 C, 1 C, 1 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
1 C, 3 C, 1 C, 3 C, 2 C, 1 C, 1 C, 1 C, 3 C, 1 C, 3 C, 1 C, 3 C, 2 C, 3 C, 1 C, 1 C, 1 C, 1 C, 1 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,
2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 2 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C, 5 C,

0 DEFINE-SFX jump.wav

: initmap  ( -- )
  mapbytes 15 0 DO
    32 0 DO
      DUP C@  J W * I + MRAM +  C!  1+
    LOOP
  LOOP DROP
;

: solid?  ( tile# -- f ) DUP 1 = SWAP 2 = OR ;

0.1e FCONSTANT gravity

VARIABLE x
VARIABLE y
VARIABLE dx
VARIABLE dy
VARIABLE grounded?
VARIABLE jump-velocity

\ moving sideways
: walk  ( -- )
  0e dx F!
  SCANCODE_A pressed? IF  -1e dx F!  THEN
  SCANCODE_D pressed? IF   1e dx F!  THEN
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
  s" default.spr" load-sprites
  MRAM W H * 5 FILL
  initmap init-player
;

: <update>  ( -- )  ?jump  walk  move  ?pushback  accumulate-gravity  fall  ?hit-floor  ?hit-ceiling ;

: <draw>  ( -- )  0 cls  map  x F@ F>S y F@ F>S 0 spr  s" v1.0 2016 - @matthughson" 15 0 0 PUTS ;

INSTALL micro
