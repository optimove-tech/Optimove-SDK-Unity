package com.optimove.unity.plugin;

import com.google.firebase.MessagingUnityPlayerActivity;
import com.optimove.android.Optimove;

import android.content.Intent;
import android.os.Bundle;

public class OptimoveMainActivity extends MessagingUnityPlayerActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Optimove.getInstance().seeIntent(getIntent(), savedInstanceState);
    }

    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        Optimove.getInstance().seeInputFocus(hasFocus);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        Optimove.getInstance().seeIntent(intent);
    }
}