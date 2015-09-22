// Copyright 2015 Google Inc.

#if UNITY_ANDROID

using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/**
 * Android-specific implementation to extend Unity to support Google Cast Remote Display.
 */
public class CastRemoteDisplayAndroidExtension : ICastRemoteDisplayExtension {

  private static string ANDROID_BRIDGE_CLASS_NAME = "com.google.cast.unityplugin.UnityBridge";

  private static string DEVICE_LIST_MARKER_DONE = "DONE";
  private static string DEVICE_LIST_MARKER_NOT_DONE = "NOT_DONE";

  // Max number of JNI calls for a single call to #GetCastDevices.
  private static int MAX_NUMBER_OF_JNI_CALLS_FOR_CAST_DEVICES = 10;

  private CastRemoteDisplayExtensionManager extensionManager;
  private string selectedCastDeviceId = null;

  public CastRemoteDisplayAndroidExtension(CastRemoteDisplayExtensionManager extensionManager) {
    this.extensionManager = extensionManager;
  }

  public void Activate() {
    Debug.Log("RemoteDisplayController started.");
    using (AndroidJavaClass bridge = new AndroidJavaClass(ANDROID_BRIDGE_CLASS_NAME)) {
      // Start scanning for cast media routes. The second parameter is the name of the game object
      // that should get callbacks from the Android side.
      bridge.CallStatic("_native_startScan", extensionManager.CastRemoteDisplayManager.CastAppId,
                        extensionManager.name);
    }
  }

  public void Deactivate() {
    using (AndroidJavaClass bridge = new AndroidJavaClass(ANDROID_BRIDGE_CLASS_NAME)) {
      bridge.CallStatic("_native_teardownRemoteDisplay");
    }
  }

  public void Update() {
  }

  public void OnAudioFilterRead(float[] data, int channels) {
  }

  public void OnRemoteDisplaySessionStart(string deviceId) {
  }

  public void SetRemoteDisplayTexture(RenderTexture texture) {
    IntPtr texturePointer = texture.GetNativeTexturePtr();
    if (texturePointer == IntPtr.Zero) {
      Debug.LogError("Couldn't obtain native pointer for the remote display texture.");
      return;
    }
    using (AndroidJavaClass bridge = new AndroidJavaClass(ANDROID_BRIDGE_CLASS_NAME)) {
      Debug.Log("Setting texture with ID: " + texturePointer.ToInt64());
      bridge.CallStatic("_native_setRemoteDisplayTexture", texturePointer.ToInt64());
    }
  }

  public void OnRemoteDisplaySessionStop() {
    selectedCastDeviceId = null;
  }

  public List<CastDevice> GetCastDevices() {
    List<CastDevice> devices = new List<CastDevice>();
    using (AndroidJavaClass bridge = new AndroidJavaClass(ANDROID_BRIDGE_CLASS_NAME)) {

      string statusString = DEVICE_LIST_MARKER_NOT_DONE;
      int numberOfJniCalls = 0;

      // The static library will respond with a partial device list.
      // The first string indicates whether the list is complete or not, after that, each
      // device is represented by 3 strings: The order is deviceId, deviceName and status.
      // We must call the method mutiple time to get the entire list.
      while(statusString != DEVICE_LIST_MARKER_DONE) {
        using (AndroidJavaObject returnedArray =
            bridge.CallStatic<AndroidJavaObject>("_native_getCastDevices")) {
          if (returnedArray != null && returnedArray.GetRawObject().ToInt32() != 0) {
            string[] deviceInfoStrings =
                AndroidJNIHelper.ConvertFromJNIArray<string[]>(returnedArray.GetRawObject());
            statusString = deviceInfoStrings[0];
            int i = 1;
            while (i < deviceInfoStrings.Length - 2) {
              CastDevice device = new CastDevice();
              device.deviceId = deviceInfoStrings[i++];
              device.deviceName = deviceInfoStrings[i++];
              device.status = deviceInfoStrings[i++];
              devices.Add(device);
            }
          } else {
            Debug.LogError("Couldn't retrieve list of Cast Devices.");
            StopRemoteDisplaySession();
            devices.Clear();
            return devices;
          }
        }
        numberOfJniCalls++;
        if (numberOfJniCalls >= MAX_NUMBER_OF_JNI_CALLS_FOR_CAST_DEVICES) {
          Debug.LogError("Couldn't retrieve the full list of cast devices. JNI call limit " +
              "exceeded.");
          break;
        }
      }
    }
    return devices;
  }

  public void SelectCastDevice(string deviceId) {
    using (AndroidJavaClass bridge = new AndroidJavaClass(ANDROID_BRIDGE_CLASS_NAME)) {
      bridge.CallStatic("_native_selectCastDevice", deviceId);
      selectedCastDeviceId = deviceId;
    }
  }

  public void StopRemoteDisplaySession() {
    using (AndroidJavaClass bridge = new AndroidJavaClass(ANDROID_BRIDGE_CLASS_NAME)) {
      bridge.CallStatic("_native_teardownRemoteDisplay");
      selectedCastDeviceId = null;
    }
  }

  public string GetSelectedCastDeviceId() {
    return selectedCastDeviceId;
  }
}
#endif
