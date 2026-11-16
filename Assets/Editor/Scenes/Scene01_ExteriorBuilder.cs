using UnityEditor;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// SCÈNE 1 — L'EXTÉRIEUR DU PALAIS ENNEJMA EZZAHRA (calée sur les photos de la visite et les photos complémentaires)
    ///
    ///   Rue → Porte jaune (z = 0), pavillon d'entrée à plaques → Skifa voûtée en brique (z 0,4 → 5,4)
    ///   → Allée de béton rose (z 5,4 → 55) : eucalyptus en têtards à droite, haie taillée, agaves, palmiers à gauche
    ///   → à gauche : jardin andalou, bassin de 2,5 × 45 m entre trois rangées de cyprès sur terrasses de pierre sèche,
    ///     pavillon à porte jaune côté allée, cascade carrelée et village au fond (axe z = 28)
    ///   → Esplanade de béton rose à balustrade bleue, haie de lantanas, face à la galerie à 11 arcades ;
    ///     l'étage du palais est en retrait derrière la terrasse de la galerie (photos 122, 126)
    ///   → Jardin persan au sud : carrés à grilles à volutes, vasque, porte de fer bleue sous auvent en treillis,
    ///     kiosque à faïences, escalier blanc au vieux tronc tordu
    ///   → Porte principale au fond de la galerie (3 marches) : transition vers la scène 2 (Hall d'entrée)
    ///
    /// Repère : +Z = nord, +X = est (la mer). Lointain aux directions réelles (REALITE_HYPOTHESES.md) :
    /// Bou Kornine plein sud, cap Bon à l'est–sud-est, port et plage au sud en contrebas, village au-dessus à l'ouest.
    /// Distances lointaines réduites (≈ 2,1 km) en gardant la taille apparente.
    /// Soleil : fin d'après-midi d'hiver (≈ 15 h, azimut 220°, hauteur 24°), d'après les ombres des photos.
    /// </summary>
    public static class Scene01_ExteriorBuilder
    {
        public const string SceneName = "01_Exterieur";
        public const string NextScene = "02_Hall_Entree";

        const float SlopeDeg = 12f;
        const float SeaY = -36f;
        static float Tan => Mathf.Tan(SlopeDeg * Mathf.Deg2Rad);
        // Relief continu : la colline descend vers la mer à l'est et au sud (côte orientée NE–SO sous le palais).
        static float Sp(float v) => v > 30f ? v : 4f * Mathf.Log(1f + Mathf.Exp(v / 4f));   // rampe adoucie (4 m)
        static float H(float x, float z) => Mathf.Max(SeaY - 25f, -0.5f - Tan * (Sp(x - 8f) + Sp(-12f - z)));
        /// <summary>z de la ligne de côte (H = niveau de la mer) pour une abscisse x.</summary>
        static float CoteZ(float x) => -12f - ((-0.5f - SeaY) / Tan - Sp(x - 8f));

        // Palais : arcades à x = −36, fond de galerie à x = −39, étage en retrait à x = −39,5
        const float FacadeX = -36f, GalerieX = -39f, EtageX = -39.5f, DoorZ = 78f;
        const float HautRdc = 5.2f, HautTotale = 10.5f;
        // Jardin andalou
        const float BassinZ = 28f, BassinX0 = -52.5f, BassinX1 = -7.5f;

        [MenuItem("Ennejma/Phase 1/Scène 1 - Extérieur (porte jaune → entrée du palais)", priority = 1)]
        public static void Build()
        {
            var scene = BuildKit.NewScene();
            if (!scene.IsValid()) return;
            BuildKit.BeginBuild(1912);

            // Soleil d'hiver vers 15 h : vient du sud-ouest (azimut 220°) → la lumière se dirige vers 40°.
            var sun = BuildKit.SetupDaylight(new Vector3(24f, 40f, 0f), 1.45f);
            sun.color = new Color(1f, 0.87f, 0.72f);
            // Perspective aérienne : horizon laiteux, lointains désaturés (photos 03, 21, 26, 30, 149)
            RenderSettings.fogDensity = 0.00042f;
            RenderSettings.fogColor = new Color(0.8f, 0.83f, 0.86f);
            // Ciel pâle et laiteux des photos (et non le bleu saturé par défaut)
            var sky = RenderSettings.skybox;
            if (sky != null)
            {
                if (sky.HasProperty("_SkyTint")) sky.SetColor("_SkyTint", new Color(0.45f, 0.55f, 0.72f));
                if (sky.HasProperty("_AtmosphereThickness")) sky.SetFloat("_AtmosphereThickness", 1.15f);
                if (sky.HasProperty("_Exposure")) sky.SetFloat("_Exposure", 1.15f);
                EditorUtility.SetDirty(sky);
                DynamicGI.UpdateEnvironment();
            }

            var root = new GameObject("=== EXTERIEUR ===").transform;
            BuildGround(root);
            BuildYellowGate(root);
            BuildSkifa(root);
            BuildAlley(root);
            BuildJardinAndalou(root);
            BuildGardenRest(root);
            BuildLointain(root);
            BuildPlaza(root);
            BuildPalace(root);
            BuildJardinPersan(root);

            var gameplay = new GameObject("=== GAMEPLAY ===").transform;
            BuildKit.Spawn(gameplay, new Vector3(0f, 0.1f, -7f), 0f, "Default");
            BuildKit.Spawn(gameplay, new Vector3(-33f, 0.2f, DoorZ), 90f, "FromInterior");
            BuildKit.CreatePlayer(new Vector3(0f, 0.1f, -7f), 0f);

            BuildKit.SaveScene(scene, SceneName);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Ennejma Ezzahra",
                "Scène 1 construite !\n\nAppuyez sur Play : marchez avec ZQSD (ou flèches), regardez avec la souris, Maj pour courir, Échap pour libérer la souris.",
                "OK");
        }

        /// <summary>Mode batch : construit la scène puis enregistre les captures de contrôle (Screenshots/).</summary>
        public static void BuildAndCapture()
        {
            Build();
            CaptureTool.CaptureScene01();
        }

        // =====================================================================  SOL, PENTES, MER
        static void BuildGround(Transform root)
        {
            var g = BuildKit.Group("Sol_et_Paysage", root);

            MeshKit.WBox("Sol_Domaine", g, new Vector3(-27f, -0.5f, 44f), new Vector3(70f, 1f, 112f), Pal.SolJardin);       // x −62..8, z −12..100
            MeshKit.WBox("Rue_Dalles", g, new Vector3(-6f, 0.025f, -6f), new Vector3(28f, 0.05f, 12f), Pal.DallesRue);
            MeshKit.WBox("Esplanade", g, new Vector3(-16f, 0.025f, 75.75f), new Vector3(40f, 0.05f, 41.5f), Pal.BetonRose);

            // Colline qui descend vers la mer à l'est et au sud, d'un seul tenant (photos 03, 11, 12, 149)
            MeshKit.Terrain("Colline_Vers_La_Mer", g, new Vector2(-600f, -700f), new Vector2(700f, 600f), 130, H, Pal.SolMaquis);
            // Le village est au-dessus du palais, à l'ouest et au nord-ouest
            BuildKit.Box("Plateau_Village_Ouest", g, new Vector3(-562f, 1.5f, 494f), new Vector3(1000f, 4f, 1012f), Pal.Colline, 0f, false);

            BuildKit.Box("Golfe_de_Tunis", g, new Vector3(1500f, SeaY - 0.5f, -1500f), new Vector3(5000f, 1f, 5000f), Pal.Mer, 0f, false);

            BuildKit.InvisibleWall("Limite_Est", g, new Vector3(8.3f, 1.5f, 44f), new Vector3(0.4f, 4f, 112f));
            BuildKit.InvisibleWall("Limite_Nord", g, new Vector3(-27f, 1.5f, 100.2f), new Vector3(70f, 4f, 0.4f));
            BuildKit.InvisibleWall("Limite_Ouest", g, new Vector3(-62.2f, 1.5f, 44f), new Vector3(0.4f, 4f, 112f));
            BuildKit.InvisibleWall("Limite_Rue_Sud", g, new Vector3(-6f, 1.5f, -12.2f), new Vector3(28f, 4f, 0.4f));
            BuildKit.InvisibleWall("Limite_Rue_Ouest", g, new Vector3(-20.2f, 1.5f, -6f), new Vector3(0.4f, 4f, 12f));
            BuildKit.InvisibleWall("Limite_Rue_Est", g, new Vector3(8.2f, 1.5f, -6f), new Vector3(0.4f, 4f, 12f));
        }

        // =====================================================================  LA PORTE JAUNE (photos 02, 128, 148)
        static void BuildYellowGate(Transform root)
        {
            var g = BuildKit.Group("Porte_Jaune", root);

            BuildKit.Wall("Enceinte_Sud_Ouest", g, new Vector3(-62f, 0f, 0f), new Vector3(-3f, 0f, 0f), 3.2f, 0.6f, Pal.Chaux);
            BuildKit.Wall("Enceinte_Sud_Est", g, new Vector3(3f, 0f, 0f), new Vector3(8f, 0f, 0f), 3.2f, 0.6f, Pal.Chaux);

            // Pavillon d'entrée : cube blanc de 6 m × 5,6 m, arc outrepassé (passage de 2,2 m) dans un cadre de pierre sculptée
            const float open = 2.2f, spring = 2.35f, horseshoe = 15f;
            BuildKit.WallWithArch("Portail", g, Vector3.zero, 0f, 6f, 5.6f, 0.8f, open, spring, Pal.Chaux, Pal.Chaux, horseshoe, 0.25f);
            BuildKit.Box("Bandeau_Sculpte", g, new Vector3(0f, 4.0f, -0.43f), new Vector3(3.4f, 0.9f, 0.05f), Pal.PortailBandeau, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Montant_Sculpte", g, new Vector3(s * 1.4f, 1.0f, -0.43f), new Vector3(0.5f, 1.8f, 0.05f), Pal.PortailMontant, 0f, false);
                BuildKit.Box("Montant_Haut", g, new Vector3(s * 1.4f, 2.75f, -0.43f), new Vector3(0.5f, 1.7f, 0.05f), Pal.PortailMontant, 0f, false);
                BuildKit.Box("Cadre_Pierre", g, new Vector3(s * 1.76f, 2.25f, -0.44f), new Vector3(0.12f, 4.5f, 0.07f), Pal.Pierre, 0f, false);
            }
            BuildKit.Box("Cadre_Pierre_Haut", g, new Vector3(0f, 4.5f, -0.44f), new Vector3(3.64f, 0.12f, 0.07f), Pal.Pierre, 0f, false);
            BuildKit.Box("Seuil", g, new Vector3(0f, 0.04f, 0f), new Vector3(open, 0.08f, 0.8f), Pal.Marbre, 0f, false);

            // Plaques à gauche du cadre (photo 128) : trois plaques de marbre et un panneau bleu
            var pl = BuildKit.Group("Plaques", g, new Vector3(-2.45f, 0f, -0.42f));
            BuildKit.Box("Plaque_CMAM", pl, new Vector3(0f, 3.0f, 0f), new Vector3(0.7f, 0.6f, 0.03f), Pal.Marbre, 0f, false);
            BuildKit.Text3D("Texte_CMAM", pl, new Vector3(0f, 2.92f, -0.02f), 0f, "CENTRE DES MUSIQUES\nARABES ET\nMÉDITERRANÉENNES", 0.0055f, new Color(0.25f, 0.25f, 0.25f));
            BuildKit.Box("Plaque_Baron", pl, new Vector3(0f, 2.15f, 0f), new Vector3(0.7f, 0.95f, 0.03f), Pal.Marbre, 0f, false);
            BuildKit.Text3D("Texte_Baron", pl, new Vector3(0f, 2.0f, -0.02f), 0f, "MAISON DU BARON\nR. D'ERLANGER\n\nMONUMENT\nHISTORIQUE CLASSÉ", 0.0055f, new Color(0.25f, 0.25f, 0.25f));
            BuildKit.Box("Panneau_Bleu", pl, new Vector3(0f, 1.25f, 0f), new Vector3(0.7f, 0.65f, 0.03f), Pal.BleuBois, 0f, false);
            // Applique sur potence en haut à gauche, jardinières basses à graminées
            BuildKit.Box("Potence", g, new Vector3(-2.75f, 4.6f, -0.6f), new Vector3(0.04f, 0.04f, 0.4f), Pal.Fer, 0f, false);
            BuildKit.Box("Applique", g, new Vector3(-2.75f, 4.45f, -0.8f), new Vector3(0.22f, 0.32f, 0.22f), Pal.LanterneVerre, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Jardiniere", g, new Vector3(s * 4.8f, 0.3f, -1.0f), new Vector3(3.4f, 0.6f, 1.2f), Pal.Chaux);
                for (int k = 0; k < 3; k++)
                    PropsDetails.Graminee(g, new Vector3(s * (3.6f + k * 1.1f), 0.6f, -1.0f), BuildKit.R(0.8f, 1.1f));
            }

            // Battants jaunes cloutés (texture tirée de la photo), ouverts vers la skifa
            const float leaf = 1.1f, leafH = 3.1f;
            for (int s = -1; s <= 1; s += 2)
            {
                var hinge = BuildKit.Group(s < 0 ? "Battant_Gauche" : "Battant_Droit", g,
                                           new Vector3(s * leaf, 0.05f, 0.42f), new Vector3(0f, s * 80f, 0f));
                BuildKit.Box("Vantail", hinge, new Vector3(-s * leaf * 0.5f, leafH * 0.5f, 0.05f), new Vector3(leaf, leafH, 0.08f), Pal.PorteJaunePhoto);
                BuildKit.Cyl("Heurtoir_Anneau", hinge, new Vector3(-s * (leaf - 0.2f), 1.75f, -0.02f), 0.07f, 0.02f, Pal.Fer, new Vector3(90f, 0f, 0f));
            }
            PropsDetails.Bougainvillier(g, new Vector3(-7f, 3.1f, -0.5f), 1.2f);

            BuildKit.Info(g, new Vector3(0f, 1.5f, -2.5f), new Vector3(6f, 3f, 3f), "La porte jaune",
                "Bienvenue au palais Ennejma Ezzahra, bâti entre 1912 et 1922 par le baron Rodolphe d'Erlanger, peintre et musicologue passionné de musique arabe. Il abrite aujourd'hui le Centre des musiques arabes et méditerranéennes.\nFranchissez la porte cloutée et la skifa voûtée.");
        }

        // =====================================================================  LA SKIFA (passage voûté en brique, photos 02, 128)
        static void BuildSkifa(Transform root)
        {
            var g = BuildKit.Group("Skifa", root);
            MeshKit.ArcadeWall("Voute", g, new Vector3(-1.8f, 0f, 2.9f), 0f, 3.6f, 4.2f, 5f, 1, 2.4f, 2.3f, 0f, Pal.Brique, null);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Mur_Chaux", g, new Vector3(s * 1.185f, 1.15f, 2.9f), new Vector3(0.03f, 2.3f, 5f), Pal.Chaux, 0f, false);
                BuildKit.Box("Pavillon_Cote", g, new Vector3(s * 2.4f, 2.8f, 2.9f), new Vector3(1.2f, 5.6f, 5f), Pal.Chaux);
            }
            BuildKit.Box("Terrasse", g, new Vector3(0f, 4.9f, 2.9f), new Vector3(3.6f, 1.4f, 5f), Pal.Chaux, 0f, false);
            MeshKit.ArcadeWall("Parement_Nord", g, new Vector3(-1.8f, 0f, 5.43f), 0f, 3.6f, 4.2f, 0.04f, 1, 2.4f, 2.3f, 0f, Pal.Chaux, null, 0.25f, false);
            MeshKit.WBox("Sol", g, new Vector3(0f, 0.03f, 2.9f), new Vector3(2.4f, 0.06f, 5f), Pal.BetonRose);
            // Porte bleue dans le mur est, lanterne suspendue sous la voûte
            BuildKit.Box("Porte_Bleue", g, new Vector3(1.15f, 1.1f, 3.8f), new Vector3(0.05f, 2.2f, 1.0f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Seuil_Porte", g, new Vector3(1.1f, 0.06f, 3.8f), new Vector3(0.15f, 0.12f, 1.1f), Pal.Marbre, 0f, false);
            BuildKit.Cyl("Chaine", g, new Vector3(0f, 3.25f, 1.6f), 0.01f, 0.5f, Pal.Fer);
            BuildKit.Box("Lanterne", g, new Vector3(0f, 2.85f, 1.6f), new Vector3(0.28f, 0.4f, 0.28f), Pal.LanterneVerre, 0f, false);
            BuildKit.Box("Lanterne_Toit", g, new Vector3(0f, 3.08f, 1.6f), new Vector3(0.34f, 0.06f, 0.34f), Pal.Fer, 0f, false);
        }

        // =====================================================================  L'ALLÉE (photos 01, 23, 147)
        static void BuildAlley(Transform root)
        {
            var g = BuildKit.Group("Allee", root);
            const float z0 = 5.4f, z1 = 55f;
            MeshKit.WBox("Allee_Beton_Rose", g, new Vector3(0f, 0.025f, (z0 + z1) * 0.5f), new Vector3(4f, 0.05f, z1 - z0), Pal.BetonRose);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Bordure", g, new Vector3(s * 2.1f, 0.06f, (z0 + z1) * 0.5f), new Vector3(0.2f, 0.12f, z1 - z0), Pal.BordureBlanche, 0f, false);

            // Côté gauche : haie taillée, agaves et aloès devant, palmiers et oliviers derrière ; ouverture vers le jardin andalou
            foreach (var seg in new[] { new Vector2(z0 + 1f, 22.5f), new Vector2(33.5f, z1 - 1f) })
            {
                Veg.Haie(g, new Vector3(-3.3f, 0f, seg.x), new Vector3(-3.3f, 0f, seg.y), 1.2f, 0.9f, Pal.CarteMaquis, Pal.CoeurSombre);
                for (float z = seg.x + 0.8f; z < seg.y; z += BuildKit.R(1.8f, 3f))
                    PropsExterieur.Agave(g, new Vector3(-2.55f, 0.05f, z), BuildKit.R(0.6f, 0.9f));
                for (float z = seg.x + 2f; z < seg.y; z += BuildKit.R(6f, 9f))
                {
                    if (BuildKit.Chance(0.5f)) PropsDetails.PalmierCanaries(g, new Vector3(BuildKit.R(-6f, -4.8f), 0f, z), BuildKit.R(3f, 6f));
                    else Veg.Olivier(g, new Vector3(BuildKit.R(-6f, -4.8f), 0f, z));
                }
            }
            // Côté droit : rangée d'eucalyptus en têtards aux troncs pâles (photo 147), arbustes bas
            for (float z = z0 + 2.5f; z < z1; z += BuildKit.R(5.5f, 7f))
                PropsExterieur.Eucalyptus(g, new Vector3(BuildKit.R(3.0f, 3.5f), 0f, z), BuildKit.R(10f, 13f), true);
            for (float z = z0 + 1.5f; z < z1; z += BuildKit.R(1.8f, 2.8f))
            {
                if (BuildKit.Chance(0.3f)) PropsExterieur.Agave(g, new Vector3(BuildKit.R(2.6f, 3f), 0.05f, z), BuildKit.R(0.6f, 0.9f));
                else PropsExterieur.Arbuste(g, new Vector3(BuildKit.R(2.8f, 7.5f), 0f, z), BuildKit.R(0.6f, 1.1f));
            }

            BuildKit.Info(g, new Vector3(0f, 1.5f, 14f), new Vector3(4f, 3f, 6f), "L'allée",
                "Une allée de béton rose bordée de blanc. À droite, les eucalyptus taillés en têtards ; à gauche, la haie, les agaves, les palmiers et les oliviers.\nPlus loin à gauche s'ouvre le jardin andalou.");
        }

        // =====================================================================  LE JARDIN ANDALOU (photos 10, 13, 16, 121, 150 ; CMAM)
        static void BuildJardinAndalou(Transform root)
        {
            var g = BuildKit.Group("Jardin_Andalou", root);
            float len = BassinX1 - BassinX0, cx = (BassinX0 + BassinX1) * 0.5f;

            BuildKit.Wall("Enceinte_Ouest", g, new Vector3(-62f, 0f, 0f), new Vector3(-62f, 0f, 44f), 3.2f, 0.6f, Pal.Chaux);

            // Bassin 2,5 × 45 m : eau verte entre deux margelles de marbre de 0,5 m, fond et parois turquoise
            BuildKit.Box("Eau", g, new Vector3(cx, 0.2f, BassinZ), new Vector3(len, 0.02f, 1.5f), Pal.EauVerte, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Margelle", g, new Vector3(cx, 0.15f, BassinZ + s * 1.0f), new Vector3(len + 1f, 0.3f, 0.5f), Pal.Marbre);
                BuildKit.Box("Carreaux_Turquoise", g, new Vector3(cx, 0.24f, BassinZ + s * 0.745f), new Vector3(len, 0.1f, 0.01f), Pal.CarreauTurquoise, 0f, false);
                float ex = s < 0 ? BassinX0 : BassinX1;
                BuildKit.Box("Margelle_Bout", g, new Vector3(ex + s * 0.25f, 0.15f, BassinZ), new Vector3(0.5f, 0.3f, 1.5f), Pal.Marbre);
            }
            // Dallage, frises de zellige, bande plantée, muret de pierre sèche et terrasse surélevée des cyprès
            for (int s = -1; s <= 1; s += 2)
            {
                MeshKit.WBox("Dallage", g, new Vector3(cx, 0.03f, BassinZ + s * 2.5f), new Vector3(len + 1f, 0.06f, 2.5f), Pal.DallesCalcaire);
                MeshKit.WBox("Frise_Zellige", g, new Vector3(cx, 0.065f, BassinZ + s * 1.35f), new Vector3(len + 1f, 0.01f, 0.2f), Pal.FriseZellige, 0f, false);
                MeshKit.WBox("Frise_Zellige_Ext", g, new Vector3(cx, 0.065f, BassinZ + s * 3.65f), new Vector3(len + 1f, 0.01f, 0.2f), Pal.FriseZellige, 0f, false);
                BuildKit.Box("Bande_Plantee", g, new Vector3(cx, 0.05f, BassinZ + s * 4.18f), new Vector3(len + 1f, 0.1f, 0.85f), Pal.Terre, 0f, false);
                for (float x = BassinX0; x < BassinX1; x += BuildKit.R(0.9f, 1.5f))
                    PropsExterieur.Arbuste(g, new Vector3(x, 0.05f, BassinZ + s * 4.1f), BuildKit.R(0.45f, 0.7f), Pal.Haie);
                MeshKit.WBox("Muret_Pierre_Seche", g, new Vector3(cx, 0.45f, BassinZ + s * 4.85f), new Vector3(len + 1f, 0.9f, 0.5f), Pal.PierreSeche);
                BuildKit.Box("Terrasse_Cypres", g, new Vector3(cx, 0.4f, BassinZ + s * 8.85f), new Vector3(len + 1f, 0.8f, 7.5f), Pal.Terre);
            }
            // Trois rangées de cyprès de chaque côté, sur la terrasse (entraxe ≈ 3 m, 14 à 19 m)
            foreach (float dz in new[] { 6.2f, 8.6f, 11.0f })
                for (int s = -1; s <= 1; s += 2)
                    for (float x = BassinX0 + 1f; x <= BassinX1 - 1f; x += 3f)
                        PropsExterieur.CypresAncien(g, new Vector3(x + BuildKit.R(-0.4f, 0.4f), 0.8f, BassinZ + s * dz + BuildKit.R(-0.3f, 0.3f)), BuildKit.R(14f, 19f));

            // Pavillon côté allée (photo 150) : deux niveaux, porte jaune en arc, fenêtres bleues, loggia bleue à l'étage
            var p = BuildKit.Group("Pavillon", g, new Vector3(-4.4f, 0f, BassinZ));
            BuildKit.Box("Volume", p, new Vector3(0f, 3.75f, 0f), new Vector3(4f, 7.5f, 6f), Pal.Chaux);
            BuildKit.Box("Acrotere", p, new Vector3(0f, 7.75f, 0f), new Vector3(4.1f, 0.5f, 6.1f), Pal.Chaux, 0f, false);
            BuildKit.Box("Marches", p, new Vector3(-2.3f, 0.1f, 0f), new Vector3(0.6f, 0.2f, 2.4f), Pal.Marbre);
            var face = BuildKit.Group("Facade_Bassin", p, new Vector3(-2.01f, 0f, 0f), new Vector3(0f, 90f, 0f));
            BuildKit.Box("Encadrement", face, new Vector3(0f, 1.4f, -0.02f), new Vector3(1.7f, 2.8f, 0.04f), Pal.Marbre, 0f, false);
            BuildKit.Cyl("Encadrement_Arc", face, new Vector3(0f, 2.8f, -0.02f), 0.85f, 0.04f, Pal.Marbre, new Vector3(90f, 0f, 0f));
            BuildKit.Box("Porte_Jaune", face, new Vector3(0f, 1.25f, -0.05f), new Vector3(1.3f, 2.5f, 0.04f), Pal.JaunePorte, 0f, false);
            BuildKit.Cyl("Porte_Jaune_Arc", face, new Vector3(0f, 2.5f, -0.05f), 0.65f, 0.04f, Pal.JaunePorte, new Vector3(90f, 0f, 0f));
            foreach (float lx in new[] { -1.9f, 1.9f })
                PropsExterieur.FenetrePersiennes(face, new Vector3(lx, 1.3f, 0f), 0f, 0.8f, 1.4f);
            BuildKit.Box("Loggia_Cadre", face, new Vector3(0f, 5.4f, -0.04f), new Vector3(3.2f, 2.0f, 0.06f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Loggia_Fond", face, new Vector3(0f, 5.5f, -0.08f), new Vector3(2.8f, 1.6f, 0.02f), Pal.VitreSombre, 0f, false);
            for (int i = -3; i <= 3; i++)
                BuildKit.Box("Balustre", face, new Vector3(i * 0.4f, 4.85f, -0.12f), new Vector3(0.05f, 0.6f, 0.05f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Lisse", face, new Vector3(0f, 5.15f, -0.12f), new Vector3(2.8f, 0.06f, 0.06f), Pal.BleuBois, 0f, false);
            var dos = BuildKit.Group("Facade_Allee", p, new Vector3(2.01f, 0f, 0f), new Vector3(0f, -90f, 0f));
            BuildKit.Box("Porte_Bleue", dos, new Vector3(0f, 1.1f, -0.03f), new Vector3(1.1f, 2.2f, 0.05f), Pal.BleuBois, 0f, false);

            // Cascade carrelée (salsabil) au fond, devant la haie et le bougainvillier, puis le village (photo 121)
            var cas = BuildKit.Group("Cascade", g, new Vector3(BassinX0 - 0.5f, 0f, BassinZ));
            for (int i = 0; i < 5; i++)
            {
                BuildKit.Box("Gradin", cas, new Vector3(-0.3f - i * 0.45f, 0.35f + i * 0.17f, 0f), new Vector3(0.45f, 0.17f, 1.8f), Pal.Marbre);
                BuildKit.Box("Lame_Eau", cas, new Vector3(-0.3f - i * 0.45f, 0.44f + i * 0.17f, 0f), new Vector3(0.44f, 0.01f, 1.2f), Pal.Eau, 0f, false);
            }
            BuildKit.Cyl("Vasque_Haute", cas, new Vector3(-2.7f, 1.35f, 0f), 0.45f, 0.2f, Pal.Marbre);
            BuildKit.Cyl("Jet", cas, new Vector3(-2.7f, 1.6f, 0f), 0.01f, 0.4f, Pal.Eau);
            Veg.Haie(cas, new Vector3(-3.6f, 0f, -4f), new Vector3(-3.6f, 0f, 4f), 2.4f, 1.2f, Pal.CarteMaquis, Pal.CoeurSombre);
            PropsDetails.Bougainvillier(cas, new Vector3(-3.4f, 2.6f, 1.6f), 1.6f);
            PropsDetails.PalmierCanaries(g, new Vector3(-60f, 0f, BassinZ + 2.5f), 9f);
            var imm = BuildKit.Group("Immeuble_Village", g, new Vector3(-74f, 3.5f, BassinZ + 3f));
            BuildKit.Box("Volume", imm, new Vector3(0f, 6f, 0f), new Vector3(10f, 12f, 14f), Pal.Chaux, 0f, false);
            for (int e = 0; e < 3; e++)
                for (int k = -2; k <= 2; k++)
                    BuildKit.Box("Fenetre", imm, new Vector3(5.03f, 2f + e * 3.5f, k * 2.6f), new Vector3(0.06f, 1.4f, 1.0f), e == 2 && k == 0 ? Pal.CarreauTurquoise : Pal.BleuBois, 0f, false);

            BuildKit.Info(g, new Vector3(BassinX1 + 1f, 1.5f, BassinZ - 3.5f), new Vector3(4f, 3f, 3f), "Le jardin andalou",
                "Aménagé par le baron dans les années 1920, sur le niveau haut du domaine : un bassin de 45 mètres sur 2,5, bordé de zellige vert et jaune, entre trois rangées de cyprès de chaque côté.\nAu fond, la cascade carrelée et, au-delà, le village de Sidi Bou Saïd.");
        }

        // =====================================================================  RESTE DU JARDIN
        static void BuildGardenRest(Transform root)
        {
            var g = BuildKit.Group("Jardin_Bas", root);
            for (float x = -58f; x < -8f; x += BuildKit.R(5f, 8f))
            {
                float z = BuildKit.R(3f, 13f);
                float t = BuildKit.R(0f, 1f);
                var pos = new Vector3(x, 0f, z);
                if (t < 0.4f) Veg.Olivier(g, pos);
                else if (t < 0.65f) PropsDetails.PalmierCanaries(g, pos, BuildKit.R(4f, 8f));
                else PropsExterieur.Eucalyptus(g, pos, BuildKit.R(12f, 17f), BuildKit.Chance(0.5f));
                PropsExterieur.Arbuste(g, new Vector3(x + BuildKit.R(-2f, 2f), 0f, BuildKit.R(2.5f, 14f)), BuildKit.R(1f, 1.6f));
            }
            for (float x = -34f; x < -6f; x += BuildKit.R(5f, 7f))
                PropsExterieur.Arbuste(g, new Vector3(x, 0f, BuildKit.R(41f, 43f)), BuildKit.R(1f, 1.6f));
        }

        // =====================================================================  LE LOINTAIN (directions réelles, photos 03, 131 à 136, 149)
        static void BuildLointain(Transform root)
        {
            var g = BuildKit.Group("Lointain", root);

            // Pente est : végétation dense en contrebas qui cadre la vue (eucalyptus, pins, maquis)
            var bois = BuildKit.Group("Bois_Pente_Est", g);
            for (int i = 0; i < 280; i++)
            {
                float x = BuildKit.R(16f, 110f), z = BuildKit.R(-140f, 160f);
                if (H(x, z) < SeaY + 3f) continue;
                float s = Mathf.Lerp(2.5f, 7f, Mathf.Clamp01((x - 16f) / 79f)) * BuildKit.R(0.8f, 1.1f);   // cimes en contrebas : la mer reste visible
                var m = BuildKit.Chance(0.35f) ? Pal.FeuillageEucalyptus : (BuildKit.Chance(0.4f) ? Pal.Pin : Pal.FeuillageMaquis);
                Veg.Couronne(bois, new Vector3(x, H(x, z) + s * 0.4f, z), new Vector3(s * 0.5f, s * 0.42f, s * 0.5f), 7, s * 0.55f, m == Pal.FeuillageEucalyptus ? Pal.CarteEucalyptus : m == Pal.Pin ? Pal.CartePin : Pal.CarteMaquis, Pal.CoeurSombre, 0.7f);
            }
            var maisons = BuildKit.Group("Maisons", g);
            for (int i = 0; i < 35; i++)
            {
                float x = BuildKit.R(40f, 150f), z = BuildKit.R(-60f, 120f);
                if (H(x, z) < SeaY + 4f) continue;
                PropsDetails.Maison(maisons, new Vector3(x, H(x, z) - 0.8f, z), BuildKit.R(-20f, 20f) + 90f);
            }
            // Pente sud : maisons et petits immeubles à toits plats jusqu'à la route de la plage
            for (int i = 0; i < 80; i++)
            {
                float x = BuildKit.R(-230f, 60f), z = BuildKit.R(-168f, -20f);
                if (H(x, z) < SeaY + 6f) continue;
                PropsDetails.Maison(maisons, new Vector3(x, H(x, z) - 0.8f, z), 180f + BuildKit.R(-15f, 15f), 4);
                if (BuildKit.Chance(0.5f))
                {
                    float s = BuildKit.R(3f, 6f);
                    float tx = x + BuildKit.R(-7f, 7f), tz = z + BuildKit.R(-6f, 6f);
                    Veg.Couronne(bois, new Vector3(tx, H(tx, tz) + s * 0.35f, tz), new Vector3(s * 0.5f, s * 0.42f, s * 0.5f), 6, s * 0.55f, Pal.CarteMaquis, Pal.CoeurSombre, 0.7f);
                }
            }
            BuildKit.NoShadows(bois);
            BuildKit.NoShadows(maisons);

            // Plage en arc, route du bord de mer et rangée de palmiers (photos 03, 149)
            var plage = BuildKit.Group("Plage", g);
            for (int i = 0; i < 13; i++)
            {
                float x = -290f + i * 23f;
                float zs = CoteZ(x);
                float bend = Mathf.Sin(i / 12f * Mathf.PI) * 12f;
                BuildKit.Box("Sable", plage, new Vector3(x, SeaY + 0.05f, zs - 7f - bend), new Vector3(26f, 0.3f, 18f), Pal.Sable, -6f + i, false);
                BuildKit.Box("Route", plage, new Vector3(x, H(x, zs + 3f) + 0.15f, zs + 3f), new Vector3(24f, 0.3f, 6f), Pal.Pierre, -6f + i, false);
                PropsDetails.PalmierCanaries(plage, new Vector3(x, SeaY + 0.2f, zs - 0.5f - bend * 0.3f), BuildKit.R(7f, 10f));
            }
            BuildKit.NoShadows(plage);

            // Port de plaisance de Sidi Bou Saïd, au sud-est, au bout de la plage : quai le long de la côte, jetées, mâts
            // (photos 12, 22, 131, 149). Repère local : x le long de la côte (vers le sud-ouest), z vers le large (sud-est).
            var port = BuildKit.Group("Port_Plaisance", g, new Vector3(95f, SeaY, CoteZ(95f) - 2f), new Vector3(0f, 135f, 0f));
            BuildKit.Box("Quai", port, new Vector3(0f, 0.6f, 4f), new Vector3(90f, 1.5f, 10f), Pal.Pierre, 0f, false);
            BuildKit.Box("Jetee_Nord", port, new Vector3(-45f, 1f, 40f), new Vector3(8f, 2.5f, 70f), Pal.Pierre, 0f, false);
            BuildKit.Box("Jetee_Large", port, new Vector3(-8f, 1f, 75f), new Vector3(80f, 2.5f, 8f), Pal.Pierre, 0f, false);
            BuildKit.Cyl("Feu_Jetee", port, new Vector3(32f, 4f, 75f), 1f, 6f, Pal.Chaux);
            for (int i = 0; i < 45; i++)
                PropsExterieur.Bateau(port, new Vector3(BuildKit.R(-38f, 35f), 0.2f, BuildKit.R(12f, 68f)), BuildKit.R(-15f, 15f), BuildKit.R(6f, 12f));
            BuildKit.NoShadows(port);

            // Village de Sidi Bou Saïd : au-dessus, à l'ouest ; minaret dans l'axe du bassin (CMAM), phare au nord-ouest
            var village = BuildKit.Group("Village_Haut", g);
            for (int i = 0; i < 60; i++)
            {
                float x = BuildKit.R(-220f, -82f), z = BuildKit.R(-5f, 220f);
                PropsDetails.Maison(village, new Vector3(x, 3.5f, z), BuildKit.R(0f, 360f));
            }
            Props.Minaret(village, new Vector3(-150f, 3.5f, BassinZ + 6f));
            Props.Lighthouse(village, new Vector3(-120f, 3.5f, 240f));
            BuildKit.NoShadows(village);

            // Bou Kornine : plein sud, deux cornes (576 m à 18,3 km → même taille apparente à 2,1 km)
            var bk = MeshKit.Relief("Bou_Kornine", g, new Vector3(-147f, SeaY, -2095f), 1700f, new[]
            {
                new Vector4(-95f, 0f, 105f, 120f), new Vector4(95f, 25f, 95f, 115f),
                new Vector4(0f, 60f, 62f, 380f), new Vector4(-480f, 60f, 30f, 300f), new Vector4(460f, 90f, 34f, 320f),
            }, Pal.Montagne);
            // Cap Bon : longue ligne de reliefs bas à l'est–sud-est (azimut ≈ 105°)
            var cb = MeshKit.Relief("Cap_Bon", g, new Vector3(2150f, SeaY, -580f), 2600f, new[]
            {
                new Vector4(0f, -900f, 38f, 260f), new Vector4(60f, -450f, 55f, 280f), new Vector4(40f, 0f, 62f, 300f),
                new Vector4(-20f, 450f, 48f, 260f), new Vector4(20f, 900f, 35f, 240f),
            }, Pal.Montagne);
            BuildKit.NoShadows(bk.transform);
            BuildKit.NoShadows(cb.transform);
        }

        // =====================================================================  L'ESPLANADE (photos 32, 41, 124, 138)
        static void BuildPlaza(Transform root)
        {
            var g = BuildKit.Group("Esplanade", root);

            PropsExterieur.BalustradeBleue(g, new Vector3(4.4f, 0.05f, 55f), new Vector3(4.4f, 0.05f, 96.7f));
            PropsExterieur.BalustradeBleue(g, new Vector3(4.4f, 0.05f, 96.7f), new Vector3(-36f, 0.05f, 96.7f));
            PropsExterieur.BalustradeBleue(g, new Vector3(2.2f, 0.05f, 55f), new Vector3(4.4f, 0.05f, 55f), 2.2f);
            Veg.Haie(g, new Vector3(-36f, 0f, 54.4f), new Vector3(-2.7f, 0f, 54.4f), 1.0f, 0.8f, Pal.CarteMaquis, Pal.CoeurSombre);

            // Haie de lantanas fleuris et bougainvilliers derrière la balustrade (photos 124, 138)
            PropsDetails.HaieLantana(g, new Vector3(5.4f, 0f, 55.5f), new Vector3(5.4f, 0f, 97.5f), 0.9f);
            PropsDetails.HaieLantana(g, new Vector3(5.4f, 0f, 97.7f), new Vector3(-36f, 0f, 97.7f), 0.9f);
            PropsDetails.Bougainvillier(g, new Vector3(5.6f, 1.4f, 70f), 1.4f);
            PropsDetails.Bougainvillier(g, new Vector3(-20f, 1.4f, 97.9f), 1.4f);
            // Grands conifères aux deux angles (photos 124, 126) et eucalyptus au-delà (photo 138)
            PropsDetails.GrandConifere(g, new Vector3(7.2f, 0f, 98.8f), 15f);
            PropsDetails.GrandConifere(g, new Vector3(-34f, 0f, 99.4f), 14f);
            PropsExterieur.Eucalyptus(g, new Vector3(7.5f, 0f, 84f), 16f, false);

            PropsExterieur.PilierLanterne(g, new Vector3(3.9f, 0.05f, 66f), 90f);
            PropsExterieur.PilierLanterne(g, new Vector3(3.9f, 0.05f, 86f), 90f);
            PropsExterieur.PilierLanterne(g, new Vector3(-14f, 0.05f, 96.2f), 0f);

            BuildKit.Info(g, new Vector3(0f, 1.5f, 76f), new Vector3(7f, 3f, 40f), "L'esplanade",
                "Devant vous, le golfe de Tunis : en contrebas le port de Sidi Bou Saïd et la plage ; plein sud, le Bou Kornine et ses deux cornes ; à l'est, les montagnes du cap Bon.\nDerrière vous : la longue façade blanche du palais et sa galerie à arcades.");
        }

        // =====================================================================  LE PALAIS (photos 122, 124, 126, W73)
        static void BuildPalace(Transform root)
        {
            var g = BuildKit.Group("Palais", root);
            const float z0 = 60f, z1 = 96f, len = z1 - z0;

            // Rez-de-chaussée derrière la galerie ; étage en retrait derrière la terrasse de la galerie (« deux marches géantes »)
            BuildKit.Box("Volume_RDC", g, new Vector3((-62f + GalerieX) * 0.5f, HautRdc * 0.5f, DoorZ), new Vector3(GalerieX + 62f, HautRdc, len), Pal.Chaux);
            BuildKit.Box("Volume_Etage", g, new Vector3((-62f + EtageX) * 0.5f, (HautRdc + HautTotale) * 0.5f, DoorZ), new Vector3(EtageX + 62f, HautTotale - HautRdc, len), Pal.Chaux);
            BuildKit.Box("Plafond_Galerie", g, new Vector3((GalerieX + FacadeX) * 0.5f, HautRdc - 0.1f, DoorZ), new Vector3(FacadeX - GalerieX, 0.2f, len), Pal.Chaux, 0f, false);
            BuildKit.Box("Parapet_Terrasse", g, new Vector3(FacadeX - 0.2f, HautRdc + 0.5f, DoorZ), new Vector3(0.4f, 1.0f, len), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Etage", g, new Vector3(EtageX + 0.15f, HautTotale + 0.35f, DoorZ), new Vector3(0.3f, 0.7f, len), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Nord", g, new Vector3(-50.75f, HautTotale + 0.35f, z1 - 0.15f), new Vector3(22.5f, 0.7f, 0.3f), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Sud", g, new Vector3(-50.75f, HautTotale + 0.35f, z0 + 0.15f), new Vector3(22.5f, 0.7f, 0.3f), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Ouest", g, new Vector3(-61.85f, HautTotale + 0.35f, DoorZ), new Vector3(0.3f, 0.7f, len), Pal.Chaux, 0f, false);

            // Galerie : sol de marbre veiné, 11 arcs plein cintre sur colonnes de marbre à chapiteaux
            MeshKit.WBox("Galerie_Sol", g, new Vector3((GalerieX + FacadeX) * 0.5f, 0.075f, DoorZ), new Vector3(FacadeX - GalerieX, 0.15f, len), Pal.MarbreVeine);
            const int n = 11; const float pier = 0.36f;
            float openW = (len - (n + 1) * pier) / n;
            const float colTop = 2.75f;
            float[] piers = MeshKit.ArcadeWall("Galerie_Arcades", g, new Vector3(FacadeX - 0.2f, colTop, z0), -90f, len, HautRdc - colTop, 0.4f,
                                               n, openW, 0f, 0f, Pal.Chaux, null, 0.25f, false);
            for (int i = 0; i < piers.Length; i++)
            {
                var c = new Vector3(FacadeX - 0.2f, 0f, z0 + piers[i]);
                BuildKit.Box("Base", g, c + new Vector3(0f, 0.24f, 0f), new Vector3(0.34f, 0.18f, 0.34f), Pal.Marbre, 0f, false);
                BuildKit.Cyl("Colonne", g, c + new Vector3(0f, 1.47f, 0f), 0.13f, 2.3f, Pal.Marbre, Vector3.zero, true);
                BuildKit.Cyl("Astragale", g, c + new Vector3(0f, colTop - 0.2f, 0f), 0.16f, 0.05f, Pal.Marbre);
                BuildKit.Box("Chapiteau", g, c + new Vector3(0f, colTop - 0.08f, 0f), new Vector3(0.4f, 0.16f, 0.4f), Pal.Marbre, 0f, false);
                if (i == 2 || i == 4 || i == 7 || i == 9)
                    PropsDetails.Urne(g, new Vector3(FacadeX - 0.75f, 0.15f, z0 + piers[i] + (i < 6 ? 1.2f : -1.2f)), true);
            }
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Galerie_Joue", g, new Vector3((GalerieX + FacadeX) * 0.5f, HautRdc * 0.5f, s < 0 ? z0 + 0.2f : z1 - 0.2f),
                             new Vector3(FacadeX - GalerieX, HautRdc, 0.4f), Pal.Chaux);

            // Fond de la galerie : repère local (x = le long de la façade vers +Z, −z = vers l'esplanade)
            var fond = BuildKit.Group("Galerie_Fond", g, new Vector3(GalerieX, 0f, DoorZ), new Vector3(0f, -90f, 0f));
            float bay = openW + pier;
            // Porte principale : trois marches de marbre, arc outrepassé, bois sombre clouté (photo 122, W73)
            for (int k = 0; k < 3; k++)
                BuildKit.Box("Marche", fond, new Vector3(0f, 0.15f + (k + 1) * 0.075f, -0.9f + k * 0.3f), new Vector3(2.6f, (k + 1) * 0.15f, 0.3f), Pal.Marbre);
            BuildKit.Box("Palier", fond, new Vector3(0f, 0.375f, -0.1f), new Vector3(2.6f, 0.45f, 0.3f), Pal.Marbre);
            const float yP = 0.6f;
            BuildKit.Arch("Arc_Porte", fond, new Vector3(0f, yP, -0.1f), 0f, 2.2f, 2.5f, 0.25f, 0.28f, Pal.Marbre, 18f, 16, true);
            BuildKit.Box("Porte_Bois", fond, new Vector3(0f, yP + 1.25f, -0.04f), new Vector3(2.2f, 2.5f, 0.06f), Pal.BoisSombre, 0f, false);
            BuildKit.Cyl("Porte_Bois_Arc", fond, new Vector3(0f, yP + 2.5f + 1.1f * Mathf.Sin(18f * Mathf.Deg2Rad), -0.04f), 1.1f, 0.06f, Pal.BoisSombre, new Vector3(90f, 0f, 0f));
            BuildKit.Studs(fond, new Vector3(0f, yP + 1.35f, 0f), 1.9f, 2.2f, 6, 8, -0.08f, Pal.Fer, 0.05f);
            BuildKit.Box("Jointure", fond, new Vector3(0f, yP + 1.45f, -0.08f), new Vector3(0.04f, 2.8f, 0.03f), Pal.Fer, 0f, false);
            for (int i = -5; i <= 5; i++)
            {
                if (i == 0) continue;
                float lx = i * bay;
                if (Mathf.Abs(i) == 3)
                {
                    BuildKit.Box("Encadrement", fond, new Vector3(lx, 1.5f, -0.02f), new Vector3(1.7f, 2.8f, 0.03f), Pal.Marbre, 0f, false);
                    BuildKit.Box("Porte_Secondaire", fond, new Vector3(lx, 1.45f, -0.04f), new Vector3(1.4f, 2.6f, 0.06f), Pal.BoisSombre, 0f, false);
                }
                else if (Mathf.Abs(i) == 5) PropsDetails.FenetreGrille(fond, new Vector3(lx, 0.95f, 0f), 0f, 1.0f, 1.8f);
                else
                {
                    PropsDetails.FenetreGrille(fond, new Vector3(lx, 0.95f, 0f), 0f, 1.1f, 1.8f);
                    if (Mathf.Abs(i) == 2) BuildKit.Box("Banc_Marbre", fond, new Vector3(lx + (i > 0 ? 1.2f : -1.2f), 0.4f, -0.35f), new Vector3(1.4f, 0.12f, 0.45f), Pal.Marbre);
                }
            }

            // Étage en retrait (photos 122, 126) : loggia bleue saillante au centre, petites fenêtres carrées, fenêtres à auvent,
            // un chebbek à chaque extrémité
            var etage = BuildKit.Group("Etage_Decor", g, new Vector3(EtageX, 0f, DoorZ), new Vector3(0f, -90f, 0f));
            for (int i = -5; i <= 5; i++)
            {
                float lx = i * bay;
                if (i == 0)
                {
                    BuildKit.Box("Loggia_Volume", etage, new Vector3(0f, 7.8f, -0.55f), new Vector3(3.6f, 5.2f, 1.1f), Pal.Chaux, 0f, false);
                    BuildKit.Box("Loggia_Cadre", etage, new Vector3(0f, 7.4f, -1.12f), new Vector3(2.5f, 3.2f, 0.06f), Pal.BleuBois, 0f, false);
                    PropsExterieur.FenetrePersiennes(etage, new Vector3(0f, 7.3f, -1.12f), 0f, 2.1f, 1.7f);
                    MeshKit.WBox("Loggia_Grille", etage, new Vector3(0f, 6.55f, -1.2f), new Vector3(2.1f, 1.0f, 0.012f), Pal.GrilleCarree, 0f, false);
                    for (int s = -1; s <= 1; s += 2)
                        BuildKit.Cyl("Demi_Colonne", etage, new Vector3(s * 1.45f, 7.6f, -1.12f), 0.09f, 3.4f, Pal.Marbre);
                    PropsDetails.Auvent(etage, new Vector3(0f, 10.45f, -1.1f), 4.0f, 0.6f, Pal.TuilesEcailles);
                }
                else if (Mathf.Abs(i) == 1)
                    BuildKit.Box("Fenetre_Carree", etage, new Vector3(lx, 7.4f, -0.04f), new Vector3(0.9f, 0.9f, 0.06f), Pal.BleuBois, 0f, false);
                else if (Mathf.Abs(i) == 5) Props.Mashrabiya(etage, new Vector3(lx, 6.4f, 0f), 0f, 1.8f, 2.2f, 0.6f);
                else PropsDetails.FenetreAuvent(etage, new Vector3(lx, 6.6f, 0f), 0f);
            }

            // Toit : coupole blanche, drapeau tunisien, projecteurs (photos 122, 126)
            BuildKit.Cyl("Tambour", g, new Vector3(-52f, HautTotale + 0.5f, 74f), 2.4f, 1f, Pal.Chaux);
            BuildKit.Sph("Coupole", g, new Vector3(-52f, HautTotale + 1f, 74f), new Vector3(4.8f, 3.8f, 4.8f), Pal.Chaux);
            PropsExterieur.DrapeauTunisien(g, new Vector3(-46f, HautTotale, DoorZ));
            foreach (float pz in new[] { z0 + 1f, z1 - 1f })
                BuildKit.Box("Projecteur", g, new Vector3(EtageX - 0.4f, HautTotale + 0.9f, pz), new Vector3(0.35f, 0.25f, 0.3f), Pal.Projecteur, 0f, false);

            BuildKit.Info(g, new Vector3(-37.5f, 1.5f, DoorZ), new Vector3(3f, 3f, 6f), "Entrée principale du palais",
                "Sous la galerie à arcades de marbre, la grande porte cloutée.\nFranchissez-la : la visite de l'intérieur andalou commence… et avec elle la recherche de la clé de la chambre secrète.");
            BuildKit.Transition(g, new Vector3(GalerieX + 0.35f, 1.6f + yP, DoorZ), new Vector3(0.5f, 2.4f, 2.0f), NextScene, "FromExterior");
        }

        // =====================================================================  LE JARDIN PERSAN (photos 28 à 45, 125 à 146 ; CMAM)
        static void BuildJardinPersan(Transform root)
        {
            var g = BuildKit.Group("Jardin_Persan", root);
            var c = new Vector3(-49f, 0.06f, 51.5f);

            MeshKit.WBox("Dallage", g, new Vector3(-49f, 0.03f, 51.75f), new Vector3(22f, 0.06f, 15.5f), Pal.DallesCalcaire);   // x −60..−38, z 44..59,5

            // Haie de cyprès taillés (CMAM) côté esplanade et côté jardin, avec passages
            Veg.Haie(g, new Vector3(-37.6f, 0f, 44f), new Vector3(-37.6f, 0f, 56.5f), 3.5f, 1f, Pal.CarteCypres, Pal.CoeurCypres);
            Veg.Haie(g, new Vector3(-60f, 0f, 43.6f), new Vector3(-51f, 0f, 43.6f), 3.5f, 1f, Pal.CarteCypres, Pal.CoeurCypres);
            Veg.Haie(g, new Vector3(-47f, 0f, 43.6f), new Vector3(-38f, 0f, 43.6f), 3.5f, 1f, Pal.CarteCypres, Pal.CoeurCypres);

            // Haut mur blanc à l'ouest, terrasse à garde-corps bleu au-dessus, escalier au vieux tronc (photos 125, 127, 129)
            BuildKit.Wall("Mur_Haut_Ouest", g, new Vector3(-62f, 0f, 43.6f), new Vector3(-62f, 0f, 60f), 6f, 0.6f, Pal.Chaux);
            for (float z = 44.5f; z < 59.5f; z += 0.14f)
                BuildKit.Box("Balustre_Terrasse", g, new Vector3(-61.75f, 6.45f, z), new Vector3(0.04f, 0.9f, 0.04f), Pal.BleuFer, 0f, false);
            BuildKit.Box("Lisse_Terrasse", g, new Vector3(-61.75f, 6.92f, 52f), new Vector3(0.08f, 0.06f, 15f), Pal.BleuFer, 0f, false);
            PropsDetails.EscalierTronc(g, new Vector3(-61.1f, 0.06f, 44.4f), 0f, 3.2f, 5.0f);
            PropsDetails.Bougainvillier(g, new Vector3(-61f, 6.2f, 57f), 2.0f);
            foreach (var z in new[] { 44.9f, 46.6f })
                PropsExterieur.CypresAncien(g, new Vector3(-57.5f + (z - 44.9f), 0.06f, z), BuildKit.R(10f, 13f));

            // Carrés de plantation : quatre autour de la vasque (bigaradiers et géraniums), un de soucis, un de lavandes
            foreach (var o in new[] { new Vector2(-2.4f, -2.4f), new Vector2(2.4f, -2.4f), new Vector2(-2.4f, 2.4f), new Vector2(2.4f, 2.4f) })
                PropsDetails.Carre(g, c + new Vector3(o.x, 0f, o.y), 3f, 3f, 0, true);
            PropsDetails.Carre(g, c + new Vector3(-7.0f, 0f, -2.4f), 3f, 3f, 1, false);
            PropsDetails.Carre(g, c + new Vector3(7.4f, 0f, -2.4f), 3f, 3f, 2, false);
            PropsDetails.Vasque(g, c);

            // Côté est : plate-bande de terre, pas japonais, lavandes, jarre (photos 129, 134)
            BuildKit.Box("Plate_Bande_Est", g, new Vector3(-40.4f, 0.07f, 55.5f), new Vector3(4f, 0.08f, 6.5f), Pal.Terre, 0f, false);
            for (int i = 0; i < 7; i++)
                BuildKit.Box("Pas_Japonais", g, new Vector3(-40.4f + Mathf.Sin(i) * 0.4f, 0.12f, 52.8f + i * 0.85f), new Vector3(0.55f, 0.04f, 0.45f), Pal.Marbre, BuildKit.R(-20f, 20f), false);
            Veg.Massif(g, new Vector3(-41.6f, 0.1f, 55.5f), 1.4f, 6f, 0.6f, Pal.CarteLavande, 4f);
            Veg.Massif(g, new Vector3(-39.0f, 0.1f, 56.5f), 1.2f, 4f, 0.6f, Pal.CarteLavande, 4f);
            Props.Pot(g, new Vector3(-39.3f, 0.1f, 54f), 1.1f, false);
            PropsExterieur.Arbuste(g, new Vector3(-39.2f, 0.06f, 58.6f), 1.4f, Pal.Haie);

            // Façade sud : porte de fer bleue sous auvent en treillis, grille bombée à l'étage, fenêtres (photos 134 à 146)
            var fac = BuildKit.Group("Facade_Sud", g, new Vector3(-49f, 0f, 60f));
            PropsDetails.PortePersane(fac, new Vector3(0f, 0.06f, 0f), 0f);
            PropsDetails.FenetreBombee(fac, new Vector3(0f, 5.7f, 0f), 0f);
            PropsDetails.FenetreAuvent(fac, new Vector3(3.6f, 6.4f, 0f), 0f, 1.0f, 1.6f);
            PropsDetails.FenetreAuvent(fac, new Vector3(-3.6f, 6.4f, 0f), 0f, 0.8f, 1.4f);
            PropsDetails.FenetreGrille(fac, new Vector3(3.4f, 1.2f, 0f), 0f, 0.9f, 1.2f);
            PropsDetails.FenetreGrille(fac, new Vector3(-4.6f, 1.2f, 0f), 0f, 0.9f, 1.2f);
            BuildKit.Box("Porte_Est_Encadrement", fac, new Vector3(8.6f, 1.3f, -0.02f), new Vector3(1.5f, 2.6f, 0.04f), Pal.Marbre, 0f, false);
            BuildKit.Box("Porte_Est", fac, new Vector3(8.6f, 1.2f, -0.05f), new Vector3(1.1f, 2.3f, 0.03f), Pal.PorteBleueFer, 0f, false);
            // Fenêtres jumelles en arc à colonnette, près du kiosque (photo 130)
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Fenetre_Arc", fac, new Vector3(-7.2f + s * 0.4f, 1.6f, -0.04f), new Vector3(0.6f, 1.5f, 0.05f), Pal.BleuBois, 0f, false);
                BuildKit.Cyl("Fenetre_Arc_Haut", fac, new Vector3(-7.2f + s * 0.4f, 2.35f, -0.04f), 0.3f, 0.05f, Pal.BleuBois, new Vector3(90f, 0f, 0f));
            }
            BuildKit.Cyl("Colonnette", fac, new Vector3(-7.2f, 1.55f, -0.08f), 0.05f, 1.7f, Pal.Marbre);
            PropsDetails.Urne(g, new Vector3(-56.4f, 0.06f, 58.9f), true);
            PropsDetails.Urne(g, new Vector3(-43.6f, 0.06f, 58.9f), true);
            PropsDetails.Buste(g, new Vector3(-59.6f, 0.06f, 51.6f), 90f);

            // Kiosque à faïences et tuiles vertes à l'ouest, bougainvillier qui le domine
            PropsDetails.Kiosque(g, new Vector3(-58.3f, 0.06f, 55.2f), -90f);
            PropsDetails.Bougainvillier(g, new Vector3(-59.5f, 4.9f, 57.8f), 1.8f);
            // Arbre caduc nu au premier plan (photos 28 à 35, 134 : visite en hiver)
            PropsExterieur.ArbreNu(g, new Vector3(-40.5f, 0.06f, 46.5f), 8f);

            BuildKit.Info(g, c + new Vector3(0f, 1.5f, 0f), new Vector3(16f, 3f, 12f), "Le jardin persan",
                "Un petit jardin serré entre les murs du palais et une haute haie de cyprès : des carrés plantés de bigaradiers et de géraniums, bordés de grilles bleues à volutes, autour d'une vasque de grès.\nAu fond, le kiosque aux faïences sous ses tuiles vertes ; devant vous, la porte de fer bleue sous son auvent en treillis.");
        }
    }
}
