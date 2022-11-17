#ifndef Swift_objc_bridging_header_h
#define Swift_objc_bridging_header_h

void OptimoveCallUnityInAppDeepLinkPressed(NSDictionary* press);
void OptimoveCallUnityInAppInboxUpdated();
void OptimoveCallUnityPushOpened(NSDictionary* push);
void OptimoveCallUnityPushReceived(NSDictionary* push);
void OptimoveCallUnityDeepLinkResolved(NSDictionary* ddl);

#endif /* Swift_objc_bridging_header_h */
