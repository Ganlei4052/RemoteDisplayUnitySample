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
   * Sets the sprites, so they can be used internally.
   */
  public void SetSprites(CastUISprites sprites) {
    uiSprites = sprites;
  }

  /**
   * Shows the "casting" state for the cast button.
   */
  public void ShowCasting() {
    castButton.image.sprite = uiSprites.casting;
    gameObject.SetActive(true);
  }

  /**
   * Shows the "not casting" state for the cast button.
   */
  public void ShowNotCasting() {
    castButton.image.sprite = uiSprites.notCasting;
    gameObject.SetActive(true);
  }

  /**
   * Hides the cast button.
   */
  public void Hide() {
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
