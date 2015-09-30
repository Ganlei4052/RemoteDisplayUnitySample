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
   * Stores the last error code thrown.
   */
  private CastErrorCode errorCode = CastErrorCode.NoError;
  public CastErrorCode ErrorCode {
    get {
      return errorCode;
    }
  }

  /**
   * When an error is thrown, set up the error dialog to display it when the Cast button is pressed.
   */
  public void SetError(CastErrorCode code, string errorString) {
    errorCode = code;
    statusLabel.text = errorString;
  }

  /**
   * Triggers the callback for closing the error dialog.  Set as the OnClick function for
   * OkayButton.
   */
  public void OnOkayButtonTapped() {
    okayButtonTappedCallback();
  }
}
