// Copyright 2015 Google Inc.

#if UNITY_IOS

using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Google.Cast.RemoteDisplay.Internal {
  /**
   * iOS-specific implementation to extend Unity to support Google Cast Remote Display.
   */
  public class CastRemoteDisplayiOSExtension : ICastRemoteDisplayExtension {
    private CastRemoteDisplayExtensionManager extensionManager;
    private string selectedCastDeviceId;

    public CastRemoteDisplayiOSExtension(CastRemoteDisplayExtensionManager extensionManager) {
      this.extensionManager = extensionManager;
    }

    public void Activate() {
      CastRemoteDisplayiOSUnityBridge.StartScan(
          extensionManager.CastRemoteDisplayManager.CastAppId, extensionManager);

      AudioSpeakerMode audioSpeakerMode = AudioSettings.GetConfiguration().speakerMode;
      int numberChannels = 0;
      switch (audioSpeakerMode) {
        case AudioSpeakerMode.Mono:
          numberChannels = 1;
          break;
        case AudioSpeakerMode.Stereo:
          numberChannels = 2;
          break;
        case AudioSpeakerMode.Quad:
          numberChannels = 4;
          break;
        case AudioSpeakerMode.Surround:
          numberChannels = 5;
          break;
        case AudioSpeakerMode.Mode5point1:
          numberChannels = 6;
          break;
        case AudioSpeakerMode.Mode7point1:
          numberChannels = 7;
          break;
        default:
          Debug.LogError("Cast remote display cannot support audio configuration speakermode: " +
            audioSpeakerMode);
          return;
      }

      // Note: Unity uses PCM float 32 interleaved audio.
      CastRemoteDisplayiOSUnityBridge.SetAudioFormat(
        CastRemoteDisplayiOSUnityBridge.AudioFormat.AVAudioPCMFormatFloat32,
        AudioSettings.GetConfiguration().sampleRate,
        numberChannels,
        /* isInterleaved */ true);
    }

    public void Deactivate() {
      CastRemoteDisplayiOSUnityBridge.TeardownRemoteDisplay();
    }

    public void RenderFrame() {
      CastRemoteDisplayiOSUnityBridge.RenderRemoteDisplay();
    }

    public void OnAudioFilterRead(float[] data, int channels) {
      CastRemoteDisplayiOSUnityBridge.EnqueueRemoteDisplayAudioBuffer(data,
          /* dataByteSize */ data.Length * sizeof(float),
          /* numberChannels */ channels,
          /* numberFrames */ data.Length / channels);
    }

    public void OnRemoteDisplaySessionStart(string deviceId) {
    }

    public void OnRemoteDisplaySessionStop() {
      selectedCastDeviceId = null;
    }

    public List<CastDevice> GetCastDevices() {
      return CastRemoteDisplayiOSUnityBridge.GetCastDevices();
    }

    public void SelectCastDevice(string deviceId) {
      if (CastRemoteDisplayiOSUnityBridge.SelectCastDevice(deviceId,
            extensionManager.CastRemoteDisplayManager.Configuration)) {
        selectedCastDeviceId = deviceId;
      }
    }

    public void SetRemoteDisplayTexture(Texture texture) {
      IntPtr texturePointer = texture.GetNativeTexturePtr();
      if (texturePointer == IntPtr.Zero) {
        Debug.LogError("Couldn't obtain native pointer for the remote display texture.");
        return;
      }
      CastRemoteDisplayiOSUnityBridge.SetRemoteDisplayTexture(texturePointer);
    }

    public void StopRemoteDisplaySession() {
      CastRemoteDisplayiOSUnityBridge.StopRemoteDisplaySession();
    }

    public string GetSelectedCastDeviceId() {
      return selectedCastDeviceId;
    }
  }
}

#endif
