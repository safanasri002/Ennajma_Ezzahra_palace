using System.Collections.Generic;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Maillages générés par code (remplacent les primitives quand il faut des courbes propres) :
    /// boîtes à UV en mètres (textures répétées à l'échelle réelle) et murs d'arcades à arcs lisses.
    /// Convention UV : 1 unité d'UV = 1 mètre ; le tiling du matériau = 1 / taille du motif.
    /// </summary>
    public static class MeshKit
    {
        // ------------------------------------------------------------------ Constructeur de maillage
        class MB
        {
            readonly List<Vector3> v = new List<Vector3>();
            readonly List<Vector3> n = new List<Vector3>();
            readonly List<Vector2> uv = new List<Vector2>();
            readonly List<int> t = new List<int>();

            /// <summary>Triangle orienté pour que sa face visible regarde « normal ».</summary>
            public void Tri(Vector3 a, Vector3 b, Vector3 c, Vector2 ua, Vector2 ub, Vector2 uc, Vector3 normal)
            {
                if (Vector3.Dot(Vector3.Cross(b - a, c - a), normal) < 0f)
                {
                    (b, c) = (c, b);
                    (ub, uc) = (uc, ub);
                }
                int i = v.Count;
                v.Add(a); v.Add(b); v.Add(c);
                uv.Add(ua); uv.Add(ub); uv.Add(uc);
                n.Add(normal); n.Add(normal); n.Add(normal);
                t.Add(i); t.Add(i + 1); t.Add(i + 2);
            }

            public void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 ua, Vector2 ub, Vector2 uc, Vector2 ud, Vector3 normal)
            {
                Tri(a, b, c, ua, ub, uc, normal);
                Tri(a, c, d, ua, uc, ud, normal);
            }

            public Mesh Build(string name)
            {
                var m = new Mesh { name = name };
                if (v.Count > 65000) m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                m.SetVertices(v);
                m.SetNormals(n);
                m.SetUVs(0, uv);
                m.SetTriangles(t, 0);
                m.RecalculateBounds();
                m.RecalculateTangents();
                return m;
            }
        }

        public static GameObject MeshObject(string name, Transform parent, Mesh mesh, Material mat, Vector3 pos, Vector3 euler)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = euler;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
            go.isStatic = true;
            return go;
        }

        // ------------------------------------------------------------------ Boîte à UV en mètres
        public static Mesh CubeMesh(Vector3 s)
        {
            var b = new MB();
            Vector3 h = s * 0.5f;
            void Face(Vector3 nrm, Vector3 u, Vector3 w, float su, float sw, float d)
            {
                Vector3 c = nrm * d;
                Vector3 p0 = c - u * su * 0.5f - w * sw * 0.5f, p1 = c + u * su * 0.5f - w * sw * 0.5f;
                Vector3 p2 = c + u * su * 0.5f + w * sw * 0.5f, p3 = c - u * su * 0.5f + w * sw * 0.5f;
                b.Quad(p0, p1, p2, p3, new Vector2(0, 0), new Vector2(su, 0), new Vector2(su, sw), new Vector2(0, sw), nrm);
            }
            Face(Vector3.back, Vector3.right, Vector3.up, s.x, s.y, h.z);
            Face(Vector3.forward, Vector3.left, Vector3.up, s.x, s.y, h.z);
            Face(Vector3.left, Vector3.back, Vector3.up, s.z, s.y, h.x);
            Face(Vector3.right, Vector3.forward, Vector3.up, s.z, s.y, h.x);
            Face(Vector3.up, Vector3.right, Vector3.forward, s.x, s.z, h.y);
            Face(Vector3.down, Vector3.right, Vector3.back, s.x, s.z, h.y);
            return b.Build("Boite");
        }

        /// <summary>Boîte dont la texture est répétée à l'échelle réelle (pas étirée).</summary>
        public static GameObject WBox(string name, Transform parent, Vector3 pos, Vector3 size, Material mat, float yaw = 0f, bool collider = true)
        {
            var go = MeshObject(name, parent, CubeMesh(size), mat, pos, new Vector3(0f, yaw, 0f));
            if (collider) go.AddComponent<BoxCollider>().size = size;
            return go;
        }

        // ------------------------------------------------------------------ Mur d'arcades
        /// <summary>
        /// Mur percé de n arcs identiques, construit le long de +X local depuis pos (base du mur, axe du mur).
        /// openW = largeur entre piédroits ; spring = hauteur de naissance ; horseshoeDeg > 0 : arc outrepassé.
        /// bandMat : bandeau de stuc qui souligne chaque arc (archivolte), sur les deux faces.
        /// Renvoie les abscisses locales des centres des piliers (pour y poser les colonnes).
        /// </summary>
        public static float[] ArcadeWall(string name, Transform parent, Vector3 pos, float yaw, float length, float height,
                                         float thick, int n, float openW, float spring, float horseshoeDeg,
                                         Material wallMat, Material bandMat, float bandW = 0.25f, bool colliders = true)
        {
            var g = BuildKit.Group(name, parent, pos, new Vector3(0f, yaw, 0f));
            float pier = (length - n * openW) / (n + 1);
            var piers = new float[n + 1];
            for (int i = 0; i <= n; i++)
            {
                float px = pier * 0.5f + i * (pier + openW);
                piers[i] = px;
                WBox("Pilier", g, new Vector3(px, height * 0.5f, 0f), new Vector3(pier, height, thick), wallMat, 0f, colliders);
            }
            for (int i = 0; i < n; i++)
            {
                float cx = pier + openW * 0.5f + i * (pier + openW);
                MeshObject("Arc", g, ArchMesh(openW, spring, horseshoeDeg, height, thick, cx), wallMat, new Vector3(cx, 0f, 0f), Vector3.zero);
                if (bandMat != null)
                    MeshObject("Archivolte", g, BandMesh(openW, spring, horseshoeDeg, thick, bandW), bandMat, new Vector3(cx, 0f, 0f), Vector3.zero);
            }
            return piers;
        }

        // ------------------------------------------------------------------ Toits et auvents
        /// <summary>Pyramide (toit en croupe) de base w × d, posée en pos (centre de la base).</summary>
        public static GameObject Pyramid(string name, Transform parent, Vector3 pos, float w, float d, float h, Material mat, float yaw = 0f)
        {
            var b = new MB();
            var top = new Vector3(0f, h, 0f);
            var c = new[] { new Vector3(-w / 2, 0, -d / 2), new Vector3(w / 2, 0, -d / 2), new Vector3(w / 2, 0, d / 2), new Vector3(-w / 2, 0, d / 2) };
            for (int i = 0; i < 4; i++)
            {
                Vector3 a = c[i], e = c[(i + 1) % 4];
                var nrm = Vector3.Cross(e - a, top - a).normalized;
                if (Vector3.Dot(nrm, (a + e) * 0.5f) < 0f) nrm = -nrm;
                b.Tri(a, e, top, new Vector2(0, 0), new Vector2(Vector3.Distance(a, e), 0), new Vector2(Vector3.Distance(a, e) * 0.5f, h), nrm);
            }
            b.Quad(c[0], c[1], c[2], c[3], Vector2.zero, new Vector2(w, 0), new Vector2(w, d), new Vector2(0, d), Vector3.down);
            return MeshObject(name, parent, b.Build(name), mat, pos, new Vector3(0f, yaw, 0f));
        }

        /// <summary>
        /// Auvent en quart de sphère (demi-coupole) adossé à un mur : base plane horizontale, dos plaqué au mur (z = 0),
        /// saillie vers -Z local. pos = milieu du bord bas contre le mur. r = rayon, depth = saillie.
        /// </summary>
        public static GameObject QuarterDome(string name, Transform parent, Vector3 pos, float r, float depth, Material mat, float yaw = 0f)
        {
            var b = new MB();
            const int nA = 16, nB = 8;
            Vector3 P(float a, float e) => new Vector3(Mathf.Cos(a) * Mathf.Cos(e) * r, Mathf.Sin(a) * Mathf.Cos(e) * r, -Mathf.Sin(e) * depth);
            for (int i = 0; i < nA; i++)
                for (int j = 0; j < nB; j++)
                {
                    float a0 = Mathf.PI * i / nA, a1 = Mathf.PI * (i + 1) / nA;
                    float e0 = Mathf.PI * 0.5f * j / nB, e1 = Mathf.PI * 0.5f * (j + 1) / nB;
                    Vector3 p0 = P(a0, e0), p1 = P(a1, e0), p2 = P(a1, e1), p3 = P(a0, e1);
                    var mid = (p0 + p2) * 0.5f;
                    var nrm = new Vector3(mid.x / (r * r), mid.y / (r * r), mid.z / (depth * depth)).normalized;
                    b.Quad(p0, p1, p2, p3, new Vector2(a0 * r, e0 * r), new Vector2(a1 * r, e0 * r), new Vector2(a1 * r, e1 * r), new Vector2(a0 * r, e1 * r), nrm);
                }
            // Sous-face plane (demi-disque horizontal)
            for (int i = 0; i < nA; i++)
            {
                float a0 = Mathf.PI * i / nA, a1 = Mathf.PI * (i + 1) / nA;
                var q0 = new Vector3(-Mathf.Cos(a0) * r, 0f, -Mathf.Sin(a0) * depth);
                var q1 = new Vector3(-Mathf.Cos(a1) * r, 0f, -Mathf.Sin(a1) * depth);
                b.Tri(Vector3.zero, q0, q1, Vector2.zero, new Vector2(q0.x, q0.z), new Vector2(q1.x, q1.z), Vector3.down);
            }
            return MeshObject(name, parent, b.Build(name), mat, pos, new Vector3(0f, yaw, 0f));
        }

        /// <summary>
        /// Grille bombée (fenêtre en fer forgé « en ventre », façade du jardin persan) : panneau de largeur w et hauteur h
        /// qui se gonfle vers −Z local de « bulge » à mi-hauteur basse. UV en mètres. pos = bas-centre.
        /// </summary>
        public static GameObject Bombe(string name, Transform parent, Vector3 pos, float w, float h, float bulge, Material mat, float yaw = 0f)
        {
            var b = new MB();
            const int n = 12;
            float Z(float t) => -bulge * Mathf.Pow(Mathf.Sin(Mathf.PI * Mathf.Clamp01(t * 1.15f)), 1.3f);
            float len = 0f;
            for (int i = 0; i < n; i++)
            {
                float t0 = i / (float)n, t1 = (i + 1) / (float)n;
                var p0 = new Vector3(-w / 2, t0 * h, Z(t0)); var p1 = new Vector3(w / 2, t0 * h, Z(t0));
                var p2 = new Vector3(w / 2, t1 * h, Z(t1)); var p3 = new Vector3(-w / 2, t1 * h, Z(t1));
                float seg = Vector3.Distance(p0, p3);
                var nrm = Vector3.Cross(p1 - p0, p3 - p0).normalized;
                if (nrm.z > 0f) nrm = -nrm;
                b.Quad(p0, p1, p2, p3, new Vector2(0, len), new Vector2(w, len), new Vector2(w, len + seg), new Vector2(0, len + seg), nrm);
                len += seg;
            }
            return MeshObject(name, parent, b.Build(name), mat, pos, new Vector3(0f, yaw, 0f));
        }

        /// <summary>
        /// Relief lointain (montagne) : grille de hauteurs formée de pics gaussiens (x, z, hauteur, rayon) en coordonnées locales,
        /// sur un carré de côté « size ». Sert au Bou Kornine (deux cornes) et aux reliefs du cap Bon.
        /// </summary>
        public static GameObject Relief(string name, Transform parent, Vector3 pos, float size, Vector4[] peaks, Material mat, int res = 64)
        {
            var b = new MB();
            float H(float x, float z)
            {
                float y = 0f;
                foreach (var p in peaks)
                {
                    float d2 = ((x - p.x) * (x - p.x) + (z - p.y) * (z - p.y)) / (p.w * p.w);
                    y = Mathf.Max(y, p.z * Mathf.Exp(-d2));
                }
                return y - 2f;
            }
            Vector3 V(int i, int j) { float x = -size / 2 + size * i / res, z = -size / 2 + size * j / res; return new Vector3(x, H(x, z), z); }
            for (int i = 0; i < res; i++)
                for (int j = 0; j < res; j++)
                {
                    Vector3 a = V(i, j), c = V(i + 1, j), d = V(i + 1, j + 1), e = V(i, j + 1);
                    var nrm = Vector3.Cross(e - a, c - a).normalized;
                    if (nrm.y < 0f) nrm = -nrm;
                    b.Quad(a, c, d, e, new Vector2(a.x, a.z), new Vector2(c.x, c.z), new Vector2(d.x, d.z), new Vector2(e.x, e.z), nrm);
                }
            var go = MeshObject(name, parent, b.Build(name), mat, pos, Vector3.zero);
            go.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
            return go;
        }

        /// <summary>Terrain continu (grille de hauteurs h(x, z) en coordonnées monde), UV en mètres.</summary>
        public static GameObject Terrain(string name, Transform parent, Vector2 min, Vector2 max, int res, System.Func<float, float, float> h, Material mat)
        {
            var b = new MB();
            Vector3 V(int i, int j) { float x = Mathf.Lerp(min.x, max.x, i / (float)res), z = Mathf.Lerp(min.y, max.y, j / (float)res); return new Vector3(x, h(x, z), z); }
            for (int i = 0; i < res; i++)
                for (int j = 0; j < res; j++)
                {
                    Vector3 a = V(i, j), c = V(i + 1, j), d = V(i + 1, j + 1), e = V(i, j + 1);
                    var nrm = Vector3.Cross(e - a, c - a).normalized;
                    if (nrm.y < 0f) nrm = -nrm;
                    b.Quad(a, c, d, e, new Vector2(a.x, a.z), new Vector2(c.x, c.z), new Vector2(d.x, d.z), new Vector2(e.x, e.z), nrm);
                }
            return MeshObject(name, parent, b.Build(name), mat, Vector3.zero, Vector3.zero);
        }

        // ------------------------------------------------------------------ Végétation : cartes de feuillage
        /// <summary>
        /// Bouquets de feuillage : pour chaque point, trois cartes croisées (texture ajourée de feuilles) de taille sizes[i].
        /// Les normales partent du centre de la couronne (éclairage doux et volumique). Un seul maillage par couronne.
        /// </summary>
        public static GameObject Cards(string name, Transform parent, Vector3 pos, Vector3[] pts, float[] sizes, Vector3 center, Material mat)
        {
            var b = new MB();
            for (int i = 0; i < pts.Length; i++)
            {
                Vector3 p = pts[i];
                float s = sizes[i] * 0.5f;
                float yaw = BuildKit.R(0f, 180f), tilt = BuildKit.R(-20f, 20f);
                Vector3 d = p - center;
                Vector3 nrm = (d.sqrMagnitude < 1e-4f ? Vector3.up : d.normalized) + Vector3.up * 0.35f;
                nrm.Normalize();
                bool flip = BuildKit.Chance(0.5f);
                for (int k = 0; k < 3; k++)
                {
                    var rot = Quaternion.Euler(tilt, yaw + k * 60f, BuildKit.R(-12f, 12f));
                    Vector3 ax = rot * Vector3.right * s, ay = rot * Vector3.up * s;
                    float u0 = flip ? 1f : 0f, u1 = 1f - u0;
                    b.Quad(p - ax - ay, p + ax - ay, p + ax + ay, p - ax + ay,
                           new Vector2(u0, 0f), new Vector2(u1, 0f), new Vector2(u1, 1f), new Vector2(u0, 1f), nrm);
                }
            }
            return MeshObject(name, parent, b.Build(name), mat, pos, Vector3.zero);
        }

        /// <summary>Palmes arquées (palmiers) : n palmes de longueur len, en 5 segments qui montent puis retombent. pos = cœur de la couronne.</summary>
        public static GameObject Fronds(string name, Transform parent, Vector3 pos, int n, float len, float width, Material mat)
        {
            var b = new MB();
            for (int i = 0; i < n; i++)
            {
                float a = (i * 360f / n + BuildKit.R(-8f, 8f)) * Mathf.Deg2Rad;
                float lift = BuildKit.R(0.1f, 0.6f), droop = BuildKit.R(0.5f, 0.9f);
                float l = len * BuildKit.R(0.8f, 1.1f);
                var dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
                var side = new Vector3(-dir.z, 0f, dir.x);
                Vector3 P(float t) => dir * (l * t) + Vector3.up * (l * (lift * t - droop * t * t));
                const int seg = 5;
                for (int k = 0; k < seg; k++)
                {
                    float t0 = k / (float)seg, t1 = (k + 1) / (float)seg;
                    float w0 = width * (1f - 0.6f * t0) * 0.5f, w1 = width * (1f - 0.6f * t1) * 0.5f;
                    Vector3 a0 = P(t0) - side * w0, b0 = P(t0) + side * w0, a1 = P(t1) - side * w1, b1 = P(t1) + side * w1;
                    var nrm = (Vector3.Cross(b0 - a0, a1 - a0).normalized * 0.4f + Vector3.up).normalized;
                    b.Quad(a0, b0, b1, a1, new Vector2(t0, 0f), new Vector2(t0, 1f), new Vector2(t1, 1f), new Vector2(t1, 0f), nrm);
                }
            }
            return MeshObject(name, parent, b.Build(name), mat, pos, Vector3.zero);
        }

        const int Seg = 28;

        static Vector3 ArcPoint(float r, float cy, float a, float z) => new Vector3(Mathf.Cos(a) * r, cy + Mathf.Sin(a) * r, z);

        /// <summary>Tympan (au-dessus de la naissance) + intrados lisse d'un arc, centré en x = 0.</summary>
        static Mesh ArchMesh(float openW, float spring, float horseshoeDeg, float top, float thick, float uOffset)
        {
            var b = new MB();
            float r = openW * 0.5f, h = horseshoeDeg * Mathf.Deg2Rad;
            float cy = spring + r * Mathf.Sin(h);
            float a0 = -h, a1 = Mathf.PI + h;

            for (int side = -1; side <= 1; side += 2)
            {
                float z = side * thick * 0.5f;
                var nrm = new Vector3(0f, 0f, side);
                Vector2 U(Vector3 p) => new Vector2(p.x + uOffset, p.y);

                // Becs de l'arc outrepassé (entre la naissance et le centre), de chaque côté
                if (h > 0.001f)
                {
                    for (int s = -1; s <= 1; s += 2)
                    {
                        var corner = new Vector3(s * r, spring, z);
                        int steps = 6;
                        for (int k = 0; k < steps; k++)
                        {
                            float ka = -h + h * k / steps, kb = -h + h * (k + 1) / steps;
                            Vector3 pa = ArcPoint(r, cy, ka, z), pb = ArcPoint(r, cy, kb, z);
                            if (s < 0) { pa.x = -pa.x; pb.x = -pb.x; }
                            b.Tri(corner, pa, pb, U(corner), U(pa), U(pb), nrm);
                        }
                    }
                }
                // Partie haute : bandes verticales de l'arc jusqu'au sommet du mur
                for (int k = 0; k < Seg; k++)
                {
                    float ka = Mathf.PI * k / Seg, kb = Mathf.PI * (k + 1) / Seg;
                    Vector3 pa = ArcPoint(r, cy, ka, z), pb = ArcPoint(r, cy, kb, z);
                    Vector3 ta = new Vector3(pa.x, top, z), tb = new Vector3(pb.x, top, z);
                    b.Quad(pa, pb, tb, ta, U(pa), U(pb), U(tb), U(ta), nrm);
                }
            }
            // Intrados
            int total = Seg + 8;
            float len = 0f;
            for (int k = 0; k < total; k++)
            {
                float ka = a0 + (a1 - a0) * k / total, kb = a0 + (a1 - a0) * (k + 1) / total;
                Vector3 fa = ArcPoint(r, cy, ka, -thick * 0.5f), fb = ArcPoint(r, cy, kb, -thick * 0.5f);
                Vector3 ba = ArcPoint(r, cy, ka, thick * 0.5f), bb = ArcPoint(r, cy, kb, thick * 0.5f);
                float seg = Vector3.Distance(fa, fb);
                var mid = (fa + fb) * 0.5f;
                var nrm = new Vector3(-mid.x, cy - mid.y, 0f).normalized;
                b.Quad(fa, fb, bb, ba, new Vector2(len, 0f), new Vector2(len + seg, 0f), new Vector2(len + seg, thick), new Vector2(len, thick), nrm);
                len += seg;
            }
            return b.Build("Arc");
        }

        /// <summary>Bandeau (archivolte) qui entoure l'arc, légèrement en saillie sur les deux faces.</summary>
        static Mesh BandMesh(float openW, float spring, float horseshoeDeg, float thick, float bandW)
        {
            var b = new MB();
            float r = openW * 0.5f, h = horseshoeDeg * Mathf.Deg2Rad;
            float cy = spring + r * Mathf.Sin(h);
            float a0 = -h, a1 = Mathf.PI + h;
            int total = Seg + 8;
            for (int side = -1; side <= 1; side += 2)
            {
                float z = side * (thick * 0.5f + 0.015f);
                var nrm = new Vector3(0f, 0f, side);
                float len = 0f;
                for (int k = 0; k < total; k++)
                {
                    float ka = a0 + (a1 - a0) * k / total, kb = a0 + (a1 - a0) * (k + 1) / total;
                    Vector3 ia = ArcPoint(r, cy, ka, z), ib = ArcPoint(r, cy, kb, z);
                    Vector3 oa = ArcPoint(r + bandW, cy, ka, z), ob = ArcPoint(r + bandW, cy, kb, z);
                    float seg = Vector3.Distance(ia, ib);
                    b.Quad(ia, ib, ob, oa, new Vector2(len, 0f), new Vector2(len + seg, 0f), new Vector2(len + seg, bandW), new Vector2(len, bandW), nrm);
                    len += seg;
                }
                // Tranche extérieure du bandeau
            }
            return b.Build("Archivolte");
        }
    }
}
