using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
   * A private copy of the UI sprites, so they can be used locally.
   */
  private CastUISprites uiSprites;

  /**
   * A private copy of the UI cast connecting animation, so they can be used locally.
   */
  private Animator connectingAnimator;

  /**
   * Sets the sprites, so they can be used internally.
   */
  public void SetSprites(CastUISprites sprites) {
    uiSprites = sprites;
  }

  /**
   * Sets the cast connecting animator, so they can be used internally.
   */
  public void SetConnectingAnimator(Animator animator) {
    connectingAnimator = animator;
  }

  /**
   * Shows the "casting" state for the cast button.
   */
  public void ShowCasting() {
    castButton.image.sprite = uiSprites.casting;
    Show();
  }

  /**
   * Shows the "not casting" state for the cast button.
   */
  public void ShowNotCasting() {
    castButton.image.sprite = uiSprites.notCasting;
    Show();
  }

  /**
   * Shows the "connecting" animation for the cast button.
   */
  public void ShowConnecting() {
    Show();
    connectingAnimator.enabled = true;
    connectingAnimator.Play("CastButtonConnecting");
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
