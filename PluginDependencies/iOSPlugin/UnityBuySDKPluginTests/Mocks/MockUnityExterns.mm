//
//  MockUnityExterns.m
//  UnityBuySDKPluginTests
//
//  Created by Stephan Leroux on 2021-02-19.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import <UnityBuySDKPlugin/UnityBuySDKPlugin.h>

// Extern declarations so we can invoke these within our library.
#ifdef __cplusplus
extern "C" {
#endif
    void _RespondToNativeMessage(const char *identifier, const char *response);
#ifdef __cplusplus
}
#endif


static UIViewController *mockRootViewController = [[UIViewController alloc] init];

static NSMutableDictionary<NSString*, NSString*> *mockResponses = [@{} mutableCopy];

extern "C" void MockUnityMessageResponseFor(NSString *method, NSString *response)
{
    mockResponses[method] = response;
}

extern "C" void UnityBuyAppControllerSetShouldResign(bool value)
{

}

extern "C" void UnitySendMessage(const char* obj, const char* method, const char* msg)
{
    NSString *msgString = [NSString stringWithCString:msg encoding:NSUTF8StringEncoding];
    NSData *msgData = [msgString dataUsingEncoding:NSUTF8StringEncoding];
    
    NSError *error = nil;
    NSDictionary *msgJson = [NSJSONSerialization JSONObjectWithData:msgData options:0 error:&error];
    
    NSString *methodInvoked = [NSString stringWithUTF8String:method];
    NSString *response = mockResponses[methodInvoked];
    _RespondToNativeMessage([msgJson[@"Identifier"] cStringUsingEncoding:NSUTF8StringEncoding] , [response cStringUsingEncoding:NSUTF8StringEncoding]);
}

extern "C" UIViewController *UnityGetGLViewController()
{
    return mockRootViewController;
}
