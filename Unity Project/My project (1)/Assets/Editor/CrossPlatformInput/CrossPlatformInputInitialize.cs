using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.Inspector
{
    [InitializeOnLoad]
    public class CrossPlatformInitialize
    {
        static CrossPlatformInitialize()
        {
            var defines = GetDefinesList(BuildTargetGroup.Standalone);
            if (!defines.Contains("CROSS_PLATFORM_INPUT"))
            {
                SetEnabled("CROSS_PLATFORM_INPUT", true, false);
                SetEnabled("MOBILE_INPUT", true, true);
            }
        }

        [MenuItem("Mobile Input/Enable")]
        private static void Enable()
        {
            SetEnabled("MOBILE_INPUT", true, true);
            EditorUtility.DisplayDialog("Mobile Input",
                "You have enabled Mobile Input. You'll need to use the Unity Remote app on a connected device to control your game in the Editor.",
                "OK");
        }

        [MenuItem("Mobile Input/Enable", true)]
        private static bool EnableValidate()
        {
            var defines = GetDefinesList(BuildTargetGroup.Android);
            return !defines.Contains("MOBILE_INPUT");
        }

        [MenuItem("Mobile Input/Disable")]
        private static void Disable()
        {
            SetEnabled("MOBILE_INPUT", false, true);
            EditorUtility.DisplayDialog("Mobile Input",
                "You have disabled Mobile Input. Mobile control rigs won't be visible, and the Cross Platform Input functions will always return standalone controls.",
                "OK");
        }

        [MenuItem("Mobile Input/Disable", true)]
        private static bool DisableValidate()
        {
            var defines = GetDefinesList(BuildTargetGroup.Android);
            return defines.Contains("MOBILE_INPUT");
        }

        private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS
        };

        private static void SetEnabled(string defineName, bool enable, bool mobile)
        {
            foreach (var group in buildTargetGroups)
            {
                var namedGroup = NamedBuildTarget.FromBuildTargetGroup(group);
                var defines = GetDefinesList(group);
                if (enable)
                {
                    if (!defines.Contains(defineName))
                        defines.Add(defineName);
                }
                else
                {
                    defines.RemoveAll(d => d == defineName);
                }
                string definesString = string.Join(";", defines.ToArray());
                PlayerSettings.SetScriptingDefineSymbols(namedGroup, definesString);
            }
        }

        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            var namedGroup = NamedBuildTarget.FromBuildTargetGroup(group);
            return new List<string>(PlayerSettings.GetScriptingDefineSymbols(namedGroup).Split(';'));
        }
    }
}
