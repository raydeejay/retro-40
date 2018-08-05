/**************************************
 ** HOOKS
 *************************************/

#include <sys/time.h>

#include "retro40.h"
#include "api.h"
#include "ficl/ficl.h"

// hardware and system variables
unsigned char *gVRAM = NULL;
unsigned char *gSRAM = NULL;
unsigned char *gMRAM = NULL;
unsigned char *gPRAM = NULL;
unsigned char *gFONT = NULL;

Uint8 keybuffer[1025];
long  gMouseX = 0;
long  gMouseY = 0;
long  gMouseButtons = 0;

// ugh...
long sx = 0;
long sy = 0;
long fg = 15;
long bg = 0;

// output
int outputAvailable = 0;

static void doScroll() {
    memmove(gVRAM,
            gVRAM + R40_WIDTH * 6,
            R40_WIDTH * (R40_HEIGHT - 6));
    memset(gVRAM + R40_WIDTH * (R40_HEIGHT - 6), bg, R40_WIDTH * 6);
}

static void maybeScroll() {
    if (sy >= R40_HEIGHT / 6) {
        doScroll();
        --sy;
    }
}

static void moveCursor() {
    ++sx;
    if (sx >= R40_WIDTH / 4) {
        sx = 0;
        ++sy;
    }
    maybeScroll();
}

static void blitChar(char c) {
    Uint32 data = (gFONT[c*8+2] << 16) | (gFONT[c*8+1] << 8) | gFONT[c*8];

    for (int j = 5; j >= 0; j--) {
        for (int i = 3; i >= 0; i--) {
            gVRAM[((sy*6+j) * R40_WIDTH) + (sx*4)+i] = ((data & 1) ? fg : bg) ;
            data = data >> 1;
        }
    }
}

void R40TextOut(ficlCallback *callback, char *text) {
    char *k = text;
    while (*k) {
        if (*k == 10 || *k == 13) {
            sx = 0;
            ++sy;
        }
        else if (*k < 128) {
            blitChar(*k);
            moveCursor();
        }
        ++k;
    }
}

void R40Time(ficlVm *vm) {
    struct timeval tv;
    gettimeofday(&tv, NULL);
    ficlStackPushInteger(vm->dataStack, tv.tv_sec * 1000 + tv.tv_usec / 1000);
    return;
}

void R40Pset(ficlVm *vm) {
    int y = ficlStackPopInteger(vm->dataStack);
    int x = ficlStackPopInteger(vm->dataStack);
    int c = ficlStackPopInteger(vm->dataStack);

    if (x >= 0 && x < R40_WIDTH && y >= 0 && y < R40_HEIGHT) {
        gVRAM[y * R40_WIDTH + x] = c;
    }

    return;
}

void R40Pget(ficlVm *vm) {
    int y = ficlStackPopInteger(vm->dataStack);
    int x = ficlStackPopInteger(vm->dataStack);

    if (x >= 0 && x < R40_WIDTH && y >= 0 && y < R40_HEIGHT) {
        ficlStackPushInteger(vm->dataStack, gVRAM[y * R40_WIDTH + x]);
    } else {
        ficlStackPushInteger(vm->dataStack, 0);
    }

    return;
}

void R40Mset(ficlVm *vm) {
    int y = ficlStackPopInteger(vm->dataStack);
    int x = ficlStackPopInteger(vm->dataStack);
    Uint8 c = ficlStackPopInteger(vm->dataStack);

    if (x >= 0 && x < R40_WIDTH && y >= 0 && y < R40_HEIGHT) {
        gMRAM[y * R40_WIDTH + x] = (Uint8) c;
    }

    return;
}

void R40Mget(ficlVm *vm) {
    int y = ficlStackPopInteger(vm->dataStack);
    int x = ficlStackPopInteger(vm->dataStack);

    if (x >= 0 && x < R40_WIDTH && y >= 0 && y < R40_HEIGHT) {
        ficlStackPushInteger(vm->dataStack, gMRAM[y * R40_WIDTH + x]);
    } else {
        ficlStackPushInteger(vm->dataStack, 0);
    }

    return;
}

void R40SPset(ficlVm *vm) {
    // arrange sprites in a row of 8x8-sprites
    int n = (int) ficlStackPopInteger(vm->dataStack);
    int y = (int) ficlStackPopInteger(vm->dataStack);
    int x = (int) ficlStackPopInteger(vm->dataStack);
    int c = (int) ficlStackPopInteger(vm->dataStack);

    int pos = n*8*8 + y*8 + x;

    if (x >= 0 && x < 8 && y >= 0 && y < 8) {
        gSRAM[pos] = c;
    }
    return;
}

void R40SPget(ficlVm *vm) {
    // arrange sprites in a row of 8x8-sprites
    int n = (int) ficlStackPopInteger(vm->dataStack);
    int y = (int) ficlStackPopInteger(vm->dataStack);
    int x = (int) ficlStackPopInteger(vm->dataStack);

    int pos = n*8*8 + y*8 + x;

    if (x >= 0 && x < 8 && y >= 0 && y < 8) {
        ficlStackPushInteger(vm->dataStack, gSRAM[pos]);
    } else {
        ficlStackPushInteger(vm->dataStack, 0);
    }
    return;
}

static void blitSprite(int n, int x, int y) {
    for (int j = 0; j < 8; ++j) {
        for (int i = 0; i < 8; ++i) {
            int pixel = gSRAM[n*8*8 + j*8 + i];
            if (pixel
                && x+i > -1 && x+i < R40_WIDTH
                && y+j > -1 && y+j < R40_HEIGHT) {
                gVRAM[(y+j)*R40_WIDTH + x+i] = pixel;
            }
        }
    }

    return;
}

void R40Spr(ficlVm *vm) {
    int n = (int) ficlStackPopInteger(vm->dataStack);
    int y = (int) ficlStackPopInteger(vm->dataStack);
    int x = (int) ficlStackPopInteger(vm->dataStack);

    blitSprite(n, x, y);
    return;
}

void R40Map(ficlVm *vm) {
    for (int j = 0; j < 24; ++j) {
        for (int i = 0; i < 32; ++i) {
            int tile = gMRAM[j*R40_WIDTH + i];
            blitSprite(tile, i * 8, j * 8);
        }
    }

    return;
}

// needs indexed PNGs
void R40ImportSprite(ficlVm *vm) {
    int n = (int) ficlStackPopInteger(vm->dataStack);
    ficlString name = ficlVmGetWord(vm);
    int success = 1;

    char* id = strndup(name.text, name.length);
    printf("file '%s'\n", name.text);
    
    SDL_Surface *surf = IMG_Load(id);
    if (surf == NULL) {
        printf("Error creating surface to import sprite %s\n", id);
        success = 0;
    }
    /* SDL_Surface *surfi = SDL_ConvertSurfaceFormat(surf, SDL_PIXELFORMAT_RGB332, 0); */

    /* if (surfi == NULL) { */
    /*     printf("Error creating surface to import sprite %s\n", id); */
    /*     success = 0; */
    /* } */

    for (int y = 0; y < 8; ++y) {
        for (int x = 0; x < 8; ++x) {
            int pos = n*8*8 + y*8 + x;
            gSRAM[pos] = ((unsigned char*) surf->pixels)[y*8+x];
            printf("%d\n", ((unsigned char*) surf->pixels)[y*8+x]);
        }
    }

    /* SDL_FreeSurface(surfi); */
    SDL_FreeSurface(surf);
    free(id);

    ficlStackPushInteger(vm->dataStack, success ? gMaxSprite : -1);
    return;
}

void R40DefineSfx(ficlVm *vm) {
    ficlString name = ficlVmGetWord(vm);
    char* id = strndup(name.text, name.length);
    int success = 1;

    ++gMaxSfx;
    printf("loading SFX %s\n", id);

    gSfx[gMaxSfx] = Mix_LoadWAV(id);
    if (gSfx[gMaxSfx] == NULL) {
        printf("SDL_mixer Error: %s\n", Mix_GetError());
        success = 0;
    }

    free(id);

    ficlStackPushInteger(vm->dataStack, success ? gMaxSfx : -1);

    return;
}

void R40Sfx(ficlVm *vm) {
    int i = ficlStackPopInteger(vm->dataStack);
    Mix_PlayChannel(-1, gSfx[i], 0);
    return;
}


void R40Cls(ficlVm *vm) {
    int c = ficlStackPopInteger(vm->dataStack);
    memset(gVRAM, c, R40_WIDTH * R40_HEIGHT);
    return;
}


typedef void (*R40_func)(ficlVm *vm);

struct { const char *name; R40_func fn; const char *doc; } R40_prims[] = {
    { "time",          R40Time,         "Returns the time in milliseconds (TBI from the start of the FC?)" },
    { "spr",           R40Spr,          "(spr id x y [w] [h]) Blits sprite id at x,y with size w,h" },
    { "map",           R40Map,          "(map id x y [w] [h]) Blits the map to the screen" },
    { "p!",            R40Pset,         "(pix x y c) Sets the pixel at x,y to color c" },
    { "p@",            R40Pget,         "(pix x y) Gets the pixel at x,y" },
    { "m!",            R40Mset,         "(mapx x y c) Sets the tile at x,y to sprite c" },
    { "m@",            R40Mget,         "(map x y) Gets the tile at x,y" },
    { "sp!",           R40SPset,        "(spix n x y c) Sets the pixel of sprite n at x,y to color c" },
    { "sp@",           R40SPget,        "(spix n x y) Gets the pixel of sprite n at x,y" },
    { "import-sprite", R40ImportSprite, "(import-sprite filename) Loads an image from filename into the sprite memory" },
    { "define-sfx",    R40DefineSfx,    "(define-sfx filename) Loads a sound effect from filename, returning it's handle" },
    { "sfx",           R40Sfx,          "(sfx id) Plays the sound efect with number id" },
    { "cls",           R40Cls,          "(cls n) Clears the screen using color n" },
    { NULL, NULL, NULL }
};

// the script
char *gScriptFilename;

char *load_file(const char *filename) {
    char *res;
    int r, size;
    FILE *fp = fopen(filename, "rb");
    if (!fp) printf("Could not open file %s\n", filename);

    /* Get size */
    fseek(fp, 0, SEEK_END);
    size = ftell(fp);
    fseek(fp, 0, SEEK_SET);

    /* Load file into string value */
    res = malloc(size+1);
    res[size] = '\0';

    r = fread(res, 1, size, fp);
    fclose(fp);

    if (r != size) printf("Could not read file %s\n", filename);

    return res;
}


// additions to Ficl
void ficlPrimitiveFloor(ficlVm *vm) {
    float f = ficlStackPopFloat(vm->floatStack);
    ficlStackPushInteger(vm->dataStack, floor(f));
    return;
}

void ficlPrimitiveIntegerToFloat(ficlVm *vm) {
    float f = ficlStackPopInteger(vm->dataStack);
    ficlStackPushFloat(vm->floatStack, f);
    return;
}


void initMachineForth(ficlSystem *system, ficlVm *vm, unsigned char *keys, unsigned char *lastkeys) {
    ficlDictionary *dictionary = ficlSystemGetDictionary(system);

    // additions to Ficl
    ficlDictionarySetPrimitive(dictionary, "FLOOR",  ficlPrimitiveFloor,  FICL_WORD_DEFAULT);
    ficlDictionarySetPrimitive(dictionary, "F>S",  ficlPrimitiveFloor,  FICL_WORD_DEFAULT);
    ficlDictionarySetPrimitive(dictionary, "S>F",  ficlPrimitiveIntegerToFloat,  FICL_WORD_DEFAULT);
    
    // system constants
    ficlDictionarySetConstant(dictionary, "W",  R40_WIDTH);
    ficlDictionarySetConstant(dictionary, "H",  R40_HEIGHT);

    // system areas
    ficlDictionarySetConstantPointer(dictionary, "KEYBUFFER", keybuffer);
    ficlDictionarySetConstantPointer(dictionary, "VRAM",      gVRAM);
    ficlDictionarySetConstantPointer(dictionary, "SRAM",      gSRAM);
    ficlDictionarySetConstantPointer(dictionary, "MRAM",      gMRAM);
    ficlDictionarySetConstantPointer(dictionary, "FONT",      gFONT);
    ficlDictionarySetConstantPointer(dictionary, "KEYS",      keys);
    ficlDictionarySetConstantPointer(dictionary, "OLDKEYS",   lastkeys);

    // system variables
    ficlDictionarySetConstantPointer(dictionary, "SX",        &sx);
    ficlDictionarySetConstantPointer(dictionary, "SY",        &sy);
    ficlDictionarySetConstantPointer(dictionary, "FG",        &fg);
    ficlDictionarySetConstantPointer(dictionary, "BG",        &bg);
    ficlDictionarySetConstantPointer(dictionary, "MOUSEX",    &gMouseX);
    ficlDictionarySetConstantPointer(dictionary, "MOUSEY",    &gMouseY);
    ficlDictionarySetConstantPointer(dictionary, "MOUSEB",    &gMouseButtons);

    // should probably override BYE or provide another means of exiting

    // primitives
    for (int i = 0; R40_prims[i].name; ++i) {
        // what about documentation...
        ficlDictionarySetPrimitive(dictionary, (char *) R40_prims[i].name, R40_prims[i].fn, FICL_WORD_DEFAULT);
    }

    // load the core library or the given file
    char *gScript = load_file(gScriptFilename);

    ficlString s;
    FICL_STRING_SET_POINTER(s, gScript);
    FICL_STRING_SET_LENGTH(s, strlen(gScript));
    ficlVmExecuteString(vm, s);

    free(gScript);

    // configure to have output to the SDL terminal
    system->callback.textOut = R40TextOut;
}
