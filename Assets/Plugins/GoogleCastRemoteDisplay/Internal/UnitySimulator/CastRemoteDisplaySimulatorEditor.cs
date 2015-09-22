// Copyright 2015 Google Inc.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

/**
 * Custom editor for the CastRemoteDisplaySimulator.
 */
[CustomEditor(typeof(CastRemoteDisplaySimulator))]
public class CastRemoteDisplaySimulatorEditor : Editor {

  protected static bool openFoldout = true;
  protected static int selected = 0;
  protected static CastErrorCode errorCode = CastErrorCode.NoError;

  public override void OnInspectorGUI() {
    CastRemoteDisplaySimulator simulator = (CastRemoteDisplaySimulator) target;

    serializedObject.Update();
    EditorGUILayout.PropertyField(serializedObject.FindProperty("simulateRemoteDisplay"), true);
    EditorGUILayout.PropertyField(serializedObject.FindProperty("remoteDisplayRect"), true);
    EditorGUILayout.PropertyField(serializedObject.FindProperty("castDevices"), true);
    serializedObject.ApplyModifiedProperties();

    if (Application.isPlaying) {
      // Update the list of devices.
      if (GUILayout.Button("Update devices")) {
        simulator.UpdateDevices();
      }
      EditorGUILayout.Space();

      // Throwing errors.
      errorCode = (CastErrorCode) EditorGUILayout.EnumPopup("Throw Error", errorCode);
      if (errorCode != CastErrorCode.NoError) {
        if (GUILayout.Button("Throw")) {
          simulator.ThrowError(errorCode);
        }
      }
    }
  }
}

#endif
