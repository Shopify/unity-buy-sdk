#import "UnityInterface.h"

@implementation UnitySendMessageWrapper

+ (void) SendMessage:(NSString *)obj to:(NSString*) method and:(NSString*) msg {
    UnitySendMessage([obj UTF8String], [method UTF8String], [msg UTF8String]);
}

@end
