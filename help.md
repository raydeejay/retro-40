# Retro-40 Documentation



## HELP  `<word> ( -- )`

Displays documentation for the given word from the `help.md` file.

---

## TIME  `( -- ms )`

Returns the number of milliseconds since 1970-01-01 00:00:00 +0000 (UTC).

---

## P!  `( c x y -- )`

Sets the pixel at X,Y to colour C.

---

## P@  `( x y -- c )`

Gets the colour of the pixel at X,Y.

---

## SPR  `( x y n -- )`

Blits sprite #N at X,Y.

---

## SPR*  `( x y w h n -- )`

Blits a block of WxH sprites, with #N as the top-left sprite, at X,Y.

---

## MAP  `( x y -- )`

Blits 32x24 tiles starting from X,Y to the screen at 0,0.

---

## MAP*  `( x y w h sx sy -- )`

Blits WxH tiles starting from X,Y to the screen at SX,SY.

---

## M!  `( n x y -- )`

Sets the tile at X,Y on the map to sprite #N.

---

## M@  `( x y --)`

Gets the tile at X,Y on the map.

---

## SP!  `( n x y c -- )`

Sets the pixel of sprite #N at X,Y to color C.

---

## SP@  `( n x y )`

Gets the pixel of sprite #N at X,Y.

---

## PAL!  `( r g b i p -- )`

Sets colour #I in palette #P to R,G,B.

---

## PAL@  `( i p -- r g b)`

Gets the RGB values of colour #I in palette #P.

---

## IMPORT-SPRITE  `<filename> ( u -- )`

Loads an image from filename into sprite #U.

---

## DEFINE-SFX  `<filename> ( -- u )`

Loads a sound effect from filename, returning it's handle.

---

## SFX  `( u -- )`

Plays sound effect #U.

---

## CLS  `( c -- )`

Clears the screen using color C.

---

## W  `( -- u )`

Gets the screen width.

---

## H  `( -- u )`

Gets the screen height.

---

## KEYBUFFER  `( -- addr )`

Address of the key buffer.

---

## VRAM  `( -- addr )`

Address of the framebuffer.

---

## SRAM  `( -- addr )`

Address of the sprite memory.

---

## MRAM  `( -- addr )`

Address of the map memory.

---

## FONT  `( -- addr )`

Address of the font memory.

---

## KEYS  `( -- addr )`

Address of the current key state array.

---

## OLDKEYS  `( -- addr )`

Address of the previous key state array.

---

## SX  `( -- addr )`

Address of the X coordinate of the text cursor.

---

## SY  `( -- addr )`

Address of the Y coordinate of the text cursor.

---

## FG  `( -- addr )`

Address of the foreground colour.

---

## BG  `( -- addr )`

Address of the background colour.

---

## MOUSEX  `( -- addr )`

Address of the X coordinate of the mouse pointer.

---

## MOUSEY  `( -- addr )`

Address of the Y coordinate of the mouse pointer.

---

## MOUSEB  `( -- addr )`

Address of the currently pressed mouse buttons, as a bitmask.

---

## COLORKEY  `( -- addr )`

Address of the color to treat as transparent when blitting.

---

## SCALE  `( -- addr )`

Address of the scale to use when blitting.

---

## FLIP  `( -- addr )`

Address of the flipping to use when blitting.

---

## ROTATE  `( -- addr )`

Address of the rotation to use when blitting.
