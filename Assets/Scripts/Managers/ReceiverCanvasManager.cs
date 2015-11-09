using UnityEngine;
using System.Collections;

using Google.Cast.RemoteDisplay;

namespace CompleteProject {
  public class ReceiverCanvasManager : MonoBehaviour {
    public GameObject startScreen;
    public GameObject pausedText;

    public void Start() {
      CastRemoteDisplayManager displayManager = CastRemoteDisplayManager.GetInstance();
      gameObject.SetActive(displayManager.IsCasting());
      pausedText.SetActive(false);

      displayManager.RemoteDisplaySessionStartEvent.AddListener(OnRemoteDisplaySessionStart);
      displayManager.RemoteDisplaySessionEndEvent.AddListener(OnRemoteDisplaySessionEnd);
    }

    private void OnDestroy() {
      CastRemoteDisplayManager displayManager = CastRemoteDisplayManager.GetInstance();
      displayManager.RemoteDisplaySessionStartEvent.RemoveListener(OnRemoteDisplaySessionStart);
      displayManager.RemoteDisplaySessionEndEvent.RemoveListener(OnRemoteDisplaySessionEnd);
    }

    private void OnRemoteDisplaySessionStart(CastRemoteDisplayManager manager) {
      gameObject.SetActive(true);
    }

    private void OnRemoteDisplaySessionEnd(CastRemoteDisplayManager manager) {
      gameObject.SetActive(false);
    }

    /**
     * These functions (StartGame, PauseGame, UnpauseGame, EndGame) are called by the UIController,
     *  and set the state depending on gameplay.
     */
    public void StartGame() {
      startScreen.SetActive(false);
    }

    public void PauseGame() {
      pausedText.SetActive(true);
    }

    public void UnpauseGame() {
      pausedText.SetActive(false);
    }

    public void EndGame() {
    }
  }
}
