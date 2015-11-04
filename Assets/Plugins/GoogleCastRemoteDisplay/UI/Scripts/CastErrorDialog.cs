// Copyright 2015 Google Inc.

using UnityEngine;
using UnityEngine.UI;

namespace Google.Cast.RemoteDisplay.UI {
  /**
   * The dialog for displaying error messages from the cast remote display manager.
   */
  public class CastErrorDialog : MonoBehaviour {

    /**
     * Outlet for the confirm button.
     */
    public Button okayButton;

    /**
     * Outlet for the title label to display errors.
     */
    public Text titleLabel;

    /**
     * Outlet for the status label to display errors.
     */
    public Text statusLabel;

    /**
     * The callback for closing the error dialog.
     */
    public UICallback okayButtonTappedCallback;

    /**
     * When an error is thrown, set up the error dialog to display it when the Cast button is
     * pressed.
     */
    public void SetError(CastError error) {
      titleLabel.text = error.ReadableErrorTitle;
      statusLabel.text = error.ReadableErrorBody;
    }

    /**
     * Triggers the callback for closing the error dialog.  Set as the OnClick function for
     * OkayButton.
     */
    public void OnOkayButtonTapped() {
      okayButtonTappedCallback();
    }
  }
}
