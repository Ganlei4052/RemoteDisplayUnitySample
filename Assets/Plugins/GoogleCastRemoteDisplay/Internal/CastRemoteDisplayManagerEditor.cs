// Copyright 2015 Google Inc.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Google.Cast.RemoteDisplay.Internal {
  /**
   * Custom editor for the CastRemoteDisplayManager.
   */
  [CustomEditor(typeof(CastRemoteDisplayManager))]
  public class CastRemoteDisplayManagerEditor : Editor {
    public override void OnInspectorGUI() {
      serializedObject.Update();

      CastRemoteDisplayManager manager = (CastRemoteDisplayManager) target;

      // Unfortunately, using property fields doesn't work with actual C# properties, and using
      // property fields on member variables directly (instead of the property) would cause the
      // inspector to not execute the custom logic on the setters of some of them. Using generic
      // object fields works, although it's more verbose.
      manager.RemoteDisplayCamera = (Camera) EditorGUILayout.ObjectField(
          "Remote Display Camera", manager.RemoteDisplayCamera, typeof(Camera), true);
      manager.RemoteDisplayTexture = (Texture) EditorGUILayout.ObjectField(
          "Remote Display Texture", manager.RemoteDisplayTexture, typeof(Texture), true);
      manager.RemoteDisplayPausedTexture = (Texture) EditorGUILayout.ObjectField(
          "Remote Display Paused Texture", manager.RemoteDisplayPausedTexture, typeof(Texture),
          true);
      manager.RemoteAudioListener = (AudioListener) EditorGUILayout.ObjectField(
          "Remote Audio Listener", manager.RemoteAudioListener, typeof(AudioListener), true);

      EditorGUILayout.Space();
      EditorGUILayout.Space();


      manager.CastAppId = (string) EditorGUILayout.TextField("Cast App Id", manager.CastAppId);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("configuration"), true);

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      EditorGUIUtility.LookLikeControls();
      EditorGUILayout.PropertyField(serializedObject.FindProperty("CastDevicesUpdatedEvent"),
          true);
      EditorGUILayout.PropertyField(
          serializedObject.FindProperty("RemoteDisplaySessionStartEvent"), true);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("RemoteDisplaySessionEndEvent"),
          true);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("RemoteDisplayErrorEvent"),
          true);

      if (GUI.changed) {
        EditorUtility.SetDirty(manager);
        serializedObject.ApplyModifiedProperties();
      }
    }
  }
}
#endif
