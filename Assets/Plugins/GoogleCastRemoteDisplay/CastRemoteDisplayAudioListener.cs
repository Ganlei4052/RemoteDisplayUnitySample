// Copyright 2015 Google Inc.

using Google.Cast.RemoteDisplay.Internal;
using UnityEngine;

namespace Google.Cast.RemoteDisplay {
  /**
   * A listener class required to be set on the Audio Listener intended to be used in the remote
   * display.
   * In iOS, the AudioListener cannot be added at runtime, so we require that this object is set
   * since the scene's initialization.
   */
  [RequireComponent(typeof(AudioListener))]
  public class CastRemoteDisplayAudioListener : MonoBehaviour {

    private CastRemoteDisplayExtensionManager extensionManager;

    public void setCastRemoteDisplayExtensionManager(
        CastRemoteDisplayExtensionManager extensionManager) {
      this.extensionManager = extensionManager;
    }

    void OnAudioFilterRead(float[] data, int channels) {
      if (extensionManager != null) {
        extensionManager.OnAudioFilterRead(data, channels);
      }
    }
  }
}

