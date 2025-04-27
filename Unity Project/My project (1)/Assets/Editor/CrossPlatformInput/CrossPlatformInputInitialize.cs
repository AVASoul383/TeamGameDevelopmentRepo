using System;
using System.Collections.Generic;
using UnityEditor;
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

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                case BuildTarget.WSAPlayer:
                    EditorUtility.DisplayDialog("Mobile Input",
                        "You have enabled Mobile Input. You'll need to use the Unity Remote app on a connected device to control your game in the Editor.",
                        "OK");
                    break;
                default:
                    EditorUtility.DisplayDialog("Mobile Input",
                        "You have enabled Mobile Input, but you have a non-mobile build target selected. Mobile controls won't be visible until you switch the build target to a mobile platform.",
                        "OK");
                    break;
            }
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

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                case BuildTarget.WSAPlayer:
                    EditorUtility.DisplayDialog("Mobile Input",
                        "You have disabled Mobile Input. Mobile controls won't be visible and Cross Platform Input will use standalone input.",
                        "OK");
                    break;
                default:
                    break;
            }
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
            BuildTargetGroup.iOS,
            BuildTargetGroup.WSA
        };

        private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[]
        {
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS,
            BuildTargetGroup.WSA
        };

        private static void SetEnabled(string defineName, bool enable, bool mobile)
        {
            foreach (var group in mobile ? mobileBuildTargetGroups : buildTargetGroups)
            {
                var defines = GetDefinesList(group);
                if (enable)
                {
                    if (!defines.Contains(defineName))
                    {
                        defines.Add(defineName);
                    }
                }
                else
                {
                    defines.Remove(defineName);
                }

                string definesString = string.Join(";", defines.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
            }
        }

        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
        }
    }
}
