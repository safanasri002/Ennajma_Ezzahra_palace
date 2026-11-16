using UnityEditor;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Détails de l'extérieur relevés sur les photos complémentaires (Reference_Photos/21_exterieur_complements) :
    /// ferronneries à volutes, porte et auvent du jardin persan, kiosque à faïences, façade à étage en retrait,
    /// maisons de Sidi Bou Saïd, végétation (lantanas, bougainvilliers, lavandes, palmiers des Canaries, pins).
    /// </summary>
    public static partial class Pal
    {
        /// <summary>Matériau à découpe alpha (ferronneries ajourées), visible des deux côtés.</summary>
        static Material Cutout(Material m)
        {
            m.SetFloat("_AlphaClip", 1f);
            m.SetFloat("_Cutoff", 0.5f);
            m.EnableKeyword("_ALPHATEST_ON");
            if (m.HasProperty("_Cull")) m.SetFloat("_Cull", 0f);
            m.SetOverrideTag("RenderType", "TransparentCutout");
            m.renderQueue = 2450;
            m.doubleSidedGI = true;
            EditorUtility.SetDirty(m);
            return m;
        }

        // Grilles basses à volutes des carrés (motif de 0,5 m × 0,4 m) et grille bombée de l'étage (motif de 0,45 × 0,45 m)
        public static Material GrilleVolutes => Cutout(BuildKit.TexMat("Fer_Volutes_Bleu", () => TexDet.Volutes, new Vector2(1f / 0.5f, 1f / 0.4f), 0.6f, Color.white));
        public static Material GrilleBombee => Cutout(BuildKit.TexMat("Fer_Volutes_Bombe", () => TexDet.Volutes, new Vector2(1f / 0.45f, 1f / 0.45f), 0.6f, Color.white));
        // Grille carrée sombre des fenêtres du rez-de-chaussée (maille 0,12 m)
        public static Material GrilleCarree => Cutout(BuildKit.TexMat("Fer_Grille_Carree", () => TexDet.GrilleCarree, Vector2.one / 0.24f, 0.4f, Color.white));
        // Treillis bleu de l'auvent demi-cercle (losanges de 8 cm)
        public static Material TreillisBleu => BuildKit.TexMat("Treillis_Bleu", () => TexDet.Treillis, Vector2.one / 0.32f, 0.3f, Color.white);
        // Tuiles vernissées vertes en écailles (kiosque, auvents, chebbeks)
        public static Material TuilesEcailles => BuildKit.TexMat("Tuiles_Vertes_Ecailles", () => TexDet.Tuiles, Vector2.one / 0.6f, 0.65f, Color.white);
        public static Material FaienceKiosque => BuildKit.TexMat("Photo_Faience_Kiosque", () => Photo("Photo_Faience_Kiosque"), new Vector2(1f / 3.6f, 1f / 1.35f), 0.6f, Color.white);
        public static Material PorteBleueFer => BuildKit.TexMat("Photo_Porte_Bleue_Fer", () => Photo("Photo_Porte_Bleue_Fer"), Vector2.one, 0.45f, Color.white);

        public static Material Lantana => BuildKit.Mat("Lantana_Fleurs", new Color(0.92f, 0.38f, 0.1f), 0.2f);
        public static Material Calendula => BuildKit.Mat("Soucis_Fleurs", new Color(0.95f, 0.62f, 0.08f), 0.2f);
        public static Material Lavande => BuildKit.Mat("Lavande_Grise", new Color(0.55f, 0.6f, 0.55f), 0.1f);
        public static Material Marguerite => BuildKit.Mat("Marguerite", new Color(0.88f, 0.88f, 0.84f), 0.2f);
        public static Material FeuilleBougainvillier => BuildKit.Mat("Feuille_Bougainvillier", new Color(0.16f, 0.3f, 0.12f), 0.15f);
        public static Material ToitTerreCuite => BuildKit.Mat("Toit_Terre_Cuite", new Color(0.52f, 0.33f, 0.24f), 0.2f);
        public static Material Pin => BuildKit.Mat("Feuillage_Pin", new Color(0.12f, 0.2f, 0.12f), 0.05f);
        public static Material TroncPin => BuildKit.Mat("Tronc_Pin", new Color(0.32f, 0.24f, 0.2f), 0.05f);
        public static Material Projecteur => BuildKit.Mat("Projecteur_Gris", new Color(0.25f, 0.26f, 0.27f), 0.5f, 0.6f);
    }

    /// <summary>Textures procédurales des détails (ferronneries, treillis, tuiles).</summary>
    public static class TexDet
    {
        static readonly Color Bleu = new Color(0.05f, 0.27f, 0.6f, 1f);

        static float Ring(float u, float v, float cu, float cv, float r)
        {
            float d = Mathf.Sqrt((u - cu) * (u - cu) + (v - cv) * (v - cv));
            return Mathf.Abs(d - r);
        }

        /// <summary>Volutes en S de fer forgé (photos 130, 142 à 145) : deux crosses opposées, lisses haute et basse, alpha = fer.</summary>
        public static Texture2D Volutes => BuildKit.ProcTexture("Fer_Volutes", 256, 256, (u, v) =>
        {
            const float t = 0.03f;
            bool fer = v < 0.05f || v > 0.95f || u < 0.025f || u > 0.975f;
            // Grandes crosses
            fer |= Ring(u, v, 0.3f, 0.6f, 0.2f) < t && !(u > 0.3f && v < 0.6f);
            fer |= Ring(u, v, 0.7f, 0.4f, 0.2f) < t && !(u < 0.7f && v > 0.4f);
            // Enroulements intérieurs
            fer |= Ring(u, v, 0.3f, 0.6f, 0.08f) < t * 0.8f && v > 0.55f;
            fer |= Ring(u, v, 0.7f, 0.4f, 0.08f) < t * 0.8f && v < 0.45f;
            // Tige centrale qui relie les deux crosses
            fer |= Mathf.Abs(u - 0.5f) < t * 0.7f && v > 0.38f && v < 0.62f;
            // Petites crosses de bord
            fer |= Ring(u, v, 0.1f, 0.2f, 0.09f) < t * 0.8f && u > 0.05f;
            fer |= Ring(u, v, 0.9f, 0.8f, 0.09f) < t * 0.8f && u < 0.95f;
            return fer ? Bleu : new Color(0f, 0f, 0f, 0f);
        });

        public static Texture2D GrilleCarree => BuildKit.ProcTexture("Fer_Grille_Carree", 128, 128, (u, v) =>
        {
            float fu = Mathf.Repeat(u * 2f, 1f), fv = Mathf.Repeat(v * 2f, 1f);
            bool fer = fu < 0.1f || fv < 0.1f || Mathf.Abs(fu - fv) < 0.05f && fu > 0.35f && fu < 0.65f;
            return fer ? new Color(0.1f, 0.12f, 0.14f, 1f) : new Color(0f, 0f, 0f, 0f);
        });

        public static Texture2D Treillis => BuildKit.ProcTexture("Treillis_Bleu", 256, 256, (u, v) =>
        {
            float fu = Mathf.Repeat(u * 4f, 1f), fv = Mathf.Repeat(v * 4f, 1f);
            bool trou = Mathf.Abs(fu - 0.5f) + Mathf.Abs(fv - 0.5f) < 0.33f;
            return trou ? new Color(0.04f, 0.09f, 0.16f) : new Color(0.08f, 0.33f, 0.66f);
        });

        /// <summary>Tuiles vernissées vertes en écailles, rangs décalés, teintes variées (photos 125, 129, 130).</summary>
        public static Texture2D Tuiles => BuildKit.ProcTexture("Tuiles_Vertes_Ecailles", 256, 256, (u, v) =>
        {
            float rows = 6f, cols = 5f;
            float rv = v * rows;
            int row = Mathf.FloorToInt(rv);
            float cu = u * cols + (row % 2) * 0.5f;
            float fu = Mathf.Repeat(cu, 1f) - 0.5f, fv = rv - row;
            float k = Mathf.Repeat(Mathf.Floor(cu) * 0.731f + row * 0.377f, 1f);
            var c = Color.Lerp(new Color(0.24f, 0.4f, 0.3f), new Color(0.42f, 0.52f, 0.36f), k);
            if (fu * fu * 3.2f + (1f - fv) * (1f - fv) * 0.35f > 0.25f && fv < 0.22f) c *= 0.55f;   // bord arrondi de l'écaille
            c *= 0.85f + 0.3f * fv;
            c.a = 1f;
            return c;
        });
    }

    /// <summary>Accessoires détaillés de l'extérieur (d'après les photos complémentaires).</summary>
    public static class PropsDetails
    {
        // ================================================================ Ferronneries
        /// <summary>Grille basse à volutes entre deux points (pommeaux sur les poteaux), photos 130, 142 à 145.</summary>
        public static void GrilleVolutes(Transform parent, Vector3 a, Vector3 b, float h = 0.42f)
        {
            Vector3 d = b - a; d.y = 0f;
            float len = d.magnitude;
            float yaw = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
            var g = BuildKit.Group("Grille_Volutes", parent, a, new Vector3(0f, yaw, 0f));
            int n = Mathf.Max(1, Mathf.CeilToInt(len / 1.2f));
            for (int i = 0; i <= n; i++)
            {
                float z = len * i / n;
                BuildKit.Box("Poteau", g, new Vector3(0f, h * 0.5f + 0.02f, z), new Vector3(0.05f, h + 0.04f, 0.05f), Pal.BleuFer, 0f, false);
                BuildKit.Sph("Pommeau", g, new Vector3(0f, h + 0.08f, z), new Vector3(0.08f, 0.1f, 0.08f), Pal.BleuFer);
            }
            MeshKit.WBox("Volutes", g, new Vector3(0f, h * 0.5f, len * 0.5f), new Vector3(0.012f, h, len), Pal.GrilleVolutes, 0f, false);
            BuildKit.Box("Main_Courante", g, new Vector3(0f, h + 0.01f, len * 0.5f), new Vector3(0.04f, 0.03f, len), Pal.BleuFer, 0f, false);
        }

        // ================================================================ Jardin persan
        /// <summary>
        /// Carré de plantation : bordure de marbre, filet de zellige au sol, grille à volutes, plantes.
        /// kind 0 = géraniums et bigaradier ; 1 = géraniums, soucis orange et jaunes ; 2 = lavandes grises.
        /// </summary>
        public static void Carre(Transform parent, Vector3 center, float sx, float sz, int kind, bool arbre)
        {
            var g = BuildKit.Group("Carre_Persan", parent, center);
            float hx = sx * 0.5f, hz = sz * 0.5f;
            MeshKit.WBox("Filet_Zellige_N", g, new Vector3(0f, 0.005f, hz + 0.08f), new Vector3(sx + 0.32f, 0.01f, 0.12f), Pal.FriseZellige, 0f, false);
            MeshKit.WBox("Filet_Zellige_S", g, new Vector3(0f, 0.005f, -hz - 0.08f), new Vector3(sx + 0.32f, 0.01f, 0.12f), Pal.FriseZellige, 0f, false);
            MeshKit.WBox("Filet_Zellige_E", g, new Vector3(hx + 0.08f, 0.005f, 0f), new Vector3(0.12f, 0.01f, sz), Pal.FriseZelligeZ, 0f, false);
            MeshKit.WBox("Filet_Zellige_O", g, new Vector3(-hx - 0.08f, 0.005f, 0f), new Vector3(0.12f, 0.01f, sz), Pal.FriseZelligeZ, 0f, false);
            BuildKit.Box("Bordure", g, new Vector3(0f, 0.12f, 0f), new Vector3(sx, 0.24f, sz), Pal.Marbre);
            BuildKit.Box("Terre", g, new Vector3(0f, 0.245f, 0f), new Vector3(sx - 0.3f, 0.01f, sz - 0.3f), Pal.Terre, 0f, false);
            float ex = hx - 0.08f, ez = hz - 0.08f;
            var p = new[] { new Vector3(-ex, 0.24f, -ez), new Vector3(ex, 0.24f, -ez), new Vector3(ex, 0.24f, ez), new Vector3(-ex, 0.24f, ez) };
            for (int i = 0; i < 4; i++) GrilleVolutes(g, p[i], p[(i + 1) % 4]);

            if (arbre) PropsExterieur.Bigaradier(g, new Vector3(BuildKit.R(-0.3f, 0.3f), 0.25f, BuildKit.R(-0.3f, 0.3f)));
            var plantes = kind == 2 ? Pal.CarteLavande : kind == 1 ? Pal.CarteSoucis : Pal.CarteGeranium;
            Veg.Massif(g, new Vector3(0f, 0.25f, 0f), sx - 0.45f, sz - 0.45f, 0.5f, plantes, 5f);
        }

        /// <summary>Vasque de grès sur fût cannelé, dans un bassin octogonal à gradin (photos 142 à 145).</summary>
        public static void Vasque(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Vasque_Persane", parent, pos);
            for (int i = 0; i < 8; i++)
            {
                float a = i * 45f, ar = a * Mathf.Deg2Rad;
                BuildKit.Box("Gradin", g, new Vector3(Mathf.Cos(ar) * 0.95f, 0.05f, Mathf.Sin(ar) * 0.95f), new Vector3(0.82f, 0.1f, 0.3f), Pal.Marbre, -(a + 90f));
                BuildKit.Box("Margelle", g, new Vector3(Mathf.Cos(ar) * 0.72f, 0.07f, Mathf.Sin(ar) * 0.72f), new Vector3(0.6f, 0.14f, 0.1f), Pal.Gres, -(a + 90f), false);
            }
            BuildKit.Cyl("Eau", g, new Vector3(0f, 0.06f, 0f), 0.7f, 0.01f, Pal.Eau);
            BuildKit.Cyl("Socle", g, new Vector3(0f, 0.12f, 0f), 0.24f, 0.2f, Pal.Gres, Vector3.zero, true);
            BuildKit.Cyl("Fut", g, new Vector3(0f, 0.5f, 0f), 0.13f, 0.6f, Pal.Gres);
            for (int i = 0; i < 12; i++)
            {
                float a = i * 30f * Mathf.Deg2Rad;
                BuildKit.Box("Cannelure", g, new Vector3(Mathf.Cos(a) * 0.13f, 0.5f, Mathf.Sin(a) * 0.13f), new Vector3(0.03f, 0.5f, 0.03f), Pal.Gres, -i * 30f, false);
            }
            BuildKit.Cyl("Bague", g, new Vector3(0f, 0.42f, 0f), 0.17f, 0.06f, Pal.Gres);
            BuildKit.Sph("Bulbe", g, new Vector3(0f, 0.82f, 0f), new Vector3(0.34f, 0.18f, 0.34f), Pal.Gres);
            BuildKit.Cyl("Coupe", g, new Vector3(0f, 0.95f, 0f), 0.4f, 0.12f, Pal.Gres);
            BuildKit.Cyl("Levre", g, new Vector3(0f, 1.02f, 0f), 0.43f, 0.03f, Pal.Gres);
            BuildKit.Cyl("Eau_Coupe", g, new Vector3(0f, 1.03f, 0f), 0.37f, 0.01f, Pal.Eau);
            BuildKit.Cyl("Jet", g, new Vector3(0f, 1.12f, 0f), 0.008f, 0.16f, Pal.Eau);
        }

        /// <summary>
        /// Porte du jardin persan (photos 134 à 146) : portes de fer forgé bleu (texture photo), encadrement de marbre,
        /// deux marches, colonnes, auvent demi-cercle en treillis avec ajour central, petit auvent latéral, lions.
        /// pos = au pied du mur, face vers −Z local.
        /// </summary>
        public static void PortePersane(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Porte_Persane", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Marche_1", g, new Vector3(0f, 0.08f, -0.95f), new Vector3(3.6f, 0.16f, 0.5f), Pal.Marbre);
            BuildKit.Box("Marche_2", g, new Vector3(0f, 0.16f, -0.4f), new Vector3(3.6f, 0.32f, 0.8f), Pal.Marbre);
            BuildKit.Box("Encadrement", g, new Vector3(0f, 1.62f, -0.03f), new Vector3(1.9f, 2.65f, 0.06f), Pal.Marbre, 0f, false);
            BuildKit.Box("Linteau_Sculpte", g, new Vector3(0f, 2.86f, -0.05f), new Vector3(1.9f, 0.18f, 0.08f), Pal.Marbre, 0f, false);
            BuildKit.Box("Porte_Fer", g, new Vector3(0f, 1.52f, -0.07f), new Vector3(1.45f, 2.4f, 0.04f), Pal.PorteBleueFer, 0f, false);
            BuildKit.Box("Applique", g, new Vector3(0f, 3.05f, -0.12f), new Vector3(0.14f, 0.22f, 0.14f), Pal.Fer, 0f, false);

            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Base", g, new Vector3(s * 1.55f, 0.24f, -0.85f), new Vector3(0.3f, 0.16f, 0.3f), Pal.Marbre, 0f, false);
                BuildKit.Cyl("Colonne", g, new Vector3(s * 1.55f, 1.62f, -0.85f), 0.1f, 2.6f, Pal.Marbre, Vector3.zero, true);
                BuildKit.Box("Chapiteau", g, new Vector3(s * 1.55f, 3.0f, -0.85f), new Vector3(0.28f, 0.16f, 0.28f), Pal.Marbre, 0f, false);
                PropsExterieur.Lion(g, new Vector3(s * 1.15f, 0.32f, -0.45f), s * 20f + 180f);
            }
            var hood = BuildKit.Group("Auvent", g, new Vector3(0f, 3.08f, 0f));
            MeshKit.QuarterDome("Demi_Coupole_Treillis", hood, Vector3.zero, 1.75f, 1.0f, Pal.TreillisBleu);
            MeshKit.QuarterDome("Ajour_Central", hood, new Vector3(0f, 0f, -0.01f), 0.8f, 1.02f, Pal.GrilleBombee);
            BuildKit.Box("Bandeau", hood, new Vector3(0f, 0.03f, -0.5f), new Vector3(3.5f, 0.07f, 1.0f), Pal.BleuBois, 0f, false);
            // Petit auvent latéral (demi-coupole réduite, à gauche de la porte)
            MeshKit.QuarterDome("Auvent_Lateral", g, new Vector3(-2.9f, 2.3f, 0f), 0.45f, 0.35f, Pal.TreillisBleu);
        }

        /// <summary>Fenêtre de l'étage à grille de fer bombée (photos 134, 141, 146) : pilastres de marbre, auvent bleu à denticules.</summary>
        public static void FenetreBombee(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Fenetre_Bombee", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Fond", g, new Vector3(0f, 1.15f, -0.02f), new Vector3(1.9f, 2.3f, 0.04f), Pal.VitreSombre, 0f, false);
            BuildKit.Box("Volets_Ouverts", g, new Vector3(0f, 1.6f, -0.05f), new Vector3(1.6f, 1.0f, 0.03f), Pal.BleuBois, 0f, false);
            MeshKit.Bombe("Grille_Bombee", g, new Vector3(0f, 0f, -0.08f), 1.95f, 2.3f, 0.55f, Pal.GrilleBombee);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Cadre_Montant", g, new Vector3(s * 1.0f, 1.15f, -0.07f), new Vector3(0.08f, 2.4f, 0.04f), Pal.BleuBois, 0f, false);
                BuildKit.Box("Cadre_Traverse", g, new Vector3(0f, s < 0 ? 0.0f : 2.32f, -0.07f), new Vector3(2.05f, 0.08f, 0.04f), Pal.BleuBois, 0f, false);
            }
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Pilastre", g, new Vector3(s * 1.35f, 1.25f, -0.1f), new Vector3(0.22f, 2.5f, 0.18f), Pal.Marbre, 0f, false);
                BuildKit.Box("Chapiteau", g, new Vector3(s * 1.35f, 2.58f, -0.12f), new Vector3(0.3f, 0.16f, 0.24f), Pal.Marbre, 0f, false);
            }
            Auvent(g, new Vector3(0f, 2.75f, 0f), 2.6f, 0.7f, Pal.ToitTerreCuite);
        }

        /// <summary>Auvent de bois bleu à denticules et toit de tuiles (au-dessus des fenêtres et des chebbeks).</summary>
        public static void Auvent(Transform parent, Vector3 pos, float w, float d, Material toit)
        {
            var g = BuildKit.Group("Auvent", parent, pos);
            BuildKit.Box("Corniche", g, new Vector3(0f, 0.06f, -d * 0.5f), new Vector3(w, 0.12f, d), Pal.BleuBois, 0f, false);
            int n = Mathf.RoundToInt(w / 0.12f);
            for (int i = 0; i < n; i++)
                BuildKit.Box("Denticule", g, new Vector3(-w * 0.5f + (i + 0.5f) * w / n, -0.04f, -d + 0.03f), new Vector3(0.05f, 0.08f, 0.05f), Pal.BleuBois, 0f, false);
            BuildKit.BoxE("Toit", g, new Vector3(0f, 0.2f, -d * 0.45f), new Vector3(w - 0.05f, 0.05f, d * 0.95f), toit, new Vector3(-14f, 0f, 0f), false);
        }

        /// <summary>Fenêtre à persiennes bleues sous petit auvent de tuiles vertes (étage de la façade, photos 122, 126).</summary>
        public static void FenetreAuvent(Transform parent, Vector3 pos, float yaw, float w = 1.1f, float h = 1.6f)
        {
            PropsExterieur.FenetrePersiennes(parent, pos, yaw, w, h);
            var g = BuildKit.Group("Auvent_Fenetre", parent, pos, new Vector3(0f, yaw, 0f));
            Auvent(g, new Vector3(0f, h + 0.12f, -0.05f), w + 0.4f, 0.45f, Pal.TuilesEcailles);
        }

        /// <summary>Fenêtre du rez-de-chaussée : encadrement de marbre, grille carrée sombre devant la vitre (photos 122, 126).</summary>
        public static void FenetreGrille(Transform parent, Vector3 pos, float yaw, float w = 1.1f, float h = 1.7f)
        {
            var g = BuildKit.Group("Fenetre_Grille", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Encadrement", g, new Vector3(0f, h * 0.5f, -0.03f), new Vector3(w + 0.36f, h + 0.36f, 0.06f), Pal.Marbre, 0f, false);
            BuildKit.Box("Vitre", g, new Vector3(0f, h * 0.5f, -0.065f), new Vector3(w, h, 0.01f), Pal.VitreSombre, 0f, false);
            MeshKit.WBox("Grille", g, new Vector3(0f, h * 0.5f, -0.1f), new Vector3(w, h, 0.012f), Pal.GrilleCarree, 0f, false);
            BuildKit.Box("Appui", g, new Vector3(0f, -0.2f, -0.1f), new Vector3(w + 0.5f, 0.06f, 0.2f), Pal.Marbre, 0f, false);
        }

        /// <summary>Kiosque à tuiles vertes (photos 125, 127, 129, 130) : panneaux de faïence (texture photo), bandeau bleu à denticules.</summary>
        public static void Kiosque(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Kiosque_Faience", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Sol", g, new Vector3(0f, 0.12f, 0.75f), new Vector3(5.2f, 0.24f, 2.9f), Pal.Marbre);
            BuildKit.Box("Fond", g, new Vector3(0f, 1.7f, 2.3f), new Vector3(5.2f, 3.4f, 0.3f), Pal.Chaux);
            MeshKit.WBox("Faiences", g, new Vector3(-0.35f, 1.0f, 2.13f), new Vector3(3.6f, 1.35f, 0.03f), Pal.FaienceKiosque, 0f, false);
            BuildKit.Box("Porte_Bleue", g, new Vector3(1.85f, 1.3f, 2.13f), new Vector3(0.8f, 2.1f, 0.04f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Banquette", g, new Vector3(-0.35f, 0.42f, 1.9f), new Vector3(3.6f, 0.36f, 0.45f), Pal.Marbre, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Joue", g, new Vector3(s * 2.45f, 1.7f, 0.75f), new Vector3(0.3f, 3.4f, 2.9f), Pal.Chaux);
                BuildKit.Cyl("Colonne", g, new Vector3(s * 1.3f, 1.5f, -0.5f), 0.1f, 2.5f, Pal.Marbre, Vector3.zero, true);
                BuildKit.Box("Chapiteau", g, new Vector3(s * 1.3f, 2.8f, -0.5f), new Vector3(0.28f, 0.14f, 0.28f), Pal.Marbre, 0f, false);
            }
            BuildKit.Box("Bandeau_Bleu", g, new Vector3(0f, 3.0f, -0.62f), new Vector3(5.4f, 0.32f, 0.12f), Pal.BleuBois, 0f, false);
            int n = 44;
            for (int i = 0; i < n; i++)
                BuildKit.Box("Denticule", g, new Vector3(-2.65f + (i + 0.5f) * 5.3f / n, 3.2f, -0.68f), new Vector3(0.06f, 0.08f, 0.06f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Plafond", g, new Vector3(0f, 3.12f, 0.8f), new Vector3(5.4f, 0.12f, 3.0f), Pal.BleuBois, 0f, false);
            MeshKit.Pyramid("Toit_Tuiles_Vertes", g, new Vector3(0f, 3.18f, 0.8f), 5.8f, 3.4f, 1.3f, Pal.TuilesEcailles);
        }

        /// <summary>Escalier blanc extérieur adossé au mur, avec le vieux tronc tordu qui grimpe (photos 125, 127, 129). Monte vers +Z local.</summary>
        public static void EscalierTronc(Transform parent, Vector3 pos, float yaw, float hauteur, float longueur)
        {
            var g = BuildKit.Group("Escalier_Exterieur", parent, pos, new Vector3(0f, yaw, 0f));
            int n = Mathf.RoundToInt(hauteur / 0.17f);
            float rise = hauteur / n, tread = longueur / n;
            for (int i = 0; i < n; i++)
                BuildKit.Box("Marche", g, new Vector3(0f, (i + 1) * rise * 0.5f, (i + 0.5f) * tread), new Vector3(1.2f, (i + 1) * rise, tread), Pal.Chaux, 0f, false);
            float slope = Mathf.Atan2(hauteur, longueur) * Mathf.Rad2Deg;
            float diag = Mathf.Sqrt(hauteur * hauteur + longueur * longueur);
            BuildKit.BoxE("Garde_Corps", g, new Vector3(0.65f, hauteur * 0.5f + 0.5f, longueur * 0.5f), new Vector3(0.15f, 0.9f, diag), Pal.Chaux, new Vector3(-slope, 0f, 0f), false);
            BuildKit.Box("Palier", g, new Vector3(0f, hauteur * 0.5f, longueur + 0.7f), new Vector3(1.2f, hauteur, 1.4f), Pal.Chaux, 0f, false);

            // Tronc tordu (bougainvillier ancien) : segments qui serpentent le long du garde-corps
            var prev = new Vector3(1.0f, 0f, 0.4f);
            for (int i = 1; i <= 10; i++)
            {
                float t = i / 10f;
                var next = new Vector3(1.0f + Mathf.Sin(t * 9f) * 0.25f, hauteur * t + 0.9f * t, longueur * t * 0.9f + Mathf.Cos(t * 7f) * 0.3f);
                var d = next - prev;
                BuildKit.Cyl("Tronc_Tordu", g, (prev + next) * 0.5f, 0.11f - t * 0.05f, d.magnitude + 0.05f, Pal.BrancheNue, Quaternion.FromToRotation(Vector3.up, d.normalized).eulerAngles);
                prev = next;
            }
            Bougainvillier(g, new Vector3(0.8f, hauteur + 1.6f, longueur * 0.85f), 2.2f);
        }

        /// <summary>Urne (vase Médicis) en marbre sur piédestal (galerie et jardin persan).</summary>
        public static void Urne(Transform parent, Vector3 pos, bool fleurs)
        {
            var g = BuildKit.Group("Urne", parent, pos);
            BuildKit.Box("Piedestal", g, new Vector3(0f, 0.35f, 0f), new Vector3(0.36f, 0.7f, 0.36f), Pal.Marbre);
            BuildKit.Cyl("Pied", g, new Vector3(0f, 0.78f, 0f), 0.1f, 0.16f, Pal.Marbre);
            BuildKit.Cyl("Panse", g, new Vector3(0f, 0.98f, 0f), 0.2f, 0.26f, Pal.Marbre);
            BuildKit.Cyl("Levre", g, new Vector3(0f, 1.14f, 0f), 0.25f, 0.06f, Pal.Marbre);
            if (fleurs)
            {
                BuildKit.Sph("Plante", g, new Vector3(0f, 1.28f, 0f), new Vector3(0.45f, 0.3f, 0.45f), Pal.Geranium);
                for (int i = 0; i < 5; i++)
                    BuildKit.Sph("Fleur", g, new Vector3(BuildKit.R(-0.15f, 0.15f), 1.38f, BuildKit.R(-0.15f, 0.15f)), Vector3.one * 0.08f, Pal.Fleur(0));
            }
        }

        /// <summary>Buste sur gaine (jardin persan, photos 125, 129).</summary>
        public static void Buste(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Buste", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Gaine", g, new Vector3(0f, 0.6f, 0f), new Vector3(0.38f, 1.2f, 0.38f), Pal.Marbre);
            BuildKit.Box("Epaules", g, new Vector3(0f, 1.35f, 0f), new Vector3(0.42f, 0.28f, 0.22f), Pal.Bronze);
            BuildKit.Sph("Tete", g, new Vector3(0f, 1.62f, 0f), new Vector3(0.2f, 0.26f, 0.22f), Pal.Bronze);
        }

        // ================================================================ Végétation
        /// <summary>Bougainvillier : masse de bractées magenta sur un feuillage sombre.</summary>
        public static void Bougainvillier(Transform parent, Vector3 pos, float size)
        {
            var g = BuildKit.Group("Bougainvillier", parent, pos);
            Veg.Couronne(g, Vector3.zero, new Vector3(size, size * 0.6f, size * 0.55f), Mathf.RoundToInt(14 * size), 1.0f, Pal.CarteBougainvillier, Pal.CoeurSombre, 0.5f);
        }

        /// <summary>Haie taillée fleurie de lantanas orange (derrière la balustrade de l'esplanade, photo 138).</summary>
        public static void HaieLantana(Transform parent, Vector3 a, Vector3 b, float h)
        {
            Veg.Haie(parent, a, b, h, 1.2f, Pal.CarteLantana, Pal.CoeurSombre);
        }

        /// <summary>Touffe de graminée ou de yucca (jardinières devant la porte jaune, photo 128).</summary>
        public static void Graminee(Transform parent, Vector3 pos, float size)
        {
            var g = BuildKit.Group("Graminee", parent, pos);
            for (int i = 0; i < 14; i++)
            {
                var leaf = BuildKit.Group("Feuille", g, Vector3.zero, new Vector3(BuildKit.R(-35f, -5f), BuildKit.R(0f, 360f), 0f));
                BuildKit.Box("Limbe", leaf, new Vector3(0f, 0f, 0.35f * size), new Vector3(0.03f, 0.01f, 0.7f * size), Pal.Agave, 0f, false);
            }
        }

        /// <summary>Palmier des Canaries : stipe épais, couronne dense de palmes arquées (photos 09, 14, 121).</summary>
        public static void PalmierCanaries(Transform parent, Vector3 pos, float height)
        {
            var g = BuildKit.Group("Palmier_Canaries", parent, pos, new Vector3(0f, BuildKit.R(0f, 360f), 0f));
            BuildKit.Cyl("Stipe", g, new Vector3(0f, height * 0.5f, 0f), 0.38f, height, Pal.TroncPalmier, Vector3.zero, true);
            BuildKit.Sph("Bourgeon", g, new Vector3(0f, height + 0.15f, 0f), new Vector3(1.0f, 0.8f, 1.0f), Pal.TroncPalmier);
            MeshKit.Fronds("Palmes", g, new Vector3(0f, height + 0.25f, 0f), 30, 4.6f, 1.3f, Pal.PalmeCanaries);
            MeshKit.Fronds("Palmes_Hautes", g, new Vector3(0f, height + 0.4f, 0f), 12, 3.2f, 1.0f, Pal.PalmeCanaries);
        }

        /// <summary>Grand conifère étalé (pin ou cyprès de Lambert, photos 124, 126) : tronc nu, plateaux de feuillage sombre.</summary>
        public static void GrandConifere(Transform parent, Vector3 pos, float height)
        {
            var g = BuildKit.Group("Grand_Conifere", parent, pos);
            BuildKit.Cyl("Tronc", g, new Vector3(0f, height * 0.35f, 0f), 0.4f, height * 0.7f, Pal.TroncPin, Vector3.zero, true);
            for (int i = 0; i < 7; i++)
            {
                float y = height * BuildKit.R(0.4f, 0.95f);
                float s = (height - y) * 0.45f + BuildKit.R(1.6f, 2.8f);
                Veg.Couronne(g, new Vector3(BuildKit.R(-1.5f, 1.5f), y, BuildKit.R(-1.5f, 1.5f)), new Vector3(s, s * 0.4f, s), Mathf.RoundToInt(s * 6f), 1.6f, Pal.CartePin, Pal.CoeurSombre, 0.32f);
            }
        }

        // ================================================================ Maisons de Sidi Bou Saïd (vues du palais)
        /// <summary>
        /// Maison ou petit immeuble blanc à toit plat (photos 03, 149, 121) : volumes empilés en retrait, terrasses
        /// à garde-corps bleus, fenêtres bleues, parfois une petite coupole ou un bougainvillier. Face vers −Z local.
        /// </summary>
        public static void Maison(Transform parent, Vector3 pos, float yaw, int etagesMax = 3)
        {
            var g = BuildKit.Group("Maison_SBS", parent, pos, new Vector3(0f, yaw, 0f));
            float w = BuildKit.R(6f, 12f), d = BuildKit.R(6f, 10f);
            int etages = BuildKit.RI(1, etagesMax + 1);
            float y = 0f;
            for (int e = 0; e < etages; e++)
            {
                float h = 3.1f;
                BuildKit.Box("Niveau", g, new Vector3(0f, y + h * 0.5f, 0f), new Vector3(w, h, d), Pal.Chaux, 0f, false);
                int nw = Mathf.Max(1, Mathf.RoundToInt(w / 2.6f));
                for (int k = 0; k < nw; k++)
                {
                    float x = -w * 0.5f + (k + 0.5f) * w / nw;
                    bool arc = BuildKit.Chance(0.25f);
                    BuildKit.Box("Fenetre", g, new Vector3(x, y + 1.5f, -d * 0.5f - 0.03f), new Vector3(0.9f, arc ? 1.3f : 1.2f, 0.06f), Pal.BleuBois, 0f, false);
                    if (arc) BuildKit.Cyl("Arc", g, new Vector3(x, y + 2.15f, -d * 0.5f - 0.03f), 0.45f, 0.06f, Pal.BleuBois, new Vector3(90f, 0f, 0f));
                }
                y += h;
                // Terrasse sur le niveau suivant, en retrait : garde-corps bleu
                float nd = d * BuildKit.R(0.55f, 0.85f);
                BuildKit.Box("Acrotere", g, new Vector3(0f, y + 0.45f, -d * 0.5f + 0.06f), new Vector3(w, 0.9f, 0.12f), BuildKit.Chance(0.5f) ? Pal.Chaux : Pal.BleuFer, 0f, false);
                if (e < etages - 1)
                {
                    g = BuildKit.Group("Retrait", g, new Vector3(BuildKit.R(-1f, 1f), 0f, (d - nd) * 0.5f));
                    w *= BuildKit.R(0.7f, 0.95f);
                    d = nd;
                }
            }
            if (BuildKit.Chance(0.2f))
                BuildKit.Sph("Coupole", g, new Vector3(BuildKit.R(-w * 0.2f, w * 0.2f), y, BuildKit.R(-d * 0.1f, d * 0.2f)), new Vector3(2.4f, 1.8f, 2.4f), Pal.Chaux);
            if (BuildKit.Chance(0.3f))
                Bougainvillier(g, new Vector3(BuildKit.R(-w * 0.4f, w * 0.4f), y - 0.5f, -d * 0.5f - 0.3f), 1.0f);
        }
    }
}
