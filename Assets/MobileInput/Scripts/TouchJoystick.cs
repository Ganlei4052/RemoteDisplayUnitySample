using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;

/**
 * A joystick control that you can anchor by touching the mobile device screen.
 * Based on standard assets TouchPad.
 */
namespace UnityStandardAssets.CrossPlatformInput
{
  [RequireComponent(typeof(Image))]
  public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    /**
     * Joystick image.
     */
    public Texture joystick;

    /**
     * Ring image.
     */
    public Texture ring;

    /**
     * The name given to the horizontal axis for the cross platform input.
     */
    public string horizontalAxisName = "Horizontal";

    /**
     * The name given to the vertical axis for the cross platform input.
     */
    public string verticalAxisName = "Vertical";

    /**
     * X sensitivity.
     */
    public float xSensitivity = 1f;

    /**
     * Y sensitivity.
     */
    public float ySensitivity = 1f;

    /**
     * Reference to the joystick in the cross platform input.
     */
    CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

    /**
     * Reference to the joystick in the cross platform input.
     */
    CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

    /**
     * Touch dragging state.
     */
    bool dragging;

    /**
     * Touch event id.
     */
    int touchId = -1;

    /**
     * Current touch position.
     */
    Vector2 currentPos = new Vector2 (0, 0);

    /**
     * Touch down position.
     */
    Vector2 downPos = new Vector2 (0, 0);

    /**
     * Diff between down and current touch positions.
     */
    Vector2 currentToDownPos = new Vector2 (0, 0);

    /**
     * Precalculated power value for ring boundary distance.
     */
    float pow;

    /**
     * Radius of the joystick image.
     */
    float joystickRadius;

    /**
     * Radius of the ring image.
     */
    float ringRadius;


    #if !UNITY_EDITOR
    /**
     * Center of game object.
     */
    private Vector3 center;

    /**
     * Image game object.
     */
    private Image image;
    #else
    /**
     * Previous mouse location.
     */
    Vector3 previousMouse;
    #endif

    /**
     * Enable the game object.
     */
    void OnEnable() {
      CreateVirtualAxes ();
      #if !UNITY_EDITOR
      image = GetComponent<Image> ();
      center = image.transform.position;
      #endif
      // Pre-calculate some values to size the graphics.
      joystickRadius = Screen.height / 8;
      ringRadius = joystickRadius * 2;
      pow = Mathf.Pow (ringRadius / 2, 2);
    }

    /**
     * Create the virtual axes for the joystick.
     */
    void CreateVirtualAxes() {
      horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis (horizontalAxisName);
      verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis (verticalAxisName);
    }

    /**
     * Update the virtual axes for the joystick.
     */
    void UpdateVirtualAxes(Vector3 value) {
      value = value.normalized;
      horizontalVirtualAxis.Update (value.x);
      verticalVirtualAxis.Update (value.y);
    }

    /**
     * Handle mouse down event.
     */
    public void OnPointerDown(PointerEventData data) {
      dragging = true;
      touchId = data.pointerId;
      #if !UNITY_EDITOR
      center = data.position;
      #endif
      downPos = data.position;
    }

    /**
     * Update handler.
     */
    void Update() {
      if (!dragging) {
        return;
      }
      if (Input.touchCount >= touchId + 1 && touchId != -1) {
        #if !UNITY_EDITOR
        Vector2 pointerDelta = new Vector2 (Input.touches [touchId].position.x - center.x,
                                            Input.touches [touchId].position.y - center.y)
                                            .normalized;
        pointerDelta.x *= xSensitivity;
        pointerDelta.y *= ySensitivity;
        currentPos.x = Input.touches [touchId].position.x;
        currentPos.y = Input.touches [touchId].position.y;
        #else
        Vector2 pointerDelta;
        pointerDelta.x = Input.mousePosition.x - previousMouse.x;
        pointerDelta.y = Input.mousePosition.y - previousMouse.y;
        previousMouse = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
        currentPos.x = Input.mousePosition.x;
        currentPos.y = Input.mousePosition.y;
        #endif
        UpdateVirtualAxes (new Vector3 (pointerDelta.x, pointerDelta.y, 0));
      }
    }

    /**
     * Handle mouse up event.
     */
    public void OnPointerUp(PointerEventData data) {
      dragging = false;
      touchId = -1;
      UpdateVirtualAxes (Vector3.zero);
    }

    /**
     * Disable the game object.
     */
    void OnDisable() {
      if (CrossPlatformInputManager.VirtualAxisReference (horizontalAxisName) != null)
        CrossPlatformInputManager.UnRegisterVirtualAxis (horizontalAxisName);

      if (CrossPlatformInputManager.VirtualAxisReference (verticalAxisName) != null)
        CrossPlatformInputManager.UnRegisterVirtualAxis (verticalAxisName);
    }

    /**
     * Update the GUI. Draw the joystick image within the ring image boundary.
     */
    void OnGUI() {
      if (!dragging) {
        return;
      }

      // Draw the outer ring.
      GUI.DrawTexture (new Rect (downPos.x - ringRadius, Screen.height - downPos.y - ringRadius,
                                 ringRadius * 2, ringRadius * 2), ring, ScaleMode.ScaleToFit, true);

      // Limit the joystick to inside the ring.
      currentToDownPos = downPos - currentPos;
      if (Mathf.Round (currentToDownPos.sqrMagnitude) >= pow) {
        currentPos = downPos + currentToDownPos.normalized * ringRadius / 2;
      }

      // Draw the joystick.
      GUI.DrawTexture (new Rect (currentPos.x - joystickRadius, Screen.height - currentPos.y -
                                 joystickRadius, joystickRadius * 2, joystickRadius * 2), joystick,
                                 ScaleMode.ScaleToFit, true);
    }
  }
}