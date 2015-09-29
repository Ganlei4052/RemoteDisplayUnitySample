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
  public CastRemoteDisplayManager displayManager;

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
      displayManager = UnityEngine.Object.FindObjectOfType<CastRemoteDisplayManager>();
    }

    if (!displayManager) {
      Debug.LogError("DebugCastUIController ERROR: No CastRemoteDisplayManager found!");
      Destroy(gameObject);
      return;
    }
    #if !UNITY_EDITOR
    displayManager.remoteDisplaySessionStartEvent += OnRemoteDisplaySessionStart;
    displayManager.remoteDisplaySessionEndEvent += OnRemoteDisplaySessionEnd;
    displayManager.remoteDisplayErrorEvent += OnRemoteDisplayError;
    #endif
    MainCamera.GetComponent<Camera>().enabled = true;
  }

  /**
   * Cast session started, so change the mobile device camera.
   */
  public void OnRemoteDisplaySessionStart(CastRemoteDisplayManager manager) {
    MainCamera.GetComponent<Camera>().enabled = true;
  }

  /**
   * Cast session ended, so change the mobile device camera.
   */
  public void OnRemoteDisplaySessionEnd(CastRemoteDisplayManager manager) {
    RemoteDisplayCamera.GetComponent<Camera>().enabled = true;
  }

  /**
   * Handles error messages from the Remote Display Manager.
   */
  public void OnRemoteDisplayError(CastRemoteDisplayManager manager,
      CastErrorCode errorCode, string errorString) {
    MainCamera.GetComponent<Camera>().enabled = true;
  }

}
