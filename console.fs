\ the shell-thing
CREATE cmdline 64 ALLOT
0 VALUE cmdlen

CREATE cmdline' 64 ALLOT
0 VALUE cmdlen'

0 VALUE tick

\  0 VALUE sx
\  0 VALUE sy
\ 15 VALUE sc
\  0 VALUE sb

\ terminal code
: PUT  ( c color x y -- )  3 ROLL  @font  3 -ROLL  udgput ;

: PUTS ( addr u color x y )
  { addr u color x y }
  u 0 ?DO
    addr I + C@  color x y PUT
    4 x + TO x
  LOOP ;


: mockup
  s" DATA DISPLAY"   12 8      8  puts
  s" 09jul18 16:40"  13 W 8 -  8  puts
  s" MOCK-UP DISPLAY" 6 W 2/   8  puts
  6 W 2/ 15 2/ 1+ 8 * -  18  W 2/ 15 2/ 2+ 8 * +  18  line ;

: scroll ( -- )
  W 6 * VRAM +  ( line1pos+fbaddr )
  VRAM          ( line1pos+fbaddr fbaddr )
  W H 6 - *     ( line1pos+fbaddr fbaddr surface )
  MOVE
  bg @ 0 H 6 - W 6 rect
;

: ?scroll sy @ H 6 / 1- >  IF  scroll  -1 sy @ + sy ! THEN ;

: move-cursor
  1 sx +!
  sx @ W 4 / 1- >  IF  1 sy +!   0 sx ! THEN
  ?scroll ;

: at-xy     ( x y -- )  sy ! sx ! ;

: ?put  ( c -- )
  \ non-printable characters
  DUP 10 = IF  0 sx !  1 sy +! DROP  ?scroll EXIT THEN
  DUP 13 = IF  0 sx !  1 sy +! DROP  ?scroll EXIT THEN
  \ printable characters
  DUP 32 127 WITHIN IF  fg @  sx @ 4 *  sy @ 6 *  put  move-cursor  EXIT THEN
  DROP
;

: ?puts  ( addr u -- )  0 ?DO DUP  C@ ?put  1+ LOOP DROP ;

: <init>
  cmdline 64 ERASE
  15 fg !  0 bg !  bg @ cls
;

: exec-cmdline
  \ breaks IF some word is NOT found (possibly other errors as well)...
  \ can we circumvent it by revectoring ABORT ?
  cmdline' 64 ERASE
  cmdline cmdline' cmdlen MOVE
  cmdlen TO cmdlen'
  cmdline 64 ERASE
  0 TO cmdlen
  cmdline' cmdlen' EVALUATE
  CR
;

0 VALUE cursor-drawn?
: undraw-cursor  ( f -- )  cursor-drawn? IF  bg @ sx @ 4 * sy @ 6 * 4 6 rect  FALSE TO cursor-drawn?  THEN ;
: draw-cursor    ( f -- )  13  sx @ 4 * sy @ 6 * 4 6 rect  TRUE TO cursor-drawn? ;

: process-key  ( c -- )
  \ keybuffer 16 dump

  \ enter/execute
  13 OVER = IF
    DROP
    undraw-cursor
    0 sx !  1 sy +!
    exec-cmdline
    ?scroll
    EXIT
  THEN

  10 OVER = IF
    DROP
    undraw-cursor
    0 sx !   1 sy +!
    exec-cmdline
    ?scroll
    EXIT
  THEN

  \ delete backwards
  8 OVER = IF
    sx @ 0> IF
      DROP undraw-cursor
      -1 cmdlen + TO cmdlen  0 cmdline cmdlen + C!
      THEN
    EXIT
  THEN

  \ exit
  \  ASCII ^ OVER = IF  TRUE stopping !  EXIT THEN

  \ other chars
  cmdline cmdlen + C!
  cmdlen 1+ TO cmdlen
  1 sx +!
;

: <update>
  keybuffer { kb }
  BEGIN  kb C@  WHILE  kb C@ process-key  kb 1+ TO kb  REPEAT
  tick 1+ 100 MOD TO tick
;

: <draw>
  0 sx !
  cmdline cmdlen TYPE
  tick 50 < IF undraw-cursor ELSE draw-cursor THEN
;

\ INSTALL retro-40


\ : boot sdl-init retro-40 run ;

\ this does weird things when the file is included...
\ BOOT


\ this word starts the system, and will restart it when invoked
INSTALL retro-40
