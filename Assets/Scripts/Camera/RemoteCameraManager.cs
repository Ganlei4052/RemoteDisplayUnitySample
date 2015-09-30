using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Manages the camera used for the mobile and the remote Cast device.
 * The mobile camera will toggle when casting.
 */
public class RemoteCameraManager : MonoBehaviour {

  /**
   * Reference to the display manager.
   */
  private CastRemoteDisplayManager displayManager;

  /**
   * Used to render graphics on the mobile display.
   */
  public Camera RemoteDisplayCamera;

  /**
   * Used to render graphics on the remote display.
   */
  public Camera MainCamera;

  /**
  * Listen to the CastRemoteDisplayManager events.
  */
  void Start() {
    if (!displayManager) {
      displayManager = CastRemoteDisplayManager.GetInstance();
    }

    if (!displayManager) {
      Debug.LogError("DebugCastUIController ERROR: No CastRemoteDisplayManager found!");
      Destroy(gameObject);
      return;
    }
    displayManager.remoteDisplaySessionStartEvent += OnRemoteDisplaySessionStart;
    displayManager.remoteDisplaySessionEndEvent += OnRemoteDisplaySessionEnd;
    displayManager.remoteDisplayErrorEvent += OnRemoteDisplayError;
    RemoteDisplayCamera.enabled = false;
    MainCamera.enabled = true;
  }

  /**
   * Cast session started, so change the mobile device camera.
   */
  public void OnRemoteDisplaySessionStart(CastRemoteDisplayManager manager) {
    displayManager.RemoteDisplayCamera = MainCamera;
    RemoteDisplayCamera.enabled = true;
  }

  /**
   * Cast session ended, so change the mobile device camera.
   */
  public void OnRemoteDisplaySessionEnd(CastRemoteDisplayManager manager) {
    RemoteDisplayCamera.enabled = false;
    MainCamera.enabled = true;
  }

  /**
   * Handles error messages from the Remote Display Manager.
   */
  public void OnRemoteDisplayError(CastRemoteDisplayManager manager,
      CastErrorCode errorCode, string errorString) {
    RemoteDisplayCamera.enabled = false;
    MainCamera.enabled = true;
  }

}
