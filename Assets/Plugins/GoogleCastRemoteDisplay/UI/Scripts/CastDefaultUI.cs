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

  private const string FIRST_TIME_CAST_SHOWN = "firstTimeCastShown";

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
   * Dialog for displaying the "first time cast" information.
   */
  public FirstTimeCastDialog firstTimeCastDialog;

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
      HideAll();
      castButtonFrame.Show();
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
   * Resets the UI to hidden, so the proper elements can be shown.
   */
  private void HideAll() {
    castButtonFrame.Hide();
    castListDialog.Hide();
    errorDialog.gameObject.SetActive(false);
    firstTimeCastDialog.Hide();
  }

  /**
   * When the list of devices updates, update the list. Called when the list of
   * devices updates.
   */
  public void OnCastDevicesUpdated(CastRemoteDisplayManager manager) {
    bool firstTimeCastShown = PlayerPrefs.GetInt(FIRST_TIME_CAST_SHOWN) == 0 ? false : true;
    if (!firstTimeCastShown) {
      HideAll();
      firstTimeCastDialog.Show();
      PlayerPrefs.SetInt(FIRST_TIME_CAST_SHOWN, 1);
    }
    castListDialog.PopulateList(manager);
  }

  /**
   * Closes the list of devices, sets up the casting display.
   */
  public void OnRemoteDisplaySessionStart(CastRemoteDisplayManager manager) {
    isCasting = true;
    HideAll();
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
    errorDialog.SetError(errorCode, errorString);
  }

  /**
   * Callback when the user taps close button.
   */
  public void OnCloseCastList() {
    HideAll();
    castButtonFrame.ShowNotCasting();
  }

  /**
   * Either stop casting or open the list of detected cast devices.
   */
  public void OnCastButtonTapped() {
    if (isCasting) {
      displayManager.StopRemoteDisplaySession();
    } else {
      HideAll();
      if (errorDialog.ErrorCode == CastErrorCode.NoError) {
        castListDialog.Show();
      } else {
        errorDialog.gameObject.SetActive(true);
      }
    }
  }

  /**
   * Called when the error dialog is confirmed.
   */
  public void OnConfirmErrorDialog() {
    HideAll();
    castButtonFrame.ShowNotCasting();
  }

  /**
   * Called when the first time dialog is confirmed.
   */
  public void OnConfirmFirstTimeDialog() {
    HideAll();
    castButtonFrame.ShowNotCasting();
  }
}
