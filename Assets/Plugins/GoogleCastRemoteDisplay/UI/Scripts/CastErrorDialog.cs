using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * The dialog for displaying error messages from the cast remote display manager.
 */
public class CastErrorDialog : MonoBehaviour {

  /**
   * Outlet for the confirm button.
   */
  public Button okayButton;

  /**
   * Outlet for the status label to display errors.
   */
  public Text statusLabel;

  /**
   * The callback for closing the error dialog.
   */
  public UICallback okayButtonTappedCallback;

  /**
   * Triggers the callback for closing the error dialog.  Set as the OnClick function for
   * OkayButton.
   */
  public void OnOkayButtonTapped() {
    okayButtonTappedCallback();
  }
}
