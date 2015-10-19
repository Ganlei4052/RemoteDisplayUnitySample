﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Google.Cast.RemoteDisplay.Internal {
  /**
   * Extension for debugging the remote display plugin in Unity.
   */
  public class CastRemoteDisplayUnityExtension : ICastRemoteDisplayExtension {
    private CastRemoteDisplayExtensionManager extensionManager;
    private CastRemoteDisplaySimulator displaySimulator;
    private string selectedDevice;
    private Texture remoteDisplayTexture;
    private Texture2D screenTexture;
    private Material material;

    public CastRemoteDisplayUnityExtension(CastRemoteDisplayExtensionManager extensionManager,
        CastRemoteDisplaySimulator displaySimulator) {
      this.extensionManager = extensionManager;
      this.displaySimulator = displaySimulator;
      this.displaySimulator.DisplayExtension = this;
    }

    public void OnEnable() {
    }

    public void Activate() {
      material = new Material(Shader.Find("Unlit/Texture"));
      material.hideFlags = HideFlags.HideAndDontSave;
    }

    public void Deactivate() {
      GameObject.DestroyImmediate(material);
    }

    public void RenderFrame() {
      if (GetSelectedCastDeviceId() != null && GetSelectedCastDeviceId().Length != 0
        && remoteDisplayTexture != null && displaySimulator.simulateRemoteDisplay
        && material != null) {
        Rect rect = new Rect(Screen.width * displaySimulator.remoteDisplayRect.xMin,
                             Screen.height * displaySimulator.remoteDisplayRect.yMin,
                             Screen.width * displaySimulator.remoteDisplayRect.width,
                             Screen.height * displaySimulator.remoteDisplayRect.height);

        Graphics.SetRenderTarget(null); // Main display.
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0); // Flip Y axis.
        Graphics.DrawTexture(rect, remoteDisplayTexture, material);
        GL.PopMatrix();
      }
    }

    public void UpdateDevices() {
      if (displaySimulator.castDevices.Count > 0) {
        extensionManager._callback_OnCastDevicesUpdated("dummy");
      }
    }

    public void ThrowError(CastErrorCode errorCode) {
      string rawErrorString = (int) errorCode + ": There was a fake error thrown by the " +
        "simulator - " + errorCode.ToString();
      extensionManager._callback_OnCastError(rawErrorString);
    }

    public void OnAudioFilterRead(float[] data, int channels) {
    }

    public void OnRemoteDisplaySessionStart(string deviceName) {
    }

    public void OnRemoteDisplaySessionStop() {
    }

    public List<CastDevice> GetCastDevices() {
      return new List<CastDevice>(displaySimulator.castDevices);
    }

    public void SelectCastDevice(string deviceId) {
      selectedDevice = deviceId;
      extensionManager._callback_OnRemoteDisplaySessionStart(selectedDevice);
    }

    public void SetRemoteDisplayTexture(Texture texture) {
      remoteDisplayTexture = texture;
    }

    public void StopRemoteDisplaySession() {
      selectedDevice = null;
      extensionManager._callback_OnRemoteDisplaySessionEnd(selectedDevice);
    }

    public string GetSelectedCastDeviceId() {
      return selectedDevice;
    }
  }
}
