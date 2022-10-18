package com.optimove.unity.plugin;

import android.content.Context;
import android.net.Uri;

import com.optimove.android.optimobile.PushBroadcastReceiver;
import com.optimove.android.optimobile.PushMessage;

import org.json.JSONException;
import org.json.JSONObject;

public class PushReceiver  extends PushBroadcastReceiver {

        @Override
        protected void onPushReceived(Context context, PushMessage pushMessage) {
            super.onPushReceived(context, pushMessage);
            
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

}
