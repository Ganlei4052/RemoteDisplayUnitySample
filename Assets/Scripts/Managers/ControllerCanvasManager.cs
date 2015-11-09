using UnityEngine;
using System.Collections;

using Google.Cast.RemoteDisplay;

namespace CompleteProject {
  public class ControllerCanvasManager : MonoBehaviour {
    public GameObject backgroundImage;

    public void Start() {
      CastRemoteDisplayManager displayManager = CastRemoteDisplayManager.GetInstance();
      backgroundImage.SetActive(displayManager.IsCasting());

      displayManager.RemoteDisplaySessionStartEvent.AddListener(OnRemoteDisplaySessionStart);
      displayManager.RemoteDisplaySessionEndEvent.AddListener(OnRemoteDisplaySessionEnd);
    }

    private void OnDestroy() {
      CastRemoteDisplayManager displayManager = CastRemoteDisplayManager.GetInstance();
      displayManager.RemoteDisplaySessionStartEvent.RemoveListener(OnRemoteDisplaySessionStart);
      displayManager.RemoteDisplaySessionEndEvent.RemoveListener(OnRemoteDisplaySessionEnd);
    }

    private void OnRemoteDisplaySessionStart(CastRemoteDisplayManager manager) {
      backgroundImage.SetActive(true);
    }

    private void OnRemoteDisplaySessionEnd(CastRemoteDisplayManager manager) {
      backgroundImage.SetActive(false);
    }
  }
}
