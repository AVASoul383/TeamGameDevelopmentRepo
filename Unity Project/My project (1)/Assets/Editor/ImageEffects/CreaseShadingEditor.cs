using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Crease Shading")]
    public class CreaseShading : PostEffectsBase
    {
        public float intensity = 0.5f;
        public int softness = 1;
        public float spread = 1.0f;

        public Shader blurShader = null;
        private Material blurMaterial = null;

        public Shader depthFetchShader = null;
        private Material depthFetchMaterial = null;

        public Shader creaseApplyShader = null;
        private Material creaseApplyMaterial = null;

        void OnEnable()
        {
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        }

        void OnDisable()
        {
            if (blurMaterial)
                DestroyImmediate(blurMaterial);
            if (depthFetchMaterial)
                DestroyImmediate(depthFetchMaterial);
            if (creaseApplyMaterial)
                DestroyImmediate(creaseApplyMaterial);
        }

        public override bool CheckResources()
        {
            CheckSupport(true);

            blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
            depthFetchMaterial = CheckShaderAndCreateMaterial(depthFetchShader, depthFetchMaterial);
            creaseApplyMaterial = CheckShaderAndCreateMaterial(creaseApplyShader, creaseApplyMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!CheckResources())
            {
                Graphics.Blit(source, destination);
                return;
            }

            int rtW = source.width;
            int rtH = source.height;

            RenderTexture depth = RenderTexture.GetTemporary(rtW, rtH, 0);
            RenderTexture blur0 = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);
            RenderTexture blur1 = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);

            Graphics.Blit(source, depth, depthFetchMaterial);

            Graphics.Blit(depth, blur0);
            for (int i = 0; i < softness; i++)
            {
                blurMaterial.SetVector("offsets", new Vector4(0, spread * 0.5f / (1.0f * blur0.height), 0, 0));
                RenderTexture temp = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);
                Graphics.Blit(blur0, temp, blurMaterial);
                RenderTexture.ReleaseTemporary(blur0);
                blur0 = temp;

                blurMaterial.SetVector("offsets", new Vector4(spread * 0.5f / (1.0f * blur0.width), 0, 0, 0));
                temp = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);
                Graphics.Blit(blur0, temp, blurMaterial);
                RenderTexture.ReleaseTemporary(blur0);
                blur0 = temp;
            }

            creaseApplyMaterial.SetTexture("_HrDepthTex", depth);
            creaseApplyMaterial.SetTexture("_LrDepthTex", blur0);
            creaseApplyMaterial.SetFloat("intensity", intensity);
            Graphics.Blit(source, destination, creaseApplyMaterial);

            RenderTexture.ReleaseTemporary(depth);
            RenderTexture.ReleaseTemporary(blur0);
            RenderTexture.ReleaseTemporary(blur1);
        }
    }
}
