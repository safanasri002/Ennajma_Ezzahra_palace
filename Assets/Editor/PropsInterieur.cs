using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>Matériaux d'intérieur : marbres, bois sculpté, stucs, vitraux, tissus (textures générées par code).</summary>
    public static partial class Pal
    {
        // ---------------------------------------------------------------- Marbres et sols
        public static Material MarbreRose => BuildKit.TexMat("Marbre_Rose", () => Tex.MarbreRose, Vector2.one, 0.7f, Color.white);
        public static Material MarbreVeine => BuildKit.TexMat("Marbre_Veine_Gris", () => Tex.MarbreGris, Vector2.one, 0.6f, Color.white);
        public static Material MarbreGris => BuildKit.Mat("Marbre_Gris", new Color(0.55f, 0.57f, 0.6f), 0.7f);

        /// <summary>Dallage de marbre à hexagones blancs et gris (un hexagone ≈ 0,5 m).</summary>
        public static Material SolHexagones(float sx, float sz) =>
            BuildKit.TexMat($"Sol_Hexagones_{sx:0.0}x{sz:0.0}", () => Tex.Hexagones, new Vector2(sx / 0.97f, sz / 0.84f), 0.65f, Color.white);

        /// <summary>Dallage de marbre en losanges blancs et gris (motif de 0,6 m).</summary>
        public static Material SolLosanges(float sx, float sz) =>
            BuildKit.TexMat($"Sol_Losanges_{sx:0.0}x{sz:0.0}", () => Tex.Losanges, new Vector2(sx / 0.6f, sz / 0.6f), 0.65f, Color.white);

        // ---------------------------------------------------------------- Bois, stucs
        public static Material BoisSculpte => BuildKit.TexMat("Bois_Sculpte", () => Tex.Bois, Vector2.one, 0.35f, Color.white);
        public static Material BoisPlafond(float sx, float sz) =>
            BuildKit.TexMat($"Bois_Plafond_{sx:0}x{sz:0}", () => Tex.Bois, new Vector2(sx / 1.5f, sz / 1.5f), 0.3f, Color.white);
        public static Material BoisDore => BuildKit.Mat("Bois_Dore", new Color(0.72f, 0.5f, 0.22f), 0.45f, 0.2f);
        public static Material BoisClair => BuildKit.Mat("Bois_Clair", new Color(0.62f, 0.42f, 0.24f), 0.4f);
        public static Material Stuc => BuildKit.Mat("Stuc", new Color(0.93f, 0.9f, 0.82f), 0.1f);

        /// <summary>Stuc ciselé (naqch hadida) : un motif d'étoile par carré de côté = hauteur de la frise.</summary>
        public static Material StucCisele(float length, float height) =>
            BuildKit.TexMat($"Stuc_Cisele_{length:0.0}x{height:0.0}", () => Tex.Stuc,
                            new Vector2(Mathf.Max(1f, Mathf.Round(length / height)), 1f), 0.12f, Color.white);

        // ---------------------------------------------------------------- Verre et lumière du jour
        public static Material Vitrail => BuildKit.TexMat("Vitrail_Qamariya", () => Tex.Vitrail, new Vector2(7f, 7f), 0.6f, Color.white, 0.9f);
        public static Material VitreJour => BuildKit.Emissive("Vitre_Jour", new Color(0.8f, 0.88f, 0.95f), new Color(0.75f, 0.85f, 1f) * 1.3f);
        public static Material Voilage => BuildKit.Emissive("Voilage", new Color(0.95f, 0.95f, 0.92f), new Color(1f, 0.98f, 0.92f) * 0.9f);
        public static Material LustreVerre => BuildKit.Emissive("Lustre_Verre", new Color(0.7f, 0.8f, 0.88f), new Color(0.6f, 0.75f, 0.9f) * 0.6f);
        public static Material Verre => BuildKit.Mat("Verre", new Color(0.85f, 0.9f, 0.92f), 0.95f);

        // ---------------------------------------------------------------- Métaux, tissus, peintures
        public static Material Cuivre => BuildKit.Mat("Cuivre", new Color(0.78f, 0.45f, 0.25f), 0.75f, 1f);
        public static Material CuivreGrave => BuildKit.Mat("Cuivre_Grave", new Color(0.45f, 0.25f, 0.14f), 0.6f, 1f);
        public static Material Velours => BuildKit.Mat("Velours_Dore", new Color(0.72f, 0.6f, 0.38f), 0.15f);
        public static Material Tapis => BuildKit.TexMat("Tapis_Rouge", () => Tex.Tapis, Vector2.one, 0.05f, Color.white);
        public static Material FeutreVert => BuildKit.Mat("Feutre_Vert", new Color(0.06f, 0.3f, 0.2f), 0.05f);
        public static Material Canne => BuildKit.Mat("Cannage", new Color(0.82f, 0.72f, 0.5f), 0.1f);
        public static Material PeintureCreme => BuildKit.Mat("Peinture_Creme", new Color(0.92f, 0.89f, 0.8f), 0.25f);
        public static Material PeintureVieillie => BuildKit.Mat("Peinture_Vieillie", new Color(0.86f, 0.85f, 0.8f), 0.15f);
        public static Material PanneauAjoure => BuildKit.TexMat("Panneau_Ajoure", () => Tex.Stuc, new Vector2(2f, 2f), 0.1f, new Color(0.88f, 0.87f, 0.83f));
        public static Material PeintureFleur => BuildKit.Mat("Peinture_Fleur", new Color(0.42f, 0.5f, 0.3f), 0.2f);
        public static Material Bakelite => BuildKit.Mat("Bakelite_Brune", new Color(0.22f, 0.13f, 0.08f), 0.5f);
        public static Material Noir => BuildKit.Mat("Noir_Brillant", new Color(0.03f, 0.03f, 0.03f), 0.7f);
        public static Material Nacre => BuildKit.Mat("Nacre", new Color(0.9f, 0.88f, 0.82f), 0.8f);
        public static Material Cadran => BuildKit.Mat("Cadran", new Color(0.95f, 0.93f, 0.86f), 0.4f);
        public static Material Cordon => BuildKit.Mat("Cordon_Orange", new Color(0.75f, 0.4f, 0.18f), 0.2f);

        public static Material Coussin(int i)
        {
            switch (((i % 3) + 3) % 3)
            {
                case 0: return BuildKit.Mat("Coussin_Ocre", new Color(0.74f, 0.6f, 0.36f), 0.15f);
                case 1: return BuildKit.Mat("Coussin_Sable", new Color(0.82f, 0.72f, 0.52f), 0.15f);
                default: return BuildKit.Mat("Coussin_Brun", new Color(0.6f, 0.45f, 0.28f), 0.15f);
            }
        }
    }

    /// <summary>Générateurs de textures (tous les motifs répétés sont raccordables : fréquences entières).</summary>
    public static class Tex
    {
        const float TwoPi = Mathf.PI * 2f;

        public static Texture2D Hexagones => BuildKit.ProcTexture("Tex_Sol_Hexagones_v1", 296, 256, HexFloor);
        public static Texture2D Losanges => BuildKit.ProcTexture("Tex_Sol_Losanges_v1", 256, 256, Losange);
        public static Texture2D MarbreRose => BuildKit.ProcTexture("Tex_Marbre_Rose_v1", 256, 256,
            (u, v) => Marble(u, v, new Color(0.8f, 0.56f, 0.5f), new Color(0.68f, 0.42f, 0.38f), new Color(0.93f, 0.82f, 0.78f)));
        public static Texture2D MarbreGris => BuildKit.ProcTexture("Tex_Marbre_Gris_v1", 256, 256,
            (u, v) => Marble(u, v, new Color(0.88f, 0.88f, 0.88f), new Color(0.78f, 0.79f, 0.81f), new Color(0.45f, 0.47f, 0.5f)));
        public static Texture2D Bois => BuildKit.ProcTexture("Tex_Bois_v1", 256, 256, Wood);
        public static Texture2D Stuc => BuildKit.ProcTexture("Tex_Stuc_Cisele_v1", 256, 256, Stucco);
        public static Texture2D Vitrail => BuildKit.ProcTexture("Tex_Vitrail_v1", 256, 256, StainedGlass);
        public static Texture2D Tapis => BuildKit.ProcTexture("Tex_Tapis_v1", 256, 384, Rug);

        static float SS(float e0, float e1, float x)
        {
            float t = Mathf.Clamp01((x - e0) / (e1 - e0));
            return t * t * (3f - 2f * t);
        }

        /// <summary>Bruit doux et raccordable (somme de sinus à fréquences entières), dans [-1, 1].</summary>
        static float Wave(float u, float v) =>
            0.5f * Mathf.Sin(TwoPi * (2f * u + 3f * v) + 1.3f)
          + 0.3f * Mathf.Sin(TwoPi * (5f * u - 2f * v) + 0.4f)
          + 0.2f * Mathf.Sin(TwoPi * (7f * u + 5f * v) + 2.1f);

        static float Vein(float u, float v)
        {
            float s = Mathf.Abs(Mathf.Sin(TwoPi * (u + 2f * v) + 2.2f * Wave(u, v)));
            return 1f - SS(0f, 0.08f, s);
        }

        static Color Marble(float u, float v, Color a, Color b, Color vein)
        {
            Color c = Color.Lerp(a, b, 0.5f + 0.5f * Wave(u * 1f, v * 1f));
            return Color.Lerp(c, vein, Vein(u, v) * 0.8f);
        }

        // Dallage d'hexagones : 4 hexagones par motif (dont 1 gris), joints gris clair.
        static Color HexFloor(float u, float v)
        {
            float W = Mathf.Sqrt(3f);
            float x = u * 2f * W, y = v * 3f;
            float d1 = 1e9f, d2 = 1e9f;
            int bc = 0, br = 0;
            for (int r = -1; r <= 2; r++)
            {
                float cy = r * 1.5f;
                float off = (r & 1) != 0 ? W * 0.5f : 0f;
                for (int c = -1; c <= 2; c++)
                {
                    float dx = x - (c * W + off), dy = y - cy;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d < d1) { d2 = d1; d1 = d; bc = c; br = r; }
                    else if (d < d2) d2 = d;
                }
            }
            if (d2 - d1 < 0.07f) return new Color(0.62f, 0.6f, 0.58f);
            bool grey = (((br % 2) + 2) % 2) == 1 && (((bc % 2) + 2) % 2) == 0;
            Color baseC = grey ? new Color(0.6f, 0.62f, 0.65f) : new Color(0.94f, 0.94f, 0.92f);
            baseC *= 0.95f + 0.05f * Wave(u, v);
            return Color.Lerp(baseC, new Color(0.55f, 0.56f, 0.58f), Vein(u, v) * 0.35f);
        }

        static Color Losange(float u, float v)
        {
            float d = Mathf.Abs(u - 0.5f) + Mathf.Abs(v - 0.5f);
            if (Mathf.Abs(d - 0.5f) < 0.018f) return new Color(0.6f, 0.6f, 0.6f);
            Color c = d < 0.5f ? new Color(0.94f, 0.94f, 0.92f) : new Color(0.5f, 0.52f, 0.55f);
            return Color.Lerp(c * (0.96f + 0.04f * Wave(u, v)), new Color(0.6f, 0.6f, 0.62f), Vein(u, v) * 0.3f);
        }

        static Color Wood(float u, float v)
        {
            float g = Mathf.Sin(TwoPi * (24f * u) + 3f * Wave(u, v));
            Color c = Color.Lerp(new Color(0.58f, 0.37f, 0.17f), new Color(0.42f, 0.25f, 0.1f), 0.5f + 0.5f * g);
            float line = 1f - SS(0f, 0.15f, Mathf.Abs(Mathf.Sin(TwoPi * 6f * u + Wave(u, v))));
            return Color.Lerp(c, new Color(0.3f, 0.17f, 0.07f), line * 0.4f);
        }

        // Stuc ciselé : étoile à 8 branches (deux carrés), rosace centrale, cadre de la cellule ; les creux sont ombrés.
        static Color Stucco(float u, float v)
        {
            float x = u - 0.5f, y = v - 0.5f;
            float s1 = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
            float s2 = (Mathf.Abs(x) + Mathf.Abs(y)) / Mathf.Sqrt(2f);
            float len = Mathf.Sqrt(x * x + y * y);
            const float r = 0.3f, w = 0.022f;
            bool groove = Mathf.Abs(s1 - r) < w || Mathf.Abs(s2 - r) < w || Mathf.Abs(len - 0.12f) < w
                       || s1 > 0.47f || Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(y)) < w * 0.7f && len > 0.14f && s2 < r;
            Color cream = new Color(0.93f, 0.89f, 0.8f);
            if (groove) return new Color(0.6f, 0.54f, 0.44f);
            bool inStar = s1 < r || s2 < r;
            return (inStar ? cream : cream * 0.94f) * (0.97f + 0.03f * Wave(u, v));
        }

        // Vitrail : étoile jaune, quarts colorés (rouge, vert, bleu, violet), cœur orange, sertis de stuc blanc.
        static Color StainedGlass(float u, float v)
        {
            float x = u - 0.5f, y = v - 0.5f;
            float s1 = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
            float s2 = (Mathf.Abs(x) + Mathf.Abs(y)) / Mathf.Sqrt(2f);
            float len = Mathf.Sqrt(x * x + y * y);
            const float r = 0.3f, w = 0.03f;
            if (Mathf.Abs(s1 - r) < w || Mathf.Abs(s2 - r) < w || Mathf.Abs(len - 0.12f) < w || s1 > 0.46f)
                return new Color(0.95f, 0.93f, 0.88f);
            if (len < 0.12f) return new Color(1f, 0.55f, 0.15f);
            if (s1 < r || s2 < r) return new Color(1f, 0.8f, 0.25f);
            if (x > 0f && y > 0f) return new Color(0.85f, 0.15f, 0.18f);
            if (x < 0f && y > 0f) return new Color(0.15f, 0.6f, 0.3f);
            if (x < 0f) return new Color(0.2f, 0.35f, 0.85f);
            return new Color(0.55f, 0.25f, 0.75f);
        }

        // Tapis : bordures bleu nuit et crème, champ rouge semé de petits losanges, médaillon central.
        static Color Rug(float u, float v)
        {
            float e = Mathf.Min(Mathf.Min(u, 1f - u), Mathf.Min(v, 1f - v));
            Color navy = new Color(0.12f, 0.15f, 0.33f), cream = new Color(0.86f, 0.78f, 0.6f), red = new Color(0.55f, 0.09f, 0.08f);
            if (e < 0.035f) return navy;
            if (e < 0.075f)
            {
                float m = Mathf.Abs(Mathf.Repeat(u * 20f, 1f) - 0.5f) + Mathf.Abs(Mathf.Repeat(v * 20f, 1f) - 0.5f);
                return m < 0.3f ? red : cream;
            }
            if (e < 0.085f) return navy;
            float x = (u - 0.5f) / 0.26f, y = (v - 0.5f) / 0.3f;
            float d = Mathf.Abs(x) + Mathf.Abs(y);
            if (d < 1f) return d < 0.45f ? cream : (d < 0.55f ? red : navy);
            float k = Mathf.Abs(Mathf.Repeat(u * 9f, 1f) - 0.5f) + Mathf.Abs(Mathf.Repeat(v * 12f, 1f) - 0.5f);
            return k < 0.12f ? cream : red * (0.95f + 0.05f * Wave(u, v));
        }
    }

    /// <summary>
    /// Accessoires d'intérieur réutilisables (hall, salons, chambres…).
    /// Convention : pos = au sol (ou sur la surface du mur), face avant de l'objet vers -Z local.
    /// </summary>
    public static class PropsInterieur
    {
        // ================================================================ Architecture intérieure
        /// <summary>Plafond en bois à caissons. center = dessous des poutres (au plafond), au centre de la pièce.</summary>
        public static void PlafondCaissons(Transform parent, Vector3 center, float sx, float sz, float cell, float beamDepth = 0.25f)
        {
            var g = BuildKit.Group("Plafond_Caissons", parent, center);
            BuildKit.Box("Fond", g, new Vector3(0f, beamDepth + 0.1f, 0f), new Vector3(sx + 0.6f, 0.2f, sz + 0.6f), Pal.BoisPlafond(sx, sz), 0f, false);

            int nx = Mathf.Max(1, Mathf.RoundToInt(sx / cell)), nz = Mathf.Max(1, Mathf.RoundToInt(sz / cell));
            float cx = sx / nx, cz = sz / nz;
            for (int i = 0; i <= nx; i++)
                BuildKit.Box("Poutre_Z", g, new Vector3(-sx * 0.5f + i * cx, beamDepth * 0.5f, 0f), new Vector3(0.16f, beamDepth, sz), Pal.BoisSombre, 0f, false);
            for (int j = 0; j <= nz; j++)
                BuildKit.Box("Poutre_X", g, new Vector3(0f, beamDepth * 0.5f, -sz * 0.5f + j * cz), new Vector3(sx, beamDepth, 0.16f), Pal.BoisSombre, 0f, false);

            float star = Mathf.Min(cx, cz) * 0.42f;
            for (int i = 0; i < nx; i++)
                for (int j = 0; j < nz; j++)
                {
                    var p = new Vector3(-sx * 0.5f + (i + 0.5f) * cx, beamDepth - 0.03f, -sz * 0.5f + (j + 0.5f) * cz);
                    BuildKit.Box("Etoile_A", g, p, new Vector3(star, 0.05f, star), Pal.BoisDore, 0f, false);
                    BuildKit.Box("Etoile_B", g, p, new Vector3(star, 0.05f, star), Pal.BoisDore, 45f, false);
                    BuildKit.Sph("Pendentif", g, p - new Vector3(0f, 0.06f, 0f), new Vector3(0.12f, 0.1f, 0.12f), Pal.BoisSombre);
                }

            // Corniche
            BuildKit.Box("Corniche_N", g, new Vector3(0f, -0.05f, sz * 0.5f), new Vector3(sx + 0.3f, 0.3f, 0.3f), Pal.BoisSombre, 0f, false);
            BuildKit.Box("Corniche_S", g, new Vector3(0f, -0.05f, -sz * 0.5f), new Vector3(sx + 0.3f, 0.3f, 0.3f), Pal.BoisSombre, 0f, false);
            BuildKit.Box("Corniche_E", g, new Vector3(sx * 0.5f, -0.05f, 0f), new Vector3(0.3f, 0.3f, sz + 0.3f), Pal.BoisSombre, 0f, false);
            BuildKit.Box("Corniche_O", g, new Vector3(-sx * 0.5f, -0.05f, 0f), new Vector3(0.3f, 0.3f, sz + 0.3f), Pal.BoisSombre, 0f, false);
        }

        /// <summary>Frise de stuc ciselé posée contre un mur (center = milieu de la frise, longueur selon X local).</summary>
        public static void FriseStuc(Transform parent, Vector3 center, float length, float height, float yaw)
        {
            var g = BuildKit.Group("Frise_Stuc", parent, center, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Frise", g, Vector3.zero, new Vector3(length, height, 0.05f), Pal.StucCisele(length, height), 0f, false);
            BuildKit.Box("Moulure_Haut", g, new Vector3(0f, height * 0.5f, 0f), new Vector3(length, 0.06f, 0.1f), Pal.Stuc, 0f, false);
            BuildKit.Box("Moulure_Bas", g, new Vector3(0f, -height * 0.5f, 0f), new Vector3(length, 0.06f, 0.1f), Pal.Stuc, 0f, false);
        }

        /// <summary>Plaque mince contre un mur (lambris de marbre…). center = milieu ; longueur selon X local.</summary>
        public static void Panneau(Transform parent, string name, Vector3 center, float length, float height, float yaw, Material m, bool filetNoir = false)
        {
            BuildKit.Box(name, parent, center, new Vector3(length, height, 0.04f), m, yaw, false);
            if (filetNoir)
                BuildKit.Box(name + "_Filet", parent, center + Vector3.up * (height * 0.5f + 0.015f), new Vector3(length, 0.03f, 0.05f), Pal.Noir, yaw, false);
        }

        /// <summary>Vitrail en arc (qamariya). pos = bas-centre sur le mur ; hRect ≥ w/2.</summary>
        public static void Qamariya(Transform parent, Vector3 pos, float yaw, float w, float hRect)
        {
            var g = BuildKit.Group("Qamariya", parent, pos, new Vector3(0f, yaw, 0f));
            float total = hRect + w * 0.5f;
            BuildKit.Box("Cadre_Stuc", g, new Vector3(0f, total * 0.5f, -0.02f), new Vector3(w + 0.3f, total + 0.16f, 0.04f), Pal.StucCisele(w + 0.3f, w + 0.3f), 0f, false);
            BuildKit.Box("Vitrail", g, new Vector3(0f, hRect * 0.5f, -0.05f), new Vector3(w, hRect, 0.02f), Pal.Vitrail, 0f, false);
            BuildKit.Cyl("Vitrail_Arc", g, new Vector3(0f, hRect, -0.05f), w * 0.5f, 0.02f, Pal.Vitrail, new Vector3(90f, 0f, 0f));
            BuildKit.Arch("Bordure", g, new Vector3(0f, 0f, -0.07f), 0f, w, hRect, 0.05f, 0.07f, Pal.Stuc, 0f, 12, false);
            BuildKit.Box("Bordure_G", g, new Vector3(-w * 0.5f - 0.035f, hRect * 0.5f, -0.07f), new Vector3(0.07f, hRect, 0.05f), Pal.Stuc, 0f, false);
            BuildKit.Box("Bordure_D", g, new Vector3(w * 0.5f + 0.035f, hRect * 0.5f, -0.07f), new Vector3(0.07f, hRect, 0.05f), Pal.Stuc, 0f, false);
            BuildKit.Box("Appui", g, new Vector3(0f, -0.03f, -0.07f), new Vector3(w + 0.14f, 0.06f, 0.06f), Pal.Stuc, 0f, false);
        }

        /// <summary>Fenêtre vue de l'intérieur : voilage lumineux, croisillons, volets de bois ouverts. pos = bas-centre sur le mur.</summary>
        public static void FenetreInterieure(Transform parent, Vector3 pos, float yaw, float w, float h)
        {
            var g = BuildKit.Group("Fenetre", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Jour", g, new Vector3(0f, h * 0.5f, -0.02f), new Vector3(w, h, 0.02f), Pal.VitreJour, 0f, false);
            for (int i = 1; i < 3; i++)
                BuildKit.Box("Montant", g, new Vector3(-w * 0.5f + i * w / 3f, h * 0.5f, -0.04f), new Vector3(0.04f, h, 0.03f), Pal.PeintureCreme, 0f, false);
            for (int j = 1; j < 4; j++)
                BuildKit.Box("Traverse", g, new Vector3(0f, j * h / 4f, -0.04f), new Vector3(w, 0.04f, 0.03f), Pal.PeintureCreme, 0f, false);
            BuildKit.Box("Voilage", g, new Vector3(0f, h * 0.55f, -0.07f), new Vector3(w * 0.96f, h * 0.9f, 0.01f), Pal.Voilage, 0f, false);
            float sx = w * 0.5f + w * 0.25f + 0.08f;
            BuildKit.Box("Volet_G", g, new Vector3(-sx, h * 0.5f, -0.06f), new Vector3(w * 0.5f, h, 0.05f), Pal.BoisSculpte, 0f, false);
            BuildKit.Box("Volet_D", g, new Vector3(sx, h * 0.5f, -0.06f), new Vector3(w * 0.5f, h, 0.05f), Pal.BoisSculpte, 0f, false);
            BuildKit.Box("Encadrement_H", g, new Vector3(0f, h + 0.06f, -0.04f), new Vector3(w + 0.2f, 0.12f, 0.08f), Pal.Stuc, 0f, false);
            BuildKit.Box("Appui", g, new Vector3(0f, -0.05f, -0.1f), new Vector3(w + 0.25f, 0.1f, 0.2f), Pal.Marbre, 0f, false);
        }

        public static void ColonneMarbre(Transform parent, Vector3 pos, float height, float radius, Material shaft)
        {
            var g = BuildKit.Group("Colonne", parent, pos);
            BuildKit.Box("Base", g, new Vector3(0f, 0.1f, 0f), new Vector3(radius * 2.6f, 0.2f, radius * 2.6f), Pal.Marbre, 0f, false);
            float len = height - 0.5f;
            BuildKit.Cyl("Fut", g, new Vector3(0f, 0.2f + len * 0.5f, 0f), radius, len, shaft, default, true);
            BuildKit.Cyl("Astragale", g, new Vector3(0f, height - 0.32f, 0f), radius * 1.15f, 0.05f, Pal.Marbre);
            BuildKit.Box("Chapiteau", g, new Vector3(0f, height - 0.15f, 0f), new Vector3(radius * 2.8f, 0.3f, radius * 2.8f), Pal.Marbre, 0f, false);
        }

        // ================================================================ Mobilier du bit gaâd
        /// <summary>Divan bas à coussins, adossé au mur (dos vers +Z local).</summary>
        public static void Divan(Transform parent, Vector3 pos, float yaw, float length, float depth = 0.85f, Material velours = null)
        {
            var g = BuildKit.Group("Divan", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Socle", g, new Vector3(0f, 0.18f, 0f), new Vector3(length, 0.36f, depth), Pal.BoisSculpte);
            BuildKit.Box("Facade_Doree", g, new Vector3(0f, 0.2f, -depth * 0.5f - 0.01f), new Vector3(length - 0.1f, 0.2f, 0.02f), Pal.BoisDore, 0f, false);
            BuildKit.Box("Matelas", g, new Vector3(0f, 0.45f, 0f), new Vector3(length - 0.04f, 0.18f, depth - 0.04f), velours != null ? velours : Pal.Velours, 0f, false);
            int n = Mathf.Max(1, Mathf.RoundToInt((length - 0.4f) / 0.72f));
            float step = (length - 0.4f) / n;
            for (int i = 0; i < n; i++)
                BuildKit.BoxE("Coussin_Dos", g, new Vector3(-length * 0.5f + 0.2f + (i + 0.5f) * step, 0.78f, depth * 0.5f - 0.17f),
                              new Vector3(step - 0.05f, 0.5f, 0.2f), Pal.Coussin(i), new Vector3(12f, 0f, 0f), false);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Cyl("Traversin", g, new Vector3(s * (length * 0.5f - 0.13f), 0.66f, 0f), 0.13f, depth - 0.1f, Pal.Coussin(2), new Vector3(90f, 0f, 0f));
        }

        /// <summary>Grand plateau rond en cuivre ciselé sur piètement de bois.</summary>
        public static void PlateauCuivre(Transform parent, Vector3 pos, float radius, float height = 0.45f)
        {
            var g = BuildKit.Group("Plateau_Cuivre", parent, pos);
            HexPrism(g, "Pietement", new Vector3(0f, (height - 0.04f) * 0.5f, 0f), radius * 0.45f, height - 0.04f, Pal.BoisSombre, false);
            BuildKit.Cyl("Plateau", g, new Vector3(0f, height, 0f), radius, 0.03f, Pal.Cuivre);
            BuildKit.Cyl("Gravure", g, new Vector3(0f, height + 0.016f, 0f), radius * 0.72f, 0.004f, Pal.CuivreGrave);
            BuildKit.Cyl("Rosace", g, new Vector3(0f, height + 0.019f, 0f), radius * 0.3f, 0.004f, Pal.Cuivre);
            const int n = 30;
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2f / n;
                BuildKit.Sph("Feston", g, new Vector3(Mathf.Cos(a) * radius, height + 0.03f, Mathf.Sin(a) * radius), new Vector3(0.07f, 0.04f, 0.07f), Pal.Cuivre);
            }
            BuildKit.Box("Livre", g, new Vector3(0.05f, height + 0.05f, 0.05f), new Vector3(0.2f, 0.06f, 0.26f), Pal.BoisClair, 20f, false);
            BuildKit.Cyl("Boite_A", g, new Vector3(-radius * 0.55f, height + 0.06f, 0.1f), 0.05f, 0.09f, Pal.Laiton);
            BuildKit.Cyl("Boite_B", g, new Vector3(radius * 0.5f, height + 0.05f, -0.2f), 0.05f, 0.07f, Pal.Laiton);
            BuildKit.InvisibleWall("Collider", g, new Vector3(0f, height * 0.5f, 0f), new Vector3(radius * 1.5f, height, radius * 1.5f));
        }

        /// <summary>Petite table hexagonale incrustée de nacre (guéridon), avec un bougeoir.</summary>
        public static void TableHexagonale(Transform parent, Vector3 pos, float apothem = 0.22f, float height = 0.55f, bool bougeoir = true)
        {
            var g = BuildKit.Group("Table_Hexagonale", parent, pos);
            HexPrism(g, "Corps", new Vector3(0f, height * 0.5f, 0f), apothem, height, Pal.BoisSombre, true);
            HexPrism(g, "Bandeau_Nacre", new Vector3(0f, height * 0.55f, 0f), apothem + 0.005f, 0.05f, Pal.Nacre, false);
            HexPrism(g, "Plateau_Nacre", new Vector3(0f, height + 0.005f, 0f), apothem * 0.9f, 0.01f, Pal.Nacre, false);
            HexPrism(g, "Etoile", new Vector3(0f, height + 0.012f, 0f), apothem * 0.45f, 0.01f, Pal.BoisDore, false);
            if (!bougeoir) return;
            BuildKit.Cyl("Bougeoir_Pied", g, new Vector3(0f, height + 0.02f, 0f), 0.06f, 0.02f, Pal.Laiton);
            BuildKit.Cyl("Bougeoir_Tige", g, new Vector3(0f, height + 0.17f, 0f), 0.015f, 0.3f, Pal.Laiton);
            BuildKit.Cyl("Bougie", g, new Vector3(0f, height + 0.38f, 0f), 0.02f, 0.12f, Pal.PeintureCreme);
        }

        static void HexPrism(Transform g, string name, Vector3 center, float apothem, float height, Material m, bool collider)
        {
            float side = 2f * apothem / Mathf.Sqrt(3f);
            for (int k = 0; k < 3; k++)
                BuildKit.Box(name, g, center, new Vector3(side, height, 2f * apothem), m, k * 60f, collider && k == 0);
        }

        public static void Tapis(Transform parent, Vector3 pos, float yaw, float w, float l)
        {
            BuildKit.Box("Tapis", parent, pos + new Vector3(0f, 0.006f, 0f), new Vector3(w, 0.012f, l), Pal.Tapis, yaw, false);
        }

        /// <summary>Lustre en disque de verre suspendu à des chaînes. attach = point d'accroche au plafond.</summary>
        public static void LustreDisque(Transform parent, Vector3 attach, float drop, float radius, float lightIntensity, float lightRange,
                                        bool shadows = false, Material disque = null)
        {
            var g = BuildKit.Group("Lustre", parent, attach);
            float hook = drop * 0.45f;
            BuildKit.Cyl("Chaine", g, new Vector3(0f, -hook * 0.5f, 0f), 0.012f, hook, Pal.Laiton);
            BuildKit.Sph("Anneau", g, new Vector3(0f, -hook, 0f), Vector3.one * 0.08f, Pal.Laiton);
            for (int i = 0; i < 4; i++)
            {
                float a = i * Mathf.PI * 0.5f + Mathf.PI * 0.25f;
                var top = new Vector3(0f, -hook, 0f);
                var bot = new Vector3(Mathf.Cos(a) * radius * 0.92f, -drop, Mathf.Sin(a) * radius * 0.92f);
                var d = bot - top;
                BuildKit.Prim(PrimitiveType.Cube, "Chainette", g, (top + bot) * 0.5f, new Vector3(0.015f, d.magnitude, 0.015f),
                              Pal.Laiton, Quaternion.FromToRotation(Vector3.up, d).eulerAngles, false);
            }
            BuildKit.Cyl("Disque", g, new Vector3(0f, -drop, 0f), radius, 0.06f, disque != null ? disque : Pal.LustreVerre);
            BuildKit.Cyl("Cerclage", g, new Vector3(0f, -drop + 0.03f, 0f), radius + 0.04f, 0.03f, Pal.Laiton);
            BuildKit.PointLight("Lumiere_Lustre", g, new Vector3(0f, -drop - 0.35f, 0f), new Color(1f, 0.85f, 0.65f), lightIntensity, lightRange, shadows);
        }

        // ================================================================ Salon de jeux
        public static void TableDeJeu(Transform parent, Vector3 pos, float yaw, int variante = 0)
        {
            var g = BuildKit.Group("Table_de_Jeu", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Plateau", g, new Vector3(0f, 0.74f, 0f), new Vector3(0.86f, 0.04f, 0.86f), Pal.BoisClair, 0f, false);
            BuildKit.Box("Tapis_Vert", g, new Vector3(0f, 0.762f, 0f), new Vector3(0.76f, 0.006f, 0.76f), Pal.FeutreVert, 0f, false);
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sz = -1; sz <= 1; sz += 2)
                    BuildKit.Box("Pied", g, new Vector3(sx * 0.39f, 0.36f, sz * 0.39f), new Vector3(0.045f, 0.72f, 0.045f), Pal.BoisClair, 0f, false);
            if (variante % 2 == 1)
                BuildKit.Box("Frange", g, new Vector3(0f, 0.68f, 0f), new Vector3(0.9f, 0.1f, 0.9f), Pal.Canne, 0f, false);
            BuildKit.Box("Jeu_de_Cartes", g, new Vector3(-0.2f, 0.78f, 0.2f), new Vector3(0.09f, 0.03f, 0.13f), Pal.Coussin(2), 15f, false);
            BuildKit.Box("Livret", g, new Vector3(0.15f, 0.775f, -0.18f), new Vector3(0.16f, 0.02f, 0.22f), Pal.BoisSculpte, -10f, false);
            BuildKit.Cyl("Jeton_A", g, new Vector3(0.05f, 0.768f, 0.05f), 0.035f, 0.005f, Pal.Cordon);
            BuildKit.Cyl("Jeton_B", g, new Vector3(-0.1f, 0.768f, -0.05f), 0.035f, 0.005f, Pal.Cordon);
            BuildKit.InvisibleWall("Collider", g, new Vector3(0f, 0.38f, 0f), new Vector3(0.86f, 0.76f, 0.86f));
        }

        /// <summary>Chaise cannée à dossier en lyre (assise tournée vers -Z local).</summary>
        public static void ChaiseLyre(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Chaise_Lyre", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Assise", g, new Vector3(0f, 0.46f, 0f), new Vector3(0.44f, 0.04f, 0.42f), Pal.Canne, 0f, false);
            BuildKit.Box("Ceinture", g, new Vector3(0f, 0.42f, 0f), new Vector3(0.46f, 0.06f, 0.44f), Pal.PeintureCreme, 0f, false);
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sz = -1; sz <= 1; sz += 2)
                    BuildKit.Cyl("Pied", g, new Vector3(sx * 0.2f, 0.2f, sz * 0.19f), 0.022f, 0.4f, Pal.PeintureCreme);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Montant", g, new Vector3(s * 0.2f, 0.76f, 0.2f), new Vector3(0.04f, 0.6f, 0.04f), Pal.PeintureCreme, 0f, false);
                BuildKit.BoxE("Lyre", g, new Vector3(s * 0.065f, 0.76f, 0.2f), new Vector3(0.03f, 0.44f, 0.02f), Pal.PeintureCreme, new Vector3(0f, 0f, s * 12f), false);
            }
            BuildKit.Box("Traverse_Haut", g, new Vector3(0f, 1.05f, 0.2f), new Vector3(0.46f, 0.06f, 0.04f), Pal.PeintureCreme, 0f, false);
            BuildKit.Box("Traverse_Lyre", g, new Vector3(0f, 0.62f, 0.2f), new Vector3(0.22f, 0.03f, 0.03f), Pal.PeintureCreme, 0f, false);
        }

        public static void HorlogeComtoise(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Horloge_Comtoise", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Socle", g, new Vector3(0f, 0.25f, 0f), new Vector3(0.55f, 0.5f, 0.36f), Pal.BoisSculpte);
            BuildKit.Box("Gaine", g, new Vector3(0f, 1.05f, 0f), new Vector3(0.42f, 1.1f, 0.3f), Pal.BoisSculpte, 0f, false);
            BuildKit.Box("Tete", g, new Vector3(0f, 1.85f, 0f), new Vector3(0.52f, 0.5f, 0.36f), Pal.BoisSculpte, 0f, false);
            BuildKit.Box("Fronton", g, new Vector3(0f, 2.14f, 0f), new Vector3(0.58f, 0.1f, 0.4f), Pal.BoisSombre, 0f, false);
            BuildKit.Cyl("Cadran", g, new Vector3(0f, 1.85f, -0.185f), 0.17f, 0.01f, Pal.Cadran, new Vector3(90f, 0f, 0f));
            BuildKit.BoxE("Aiguille_H", g, new Vector3(0.03f, 1.88f, -0.195f), new Vector3(0.012f, 0.09f, 0.005f), Pal.Noir, new Vector3(0f, 0f, -40f), false);
            BuildKit.BoxE("Aiguille_M", g, new Vector3(-0.02f, 1.9f, -0.197f), new Vector3(0.008f, 0.13f, 0.005f), Pal.Noir, new Vector3(0f, 0f, 20f), false);
            BuildKit.Box("Vitre_Balancier", g, new Vector3(0f, 1.0f, -0.155f), new Vector3(0.24f, 0.7f, 0.01f), Pal.VitreSombre, 0f, false);
            BuildKit.Cyl("Balancier", g, new Vector3(0f, 0.8f, -0.16f), 0.08f, 0.01f, Pal.Laiton, new Vector3(90f, 0f, 0f));
            BuildKit.Box("Tige_Balancier", g, new Vector3(0f, 1.1f, -0.16f), new Vector3(0.015f, 0.55f, 0.005f), Pal.Laiton, 0f, false);
        }

        public static void PorteSculptee(Transform parent, Vector3 pos, float yaw, float w, float h)
        {
            var g = BuildKit.Group("Porte_Sculptee", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Encadrement", g, new Vector3(0f, (h + 0.25f) * 0.5f, -0.02f), new Vector3(w + 0.4f, h + 0.25f, 0.04f), Pal.Marbre, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                float cx = s * w * 0.25f;
                BuildKit.Box("Vantail", g, new Vector3(cx, h * 0.5f, -0.07f), new Vector3(w * 0.5f - 0.02f, h, 0.07f), Pal.BoisSculpte, 0f, false);
                for (int k = 0; k < 3; k++)
                {
                    float py = h * (0.2f + 0.3f * k);
                    BuildKit.Box("Panneau", g, new Vector3(cx, py, -0.115f), new Vector3(w * 0.32f, h * 0.22f, 0.03f), Pal.BoisSombre, 0f, false);
                    BuildKit.Box("Losange", g, new Vector3(cx, py, -0.135f), new Vector3(w * 0.1f, w * 0.1f, 0.02f), Pal.BoisDore, 45f, false);
                }
                BuildKit.Studs(g, new Vector3(cx, h * 0.5f, 0f), w * 0.42f, h * 0.92f, 4, 9, -0.11f, Pal.Laiton, 0.035f);
                BuildKit.Sph("Heurtoir", g, new Vector3(cx - s * w * 0.18f, h * 0.5f, -0.13f), Vector3.one * 0.07f, Pal.Laiton);
            }
        }

        // ================================================================ Le téléphone de campagne (premier indice)
        /// <summary>Coffre peint, blanc vieilli, à panneaux ajourés. Renvoie la hauteur du dessus.</summary>
        public static float CoffrePeint(Transform parent, Vector3 pos, float yaw, float width = 1.15f)
        {
            var g = BuildKit.Group("Coffre_Peint", parent, pos, new Vector3(0f, yaw, 0f));
            const float d = 0.58f;
            BuildKit.Box("Caisse", g, new Vector3(0f, 0.45f, 0f), new Vector3(width, 0.7f, d), Pal.PeintureVieillie);
            BuildKit.Box("Dessus", g, new Vector3(0f, 0.825f, 0f), new Vector3(width + 0.06f, 0.05f, d + 0.06f), Pal.PeintureVieillie, 0f, false);
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sz = -1; sz <= 1; sz += 2)
                    BuildKit.Cyl("Pied", g, new Vector3(sx * (width * 0.5f - 0.07f), 0.05f, sz * (d * 0.5f - 0.07f)), 0.055f, 0.1f, Pal.BoisSombre);
            float zf = -d * 0.5f - 0.005f;
            BuildKit.Box("Panneau_G", g, new Vector3(-width * 0.36f, 0.45f, zf), new Vector3(width * 0.16f, 0.48f, 0.01f), Pal.PanneauAjoure, 0f, false);
            BuildKit.Box("Panneau_D", g, new Vector3(width * 0.36f, 0.45f, zf), new Vector3(width * 0.16f, 0.48f, 0.01f), Pal.PanneauAjoure, 0f, false);
            BuildKit.Prim(PrimitiveType.Cylinder, "Medaillon_Ovale", g, new Vector3(0f, 0.45f, zf), new Vector3(width * 0.4f, 0.005f, 0.42f), Pal.PanneauAjoure, new Vector3(90f, 0f, 0f), false);
            // Filets peints (rinceaux)
            BuildKit.Box("Filet_Haut", g, new Vector3(0f, 0.74f, zf - 0.003f), new Vector3(width * 0.94f, 0.015f, 0.005f), Pal.PeintureFleur, 0f, false);
            BuildKit.Box("Filet_Bas", g, new Vector3(0f, 0.16f, zf - 0.003f), new Vector3(width * 0.94f, 0.015f, 0.005f), Pal.PeintureFleur, 0f, false);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Filet_Vertical", g, new Vector3(s * width * 0.24f, 0.45f, zf - 0.003f), new Vector3(0.015f, 0.55f, 0.005f), Pal.PeintureFleur, 0f, false);
            return 0.85f;
        }

        /// <summary>Téléphone de campagne ancien : boîtier en bakélite brune, combiné noir, manivelle.</summary>
        public static Transform TelephoneCampagne(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Telephone_Campagne", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Boitier", g, new Vector3(0f, 0.09f, 0f), new Vector3(0.3f, 0.18f, 0.17f), Pal.Bakelite, 0f, false);
            BuildKit.Box("Platine", g, new Vector3(0f, 0.182f, 0f), new Vector3(0.26f, 0.006f, 0.14f), Pal.Noir, 0f, false);
            BuildKit.BoxE("Couvercle", g, new Vector3(0f, 0.24f, 0.1f), new Vector3(0.3f, 0.13f, 0.02f), Pal.Bakelite, new Vector3(-15f, 0f, 0f), false);
            BuildKit.Box("Grille_HP", g, new Vector3(0f, 0.08f, -0.087f), new Vector3(0.05f, 0.04f, 0.005f), Pal.Noir, 0f, false);
            BuildKit.Cyl("Combine", g, new Vector3(0f, 0.215f, 0f), 0.022f, 0.2f, Pal.Noir, new Vector3(0f, 0f, 90f));
            BuildKit.Sph("Ecouteur", g, new Vector3(-0.11f, 0.21f, 0f), new Vector3(0.07f, 0.05f, 0.07f), Pal.Noir);
            BuildKit.Sph("Micro", g, new Vector3(0.11f, 0.21f, 0f), new Vector3(0.07f, 0.05f, 0.07f), Pal.Noir);
            BuildKit.Box("Manivelle_Axe", g, new Vector3(0.165f, 0.07f, 0f), new Vector3(0.03f, 0.02f, 0.02f), Pal.Fer, 0f, false);
            BuildKit.Box("Manivelle_Bras", g, new Vector3(0.18f, 0.05f, 0f), new Vector3(0.01f, 0.05f, 0.015f), Pal.Fer, 0f, false);
            BuildKit.Cyl("Manivelle_Poignee", g, new Vector3(0.19f, 0.03f, 0f), 0.01f, 0.04f, Pal.Noir, new Vector3(0f, 0f, 90f));
            BuildKit.Cyl("Cordon", g, new Vector3(0.12f, 0.12f, -0.02f), 0.006f, 0.16f, Pal.Cordon, new Vector3(0f, 0f, 60f));
            return g;
        }

        /// <summary>Chandelier en fer forgé à godets de verre (deux étages de bras).</summary>
        public static void ChandelierFer(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Chandelier_Fer", parent, pos);
            BuildKit.Cyl("Pied", g, new Vector3(0f, 0.01f, 0f), 0.11f, 0.02f, Pal.Fer);
            BuildKit.Cyl("Tige", g, new Vector3(0f, 0.27f, 0f), 0.012f, 0.5f, Pal.Fer);
            for (int k = 0; k < 6; k++)
            {
                float a = k * 60f + (k % 2) * 30f;
                float r = k % 2 == 0 ? 0.22f : 0.14f;
                float y = k % 2 == 0 ? 0.5f : 0.36f;
                var dir = Quaternion.Euler(0f, a, 0f) * Vector3.forward;
                BuildKit.Box("Bras", g, new Vector3(0f, y, 0f) + dir * (r * 0.5f), new Vector3(0.012f, 0.012f, r), Pal.Fer, a, false);
                BuildKit.Cyl("Bague", g, new Vector3(0f, y - 0.02f, 0f) + dir * r, 0.04f, 0.01f, Pal.Fer);
                BuildKit.Cyl("Godet", g, new Vector3(0f, y - 0.06f, 0f) + dir * r, 0.034f, 0.08f, Pal.Verre);
            }
        }

        public static void LanterneMurale(Transform parent, Vector3 pos, float yaw, float lightIntensity = 1.2f, float lightRange = 6f)
        {
            var g = BuildKit.Group("Lanterne_Murale", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Potence", g, new Vector3(0f, 0f, -0.13f), new Vector3(0.03f, 0.03f, 0.26f), Pal.Fer, 0f, false);
            BuildKit.Box("Verre", g, new Vector3(0f, -0.18f, -0.26f), new Vector3(0.18f, 0.3f, 0.18f), Pal.LanterneVerre, 0f, false);
            BuildKit.Box("Chapeau", g, new Vector3(0f, 0f, -0.26f), new Vector3(0.24f, 0.05f, 0.24f), Pal.Fer, 0f, false);
            BuildKit.Box("Fond", g, new Vector3(0f, -0.35f, -0.26f), new Vector3(0.2f, 0.04f, 0.2f), Pal.Fer, 0f, false);
            BuildKit.Sph("Fleuron", g, new Vector3(0f, 0.06f, -0.26f), Vector3.one * 0.07f, Pal.Fer);
            if (lightIntensity > 0f)
                BuildKit.PointLight("Lumiere_Lanterne", g, new Vector3(0f, -0.18f, -0.45f), new Color(1f, 0.8f, 0.55f), lightIntensity, lightRange);
        }
    }
}
