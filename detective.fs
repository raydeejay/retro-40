17 ficl-vocabulary detective-voc
also detective-voc definitions

\ : in-area?  ( x/8 y/8 -- f )    0 24 WITHIN  SWAP  0 32 WITHIN  AND ;

0 DEFINE-SFX jump.wav

: xy>tilexy  ( x y -- x/8 y/8 )  8 /  SWAP  8 /  SWAP ;
: tilexy>xy  ( x/8 y/8 -- x y )  8 *  SWAP  8 *  SWAP ;

: dude?     ( u -- )  8 12 WITHIN ;
: place-dude  ( -- )  8 4 RND +  32 RND 24 RND  M! ;
: reset       ( -- )  s" detective.map" load-map  place-dude ;
: win         ( -- )  0 sfx  reset ;

: check-dude  ( mx my -- f )  xy>tilexy  M@ dude?  IF  win  THEN ;

: update-mouse  ( -- )  MOUSEB @ IF  MOUSEX @  MOUSEY @  check-dude  THEN ;
: ?exit         ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;
: <update>      ( -- )  update-mouse  ?exit ;


\ only blit the map portion under the spotlight
: partial-map  ( mx my -- )
  { mx my | xc yc }
  \ compensate for the mouse coordinates
  mx 16 <   mx 8 MOD NOT   OR  TO xc
  my 16 <   my 8 MOD NOT   OR  TO yc
  mx 16 - 8 /  my 16 - 8 /   5 xc +  5 yc +  2OVER tilexy>xy  map*
;

\ blit the spotlight, using white as the transparent color
: spotlight  ( mx my --)  15 colorkey !   -24 UNDER+ -24 +   6 6  16 spr*   -1 colorkey ! ;

: <draw>    ( -- )  0 cls  MOUSEX @ MOUSEY @ 2DUP  partial-map  spotlight ;


: <init>    ( -- )  s" detective.spr" load-sprites  reset ;

\ install the software
PREVIOUS DEFINITIONS

ALSO detective-voc

INSTALL detective

PREVIOUS
