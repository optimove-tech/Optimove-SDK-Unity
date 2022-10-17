#ifndef UnityPluginBridge_h
#define UnityPluginBridge_h

void OptimoveReportEvent(const char* type, const char* jsonData);
void OptimoveReportScreenVisit(const char* screenTitle, const char* screenCategory);
void OptimoveRegisterUser(const char* userId, const char* email);
void OptimoveSetUserId(const char* userId);
void OptimoveSetUserEmail(const char* email);
char* OptimoveGetVisitorId();
void OptimoveSignOutUser();
void OptimoveUpdatePushRegistration(int status);
void OptimoveInAppUpdateConsentForUser(int consented);


char* OptimoveInAppGetInboxItems();
char* OptimoveInAppPresentInboxMessage(int messageId);
BOOL OptimoveInAppDeleteMessageFromInbox(int messageId);
BOOL OptimoveInAppMarkAsRead(int messageId);
void OptimoveInAppGetInboxSummary(const char* guid);


#endif /* UnityPluginBridge_h */


