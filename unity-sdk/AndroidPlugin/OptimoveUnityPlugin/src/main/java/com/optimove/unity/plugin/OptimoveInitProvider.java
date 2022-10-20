package com.optimove.unity.plugin;

import android.app.Application;
import android.content.ContentProvider;
import android.content.ContentValues;
import android.database.Cursor;
import android.net.Uri;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.optimove.android.Optimove;
import com.optimove.android.OptimoveConfig;
import com.optimove.android.optimobile.OptimoveInApp;

public class OptimoveInitProvider extends ContentProvider {
    @Override
    public boolean onCreate() {
        Application app = (Application) this.getContext().getApplicationContext();
        OptimoveConfig.Builder configBuilder = new OptimoveConfig.Builder("YOUR_OPTIMOVE_CREDENTIALS", "YOUR_OPTIMOVE_MOBILE_CREDENTIALS");
        configBuilder.enableInAppMessaging(OptimoveConfig.InAppConsentStrategy.AUTO_ENROLL);
        Optimove.initialize(app, configBuilder.build());
        Optimove.getInstance().pushRegister();
        Optimove.getInstance().setPushActionHandler(new PushReceiver.PushActionHandler());
        OptimoveInApp.getInstance().setDeepLinkHandler(new UnityProxy.DeepLinkHandler());
        OptimoveInApp.getInstance().setOnInboxUpdated(new UnityProxy.InboxUpdatedHandler());
        return false;
    }

    @Nullable
    @Override
    public Cursor query(@NonNull Uri uri, @Nullable String[] strings, @Nullable String s, @Nullable String[] strings1, @Nullable String s1) {
        return null;
    }

    @Nullable
    @Override
    public String getType(@NonNull Uri uri) {
        return null;
    }

    @Nullable
    @Override
    public Uri insert(@NonNull Uri uri, @Nullable ContentValues contentValues) {
        return null;
    }

    @Override
    public int delete(@NonNull Uri uri, @Nullable String s, @Nullable String[] strings) {
        return 0;
    }

    @Override
    public int update(@NonNull Uri uri, @Nullable ContentValues contentValues, @Nullable String s, @Nullable String[] strings) {
        return 0;
    }
}
