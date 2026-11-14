using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Contrôle qualité : place une caméra à des points de vue définis et enregistre des PNG dans Screenshots/.
    /// Utilisable depuis le menu ou en mode batch (-executeMethod …BuildAndCapture).
    /// </summary>
    public static class CaptureTool
    {
        public const string Folder = "Screenshots";
        const int W = 1600, H = 900;

        public readonly struct Pose
        {
            public readonly string name;
            public readonly Vector3 position;
            public readonly Vector3 euler;
            public Pose(string name, Vector3 position, Vector3 euler) { this.name = name; this.position = position; this.euler = euler; }
        }

        [MenuItem("Ennejma/Outils/Captures - Scène 2", priority = 100)]
        public static void CaptureScene02() => Capture(Scene02_HallBuilder.ScenePath, "scene02", Scene02_HallBuilder.CapturePoses);

        [MenuItem("Ennejma/Outils/Captures - Scène 1", priority = 99)]
        public static void CaptureScene01() => Capture("Assets/Scenes/01_Exterieur.unity", "scene01", Scene01Poses);

        /// <summary>Captures « avant » (préfixe distinct) pour comparer les étapes du rendu.</summary>
        public static void CaptureAvant()
        {
            Capture("Assets/Scenes/01_Exterieur.unity", "avant_scene01", Scene01Poses);
            Capture(Scene02_HallBuilder.ScenePath, "avant_scene02", Scene02_HallBuilder.CapturePoses);
        }

        /// <summary>Points de vue de l'extérieur, alignés autant que possible sur les photos de référence.</summary>
        public static readonly Pose[] Scene01Poses =
        {
            new Pose("01_rue_porte_jaune", new Vector3(0f, 1.65f, -8f), new Vector3(0f, 0f, 0f)),
            new Pose("02_allee_vers_le_nord", new Vector3(0f, 1.65f, 5f), new Vector3(2f, 0f, 0f)),
            new Pose("03_allee_vue_mer", new Vector3(1.2f, 1.65f, 30f), new Vector3(4f, 70f, 0f)),
            new Pose("04_esplanade_facade", new Vector3(-6f, 1.65f, 78f), new Vector3(-4f, -90f, 0f)),
            new Pose("05_esplanade_panorama", new Vector3(-12f, 1.65f, 86f), new Vector3(3f, 80f, 0f)),
            new Pose("06_coin_fontaine", new Vector3(-42f, 1.65f, 45f), new Vector3(4f, -55f, 0f)),
            new Pose("07_skifa", new Vector3(0f, 1.65f, -1.5f), new Vector3(0f, 0f, 0f)),
            new Pose("08_bassin_cypres", new Vector3(-5.5f, 1.65f, 28f), new Vector3(3f, -90f, 0f)),
            new Pose("09_vue_sud_bou_kornine", new Vector3(3.2f, 1.65f, 60f), new Vector3(4f, 168f, 0f)),
            new Pose("10_galerie_arcades", new Vector3(-37.6f, 1.8f, 62f), new Vector3(0f, 0f, 0f)),
            new Pose("11_jardin_persan_porte", new Vector3(-49f, 1.65f, 46f), new Vector3(-2f, 0f, 0f)),
        };

        public static void Capture(string scenePath, string prefix, Pose[] poses)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Directory.CreateDirectory(Folder);

            var go = new GameObject("Camera_Capture");
            var cam = go.AddComponent<Camera>();
            cam.fieldOfView = 70f;
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 2500f;
            var rt = new RenderTexture(W, H, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB) { antiAliasing = 4 };
            cam.targetTexture = rt;
            var tex = new Texture2D(W, H, TextureFormat.RGB24, false);
            // Compilation des shaders synchrone : sinon la première image peut montrer des shaders pas encore prêts
            bool async = ShaderUtil.allowAsyncCompilation;
            ShaderUtil.allowAsyncCompilation = false;

            foreach (var p in poses)
            {
                go.transform.SetPositionAndRotation(p.position, Quaternion.Euler(p.euler));
                cam.Render();   // image de chauffe
                cam.Render();
                var prev = RenderTexture.active;
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, W, H), 0, 0);
                tex.Apply();
                RenderTexture.active = prev;
                string path = Path.Combine(Folder, $"{prefix}_{p.name}.png");
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Debug.Log($"[Ennejma] Capture enregistrée : {path}");
            }

            ShaderUtil.allowAsyncCompilation = async;
            cam.targetTexture = null;
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);
            // La scène n'est pas réenregistrée : la caméra de capture n'y reste pas.
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}
