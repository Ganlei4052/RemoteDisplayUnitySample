using UnityEngine;

namespace Google.Cast.RemoteDisplay.Internal {
  /**
   * Required to be set on the AudioListener intended to be used in the remote display. Can't
   * add [RequereComponent(typeof(AudioListener))] to avoid requiring developers to remove this
   * internal component before destroying an AudioListener.
   */
  public class CastRemoteDisplayAudioInterceptor : MonoBehaviour {

    private CastRemoteDisplayExtensionManager extensionManager;

    public void SetCastRemoteDisplayExtensionManager(
        CastRemoteDisplayExtensionManager extensionManager) {
      this.extensionManager = extensionManager;
    }

    /**
     * The Unity callback for when the AudioListener component on this object picks up audio every
     *  frame.
     */
    void OnAudioFilterRead(float[] data, int channels) {
      if (extensionManager != null) {
        extensionManager.OnAudioFilterRead(data, channels);
      }
    }
  }
}
