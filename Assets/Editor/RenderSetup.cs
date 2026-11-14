using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Passage du projet en URP : crée deux presets de rendu dans Assets/Settings/ et les associe aux niveaux de qualité.
    ///   - « Haute »   : HDR, MSAA 4x, ombres 4096 sur 4 cascades jusqu'à 150 m (captures, démonstrations) ;
    ///   - « Moyenne » : HDR, MSAA 2x, ombres 2048 sur 2 cascades jusqu'à 90 m (visée : 60 FPS sur Intel Iris Xe).
    /// Relançable : les assets existants sont réutilisés et remis aux mêmes réglages.
    /// </summary>
    public static class RenderSetup
    {
        public const string Folder = "Assets/Settings";
        public const string HautePath = Folder + "/URP_Haute.asset";
        public const string MoyennePath = Folder + "/URP_Moyenne.asset";

        [MenuItem("Ennejma/Outils/Configurer le rendu URP", priority = 110)]
        public static void SetupURP()
        {
            BuildKit.EnsureFolder(Folder);
            var haute = GetOrCreate(HautePath, Folder + "/URP_Haute_Renderer.asset");
            var moyenne = GetOrCreate(MoyennePath, Folder + "/URP_Moyenne_Renderer.asset");

            Configure(haute, msaa: 4, shadowRes: 4096, cascades: 4, distance: 150f);
            Configure(moyenne, msaa: 2, shadowRes: 2048, cascades: 2, distance: 90f);

            GraphicsSettings.defaultRenderPipeline = haute;
            // Niveaux de qualité : les trois plus bas = Moyenne, les autres = Haute
            string[] names = QualitySettings.names;
            int current = QualitySettings.GetQualityLevel();
            for (int i = 0; i < names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = i < 3 ? moyenne : haute;
            }
            QualitySettings.SetQualityLevel(current, false);

            AssetDatabase.SaveAssets();
            Debug.Log($"[Ennejma] URP configuré : défaut = Haute ; niveaux 0-2 = Moyenne, 3-{names.Length - 1} = Haute ({string.Join(", ", names)}).");
        }

        /// <summary>Mode batch : configure URP puis régénère les scènes et les captures.</summary>
        public static void SetupURPAndRebuild()
        {
            SetupURP();
            Scene01_ExteriorBuilder.Build();
            CaptureTool.CaptureScene01();
            Scene02_HallBuilder.BuildAndCapture();
        }

        static UniversalRenderPipelineAsset GetOrCreate(string assetPath, string rendererPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(assetPath);
            if (asset != null) return asset;

            var renderer = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(rendererPath);
            if (renderer == null)
            {
                // Méthode interne d'URP : crée le renderer avec ses données de post-traitement par défaut
                var create = typeof(UniversalRenderPipelineAsset).GetMethod("CreateRendererAsset", BindingFlags.NonPublic | BindingFlags.Static);
                renderer = (ScriptableRendererData)create.Invoke(null, new object[] { rendererPath, RendererType.UniversalRenderer, false, "Renderer" });
            }
            asset = UniversalRenderPipelineAsset.Create(renderer);
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        static void Configure(UniversalRenderPipelineAsset a, int msaa, int shadowRes, int cascades, float distance)
        {
            a.supportsHDR = true;
            a.msaaSampleCount = msaa;
            a.renderScale = 1f;
            a.shadowDistance = distance;
            a.shadowCascadeCount = cascades;
            a.mainLightShadowmapResolution = shadowRes;
            var so = new SerializedObject(a);   // ombres douces : propriété en lecture seule, on passe par le champ sérialisé
            so.FindProperty("m_SoftShadowsSupported").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(a);
        }
    }
}
