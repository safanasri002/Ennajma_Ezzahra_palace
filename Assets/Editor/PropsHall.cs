using UnityEditor;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// Matériaux tirés des photos du palais (Assets/Art/Textures/Photos, générés par Tools/make_photo_textures.py)
    /// et matériaux procéduraux de la grande nef. Les textures « photo » s'appliquent :
    ///  - en motif répété sur les boîtes MeshKit.WBox (UV en mètres, tiling = 1 / taille du motif) ;
    ///  - en image entière sur les primitives BuildKit.Box (UV 0→1 par face, tiling = 1).
    /// </summary>
    public static partial class Pal
    {
        const string PhotoDir = "Assets/Art/Textures/Photos/";
        static Texture2D Photo(string n) => AssetDatabase.LoadAssetAtPath<Texture2D>(PhotoDir + n + ".jpg");

        // Stuc ciselé (treillis) : motif de 0,6 m — murs, tympans des arcs
        public static Material StucTreillis => BuildKit.TexMat("Photo_Stuc_Treillis", () => Photo("Photo_Stuc_Treillis"), Vector2.one / 0.6f, 0.15f, Color.white);
        // Onyx rouge (lambris des bas-côtés et de l'escalier) : motif de 1,2 m
        public static Material OnyxRouge => BuildKit.TexMat("Photo_Onyx_Rouge", () => Photo("Photo_Onyx_Rouge"), Vector2.one / 1.2f, 0.8f, Color.white);
        public static Material MarbreNoir => BuildKit.TexMat("Photo_Marbre_Noir", () => Photo("Photo_Marbre_Noir"), Vector2.one / 0.8f, 0.85f, Color.white);
        // Panneaux de marbre blanc veiné de gris (procédural, motif de 1,6 m)
        public static Material MarbreBlancPanneau => BuildKit.TexMat("Marbre_Blanc_Panneau", () => Tex.MarbreGris, Vector2.one / 1.6f, 0.75f, new Color(1f, 1f, 1f));
        // Images entières (primitives BuildKit.Box)
        public static Material RosaceStuc => BuildKit.TexMat("Photo_Rosace_Stuc", () => Photo("Photo_Rosace_Stuc"), Vector2.one, 0.15f, Color.white);
        public static Material MoucharabiehRouge => BuildKit.TexMat("Photo_Moucharabieh_Rouge", () => Photo("Photo_Moucharabieh_Rouge"), Vector2.one, 0.35f, Color.white, 0.25f);
        public static Material PlafondDore(Vector2 tiling) => BuildKit.TexMat($"Photo_Plafond_Dore_{tiling.x:0.0}x{tiling.y:0.0}", () => Photo("Photo_Plafond_Dore"), tiling, 0.55f, Color.white, 0.12f, 0.3f);
        public static Material FriseDoree(float length, float height) =>
            BuildKit.TexMat($"Photo_Frise_Doree_{length:0.0}", () => Photo("Photo_Frise_Doree"), new Vector2(Mathf.Max(1f, Mathf.Round(length / (height * 8f))), 1f), 0.5f, Color.white, 0f, 0.3f);
        public static Material FriseCalligraphie(float length, float height) =>
            BuildKit.TexMat($"Photo_Frise_Calligraphie_{length:0.0}", () => Photo("Photo_Frise_Stuc_Calligraphie"), new Vector2(Mathf.Max(1f, Mathf.Round(length / (height * 16f))), 1f), 0.15f, Color.white);

        // Procéduraux
        public static Material SolDamier => BuildKit.TexMat("Sol_Damier_Rose_Blanc", () => TexHall.Damier, Vector2.one, 0.7f, Color.white);
        public static Material Lattes => BuildKit.TexMat("Plafond_Lattes", () => TexHall.Lattes, Vector2.one, 0.3f, Color.white);
        public static Material BoisRouge => BuildKit.Mat("Bois_Rouge_Sculpte", new Color(0.55f, 0.17f, 0.07f), 0.35f);
        public static Material BoisTourne => BuildKit.Mat("Bois_Tourne_Brun", new Color(0.3f, 0.14f, 0.07f), 0.4f);
        public static Material Bronze => BuildKit.Mat("Bronze", new Color(0.33f, 0.24f, 0.12f), 0.65f, 0.8f);
        public static Material MarbreVert => BuildKit.Mat("Marbre_Vert_Sombre", new Color(0.07f, 0.15f, 0.12f), 0.8f);
        public static Material VertDeGris => BuildKit.Mat("Bronze_Vert_de_Gris", new Color(0.36f, 0.45f, 0.43f), 0.35f, 0.4f);
        public static Material VeloursRouge => BuildKit.Mat("Velours_Rouge", new Color(0.62f, 0.13f, 0.1f), 0.12f);
        public static Material MarbreColonne => BuildKit.Mat("Marbre_Colonne_Rose", new Color(0.82f, 0.68f, 0.64f), 0.75f);
        public static Material EauSombre => BuildKit.Mat("Eau_Rigole", new Color(0.12f, 0.2f, 0.2f), 0.97f);
        public static Material VitreHaute => BuildKit.Emissive("Vitre_Haute", new Color(1f, 0.92f, 0.7f), new Color(1f, 0.85f, 0.55f) * 1.6f);
        public static Material Flamme => BuildKit.Emissive("Flamme_Vasque", new Color(1f, 0.8f, 0.5f), new Color(1f, 0.6f, 0.25f) * 3f);
    }

    public static class TexHall
    {
        public static Texture2D Damier => BuildKit.ProcTexture("Tex_Sol_Damier_v1", 256, 256, Damier1m);
        public static Texture2D Lattes => BuildKit.ProcTexture("Tex_Plafond_Lattes_v1", 256, 256, Lattes1m);

        // Damier de losanges (2 par mètre), rose et blanc, joints gris : un motif = 1 m
        static Color Damier1m(float u, float v)
        {
            float a = (u + v) * 2f, b = (u - v) * 2f + 8f;
            float fa = a - Mathf.Floor(a), fb = b - Mathf.Floor(b);
            if (fa < 0.025f || fa > 0.975f || fb < 0.025f || fb > 0.975f) return new Color(0.55f, 0.5f, 0.48f);
            bool rose = ((int)Mathf.Floor(a) + (int)Mathf.Floor(b)) % 2 == 0;
            float w = 0.97f + 0.03f * Mathf.Sin(6.2832f * (3f * u + 2f * v) + 1.7f);
            return (rose ? new Color(0.8f, 0.6f, 0.56f) : new Color(0.94f, 0.92f, 0.89f)) * w;
        }

        // Plafond à lattes de bois (8 lattes par mètre)
        static Color Lattes1m(float u, float v)
        {
            float f = u * 8f - Mathf.Floor(u * 8f);
            float g = 0.9f + 0.1f * Mathf.Sin(6.2832f * (v * 3f + u * 8f));
            return f < 0.72f ? new Color(0.5f, 0.27f, 0.12f) * g : new Color(0.16f, 0.08f, 0.04f);
        }
    }

    /// <summary>Accessoires de la grande nef (hall) : balustrades, vasque, lanternes, porte cloutée, niche de marbre noir.</summary>
    public static class PropsHall
    {
        /// <summary>Balustrade de bois tourné entre a et b (au sol, y local), hauteur 1 m.</summary>
        public static void Balustrade(Transform parent, Vector3 a, Vector3 b, float height = 1f)
        {
            var g = BuildKit.Group("Balustrade", parent, a);
            Vector3 d = b - a;
            float len = d.magnitude;
            float yaw = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
            g.localEulerAngles = new Vector3(0f, yaw, 0f);
            BuildKit.Box("Main_Courante", g, new Vector3(0f, height, len * 0.5f), new Vector3(0.1f, 0.07f, len), Pal.BoisTourne, 0f, false);
            BuildKit.Box("Lisse_Basse", g, new Vector3(0f, 0.06f, len * 0.5f), new Vector3(0.08f, 0.08f, len), Pal.BoisTourne, 0f, false);
            BuildKit.Box("Lisse_Mediane", g, new Vector3(0f, height * 0.62f, len * 0.5f), new Vector3(0.06f, 0.05f, len), Pal.BoisTourne, 0f, false);
            int n = Mathf.Max(2, Mathf.RoundToInt(len / 0.11f));
            for (int i = 0; i <= n; i++)
            {
                float z = len * i / n;
                BuildKit.Cyl("Balustre", g, new Vector3(0f, height * 0.33f, z), 0.022f, height * 0.54f, Pal.BoisTourne);
                BuildKit.Cyl("Balustre_Haut", g, new Vector3(0f, height * 0.81f, z), 0.018f, height * 0.36f, Pal.BoisTourne);
            }
        }

        /// <summary>Paire de colonnes de marbre sous une imposte de stuc (pos = au sol, entre les deux fûts).</summary>
        public static void ColonnesJumelees(Transform parent, Vector3 pos, float yaw, float height, float radius, Material shaft, float gap = 0.36f)
        {
            var g = BuildKit.Group("Colonnes_Jumelees", parent, pos, new Vector3(0f, yaw, 0f));
            for (int s = -1; s <= 1; s += 2)
            {
                float x = s * gap * 0.5f;
                BuildKit.Cyl("Base", g, new Vector3(x, 0.08f, 0f), radius * 1.35f, 0.16f, Pal.Marbre);
                BuildKit.Cyl("Fut", g, new Vector3(x, (height - 0.3f) * 0.5f + 0.16f, 0f), radius, height - 0.46f, shaft, default, true);
                BuildKit.Cyl("Astragale", g, new Vector3(x, height - 0.32f, 0f), radius * 1.15f, 0.05f, Pal.Marbre);
                BuildKit.Box("Chapiteau", g, new Vector3(x, height - 0.2f, 0f), new Vector3(radius * 2.6f, 0.2f, radius * 2.6f), Pal.Stuc, 0f, false);
            }
            BuildKit.Box("Imposte", g, new Vector3(0f, height - 0.05f, 0f), new Vector3(gap + radius * 3.2f, 0.12f, radius * 3f), Pal.Stuc, 0f, false);
        }

        /// <summary>Vasque de marbre avec sa veilleuse, au bout de la rigole, dans un bassin carré affleurant.</summary>
        public static void Vasque(Transform parent, Vector3 pos)
        {
            var g = BuildKit.Group("Vasque", parent, pos);
            const float s = 1.5f;
            for (int k = 0; k < 4; k++)
            {
                float a = k * 90f * Mathf.Deg2Rad;
                var dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
                BuildKit.Box("Margelle", g, dir * (s * 0.5f - 0.08f) + Vector3.up * 0.01f, new Vector3(Mathf.Abs(dir.x) > 0.5f ? 0.16f : s, 0.02f, Mathf.Abs(dir.z) > 0.5f ? 0.16f : s), Pal.MarbreRose, 0f, false);
            }
            BuildKit.Box("Bassin", g, new Vector3(0f, 0.004f, 0f), new Vector3(s - 0.3f, 0.008f, s - 0.3f), Pal.EauSombre, 0f, false);
            BuildKit.Cyl("Pied", g, new Vector3(0f, 0.28f, 0f), 0.11f, 0.56f, Pal.Marbre);
            BuildKit.Cyl("Coupe", g, new Vector3(0f, 0.62f, 0f), 0.42f, 0.16f, Pal.MarbreColonne);
            BuildKit.Cyl("Coupe_Fond", g, new Vector3(0f, 0.55f, 0f), 0.25f, 0.06f, Pal.MarbreColonne);
            BuildKit.Cyl("Veilleuse", g, new Vector3(0f, 0.79f, 0f), 0.08f, 0.18f, Pal.Flamme);
            BuildKit.Cyl("Veilleuse_Cerclage", g, new Vector3(0f, 0.89f, 0f), 0.09f, 0.03f, Pal.Laiton);
            BuildKit.PointLight("Lumiere_Vasque", g, new Vector3(0f, 1.1f, 0f), new Color(1f, 0.72f, 0.42f), 1.6f, 6f);
            BuildKit.InvisibleWall("Collider", g, new Vector3(0f, 0.5f, 0f), new Vector3(0.95f, 1f, 0.95f));
        }

        /// <summary>Lanterne suspendue (cloche de verre ou lanterne de bronze), avec sa lumière.</summary>
        public static void LanterneSuspendue(Transform parent, Vector3 attach, float drop, float size, float intensity, float range, bool shadows = false)
        {
            var g = BuildKit.Group("Lanterne_Suspendue", parent, attach);
            BuildKit.Cyl("Chaine", g, new Vector3(0f, -drop * 0.5f, 0f), 0.012f, drop, Pal.Bronze);
            BuildKit.Cyl("Couronne", g, new Vector3(0f, -drop, 0f), size * 0.45f, 0.05f, Pal.Bronze);
            BuildKit.Cyl("Cloche", g, new Vector3(0f, -drop - size * 0.4f, 0f), size * 0.38f, size * 0.75f, Pal.LanterneVerre);
            BuildKit.Cyl("Fond", g, new Vector3(0f, -drop - size * 0.8f, 0f), size * 0.3f, 0.04f, Pal.Bronze);
            BuildKit.Sph("Goutte", g, new Vector3(0f, -drop - size * 0.9f, 0f), Vector3.one * size * 0.15f, Pal.Bronze);
            BuildKit.PointLight("Lumiere_Lanterne", g, new Vector3(0f, -drop - size * 1.1f, 0f), new Color(1f, 0.82f, 0.58f), intensity, range, shadows);
        }

        /// <summary>Porte d'entrée à deux vantaux en bronze vert-de-gris à gros clous (vue de l'intérieur).</summary>
        public static void PorteCloutee(Transform parent, Vector3 pos, float yaw, float w, float h)
        {
            var g = BuildKit.Group("Porte_Cloutee", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Encadrement", g, new Vector3(0f, (h + 0.3f) * 0.5f, -0.02f), new Vector3(w + 0.5f, h + 0.3f, 0.04f), Pal.Marbre, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                float cx = s * w * 0.25f;
                BuildKit.Box("Vantail", g, new Vector3(cx, h * 0.5f, -0.07f), new Vector3(w * 0.5f - 0.02f, h, 0.08f), Pal.VertDeGris, 0f, false);
                BuildKit.Studs(g, new Vector3(cx, h * 0.5f, 0f), w * 0.4f, h * 0.9f, 3, 11, -0.12f, Pal.Fer, 0.07f);
                BuildKit.Cyl("Heurtoir", g, new Vector3(cx - s * w * 0.12f, h * 0.55f, -0.13f), 0.08f, 0.02f, Pal.Fer, new Vector3(90f, 0f, 0f));
            }
        }

        /// <summary>Niche de marbre noir à arc (cheminée décorative), contre un mur, face vers -Z local.</summary>
        public static void NicheMarbreNoir(Transform parent, Vector3 pos, float yaw)
        {
            var g = BuildKit.Group("Niche_Marbre_Noir", parent, pos, new Vector3(0f, yaw, 0f));
            MeshKit.WBox("Cadre", g, new Vector3(0f, 1.45f, -0.06f), new Vector3(1.9f, 2.9f, 0.12f), Pal.MarbreNoir, 0f, false);
            BuildKit.Box("Bordure_Doree", g, new Vector3(0f, 1.45f, -0.05f), new Vector3(2.0f, 3.0f, 0.1f), Pal.BoisDore, 0f, false);
            BuildKit.Box("Fond", g, new Vector3(0f, 0.95f, -0.125f), new Vector3(1.1f, 1.9f, 0.01f), Pal.Noir, 0f, false);
            BuildKit.Cyl("Fond_Arc", g, new Vector3(0f, 1.9f, -0.125f), 0.55f, 0.01f, Pal.Noir, new Vector3(90f, 0f, 0f));
            BuildKit.Arch("Arc_Stuc", g, new Vector3(0f, 0f, -0.13f), 0f, 1.1f, 1.9f, 0.04f, 0.1f, Pal.Stuc, 0f, 14, false);
            BuildKit.Box("Seuil", g, new Vector3(0f, 0.08f, -0.3f), new Vector3(1.6f, 0.16f, 0.5f), Pal.MarbreNoir, 0f, false);
        }

        /// <summary>Petite fenêtre haute en arc (claire-voie), lumineuse. pos = bas-centre sur le mur, face vers -Z local.</summary>
        public static void FenetreHaute(Transform parent, Vector3 pos, float yaw, float w, float h)
        {
            var g = BuildKit.Group("Fenetre_Haute", parent, pos, new Vector3(0f, yaw, 0f));
            BuildKit.Box("Vitre", g, new Vector3(0f, (h - w * 0.5f) * 0.5f, -0.02f), new Vector3(w, h - w * 0.5f, 0.02f), Pal.VitreHaute, 0f, false);
            BuildKit.Cyl("Vitre_Arc", g, new Vector3(0f, h - w * 0.5f, -0.02f), w * 0.5f, 0.02f, Pal.VitreHaute, new Vector3(90f, 0f, 0f));
            BuildKit.Arch("Cadre", g, new Vector3(0f, 0f, -0.04f), 0f, w, h - w * 0.5f, 0.04f, 0.07f, Pal.Stuc, 0f, 10, true);
        }
    }
}
