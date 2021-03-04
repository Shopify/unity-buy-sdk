//
//  UnityBuySDKPlugin.h
//  UnityBuySDKPlugin
//
//  Created by Stephan Leroux on 2021-02-22.
//

#import <Foundation/Foundation.h>

@class UIViewController;

//! Project version number for UnityBuySDKPlugin.
FOUNDATION_EXPORT double UnityBuySDKPluginVersionNumber;

//! Project version string for UnityBuySDKPlugin.
FOUNDATION_EXPORT const unsigned char UnityBuySDKPluginVersionString[];

#import <UnityBuySDKPlugin/Cart+ApplePay.h>
#import <UnityBuySDKPlugin/Cart+WebCheckout.h>
#import <UnityBuySDKPlugin/Tester.h>
#import <UnityBuySDKPlugin/ApplePayButtonGenerator.h>

// Extern declarations so we can invoke these within our library.
#ifdef __cplusplus
extern "C" {
#endif
    UIViewController *UnityGetGLViewController();
    void UnityBuyAppControllerSetShouldResign(bool value);
    void UnitySendMessage(const char *, const char *, const char *);
#ifdef __cplusplus
}
#endif
