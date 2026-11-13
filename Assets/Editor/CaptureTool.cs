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

            foreach (var p in poses)
            {
                go.transform.SetPositionAndRotation(p.position, Quaternion.Euler(p.euler));
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

            cam.targetTexture = null;
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);
            // La scène n'est pas réenregistrée : la caméra de capture n'y reste pas.
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}
