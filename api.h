#ifndef R40_API_H_
#define R40_API_H_

#include "ficl/ficl.h"

//Screen dimension constants
#define R40_WIDTH 256
#define R40_HEIGHT 192
#define SCREEN_SCALE 3
#define SCREEN_WIDTH R40_WIDTH * SCREEN_SCALE
#define SCREEN_HEIGHT R40_HEIGHT * SCREEN_SCALE
#define SCREEN_FPS 60
#define SCREEN_TICKS_PER_FRAME (1000 / SCREEN_FPS)

// the video ram
extern unsigned char *gVRAM;
extern unsigned char *gSRAM;
extern unsigned char *gMRAM;
extern unsigned char *gMRAM2;
extern unsigned char *gPRAM;
extern unsigned char *gFONT;

extern char *gScriptFilename;
extern Uint8 keybuffer[];

extern long gMouseX;
extern long gMouseY;
extern long gMouseButtons;

extern int outputAvailable;

extern void R40TextOut(ficlCallback *callback, char *text);

extern void R40Time(ficlVm *vm);
extern void R40Pset(ficlVm *vm);
extern void R40Pget(ficlVm *vm);
extern void R40SPset(ficlVm *vm);
extern void R40SPget(ficlVm *vm);
extern void R40Spr(ficlVm *vm);
extern void R40ImportSprite(ficlVm *vm);
extern void R40DefineSfx(ficlVm *vm);
extern void R40Sfx(ficlVm *vm);
extern void R40Cls(ficlVm *vm);
extern void initMachineForth(ficlSystem *system, ficlVm *vm, unsigned char *keys, unsigned char *lastkeys);

#endif // R40_API_H_
