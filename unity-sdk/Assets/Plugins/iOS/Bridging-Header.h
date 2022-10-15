

//copied from ProductModuleName-Swift.h
@interface Optimove_Unity
+ (void)didFinishLaunching:(NSNotification * _Nonnull)notification unityVersion:String;

+ (void)reportEvent:(NSString * _Nonnull)type parameters:(NSDictionary * _Nullable) parameters;
+ (void)reportScreenVisit:(NSString * _Nonnull)screenTitle screenCategory:(NSString * _Nullable) screenCategory;

+ (void)registerUser:(NSString * _Nonnull)userId email:(NSString * _Nonnull) email;
+ (void)setUserId:(NSString * _Nonnull)userId;
+ (void)setUserEmail:(NSString * _Nonnull)email;
+ (NSString * _Nullable)getVisitorId;
+ (void)signOutUser;
@end

