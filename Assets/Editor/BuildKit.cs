using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Boîte à outils de construction réutilisée par toutes les scènes du palais :
    /// matériaux, primitives, murs, arcs andalous (outrepassés), triggers, joueur, lumière.
    /// </summary>
    public static class BuildKit
    {
        public const string MaterialFolder = "Assets/Materials/Generated";
        public const string SceneFolder = "Assets/Scenes";

        // ------------------------------------------------------------------ Aléatoire
        static System.Random rng = new System.Random(1912);
        static readonly Dictionary<string, Material> cache = new Dictionary<string, Material>();

        public static void BeginBuild(int seed)
        {
            rng = new System.Random(seed);
            cache.Clear();
        }

        public static float R(float min, float max) => min + (float)rng.NextDouble() * (max - min);
        public static int RI(int min, int maxExclusive) => rng.Next(min, maxExclusive);
        public static bool Chance(float p) => rng.NextDouble() < p;

        // ------------------------------------------------------------------ Matériaux
        public static bool UsesSRP => GraphicsSettings.currentRenderPipeline != null;

        public static Shader LitShader()
        {
            Shader s = null;
            if (UsesSRP)
            {
                s = Shader.Find("Universal Render Pipeline/Lit");
                if (s == null) s = Shader.Find("HDRP/Lit");
            }
            if (s == null) s = Shader.Find("Standard");
            return s;
        }

        public static Material Mat(string name, Color color, float smoothness = 0.15f, float metallic = 0f)
        {
            if (cache.TryGetValue(name, out var cached) && cached != null) return cached;
            var m = LoadOrCreate(name, LitShader());
            m.color = color;
            SetIf(m, "_Smoothness", smoothness);
            SetIf(m, "_Glossiness", smoothness);
            SetIf(m, "_Metallic", metallic);
            EditorUtility.SetDirty(m);
            cache[name] = m;
            return m;
        }

        public static Material Emissive(string name, Color color, Color emission)
        {
            var m = Mat(name, color, 0.3f);
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", emission);
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            EditorUtility.SetDirty(m);
            return m;
        }

        public static Material ParticleMat()
        {
            Shader s = null;
            if (UsesSRP) s = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (s == null) s = Shader.Find("Particles/Standard Unlit");
            var m = LoadOrCreate("Particule_Eau", s != null ? s : LitShader());
            m.color = new Color(0.85f, 0.93f, 1f);
            EditorUtility.SetDirty(m);
            return m;
        }

        static Material LoadOrCreate(string name, Shader shader)
        {
            if (shader == null) shader = LitShader();
            EnsureFolder(MaterialFolder);
            string path = $"{MaterialFolder}/{name}.mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null)
            {
                m = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(m, path);
            }
            else if (m.shader != shader) m.shader = shader;
            return m;
        }

        static void SetIf(Material m, string prop, float v) { if (m.HasProperty(prop)) m.SetFloat(prop, v); }
        static void SetColorIf(Material m, string prop, Color c) { if (m.HasProperty(prop)) m.SetColor(prop, c); }

        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        // ------------------------------------------------------------------ Hiérarchie & primitives
        public static Transform Group(string name, Transform parent, Vector3 pos = default, Vector3 euler = default)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = euler;
            go.isStatic = true;
            return go.transform;
        }

        public static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale,
                                      Material mat, Vector3 euler, bool collider)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = euler;
            go.transform.localScale = scale;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
            go.isStatic = true;
            return go;
        }

        public static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat,
                                     float yaw = 0f, bool collider = true)
            => Prim(PrimitiveType.Cube, name, parent, pos, scale, mat, new Vector3(0f, yaw, 0f), collider);

        public static GameObject BoxE(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat,
                                      Vector3 euler, bool collider = true)
            => Prim(PrimitiveType.Cube, name, parent, pos, scale, mat, euler, collider);

        /// <summary>Cylindre défini par rayon et hauteur (pas de collider par défaut).</summary>
        public static GameObject Cyl(string name, Transform parent, Vector3 pos, float radius, float height, Material mat,
                                     Vector3 euler = default, bool collider = false)
            => Prim(PrimitiveType.Cylinder, name, parent, pos, new Vector3(radius * 2f, height * 0.5f, radius * 2f), mat, euler, collider);

        public static GameObject Sph(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool collider = false)
            => Prim(PrimitiveType.Sphere, name, parent, pos, scale, mat, Vector3.zero, collider);

        public static GameObject Cap(string name, Transform parent, Vector3 pos, float radius, float height, Material mat, bool collider = true)
            => Prim(PrimitiveType.Capsule, name, parent, pos, new Vector3(radius * 2f, height * 0.5f, radius * 2f), mat, Vector3.zero, collider);

        /// <summary>Mur droit entre deux points au sol (a.y = base du mur).</summary>
        public static GameObject Wall(string name, Transform parent, Vector3 a, Vector3 b, float height, float thickness,
                                      Material mat, bool collider = true)
        {
            Vector3 d = b - a; d.y = 0f;
            float yaw = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
            Vector3 mid = (a + b) * 0.5f;
            mid.y = a.y + height * 0.5f;
            return Box(name, parent, mid, new Vector3(thickness, height, d.magnitude), mat, yaw, collider);
        }

        /// <summary>Angle Y pour que la face avant (-Z local) d'un objet regarde la cible.</summary>
        public static float FaceYaw(Vector3 from, Vector3 to)
        {
            Vector3 d = to - from;
            return Mathf.Atan2(-d.x, -d.z) * Mathf.Rad2Deg;
        }

        public static void NoShadows(Transform root)
        {
            foreach (var r in root.GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = ShadowCastingMode.Off;
        }

        // ------------------------------------------------------------------ Architecture andalouse
        /// <summary>
        /// Arc (plein cintre ou outrepassé "fer à cheval") fait de claveaux, dans le plan XY local.
        /// span = largeur intérieure ; horseshoeDeg = 0 plein cintre, 15–25° arc andalou.
        /// </summary>
        public static Transform Arch(string name, Transform parent, Vector3 pos, float yaw, float span, float springHeight,
                                     float depth, float thickness, Material mat, float horseshoeDeg = 0f,
                                     int segments = 16, bool pillars = true)
        {
            var g = Group(name, parent, pos, new Vector3(0f, yaw, 0f));
            float rin = span * 0.5f;
            float r = rin + thickness * 0.5f;
            float h = horseshoeDeg * Mathf.Deg2Rad;
            float cy = springHeight + rin * Mathf.Sin(h);
            float a0 = -h, a1 = Mathf.PI + h;
            float step = (a1 - a0) / segments;
            float segLen = r * step * 1.12f;

            for (int i = 0; i < segments; i++)
            {
                float a = a0 + (i + 0.5f) * step;
                Prim(PrimitiveType.Cube, "Claveau", g,
                     new Vector3(Mathf.Cos(a) * r, cy + Mathf.Sin(a) * r, 0f),
                     new Vector3(segLen, thickness, depth), mat,
                     new Vector3(0f, 0f, a * Mathf.Rad2Deg + 90f), false);
            }

            if (pillars)
            {
                float px = r * Mathf.Cos(h);
                float top = cy - r * Mathf.Sin(h);
                for (int s = -1; s <= 1; s += 2)
                    Prim(PrimitiveType.Cube, s < 0 ? "Pied_G" : "Pied_D", g,
                         new Vector3(s * px, top * 0.5f, 0f), new Vector3(thickness, top, depth), mat, Vector3.zero, true);
            }
            return g;
        }

        /// <summary>
        /// Pan de mur percé d'une ouverture en arc. Mur dans le plan XY local, épaisseur sur Z,
        /// face avant vers -Z. Les écoinçons au-dessus de l'arc sont remplis.
        /// </summary>
        public static Transform WallWithArch(string name, Transform parent, Vector3 pos, float yaw,
                                             float wallWidth, float wallHeight, float thickness,
                                             float openWidth, float springHeight,
                                             Material wallMat, Material archMat,
                                             float horseshoeDeg = 0f, float archThickness = 0.3f)
        {
            var g = Group(name, parent, pos, new Vector3(0f, yaw, 0f));
            float side = (wallWidth - openWidth) * 0.5f;
            Box("Mur_G", g, new Vector3(-(openWidth + side) * 0.5f, wallHeight * 0.5f, 0f), new Vector3(side, wallHeight, thickness), wallMat);
            Box("Mur_D", g, new Vector3((openWidth + side) * 0.5f, wallHeight * 0.5f, 0f), new Vector3(side, wallHeight, thickness), wallMat);

            float rin = openWidth * 0.5f;
            float cy = springHeight + rin * Mathf.Sin(horseshoeDeg * Mathf.Deg2Rad);
            float top = cy + rin;
            if (wallHeight > top)
                Box("Mur_Haut", g, new Vector3(0f, (top + wallHeight) * 0.5f, 0f), new Vector3(openWidth, wallHeight - top, thickness), wallMat);

            const int cols = 20;
            float dx = openWidth / cols;
            for (int i = 0; i < cols; i++)
            {
                float x = -rin + (i + 0.5f) * dx;
                float yArc = cy + Mathf.Sqrt(Mathf.Max(0f, rin * rin - x * x));
                float hgt = top - yArc;
                if (hgt > 0.01f)
                    Box("Ecoincon", g, new Vector3(x, yArc + hgt * 0.5f, 0f), new Vector3(dx * 1.02f, hgt, thickness), wallMat, 0f, false);
            }

            Arch("Arc", g, Vector3.zero, 0f, openWidth, springHeight, thickness + 0.12f, archThickness, archMat, horseshoeDeg);
            return g;
        }

        public static void Studs(Transform parent, Vector3 center, float w, float h, int cols, int rows, float z, Material m, float size = 0.06f)
        {
            for (int i = 0; i < cols; i++)
                for (int j = 0; j < rows; j++)
                {
                    float x = cols > 1 ? -w * 0.5f + i * w / (cols - 1) : 0f;
                    float y = rows > 1 ? -h * 0.5f + j * h / (rows - 1) : 0f;
                    Sph("Clou", parent, center + new Vector3(x, y, z), Vector3.one * size, m);
                }
        }

        public static void Text3D(string name, Transform parent, Vector3 pos, float yaw, string text, float charSize, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.characterSize = charSize;
            tm.fontSize = 64;
            tm.color = color;
            Font f = null;
            try { f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
            if (f == null) { try { f = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { } }
            if (f != null)
            {
                tm.font = f;
                go.GetComponent<MeshRenderer>().sharedMaterial = f.material;
            }
        }

        // ------------------------------------------------------------------ Gameplay
        public static GameObject Trigger(string name, Transform parent, Vector3 pos, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var box = go.AddComponent<BoxCollider>();
            box.size = size;
            box.isTrigger = true;
            return go;
        }

        public static void InvisibleWall(string name, Transform parent, Vector3 pos, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.AddComponent<BoxCollider>().size = size;
            go.isStatic = true;
        }

        public static InfoZone Info(Transform parent, Vector3 pos, Vector3 size, string title, string text)
        {
            var z = Trigger("Info_" + title, parent, pos, size).AddComponent<InfoZone>();
            z.title = title;
            z.text = text;
            return z;
        }

        public static SceneTransition Transition(Transform parent, Vector3 pos, Vector3 size, string scene, string spawnId)
        {
            var t = Trigger("Transition_vers_" + scene, parent, pos, size).AddComponent<SceneTransition>();
            t.targetScene = scene;
            t.targetSpawnId = spawnId;
            return t;
        }

        public static SpawnPoint Spawn(Transform parent, Vector3 pos, float yaw, string id)
        {
            var go = new GameObject("Spawn_" + id);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
            var sp = go.AddComponent<SpawnPoint>();
            sp.spawnId = id;
            return sp;
        }

        /// <summary>Le visiteur : CharacterController + caméra sur la tête (1,65 m).</summary>
        public static GameObject CreatePlayer(Vector3 pos, float yaw)
        {
            var p = new GameObject("Visiteur");
            p.tag = "Player";
            p.transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, yaw, 0f));

            var cc = p.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.stepOffset = 0.4f;
            cc.slopeLimit = 50f;

            var camGo = new GameObject("Camera_Tete");
            camGo.tag = "MainCamera";
            camGo.transform.SetParent(p.transform, false);
            camGo.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            var cam = camGo.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 2500f;
            cam.fieldOfView = 70f;
            camGo.AddComponent<AudioListener>();

            p.AddComponent<FirstPersonController>().cameraRoot = camGo.transform;
            return p;
        }

        // ------------------------------------------------------------------ Lumière
        public static Light SetupDaylight(Vector3 sunEuler, float intensity)
        {
            var go = new GameObject("Soleil");
            var l = go.AddComponent<Light>();
            l.type = LightType.Directional;
            l.color = new Color(1f, 0.95f, 0.86f);
            l.intensity = intensity;
            l.shadows = LightShadows.Soft;
            l.shadowStrength = 0.85f;
            go.transform.rotation = Quaternion.Euler(sunEuler);

            var skyShader = Shader.Find("Skybox/Procedural");
            if (skyShader != null)
            {
                var sky = LoadOrCreate("Ciel_SidiBouSaid", skyShader);
                SetColorIf(sky, "_SkyTint", new Color(0.32f, 0.52f, 0.95f));
                SetColorIf(sky, "_GroundColor", new Color(0.4f, 0.5f, 0.6f));
                SetIf(sky, "_AtmosphereThickness", 0.8f);
                SetIf(sky, "_Exposure", 1.25f);
                SetIf(sky, "_SunSize", 0.035f);
                EditorUtility.SetDirty(sky);
                RenderSettings.skybox = sky;
            }
            RenderSettings.sun = l;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 1.1f;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.0018f;
            RenderSettings.fogColor = new Color(0.78f, 0.86f, 0.95f);
            DynamicGI.UpdateEnvironment();
            return l;
        }

        // ------------------------------------------------------------------ Scènes
        public static Scene NewScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return default;
            return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        public static void SaveScene(Scene scene, string sceneName)
        {
            EnsureFolder(SceneFolder);
            string path = $"{SceneFolder}/{sceneName}.unity";
            EditorSceneManager.SaveScene(scene, path);

            var list = EditorBuildSettings.scenes.ToList();
            if (!list.Any(s => s.path == path))
            {
                list.Add(new EditorBuildSettingsScene(path, true));
                EditorBuildSettings.scenes = list.ToArray();
            }
            AssetDatabase.SaveAssets();
            Debug.Log($"[Ennejma] Scène '{sceneName}' construite et enregistrée dans {path}");
        }
    }
}
