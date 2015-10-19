// Copyright 2015 Google Inc.
using UnityEngine;
using System.Collections.Generic;

namespace Google.Cast.RemoteDisplay.Internal {
  /**
   * Extends Unity lifecycle methods and adds methods that rely on platform-specific native code.
   */
  public interface ICastRemoteDisplayExtension {

    void Activate();

    void Deactivate();

    void RenderFrame();

    void OnAudioFilterRead(float[] data, int channels);

    void OnRemoteDisplaySessionStart(string deviceName);

    void OnRemoteDisplaySessionStop();

    List<CastDevice> GetCastDevices();

    void SelectCastDevice(string deviceId);

    void SetRemoteDisplayTexture(Texture texture);

    void StopRemoteDisplaySession();

    string GetSelectedCastDeviceId();
  }
}
