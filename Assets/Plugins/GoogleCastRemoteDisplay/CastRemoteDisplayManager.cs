// Copyright 2015 Google Inc.

using Google.Cast.RemoteDisplay.Internal;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Google.Cast.RemoteDisplay {
  /**
   * Entry point to the Google Cast Remote Display API for Unity. Add only one of these to your
   * scene as a top level game object.
   */
  public class CastRemoteDisplayManager : MonoBehaviour {

    /**
     * Fired when the list of available cast devices has been updated. Call #GetCastDevices on the
     * CastRemoteDisplayManager to get the actual list.
     */
    public CastRemoteDisplayEvent CastDevicesUpdatedEvent;

    /**
     * Fired when the remote display session starts. Call #GetSelectedDevice on the
     * CastRemoteDisplayManager to get the name of the selected cast device.
     */
    public CastRemoteDisplayEvent RemoteDisplaySessionStartEvent;

    /**
     * Fired when the remote display session ends.
     */
    public CastRemoteDisplayEvent RemoteDisplaySessionEndEvent;


    /**
     * Fired when the remote display session encounters an error. When this event is fired, the
     * game object that owns this component will be disabled. Call #GetLastError to get information
     * about the error that was fired.
     */
    public CastRemoteDisplayEvent RemoteDisplayErrorEvent;

    /**
     * There should only be one DisplayManager in the scene at a time, this instance enforces that.
     */
    private static CastRemoteDisplayManager instance = null;
    public static CastRemoteDisplayManager GetInstance() {
      return instance;
    }

    /**
     * Used to render graphics on the remote display. Only used if #RemoteDisplayTexture is not
     * set.
     */
    [SerializeField]
    private Camera remoteDisplayCamera;
    public Camera RemoteDisplayCamera {
      get {
        return remoteDisplayCamera;
      }
      set {
        // Needed because the unity editor can call this multiple times.
        if (value == remoteDisplayCamera) {
          return;
        }
        remoteDisplayCamera = value;
        if (extensionManager != null) {
          // Must be called after the new value has been set.
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
        // Needed because the unity editor can call this multiple times.
        if (value == remoteDisplayTexture) {
          return;
        }
        remoteDisplayTexture = value;
        if (extensionManager != null) {
          // Must be called after the new value has been set.
          extensionManager.UpdateRemoteDisplayTexture();
        }
      }
    }

    /**
     * Used to render graphics on the remote display when the application is paused or
     * backgrounded.
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
    private AudioListener remoteAudioListener;
    public AudioListener RemoteAudioListener {
      get {
        return remoteAudioListener;
      }
      set {
        if (extensionManager) {
          extensionManager.UpdateAudioListener(remoteAudioListener, value);
        }
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
        Debug.LogWarning("Second CastRemoteDisplayManager detected - destroying. Please make " +
          "sure the appropriate configuration gets migrated to the singleton DisplayManager if " +
          "this is intended.");
        DestroyImmediate(gameObject);
        return;
      } else {
        instance = this;
        DontDestroyOnLoad(gameObject);
      }

      extensionManager = gameObject.AddComponent<CastRemoteDisplayExtensionManager>();
      // Notify the extension manager that a new audio listener was added so that it can attach the
      // required script to it.
      extensionManager.UpdateAudioListener(null, remoteAudioListener);
      extensionManager.SetEventHandlers(fireCastDevicesUpdatedEvent,
          fireRemoteDisplaySessionStartEvent, fireRemoteDisplaySessionEndEvent,
          fireErrorEvent);
    }

    /**
     * Selects a cast device for remote display.
     */
    public void SelectCastDevice(string deviceId) {
      extensionManager.SelectCastDevice(deviceId);
    }

    /**
     * Returns the list of available cast devices for remote display.
     */
    public IList<CastDevice> GetCastDevices() {
      return extensionManager.GetCastDevices().AsReadOnly();
    }

    /**
     * Returns the CastDevice selected for remote display.
     */
    public CastDevice GetSelectedCastDevice() {
      return extensionManager.GetSelectedCastDevice();
    }

    /**
     * Returns whether there is an active cast session. This will be set to true from the moment
     * the #RemoteDisplaySessionStartEvent fires and until the session ends.
     */
    public bool IsCasting() {
      return extensionManager.IsCasting();
    }

    /**
     * Stops the current remote display session. This can be used in the middle of the game to let
     * the user stop and disconnect and later select another Cast device.
     */
    public void StopRemoteDisplaySession() {
      extensionManager.StopRemoteDisplaySession();
    }

    /**
     * Returns the last error encountered by the Cast Remote Display Plugin, or null if no error
     * has occurred.
     */
    public CastError GetLastError() {
      return extensionManager.GetLastError();
    }

    /**
     * Used to allow internal classes to fire events published by this class.
     */
    private void fireCastDevicesUpdatedEvent() {
      if (CastDevicesUpdatedEvent != null) {
        CastDevicesUpdatedEvent.Invoke(this);
      }
    }

    /**
     * Used to allow internal classes to fire events published by this class.
     */
    private void fireRemoteDisplaySessionStartEvent() {
      if (RemoteDisplaySessionStartEvent != null) {
        RemoteDisplaySessionStartEvent.Invoke(this);
      }
    }

    /**
     * Used to allow internal classes to fire events published by this class.
     */
    private void fireRemoteDisplaySessionEndEvent() {
      if (RemoteDisplaySessionEndEvent != null) {
        RemoteDisplaySessionEndEvent.Invoke(this);
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
      Debug.LogError("Remote display error. ErrorCode: " + error.ErrorCode +
          " errorMessage: " + error.Message);

      // Always disable the manager in case of an error.
      gameObject.SetActive(false);

      if (RemoteDisplayErrorEvent != null) {
        RemoteDisplayErrorEvent.Invoke(this);
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
    Normal = 100,

    /**
     * Specifies a high target delay for the session. Remote display will have more time to encode,
     * transmit, retransmit, decode, and render on the TV. This is useful for smoother rendering on
     * the TV at the cost of higher latency and more memory to buffer and encode.
     */
    High = 400
  };

  /**
   * Represents a cast device.
   */
  [System.Serializable]
  public class CastDevice {
    /**
     * The ID of the device. This value must be passed when selecting a cast device to start the
     * remote display session.
     */
    [SerializeField]
    private string deviceId;
    public string DeviceId {
      get {
        return deviceId;
      }
    }

    /**
     * Name of the device. This should be used when populating a list of devices in the UI.
     */
    [SerializeField]
    private string deviceName;
    public string DeviceName {
      get {
        return deviceName;
      }
    }

    /**
     * The current status of the device.
     */
    [SerializeField]
    private string status;
    public string Status {
      get {
        return status;
      }
    }

    private CastDevice() {}

    /**
     * Constructor for CastDevice.
     */
    public CastDevice(string deviceId, string deviceName, string status) {
      this.deviceId = deviceId;
      this.deviceName = deviceName;
      this.status = status;
    }
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
    private CastErrorCode errorCode;
    public CastErrorCode ErrorCode {
      get {
        return errorCode;
      }
    }

    /**
     * Name of the device. This should be used when populating a list of devices in the UI.
     */
    private string message;
    public string Message {
      get {
        return message;
      }
    }

    public string ReadableErrorTitle {
      get {
        switch (errorCode) {
          case CastErrorCode.RemoteDisplayUnsupported:
            return "Remote Display Unsupported";
          case CastErrorCode.GooglePlayServicesUpdateRequired:
            return "Play Services Update Required";
          case CastErrorCode.RemoteDisplayConfigurationRejected:
            return "Remote Display Config Rejected";
          default:
            return "Unknown Error";
        }
      }
    }

    public string ReadableErrorBody {
      get {
        switch (errorCode) {
          case CastErrorCode.RemoteDisplayUnsupported:
            return "Your mobile device is not supported for this game. Please play using any of " +
                "the following:\n\nAndroid 4.4+\niOS 8+ and (iPad Mini 2+, iPad 3+, iPhone 5+)";
          case CastErrorCode.GooglePlayServicesUpdateRequired:
            return "Google Play Services requires an update.";
          case CastErrorCode.RemoteDisplayConfigurationRejected:
            return "Remote display configuration rejected by Cast device.";
          default:
            return "Unknown Error";
        }
      }
    }

    private CastError() {}

    /**
     * Constructor for CastError.
     */
    public CastError(CastErrorCode errorCode, string message) {
      this.errorCode = errorCode;
      this.message = message;
    }
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
    private CastRemoteDisplayResolution resolution;
    public CastRemoteDisplayResolution Resolution {
      get {
        return resolution;
      }
    }

    /**
     * The target framerate to use for the remote display session. Slower devices and bandwidth
     * conditions might slow down actual framerate on the TV.
     * This field is iOS-only. Ignored by Android and Unity simulator.
     */
    [Tooltip("iOS-only. Ignored by Android and Unity simulator.")]
    private CastRemoteDisplayFrameRate frameRate;
    public CastRemoteDisplayFrameRate FrameRate {
      get {
        return frameRate;
      }
    }

    /**
     * The target delay to use for the remote display session. Lower target delays might improve
     * latency at the expense of visual and audio quality since there will be less time to encode,
     * transmit, retransmit, decode, and render on the TV.
     * This field is iOS-only. Ignored by Android and Unity simulator.
     */
    [Tooltip("iOS-only. Ignored by Android and Unity simulator.")]
    private CastRemoteDisplayTargetDelay targetDelay;
    public CastRemoteDisplayTargetDelay TargetDelay {
      get {
        return targetDelay;
      }
    }

    /**
     * Whether to disable adaptive video bitrate. If true, use a fixed bitrate set to
     * 3 Mbps. The default is false.
     * This is an experimental feature.
     * This field is iOS-only. Ignored by Android and Unity simulator.
     */
    [Tooltip("Experimental and iOS-only. Ignored by Android and Unity simulator.")]
    private bool disableAdaptiveVideoBitrate;
    public bool DisableAdaptiveVideoBitrate {
      get {
        return disableAdaptiveVideoBitrate;
      }
    }

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
      frameRate = CastRemoteDisplayFrameRate.Fps60;
      resolution = CastRemoteDisplayResolution.Resolution720p;
      targetDelay = CastRemoteDisplayTargetDelay.Normal;
      disableAdaptiveVideoBitrate = false;
    }
  }

  /**
   * Used to allow the events we fire to be serializable in the inspector.
   */
  [System.Serializable]
  public class CastRemoteDisplayEvent : UnityEvent<CastRemoteDisplayManager> { }
}
