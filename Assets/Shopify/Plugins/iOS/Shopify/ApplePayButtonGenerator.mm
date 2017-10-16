#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <PassKit/PassKit.h>
#import "UnityAppController.h"

#ifdef __cplusplus
extern "C" {
#endif
    PKPaymentButtonStyle SBPayButtonStyleFromString(const char *style) {
        if (strcmp(style, "white") == 0) {
            return PKPaymentButtonStyleWhite;
        } else if (strcmp(style, "black") == 0) {
            return PKPaymentButtonStyleBlack;
        } else if (strcmp(style, "whiteOutline") == 0) {
            return PKPaymentButtonStyleWhiteOutline;
        } else {
            return PKPaymentButtonStyleBlack;
        }
    }
    
    PKPaymentButtonType SBPayButtonTypeFromString(const char *type) {
        if (strcmp(type, "plain") == 0) {
            return PKPaymentButtonTypePlain;
        } else if (strcmp(type, "buy") == 0) {
            return PKPaymentButtonTypeBuy;
        } else if (strcmp(type, "setUp") == 0) {
            return PKPaymentButtonTypeSetUp;
        } else if (strcmp(type, "inStore") == 0) {
            return PKPaymentButtonTypeInStore;
        } else if (strcmp(type, "donate") == 0) {
            return PKPaymentButtonTypeDonate;
        } else {
            return PKPaymentButtonTypePlain;
        }
    }
    
    const char* cStringCopy(const char* string) {
        if (string == NULL)
            return NULL;
        
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        
        return res;
    }
    
    const char* _GenerateApplePayButtonImage(const char *type, const char* style, float width, float height) {
        PKPaymentButton* button = [PKPaymentButton buttonWithType:SBPayButtonTypeFromString(type)
                                                            style:SBPayButtonStyleFromString(style)];
        CGSize size = CGSizeMake(width, height);
        button.frame = CGRectMake(0, 0, size.width, size.height);
        
        UnityAppController* unityController = (UnityAppController*)UIApplication.sharedApplication.delegate;
        [unityController.rootView insertSubview:button atIndex:0];
        
        // Take snapshot of the Apple Pay button
        UIGraphicsBeginImageContextWithOptions(size, NO, [UIScreen mainScreen].scale);
        CGContextRef context = UIGraphicsGetCurrentContext();
        [button.layer renderInContext:context];
        UIImage* snapshot = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();
        
        [button removeFromSuperview];
        
        // Convert to bytes and pass to Unity
        NSString *byteArray = [UIImagePNGRepresentation(snapshot) base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
        return cStringCopy([byteArray UTF8String]);
    }
#ifdef __cplusplus
}
#endif

