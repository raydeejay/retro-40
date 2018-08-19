# retro-40

A Fantasy Computer using Forth as its system language.

## About

## Inspiration
    Lambda/8
    Jupiter ACE

## Specs
    256x192 screen
    16-colour palettes

## Usage
    Forth is both the programming and the command language for
    Retro-40. You can type commands or even new definitions on the
    console.


## Storage
    TBD

## Included software
    Sprite editor
    Snake

## The language
    Retro-40 software is written in Ficl, the Forth-Inspired Command Language.


## Writing a program
    Vocabularies
    <init>
    <update>
    <draw>
    INSTALL

## Blitting variables

## API
    P!
    P@
    SP!
    SP@
    M!
    M@
    SPR
    MAP
    pressed?
    just-pressed?
    was-pressed?
    MOUSEX
    MOUSEY
    MOUSEB

## Skeleton program
```forth
17 FICL-VOCABULARY detective-voc
ALSO detective-voc DEFINITIONS


: ?exit     ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;


: in-area?  ( x/8 y/8 -- f )    0 24 WITHIN  SWAP  0 32 WITHIN  AND ;
: tile>xy   ( x/8 y/8 -- x y )  8 *  SWAP  8 *  SWAP ;

\ only blit the map portion under the spotlight
: partial-map  ( mx my -- )
  { mx my | xc yc }
  \ compensate for the mouse coordinates
  mx 16 <   mx 8 MOD NOT   OR  TO xc
  my 16 <   my 8 MOD NOT   OR  TO yc
  mx 16 - 8 /  my 16 - 8 /   5 xc +  5 yc +  2OVER tile>xy  map*
;

\ blit the spotlight, using white as the transparent color
: spotlight  ( mx my --)  15 colorkey !   -24 UNDER+ -24 +   6 6  16 spr*   -1 colorkey ! ;


: <init>    ( -- )  S" detective.spr" load-sprites  s" detective.map" load-map ;
: <update>  ( -- )  ?exit ;
: <draw>    ( -- )  0 cls  MOUSEX @ MOUSEY @ 2DUP  partial-map  spotlight ;


\ install the software
PREVIOUS DEFINITIONS

ALSO detective-voc

INSTALL detective

PREVIOUS
```
