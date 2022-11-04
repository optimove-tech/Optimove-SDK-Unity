package com.optimove.unity.plugin;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import androidx.core.app.TaskStackBuilder;
import com.optimove.android.Optimove;
import com.optimove.android.optimobile.Optimobile;
import com.optimove.android.optimobile.PushActionHandlerInterface;
import com.optimove.android.optimobile.PushBroadcastReceiver;
import com.optimove.android.optimobile.PushMessage;
import com.unity3d.player.UnityPlayer;
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

        handlePushOpen(context, pushMessage, null);
    }

    @Override
    protected Intent getPushOpenActivityIntent(Context context, PushMessage pushMessage) {
        // Dont launch any activity from base sdk. This lets launching activity from here with desired
        // launch flags in order to avoid unity bug with FLAG_ACTIVITY_CLEAR_TASK.
        return null;
    }

    private static void handlePushOpen(Context context, PushMessage pushMessage, String actionId) {
        Intent launchIntent = context.getPackageManager().getLaunchIntentForPackage(context.getPackageName());
        if (null == launchIntent) {
            return;
        }

        launchIntent.putExtra(PushMessage.EXTRAS_KEY, pushMessage);
        launchIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK/* | Intent.FLAG_ACTIVITY_CLEAR_TASK*/);

        ComponentName component = launchIntent.getComponent();
        if (null == component) {
            return;
        }

        Class<? extends Activity> cls = null;
        try {
            cls = Class.forName(component.getClassName()).asSubclass(Activity.class);
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        }

        // Ensure we're trying to launch an Activity
        if (null == cls) {
            return;
        }

        if (UnityPlayer.currentActivity != null) {
            Intent existingIntent = UnityPlayer.currentActivity.getIntent();
            addDeepLinkExtras(pushMessage, existingIntent);
        }

        if (null != pushMessage.getUrl()) {
            launchIntent = new Intent(Intent.ACTION_VIEW, pushMessage.getUrl());

            addDeepLinkExtras(pushMessage, launchIntent);

            TaskStackBuilder taskStackBuilder = TaskStackBuilder.create(context);
            taskStackBuilder.addParentStack(component);
            taskStackBuilder.addNextIntent(launchIntent);
            taskStackBuilder.startActivities();
        } else {
            addDeepLinkExtras(pushMessage, launchIntent);

            context.startActivity(launchIntent);
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
