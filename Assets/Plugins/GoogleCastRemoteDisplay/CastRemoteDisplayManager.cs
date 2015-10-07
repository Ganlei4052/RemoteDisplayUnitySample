// Copyright 2015 Google Inc.

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/**
 * Entry point to the Google Cast Remote Display API for Unity. Add only one of these to your scene
 * as a top level game object.
 */
public class CastRemoteDisplayManager : MonoBehaviour {

  /**
   * Fired when the list of available cast devices has been updated. Call #GetCastDevices on the
   * CastRemoteDisplayManager to get the actual list.
   */
  public CastRemoteDisplayEvent castDevicesUpdatedEvent;

  /**
   * Fired when the remote display session starts. Call #GetSelectedDevice on the
   * CastRemoteDisplayManager to get the name of the selected cast device.
   */
  public CastRemoteDisplayEvent remoteDisplaySessionStartEvent;

  /**
   * Fired when the remote display session ends.
   */
  public CastRemoteDisplayEvent remoteDisplaySessionEndEvent;


  /**
   * Fired when the remote display session encounters an error. When this event is fired, the game
   * object that owns this component will be disabled. Call #GetLastError to get information about
   * the error that was fired.
   */
  public CastRemoteDisplayEvent remoteDisplayErrorEvent;


  /**
   * There should only be one DisplayManager in the scene at a time, this instance enforces that.
   */
  private static CastRemoteDisplayManager instance = null;
  public static CastRemoteDisplayManager GetInstance() {
    return instance;
  }

  /**
   * Used to render graphics on the remote display. Only used if #RemoteDisplayTexture is not set.
   */
  [SerializeField]
  private Camera remoteDisplayCamera;
  public Camera RemoteDisplayCamera {
    get {
      return remoteDisplayCamera;
    }
    set {
      remoteDisplayCamera = value;
      if (extensionManager != null) {
        extensionManager.UpdateRemoteDisplayTexture();
      }
    }
  }

  [SerializeField]
  private Texture remoteDisplayTexture;
  public Texture RemoteDisplayTexture {
    get {
      return remoteDisplayTexture;
    }
    set {
      remoteDisplayTexture = value;
      if (extensionManager != null) {
        extensionManager.UpdateRemoteDisplayTexture();
      }
    }
  }

  /**
   * Used to render graphics on the remote display when the application is paused or backgrounded.
   */
  [SerializeField]
  private Texture remoteDisplayPausedTexture;
  public Texture RemoteDisplayPausedTexture {
    get {
      return remoteDisplayPausedTexture;
    }
    set {
      remoteDisplayPausedTexture = value;
    }
  }

  /**
   * Used to play audio on the remote display.
   */
  [SerializeField]
  private CastRemoteDisplayAudioListener remoteAudioListener;
  public CastRemoteDisplayAudioListener RemoteAudioListener {
    get {
      return remoteAudioListener;
    }
    set {
      remoteAudioListener = value;
    }
  }

  /**
   * The remote display application ID.
   */
  [SerializeField]
  private string castAppId = "";
  public string CastAppId {
    get {
      return castAppId;
    }
    set {
      castAppId = value;
    }
  }

  /**
   * The configuration to use to set up a remote display session when a cast
   * device is selected. See #SelectCastDevice.
   */
  [SerializeField]
  private CastRemoteDisplayConfiguration configuration = new CastRemoteDisplayConfiguration();
  public CastRemoteDisplayConfiguration Configuration {
    get {
      return configuration;
    }
    set {
      if (value == null) {
        throw new System.ArgumentException("Configuration cannot be set to null");
      }
      configuration = value;
    }
  }

  private CastRemoteDisplayExtensionManager extensionManager;

  private void Awake() {
    if (instance && instance != this) {
      Debug.LogWarning("Second CastRemoteDisplayManager detected - destroying. Please make sure" +
        "the appropriate configuration gets migrated to the singleton DisplayManager if this is " +
        "intended.");
      DestroyImmediate(gameObject);
      return;
    } else {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }

    extensionManager = gameObject.AddComponent<CastRemoteDisplayExtensionManager>();
    extensionManager.SetEventHandlers(fireCastDevicesUpdatedEvent,
        fireRemoteDisplaySessionStartEvent, fireRemoteDisplaySessionEndEvent,
        fireErrorEvent);
  }

  /**
   * Returns the list of available cast devices for remote display.
   */
  public List<CastDevice> GetCastDevices() {
    return extensionManager.GetCastDevices();
  }

  /**
   * Selects a cast device for remote display.
   */
  public void SelectCastDevice(string deviceId) {
    extensionManager.SelectCastDevice(deviceId);
  }

  /**
   * Returns the ID of the selected cast device for remote display.
   */
  public string GetSelectedCastDeviceId() {
    return extensionManager.GetSelectedCastDeviceId();
  }

  /**
   * Stops the current remote display session. This can be used in the middle of the game to let the
   * user stop and disconnect and later select another Cast device.
   */
  public void StopRemoteDisplaySession() {
    extensionManager.StopRemoteDisplaySession();
  }

  /**
   * Returns the last error encountered by the Cast Remote Display Plugin, or null if no error has
   * occurred.
   */
  public CastError GetLastError() {
    return extensionManager.GetLastError();
  }

  /**
   * Used to allow internal classes to fire events published by this class.
   */
  private void fireCastDevicesUpdatedEvent() {
    if (castDevicesUpdatedEvent != null) {
      castDevicesUpdatedEvent.Invoke(this);
    }
  }

  /**
   * Used to allow internal classes to fire events published by this class.
   */
  private void fireRemoteDisplaySessionStartEvent() {
    if (remoteDisplaySessionStartEvent != null) {
      remoteDisplaySessionStartEvent.Invoke(this);
    }
  }

  /**
   * Used to allow internal classes to fire events published by this class.
   */
  private void fireRemoteDisplaySessionEndEvent() {
    if (remoteDisplaySessionEndEvent != null) {
      remoteDisplaySessionEndEvent.Invoke(this);
    }
  }

  /**
   * Used to allow internal classes to fire events published by this class.
   */
  private void fireErrorEvent() {
    CastError error = GetLastError();
    if (error == null) {
      Debug.LogError("Got an error callback but no error was found");
      return;
    }
    Debug.LogError("Remote display error. ErrorCode: " + error.errorCode +
        " errorMessage: " + error.message);

    // Always disable the manager in case of an error.
    gameObject.SetActive(false);

    if (remoteDisplayErrorEvent != null) {
      remoteDisplayErrorEvent.Invoke(this);
    }
  }
}

/**
 * Error codes for Cast Remote Display.
 */
public enum CastErrorCode {
  /**
   * The default error code when there are no errors.
   */
  NoError = 0,

  /**
   * Thrown when the device OS is unsupported. Remote Display requires a minimum of iOS 8 or
   * Android KitKat. (4.4)
   */
  RemoteDisplayUnsupported = 1,

  /**
   * Thrown when the version of Google Play Services needs to be updated.
   */
  GooglePlayServicesUpdateRequired = 2,

  /**
   * Thrown when the Remote Display configuration was rejected by the cast device.
   */
  RemoteDisplayConfigurationRejected = 3,

  /**
   * Thrown when the error message is malformed. Indicates an internal Cast error.
   */
  ErrorCodeMalformed = 1000,
};

/**
 * Frame rates supported by a remote display session.
 */
public enum CastRemoteDisplayFrameRate {
  /**
   * Specifies 15 frames per second for the session.
   */
  Fps15 = 15,

  /**
   * Specifies 24 frames per second for the session.
   */
  Fps24 = 24,

  /**
   * Specifies 25 frames per second for the session.
   */
  Fps25 = 25,

  /**
   * Specifies 30 frames per second for the session.
   */
  Fps30 = 30,

  /**
   * Specifies 60 frames per second for the session.
   */
  Fps60 = 60,
};

/**
 * TV resolutions supported by a remote display session.
 */
public enum CastRemoteDisplayResolution {
  /**
   * Specifies 848x480 for the session.
   */
  Resolution480p = 480,

  /**
   * Specifies 1280x720 for the session.
   */
  Resolution720p = 720,

  /**
   * Specifies 1920x1080 for the session.
   */
  Resolution1080p = 1080,
};

/**
 * Target delays supported by a remote display session. See #CastRemoteDisplayConfiguration for
 * details about using target delay.
 */
public enum CastRemoteDisplayTargetDelay {
  /**
   * Specifies the minimum target delay for the session. Remote display will have the least time
   * to encode, transmit, retransmit, decode, and render on the TV.
   */
  Minimum = 25,

  /**
   * Specifies a low target delay for the session. Remote display will have less time to encode,
   * transmit, retransmit, decode, and render on the TV.
   */
  Low = 50,

  /**
   * Specifies a normal target delay for the session. Remote display is optimized for this delay.
   */
  Normal = 100
};

/**
 * Represents a cast device.
 */
[System.Serializable]
public struct CastDevice {
  /**
   * The ID of the device. This value must be passed when selecting a cast device to start the
   * remote display session.
   */
  public string deviceId;

  /**
   * Name of the device. This should be used when populating a list of devices in the UI.
   */
  public string deviceName;

  /**
   * The current status of the device.
   */
  public string status;
}

/**
 * Represents an error that triggered by the Cast Remote Display Plugin.
 */
[System.Serializable]
public class CastError {
  /**
   * The ID of the device. This value must be passed when selecting a cast device to start the
   * remote display session.
   */
  public CastErrorCode errorCode;

  /**
   * Name of the device. This should be used when populating a list of devices in the UI.
   */
  public string message;
}

/**
 * Specifies the remote display configuration to set up a session.
 */
[System.Serializable]
public class CastRemoteDisplayConfiguration {
  /**
   * The TV resolution to use for the remote display session. Higher the resolution, the more
   * bandwidth required.
   * This field is fully supported on iOS. Android and Unity simulator use this to change the
   * camera texture resolution - using a custom render texture instead of a camera will not use
   * the resolution set here.
   */
  [Tooltip("iOS fully supported. Changes camera resolution on Android and Unity simulator.")]
  public CastRemoteDisplayResolution resolution;

  /**
   * The target framerate to use for the remote display session. Slower devices and bandwidth
   * conditions might slow down actual framerate on the TV.
   * This field is iOS-only. Ignored by Android and Unity simulator.
   */
  [Tooltip("iOS-only. Ignored by Android and Unity simulator.")]
  public CastRemoteDisplayFrameRate frameRate;

  /**
   * The target delay to use for the remote display session. Lower target delays might improve
   * latency at the expense of visual and audio quality since there will be less time to encode,
   * transmit, retransmit, decode, and render on the TV.
   * This field is iOS-only. Ignored by Android and Unity simulator.
   */
  [Tooltip("iOS-only. Ignored by Android and Unity simulator.")]
  public CastRemoteDisplayTargetDelay targetDelay;

  /**
   * The audio bitrate to use for the remote display session. Higher the bitrate, more audio
   * quality but more bandwidth required.
   * This field is iOS-only. Ignored by Android and Unity simulator.
   */
  [Tooltip("iOS-only. Ignored by Android and Unity simulator.")]
  public uint audioBitrate;

  /**
   * The video bitrate to use for the remote display session. Higher the bitrate, more video
   * quality but more bandwidth required.
   * This field is iOS-only. Ignored by Android and Unity simulator.
   */
  [Tooltip("iOS-only. Ignored by Android and Unity simulator.")]
  public uint videoBitrate;

  private static readonly Dictionary<CastRemoteDisplayResolution, Vector2> resolutionMap =
    new Dictionary<CastRemoteDisplayResolution, Vector2> {
      { CastRemoteDisplayResolution.Resolution480p, new Vector2(848, 480) },
      { CastRemoteDisplayResolution.Resolution720p, new Vector2(1280, 720) },
      { CastRemoteDisplayResolution.Resolution1080p, new Vector2(1920, 1080) }
    };

  /**
   * Returns the current #resolution width and height as a 2-D vector.
   */
  public Vector2 ResolutionDimensions {
    get {
      return resolutionMap[resolution];
    }
  }

  /**
   * Creates default remote display configuration to set up a session.
   */
  public CastRemoteDisplayConfiguration() {
    audioBitrate = 128000;
    frameRate = CastRemoteDisplayFrameRate.Fps60;
    resolution = CastRemoteDisplayResolution.Resolution720p;
    targetDelay = CastRemoteDisplayTargetDelay.Normal;
    videoBitrate = 3000000;
  }
}

/**
 * Used to allow the events we fire to be serializable in the inspector.
 */
[System.Serializable]
public class CastRemoteDisplayEvent : UnityEvent<CastRemoteDisplayManager> { }
