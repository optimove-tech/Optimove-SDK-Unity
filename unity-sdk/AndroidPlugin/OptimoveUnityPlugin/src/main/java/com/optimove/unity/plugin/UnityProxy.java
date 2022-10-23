package com.optimove.unity.plugin;

import android.content.Context;

import com.optimove.android.Optimove;
import com.optimove.android.optimobile.InAppDeepLinkHandlerInterface;
import com.optimove.android.optimobile.InAppInboxItem;
import com.optimove.android.optimobile.InAppInboxSummary;
import com.optimove.android.optimobile.OptimoveInApp;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.net.URL;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.TimeZone;

public class UnityProxy {
    private static JSONObject pendingPush;

    public static void setUserId(String userId) {
        try {
            Optimove.getInstance().setUserId(userId);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void setUserEmail(String userEmail) {
        try {
            Optimove.getInstance().setUserEmail(userEmail);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void reportEvent(String eventName) {
        try {
            Optimove.getInstance().reportEvent(eventName);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void reportEvent(String eventName, String eventParams) {

        if (eventParams == null) {
            reportEvent(eventName);
            return;
        }

        try {
            JSONObject eventParamsJson = new JSONObject(eventParams);
            Iterator<String> iterator = eventParamsJson.keys();
            Map<String, Object> eventParamsMap = new HashMap<>();
            while (iterator.hasNext()) {
                String key = iterator.next();
                eventParamsMap.put(key, eventParamsJson.get(key));
            }
            Optimove.getInstance().reportEvent(eventName, eventParamsMap);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void registerUser(String userId, String userEmail) {
        try {
            Optimove.getInstance().registerUser(userId, userEmail);
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public static void reportScreenVisit(String screenName) {
        try {
            Optimove.getInstance().reportScreenVisit(screenName);
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public static void reportScreenVisit(String screenName, String screenCategory) {
        try {
            Optimove.getInstance().reportScreenVisit(screenName, screenCategory);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static String getVisitorId() {
        try {

            return Optimove.getInstance().getVisitorId();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }

    public static void pushRegister() {
        try {
            Optimove.getInstance().pushRegister();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void pushUnregister() {
        try {

            Optimove.getInstance().pushUnregister();
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public static void inAppUpdateConsent(boolean consented) {
        try {

            OptimoveInApp.getInstance().updateConsentForUser(consented);
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public static void signOutUser() {
        try {

            Optimove.getInstance().signOutUser();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void pollPendingPush() {
        if (pendingPush == null) {
            return;
        }
        try {
            if (pendingPush.getBoolean("pushOpened")) {
                notifyUnityOfPush(pendingPush.getJSONObject("pushMessage"), "Opened");
            } else {
                notifyUnityOfPush(pendingPush.getJSONObject("pushMessage"), "Received");
            }

        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            pendingPush = null;
        }
    }


    public static String inAppGetInboxItems() {
        SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
        formatter.setTimeZone(TimeZone.getTimeZone("UTC"));

        List<InAppInboxItem> items = OptimoveInApp.getInstance().getInboxItems();
        JSONArray results = new JSONArray();
        try {
            for (InAppInboxItem item : items) {
                JSONObject mapped = new JSONObject();

                mapped.put("id", item.getId());
                mapped.put("title", item.getTitle());
                mapped.put("subtitle", item.getSubtitle());
                mapped.put("isRead", item.isRead());
                mapped.put("sentAt", formatter.format(item.getSentAt()));

                Date availableFrom = item.getAvailableFrom();
                Date availableTo = item.getAvailableTo();
                Date dismissedAt = item.getDismissedAt();

                JSONObject data = item.getData();
                mapped.put("data", data == null ? JSONObject.NULL : data);

                URL imageUrl = item.getImageUrl();
                mapped.put("imageUrl", imageUrl == null ? JSONObject.NULL : imageUrl.toString());

                if (null == availableFrom) {
                    mapped.put("availableFrom", JSONObject.NULL);
                } else {
                    mapped.put("availableFrom", formatter.format(availableFrom));
                }

                if (null == availableTo) {
                    mapped.put("availableTo", JSONObject.NULL);
                } else {
                    mapped.put("availableTo", formatter.format(availableTo));
                }

                if (null == dismissedAt) {
                    mapped.put("dismissedAt", JSONObject.NULL);
                } else {
                    mapped.put("dismissedAt", formatter.format(dismissedAt));
                }

                results.put(mapped);
            }
        } catch (JSONException e) {
            e.printStackTrace();
            return null;
        }

        return results.toString();
    }

    public static String inAppPresentInboxMessage(long messageId) {
        try {
            List<InAppInboxItem> items = OptimoveInApp.getInstance().getInboxItems();
            for (InAppInboxItem item : items) {
                if (item.getId() == messageId) {
                    OptimoveInApp.InboxMessagePresentationResult result = OptimoveInApp.getInstance().presentInboxMessage(item);
                    return result.toString();
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }


        return OptimoveInApp.InboxMessagePresentationResult.FAILED.toString();
    }

    public static boolean inAppDeleteMessageFromInbox(long messageId) {
        try {

            List<InAppInboxItem> items = OptimoveInApp.getInstance().getInboxItems();
            for (InAppInboxItem item : items) {
                if (item.getId() == messageId) {
                    return OptimoveInApp.getInstance().deleteMessageFromInbox(item);
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }


        return false;
    }

    public static boolean inAppMarkInboxItemRead(long messageId) {
        try {

            List<InAppInboxItem> items = OptimoveInApp.getInstance().getInboxItems();
            for (InAppInboxItem item : items) {
                if (item.getId() == messageId) {
                    return OptimoveInApp.getInstance().markAsRead(item);
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }


        return false;
    }

    public static boolean inAppMarkAllInboxItemsRead() {
        try {
            return OptimoveInApp.getInstance().markAllInboxItemsAsRead();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return false;
    }

    public static void inAppGetInboxSummary(String guid) {
        try {

            OptimoveInApp.getInstance().getInboxSummaryAsync((InAppInboxSummary summary) -> {
                notifyUnityOfInboxSummary(guid, summary);
            });
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    private static void notifyUnityOfInboxSummary(String guid, InAppInboxSummary summary) {
        JSONObject res = new JSONObject();
        boolean success = false;
        try {
            res.put("guid", guid);
            if (summary != null) {
                success = true;
                res.put("totalCount", summary.getTotalCount());
                res.put("unreadCount", summary.getUnreadCount());
            }
            res.put("success", success);

            String unityMessage = res.toString();
            UnityPlayer.UnitySendMessage("OptimoveSdkGameObject", "InvokeInboxSummaryHandler", unityMessage);
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    //-- Internal / helper APIs

    static void queueOrSendPushToUnity(JSONObject msg, boolean didOpenFromPush) {

        try {
            if (UnityPlayer.currentActivity == null) {
                JSONObject pendingPush = new JSONObject();
                pendingPush.put("pushOpened", didOpenFromPush);
                pendingPush.put("pushMessage", msg);
                UnityProxy.pendingPush = pendingPush;
            } else {
                if (didOpenFromPush) {
                    notifyUnityOfPush(msg, "Opened");
                } else {

                    notifyUnityOfPush(msg, "Received");
                }

            }
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    private static void notifyUnityOfPush(JSONObject pushMessage, String eventType) {
        String unityMessage = pushMessage.toString();
        UnityPlayer.UnitySendMessage("OptimoveSdkGameObject", "Push" + eventType, unityMessage);
    }


    static class InAppDeepLinkHandler implements InAppDeepLinkHandlerInterface {

        @Override
        public void handle(Context context, InAppButtonPress buttonPress) {
            JSONObject inAppButtonPress = new JSONObject();
            try {
                inAppButtonPress.put("deepLinkData", buttonPress.getDeepLinkData());
                inAppButtonPress.put("messageId", buttonPress.getMessageId());
                JSONObject messageData = buttonPress.getMessageData();
                inAppButtonPress.put("messageData", messageData == null ? JSONObject.NULL : messageData);
            } catch (JSONException e) {
                // noop
            }
            UnityPlayer.UnitySendMessage("OptimoveSdkGameObject", "InAppDeepLinkPressed", inAppButtonPress.toString());
        }
    }

    static class InboxUpdatedHandler implements OptimoveInApp.InAppInboxUpdatedHandler {
        @Override
        public void run() {
            UnityPlayer.UnitySendMessage("OptimoveSdkGameObject", "InAppInboxUpdated", "");
        }
    }

}