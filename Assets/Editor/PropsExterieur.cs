using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Matériaux de l'extérieur calibrés sur les photos de la visite (Reference_Photos/01 à 04) :
    /// teintes relevées à la pipette en zone éclairée puis ramenées à un albédo physique
    /// (chaux ≈ 0,85, jamais 1). Textures « photo » : Tools/make_photo_textures.py.
    /// </summary>
    public static partial class Pal
    {
        // Béton rose des allées et de l'esplanade, joints sciés tous les 2,5 m (photos 01, 24, 32)
        public static Material BetonRose => BuildKit.TexMat("Beton_Rose_Joints", () => TexExt.BetonRose, Vector2.one / 2.5f, 0.18f, Color.white);
        public static Material BordureBlanche => BuildKit.Mat("Bordure_Blanche", new Color(0.82f, 0.81f, 0.77f), 0.2f);
        // Bleus de Sidi Bou Saïd : bois peint mat (volets, chebbeks) et fer forgé laqué (grilles, balustrades)
        public static Material BleuBois => BuildKit.Mat("Bleu_Bois_Mat", new Color(0.06f, 0.29f, 0.58f), 0.25f);
        public static Material BleuFer => BuildKit.Mat("Bleu_Fer_Laque", new Color(0.05f, 0.27f, 0.6f), 0.62f, 0.25f);
        public static Material Brique => BuildKit.TexMat("Brique_Voute", () => TexExt.Brique, new Vector2(1f / 0.9f, 1f / 0.6f), 0.1f, Color.white);
        public static Material Gres => BuildKit.Mat("Gres_Ocre", new Color(0.72f, 0.6f, 0.43f), 0.15f);
        public static Material PierreSeche => BuildKit.TexMat("Pierre_Seche", () => TexExt.PierreSeche, Vector2.one / 1.2f, 0.08f, Color.white);
        public static Material EauVerte => BuildKit.Mat("Eau_Bassin_Verte", new Color(0.13f, 0.27f, 0.17f), 0.96f);
        public static Material CarreauTurquoise => BuildKit.Mat("Carreau_Turquoise", new Color(0.18f, 0.55f, 0.55f), 0.8f);
        public static Material Sable => BuildKit.Mat("Sable_Plage", new Color(0.78f, 0.7f, 0.55f), 0.05f);
        public static Material Maquis => BuildKit.Mat("Maquis_Pente", new Color(0.24f, 0.29f, 0.17f), 0.04f);
        public static Material FeuillageMaquis => BuildKit.Mat("Feuillage_Maquis", new Color(0.2f, 0.3f, 0.15f), 0.05f);
        public static Material EcorceEucalyptus => BuildKit.Mat("Ecorce_Eucalyptus", new Color(0.72f, 0.69f, 0.62f), 0.12f);
        public static Material FeuillageEucalyptus => BuildKit.Mat("Feuillage_Eucalyptus", new Color(0.36f, 0.45f, 0.31f), 0.1f);
        public static Material TroncCypres => BuildKit.Mat("Tronc_Cypres", new Color(0.33f, 0.29f, 0.25f), 0.05f);
        public static Material Agave => BuildKit.Mat("Agave", new Color(0.42f, 0.53f, 0.48f), 0.25f);
        public static Material Geranium => BuildKit.Mat("Geranium_Feuilles", new Color(0.22f, 0.4f, 0.17f), 0.1f);
        public static Material BrancheNue => BuildKit.Mat("Branche_Nue", new Color(0.3f, 0.26f, 0.22f), 0.05f);
        public static Material RougeDrapeau => BuildKit.Mat("Rouge_Drapeau", new Color(0.78f, 0.05f, 0.09f), 0.2f);
        public static Material BlancDrapeau => BuildKit.Mat("Blanc_Drapeau", new Color(0.9f, 0.9f, 0.88f), 0.2f);
        public static Material CoqueBateau => BuildKit.Mat("Coque_Bateau", new Color(0.86f, 0.86f, 0.84f), 0.4f);

        // Textures tirées des photos de la visite
        public static Material PorteJaunePhoto => BuildKit.TexMat("Photo_Porte_Jaune", () => Photo("Photo_Porte_Jaune"), Vector2.one, 0.3f, Color.white);
        public static Material PortailBandeau => BuildKit.TexMat("Photo_Portail_Bandeau", () => Photo("Photo_Portail_Bandeau"), Vector2.one, 0.15f, Color.white);
        public static Material PortailMontant => BuildKit.TexMat("Photo_Portail_Montant", () => Photo("Photo_Portail_Montant"), Vector2.one, 0.15f, Color.white);
        public static Material DallesRue => BuildKit.TexMat("Photo_Dalles_Rue", () => Photo("Photo_Dalles_Rue"), new Vector2(1f / 9f, 1f / 4f), 0.12f, Color.white);
        public static Material DallesCalcaire => BuildKit.TexMat("Photo_Dalles_Calcaire", () => Photo("Photo_Dalles_Calcaire"), new Vector2(1f / 8f, 1f / 3f), 0.25f, Color.white);
        // Frise de zellige vert et jaune du bassin : 0,20 m de haut, motif de 8,8 m (miroir compris)
        public static Material FriseZellige => BuildKit.TexMat("Photo_Frise_Zellige_Bassin", () => Photo("Photo_Frise_Zellige_Bassin"), new Vector2(1f / 8.8f, 1f / 0.2f), 0.7f, Color.white);
        public static Material FriseZelligeZ => BuildKit.TexMat("Photo_Frise_Zellige_Bassin_Z", () => Photo("Photo_Frise_Zellige_Bassin"), new Vector2(1f / 0.2f, 1f / 8.8f), 0.7f, Color.white);
    }

    /// <summary>Textures procédurales de l'extérieur (une texture = un motif de taille réelle).</summary>
    public static class TexExt
    {
        static float N(float u, float v, float s, float seed) => Mathf.PerlinNoise(u * s + seed, v * s + seed * 1.7f);

        /// <summary>Béton rose teinté dans la masse, 2,5 m × 2,5 m, joint scié sur deux bords.</summary>
        public static Texture2D BetonRose => BuildKit.ProcTexture("Beton_Rose_Joints", 512, 512, (u, v) =>
        {
            var c = new Color(0.56f, 0.4f, 0.38f);
            float n = 0.88f + 0.16f * N(u, v, 6f, 3.1f) + 0.06f * N(u, v, 40f, 7.3f);
            c *= n;
            float j = Mathf.Min(u, v) * 2.5f;                 // distance au joint, en mètres
            if (j < 0.012f) c *= 0.55f;
            else if (j < 0.03f) c *= 0.85f;
            c.a = 1f;
            return c;
        });

        /// <summary>Brique de la voûte de la skifa : rangs de 6 cm, briques de 22 cm, motif de 0,9 × 0,6 m.</summary>
        public static Texture2D Brique => BuildKit.ProcTexture("Brique_Voute", 512, 512, (u, v) =>
        {
            float y = v * 0.6f, x = u * 0.9f;
            int row = Mathf.FloorToInt(y / 0.06f);
            float xo = x + (row % 2) * 0.11f;
            float bx = xo % 0.225f, by = y % 0.06f;
            bool joint = bx < 0.012f || by < 0.01f;
            float k = Mathf.Repeat(Mathf.Floor(xo / 0.225f) * 0.618f + row * 0.37f, 1f);
            var c = joint ? new Color(0.7f, 0.64f, 0.55f) : Color.Lerp(new Color(0.5f, 0.28f, 0.2f), new Color(0.64f, 0.38f, 0.27f), k);
            c *= 0.9f + 0.15f * N(u, v, 30f, 1.3f);
            c.a = 1f;
            return c;
        });

        /// <summary>Murets de pierre sèche : moellons calcaires irréguliers, motif de 1,2 m.</summary>
        public static Texture2D PierreSeche => BuildKit.ProcTexture("Pierre_Seche", 512, 512, (u, v) =>
        {
            float cells = 6f;
            float bu = u * cells + 0.5f * Mathf.Floor(v * cells * 1.6f);
            float fu = Mathf.Repeat(bu, 1f), fv = Mathf.Repeat(v * cells * 1.6f, 1f);
            float edge = Mathf.Min(Mathf.Min(fu, 1f - fu), Mathf.Min(fv, 1f - fv) * 0.8f);
            float k = N(Mathf.Floor(bu), Mathf.Floor(v * cells * 1.6f), 0.37f, 2.2f);
            var c = Color.Lerp(new Color(0.55f, 0.5f, 0.42f), new Color(0.72f, 0.66f, 0.55f), k);
            c *= 0.85f + 0.2f * N(u, v, 25f, 5.5f);
            if (edge < 0.06f) c *= 0.45f;
            c.a = 1f;
            return c;
        });
    }

    /// <summary>Accessoires de l'extérieur relevés sur les photos (végétation réelle, ferronneries, jardin persan…).</summary>
    public static class PropsExterieur
    {
        // ================================================================ Végétation
        /// <summary>
        /// Eucalyptus à tronc pâle. tetard = taillé en têtard : tronc court et repousses en boules (photos 12, 21) ;
        /// sinon port libre, grandes branches obliques et feuillage clair et aéré (photos 01, 32).
        /// </summary>
        public static void Eucalyptus(Transform parent, Vector3 pos, float height, bool tetard)
        {
            var g = BuildKit.Group(tetard ? "Eucalyptus_Tetard" : "Eucalyptus", parent, pos, new Vector3(0f, BuildKit.R(0f, 360f), 0f));
            var lean = new Vector3(BuildKit.R(-5f, 5f), 0f, BuildKit.R(-5f, 5f));
            if (tetard)
            {
                // Têtard : tronc pâle étêté, repousses courtes en boules de feuillage (photos 12, 21, 147)
                float h = height * 0.55f;
                BuildKit.Cyl("Tronc", g, new Vector3(0f, h * 0.5f, 0f), 0.22f, h, Pal.EcorceEucalyptus, lean, true);
                int n = BuildKit.RI(4, 7);
                for (int i = 0; i < n; i++)
                {
                    float a = i * Mathf.PI * 2f / n + BuildKit.R(-0.3f, 0.3f);
                    var top = new Vector3(Mathf.Cos(a) * BuildKit.R(0.4f, 1.0f), h + BuildKit.R(0.6f, height * 0.4f), Mathf.Sin(a) * BuildKit.R(0.4f, 1.0f));
                    var d = top - new Vector3(0f, h, 0f);
                    BuildKit.Cyl("Repousse", g, new Vector3(0f, h, 0f) + d * 0.5f, 0.06f, d.magnitude, Pal.EcorceEucalyptus, Quaternion.FromToRotation(Vector3.up, d.normalized).eulerAngles);
                    float s = BuildKit.R(0.6f, 0.95f);
                    Veg.Couronne(g, top, new Vector3(s, s * 0.9f, s), 9, 1.0f, Pal.CarteEucalyptus, Pal.CoeurEucalyptus, 0.55f);
                }
                return;
            }
            // Port libre : grandes branches obliques et feuillage aéré qui retombe (photos 01, 32, 138)
            BuildKit.Cyl("Tronc", g, new Vector3(0f, height * 0.3f, 0f), BuildKit.R(0.16f, 0.24f), height * 0.6f, Pal.EcorceEucalyptus, lean, true);
            int b = BuildKit.RI(3, 5);
            for (int i = 0; i < b; i++)
            {
                float a = i * Mathf.PI * 2f / b + BuildKit.R(-0.4f, 0.4f);
                var start = new Vector3(0f, height * BuildKit.R(0.45f, 0.6f), 0f);
                var end = start + new Vector3(Mathf.Cos(a) * BuildKit.R(2f, 4f), height * BuildKit.R(0.25f, 0.4f), Mathf.Sin(a) * BuildKit.R(2f, 4f));
                var d = end - start;
                BuildKit.Cyl("Branche", g, (start + end) * 0.5f, 0.08f, d.magnitude, Pal.EcorceEucalyptus, Quaternion.FromToRotation(Vector3.up, d.normalized).eulerAngles);
                float s = BuildKit.R(1.8f, 2.6f);
                Veg.Couronne(g, end, new Vector3(s, s * 0.6f, s), 16, 1.8f, Pal.CarteEucalyptus, Pal.CoeurEucalyptus, 0.45f);
            }
        }

        /// <summary>Cyprès méditerranéen ancien : tronc visible en bas, flamme haute et irrégulière (photos 10, 13, 16).</summary>
        public static void CypresAncien(Transform parent, Vector3 pos, float height)
        {
            var g = BuildKit.Group("Cypres_Ancien", parent, pos);
            float r = BuildKit.R(0.9f, 1.3f);
            float bottom = BuildKit.R(3f, 5.5f);   // troncs nus visibles sur 3 à 5 m (photos 10, 121, 157)
            BuildKit.Cyl("Tronc", g, new Vector3(0f, bottom * 0.5f + 0.2f, 0f), 0.22f, bottom + 0.4f, Pal.TroncCypres, Vector3.zero, true);
            float body = height - bottom;
            BuildKit.Cap("Coeur", g, new Vector3(0f, bottom + body * 0.5f, 0f), r * 0.7f, body * 0.92f, Pal.CoeurCypres, false);
            Veg.Couronne(g, new Vector3(0f, bottom + body * 0.5f, 0f), new Vector3(r, body * 0.5f, r), Mathf.RoundToInt(body * 3.2f), 1.4f, Pal.CarteCypres, null);
        }

        /// <summary>Agave : rosette de feuilles épaisses gris-bleu.</summary>
        public static void Agave(Transform parent, Vector3 pos, float size = 1f)
        {
            var g = BuildKit.Group("Agave", parent, pos, new Vector3(0f, BuildKit.R(0f, 360f), 0f));
            int n = BuildKit.RI(11, 16);
            for (int i = 0; i < n; i++)
            {
                var leaf = BuildKit.Group("Feuille", g, Vector3.zero, new Vector3(BuildKit.R(-70f, -30f), i * 360f / n, 0f));
                BuildKit.Box("Limbe", leaf, new Vector3(0f, 0f, 0.45f * size), new Vector3(0.14f * size, 0.04f * size, 0.9f * size), Pal.Agave, 0f, false);
            }
        }

        /// <summary>Arbuste bas (pittosporum, laurier-rose, lentisque) en touffes.</summary>
        public static void Arbuste(Transform parent, Vector3 pos, float size, Material m = null)
        {
            var g = BuildKit.Group("Arbuste", parent, pos);
            var feuilles = m == Pal.Haie ? Pal.CarteMaquis : Pal.CarteMaquis;
            Veg.Couronne(g, new Vector3(0f, size * 0.42f, 0f), new Vector3(size * 0.55f, size * 0.42f, size * 0.55f), Mathf.Max(6, Mathf.RoundToInt(size * 7f)), Mathf.Max(0.6f, size * 0.55f), feuilles, Pal.CoeurSombre);
        }

        /// <summary>Jeune bigaradier (oranger amer) des carrés du jardin persan.</summary>
        public static void Bigaradier(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Bigaradier", parent, pos);
            BuildKit.Cyl("Tronc", g, new Vector3(0f, 0.6f, 0f), 0.05f, 1.2f, Pal.Tronc);
            Veg.Couronne(g, new Vector3(0f, 1.7f, 0f), new Vector3(0.6f, 0.7f, 0.6f), 16, 0.75f, Pal.CarteBigaradier, null);
        }

        /// <summary>Arbre caduc dénudé (photos d'hiver 28 à 35) : tronc et branches sans feuilles.</summary>
        public static void ArbreNu(Transform parent, Vector3 pos, float height)
        {
            var g = BuildKit.Group("Arbre_Caduc_Nu", parent, pos);
            BuildKit.Cyl("Tronc", g, new Vector3(0f, height * 0.25f, 0f), 0.2f, height * 0.5f, Pal.BrancheNue, Vector3.zero, true);
            Branches(g, new Vector3(0f, height * 0.45f, 0f), Vector3.up, height * 0.35f, 0.12f, 3);
        }

        static void Branches(Transform g, Vector3 start, Vector3 dir, float len, float r, int depth)
        {
            if (depth == 0) return;
            int n = BuildKit.RI(2, 4);
            for (int i = 0; i < n; i++)
            {
                var d = (dir + new Vector3(BuildKit.R(-0.9f, 0.9f), BuildKit.R(0.2f, 0.8f), BuildKit.R(-0.9f, 0.9f))).normalized;
                var end = start + d * len;
                BuildKit.Cyl("Branche", g, (start + end) * 0.5f, r, len, Pal.BrancheNue, Quaternion.FromToRotation(Vector3.up, d).eulerAngles);
                Branches(g, end, d, len * 0.65f, r * 0.6f, depth - 1);
            }
        }

        // ================================================================ Esplanade
        /// <summary>Pilier blanc carré à chaperon (photos 32, 41).</summary>
        public static void PilierBlanc(Transform parent, Vector3 pos, float h = 0.95f, float w = 0.45f)
        {
            var g = BuildKit.Group("Pilier", parent, pos);
            BuildKit.Box("Fut", g, new Vector3(0f, h * 0.5f, 0f), new Vector3(w, h, w), Pal.Chaux);
            BuildKit.Box("Chaperon", g, new Vector3(0f, h + 0.04f, 0f), new Vector3(w + 0.08f, 0.08f, w + 0.08f), Pal.Chaux, 0f, false);
        }

        /// <summary>
        /// Balustrade bleue à barreaux entre piliers blancs (photo 32), de a à b (au sol).
        /// Piliers tous les ≈ 3 m ; une barrière invisible empêche de l'enjamber.
        /// </summary>
        public static void BalustradeBleue(Transform parent, Vector3 a, Vector3 b, float spacing = 3f)
        {
            var g = BuildKit.Group("Balustrade_Bleue", parent);
            Vector3 d = b - a; d.y = 0f;
            float len = d.magnitude;
            Vector3 dir = d / len;
            float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            int n = Mathf.Max(1, Mathf.RoundToInt(len / spacing));
            float step = len / n;
            for (int i = 0; i <= n; i++) PilierBlanc(g, a + dir * (i * step));
            for (int i = 0; i < n; i++)
            {
                Vector3 s0 = a + dir * (i * step + 0.25f), s1 = a + dir * ((i + 1) * step - 0.25f);
                Vector3 mid = (s0 + s1) * 0.5f;
                float l = (s1 - s0).magnitude;
                BuildKit.Box("Lisse_Haute", g, mid + Vector3.up * 0.78f, new Vector3(0.07f, 0.06f, l), Pal.BleuFer, yaw, false);
                BuildKit.Box("Lisse_Basse", g, mid + Vector3.up * 0.12f, new Vector3(0.06f, 0.05f, l), Pal.BleuFer, yaw, false);
                int bars = Mathf.RoundToInt(l / 0.13f);
                for (int k = 1; k < bars; k++)
                    BuildKit.Box("Barreau", g, Vector3.Lerp(s0, s1, k / (float)bars) + Vector3.up * 0.45f, new Vector3(0.035f, 0.66f, 0.035f), Pal.BleuFer, yaw, false);
            }
            BuildKit.InvisibleWall("Limite_Balustrade", g, (a + b) * 0.5f + Vector3.up * 1.5f, (Quaternion.Euler(0f, yaw, 0f) * new Vector3(0.3f, 3f, len)).Abs());
        }

        public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        /// <summary>Pilier blanc à chapeau pyramidal portant une lanterne de fer (photos 22, 25, 27).</summary>
        public static void PilierLanterne(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Pilier_Lanterne", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Fut", g, new Vector3(0f, 0.65f, 0f), new Vector3(0.55f, 1.3f, 0.55f), Pal.Chaux);
            BuildKit.Box("Corniche", g, new Vector3(0f, 1.34f, 0f), new Vector3(0.66f, 0.08f, 0.66f), Pal.Chaux, 0f, false);
            MeshKit.Pyramid("Chapeau", g, new Vector3(0f, 1.38f, 0f), 0.62f, 0.62f, 0.35f, Pal.Chaux);
            BuildKit.Box("Potence", g, new Vector3(0f, 1.05f, -0.36f), new Vector3(0.03f, 0.03f, 0.18f), Pal.Fer, 0f, false);
            BuildKit.Box("Lanterne", g, new Vector3(0f, 0.9f, -0.44f), new Vector3(0.16f, 0.26f, 0.16f), Pal.LanterneVerre, 0f, false);
            BuildKit.Box("Toit_Lanterne", g, new Vector3(0f, 1.05f, -0.44f), new Vector3(0.2f, 0.04f, 0.2f), Pal.Fer, 0f, false);
        }

        // ================================================================ Jardin persan
        /// <summary>Petit lion couché en marbre blanc, sur socle (photos 36, 45).</summary>
        public static void Lion(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Lion_Marbre", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Socle", g, new Vector3(0f, 0.12f, 0f), new Vector3(0.36f, 0.24f, 0.6f), Pal.Marbre);
            BuildKit.Box("Corps", g, new Vector3(0f, 0.38f, 0.05f), new Vector3(0.26f, 0.28f, 0.48f), Pal.Marbre, 0f, false);
            BuildKit.Sph("Criniere", g, new Vector3(0f, 0.6f, -0.17f), new Vector3(0.3f, 0.32f, 0.26f), Pal.Marbre);
            BuildKit.Sph("Museau", g, new Vector3(0f, 0.56f, -0.31f), new Vector3(0.14f, 0.12f, 0.12f), Pal.Marbre);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Patte", g, new Vector3(s * 0.08f, 0.29f, -0.26f), new Vector3(0.07f, 0.1f, 0.18f), Pal.Marbre, 0f, false);
        }

        // ================================================================ Façades
        /// <summary>Fenêtre à volets persiennés bleus fermés (étage de la façade, W94, W98). pos = bas-centre, face vers -Z local.</summary>
        public static void FenetrePersiennes(Transform parent, Vector3 pos, float yaw, float w = 1.1f, float h = 1.6f)
        {
            var g = BuildKit.Group("Fenetre_Persiennes", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Tableau", g, new Vector3(0f, h * 0.5f, -0.02f), new Vector3(w + 0.24f, h + 0.24f, 0.05f), Pal.Chaux, 0f, false);
            BuildKit.Box("Volets", g, new Vector3(0f, h * 0.5f, -0.06f), new Vector3(w, h, 0.04f), Pal.BleuBois, 0f, false);
            int n = Mathf.RoundToInt(h / 0.07f);
            for (int i = 1; i < n; i++)
                BuildKit.Box("Lame", g, new Vector3(0f, i * h / n, -0.09f), new Vector3(w - 0.08f, 0.018f, 0.03f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Battement", g, new Vector3(0f, h * 0.5f, -0.1f), new Vector3(0.04f, h, 0.03f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Appui", g, new Vector3(0f, -0.05f, -0.1f), new Vector3(w + 0.3f, 0.08f, 0.22f), Pal.Marbre, 0f, false);
        }

        /// <summary>Drapeau tunisien sur hampe (photo 23, W94, W95, W98).</summary>
        public static void DrapeauTunisien(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Drapeau_Tunisien", parent, pos);
            BuildKit.Cyl("Hampe", g, new Vector3(0f, 2.5f, 0f), 0.04f, 5f, Pal.BlancDrapeau);
            var f = BuildKit.Group("Drapeau", g, new Vector3(0.95f, 4.3f, 0f));
            BuildKit.Box("Fond_Rouge", f, Vector3.zero, new Vector3(1.8f, 1.2f, 0.02f), Pal.RougeDrapeau, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                float z = s * 0.012f;
                BuildKit.Cyl("Disque_Blanc", f, new Vector3(0f, 0f, z), 0.3f, 0.004f, Pal.BlancDrapeau, new Vector3(90f, 0f, 0f));
                BuildKit.Cyl("Croissant", f, new Vector3(-0.03f, 0f, z * 1.25f), 0.22f, 0.004f, Pal.RougeDrapeau, new Vector3(90f, 0f, 0f));
                BuildKit.Cyl("Croissant_Creux", f, new Vector3(0.03f, 0f, z * 1.5f), 0.18f, 0.004f, Pal.BlancDrapeau, new Vector3(90f, 0f, 0f));
                BuildKit.Cyl("Etoile", f, new Vector3(0.08f, 0f, z * 1.75f), 0.07f, 0.004f, Pal.RougeDrapeau, new Vector3(90f, 0f, 0f));
            }
        }

        // ================================================================ Lointain
        /// <summary>Voilier ou bateau de plaisance au mouillage (port de Sidi Bou Saïd).</summary>
        public static void Bateau(Transform parent, Vector3 pos, float yaw, float len)
        {
            var g = BuildKit.Group("Bateau", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Coque", g, new Vector3(0f, 0.3f, 0f), new Vector3(len * 0.32f, 0.8f, len), Pal.CoqueBateau, 0f, false);
            BuildKit.Box("Cabine", g, new Vector3(0f, 0.9f, -len * 0.1f), new Vector3(len * 0.2f, 0.5f, len * 0.35f), Pal.CoqueBateau, 0f, false);
            if (BuildKit.Chance(0.7f))
                BuildKit.Cyl("Mat", g, new Vector3(0f, len * 0.7f, len * 0.08f), 0.06f, len * 1.3f, Pal.CoqueBateau);
        }
    }
}
