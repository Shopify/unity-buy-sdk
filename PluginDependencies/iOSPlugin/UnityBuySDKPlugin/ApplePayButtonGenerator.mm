#import <UIKit/UIKit.h>
#import <PassKit/PassKit.h>
#import "UnityBuySDKPlugin.h"

#ifdef __cplusplus
extern "C" {
#endif
    extern UIView           *UnityGetGLView();

    // Types
    static char plainType[] = "PLAIN";
    static char buyType[] = "BUY";
    static char setupType[] = "SETUP";
    static char inStoreType[] = "IN_STORE";
    static char donateType[] = "DONATE";
    
    // Styles
    static char whiteStyle[] = "WHITE";
    static char blackStyle[] = "BLACK";
    static char whiteOutlineStyle[] = "WHITE_OUTLINE";
    
    PKPaymentButtonStyle SBPayButtonStyleFromString(const char *style) {
        if (strncmp(style, whiteStyle, sizeof(whiteStyle)) == 0) {
            return PKPaymentButtonStyleWhite;
        } else if (strncmp(style, blackStyle, sizeof(blackStyle)) == 0) {
            return PKPaymentButtonStyleBlack;
        } else if (strncmp(style, whiteOutlineStyle, sizeof(whiteOutlineStyle)) == 0) {
            return PKPaymentButtonStyleWhiteOutline;
        } else {
            return PKPaymentButtonStyleBlack;
        }
    }
    
    PKPaymentButtonType SBPayButtonTypeFromString(const char *type) {
        if (strncmp(type, plainType, sizeof(plainType)) == 0) {
            return PKPaymentButtonTypePlain;
        } else if (strncmp(type, buyType, sizeof(buyType)) == 0) {
            return PKPaymentButtonTypeBuy;
        } else if (strncmp(type, setupType, sizeof(setupType)) == 0) {
            return PKPaymentButtonTypeSetUp;
        } else if (strncmp(type, inStoreType, sizeof(inStoreType)) == 0) {
            return PKPaymentButtonTypeInStore;
        } else if (strncmp(type, donateType, sizeof(donateType)) == 0) {
            if (@available(iOS 10.2, *)) {
                return PKPaymentButtonTypeDonate;
            } else {
                return PKPaymentButtonTypePlain;
            }
        } else {
            return PKPaymentButtonTypePlain;
        }
    }
    
    // Helper method for making a copy of a C String.
    const char* cStringCopy(const char* string, size_t length) {
        char* res = (char*)malloc((length + 1) * sizeof(char));
        strncpy(res, string, length + 1);
        return res;
    }
    
    // Generates a base64 encoded string containing the rendered image output of a PKPaymentButton from iOS.
    const char* _GenerateApplePayButtonImage(const char *type, const char* style, float width, 
                                             float height) {
        PKPaymentButton* button = [PKPaymentButton buttonWithType:SBPayButtonTypeFromString(type)
                                                            style:SBPayButtonStyleFromString(style)];
        CGSize size = CGSizeMake(width, height);
        button.frame = CGRectMake(0, 0, size.width, size.height);
        
        [UnityGetGLView() insertSubview:button atIndex:0];
        
        // Take snapshot of the Apple Pay button.
        UIGraphicsBeginImageContextWithOptions(size, NO, [UIScreen mainScreen].scale);
        CGContextRef context = UIGraphicsGetCurrentContext();
        [button.layer renderInContext:context];
        UIImage* snapshot = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();
        
        [button removeFromSuperview];
        
        // Convert to bytes and pass to Unity.
        NSString *byteArray = [UIImagePNGRepresentation(snapshot) base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
        
        // We create a malloc'ed copy of the string here so we can manually release the memory associated
        // with it on the Unity side using Marshal.FreeHGlobal.
        const char *utf8String = [byteArray UTF8String];
        if (utf8String == NULL) {
            return NULL;
        }
        
        return cStringCopy(utf8String, strlen(utf8String));
    }
#ifdef __cplusplus
}
#endif

