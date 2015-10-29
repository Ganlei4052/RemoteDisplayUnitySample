// Copyright 2015 Google Inc.

using UnityEngine;
using UnityEngine.UI;

namespace Google.Cast.RemoteDisplay.UI {
  /**
   * Contains the functionality for the cast button and corresponding frame.
   */
  public class CastButtonFrame : MonoBehaviour {

    /**
     * Start/stop casting button.
     */
    public Button castButton;

    /**
     * The callback for tapping the cast button.
     */
    public UICallback castButtonTappedCallback;

    /**
     * Tracks whether the frame is currently casting.
     */
    private bool isCasting = false;

    /**
     * A private copy of the UI sprites, so they can be used locally.
     */
    private CastUISprites uiSprites;
    public CastUISprites UiSprites {
      set {
        uiSprites = value;
      }
    }

    /**
     * A private copy of the UI cast connecting animation, so they can be used locally.
     */
    private Animator connectingAnimator;
    public Animator ConnectingAnimator {
      set {
        connectingAnimator = value;
      }
    }

    /**
     * Shows the "casting" state for the cast button.
     */
    public void ShowCasting() {
      isCasting = true;
      connectingAnimator.enabled = false;
      castButton.image.sprite = uiSprites.casting;
      Show();
    }

    /**
     * Shows the "connecting" animation for the cast button.
     */
    public void ShowConnecting() {
      // The state is already casting - do nothing.
      if (isCasting) {
        return;
      }
      Show();
      connectingAnimator.enabled = true;
      connectingAnimator.Play("CastButtonConnecting");
    }

    /**
     * Shows the "not casting" state for the cast button.
     */
    public void ShowNotCasting() {
      isCasting = false;
      connectingAnimator.enabled = false;
      castButton.image.sprite = uiSprites.notCasting;
      Show();
    }

    /**
     * Shows the cast button.
     */
    public void Show() {
      connectingAnimator.enabled = false;
      gameObject.SetActive(true);
    }

    /**
     * Hides the cast button.
     */
    public void Hide() {
      connectingAnimator.enabled = false;
      gameObject.SetActive(false);
    }

    /**
     * Triggers the callback for tapping the cast button.  Set as the OnClick function for
     * CastButton.
     */
    public void OnCastButtonTapped() {
      castButtonTappedCallback();
    }
  }
}
