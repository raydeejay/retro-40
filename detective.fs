17 ficl-vocabulary detective-voc
also detective-voc definitions

0 DEFINE-SFX jump.wav

\ use a 128x96 positions map block
\ each tile represents an object
\ display the whole map one pixel per position
\ display a zoomed view, one sprite per position


: xy>tilexy  ( x y -- x/8 y/8 )  88 -  SWAP  120 -  SWAP ;

: dude?     ( u -- )  8 12 WITHIN ;
: place-dude  ( -- )  8 4 RND +  128 RND 96 RND  M! ;
: reset       ( -- )  s" detective.map" load-map  place-dude ;
: win         ( -- )  0 sfx  reset ;

: check-dude  ( mx my -- f )  xy>tilexy  M@ dude? IF  win  THEN ;

: update-mouse  ( -- )  MOUSEB @ IF  MOUSEX @  MOUSEY @  check-dude  THEN ;
: ?exit         ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;
: <update>      ( -- )  update-mouse  ?exit ;


: whole-map  ( -- )
  96 0 DO
    128 0 DO
      I J M@  120 I +  88 J +  P!
    LOOP
  LOOP
;

: zoomed-map  ( mx my -- )  88 - 3 -  SWAP  120 - 3 - SWAP  7 7  8 8  map* ;

\ blit the spotlight, using white as the transparent color
: spotlight  ( mx my --)  15 colorkey !   -24 UNDER+ -24 +   6 6  16 spr*   -1 colorkey ! ;

: <draw>    ( -- )  0 cls  whole-map  MOUSEX @ MOUSEY @ zoomed-map ;


: <init>    ( -- )  s" detective.spr" load-sprites  reset ;

\ install the software
PREVIOUS DEFINITIONS

ALSO detective-voc

INSTALL detective

PREVIOUS
