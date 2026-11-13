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
