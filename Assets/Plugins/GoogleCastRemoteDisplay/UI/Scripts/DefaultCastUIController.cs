using UnityEngine;

/**
 * Controller class for the default cast UI.
 */
public class DefaultCastUIController : MonoBehaviour {

  /**
   * Outlet for the display manager.
   */
  public CastRemoteDisplayManager remoteDisplayManager;

  /**
   * Outlet for the cast debug UI element.
   */
  private CastDefaultUI defaultUI;

  /**
   * Enforces the uniqueness of the DefaultCastUIController across scenes.
   */
  static private DefaultCastUIController instance;

  /**
   * If a default UI controller already exists, destroy the new one.
   */
  void Awake() {
    if (instance) {
      Debug.LogWarning("DebugCastUIController: Duplicate UI controller found - destroying.");
      DestroyImmediate(gameObject);
      return;
    } else {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  /**
   * Finds the display manager automatically.
   */
  void Start() {
    if (instance != this) {
      return;
    }

    if (!remoteDisplayManager) {
      remoteDisplayManager = Object.FindObjectOfType<CastRemoteDisplayManager>();
    }

    if (!remoteDisplayManager) {
      Debug.LogError("DebugCastUIController ERROR: No CastRemoteDisplayManager found!");
      Destroy(gameObject);
      return;
    }

    gameObject.transform.localScale = Vector3.one;
    defaultUI = gameObject.GetComponentInChildren<CastDefaultUI>();
    defaultUI.Initialize(remoteDisplayManager);
  }

  /**
   * When the UI is enabled, listen to the relevant events for the UI.
   */
  void OnEnable() {
    if (remoteDisplayManager && defaultUI) {
      defaultUI.Initialize(remoteDisplayManager);
    }
  }

  /**
   * When the UI is disabled, stop listening to the relevant events.
   */
  void OnDisable() {
    if (remoteDisplayManager && defaultUI) {
      defaultUI.Uninitialize(remoteDisplayManager);
    }
  }
}
