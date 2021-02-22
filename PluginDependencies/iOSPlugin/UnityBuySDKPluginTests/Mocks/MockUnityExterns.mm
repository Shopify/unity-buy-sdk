//
//  MockUnityExterns.m
//  UnityBuySDKPluginTests
//
//  Created by Stephan Leroux on 2021-02-19.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

static UIViewController *mockRootViewController = [[UIViewController alloc] init];

extern "C" void UnityBuyAppControllerSetShouldResign(bool value)
{

}

extern "C" void UnitySendMessage(const char* obj, const char* method, const char* msg)
{
//    _RespondToNativeMessage(const char *identifier, const char *response) {
}

extern "C" UIViewController *UnityGetGLViewController()
{
    return mockRootViewController;
}
