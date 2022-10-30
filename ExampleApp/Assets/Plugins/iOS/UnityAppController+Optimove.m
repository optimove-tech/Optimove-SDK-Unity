#import <objc/runtime.h>
#import "UnityAppController+Optimove.h"
#import "Objc-swift-bridging-header.h"

@implementation UnityAppController (Optimove)

typedef void (^RestorationHandler)(NSArray<id<UIUserActivityRestoring>>*);
static IMP existingContinueUserActivityDelegate = NULL;

BOOL optimove_continueUserActivity(
        id self,
        SEL _cmd,
        UIApplication* application,
        NSUserActivity* userActivity,
        RestorationHandler restorationHandler) {

    BOOL result = YES;
    if (existingContinueUserActivityDelegate) {
        result = ((BOOL(*)(id,SEL,UIApplication*, NSUserActivity*, RestorationHandler))existingContinueUserActivityDelegate)(self, _cmd, application, userActivity, restorationHandler);
    }

    [Optimove_Unity application:application userActivity:userActivity restorationHandler:restorationHandler];

    return result;
}



+ (void) load {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        [NSNotificationCenter.defaultCenter addObserver: self
                                               selector: @selector(didFinishLaunching:)
                                                   name: UIApplicationDidFinishLaunchingNotification
                                                 object: nil];

        Class class = self.class;
        SEL sel = @selector(application:continueUserActivity:restorationHandler:);
        const char *launchTypes = method_getTypeEncoding(class_getInstanceMethod(class, sel));
        existingContinueUserActivityDelegate = class_replaceMethod(class, sel, (IMP) optimove_continueUserActivity, launchTypes);
    });
}

+ (void) didFinishLaunching: (NSNotification*) n {
    [Optimove_Unity didFinishLaunching:n];
}

@end
