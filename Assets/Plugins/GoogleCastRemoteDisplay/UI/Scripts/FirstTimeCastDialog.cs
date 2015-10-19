using UnityEngine;

namespace Google.Cast.RemoteDisplay.UI {
  /**
   * Dialog to show the first time cast information.  Only to be shown once, on the first time a
   * Cast device is detected.
   */
  public class FirstTimeCastDialog : MonoBehaviour {
    /**
     * Shows the first time cast dialog.
     */
    public void Show() {
      gameObject.SetActive(true);
    }

    /**
     * Hides the first time cast dialog.
     */
    public void Hide() {
      gameObject.SetActive(false);
    }
  }
}
