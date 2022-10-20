#import "InitializePluginManager.h"
#import "Objc-swift-bridging-header.h"

@implementation InitializePluginManager

+ (void) load {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        [NSNotificationCenter.defaultCenter addObserver: self
                                               selector: @selector(didFinishLaunching:)
                                                   name: UIApplicationDidFinishLaunchingNotification
                                                 object: nil];
    });
}

+ (void) didFinishLaunching: (NSNotification*) n {
    [Optimove_Unity didFinishLaunching:n unityVersion: @"Unknown"];
}

@end

