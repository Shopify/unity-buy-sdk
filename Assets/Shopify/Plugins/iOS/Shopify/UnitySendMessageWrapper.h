#import "UnityAppController.h"

@interface UnitySendMessageWrapper : UnityAppController

+ (void) SendMessage:(NSString *)obj :(NSString*)method :(NSString*)msg;

@end
