package com.optimove.unity.android;

import android.app.Activity;
import android.os.Bundle;

import com.optimove.unity.plugin.UnityProxy;

public class MainActivity extends Activity {

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        UnityProxy.pushRequestDeviceToken();
    }
}
