/***********************************************************
 * Retro-40
 *
 * A Fictional Computer using Forth as its system language
 **********************************************************/

#include <stdio.h>
#include <string.h>
#include <float.h>
#include <unistd.h>

#include <SDL2/SDL.h>
#include <SDL2/SDL_image.h>
#include <SDL2/SDL_mixer.h>

#include "ficl/ficl.h"
#include "api.h"

// Starts up SDL and creates window
int init();

// Frees media and shuts down SDL
void myclose();

// The window we'll be rendering to
SDL_Window* gWindow = NULL;

// The window renderer
SDL_Renderer* gRenderer = NULL;

// Sprite textures (8 7-sprites, one per image)
#define MAX_SPRITES 256
SDL_Texture *gSprites[MAX_SPRITES] = { NULL };
int gMaxSprite = -1;

// Sound effects, not sure about the limit yet
Mix_Chunk *gSfx[72] = { NULL };
int gMaxSfx = -1;

SDL_Texture *gCanvas = NULL;

/*************************************************
 ** MAIN CODE
 ************************************************/
int init()
{
    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0) {
        printf("SDL could not initialize! SDL Error: %s\n", SDL_GetError());
        return 0;
    }

    /* if (!SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "1")) { */
    /*     printf("Warning: Linear texture filtering not enabled!"); */
    /* } */

    if (!SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "0")) {
        printf("Warning: Linear texture filtering not enabled!");
    }

    gWindow = SDL_CreateWindow("Retro-40",
                                SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                                SCREEN_WIDTH, SCREEN_HEIGHT,
                                SDL_WINDOW_SHOWN);

    if (gWindow == NULL) {
        printf("Window could not be created! SDL Error: %s\n", SDL_GetError());
        return 0;
    }

    gRenderer = SDL_CreateRenderer(gWindow, -1, SDL_RENDERER_ACCELERATED /* | SDL_RENDERER_PRESENTVSYNC */);
    if (gRenderer == NULL) {
        printf("Renderer could not be created! SDL Error: %s\n", SDL_GetError());
        return 0;
    }

    SDL_RenderSetLogicalSize(gRenderer, SCREEN_WIDTH, SCREEN_HEIGHT);

    int imgFlags = IMG_INIT_PNG;
    if (!(IMG_Init(imgFlags) & imgFlags)) {
        printf("SDL_image could not initialize! SDL_image Error: %s\n", IMG_GetError());
        return 0;
    }

    if (Mix_OpenAudio(22050, AUDIO_U8, 2, 2048) < 0) {
        printf("SDL_mixer could not initialize! SDL_mixer Error: %s\n", Mix_GetError());
        return 0;
    }

    // create VRAM and canvas texture
    gVRAM = malloc(R40_WIDTH * R40_HEIGHT);
    memset(gVRAM, 0, R40_WIDTH * R40_HEIGHT);
    gCanvas = SDL_CreateTexture(gRenderer,
                                SDL_PIXELFORMAT_RGBA8888,
                                SDL_TEXTUREACCESS_STREAMING,
                                R40_WIDTH, R40_HEIGHT);

    // sprite ram
    // 2 pages of 32 * 16 8x8 sprites
    gSRAM = malloc(2 * 32*16 * 8*8);
    memset(gSRAM, 0, 2 * 32*16 * 8*8);

    // font memory, 128*8 bytes (cells are apparently 64 bits?)
    gFONT = malloc(128 * 8);
    memset(gFONT, 0, 128*8);

    // map ram
    gMRAM = malloc(R40_WIDTH * R40_HEIGHT);
    memset(gMRAM, 0, R40_WIDTH * R40_HEIGHT);
    
    return 1;
}

void myclose()
{
    SDL_DestroyTexture(gCanvas);
    /* free(gSRAM); */
    /* free(gVRAM); */
    /* free(gFONT); */

    //Free loaded images
    for (int i = 0; gSprites[i]; ++i) {
        if (gSprites[i]) {
            SDL_DestroyTexture(gSprites[i]);
            gSprites[i] = NULL;
        }
    }

    for (int i = 0; gSfx[i]; ++i) {
        if (gSfx[i]) {
            Mix_FreeChunk(gSfx[i]);
            gSfx[i] = NULL;
        }
    }

    //Destroy window
    SDL_DestroyRenderer(gRenderer);
    SDL_DestroyWindow(gWindow);
    gWindow = NULL;
    gRenderer = NULL;

    //Quit SDL subsystems
    Mix_Quit();
    IMG_Quit();
    SDL_Quit();
}


int main(int argc, char* argv[]) {
    ficlSystem *system = ficlSystemCreate(NULL);
    ficlVm *vm = ficlSystemCreateVm(system);
    ficlDictionary *dictionary = ficlSystemGetDictionary(system);

    //Start up SDL and create window
    if (!init())
    {
        printf("Failed to initialize SDL!\n");
        exit(-1);
    }

    // bail out if Ficl fails to initialise
    if (!vm) {
        printf("out of memory\n");
        return EXIT_FAILURE;
    }

    //Main loop flag
    int quit = 0;

    //Event handler
    SDL_Event e;
    const Uint8 *keys = SDL_GetKeyboardState(NULL);
    Uint8 lastkeys[284+1] = { 0 } ;

    // use game.lsp as default filename
    gScriptFilename = argc < 2 ? "boot.fs" : argv[1];

    // bail out if the file can't be loaded
    if (!access(gScriptFilename, R_OK) == 0) {
        printf("Cannot load %s\n", gScriptFilename);
        return 1;
    }

    // init the machine's Forth
    initMachineForth(system, vm, keys, lastkeys);

    // enable output to the SDL terminal
    outputAvailable = 1;
    
    // maybe cache update's XT ?
    /* ficlString updatefs; */
    /* FICL_STRING_SET_FROM_CSTRING(updatefs, "update"); */
    /* int updateXT = ficlDictionaryLookup(dictionary, updatefs); */

    //Start counting frames per second
    int countedFrames = 0;

    //While application is running
    while (!quit) {
        int startFrame = SDL_GetTicks();
        int draw_gfx = 1;

        // pump the events
        memset(&keybuffer, 0, 1024);
        Uint8 *k = keybuffer;

        while (SDL_PollEvent(&e) != 0) {
            if (e.type == SDL_QUIT) {
                quit = 1;
            }
            else if (e.type == SDL_KEYDOWN) {
                switch (e.key.keysym.sym) {
                case SDLK_ESCAPE:
                    quit = 1;
                    break;
                    case SDLK_F5:
                        if (e.key.keysym.mod & KMOD_CTRL) {
                            ficlSystemDestroyVm(vm);
                            ficlSystemDestroy(system);

                            system = ficlSystemCreate(NULL);
                            vm = ficlSystemCreateVm(system);
                            dictionary = ficlSystemGetDictionary(system);

                            initMachineForth(system, vm, keys, lastkeys);
                        }
                        break;
                default:
                    if (e.key.keysym.sym <= 127) {
                        *k = e.key.keysym.sym;

                        /* some SDL keys are weird */
                        if ( *k == '\\') { *k = '\''; }  /* will disable backslash... */

                        if (e.key.keysym.mod & KMOD_SHIFT) {
                            if ( *k >= 'a' && *k <= 'z') { *k = toupper(*k); }

                            if ( *k == '1') { *k = '!'; }
                            if ( *k == '2') { *k = '@'; }
                            if ( *k == '3') { *k = '#'; }
                            if ( *k == '4') { *k = '$'; }
                            if ( *k == '5') { *k = '%'; }
                            if ( *k == '6') { *k = '^'; }
                            if ( *k == '7') { *k = '&'; }
                            if ( *k == '8') { *k = '*'; }
                            if ( *k == '9') { *k = '{'; }
                            if ( *k == '0') { *k = '}'; }

                            if ( *k == '/') { *k = '?'; }
                            if ( *k == '=') { *k = '+'; }
                            if ( *k == '-') { *k = '_'; }
                            if ( *k == ',') { *k = '<'; }
                            if ( *k == '.') { *k = '>'; }
                            if ( *k == ';') { *k = ':'; }
                            if ( *k == '\'') { *k = '"'; }
                        }
                        if (e.key.keysym.mod & KMOD_CTRL) {
                            if ( *k >= 'a' && *k <= 'z') { *k -= 96; }
                            if ( *k >= 'A' && *k <= 'Z') { *k -= 64; }
                        }
                        ++k;
                    }
                    
                    break;
                }
            }
        }

        // get mouse
        int x, y;
        gMouseButtons = SDL_GetMouseState(&x, &y);

        // scale the physical coordinates to logical coordinates
        gMouseX = x * R40_WIDTH / SCREEN_WIDTH;
        gMouseY = y * R40_HEIGHT / SCREEN_HEIGHT;

        gMouseX = x / SCREEN_SCALE;
        gMouseY = y / SCREEN_SCALE;
        
        //Clear screen
        //SDL_SetRenderDrawColor(gRenderer, 0, 0, 0, 255);
        //SDL_RenderClear(gRenderer);

        // run forth hook
        /* ficlVmExecuteXT(vm, updateXT); */
        ficlVmEvaluate(vm, "update");

        // store the keys pressed this cycle as the old keys for the next cycle
        memcpy(&lastkeys, keys, 284);

        if (draw_gfx) {
            // copy VRAM to texture
            int palette[16][3] = {
                {0x14, 0x0C, 0x1C}, /* Black */
                {0x44, 0x24, 0x34}, /* Dark Red  */
                {0x30, 0x34, 0x6D}, /* Dark Blue  */
                {0x4E, 0x4A, 0x4F}, /* Dark Gray  */
                {0x85, 0x4C, 0x30}, /* Brown  */
                {0x34, 0x65, 0x24}, /* Dark Green  */
                {0xD0, 0x46, 0x48}, /* Red  */
                {0x75, 0x71, 0x61}, /* Light Gray  */
                {0x59, 0x7D, 0xCE}, /* Light Blue  */
                {0xD2, 0x7D, 0x2C}, /* Orange  */
                {0x85, 0x95, 0xA1}, /* Blue/Gray  */
                {0x6D, 0xAA, 0x2C}, /* Light Green  */
                {0xD2, 0xAA, 0x99}, /* Peach  */
                {0x6D, 0xC2, 0xCA}, /* Cyan  */
                {0xDA, 0xD4, 0x5E}, /* Yellow  */
                {0xDE, 0xEE, 0xD6}  /* White  */
            };

            Uint8* pixels = NULL;
            Uint32 *px;
            int pitch = 0;

            SDL_LockTexture(gCanvas, NULL, (void**)&px, &pitch);
            for (int y = 0; y < R40_HEIGHT; ++y) {
                // pixels points to the beginning of the current line
                pixels = (Uint8*) px + (y * pitch);

                for (int x = 0; x < R40_WIDTH; ++x) {
                    int c = gVRAM[y * R40_WIDTH + x];
                    // the format of the texture is ABGR
                    pixels[(x * 4)]     = 0;
                    pixels[(x * 4) + 1] = palette[c][2];
                    pixels[(x * 4) + 2] = palette[c][1];
                    pixels[(x * 4) + 3] = palette[c][0];
                }
            }
            SDL_UnlockTexture(gCanvas);

            // render texture
            SDL_RenderCopy(gRenderer, gCanvas, NULL, NULL);
        }

        // render renderer
        SDL_RenderPresent(gRenderer);
        ++countedFrames;

        //If frame finished early
        int frameTicks = SDL_GetTicks() - startFrame;
        if (frameTicks < SCREEN_TICKS_PER_FRAME) {
            //Wait remaining time
            SDL_Delay(SCREEN_TICKS_PER_FRAME - frameTicks);
        }
        /* else { */
        /*     printf("Stalling!!!\n"); */
        /* } */
    }

    //Free resources and close SDL
    myclose();
    free(vm);

    return 0;
}
