using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * Controller class for the default cast UI.
 */
public class DefaultCastUIController : MonoBehaviour {

  /**
   * Outlet for the display manager.
   */
  public CastRemoteDisplayManager displayManager;

  /**
   * Outlet for the cast debug UI element.
   */
  private CastDefaultUI defaultUI;

  /**
   * Finds the display manager automatically.
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

    gameObject.transform.localScale = Vector3.one;
    defaultUI = gameObject.GetComponentInChildren<CastDefaultUI>();
    defaultUI.Initialize(displayManager);
  }

  /**
   * When the UI is enabled, listen to the relevant events for the UI.
   */
  void OnEnable() {
    if (displayManager && defaultUI) {
      defaultUI.Initialize(displayManager);
    }
  }

  /**
   * When the UI is disabled, stop listening to the relevant events.
   */
  void OnDisable() {
    defaultUI.Uninitialize(displayManager);
  }
}
