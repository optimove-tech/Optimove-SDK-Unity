package com.example.optimoveunityplugin;

import android.app.Activity;

import com.optimove.android.Optimove;
import com.optimove.android.OptimoveConfig;

public class OptimovePlugin {
    private Activity unityActivity;

    public void initialize(String optimoveCredentials, String optimoveMobileCredentials, Activity unityActivity) {
        this.unityActivity = unityActivity;
        OptimoveConfig.Builder builder = new OptimoveConfig.Builder(optimoveCredentials, optimoveMobileCredentials);
        Optimove.initialize(unityActivity.getApplication(), builder.build());
    }
}
