package com.optimove.unity.plugin;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.util.Log;

import com.optimove.android.Optimove;
import com.optimove.android.optimobile.Optimobile;
import com.optimove.android.optimobile.PushActionHandlerInterface;
import com.optimove.android.optimobile.PushBroadcastReceiver;
import com.optimove.android.optimobile.PushMessage;

import org.json.JSONException;
import org.json.JSONObject;

public class PushReceiver extends PushBroadcastReceiver {

    @Override
    protected void onPushReceived(Context context, PushMessage pushMessage) {
        super.onPushReceived(context, pushMessage);
        UnityProxy.queueOrSendPushToUnity(PushReceiver.pushMessageToJsonObject(pushMessage, null), false);
    }

    static JSONObject pushMessageToJsonObject(PushMessage pushMessage, String actionId) {
        JSONObject result = new JSONObject();
        String title = pushMessage.getTitle();
        String message = pushMessage.getMessage();
        Uri uri = pushMessage.getUrl();
        JSONObject data = pushMessage.getData();

        try {
            result.put("id", pushMessage.getId());
            result.put("title", title == null ? JSONObject.NULL : title);
            result.put("message", message == null ? JSONObject.NULL : message);
            result.put("url", uri == null ? JSONObject.NULL : uri.toString());
            result.put("actionId", actionId == null ? JSONObject.NULL : actionId);
            result.put("data", data == null ? JSONObject.NULL : data);
        } catch (JSONException e) {
            e.printStackTrace();
        }

        return result;
    }

    @Override
    @SuppressWarnings("unchecked")
    protected void onPushOpened(Context context, PushMessage pushMessage) {
        try {
            Optimove.getInstance().pushTrackOpen(pushMessage.getId());
        } catch (Optimobile.UninitializedException ignored) {
        }
        PushReceiver.handlePushOpen(context, pushMessage, null);
    }

    private static void handlePushOpen(Context context, PushMessage pushMessage, String actionId) {
        PushReceiver pr = new PushReceiver();
        Intent launchIntent = pr.getPushOpenActivityIntent(context, pushMessage);

        if (null == launchIntent) {
            return;
        }
        ComponentName component = launchIntent.getComponent();
        if (null == component) {
            return;
        }
        Class<? extends Activity> cls = null;
        try {
            cls = (Class<? extends Activity>) Class.forName(component.getClassName());
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        }
        // Ensure we're trying to launch an Activity
        if (null == cls) {
            return;
        }
        if (null != pushMessage.getUrl()) {
            launchIntent = new Intent(Intent.ACTION_VIEW, pushMessage.getUrl());
        }

        addDeepLinkExtras(pushMessage, launchIntent);

        launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        context.startActivity(launchIntent);

        if (pushMessage.getData().has("k.deepLink")) {
            return;
        }

        JSONObject msg = PushReceiver.pushMessageToJsonObject(pushMessage, actionId);
        UnityProxy.queueOrSendPushToUnity(msg, true);
    }

    static class PushActionHandler implements PushActionHandlerInterface {
        @SuppressLint("MissingPermission")
        @Override
        public void handle(Context context, PushMessage pushMessage, String actionId) {
            PushReceiver.handlePushOpen(context, pushMessage, actionId);
            try {
                Intent it = new Intent(Intent.ACTION_CLOSE_SYSTEM_DIALOGS);
                context.sendBroadcast(it);
            } catch (SecurityException ignored) {
            }
        }
    }


}
