using UnityEngine;
using UnityEditor;

namespace UnityStandardAssets.ImageEffects
{
    [CustomEditor(typeof(ColorCorrectionLookup))]
    public class ColorCorrectionLookupEditor : Editor
    {
        SerializedProperty lutTexture;
        SerializedProperty basedOnTempTex;

        void OnEnable()
        {
            lutTexture = serializedObject.FindProperty("converted3DLut");
            basedOnTempTex = serializedObject.FindProperty("basedOnTempTex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(lutTexture, new GUIContent("Lookup Texture"));
            EditorGUILayout.PropertyField(basedOnTempTex, new GUIContent("Base Texture"));

            ColorCorrectionLookup effect = (ColorCorrectionLookup)target;

            if (effect.converted3DLut != null)
            {
                TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(effect.converted3DLut)) as TextureImporter;
                if (importer != null)
                {
                    if (importer.textureType != TextureImporterType.Default || importer.isReadable == false)
                    {
                        EditorGUILayout.HelpBox("Lookup texture must be marked as 'Default' type and Read/Write enabled.", MessageType.Warning);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
