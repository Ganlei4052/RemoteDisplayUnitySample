// Copyright 2015 Google Inc.
using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * Extends Unity lifecycle methods and adds methods that rely on platform-specific native code.
 */
public interface ICastRemoteDisplayExtension {

  void Activate();

  void Deactivate();

  void Update();

  void OnAudioFilterRead(float[] data, int channels);

  void OnRemoteDisplaySessionStart(string deviceName);

  void OnRemoteDisplaySessionStop();

  List<CastDevice> GetCastDevices();

  void SelectCastDevice(string deviceId);

  void SetRemoteDisplayTexture(RenderTexture texture);

  void StopRemoteDisplaySession();

  string GetSelectedCastDeviceId();
}
