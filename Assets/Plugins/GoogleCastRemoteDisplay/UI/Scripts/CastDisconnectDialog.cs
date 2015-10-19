using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Google.Cast.RemoteDisplay.UI {
  /**
   * The dialog for displaying the disconnect dialog.
   */
  public class CastDisconnectDialog : MonoBehaviour {

    /**
     * Outlet for the disconnect button.
     */
    public Button disconnectButton;

    /**
     * The current cast device name.
     */
    public Text deviceName;

    /**
     * The callback for disconnecting and closing the dialog.
     */
    public UICallback disconnectButtonTappedCallback;

    /**
     * The callback for closing the disconnect dialog.
     */
    public UICallback closeButtonTappedCallback;

    /**
     * Set the cast device name for the dialog title.
     */
    public void SetDeviceName(string name) {
      deviceName.text = name;
    }

    /**
     * Triggers the callback for closing the disconnect dialog.  Set as the OnClick function for
     * DisconnectButton.
     */
    public void OnDisconnectButtonTapped() {
      disconnectButtonTappedCallback();
    }

    /**
     * Triggers the callback for closing the disconnect dialog.  Set as the OnClick function for
     * CloseButton.
     */
    public void OnCloseButtonTapped() {
      closeButtonTappedCallback();
    }
  }
}
