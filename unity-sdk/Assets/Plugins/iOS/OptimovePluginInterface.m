#import <Foundation/Foundation.h>
#import "Objc-swift-bridging-header.h"
#import "Swift-objc-bridging-header.h"

// ===== HELPERS =====

NSDictionary* parseJson(const char* jsonStr){
    if (NULL == jsonStr){
        return NULL;
    }

    NSError* err = NULL;
    NSDictionary* json = [NSJSONSerialization JSONObjectWithData: [[NSString stringWithUTF8String:jsonStr] dataUsingEncoding:NSUTF8StringEncoding] options: 0 error: &err];
    if (err) {
        NSLog(@"Failed to decode JSON: %s", jsonStr);
        return NULL;
    }

    return json;
}

char* KSCStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);

    return res;
}

NSString* parseCString(const char* str){
    return [NSString stringWithUTF8String:str];
}

char* createCString(NSString* str){
    return KSCStringCopy([str cStringUsingEncoding:NSUTF8StringEncoding]);
}

char* dictionaryToCString(NSDictionary* data){
    if (nil == data) {
        return NULL;
    }

    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:data options:kNilOptions error:nil];
    if (!jsonData) {
        return NULL;
    }

    NSString *jsonStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    return KSCStringCopy(createCString(jsonStr));
}

// ===== API FOR C# =====

void OptimoveReportEvent(const char* type, const char* jsonData) {
    [Optimove_Unity reportEvent:parseCString(type) parameters:parseJson(jsonData)];
}

void OptimoveReportScreenVisit(const char* screenTitle, const char* screenCategory) {
    [Optimove_Unity reportScreenVisit:parseCString(screenTitle) screenCategory:parseCString(screenCategory)];
}

void OptimoveRegisterUser(const char* userId, const char* email) {
    [Optimove_Unity registerUser:parseCString(userId) email:parseCString(email)];
}

void OptimoveSetUserId(const char* userId) {
    [Optimove_Unity setUserId:parseCString(userId)];
}

void OptimoveSetUserEmail(const char* email) {
    [Optimove_Unity setUserEmail:parseCString(email)];
}

char* OptimoveGetVisitorId() {
    return KSCStringCopy([[Optimove_Unity getVisitorId] cStringUsingEncoding:NSUTF8StringEncoding]);
}

void OptimoveSignOutUser() {
    [Optimove_Unity signOutUser];
}

void OptimoveUpdatePushRegistration(int status) {
    if (status == 1){
        [Optimove_Unity pushRegister];
    }
    else if (status == 0){
        [Optimove_Unity pushUnregister];
    }
}

void OptimoveInAppUpdateConsentForUser(int consented) {
    NSNumber* consentedNumber = [NSNumber numberWithInt:consented];

    [Optimove_Unity inAppUpdateConsent:[consentedNumber boolValue]];
}

char* OptimoveInAppGetInboxItems() {
    NSMutableArray<NSDictionary*>* items = [Optimove_Unity inAppGetInboxItems];

    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:items options:kNilOptions error:nil];
    if (!jsonData) {
        return createCString(@"[]");
    }

    NSString *jsonStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return createCString(jsonStr);
}

char* OptimoveInAppPresentInboxMessage(int messageId) {
    NSString* result = [Optimove_Unity inAppPresentInboxMessage:messageId];

    return createCString(result);
}

BOOL OptimoveInAppDeleteMessageFromInbox(int messageId) {
    return [Optimove_Unity inAppDeleteMessageFromInbox:messageId];
}

BOOL OptimoveInAppMarkAsRead(int messageId) {
    return [Optimove_Unity inAppMarkAsRead:messageId];
}

BOOL OptimoveMarkAllInboxItemsAsRead() {
    return [Optimove_Unity inAppMarkAllInboxItemsAsRead];
}

// ===== Native -> C# =====

void OptimoveInAppGetInboxSummary(const char* guid){
    NSString* guidStr = [NSString stringWithUTF8String:guid];

    [Optimove_Unity inAppGetInboxSummary:guidStr handler:^(NSDictionary *result) {
        char* cStr = dictionaryToCString(result);
        if (cStr == NULL){
            return;
        }

        UnitySendMessage("OptimoveSdkGameObject", "InvokeInboxSummaryHandler", cStr);
    }];
}

void OptimoveCallUnityInAppDeepLinkPressed(NSDictionary* press){
    char* cStr = dictionaryToCString(press);
    if (cStr == NULL){
        return;
    }

    UnitySendMessage("OptimoveSdkGameObject", "InAppDeepLinkPressed", cStr);
}

void OptimoveCallUnityInAppInboxUpdated(){
    UnitySendMessage("OptimoveSdkGameObject", "InAppInboxUpdated", "");
}

void OptimoveCallUnityPushOpened(NSDictionary* push){
    char* cStr = dictionaryToCString(push);
    if (cStr == NULL){
        return;
    }

    UnitySendMessage("OptimoveSdkGameObject", "PushOpened", cStr);
}

void OptimoveCallUnityPushReceived(NSDictionary* push){
    char* cStr = dictionaryToCString(push);
    if (cStr == NULL){
        return;
    }

    UnitySendMessage("OptimoveSdkGameObject", "PushReceived", cStr);
}

void OptimoveCallUnityDeepLinkResolved(NSDictionary* ddl){
    char* cStr = dictionaryToCString(ddl);
    if (cStr == NULL){
        return;
    }

    UnitySendMessage("OptimoveSdkGameObject", "DeepLinkResolved", cStr);
}
