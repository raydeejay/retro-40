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
17 FICL-VOCABULARY myprog-voc
ALSO myprog-voc DEFINITIONS


\ your code goes here

: ?exit     ( -- )  SCANCODE_Q pressed? IF  retro-40  THEN ;


\ these are the hooks that R40 expect to find

: <init>    ( -- )  S" myprog.spr" load-sprites  s" myprog.map" load-map  ( ... ) ;
: <update>  ( -- )  ?exit ( ... ) ;
: <draw>    ( -- )  ( ... )  ;


\ install the software ()
PREVIOUS DEFINITIONS

ALSO myprog-voc

INSTALL myprog

PREVIOUS
```
