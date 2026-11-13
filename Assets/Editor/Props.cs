using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>Palette de matériaux du palais et de Sidi Bou Saïd.</summary>
    public static partial class Pal
    {
        public static Material Chaux => BuildKit.Mat("Chaux_Blanche", new Color(0.96f, 0.95f, 0.91f), 0.08f);
        public static Material BleuSBS => BuildKit.Mat("Bleu_SidiBouSaid", new Color(0.05f, 0.36f, 0.68f), 0.35f);
        public static Material JaunePorte => BuildKit.Mat("Jaune_Porte", new Color(0.95f, 0.74f, 0.12f), 0.35f);
        public static Material JauneSombre => BuildKit.Mat("Jaune_Porte_Moulure", new Color(0.82f, 0.6f, 0.08f), 0.35f);
        public static Material Pierre => BuildKit.Mat("Pierre_Kadhal", new Color(0.82f, 0.76f, 0.64f), 0.15f);
        public static Material Marbre => BuildKit.Mat("Marbre_Blanc", new Color(0.93f, 0.93f, 0.95f), 0.75f);
        public static Material Pave => BuildKit.Mat("Pave_Allee", new Color(0.72f, 0.67f, 0.58f), 0.1f);
        public static Material Dallage => BuildKit.Mat("Dallage_Esplanade", new Color(0.85f, 0.8f, 0.72f), 0.2f);
        public static Material Gravier => BuildKit.Mat("Gravier", new Color(0.78f, 0.72f, 0.6f), 0.05f);
        public static Material Herbe => BuildKit.Mat("Herbe", new Color(0.32f, 0.5f, 0.22f), 0.05f);
        public static Material Haie => BuildKit.Mat("Haie", new Color(0.16f, 0.36f, 0.14f), 0.05f);
        public static Material Feuillage => BuildKit.Mat("Feuillage", new Color(0.2f, 0.42f, 0.16f), 0.1f);
        public static Material FeuillageOlivier => BuildKit.Mat("Feuillage_Olivier", new Color(0.45f, 0.52f, 0.38f), 0.1f);
        public static Material FeuillePalmier => BuildKit.Mat("Feuille_Palmier", new Color(0.28f, 0.48f, 0.18f), 0.15f);
        public static Material Cypres => BuildKit.Mat("Cypres", new Color(0.1f, 0.26f, 0.12f), 0.05f);
        public static Material Tronc => BuildKit.Mat("Tronc", new Color(0.36f, 0.26f, 0.17f), 0.05f);
        public static Material TroncPalmier => BuildKit.Mat("Tronc_Palmier", new Color(0.5f, 0.4f, 0.28f), 0.05f);
        public static Material Orange => BuildKit.Mat("Orange", new Color(1f, 0.55f, 0.05f), 0.4f);
        public static Material Bougainvillier => BuildKit.Mat("Bougainvillier", new Color(0.86f, 0.15f, 0.52f), 0.1f);
        public static Material Zellige => BuildKit.Mat("Zellige_Vert", new Color(0.08f, 0.45f, 0.38f), 0.8f);
        public static Material TuileVerte => BuildKit.Mat("Tuile_Verte", new Color(0.12f, 0.42f, 0.25f), 0.6f);
        public static Material TerreCuite => BuildKit.Mat("Terre_Cuite", new Color(0.72f, 0.36f, 0.2f), 0.1f);
        public static Material Terre => BuildKit.Mat("Terre", new Color(0.33f, 0.24f, 0.16f), 0.02f);
        public static Material Eau => BuildKit.Mat("Eau", new Color(0.18f, 0.48f, 0.72f), 0.95f);
        public static Material Mer => BuildKit.Mat("Mer", new Color(0.04f, 0.3f, 0.55f), 0.9f);
        public static Material Colline => BuildKit.Mat("Colline", new Color(0.6f, 0.56f, 0.4f), 0.05f);
        public static Material Montagne => BuildKit.Mat("Montagne_Lointaine", new Color(0.5f, 0.55f, 0.62f), 0.05f);
        public static Material BoisSombre => BuildKit.Mat("Bois_Sombre", new Color(0.28f, 0.16f, 0.08f), 0.3f);
        public static Material Fer => BuildKit.Mat("Fer_Forge", new Color(0.08f, 0.08f, 0.09f), 0.4f, 0.6f);
        public static Material Laiton => BuildKit.Mat("Laiton", new Color(0.78f, 0.6f, 0.25f), 0.7f, 1f);
        public static Material VitreSombre => BuildKit.Mat("Vitre_Sombre", new Color(0.08f, 0.12f, 0.16f), 0.9f);
        public static Material LanterneVerre => BuildKit.Emissive("Lanterne_Verre", new Color(1f, 0.86f, 0.6f), new Color(1f, 0.7f, 0.35f) * 1.4f);

        public static Material Fleur(int i)
        {
            switch (i % 5)
            {
                case 0: return BuildKit.Mat("Fleur_Rouge", new Color(0.85f, 0.1f, 0.12f), 0.2f);
                case 1: return BuildKit.Mat("Fleur_Jaune", new Color(0.98f, 0.82f, 0.2f), 0.2f);
                case 2: return BuildKit.Mat("Fleur_Blanche", new Color(0.97f, 0.97f, 0.95f), 0.2f);
                case 3: return BuildKit.Mat("Fleur_Violette", new Color(0.55f, 0.25f, 0.75f), 0.2f);
                default: return Bougainvillier;
            }
        }
    }

    /// <summary>Accessoires réutilisables : végétation, maisons, mobilier, fenêtres, fontaine.</summary>
    public static class Props
    {
        // ================================================================ Végétation
        public static void Palm(Transform parent, Vector3 pos, float height = -1f)
        {
            if (height < 0f) height = BuildKit.R(6f, 10f);
            var g = BuildKit.Group("Palmier", parent, pos, new Vector3(0f, BuildKit.R(0f, 360f), 0f));
            var trunk = BuildKit.Group("Tronc", g, Vector3.zero, new Vector3(BuildKit.R(-6f, 6f), 0f, BuildKit.R(-6f, 6f)));
            BuildKit.Cyl("Stipe", trunk, new Vector3(0f, height * 0.5f, 0f), 0.22f, height, Pal.TroncPalmier, Vector3.zero, true);
            for (float y = 0.6f; y < height - 0.3f; y += 0.7f)
                BuildKit.Cyl("Anneau", trunk, new Vector3(0f, y, 0f), 0.25f, 0.08f, Pal.Tronc);

            var crown = BuildKit.Group("Couronne", trunk, new Vector3(0f, height, 0f));
            BuildKit.Sph("Coeur", crown, Vector3.zero, Vector3.one * 0.6f, Pal.TroncPalmier);
            int n = BuildKit.RI(9, 13);
            for (int i = 0; i < n; i++)
            {
                var f = BuildKit.Group("Palme", crown, Vector3.zero, new Vector3(BuildKit.R(10f, 40f), i * 360f / n + BuildKit.R(-10f, 10f), 0f));
                BuildKit.Box("Feuille", f, new Vector3(0f, 0f, 1.4f), new Vector3(0.55f, 0.04f, 2.8f), Pal.FeuillePalmier, 0f, false);
                var tip = BuildKit.Group("Pointe", f, new Vector3(0f, 0f, 2.7f), new Vector3(30f, 0f, 0f));
                BuildKit.Box("Feuille_Bout", tip, new Vector3(0f, 0f, 0.6f), new Vector3(0.4f, 0.04f, 1.2f), Pal.FeuillePalmier, 0f, false);
            }
        }

        public static void Cypress(Transform parent, Vector3 pos, float height = -1f)
        {
            if (height < 0f) height = BuildKit.R(6f, 9f);
            var g = BuildKit.Group("Cypres", parent, pos);
            BuildKit.Cap("Flamme", g, new Vector3(0f, height * 0.5f + 0.3f, 0f), BuildKit.R(0.45f, 0.6f), height, Pal.Cypres);
        }

        public static void OrangeTree(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Oranger", parent, pos, new Vector3(0f, BuildKit.R(0f, 360f), 0f));
            BuildKit.Cyl("Tronc", g, new Vector3(0f, 0.75f, 0f), 0.12f, 1.5f, Pal.Tronc, Vector3.zero, true);
            Vector3 c = new Vector3(0f, 2.3f, 0f);
            BuildKit.Sph("Couronne", g, c, new Vector3(2.4f, 2.0f, 2.4f), Pal.Feuillage);
            for (int i = 0; i < 9; i++)
            {
                Vector3 dir = new Vector3(BuildKit.R(-1f, 1f), BuildKit.R(-0.3f, 0.8f), BuildKit.R(-1f, 1f)).normalized;
                BuildKit.Sph("Orange", g, c + Vector3.Scale(dir, new Vector3(1.18f, 0.98f, 1.18f)), Vector3.one * 0.15f, Pal.Orange);
            }
        }

        public static void OliveTree(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Olivier", parent, pos, new Vector3(0f, BuildKit.R(0f, 360f), 0f));
            BuildKit.Cyl("Tronc", g, new Vector3(0f, 0.8f, 0f), 0.2f, 1.6f, Pal.Tronc, new Vector3(BuildKit.R(-8f, 8f), 0f, BuildKit.R(-8f, 8f)), true);
            for (int i = 0; i < 3; i++)
                BuildKit.Sph("Feuillage", g, new Vector3(BuildKit.R(-0.8f, 0.8f), BuildKit.R(2.0f, 2.6f), BuildKit.R(-0.8f, 0.8f)),
                             new Vector3(BuildKit.R(1.8f, 2.4f), BuildKit.R(1.2f, 1.6f), BuildKit.R(1.8f, 2.4f)), Pal.FeuillageOlivier);
        }

        public static void Hedge(Transform parent, Vector3 a, Vector3 b, float height = 1f, float width = 0.7f)
            => BuildKit.Wall("Haie", parent, a, b, height, width, Pal.Haie);

        public static void FlowerBed(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Massif", parent, pos, new Vector3(0f, BuildKit.R(-10f, 10f), 0f));
            BuildKit.Box("Bordure", g, new Vector3(0f, 0.1f, 0f), new Vector3(1.6f, 0.2f, 0.8f), Pal.Pierre, 0f, false);
            BuildKit.Box("Terre", g, new Vector3(0f, 0.21f, 0f), new Vector3(1.45f, 0.04f, 0.65f), Pal.Terre, 0f, false);
            int color = BuildKit.RI(0, 5);
            for (int i = 0; i < 7; i++)
            {
                BuildKit.Sph("Feuilles", g, new Vector3(BuildKit.R(-0.6f, 0.6f), 0.3f, BuildKit.R(-0.25f, 0.25f)), Vector3.one * 0.28f, Pal.Feuillage);
                BuildKit.Sph("Fleur", g, new Vector3(BuildKit.R(-0.6f, 0.6f), 0.42f, BuildKit.R(-0.25f, 0.25f)), Vector3.one * 0.14f, Pal.Fleur(color));
            }
        }

        public static void Bougainvillea(Transform parent, Vector3 pos, float size = 1.5f)
        {
            var g = BuildKit.Group("Bougainvillier", parent, pos);
            int n = BuildKit.RI(4, 7);
            for (int i = 0; i < n; i++)
            {
                var m = BuildKit.Chance(0.75f) ? Pal.Bougainvillier : Pal.Feuillage;
                float s = BuildKit.R(0.5f, 1f) * size;
                BuildKit.Sph("Grappe", g, new Vector3(BuildKit.R(-size, size), BuildKit.R(-size * 0.8f, size * 0.3f), BuildKit.R(-0.2f, 0.2f)),
                             new Vector3(s, s * 0.8f, s * 0.6f), m);
            }
        }

        public static void Pot(Transform parent, Vector3 pos, float s = 1f, bool flowers = true)
        {
            var g = BuildKit.Group("Pot", parent, pos);
            BuildKit.Cyl("Jarre", g, new Vector3(0f, 0.275f * s, 0f), 0.32f * s, 0.55f * s, Pal.TerreCuite, Vector3.zero, s >= 1f);
            BuildKit.Cyl("Bord", g, new Vector3(0f, 0.56f * s, 0f), 0.36f * s, 0.06f * s, Pal.TerreCuite);
            BuildKit.Sph("Plante", g, new Vector3(0f, 0.85f * s, 0f), Vector3.one * 0.8f * s, Pal.Feuillage);
            if (!flowers) return;
            int c = BuildKit.RI(0, 5);
            for (int i = 0; i < 6; i++)
            {
                Vector3 dir = new Vector3(BuildKit.R(-1f, 1f), BuildKit.R(0f, 1f), BuildKit.R(-1f, 1f)).normalized;
                BuildKit.Sph("Fleur", g, new Vector3(0f, 0.85f * s, 0f) + dir * 0.38f * s, Vector3.one * 0.13f * s, Pal.Fleur(c));
            }
        }

        // ================================================================ Maisons de Sidi Bou Saïd
        public static void SbsHouse(Transform parent, Vector3 pos, float yaw)
        {
            float w = BuildKit.R(4f, 8f), d = BuildKit.R(4f, 8f), h = BuildKit.R(3.5f, 7.5f);
            var g = BuildKit.Group("Maison", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Murs", g, new Vector3(0f, h * 0.5f, 0f), new Vector3(w, h, d), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere", g, new Vector3(0f, h + 0.08f, 0f), new Vector3(w + 0.12f, 0.16f, d + 0.12f), Pal.Chaux, 0f, false);

            float dx = BuildKit.R(-w * 0.25f, w * 0.25f);
            float front = -d * 0.5f - 0.04f;
            BuildKit.Box("Porte", g, new Vector3(dx, 1.0f, front), new Vector3(1f, 2f, 0.08f), Pal.BleuSBS, 0f, false);
            BuildKit.Cyl("Porte_Arc", g, new Vector3(dx, 2.0f, front), 0.5f, 0.08f, Pal.BleuSBS, new Vector3(90f, 0f, 0f));

            int windows = h > 5.5f ? 3 : 2;
            for (int k = 0; k < windows; k++)
            {
                float wx = BuildKit.R(-w * 0.5f + 0.7f, w * 0.5f - 0.7f);
                float wy = h > 5.5f && BuildKit.Chance(0.6f) ? BuildKit.R(3.4f, h - 1.2f) : 1.6f;
                if (wy < 2.6f && Mathf.Abs(wx - dx) < 1.2f) continue;
                BuildKit.Box("Fenetre", g, new Vector3(wx, wy, front - 0.01f), new Vector3(0.8f, 1.0f, 0.1f), Pal.BleuSBS, 0f, false);
            }
            if (BuildKit.Chance(0.3f))
                BuildKit.Sph("Coupole", g, new Vector3(BuildKit.R(-w * 0.2f, w * 0.2f), h, BuildKit.R(-d * 0.2f, d * 0.2f)),
                             new Vector3(w * 0.45f, w * 0.35f, w * 0.45f), Pal.Chaux);
            if (BuildKit.Chance(0.35f))
                Bougainvillea(g, new Vector3(BuildKit.R(-w * 0.4f, w * 0.4f), h - 0.4f, front - 0.2f), 1.4f);
        }

        public static void Minaret(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Minaret", parent, pos);
            BuildKit.Box("Tour", g, new Vector3(0f, 7f, 0f), new Vector3(3f, 14f, 3f), Pal.Chaux, 0f, false);
            BuildKit.Box("Bandeau", g, new Vector3(0f, 12.5f, 0f), new Vector3(3.05f, 0.6f, 3.05f), Pal.TuileVerte, 0f, false);
            BuildKit.Box("Balcon", g, new Vector3(0f, 14.2f, 0f), new Vector3(3.8f, 0.4f, 3.8f), Pal.Chaux, 0f, false);
            BuildKit.Box("Lanternon", g, new Vector3(0f, 15.9f, 0f), new Vector3(2f, 3f, 2f), Pal.Chaux, 0f, false);
            BuildKit.Sph("Dome", g, new Vector3(0f, 17.4f, 0f), new Vector3(2f, 1.6f, 2f), Pal.TuileVerte);
            BuildKit.Cyl("Jamour", g, new Vector3(0f, 18.6f, 0f), 0.05f, 1.2f, Pal.Laiton);
        }

        public static void Lighthouse(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Phare", parent, pos);
            BuildKit.Cyl("Fut", g, new Vector3(0f, 9f, 0f), 2f, 18f, Pal.Chaux);
            BuildKit.Cyl("Galerie", g, new Vector3(0f, 18.2f, 0f), 2.6f, 0.4f, Pal.Fer);
            BuildKit.Cyl("Lanterne", g, new Vector3(0f, 19.4f, 0f), 1.3f, 2f, Pal.LanterneVerre);
            BuildKit.Sph("Toit", g, new Vector3(0f, 20.5f, 0f), new Vector3(2.8f, 1.6f, 2.8f), Pal.Fer);
        }

        // ================================================================ Mobilier extérieur
        public static void Lantern(Transform parent, Vector3 pos, float postHeight = 2.4f)
        {
            var g = BuildKit.Group("Lanterne", parent, pos);
            BuildKit.Cyl("Poteau", g, new Vector3(0f, postHeight * 0.5f, 0f), 0.05f, postHeight, Pal.Fer);
            BuildKit.Box("Base", g, new Vector3(0f, postHeight + 0.03f, 0f), new Vector3(0.36f, 0.06f, 0.36f), Pal.Fer, 0f, false);
            BuildKit.Box("Verre", g, new Vector3(0f, postHeight + 0.26f, 0f), new Vector3(0.28f, 0.4f, 0.28f), Pal.LanterneVerre, 0f, false);
            BuildKit.Box("Chapeau", g, new Vector3(0f, postHeight + 0.5f, 0f), new Vector3(0.42f, 0.08f, 0.42f), Pal.Fer, 0f, false);
            BuildKit.Sph("Fleuron", g, new Vector3(0f, postHeight + 0.6f, 0f), Vector3.one * 0.12f, Pal.Fer);
        }

        /// <summary>Banc maçonné recouvert de zellige. Le visiteur assis regarde vers -Z local.</summary>
        public static void Bench(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Banc", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Socle", g, new Vector3(0f, 0.21f, 0f), new Vector3(2.2f, 0.42f, 0.6f), Pal.Chaux);
            BuildKit.Box("Assise", g, new Vector3(0f, 0.45f, 0f), new Vector3(2.3f, 0.06f, 0.66f), Pal.Zellige, 0f, false);
            BuildKit.Box("Dossier", g, new Vector3(0f, 0.75f, 0.3f), new Vector3(2.3f, 0.55f, 0.14f), Pal.Zellige);
            BuildKit.Box("Chaperon", g, new Vector3(0f, 1.05f, 0.3f), new Vector3(2.36f, 0.06f, 0.2f), Pal.Marbre, 0f, false);
        }

        public static void ZelligeStar(Transform parent, Vector3 pos, float radius)
        {
            var g = BuildKit.Group("Motif_Zellige", parent, pos);
            BuildKit.Cyl("Cercle", g, new Vector3(0f, 0.01f, 0f), radius, 0.02f, Pal.Zellige);
            BuildKit.Cyl("Cercle_Blanc", g, new Vector3(0f, 0.015f, 0f), radius * 0.85f, 0.02f, Pal.Marbre);
            float side = radius * 1.05f;
            BuildKit.Box("Etoile_A", g, new Vector3(0f, 0.022f, 0f), new Vector3(side, 0.02f, side), Pal.BleuSBS, 0f, false);
            BuildKit.Box("Etoile_B", g, new Vector3(0f, 0.022f, 0f), new Vector3(side, 0.02f, side), Pal.BleuSBS, 45f, false);
            BuildKit.Cyl("Coeur", g, new Vector3(0f, 0.03f, 0f), radius * 0.3f, 0.02f, Pal.JaunePorte);
        }

        // ================================================================ Fenêtres
        /// <summary>Fenêtre andalouse sur un mur (pos = bas-centre sur la surface, face vers -Z local).</summary>
        public static void Window(Transform parent, Vector3 pos, float yaw, float w, float h, bool grille)
        {
            var g = BuildKit.Group("Fenetre", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Encadrement", g, new Vector3(0f, h * 0.5f, -0.05f), new Vector3(w + 0.3f, h + 0.3f, 0.1f), Pal.Pierre, 0f, false);
            BuildKit.Cyl("Arc", g, new Vector3(0f, h, -0.05f), w * 0.5f + 0.15f, 0.1f, Pal.Pierre, new Vector3(90f, 0f, 0f));
            BuildKit.Box("Vitre", g, new Vector3(0f, h * 0.5f, -0.11f), new Vector3(w, h, 0.02f), Pal.VitreSombre, 0f, false);
            BuildKit.Cyl("Vitre_Arc", g, new Vector3(0f, h, -0.11f), w * 0.5f, 0.02f, Pal.VitreSombre, new Vector3(90f, 0f, 0f));
            float sx = w * 0.5f + 0.15f + w * 0.25f;
            BuildKit.Box("Volet_G", g, new Vector3(-sx, h * 0.5f, -0.13f), new Vector3(w * 0.5f, h, 0.05f), Pal.BleuSBS, 0f, false);
            BuildKit.Box("Volet_D", g, new Vector3(sx, h * 0.5f, -0.13f), new Vector3(w * 0.5f, h, 0.05f), Pal.BleuSBS, 0f, false);
            BuildKit.Box("Appui", g, new Vector3(0f, -0.08f, -0.15f), new Vector3(w + 0.45f, 0.12f, 0.3f), Pal.Marbre, 0f, false);
            if (!grille) return;
            int bars = Mathf.Max(3, Mathf.RoundToInt(w / 0.16f));
            for (int i = 1; i < bars; i++)
                BuildKit.Cyl("Barreau", g, new Vector3(-w * 0.5f + i * w / bars, h * 0.5f, -0.22f), 0.015f, h, Pal.BleuSBS);
            for (int j = 1; j <= 3; j++)
                BuildKit.Box("Traverse", g, new Vector3(0f, j * h / 4f, -0.22f), new Vector3(w, 0.03f, 0.03f), Pal.BleuSBS, 0f, false);
        }

        /// <summary>Moucharabieh en bois peint en bleu (pos = bas-centre sur la façade, face vers -Z local).</summary>
        public static void Mashrabiya(Transform parent, Vector3 pos, float yaw, float w = 1.8f, float h = 2.0f, float d = 0.6f)
        {
            var g = BuildKit.Group("Moucharabieh", parent, pos, new Vector3(0f, yaw, 0f));
            var wood = Pal.BleuSBS;
            BuildKit.Box("Socle", g, new Vector3(0f, -0.1f, -d * 0.5f), new Vector3(w + 0.2f, 0.2f, d + 0.1f), wood, 0f, false);
            BuildKit.Box("Console", g, new Vector3(0f, -0.4f, -0.15f), new Vector3(w * 0.6f, 0.4f, 0.3f), wood, 0f, false);
            BuildKit.Box("Fond", g, new Vector3(0f, h * 0.5f, -0.02f), new Vector3(w, h, 0.04f), Pal.VitreSombre, 0f, false);
            BuildKit.Box("Toit", g, new Vector3(0f, h + 0.07f, -d * 0.5f), new Vector3(w + 0.3f, 0.14f, d + 0.2f), wood, 0f, false);
            BuildKit.Box("Tuiles", g, new Vector3(0f, h + 0.2f, -d * 0.5f), new Vector3(w + 0.15f, 0.12f, d + 0.05f), Pal.TuileVerte, 0f, false);
            int nv = Mathf.Max(3, Mathf.RoundToInt(w / 0.16f));
            for (int i = 0; i <= nv; i++)
                BuildKit.Box("Lv", g, new Vector3(-w * 0.5f + i * w / nv, h * 0.5f, -d), new Vector3(0.04f, h, 0.04f), wood, 0f, false);
            int nh = Mathf.RoundToInt(h / 0.18f);
            for (int j = 0; j <= nh; j++)
            {
                float y = j * h / nh;
                BuildKit.Box("Lh", g, new Vector3(0f, y, -d), new Vector3(w, 0.035f, 0.035f), wood, 0f, false);
                BuildKit.Box("Lh_G", g, new Vector3(-w * 0.5f, y, -d * 0.5f), new Vector3(0.035f, 0.035f, d), wood, 0f, false);
                BuildKit.Box("Lh_D", g, new Vector3(w * 0.5f, y, -d * 0.5f), new Vector3(0.035f, 0.035f, d), wood, 0f, false);
            }
        }

        // ================================================================ Fontaine
        public static void Fountain(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Fontaine", parent, pos);
            const float apothem = 2.2f;
            float side = 2f * apothem * Mathf.Tan(22.5f * Mathf.Deg2Rad);
            for (int i = 0; i < 8; i++)
            {
                float a = i * 45f;
                float ar = a * Mathf.Deg2Rad;
                Vector3 p = new Vector3(Mathf.Cos(ar) * apothem, 0.35f, Mathf.Sin(ar) * apothem);
                BuildKit.Box("Margelle", g, p, new Vector3(side + 0.06f, 0.7f, 0.28f), Pal.Marbre, -(a + 90f));
                BuildKit.Box("Margelle_Zellige", g, p + new Vector3(Mathf.Cos(ar) * 0.15f, -0.05f, Mathf.Sin(ar) * 0.15f),
                             new Vector3(side, 0.4f, 0.03f), Pal.Zellige, -(a + 90f), false);
            }
            BuildKit.Cyl("Fond_Bassin", g, new Vector3(0f, 0.05f, 0f), 2.3f, 0.1f, Pal.Zellige);
            BuildKit.Cyl("Eau", g, new Vector3(0f, 0.55f, 0f), 2.15f, 0.04f, Pal.Eau);
            BuildKit.Cyl("Colonne", g, new Vector3(0f, 0.8f, 0f), 0.22f, 1.4f, Pal.Marbre);
            BuildKit.Cyl("Vasque", g, new Vector3(0f, 1.5f, 0f), 0.9f, 0.16f, Pal.Marbre);
            BuildKit.Cyl("Vasque_Eau", g, new Vector3(0f, 1.59f, 0f), 0.8f, 0.03f, Pal.Eau);
            BuildKit.Sph("Fleuron", g, new Vector3(0f, 1.75f, 0f), Vector3.one * 0.25f, Pal.Marbre);
            WaterJet(g, new Vector3(0f, 1.85f, 0f));
        }

        public static void WaterJet(Transform parent, Vector3 localPos)
        {
            var go = new GameObject("Jet_Eau");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 0.9f;
            main.startSpeed = 2.6f;
            main.startSize = 0.06f;
            main.gravityModifier = 1f;
            main.maxParticles = 600;
            main.startColor = new Color(0.85f, 0.93f, 1f, 0.8f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var em = ps.emission;
            em.rateOverTime = 220f;
            var sh = ps.shape;
            sh.shapeType = ParticleSystemShapeType.Cone;
            sh.angle = 12f;
            sh.radius = 0.02f;
            go.GetComponent<ParticleSystemRenderer>().sharedMaterial = BuildKit.ParticleMat();
        }
    }
}
