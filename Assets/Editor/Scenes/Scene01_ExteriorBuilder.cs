using UnityEditor;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// SCÈNE 1 — L'EXTÉRIEUR DU PALAIS ENNEJMA EZZAHRA (version calée sur les photos de la visite)
    ///
    ///   Rue → Porte jaune (z = 0) → Skifa voûtée en brique (z 0,4 → 5,4) → Allée des eucalyptus (z 5,4 → 55)
    ///   → à gauche : jardin andalou, bassin de 2,5 × 45 m entre trois rangées de cyprès de chaque côté (axe z = 28)
    ///   → Esplanade de béton rose à balustrade bleue, face à la galerie à arcades de la façade principale (x = −36)
    ///   → Jardin persan au sud du palais : carrés à grilles bleues, vasque, porte bleue sous auvent, kiosque
    ///   → Porte principale au fond de la galerie : transition vers la scène 2 (Hall d'entrée)
    ///
    /// Repère : +Z = nord, +X = est (la mer). Lointain placé aux directions réelles (REALITE_HYPOTHESES.md) :
    /// Bou Kornine plein sud (azimut 184°), cap Bon à l'est–sud-est (105°), port et plage en contrebas au sud–sud-est,
    /// village de Sidi Bou Saïd au-dessus, à l'ouest et au nord-ouest. Les distances lointaines sont réduites
    /// (≈ 2,1 km au lieu de 18–20 km) en gardant la taille apparente.
    /// Soleil : fin d'après-midi d'hiver (≈ 15 h, azimut 220°, hauteur 24°), d'après les ombres des photos.
    /// </summary>
    public static class Scene01_ExteriorBuilder
    {
        public const string SceneName = "01_Exterieur";
        public const string NextScene = "02_Hall_Entree";

        const float SlopeDeg = 12f;
        const float SeaY = -36f;
        static float SlopeY(float x) => -1.5f - (x - 8f) * Mathf.Tan(SlopeDeg * Mathf.Deg2Rad);          // pente est
        static float SlopeSudY(float z) => -0.5f - (-12f - z) * Mathf.Tan(SlopeDeg * Mathf.Deg2Rad);     // pente sud (z < −12)

        // Façade principale : arcades à x = −36, fond de la galerie (porte) à x = −39
        const float FacadeX = -36f, GalerieX = -39f, DoorZ = 78f;
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
            // Perspective aérienne : horizon laiteux, lointains désaturés (photos 03, 21, 26, 30)
            RenderSettings.fogDensity = 0.00042f;
            RenderSettings.fogColor = new Color(0.8f, 0.83f, 0.86f);

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

            BuildKit.Box("Sol_Domaine", g, new Vector3(-27f, -0.5f, 44f), new Vector3(70f, 1f, 112f), Pal.Herbe);       // x −62..8, z −12..100
            MeshKit.WBox("Rue_Dalles", g, new Vector3(-6f, 0.025f, -6f), new Vector3(28f, 0.05f, 12f), Pal.DallesRue);
            MeshKit.WBox("Esplanade", g, new Vector3(-16f, 0.025f, 75.75f), new Vector3(40f, 0.05f, 41.5f), Pal.BetonRose);

            float a = SlopeDeg * Mathf.Deg2Rad;
            // Pente boisée à l'est, jusqu'à la mer
            var est = new Vector3(8f + 100f * Mathf.Cos(a), -1.5f - 100f * Mathf.Sin(a) - 0.5f, -50f);
            BuildKit.BoxE("Pente_Est", g, est, new Vector3(200f, 1f, 900f), Pal.Maquis, new Vector3(0f, 0f, -SlopeDeg), false);
            // Pente sud : toits en contrebas jusqu'à la plage (photo 03)
            var sud = new Vector3(-275f, -0.5f - 200f * Mathf.Sin(a) - 0.5f, -12f - 200f * Mathf.Cos(a));
            BuildKit.BoxE("Pente_Sud", g, sud, new Vector3(570f, 1f, 400f), Pal.Maquis, new Vector3(-SlopeDeg, 0f, 0f), false);
            // Le village est au-dessus du palais, à l'ouest et au nord-ouest
            BuildKit.Box("Plateau_Village_Ouest", g, new Vector3(-562f, 1.5f, 494f), new Vector3(1000f, 4f, 1012f), Pal.Colline, 0f, false);
            BuildKit.Box("Terres_Nord", g, new Vector3(-27f, 0.5f, 550f), new Vector3(70f, 2f, 900f), Pal.Maquis, 0f, false);

            BuildKit.Box("Golfe_de_Tunis", g, new Vector3(1500f, SeaY - 0.5f, -1500f), new Vector3(5000f, 1f, 5000f), Pal.Mer, 0f, false);

            BuildKit.InvisibleWall("Limite_Est", g, new Vector3(8.3f, 1.5f, 44f), new Vector3(0.4f, 4f, 112f));
            BuildKit.InvisibleWall("Limite_Nord", g, new Vector3(-27f, 1.5f, 100.2f), new Vector3(70f, 4f, 0.4f));
            BuildKit.InvisibleWall("Limite_Ouest", g, new Vector3(-62.2f, 1.5f, 44f), new Vector3(0.4f, 4f, 112f));
            BuildKit.InvisibleWall("Limite_Rue_Sud", g, new Vector3(-6f, 1.5f, -12.2f), new Vector3(28f, 4f, 0.4f));
            BuildKit.InvisibleWall("Limite_Rue_Ouest", g, new Vector3(-20.2f, 1.5f, -6f), new Vector3(0.4f, 4f, 12f));
            BuildKit.InvisibleWall("Limite_Rue_Est", g, new Vector3(8.2f, 1.5f, -6f), new Vector3(0.4f, 4f, 12f));
        }

        // =====================================================================  LA PORTE JAUNE (photo 02, W66, W76, W93)
        static void BuildYellowGate(Transform root)
        {
            var g = BuildKit.Group("Porte_Jaune", root);

            BuildKit.Wall("Enceinte_Sud_Ouest", g, new Vector3(-62f, 0f, 0f), new Vector3(-2.2f, 0f, 0f), 3.2f, 0.6f, Pal.Chaux);
            BuildKit.Wall("Enceinte_Sud_Est", g, new Vector3(2.2f, 0f, 0f), new Vector3(8f, 0f, 0f), 3.2f, 0.6f, Pal.Chaux);

            // Portail : arc outrepassé, passage de 2,2 m, clé à ≈ 3,7 m ; encadrement de pierre sculptée (textures photo)
            const float open = 2.2f, spring = 2.35f, horseshoe = 15f;
            BuildKit.WallWithArch("Portail", g, Vector3.zero, 0f, 4.4f, 5.0f, 0.8f, open, spring, Pal.Chaux, Pal.Chaux, horseshoe, 0.25f);
            BuildKit.Box("Bandeau_Sculpte", g, new Vector3(0f, 4.3f, -0.43f), new Vector3(3.4f, 0.9f, 0.05f), Pal.PortailBandeau, 0f, false);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Montant_Sculpte", g, new Vector3(s * 1.4f, 1.0f, -0.43f), new Vector3(0.5f, 1.8f, 0.05f), Pal.PortailMontant, 0f, false);
            BuildKit.Box("Corniche", g, new Vector3(0f, 5.05f, -0.05f), new Vector3(4.6f, 0.12f, 0.95f), Pal.Chaux, 0f, false);
            BuildKit.Box("Seuil", g, new Vector3(0f, 0.04f, 0f), new Vector3(open, 0.08f, 0.8f), Pal.Marbre, 0f, false);

            // Battants jaunes cloutés (texture tirée de la photo), ouverts vers la skifa
            const float leaf = 1.1f, leafH = 3.1f;
            for (int s = -1; s <= 1; s += 2)
            {
                var hinge = BuildKit.Group(s < 0 ? "Battant_Gauche" : "Battant_Droit", g,
                                           new Vector3(s * leaf, 0.05f, 0.42f), new Vector3(0f, s * 80f, 0f));
                var v = BuildKit.Box("Vantail", hinge, new Vector3(-s * leaf * 0.5f, leafH * 0.5f, 0.05f), new Vector3(leaf, leafH, 0.08f), Pal.PorteJaunePhoto);
                v.isStatic = true;
                BuildKit.Cyl("Heurtoir_Anneau", hinge, new Vector3(-s * (leaf - 0.2f), 1.75f, -0.02f), 0.07f, 0.02f, Pal.Fer, new Vector3(90f, 0f, 0f));
            }

            Props.Bougainvillea(g, new Vector3(-6f, 3.0f, -0.45f), 1.4f);
            Props.Bougainvillea(g, new Vector3(4.5f, 3.0f, -0.45f), 1.2f);

            BuildKit.Info(g, new Vector3(0f, 1.5f, -2.5f), new Vector3(6f, 3f, 3f), "La porte jaune",
                "Bienvenue au palais Ennejma Ezzahra, bâti entre 1912 et 1922 par le baron Rodolphe d'Erlanger, peintre et musicologue passionné de musique arabe.\nFranchissez la porte cloutée et la skifa voûtée.");
        }

        // =====================================================================  LA SKIFA (passage voûté en brique, photo 02)
        static void BuildSkifa(Transform root)
        {
            var g = BuildKit.Group("Skifa", root);
            // Bloc percé d'un berceau : 3,6 m de large, 5 m de profondeur, voûte de brique (clé à 3,5 m)
            MeshKit.ArcadeWall("Voute", g, new Vector3(-1.8f, 0f, 2.9f), 0f, 3.6f, 4.2f, 5f, 1, 2.4f, 2.3f, 0f, Pal.Brique, null);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Mur_Chaux", g, new Vector3(s * 1.185f, 1.15f, 2.9f), new Vector3(0.03f, 2.3f, 5f), Pal.Chaux, 0f, false);
                BuildKit.Box("Enduit_Ext", g, new Vector3(s * 1.83f, 2.1f, 2.9f), new Vector3(0.06f, 4.2f, 5f), Pal.Chaux, 0f, false);
            }
            BuildKit.Box("Terrasse", g, new Vector3(0f, 4.3f, 2.9f), new Vector3(3.8f, 0.2f, 5.1f), Pal.Chaux, 0f, false);
            MeshKit.ArcadeWall("Parement_Nord", g, new Vector3(-1.8f, 0f, 5.43f), 0f, 3.6f, 4.2f, 0.04f, 1, 2.4f, 2.3f, 0f, Pal.Chaux, null, 0.25f, false);
            MeshKit.WBox("Sol", g, new Vector3(0f, 0.03f, 2.9f), new Vector3(2.4f, 0.06f, 5f), Pal.BetonRose);
            // Porte bleue dans le mur est
            BuildKit.Box("Porte_Bleue", g, new Vector3(1.15f, 1.1f, 3.8f), new Vector3(0.05f, 2.2f, 1.0f), Pal.BleuBois, 0f, false);
            BuildKit.Box("Seuil_Porte", g, new Vector3(1.1f, 0.06f, 3.8f), new Vector3(0.15f, 0.12f, 1.1f), Pal.Marbre, 0f, false);
        }

        // =====================================================================  L'ALLÉE DES EUCALYPTUS (photos 01, 23)
        static void BuildAlley(Transform root)
        {
            var g = BuildKit.Group("Allee", root);
            const float z0 = 5.4f, z1 = 55f;
            MeshKit.WBox("Allee_Beton_Rose", g, new Vector3(0f, 0.025f, (z0 + z1) * 0.5f), new Vector3(4f, 0.05f, z1 - z0), Pal.BetonRose);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Bordure", g, new Vector3(s * 2.1f, 0.06f, (z0 + z1) * 0.5f), new Vector3(0.2f, 0.12f, z1 - z0), Pal.BordureBlanche, 0f, false);

            // Côté gauche : arbustes bas et agaves, ouverture vers le jardin andalou (z 23,5..32,5)
            for (float z = z0 + 1.5f; z < z1 - 1f; z += BuildKit.R(1.6f, 2.6f))
            {
                if (z > 22.5f && z < 33.5f) continue;
                if (BuildKit.Chance(0.3f)) PropsExterieur.Agave(g, new Vector3(-2.9f, 0.05f, z), BuildKit.R(0.8f, 1.2f));
                else PropsExterieur.Arbuste(g, new Vector3(BuildKit.R(-3.4f, -2.7f), 0f, z), BuildKit.R(0.9f, 1.5f));
            }
            // Eucalyptus des deux côtés (dont des têtards à droite, côté pente)
            for (float z = z0 + 3f; z < z1; z += BuildKit.R(6.5f, 8.5f))
            {
                if (!(z > 21f && z < 35f)) PropsExterieur.Eucalyptus(g, new Vector3(BuildKit.R(-4.6f, -3.8f), 0f, z), BuildKit.R(13f, 18f), false);
                PropsExterieur.Eucalyptus(g, new Vector3(BuildKit.R(3.4f, 4.4f), 0f, z + 3.5f), BuildKit.R(10f, 16f), BuildKit.Chance(0.5f));
            }
            for (float z = z0 + 2f; z < z1; z += BuildKit.R(2f, 3.2f))
                PropsExterieur.Arbuste(g, new Vector3(BuildKit.R(2.8f, 7.5f), 0f, z), BuildKit.R(0.6f, 1.1f));

            BuildKit.Info(g, new Vector3(0f, 1.5f, 14f), new Vector3(4f, 3f, 6f), "L'allée des eucalyptus",
                "Une allée de béton rose bordée de blanc, sous les eucalyptus aux troncs pâles ; certains sont taillés en têtards.\nÀ gauche s'ouvre le jardin andalou et son long bassin entre les cyprès.");
        }

        // =====================================================================  LE JARDIN ANDALOU (photos 10, 13, 16 ; CMAM)
        static void BuildJardinAndalou(Transform root)
        {
            var g = BuildKit.Group("Jardin_Andalou", root);
            float len = BassinX1 - BassinX0, cx = (BassinX0 + BassinX1) * 0.5f;

            BuildKit.Wall("Enceinte_Ouest", g, new Vector3(-62f, 0f, 0f), new Vector3(-62f, 0f, 60f), 3.2f, 0.6f, Pal.Chaux);

            // Bassin 2,5 × 45 m : eau verte entre deux margelles de marbre de 0,5 m
            BuildKit.Box("Eau", g, new Vector3(cx, 0.2f, BassinZ), new Vector3(len, 0.02f, 1.5f), Pal.EauVerte, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Margelle", g, new Vector3(cx, 0.15f, BassinZ + s * 1.0f), new Vector3(len + 1f, 0.3f, 0.5f), Pal.Marbre);
                BuildKit.Box("Carreaux_Turquoise", g, new Vector3(cx, 0.24f, BassinZ + s * 0.745f), new Vector3(len, 0.1f, 0.01f), Pal.CarreauTurquoise, 0f, false);
                float ex = s < 0 ? BassinX0 : BassinX1;
                BuildKit.Box("Margelle_Bout", g, new Vector3(ex + s * 0.25f, 0.15f, BassinZ), new Vector3(0.5f, 0.3f, 1.5f), Pal.Marbre);
            }
            // Dallage de calcaire et frises de zellige vert et jaune de part et d'autre
            for (int s = -1; s <= 1; s += 2)
            {
                MeshKit.WBox("Dallage", g, new Vector3(cx, 0.03f, BassinZ + s * 2.5f), new Vector3(len + 1f, 0.06f, 2.5f), Pal.DallesCalcaire);
                MeshKit.WBox("Frise_Zellige", g, new Vector3(cx, 0.065f, BassinZ + s * 1.35f), new Vector3(len + 1f, 0.01f, 0.2f), Pal.FriseZellige, 0f, false);
                MeshKit.WBox("Frise_Zellige_Ext", g, new Vector3(cx, 0.065f, BassinZ + s * 3.65f), new Vector3(len + 1f, 0.01f, 0.2f), Pal.FriseZellige, 0f, false);
                // Bande plantée puis muret de pierre sèche
                BuildKit.Box("Bande_Plantee", g, new Vector3(cx, 0.05f, BassinZ + s * 4.18f), new Vector3(len + 1f, 0.1f, 0.85f), Pal.Terre, 0f, false);
                for (float x = BassinX0; x < BassinX1; x += BuildKit.R(1.2f, 2f))
                    PropsExterieur.Arbuste(g, new Vector3(x, 0.05f, BassinZ + s * 4.18f), BuildKit.R(0.5f, 0.8f), Pal.Haie);
                MeshKit.WBox("Muret_Pierre_Seche", g, new Vector3(cx, 0.4f, BassinZ + s * 4.85f), new Vector3(len + 1f, 0.8f, 0.5f), Pal.PierreSeche);
            }
            MeshKit.WBox("Frise_Zellige_Est", g, new Vector3(BassinX1 + 0.6f, 0.066f, BassinZ), new Vector3(0.2f, 0.01f, 7.5f), Pal.FriseZelligeZ, 0f, false);
            MeshKit.WBox("Dallage_Entree", g, new Vector3(BassinX1 + 2.6f, 0.03f, BassinZ), new Vector3(3.8f, 0.06f, 9.2f), Pal.DallesCalcaire);

            // Trois rangées de cyprès de chaque côté, derrière les murets (entraxe ≈ 3 m, 14 à 19 m de haut)
            foreach (float dz in new[] { 6.0f, 8.4f, 10.8f })
                for (int s = -1; s <= 1; s += 2)
                    for (float x = BassinX0 + 1f; x <= BassinX1 - 1f; x += 3f)
                        PropsExterieur.CypresAncien(g, new Vector3(x + BuildKit.R(-0.4f, 0.4f), 0f, BassinZ + s * dz + BuildKit.R(-0.3f, 0.3f)), BuildKit.R(14f, 19f));

            // Pavillon (Belvédère) au bout du bassin : volume blanc, porte bleue en arc (photo 10)
            var p = BuildKit.Group("Pavillon_Belvedere", g, new Vector3(-57.5f, 0f, BassinZ));
            BuildKit.Box("Volume", p, new Vector3(0f, 3.25f, 0f), new Vector3(6f, 6.5f, 10f), Pal.Chaux);
            BuildKit.Box("Acrotere", p, new Vector3(0f, 6.7f, 0f), new Vector3(6.2f, 0.4f, 10.2f), Pal.Chaux, 0f, false);
            BuildKit.Box("Marches", p, new Vector3(3.3f, 0.1f, 0f), new Vector3(0.6f, 0.2f, 3f), Pal.Marbre);
            var face = BuildKit.Group("Facade", p, new Vector3(3.01f, 0f, 0f), new Vector3(0f, -90f, 0f));
            BuildKit.Box("Encadrement", face, new Vector3(0f, 1.5f, -0.02f), new Vector3(2.0f, 3.0f, 0.04f), Pal.Marbre, 0f, false);
            BuildKit.Cyl("Encadrement_Arc", face, new Vector3(0f, 3.0f, -0.02f), 1.0f, 0.04f, Pal.Marbre, new Vector3(90f, 0f, 0f));
            BuildKit.Box("Porte", face, new Vector3(0f, 1.35f, -0.05f), new Vector3(1.5f, 2.7f, 0.04f), Pal.BleuBois, 0f, false);
            BuildKit.Cyl("Porte_Arc", face, new Vector3(0f, 2.7f, -0.05f), 0.75f, 0.04f, Pal.BleuBois, new Vector3(90f, 0f, 0f));
            foreach (float lx in new[] { -3.3f, 3.3f })
                PropsExterieur.FenetrePersiennes(face, new Vector3(lx, 1.4f, 0f), 0f);

            BuildKit.Info(g, new Vector3(BassinX1 + 2f, 1.5f, BassinZ), new Vector3(4f, 3f, 8f), "Le jardin andalou",
                "Aménagé par le baron dans les années 1920, sur le niveau haut du domaine : un bassin de 45 mètres sur 2,5, bordé de zellige vert et jaune, entre trois rangées de cyprès de chaque côté.\nAu fond, le pavillon blanc à la porte bleue.");
        }

        // =====================================================================  RESTE DU JARDIN (oliviers, palmiers, eucalyptus)
        static void BuildGardenRest(Transform root)
        {
            var g = BuildKit.Group("Jardin_Bas", root);
            // Bande sud (z 2..15) et petite bande nord (z 40..43) : plantations clairsemées, sans le hasard « pépinière » d'avant
            for (float x = -58f; x < -6f; x += BuildKit.R(5f, 8f))
            {
                float z = BuildKit.R(3f, 13f);
                float t = BuildKit.R(0f, 1f);
                var pos = new Vector3(x, 0f, z);
                if (t < 0.4f) Props.OliveTree(g, pos);
                else if (t < 0.65f) Props.Palm(g, pos, BuildKit.R(7f, 10f));
                else PropsExterieur.Eucalyptus(g, pos, BuildKit.R(12f, 17f), BuildKit.Chance(0.4f));
                PropsExterieur.Arbuste(g, new Vector3(x + BuildKit.R(-2f, 2f), 0f, BuildKit.R(2.5f, 14f)), BuildKit.R(1f, 1.6f));
            }
            for (float x = -34f; x < -6f; x += BuildKit.R(5f, 7f))
                PropsExterieur.Arbuste(g, new Vector3(x, 0f, BuildKit.R(41f, 43f)), BuildKit.R(1f, 1.6f));
        }

        // =====================================================================  LE LOINTAIN (directions réelles)
        static void BuildLointain(Transform root)
        {
            var g = BuildKit.Group("Lointain", root);

            // Pente est : végétation dense en contrebas qui cadre la vue (photos 11, 12, 19, 20) et quelques maisons
            var bois = BuildKit.Group("Bois_Pente_Est", g);
            for (int i = 0; i < 260; i++)
            {
                float x = BuildKit.R(16f, 95f), z = BuildKit.R(-60f, 140f);
                float s = Mathf.Lerp(2.5f, 7f, (x - 16f) / 79f) * BuildKit.R(0.8f, 1.1f);   // cimes en contrebas : la mer reste visible
                var m = BuildKit.Chance(0.35f) ? Pal.FeuillageEucalyptus : Pal.FeuillageMaquis;
                BuildKit.Sph("Cime", bois, new Vector3(x, SlopeY(x) + s * 0.35f, z), new Vector3(s, s * 0.8f, s), m);
            }
            var maisons = BuildKit.Group("Maisons_Pente", g);
            for (int i = 0; i < 45; i++)
            {
                float x = BuildKit.R(40f, 150f), z = BuildKit.R(-120f, 60f);
                var p = new Vector3(x, SlopeY(x) - 0.8f, z);
                Props.SbsHouse(maisons, p, BuildKit.R(-20f, 20f) + 90f);   // façade vers le palais (ouest)
            }
            // Pente sud : toits plats et terrasses jusqu'à la plage (photo 03)
            for (int i = 0; i < 70; i++)
            {
                float x = BuildKit.R(-220f, 30f), z = BuildKit.R(-150f, -20f);
                var p = new Vector3(x, SlopeSudY(z) - 0.8f, z);
                Props.SbsHouse(maisons, p, 180f + BuildKit.R(-15f, 15f));
                if (BuildKit.Chance(0.4f))
                {
                    float s = BuildKit.R(3f, 6f);
                    BuildKit.Sph("Cime", bois, new Vector3(x + BuildKit.R(-6f, 6f), SlopeSudY(z) + s * 0.3f, z + BuildKit.R(-6f, 6f)), new Vector3(s, s * 0.8f, s), Pal.FeuillageMaquis);
                }
            }
            BuildKit.NoShadows(bois);
            BuildKit.NoShadows(maisons);

            // Plage en arc bordée de palmiers, au sud (photos 03, 08, 15)
            var plage = BuildKit.Group("Plage", g);
            float zs = -12f - (-0.5f - SeaY) / Mathf.Tan(SlopeDeg * Mathf.Deg2Rad);
            for (int i = 0; i < 12; i++)
            {
                float x = -260f + i * 23f;
                float bend = Mathf.Sin(i / 11f * Mathf.PI) * 14f;
                BuildKit.Box("Sable", plage, new Vector3(x, SeaY + 0.05f, zs - 8f - bend), new Vector3(26f, 0.2f, 16f), Pal.Sable, -8f + i * 1.5f, false);
                Props.Palm(plage, new Vector3(x, SeaY + 0.1f, zs - 1f - bend), BuildKit.R(9f, 13f));
            }
            BuildKit.NoShadows(plage);

            // Port de plaisance de Sidi Bou Saïd, au sud–sud-est, en contrebas : jetées, quais et mâts
            var port = BuildKit.Group("Port_Plaisance", g);
            BuildKit.Box("Jetee_Est", port, new Vector3(265f, SeaY + 1f, -150f), new Vector3(8f, 2.5f, 110f), Pal.Pierre, 0f, false);
            BuildKit.Box("Jetee_Sud", port, new Vector3(222f, SeaY + 1f, -205f), new Vector3(90f, 2.5f, 8f), Pal.Pierre, 0f, false);
            BuildKit.Box("Quai", port, new Vector3(205f, SeaY + 0.6f, -110f), new Vector3(70f, 1.5f, 18f), Pal.Pierre, 0f, false);
            for (int i = 0; i < 40; i++)
                PropsExterieur.Bateau(port, new Vector3(BuildKit.R(185f, 255f), SeaY + 0.2f, BuildKit.R(-195f, -125f)), BuildKit.R(-20f, 20f), BuildKit.R(6f, 12f));
            BuildKit.NoShadows(port);

            // Village de Sidi Bou Saïd : au-dessus, à l'ouest ; minaret dans l'axe du bassin (CMAM), phare au nord-ouest
            var village = BuildKit.Group("Village_Haut", g);
            for (int i = 0; i < 60; i++)
            {
                float x = BuildKit.R(-220f, -75f), z = BuildKit.R(-5f, 220f);
                Props.SbsHouse(village, new Vector3(x, 3.5f, z), BuildKit.R(0f, 360f));
            }
            Props.Minaret(village, new Vector3(-150f, 3.5f, BassinZ + 6f));
            Props.Lighthouse(village, new Vector3(-120f, 3.5f, 240f));
            BuildKit.NoShadows(village);

            // Bou Kornine : plein sud (azimut 184°), 576 m à 18,3 km → même taille apparente à 2,1 km
            var bk = BuildKit.Group("Bou_Kornine", g, new Vector3(-147f, SeaY, -2095f));
            BuildKit.Sph("Corne_Ouest", bk, new Vector3(-120f, 0f, 0f), new Vector3(420f, 132f, 260f), Pal.Montagne);
            BuildKit.Sph("Corne_Est", bk, new Vector3(110f, 0f, 20f), new Vector3(380f, 114f, 240f), Pal.Montagne);
            BuildKit.Sph("Massif", bk, new Vector3(0f, 0f, 60f), new Vector3(1100f, 70f, 420f), Pal.Montagne);
            // Cap Bon : à l'est–sud-est (azimut ≈ 105°), longue ligne de reliefs bas
            var cb = BuildKit.Group("Cap_Bon", g, new Vector3(2125f, SeaY, -570f), new Vector3(0f, 15f, 0f));
            for (int i = -2; i <= 2; i++)
                BuildKit.Sph("Relief", cb, new Vector3(0f, 0f, i * 380f), new Vector3(300f, BuildKit.R(50f, 80f), 520f), Pal.Montagne);
            BuildKit.NoShadows(bk);
            BuildKit.NoShadows(cb);
        }

        // =====================================================================  L'ESPLANADE (photos 32, 41, 22, 25)
        static void BuildPlaza(Transform root)
        {
            var g = BuildKit.Group("Esplanade", root);

            PropsExterieur.BalustradeBleue(g, new Vector3(4.4f, 0.05f, 55f), new Vector3(4.4f, 0.05f, 96.7f));
            PropsExterieur.BalustradeBleue(g, new Vector3(4.4f, 0.05f, 96.7f), new Vector3(-36f, 0.05f, 96.7f));
            PropsExterieur.BalustradeBleue(g, new Vector3(2.2f, 0.05f, 55f), new Vector3(4.4f, 0.05f, 55f), 2.2f);
            Props.Hedge(g, new Vector3(-36f, 0f, 54.4f), new Vector3(-2.7f, 0f, 54.4f));

            PropsExterieur.PilierLanterne(g, new Vector3(3.9f, 0.05f, 66f), 90f);
            PropsExterieur.PilierLanterne(g, new Vector3(3.9f, 0.05f, 86f), 90f);
            PropsExterieur.PilierLanterne(g, new Vector3(-14f, 0.05f, 96.2f), 0f);

            BuildKit.Info(g, new Vector3(0f, 1.5f, 76f), new Vector3(7f, 3f, 40f), "L'esplanade",
                "Devant vous, le golfe de Tunis : en contrebas le port de Sidi Bou Saïd et la plage ; plein sud, le Bou Kornine et ses deux cornes ; à l'est, les montagnes du cap Bon.\nDerrière vous : la longue façade blanche du palais et sa galerie à arcades.");
        }

        // =====================================================================  LE PALAIS (W94, W95, W98, W73, photo 41)
        static void BuildPalace(Transform root)
        {
            var g = BuildKit.Group("Palais", root);
            const float z0 = 60f, z1 = 96f, len = z1 - z0;

            // Volumes : rez-de-chaussée derrière la galerie (x ≤ −39), étage au nu des arcades (x ≤ −36)
            BuildKit.Box("Volume_RDC", g, new Vector3((-62f + GalerieX) * 0.5f, HautRdc * 0.5f, DoorZ), new Vector3(GalerieX + 62f, HautRdc, len), Pal.Chaux);
            BuildKit.Box("Volume_Etage", g, new Vector3((-62f + FacadeX) * 0.5f, (HautRdc + HautTotale) * 0.5f, DoorZ), new Vector3(FacadeX + 62f, HautTotale - HautRdc, len), Pal.Chaux);
            BuildKit.Box("Bandeau_Etage", g, new Vector3(FacadeX + 0.07f, HautRdc + 0.08f, DoorZ), new Vector3(0.14f, 0.16f, len), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Est", g, new Vector3(FacadeX + 0.15f, HautTotale + 0.4f, DoorZ), new Vector3(0.3f, 0.8f, len), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Nord", g, new Vector3(-49f, HautTotale + 0.4f, z1 - 0.15f), new Vector3(26f, 0.8f, 0.3f), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Sud", g, new Vector3(-49f, HautTotale + 0.4f, z0 + 0.15f), new Vector3(26f, 0.8f, 0.3f), Pal.Chaux, 0f, false);
            BuildKit.Box("Acrotere_Ouest", g, new Vector3(-61.85f, HautTotale + 0.4f, DoorZ), new Vector3(0.3f, 0.8f, len), Pal.Chaux, 0f, false);

            // Galerie : sol de marbre veiné (une marche de 15 cm), 11 arcs plein cintre sur colonnes de marbre
            MeshKit.WBox("Galerie_Sol", g, new Vector3((GalerieX + FacadeX) * 0.5f, 0.075f, DoorZ), new Vector3(FacadeX - GalerieX, 0.15f, len), Pal.MarbreVeine);
            const int n = 11; const float pier = 0.36f;
            float openW = (len - (n + 1) * pier) / n;
            const float colTop = 2.75f;
            float[] piers = MeshKit.ArcadeWall("Galerie_Arcades", g, new Vector3(FacadeX - 0.2f, colTop, z0), -90f, len, HautRdc - colTop, 0.4f,
                                               n, openW, 0f, 0f, Pal.Chaux, null, 0.25f, false);
            foreach (float px in piers)
            {
                var c = new Vector3(FacadeX - 0.2f, 0f, z0 + px);
                BuildKit.Box("Base", g, c + new Vector3(0f, 0.24f, 0f), new Vector3(0.34f, 0.18f, 0.34f), Pal.Marbre, 0f, false);
                BuildKit.Cyl("Colonne", g, c + new Vector3(0f, 1.47f, 0f), 0.13f, 2.3f, Pal.Marbre, Vector3.zero, true);
                BuildKit.Box("Chapiteau", g, c + new Vector3(0f, colTop - 0.08f, 0f), new Vector3(0.4f, 0.16f, 0.4f), Pal.Marbre, 0f, false);
            }
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Galerie_Joue", g, new Vector3((GalerieX + FacadeX) * 0.5f, HautRdc * 0.5f, s < 0 ? z0 + 0.2f : z1 - 0.2f),
                             new Vector3(FacadeX - GalerieX, HautRdc, 0.4f), Pal.Chaux);

            // Fond de la galerie : repère local (x = le long de la façade vers +Z, −z = vers l'esplanade)
            var fond = BuildKit.Group("Galerie_Fond", g, new Vector3(GalerieX, 0f, DoorZ), new Vector3(0f, -90f, 0f));
            float bay = openW + pier;
            // Porte principale : arc outrepassé de marbre, bois sombre clouté (W73, W77)
            BuildKit.Arch("Arc_Porte", fond, new Vector3(0f, 0.15f, -0.1f), 0f, 2.2f, 2.5f, 0.25f, 0.28f, Pal.Marbre, 18f, 16, true);
            BuildKit.Box("Porte_Bois", fond, new Vector3(0f, 1.4f, -0.04f), new Vector3(2.2f, 2.5f, 0.06f), Pal.BoisSombre, 0f, false);
            BuildKit.Cyl("Porte_Bois_Arc", fond, new Vector3(0f, 0.15f + 2.5f + 1.1f * Mathf.Sin(18f * Mathf.Deg2Rad), -0.04f), 1.1f, 0.06f, Pal.BoisSombre, new Vector3(90f, 0f, 0f));
            BuildKit.Studs(fond, new Vector3(0f, 1.5f, 0f), 1.9f, 2.2f, 6, 8, -0.08f, Pal.Fer, 0.05f);
            BuildKit.Box("Jointure", fond, new Vector3(0f, 1.6f, -0.08f), new Vector3(0.04f, 2.8f, 0.03f), Pal.Fer, 0f, false);
            for (int i = -5; i <= 5; i++)
            {
                if (i == 0) continue;
                float lx = i * bay;
                if (Mathf.Abs(i) == 2 || Mathf.Abs(i) == 4)
                    Props.Window(fond, new Vector3(lx, 1.15f, 0f), 0f, 1.2f, 1.8f, true);
                else if (Mathf.Abs(i) == 3)
                {
                    BuildKit.Box("Porte_Secondaire", fond, new Vector3(lx, 1.45f, -0.04f), new Vector3(1.4f, 2.6f, 0.06f), Pal.BoisSombre, 0f, false);
                    BuildKit.Box("Encadrement", fond, new Vector3(lx, 1.5f, -0.02f), new Vector3(1.7f, 2.8f, 0.03f), Pal.Marbre, 0f, false);
                }
                else
                {
                    BuildKit.Cyl("Vasque_Pied", fond, new Vector3(lx, 0.45f, -0.6f), 0.12f, 0.6f, Pal.Marbre, Vector3.zero, true);
                    BuildKit.Cyl("Vasque", fond, new Vector3(lx, 0.85f, -0.6f), 0.32f, 0.3f, Pal.Marbre);
                    BuildKit.Sph("Plante", fond, new Vector3(lx, 1.15f, -0.6f), new Vector3(0.7f, 0.5f, 0.7f), Pal.Feuillage);
                }
            }

            // Étage (au nu des arcades) : volets persiennés bleus, deux chebbeks, loggia centrale bleue
            var etage = BuildKit.Group("Etage_Decor", g, new Vector3(FacadeX, 0f, DoorZ), new Vector3(0f, -90f, 0f));
            for (int i = -5; i <= 5; i++)
            {
                float lx = i * bay;
                if (i == 0)
                {
                    BuildKit.Box("Loggia_Cadre", etage, new Vector3(0f, 7.6f, -0.05f), new Vector3(3.2f, 3.6f, 0.1f), Pal.BleuBois, 0f, false);
                    BuildKit.Box("Loggia_Fond", etage, new Vector3(0f, 7.5f, -0.11f), new Vector3(2.7f, 3.0f, 0.03f), Pal.VitreSombre, 0f, false);
                    Props.Mashrabiya(etage, new Vector3(0f, 6.2f, -0.1f), 0f, 2.5f, 2.6f, 0.35f);
                }
                else if (i == -4 || i == 4) Props.Mashrabiya(etage, new Vector3(lx, 6.4f, 0f), 0f, 1.8f, 2.2f, 0.6f);
                else PropsExterieur.FenetrePersiennes(etage, new Vector3(lx, 6.6f, 0f), 0f);
            }

            // Toit : coupole blanche, drapeau tunisien
            BuildKit.Cyl("Tambour", g, new Vector3(-50f, HautTotale + 0.5f, 86f), 2.4f, 1f, Pal.Chaux);
            BuildKit.Sph("Coupole", g, new Vector3(-50f, HautTotale + 1f, 86f), new Vector3(4.8f, 3.8f, 4.8f), Pal.Chaux);
            PropsExterieur.DrapeauTunisien(g, new Vector3(-42f, HautTotale, DoorZ));

            // Entrée dans le palais → Scène 2
            BuildKit.Info(g, new Vector3(-37.5f, 1.5f, DoorZ), new Vector3(3f, 3f, 6f), "Entrée principale du palais",
                "Sous la galerie à arcades de marbre, la grande porte cloutée.\nFranchissez-la : la visite de l'intérieur andalou commence… et avec elle la recherche de la clé de la chambre secrète.");
            BuildKit.Transition(g, new Vector3(GalerieX + 0.35f, 1.6f, DoorZ), new Vector3(0.5f, 2.4f, 2.0f), NextScene, "FromExterior");
        }

        // =====================================================================  LE JARDIN PERSAN (photos 28 à 45, W71, W82, W92 ; CMAM)
        static void BuildJardinPersan(Transform root)
        {
            var g = BuildKit.Group("Jardin_Persan", root);
            var c = new Vector3(-49f, 0.06f, 51.5f);

            MeshKit.WBox("Dallage", g, new Vector3(-49f, 0.03f, 51.75f), new Vector3(22f, 0.06f, 15.5f), Pal.DallesCalcaire);   // x −60..−38, z 44..59,5

            // Haie de cyprès taillés (CMAM) côté esplanade et côté jardin, avec passages
            BuildKit.Wall("Haie_Cypres_Est", g, new Vector3(-37.6f, 0f, 44f), new Vector3(-37.6f, 0f, 56.5f), 3.5f, 1f, Pal.Cypres);
            BuildKit.Wall("Haie_Cypres_Sud_O", g, new Vector3(-60f, 0f, 43.6f), new Vector3(-51f, 0f, 43.6f), 3.5f, 1f, Pal.Cypres);
            BuildKit.Wall("Haie_Cypres_Sud_E", g, new Vector3(-47f, 0f, 43.6f), new Vector3(-38f, 0f, 43.6f), 3.5f, 1f, Pal.Cypres);

            // Quatre carrés autour de la vasque, deux de plus le long des côtés
            foreach (var o in new[] { new Vector2(-2.4f, -2.4f), new Vector2(2.4f, -2.4f), new Vector2(-2.4f, 2.4f), new Vector2(2.4f, 2.4f) })
                PropsExterieur.CarreJardin(g, c + new Vector3(o.x, 0f, o.y), 3f);
            PropsExterieur.CarreJardin(g, c + new Vector3(-7.4f, 0f, 0f), 3f, false);
            PropsExterieur.CarreJardin(g, c + new Vector3(7.4f, 0f, -2.4f), 3f, false);
            PropsExterieur.VasqueGres(g, c);

            // Façade sud : porte bleue sous auvent, chebbek au-dessus, fenêtres (photos 33, 34, 40)
            var fac = BuildKit.Group("Facade_Sud", g, new Vector3(-49f, 0f, 60f), new Vector3(0f, 0f, 0f));
            PropsExterieur.PorteBleueAuvent(fac, new Vector3(0f, 0.06f, 0f), 0f);
            Props.Mashrabiya(fac, new Vector3(0f, 6.3f, 0f), 0f, 1.9f, 2.2f, 0.6f);
            foreach (float lx in new[] { -5f, 5f })
                Props.Window(fac, new Vector3(lx, 1.1f, 0f), 0f, 1.0f, 1.5f, true);
            foreach (float lx in new[] { -9f, -4f, 4f, 9f })
                PropsExterieur.FenetrePersiennes(fac, new Vector3(lx, 6.8f, 0f), 0f);

            // Kiosque à tuiles vertes au fond (ouest), bougainvillier
            PropsExterieur.Kiosque(g, new Vector3(-58.8f, 0.06f, 51.5f), -90f);
            Props.Bougainvillea(g, new Vector3(-56.5f, 3.2f, 55.5f), 1.6f);
            // Arbre caduc nu au premier plan (photos 28 à 35 : visite en hiver)
            PropsExterieur.ArbreNu(g, new Vector3(-40.5f, 0.06f, 46.5f), 8f);

            BuildKit.Info(g, c + new Vector3(0f, 1.5f, 0f), new Vector3(16f, 3f, 12f), "Le jardin persan",
                "Un petit jardin serré entre les murs du palais et une haute haie de cyprès : des carrés plantés de bigaradiers, bordés de grilles bleues, autour d'une vasque.\nAu fond, la porte bleue sous son auvent et, à l'étage, le moucharabieh d'une chambre.");
        }
    }
}
