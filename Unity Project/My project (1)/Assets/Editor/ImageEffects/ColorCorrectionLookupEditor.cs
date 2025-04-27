using UnityEngine;
using UnityEditor;

namespace UnityStandardAssets.ImageEffects
{
    [CustomEditor(typeof(ColorCorrectionLookup))]
    public class ColorCorrectionLookupEditor : Editor
    {
        SerializedProperty texture;

        void OnEnable()
        {
            texture = serializedObject.FindProperty("lookupTexture");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(texture, new GUIContent("Lookup Texture"));

            if (texture.objectReferenceValue != null)
            {
                string path = AssetDatabase.GetAssetPath(texture.objectReferenceValue);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                if (textureImporter != null)
                {
                    if (!textureImporter.isReadable || 
                        textureImporter.textureCompression != TextureImporterCompression.Uncompressed || 
                        textureImporter.mipmapEnabled)
                    {
                        EditorGUILayout.HelpBox(
                            "Lookup texture settings are incorrect! It must be:\n" +
                            "- Read/Write Enabled\n" +
                            "- Uncompressed\n" +
                            "- No Mip Maps",
                            MessageType.Warning);

                        if (GUILayout.Button("Fix Lookup Texture Import Settings"))
                        {
                            textureImporter.isReadable = true;
                            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                            textureImporter.mipmapEnabled = false;
                            textureImporter.SaveAndReimport();
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
