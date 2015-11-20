using UnityEngine;
using System.Collections;

using Google.Cast.RemoteDisplay.UI;

namespace CompleteProject
{
  public class UIController : MonoBehaviour {

    public PlayerHealth playerHealth;
    public GameObject mainMenu;
    public GameObject pauseButton;
    public GameObject pausePanel;
    public GameObject joysticks;
    public ControllerCanvasManager controllerCanvas;
    public ReceiverCanvasManager receiverCanvas;

    private GameObject castUIController;
    private bool gameOver = false;

    private static bool restarted = false;

    public void Start () {
      Time.timeScale = 0f;
      castUIController = CastDefaultUI.GetInstance().gameObject;
      pausePanel.SetActive(false);
      if (restarted) {
        StartGame();
      }
    }

    public void Update() {
      if (!gameOver && playerHealth.currentHealth <= 0) {
        gameOver = true;
        EndGame();
      }
    }

    public void StartGame() {
      Time.timeScale = 1f;
      mainMenu.SetActive(false);
      castUIController.SetActive(false);
      receiverCanvas.StartGame();
    }

    public void PauseGame() {
      pauseButton.SetActive(false);
      pausePanel.SetActive(true);
      castUIController.SetActive(true);
      Time.timeScale = 0f;
      receiverCanvas.PauseGame();
    }

    public void UnpauseGame() {
      pauseButton.SetActive(true);
      pausePanel.SetActive(false);
      castUIController.SetActive(false);
      Time.timeScale = 1f;
      receiverCanvas.UnpauseGame();
    }

    public void EndGame() {
      pauseButton.SetActive(false);
      joysticks.SetActive(false);
      castUIController.SetActive(true);
      receiverCanvas.EndGame();
    }

    public void RestartLevel() {
      // Reload the level that is currently loaded.
      castUIController.SetActive(true);
      restarted = true;
      Application.LoadLevel (Application.loadedLevel);
    }
  }
}
