using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

  private Canvas canvas;

  // Use this for initialization
  void Start() {
    Time.timeScale = 0f;
    canvas = GetComponent<Canvas>();
  }

  // Update is called once per frame
  public void StartGame() {
    Time.timeScale = 1f;
    canvas.enabled = false;
  }
}
