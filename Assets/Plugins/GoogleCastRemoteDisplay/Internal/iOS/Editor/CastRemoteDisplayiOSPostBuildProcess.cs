// Copyright 2015 Google Inc.

#if UNITY_IOS && UNITY_EDITOR

using UnityEngine;

using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;

using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

/**
 * Customize XCode project build settings with flags and dependencies to build remote display for
 * iOS.
 */
public class CastRemoteDisplayiOSPostBuildProcess : MonoBehaviour {
  [PostProcessBuildAttribute]
  public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject) {
    if (buildTarget != BuildTarget.iOS) {
      return;
    }

    PBXProject project = new PBXProject();
    string pbxProjectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
    project.ReadFromString(File.ReadAllText(pbxProjectPath));

    string target = project.TargetGuidByName(PBXProject.GetUnityTargetName());
    string testTarget = project.TargetGuidByName(PBXProject.GetUnityTestTargetName());

    // Linker flags.
    project.SetBuildProperty(target, "ARCHS", "$(ARCHS_STANDARD)");
    project.SetBuildProperty(testTarget, "ARCHS", "$(ARCHS_STANDARD)");
    project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
    project.AddBuildProperty(testTarget, "OTHER_LDFLAGS", "-ObjC");

    // Search paths.
    project.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
    project.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");

    // Framework dependencies.
    project.AddFrameworkToProject(target, "AVFoundation.framework", true);
    project.AddFrameworkToProject(target, "Accelerate.framework", true);
    project.AddFrameworkToProject(target, "CFNetwork.framework", true);
    project.AddFrameworkToProject(target, "CoreBluetooth.framework", true);
    project.AddFrameworkToProject(target, "CoreText.framework", true);
    project.AddFrameworkToProject(target, "MediaPlayer.framework", true);
    project.AddFrameworkToProject(target, "MediaToolbox.framework", true);
    project.AddFrameworkToProject(target, "Metal.framework", true);
    project.AddFrameworkToProject(target, "Security.framework", true);
    project.AddFrameworkToProject(target, "SystemConfiguration.framework", true);

    // Dynamic library dependencies.
    string sqlite3dylibGuid = project.AddFile("usr/lib/libsqlite3.dylib",
                                              "usr/lib/libsqlite3.dylib", PBXSourceTree.Sdk);
    project.AddFileToBuild(target, sqlite3dylibGuid);
    string libCppdylibGuid = project.AddFile("usr/lib/libc++.dylib",
                                             "usr/lib/libc++.dylib", PBXSourceTree.Sdk);
    project.AddFileToBuild(target, libCppdylibGuid);

    File.WriteAllText(pbxProjectPath, project.WriteToString());
  }
}

#endif
