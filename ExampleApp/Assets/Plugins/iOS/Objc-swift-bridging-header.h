@interface Optimove_Unity

typedef void (^InboxSummaryResultHandler)(NSDictionary* _Nonnull);


+ (void)didFinishLaunching:(NSNotification * _Nonnull)notification;

+ (void)reportEvent:(NSString * _Nonnull)type parameters:(NSDictionary * _Nullable) parameters;
+ (void)reportScreenVisit:(NSString * _Nonnull)screenTitle screenCategory:(NSString * _Nullable) screenCategory;

+ (void)registerUser:(NSString * _Nonnull)userId email:(NSString * _Nonnull) email;
+ (void)setUserId:(NSString * _Nonnull)userId;
+ (void)setUserEmail:(NSString * _Nonnull)email;
+ (NSString * _Nullable)getVisitorId;
+ (void)signOutUser;

+ (void)pushRegister;
+ (void)pushUnregister;
+ (void)inAppUpdateConsent:(BOOL)consented;
+ (NSMutableArray<NSDictionary*>* _Nonnull)inAppGetInboxItems;
+ (NSString * _Nonnull)inAppPresentInboxMessage:(int)messageId;
+ (BOOL)inAppDeleteMessageFromInbox:(int)messageId;
+ (BOOL)inAppMarkAsRead:(int)messageId;
+ (BOOL)inAppMarkAllInboxItemsAsRead;
+ (void)inAppGetInboxSummary:(NSString * _Nonnull)guid handler:(InboxSummaryResultHandler _Nonnull)handler;

@end