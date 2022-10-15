#import <Foundation/Foundation.h>
#import "Bridging-Header.h"


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

NSString* parseCString(const char* str){
    return [NSString stringWithUTF8String:str];
}

char* KSCStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);

    return res;
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