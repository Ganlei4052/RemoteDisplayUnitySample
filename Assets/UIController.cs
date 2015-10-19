using UnityEngine;
using System.Collections;


namespace CompleteProject
{
  public class UIController : MonoBehaviour {

    public PlayerHealth playerHealth;
    public GameObject mainMenu;
    public GameObject pauseButton;
    public GameObject pausePanel;
    public GameObject joysticks;

    private GameObject castUIController;
    private bool gameOver = false;

    public void Start () {
      Time.timeScale = 0f;
      castUIController = GameObject.Find("CastDefaultUI");
      pausePanel.SetActive(false);
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
    }

    public void PauseGame() {
      pauseButton.SetActive(false);
      pausePanel.SetActive(true);
      castUIController.SetActive(true);
      Time.timeScale = 0f;
    }

    public void UnpauseGame() {
      pauseButton.SetActive(true);
      pausePanel.SetActive(false);
      castUIController.SetActive(false);
      Time.timeScale = 1f;
    }

    public void EndGame() {
      pauseButton.SetActive(false);
      joysticks.SetActive(false);
    }

    public void RestartLevel() {
      // Reload the level that is currently loaded.
      castUIController.SetActive(true);
      Application.LoadLevel (Application.loadedLevel);
    }
  }
}
