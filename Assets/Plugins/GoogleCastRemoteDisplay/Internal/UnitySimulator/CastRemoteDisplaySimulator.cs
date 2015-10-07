using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Drives the unity simulator prefab for the cast remote display plugin.  Reports devices and
 * errors to the CastRemoteDisplayUnityExtension.
 */
public class CastRemoteDisplaySimulator : MonoBehaviour {

  [Tooltip("Enable to render the remote display on top of the Play Window of the Unity Editor.")]
  public bool simulateRemoteDisplay = true;

  [Tooltip("Normalized screen coordinates [0, 1] for the position of the simulated remote " +
      "display.")]
  public Rect remoteDisplayRect = new Rect(0.6f, 0.6f, 0.39f, 0.39f);

  [Tooltip("List of simulated cast devices.")]
  public List<CastDevice> castDevices;

  private CastRemoteDisplayUnityExtension displayExtension;
  public CastRemoteDisplayUnityExtension DisplayExtension {
    set {
      displayExtension = value;
    }
  }

  /**
   * The Simulator is a singleton, because it works in parallel with the DisplayManager, which
   * is also a singleton.
   */
  static private CastRemoteDisplaySimulator instance;

  /**
   * Enforces uniqueness of the DisplaySimulator.
   */
  void Awake() {
    if (instance) {
      Debug.LogWarning("CastRemoteDisplaySimulator: Duplicate simulator found - destroying.");
      DestroyImmediate(gameObject);
      return;
    } else {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  /**
   * Ensures the devices get updated after a short amount of time.
   */
  public IEnumerator Start() {
    yield return new WaitForSeconds(3);
    UpdateDevices();
  }

  /**
   * Updates the list of devices.  Called shortly after Start, and called on the custom inspector
   * when the values change.
   */
  public void UpdateDevices() {
    if (gameObject.activeInHierarchy && displayExtension != null) {
      displayExtension.UpdateDevices();
    }
  }

  /**
   * Throws an error string, for testing error flows.
   */
  public void ThrowError(CastErrorCode errorCode) {
    if (gameObject.activeInHierarchy && displayExtension != null) {
      displayExtension.ThrowError(errorCode);
    }
  }
}
