using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Végétation réaliste par « cartes » de feuillage : textures de feuilles ajourées générées par code (une par espèce),
    /// posées en bouquets de cartes croisées autour d'un cœur sombre qui donne la densité.
    /// Espèces relevées sur les photos : eucalyptus, cyprès, olivier, bigaradier, pin, palmier des Canaries,
    /// pittosporum (haies), bougainvillier, lantana, géranium, lavande, soucis.
    /// </summary>
    public static partial class Pal
    {
        static Material MatFeuillage(string name, System.Func<Texture2D> tex) => Cutout(BuildKit.TexMat(name, tex, Vector2.one, 0.15f, Color.white));

        public static Material CarteEucalyptus => MatFeuillage("Cartes_Eucalyptus", () => TexVeg.Eucalyptus);
        public static Material CarteOlivier => MatFeuillage("Cartes_Olivier", () => TexVeg.Olivier);
        public static Material CarteBigaradier => MatFeuillage("Cartes_Bigaradier", () => TexVeg.Bigaradier);
        public static Material CarteMaquis => MatFeuillage("Cartes_Maquis", () => TexVeg.Maquis);
        public static Material CarteCypres => MatFeuillage("Cartes_Cypres", () => TexVeg.Cypres);
        public static Material CartePin => MatFeuillage("Cartes_Pin", () => TexVeg.Pin);
        public static Material CarteBougainvillier => MatFeuillage("Cartes_Bougainvillier", () => TexVeg.Bougainvillier);
        public static Material CarteLantana => MatFeuillage("Cartes_Lantana", () => TexVeg.Lantana);
        public static Material CarteGeranium => MatFeuillage("Cartes_Geranium", () => TexVeg.Geranium);
        public static Material CarteSoucis => MatFeuillage("Cartes_Soucis", () => TexVeg.Soucis);
        public static Material CarteLavande => MatFeuillage("Cartes_Lavande", () => TexVeg.Lavande);
        public static Material PalmeCanaries => MatFeuillage("Palme_Canaries", () => TexVeg.Palme);

        // Cœurs de couronne (opaques, plus sombres : l'ombre interne du feuillage)
        public static Material CoeurEucalyptus => BuildKit.Mat("Coeur_Eucalyptus", new Color(0.2f, 0.25f, 0.18f), 0.05f);
        public static Material CoeurOlivier => BuildKit.Mat("Coeur_Olivier", new Color(0.26f, 0.3f, 0.22f), 0.05f);
        public static Material CoeurSombre => BuildKit.Mat("Coeur_Sombre", new Color(0.07f, 0.14f, 0.06f), 0.05f);
        public static Material CoeurCypres => BuildKit.Mat("Coeur_Cypres", new Color(0.05f, 0.11f, 0.05f), 0.05f);

        // Sols : terre sèche et herbe clairsemée sous les arbres (photos 01, 10, 129) ; maquis de la pente
        public static Material SolJardin => BuildKit.TexMat("Sol_Jardin_Terre_Herbe", () => TexVeg.SolJardin, Vector2.one / 4f, 0.05f, Color.white);
        public static Material SolMaquis => BuildKit.TexMat("Sol_Maquis_Pente", () => TexVeg.SolMaquis, Vector2.one / 6f, 0.04f, Color.white);
    }

    /// <summary>Textures de feuillage générées par code (alpha = feuilles).</summary>
    public static class TexVeg
    {
        static float Hh(int x, int y, int s)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + s * 1442695041);
                h = (h ^ (h >> 13)) * 1274126177u;
                h ^= h >> 16;
                return (h & 0xFFFFFF) / 16777216f;
            }
        }

        static readonly Color Vide = new Color(0f, 0f, 0f, 0f);

        /// <summary>
        /// Bouquet de feuilles en lentille dans un disque : cells = finesse de la grille, len/wid = demi-longueur et demi-largeur
        /// de la feuille (en UV), mode d'orientation 0 = aléatoire, 1 = pendantes (eucalyptus), 2 = rayonnantes.
        /// fleur : couleur de fleurs rondes (null = pas de fleurs), avec leur probabilité et leur rayon.
        /// </summary>
        static Color Bouquet(float u, float v, int cells, float len, float wid, Color c0, Color c1, int seed, float density, int mode,
                             Color? fleur = null, float pFleur = 0f, float rFleur = 0f, Color? fleur2 = null)
        {
            float dx = u - 0.5f, dy = v - 0.5f;
            float r = Mathf.Sqrt(dx * dx + dy * dy) * 2f;
            if (r > 1f) return Vide;
            int gx = Mathf.FloorToInt(u * cells), gy = Mathf.FloorToInt(v * cells);
            float best = -1f;
            Color col = Vide;
            for (int i = -2; i <= 2; i++)
                for (int j = -2; j <= 2; j++)
                {
                    int cx = gx + i, cy = gy + j;
                    float px = (cx + Hh(cx, cy, seed)) / cells, py = (cy + Hh(cx, cy, seed + 1)) / cells;
                    float rc = Mathf.Sqrt((px - 0.5f) * (px - 0.5f) + (py - 0.5f) * (py - 0.5f)) * 2f;
                    if (rc > 0.98f || Hh(cx, cy, seed + 3) > density * (1f - 0.75f * rc * rc)) continue;
                    float pri = Hh(cx, cy, seed + 4);
                    if (pri <= best) continue;
                    float ex = u - px, ey = v - py;
                    if (fleur.HasValue && Hh(cx, cy, seed + 5) < pFleur)
                    {
                        float d = Mathf.Sqrt(ex * ex + ey * ey);
                        if (d < rFleur)
                        {
                            // Petites fleurs agglomérées (ombelle) : grain plus sombre entre les fleurettes
                            float grain = Mathf.Repeat((ex * 97f + ey * 61f) * 7f, 1f);
                            var fc = fleur2.HasValue && Hh(cx, cy, seed + 6) < 0.5f ? fleur2.Value : fleur.Value;
                            col = fc * (0.8f + 0.25f * (1f - d / rFleur)) * (grain < 0.2f ? 0.8f : 1f);
                            col.a = 1f;
                            best = pri;
                        }
                        continue;
                    }
                    float ang;
                    if (mode == 1) ang = -Mathf.PI * 0.5f + (Hh(cx, cy, seed + 2) - 0.5f) * 1.6f;
                    else if (mode == 2) ang = Mathf.Atan2(py - 0.5f, px - 0.5f) + (Hh(cx, cy, seed + 2) - 0.5f) * 0.8f;
                    else ang = Hh(cx, cy, seed + 2) * Mathf.PI;
                    float ca = Mathf.Cos(ang), sa = Mathf.Sin(ang);
                    float lx = ex * ca + ey * sa, ly = -ex * sa + ey * ca;
                    lx -= len * 0.6f;   // la feuille part de son point d'attache
                    if (Mathf.Abs(lx) > len) continue;
                    float t = lx / len;
                    float w = wid * (1f - t * t);
                    if (Mathf.Abs(ly) > w) continue;
                    float shade = 0.72f + 0.28f * (1f - Mathf.Abs(ly) / Mathf.Max(w, 1e-4f));
                    if (Mathf.Abs(ly) < w * 0.12f) shade *= 0.85f;   // nervure
                    col = Color.Lerp(c0, c1, pri) * shade;
                    col.a = 1f;
                    best = pri;
                }
            return col;
        }

        // Eucalyptus : feuilles longues et étroites, pendantes, vert-gris (photos 01, 12, 32, 138)
        public static Texture2D Eucalyptus => BuildKit.ProcTexture("Feuilles_Eucalyptus", 512, 512, (u, v) =>
            Bouquet(u, v, 13, 0.075f, 0.014f, new Color(0.36f, 0.44f, 0.33f), new Color(0.56f, 0.6f, 0.46f), 11, 0.85f, 1));
        // Olivier : petites feuilles étroites, gris argenté
        public static Texture2D Olivier => BuildKit.ProcTexture("Feuilles_Olivier", 512, 512, (u, v) =>
            Bouquet(u, v, 18, 0.045f, 0.01f, new Color(0.42f, 0.47f, 0.37f), new Color(0.62f, 0.65f, 0.55f), 23, 0.9f, 0));
        // Bigaradier : feuilles ovales vert sombre luisant et quelques fruits orange (photos 130, 145)
        public static Texture2D Bigaradier => BuildKit.ProcTexture("Feuilles_Bigaradier", 512, 512, (u, v) =>
            Bouquet(u, v, 14, 0.05f, 0.022f, new Color(0.1f, 0.24f, 0.08f), new Color(0.26f, 0.42f, 0.14f), 37, 0.95f, 0,
                    new Color(0.95f, 0.5f, 0.06f), 0.04f, 0.025f));
        // Maquis, pittosporum, haies : feuilles ovales vert moyen
        public static Texture2D Maquis => BuildKit.ProcTexture("Feuilles_Maquis", 512, 512, (u, v) =>
            Bouquet(u, v, 16, 0.04f, 0.02f, new Color(0.13f, 0.25f, 0.1f), new Color(0.3f, 0.43f, 0.18f), 41, 0.95f, 0));
        // Cyprès : écailles serrées vert très sombre
        public static Texture2D Cypres => BuildKit.ProcTexture("Feuilles_Cypres", 512, 512, (u, v) =>
            Bouquet(u, v, 30, 0.028f, 0.012f, new Color(0.06f, 0.15f, 0.06f), new Color(0.2f, 0.32f, 0.14f), 53, 1f, 2));
        // Pin : aiguilles fines rayonnantes
        public static Texture2D Pin => BuildKit.ProcTexture("Aiguilles_Pin", 512, 512, (u, v) =>
            Bouquet(u, v, 22, 0.06f, 0.0045f, new Color(0.1f, 0.2f, 0.1f), new Color(0.22f, 0.32f, 0.16f), 61, 1f, 2));
        // Bougainvillier : bractées magenta mêlées de feuilles (photos 125, 127, 82)
        public static Texture2D Bougainvillier => BuildKit.ProcTexture("Bractees_Bougainvillier", 512, 512, (u, v) =>
            Bouquet(u, v, 15, 0.04f, 0.026f, new Color(0.12f, 0.24f, 0.09f), new Color(0.22f, 0.36f, 0.14f), 71, 0.95f, 0,
                    new Color(0.82f, 0.1f, 0.5f), 0.65f, 0.045f, new Color(0.7f, 0.05f, 0.42f)));
        // Lantana : feuilles vert franc et ombelles orange et rouges (photos 124, 138)
        public static Texture2D Lantana => BuildKit.ProcTexture("Fleurs_Lantana", 512, 512, (u, v) =>
            Bouquet(u, v, 15, 0.04f, 0.022f, new Color(0.14f, 0.28f, 0.1f), new Color(0.28f, 0.42f, 0.16f), 83, 0.95f, 0,
                    new Color(0.95f, 0.42f, 0.08f), 0.3f, 0.035f, new Color(0.85f, 0.15f, 0.12f)));
        // Géranium : feuilles lobées arrondies, quelques fleurs rouges
        public static Texture2D Geranium => BuildKit.ProcTexture("Feuilles_Geranium_Hiver", 512, 512, (u, v) =>
            Bouquet(u, v, 11, 0.05f, 0.045f, new Color(0.16f, 0.32f, 0.11f), new Color(0.3f, 0.47f, 0.18f), 97, 0.95f, 2,
                    new Color(0.85f, 0.12f, 0.12f), 0.012f, 0.03f));
        // Soucis : fleurs orange et jaunes (carré du jardin persan, photo 130)
        public static Texture2D Soucis => BuildKit.ProcTexture("Fleurs_Soucis", 512, 512, (u, v) =>
            Bouquet(u, v, 14, 0.04f, 0.014f, new Color(0.2f, 0.34f, 0.14f), new Color(0.32f, 0.45f, 0.2f), 101, 0.95f, 2,
                    new Color(0.98f, 0.55f, 0.05f), 0.35f, 0.035f, new Color(0.98f, 0.8f, 0.1f)));
        // Lavande / santoline : feuilles fines gris-vert, quelques épis mauves
        public static Texture2D Lavande => BuildKit.ProcTexture("Feuilles_Lavande", 512, 512, (u, v) =>
            Bouquet(u, v, 20, 0.045f, 0.007f, new Color(0.45f, 0.5f, 0.45f), new Color(0.62f, 0.66f, 0.6f), 113, 0.95f, 2,
                    new Color(0.52f, 0.42f, 0.66f), 0.06f, 0.02f));

        /// <summary>Palme pennée (u = le long de la palme, v = en travers) : rachis central et folioles obliques.</summary>
        public static Texture2D Palme => BuildKit.ProcTexture("Palme_Canaries", 512, 256, (u, v) =>
        {
            float d = Mathf.Abs(v - 0.5f) * 2f;
            if (d < 0.045f) return new Color(0.42f, 0.4f, 0.22f, 1f);
            if (u < 0.08f) return Vide;
            float taper = 0.95f * Mathf.Pow(1f - u, 0.5f) + 0.05f;
            if (d > taper) return Vide;
            float phase = Mathf.Repeat(u * 42f + d * 5f, 1f);
            if (phase > 0.42f) return Vide;
            var c = Color.Lerp(new Color(0.18f, 0.32f, 0.12f), new Color(0.38f, 0.48f, 0.2f), d * 0.7f + Hh((int)(u * 42f), v > 0.5f ? 1 : 0, 5) * 0.3f);
            c *= 0.8f + 0.2f * (1f - phase / 0.42f);
            c.a = 1f;
            return c;
        });

        public static Texture2D SolJardin => BuildKit.ProcTexture("Sol_Jardin_Terre_Herbe", 512, 512, (u, v) =>
        {
            float n = Mathf.PerlinNoise(u * 5f + 3.1f, v * 5f + 7.7f) * 0.7f + Mathf.PerlinNoise(u * 23f, v * 23f) * 0.3f;
            var terre = new Color(0.4f, 0.33f, 0.24f);
            var herbe = new Color(0.3f, 0.37f, 0.18f);
            var c = Color.Lerp(terre, herbe, Mathf.SmoothStep(0.35f, 0.65f, n));
            float grain = Mathf.PerlinNoise(u * 120f, v * 120f);
            c *= 0.85f + 0.3f * grain;
            if (Hh((int)(u * 160f), (int)(v * 160f), 7) < 0.04f) c = Color.Lerp(c, new Color(0.55f, 0.45f, 0.3f), 0.6f);   // feuilles mortes, cailloux
            c.a = 1f;
            return c;
        });

        public static Texture2D SolMaquis => BuildKit.ProcTexture("Sol_Maquis_Pente", 512, 512, (u, v) =>
        {
            float n = Mathf.PerlinNoise(u * 4f + 1.3f, v * 4f + 2.9f) * 0.7f + Mathf.PerlinNoise(u * 19f, v * 19f) * 0.3f;
            var c = Color.Lerp(new Color(0.36f, 0.31f, 0.23f), new Color(0.2f, 0.27f, 0.13f), Mathf.SmoothStep(0.3f, 0.6f, n));
            c *= 0.85f + 0.3f * Mathf.PerlinNoise(u * 90f, v * 90f);
            c.a = 1f;
            return c;
        });
    }

    /// <summary>Couronnes de feuillage et plantes construites avec les cartes.</summary>
    public static class Veg
    {
        /// <summary>
        /// Couronne : n bouquets répartis dans un ellipsoïde (rayons radii, centre local c), cœur opaque sombre à 60 %.
        /// </summary>
        public static void Couronne(Transform parent, Vector3 c, Vector3 radii, int n, float carte, Material feuilles, Material coeur, float coeurEchelle = 0.62f)
        {
            var pts = new Vector3[n];
            var sz = new float[n];
            for (int i = 0; i < n; i++)
            {
                Vector3 d;
                do d = new Vector3(BuildKit.R(-1f, 1f), BuildKit.R(-1f, 1f), BuildKit.R(-1f, 1f)); while (d.sqrMagnitude > 1f);
                d = d.normalized * Mathf.Pow(BuildKit.R(0.35f, 1f), 0.5f);   // plutôt vers la surface
                pts[i] = c + Vector3.Scale(d, radii);
                sz[i] = carte * BuildKit.R(0.8f, 1.2f);
            }
            MeshKit.Cards("Feuillage", parent, Vector3.zero, pts, sz, c, feuilles);
            if (coeur != null)
                BuildKit.Sph("Coeur", parent, c, radii * 2f * coeurEchelle, coeur);
        }

        /// <summary>Haie taillée feuillue entre a et b (cœur opaque + cartes en surface).</summary>
        public static void Haie(Transform parent, Vector3 a, Vector3 b, float h, float w, Material feuilles, Material coeur)
        {
            var g = BuildKit.Group("Haie_Feuillue", parent);
            BuildKit.Wall("Coeur_Haie", g, a, b, h * 0.92f, w * 0.85f, coeur);
            Vector3 d = b - a; d.y = 0f;
            float len = d.magnitude;
            Vector3 dir = d / Mathf.Max(len, 0.01f), side = new Vector3(-dir.z, 0f, dir.x);
            int n = Mathf.RoundToInt(len * h * 2.2f);
            var pts = new Vector3[n];
            var sz = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = BuildKit.R(0f, 1f);
                int face = BuildKit.RI(0, 3);
                float y = face == 2 ? h - 0.1f : BuildKit.R(0.2f, h - 0.15f);
                float sOff = face == 0 ? w * 0.45f : face == 1 ? -w * 0.45f : BuildKit.R(-w * 0.4f, w * 0.4f);
                pts[i] = Vector3.Lerp(a, b, t) + side * sOff + Vector3.up * y;
                sz[i] = BuildKit.R(0.6f, 0.9f);
            }
            MeshKit.Cards("Feuillage", g, Vector3.zero, pts, sz, (a + b) * 0.5f + Vector3.up * (h * 0.4f), feuilles);
        }

        /// <summary>Massif bas (géraniums, soucis, lavandes) couvrant un rectangle sx × sz à la hauteur y.</summary>
        public static void Massif(Transform parent, Vector3 c, float sx, float sz, float h, Material feuilles, float densite = 4f)
        {
            int n = Mathf.Max(4, Mathf.RoundToInt(sx * sz * densite));
            var pts = new Vector3[n];
            var s = new float[n];
            for (int i = 0; i < n; i++)
            {
                pts[i] = c + new Vector3(BuildKit.R(-sx * 0.5f, sx * 0.5f), BuildKit.R(h * 0.3f, h), BuildKit.R(-sz * 0.5f, sz * 0.5f));
                s[i] = BuildKit.R(0.45f, 0.7f);
            }
            MeshKit.Cards("Massif", parent, Vector3.zero, pts, s, c, feuilles);
        }

        /// <summary>Olivier : tronc noueux, trois couronnes argentées (photo 147).</summary>
        public static void Olivier(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Olivier", parent, pos, new Vector3(0f, BuildKit.R(0f, 360f), 0f));
            BuildKit.Cyl("Tronc", g, new Vector3(0f, 0.8f, 0f), 0.22f, 1.6f, Pal.Tronc, new Vector3(BuildKit.R(-8f, 8f), 0f, BuildKit.R(-8f, 8f)), true);
            for (int i = 0; i < 3; i++)
            {
                float a = i * 2.1f + BuildKit.R(-0.3f, 0.3f);
                var c = new Vector3(Mathf.Cos(a) * 0.9f, BuildKit.R(2.3f, 3.0f), Mathf.Sin(a) * 0.9f);
                var d = c - new Vector3(0f, 1.5f, 0f);
                BuildKit.Cyl("Branche", g, new Vector3(0f, 1.5f, 0f) + d * 0.5f, 0.09f, d.magnitude, Pal.Tronc, Quaternion.FromToRotation(Vector3.up, d.normalized).eulerAngles);
                Couronne(g, c, new Vector3(1.4f, 0.9f, 1.4f), 14, 1.2f, Pal.CarteOlivier, Pal.CoeurOlivier);
            }
        }
    }
}
