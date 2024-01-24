/* 
* chromatron_mac.h
*/

#ifndef __CHROMATRON_MAC_H__
#define __CHROMATRON_MAC_H__

#include <stddef.h>
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

#define _DLL_EXPORT __attribute__((visibility("default"))) extern

typedef void (*OnRunMessageLoopCallback)();
typedef void (*OnCefShutdownCallback)();
typedef void (*OnInitCallback)(void *app, void *pool);
typedef void (*OnCreateCallback)(void *window, void *view);
typedef void (*OnMovingCallback)();
typedef void (*OnResizeCallback)(int width, int height);
typedef int  (*OnCloseBrowserCallback)();
typedef void (*OnExitCallback)();

typedef struct APPDATA APPDATA;
typedef struct CHROMATRONPARAM CHROMATRONPARAM;

struct APPDATA {
    void *app; 
    void *pool;
};

struct CHROMATRONPARAM {
    int x;
    int y;
    int width;
    int height;
    int centerscreen;
    int frameless;
    int fullscreen;
    int noresize;
    int nominbutton;
    int nomaxbutton;
    char* titleUtf8Ptr;
    OnRunMessageLoopCallback runMessageLoopCallback;
    OnCefShutdownCallback cefShutdownCallback;
    OnInitCallback initCallback;
    OnCreateCallback createCallback;
    OnMovingCallback movingCallback;
    OnResizeCallback resizeCallback;
    OnCloseBrowserCallback closeBrowserCallback;
    OnExitCallback exitCallback;
};

/*
* Exported methods
*/

_DLL_EXPORT void createwindow(CHROMATRONPARAM* pParam);
_DLL_EXPORT APPDATA createwindowdata(CHROMATRONPARAM* pParam);
_DLL_EXPORT void run(void* application);
_DLL_EXPORT void quit(void* application, void* pool);
_DLL_EXPORT void minimize(void* view);
_DLL_EXPORT void maximize(void* view);

#ifdef __cplusplus
}
#endif

#endif




