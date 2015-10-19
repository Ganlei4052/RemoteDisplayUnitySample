// Copyright 2015 Google Inc.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Google.Cast.RemoteDisplay.UI {
  /**
   * Custom editor for the CastDefaultUI.
   */
  [CustomEditor(typeof(CastDefaultUI))]
  public class CastDefaultUIEditor : Editor {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();

      if (GUILayout.Button("Reset First Time Flag")) {
        PlayerPrefs.SetInt("firstTimeCastShown", 0);
      }
    }
  }
}

#endif
