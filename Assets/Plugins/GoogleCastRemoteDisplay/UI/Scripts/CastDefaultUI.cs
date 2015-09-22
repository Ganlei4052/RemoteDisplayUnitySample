using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/**
 * Encapsulates the sprite outlets to organize assets required throughout the UI.
 */
[System.Serializable]
public class CastUISprites {
  /**
   * Sprite for the "casting" status.
   */
  public Sprite casting;

  /**
   * Sprite for the "not casting" status.
   */
  public Sprite notCasting;
}

/**
 * Functions for the various children of the Default UI to callback to this class
 */
public delegate void UICallback();

/**
 * The default UI for Cast functionality.  Handles cast selection, display, starting/ending Cast
 * sessions, basic error display, etc.
 */
public class CastDefaultUI : MonoBehaviour {

  /**
   * Container for the start/stop casting button.
   */
  public CastButtonFrame castButtonFrame;

  /**
   * The selection list UI element.
   */
   public CastListDialog castListDialog;

  /**
   * Dialog for displaying errors from the Remote Display Manager.
   */
  public CastErrorDialog errorDialog;

  /**
   * Outlet for the sprites needed by various Cast UI components.
   */
  public CastUISprites castUISprites;

  /**
   * Current display manager - set by the UI Controller.
   */
  private CastRemoteDisplayManager displayManager;

  /**
   * Tracks whether the UI Controller has performed initialization
   */
  private bool isInitialized;

  /**
   * Tracks whether the app is casting.
   */
  private bool isCasting = false;

  /**
   * Listens to the cast manager notifications, and sets up the start
   * state for the UI.
   */
  public void Initialize(CastRemoteDisplayManager manager) {
    if (!isInitialized) {
      displayManager = manager;
      manager.castDevicesUpdatedEvent += OnCastDevicesUpdated;
      manager.remoteDisplaySessionStartEvent += OnRemoteDisplaySessionStart;
      manager.remoteDisplaySessionEndEvent += OnRemoteDisplaySessionEnd;
      manager.remoteDisplayErrorEvent += OnRemoteDisplayError;
      isInitialized = true;

      castButtonFrame.castButtonTappedCallback = OnCastButtonTapped;
      castListDialog.closeButtonTappedCallback = OnCloseCastList;
      errorDialog.okayButtonTappedCallback = OnConfirmErrorDialog;

      castButtonFrame.SetSprites(castUISprites);
      castListDialog.Hide();
      errorDialog.gameObject.SetActive(false);
    }
  }

  /**
   * Unlistens to the cast manager notifications.
   */
  public void Uninitialize(CastRemoteDisplayManager manager) {
    if (isInitialized) {
      manager.castDevicesUpdatedEvent -= OnCastDevicesUpdated;
      manager.remoteDisplaySessionStartEvent -= OnRemoteDisplaySessionStart;
      manager.remoteDisplaySessionEndEvent -= OnRemoteDisplaySessionEnd;
      manager.remoteDisplayErrorEvent -= OnRemoteDisplayError;
      isInitialized = false;
    }
  }

  /**
   * When the list of devices updates, update the list. Called when the list of
   * devices updates.
   */
  public void OnCastDevicesUpdated(CastRemoteDisplayManager manager) {
    castListDialog.PopulateList(manager);
  }

  /**
   * Closes the list of devices, sets up the casting display.
   */
  public void OnRemoteDisplaySessionStart(CastRemoteDisplayManager manager) {
    isCasting = true;
    castListDialog.Hide();
    castButtonFrame.ShowCasting();
  }

  /**
   * Cleans up display when the session is over.
   */
  public void OnRemoteDisplaySessionEnd(CastRemoteDisplayManager manager) {
    isCasting = false;
    castButtonFrame.ShowNotCasting();
  }

  /**
   * Handles error messages from the Remote Display Manager.
   */
  public void OnRemoteDisplayError(CastRemoteDisplayManager manager,
      CastErrorCode errorCode, string errorString) {

    castButtonFrame.Hide();
    errorDialog.gameObject.SetActive(true);
    errorDialog.statusLabel.text = errorString;
  }

  /**
   * Callback when the user taps close button.
   */
  public void OnCloseCastList() {
    castListDialog.Hide();
    castButtonFrame.ShowNotCasting();
  }

  /**
   * Either stop casting or open the list of detected cast devices.
   */
  public void OnCastButtonTapped() {
    if (isCasting) {
      displayManager.StopRemoteDisplaySession();
    } else {
      castButtonFrame.Hide();
      castListDialog.Show();
    }
  }

  /**
   * Called when the error dialog is confirmed.
   */
  public void OnConfirmErrorDialog() {
    castButtonFrame.ShowNotCasting();
    errorDialog.gameObject.SetActive(false);
  }
}
