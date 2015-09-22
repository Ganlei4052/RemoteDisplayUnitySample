using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The dialog for displaying/selecting the list of Cast devices.
 */
public class CastListDialog : MonoBehaviour {

  /**
   * Prefab for the cast list elements.
   */
  [Tooltip("Default: CastListElementPrefab")]
  public GameObject listElementPrefab;

  /**
   * GameObject that contains (and formats) the list of cast buttons.
   */
  [Tooltip("Default: CastListDialog->ScrollView->ContentPanel")]
  public GameObject contentPanel;

  /**
   * The callback for closing the cast list.
   */
  public UICallback closeButtonTappedCallback;

  /**
   * Currently displayed list of buttons - one for each cast device.
   */
  private List<GameObject> currentButtons = new List<GameObject>();

  /**
   * Shows the list of cast devices.
   */
  public void Show() {
    gameObject.SetActive(true);
  }

  /**
   * Hides the list of cast devices.
   */
  public void Hide() {
    gameObject.SetActive(false);
  }

  /**
   * Populates the list of casts with devices, and sets up callbacks
   * for selecting said devices.
   */
  public void PopulateList(CastRemoteDisplayManager manager) {
    foreach (var button in currentButtons) {
      Destroy(button);
    }
    currentButtons.Clear();

    List<CastDevice> devices = manager.GetCastDevices();
    foreach (CastDevice listDevice in devices) {
      GameObject newButton = Instantiate(listElementPrefab) as GameObject;
      CastListButton button = newButton.GetComponent<CastListButton>();
      button.nameLabel.text = listDevice.deviceName;
      button.statusLabel.text = listDevice.status;
      string deviceId = listDevice.deviceId;
      button.button.onClick.AddListener(() => {
        manager.SelectCastDevice(deviceId);
      });
      newButton.transform.SetParent(contentPanel.transform, false);
      currentButtons.Add(newButton);
    }
  }

  /**
   * Triggers the callback for closing the cast list.  Set as the OnClick function for
   * CloseButton.
   */
  public void OnCloseButtonTapped() {
    closeButtonTappedCallback();
  }
}
