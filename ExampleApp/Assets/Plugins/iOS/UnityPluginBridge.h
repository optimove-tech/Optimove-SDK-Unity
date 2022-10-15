#ifndef UnityPluginBridge_h
#define UnityPluginBridge_h

void OptimoveSayHello();

void OptimoveReportEvent(const char* type, const char* jsonData);
void OptimoveReportScreenVisit(const char* screenTitle, const char* screenCategory);
void OptimoveRegisterUser(const char* userId, const char* email);
void OptimoveSetUserId(const char* userId);
void OptimoveSetUserEmail(const char* email);
char* OptimoveGetVisitorId();
void OptimoveSignOutUser();

#endif /* UnityPluginBridge_h */


