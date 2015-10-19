// Copyright 2015 Google Inc.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Google.Cast.RemoteDisplay.Internal {
  public class CastRemoteDisplayExtensionManager : MonoBehaviour {

    // We need this in order to tell the CastRemoteDisplayManager to fire events for us.
    public delegate void CastEventHandler();

    public CastRemoteDisplayManager CastRemoteDisplayManager {
      get {
        return CastRemoteDisplayManager.GetInstance();
      }
    }

    private CastEventHandler onCastDevicesUpdatedCallback;
    private CastEventHandler onRemoteDisplaySessionStartCallback;
    private CastEventHandler onRemoteDisplaySessionEndCallback;
    private CastEventHandler onErrorCallback;

    private bool isApplicationPaused = false;
    private bool isCasting = false;

    private List<CastDevice> castDevices = new List<CastDevice>();
    private CastError lastError = null;
    private ICastRemoteDisplayExtension castRemoteDisplayExtension = null;

    // When Unity specifies a camera but no render texture, we must generate one and assign it.
    // When casting is complete, we must reclaim it - this bool tracks that possibility.
    private bool renderTextureGenerated = false;

    // True if remote display texture was updated by #UpdateRemoteDisplayTexture should be rendered
    // if possible.
    private bool remoteDisplayTextureUpdated = false;

    /**
     * Sets the event handlers that should be invoked by this class.
     */
    public void SetEventHandlers(CastEventHandler onCastDevicesUpdatedCallback,
        CastEventHandler onRemoteDisplaySessionStartCallback,
        CastEventHandler onRemoteDisplaySessionEndCallback,
        CastEventHandler onErrorCallback) {
      this.onCastDevicesUpdatedCallback = onCastDevicesUpdatedCallback;
      this.onRemoteDisplaySessionStartCallback = onRemoteDisplaySessionStartCallback;
      this.onRemoteDisplaySessionEndCallback = onRemoteDisplaySessionEndCallback;
      this.onErrorCallback = onErrorCallback;
    }

    /**
     * Unity callback pre-scene initialization.
     */
    void Awake() {
      // Do not initialize or perform checks yet, but if there is a remote display camera disable
      // it so that it doesn't become the main camera.
      if (CastRemoteDisplayManager != null &&
          CastRemoteDisplayManager.RemoteDisplayCamera != null) {
        CastRemoteDisplayManager.RemoteDisplayCamera.enabled = false;
      }
    }

    /**
     * Unity callback for when this game object is enabled. Called after Awake and before Start on
     * scene load.
     */
    void OnEnable() {
      Activate();
    }

    /**
     * Unity callback for when this object is disabled.
     */
    void OnDisable() {
      Deactivate();
    }

    /**
     * Unity callback for when the app gets paused/backgrounded, and resumed/foregrounded.
     */
    void OnApplicationPause(bool paused) {
      isApplicationPaused = paused;
      DiscardGeneratedTexture();
      UpdateRemoteDisplayTexture();
    }

    /**
     * Matches the Unity callback of the same name, but this is actually called by the
     * CastRemoteDisplayAudioListener. This is due to the fact that this callback should come from
     * the game object with the AudioListener script.
     * Note: On iOS, this callback won't trigger unless the AudioListener was attached to the game
     * object since scene load, that is why we need a custom script.
     */
    public void OnAudioFilterRead(float[] data, int channels) {
      if (castRemoteDisplayExtension != null) {
        castRemoteDisplayExtension.OnAudioFilterRead(data, channels);
      }
    }

    /**
     * Returns the list of cast devices.
     */
    public List<CastDevice> GetCastDevices() {
      return castDevices;
    }

    /**
     * Returns the last error encountered by the Cast Remote Display Plugin, or null if no error
     * has occurred.
     */
    public CastError GetLastError() {
      return lastError;
    }

    /**
     * Starts a remote display session on the cast device with the specified device ID.
     */
    public void SelectCastDevice(string deviceId) {
      if (castRemoteDisplayExtension != null) {
        castRemoteDisplayExtension.SelectCastDevice(deviceId);
      }
    }

    /**
     * Returns the device ID of of the receiver device this sender is currently casting to.
     */
    public string GetSelectedCastDeviceId() {
      if (castRemoteDisplayExtension != null) {
        return castRemoteDisplayExtension.GetSelectedCastDeviceId();
      }
      return null;
    }

    /**
     * Returns whether there is an active cast session.
     */
    public bool IsCasting() {
      return isCasting;
    }

    /**
     * Stops the current remote display session. This can be used in the middle of the game to let
     * the user stop and disconnect and later select another Cast device.
     */
    public void StopRemoteDisplaySession() {
      if (GetSelectedCastDeviceId() != null) {
        DiscardGeneratedTexture();
        castRemoteDisplayExtension.StopRemoteDisplaySession();
      }
    }

    /**
     * Notifies the static library that a new texture id should be used for remote display
     * rendering. It will select the #RemoteDisplayTexture property of the
     * #CastRemoteDisplayManager, if not set it will try to use the #RemoteDisplayCamera. If the
     * application is paused and the #RemoteDisplayPausedTexture is set, it will be selected.
     */
    public void UpdateRemoteDisplayTexture() {
      if (castRemoteDisplayExtension == null) {
        Debug.Log("Remote display is not activated. Can't update remote display texture or " +
            "camera.");
        return;
      }

      Texture texture = null;
      CastRemoteDisplayManager manager = CastRemoteDisplayManager;
      renderTextureGenerated = false;

      if (isApplicationPaused && manager.RemoteDisplayPausedTexture != null) {
        // Application paused and we have a paused texture to display.
        texture = manager.RemoteDisplayPausedTexture;
      } else {
        if (manager.RemoteDisplayTexture != null) {
          // The user explicitly set a texture to use.
          texture = manager.RemoteDisplayTexture;
        } else if (manager.RemoteDisplayCamera != null) {
          // We are using a camera. If it has a target render texture set, use that.
          manager.RemoteDisplayCamera.enabled = true;
          manager.RemoteDisplayCamera.gameObject.SetActive(true);
          if (manager.RemoteDisplayCamera.targetTexture == null) {
            // Otherwise create our own.
            RenderTexture renderTexture = new RenderTexture(
                (int) manager.Configuration.ResolutionDimensions.x,
                (int) manager.Configuration.ResolutionDimensions.y,
                24, RenderTextureFormat.ARGB32);
            manager.RemoteDisplayCamera.targetTexture = renderTexture;
            renderTextureGenerated = true;
          }
          if (!manager.RemoteDisplayCamera.targetTexture.IsCreated()) {
            manager.RemoteDisplayCamera.targetTexture.Create();
          }
          texture = manager.RemoteDisplayCamera.targetTexture;
        }
      }

      if (texture == null) {
        Debug.LogError("No texture or camera set. Can't cast to remote display.");
        return;
      }
      castRemoteDisplayExtension.SetRemoteDisplayTexture(texture);
      remoteDisplayTextureUpdated = true;
    }

    /**
     * Sets everything up and starts discovery on the native extension.
     */
    private void Activate() {
      Debug.Log("Activating Cast Remote Display.");
      if (CastRemoteDisplayManager == null) {
        Debug.LogError("FATAL: CastRemoteDisplayManager script not found.");
        return;
      }

      if (CastRemoteDisplayManager.CastAppId == null ||
          CastRemoteDisplayManager.CastAppId.Equals("")) {
        Debug.LogError("FATAL: CastRemoteDisplayManager needs a CastAppId");
        return;
      }

      if (CastRemoteDisplayManager.Configuration == null) {
        Debug.LogError("FATAL: CastRemoteDisplayManager has null configuration");
        return;
      }

      if (CastRemoteDisplayManager.RemoteAudioListener != null) {
        CastRemoteDisplayManager.RemoteAudioListener.setCastRemoteDisplayExtensionManager(this);
      }

#if UNITY_ANDROID && !UNITY_EDITOR
    castRemoteDisplayExtension = new CastRemoteDisplayAndroidExtension(this);
#elif UNITY_IOS && !UNITY_EDITOR
    castRemoteDisplayExtension = new CastRemoteDisplayiOSExtension(this);
#elif UNITY_EDITOR
      CastRemoteDisplaySimulator displaySimulator =
        UnityEngine.Object.FindObjectOfType<CastRemoteDisplaySimulator>();
      if (displaySimulator) {
        castRemoteDisplayExtension = new CastRemoteDisplayUnityExtension(this, displaySimulator);
      }
#endif

      if (castRemoteDisplayExtension != null) {
        castRemoteDisplayExtension.Activate();
#if !UNITY_IOS
        // Non iOS platforms can render using RenderRemoteDisplayCoroutine.
        StartCoroutine(RenderRemoteDisplayCoroutine());
#endif
      } else {
        Debug.LogWarning("Disabling the CastRemoteDisplayManager because the platform is not " +
                       "Android or iOS, and no simulator is found.");
      }
    }

    /**
     * Terminates the current remote display session (if any) and stops discovery on the native
     * extension.
     */
    private void Deactivate() {
      Debug.Log("Deactivating Cast Remote Display.");
      if (GetSelectedCastDeviceId() != null) {
        StopRemoteDisplaySession();
      }

      DiscardGeneratedTexture();

      if (castRemoteDisplayExtension != null) {
        castRemoteDisplayExtension.Deactivate();
      }
      StopAllCoroutines();
      castRemoteDisplayExtension = null;
      castDevices.Clear();
    }

    /**
     * Callback called from the native plugins when the remote display session starts.
     */
    public void _callback_OnRemoteDisplaySessionStart(string deviceId) {
      if (castRemoteDisplayExtension != null) {
        Debug.Log("Remote display session started.");
        castRemoteDisplayExtension.OnRemoteDisplaySessionStart(deviceId);
      }

      UpdateRemoteDisplayTexture();
      isCasting = true;
      onRemoteDisplaySessionStartCallback();
    }

    /**
     * Callback called from the native plugins when the remote display session ends.
     */
    public void _callback_OnRemoteDisplaySessionEnd(string dummy) {
      if (castRemoteDisplayExtension != null) {
        Debug.Log("Remote display session stopped.");
        castRemoteDisplayExtension.OnRemoteDisplaySessionStop();
      }
      isCasting = false;
      onRemoteDisplaySessionEndCallback();
    }

    /**
     * Callback called from the native plugins when the list of available cast devices has changed.
     */
    public void _callback_OnCastDevicesUpdated(string dummy) {
      UpdateCastDevicesFromNativeCode();

      onCastDevicesUpdatedCallback();
    }

    /**
     * Callback called from the native plugins when the SDK throws an error.
     */
    public void _callback_OnCastError(string rawErrorString) {
      string errorString = extractErrorString(rawErrorString);
      CastErrorCode errorCode = extractErrorCode(rawErrorString);
      lastError = new CastError { errorCode = errorCode, message = errorString };
      isCasting = false;
      onErrorCallback();
    }

    /**
     * Pulls the error code out of the raw error string that gets sent from the SDK.
     */
    private CastErrorCode extractErrorCode(string rawErrorString) {
      string[] splitString = rawErrorString.Split(':');
      int errorInt = -1;
      bool foundCode = Int32.TryParse(splitString[0], out errorInt);
      if (splitString.Length < 2 || !foundCode) {
        Debug.LogError("Error string malformed: " + rawErrorString);
        return CastErrorCode.ErrorCodeMalformed;
      } else {
        return (CastErrorCode) errorInt;
      }
    }

    /**
     * Pulls the error string out of the raw error string that gets sent from the SDK.
     */
    private string extractErrorString(string rawErrorString) {
      string[] splitString = rawErrorString.Split(':');
      if (splitString.Length < 2) {
        Debug.LogError("Error string malformed: " + rawErrorString);
        return "Error string malformed: " + rawErrorString;
      } else {
        return splitString[1];
      }
    }

    private void UpdateCastDevicesFromNativeCode() {
      castDevices.Clear();
      if (castRemoteDisplayExtension != null) {
        castDevices = castRemoteDisplayExtension.GetCastDevices();
      } else {
        Debug.Log("Can't update the list of cast devices because there is no extension object.");
      }
    }

    private void DiscardGeneratedTexture() {
      var manager = CastRemoteDisplayManager;
      if (manager.RemoteDisplayCamera != null) {
        manager.RemoteDisplayCamera.enabled = false;
        if (renderTextureGenerated) {
          if (manager.RemoteDisplayCamera.targetTexture != null) {
            manager.RemoteDisplayCamera.targetTexture.DiscardContents();
            manager.RemoteDisplayCamera.targetTexture = null;
          }
          renderTextureGenerated = false;
        }
      }
    }

    private void MaybeRenderFrame() {
      if (castRemoteDisplayExtension != null
          && (!isApplicationPaused || remoteDisplayTextureUpdated) && isCasting) {
        castRemoteDisplayExtension.RenderFrame();
        remoteDisplayTextureUpdated = false;
      }
    }

#if UNITY_IOS
    /**
     * iOS Metal graphics API will only render in Update()..
     */
    private void Update() {
      MaybeRenderFrame();
    }
#else
    /**
     * Coroutine that will call render on the native plugin at the end of every frame.
     */
    private IEnumerator RenderRemoteDisplayCoroutine() {
      var waitObject = new WaitForEndOfFrame();
      while (gameObject.activeInHierarchy && enabled) {
        yield return waitObject;
        MaybeRenderFrame();
      }
      yield return 0;
    }
#endif
  }
}
