using UnityEngine;
using System.Collections;

using Google.Cast.RemoteDisplay;

namespace CompleteProject {
  public class ControllerCanvasManager : MonoBehaviour {
    private GameObject playOnTvButton;

    public GameObject backgroundImage;

    public void Start() {
      CastRemoteDisplayManager displayManager = CastRemoteDisplayManager.GetInstance();
      GameObject buttonFrame = GameObject.Find("CastButtonFrame");
      playOnTvButton = buttonFrame.transform.Find("PlayOnTvButton").gameObject;
      backgroundImage.SetActive(displayManager.IsCasting());
      playOnTvButton.SetActive(!displayManager.IsCasting());

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
      playOnTvButton.SetActive(false);
    }

    private void OnRemoteDisplaySessionEnd(CastRemoteDisplayManager manager) {
      backgroundImage.SetActive(false);
      playOnTvButton.SetActive(true);
    }
  }
}
