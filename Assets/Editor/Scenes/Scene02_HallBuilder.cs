using UnityEditor;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// SCÈNE 2 — LE HALL D'ENTRÉE DU PALAIS ENNEJMA EZZAHRA (version 2, d'après les photos de la visite)
    ///
    /// Même repère que la scène 1 (+Z = nord, +X = vers la mer). Parcours :
    ///   Porte (x = −36,3) → Vestibule (y = 0) → Descente de 8 marches de marbre rose
    ///   → GRANDE NEF (sol y = −1,28) : damier rose et blanc, rigole d'eau et vasque, arcades sur colonnes jumelées
    ///     de marbre rose, stuc ciselé ; galerie haute à balustrades de bois tourné, plafond doré (y = 9,6)
    ///      ├─ bas-côtés nord et sud : lambris d'onyx, divans rouges, niche de marbre noir, lampes de bronze
    ///      │   et les deux escaliers de marbre rose → transition vers l'étage (scène 3)
    ///      └─ au fond (ouest) : porte du salon « bit gaâd », rosaces de stuc ; au-dessus, la galerie de moucharabiehs rouges
    ///
    ///   x : nef −43,3 → −55,3 (12 m) ; bit gaâd −55,8 → −61,4
    ///   z : nef 74,25 → 81,75 (7,5 m) ; bas-côtés 70,25 → 73,75 et 82,25 → 85,75
    /// </summary>
    public static class Scene02_HallBuilder
    {
        public const string SceneName = "02_Hall_Entree";
        public const string ScenePath = "Assets/Scenes/02_Hall_Entree.unity";
        public const string PrevScene = "01_Exterieur";
        public const string NextScene = "03_Etage_Chambres";

        const float YH = -1.28f;            // sol de la nef, des bas-côtés et du bit gaâd
        const float YG = YH + 5.2f;         // sol de la galerie haute (+3,92)
        const float YGc = YG + 3.4f;        // plafond des galeries (+7,32)
        const float YC = 9.6f;              // plafond doré de la nef
        const float YV = 4.5f;              // plafond du vestibule et de la descente
        const float YB = YH + 4.6f;         // plafond du bit gaâd (+3,32)
        const float ZA = 78f;               // axe de la visite
        const float XE = -43.3f, XW = -55.3f;
        const float Rise = 0.16f, Tread = 0.30f;
        const int AisleSteps = 12;
        const float YL = YH + AisleSteps * Rise;   // palier des escaliers (+0,64)

        [MenuItem("Ennejma/Phase 1/Scène 2 - Hall d'entrée", priority = 2)]
        public static void Build()
        {
            var scene = BuildKit.NewScene();
            if (!scene.IsValid()) return;
            BuildKit.BeginBuild(1913);

            BuildKit.SetupInterior(new Color(0.6f, 0.55f, 0.48f), new Color(0.45f, 0.38f, 0.32f), new Color(0.28f, 0.22f, 0.18f), 0.3f);

            var root = new GameObject("=== HALL D'ENTREE ===").transform;
            BuildVestibule(root);
            BuildNef(root);
            BuildGalerieEtPlafond(root);
            BuildBasCote(root, true);
            BuildBasCote(root, false);
            BuildFondOuest(root);
            BuildBitGaad(root);

            var gameplay = new GameObject("=== GAMEPLAY ===").transform;
            var spawnPos = new Vector3(-38.2f, 0.05f, ZA);
            BuildKit.Spawn(gameplay, spawnPos, -90f, "FromExterior");
            BuildKit.Spawn(gameplay, new Vector3(-45.6f, YH + 0.05f, 84.95f), 180f, "FromEtage");
            BuildKit.Transition(gameplay, new Vector3(-36.85f, 1.5f, ZA), new Vector3(0.5f, 3f, 2.8f), PrevScene, "FromInterior");
            BuildKit.CreatePlayer(spawnPos, -90f);

            BuildKit.SaveScene(scene, SceneName);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Ennejma Ezzahra",
                    "Scène 2 (hall d'entrée) construite !\n\nAppuyez sur Play : descendez les marches de marbre rose vers la grande nef.", "OK");
        }

        /// <summary>Génère la scène puis enregistre les captures de contrôle dans Screenshots/ (mode batch).</summary>
        public static void BuildAndCapture()
        {
            Build();
            CaptureTool.Capture(ScenePath, "scene02", CapturePoses);
        }

        public static readonly CaptureTool.Pose[] CapturePoses =
        {
            new CaptureTool.Pose("01_nef_depuis_descente", new Vector3(-43.6f, YH + 1.65f, ZA), new Vector3(-4f, -90f, 0f)),
            new CaptureTool.Pose("02_plafond_dore_galerie", new Vector3(-46f, YH + 1.65f, ZA), new Vector3(-48f, -90f, 0f)),
            new CaptureTool.Pose("03_moucharabieh_et_rosaces", new Vector3(-47.5f, YH + 1.65f, ZA), new Vector3(-22f, -90f, 0f)),
            new CaptureTool.Pose("04_bas_cote_nord_escalier", new Vector3(-43.9f, YH + 1.65f, 84.3f), new Vector3(2f, -100f, 0f)),
            new CaptureTool.Pose("05_bit_gaad", new Vector3(-56.2f, YH + 1.65f, ZA), new Vector3(4f, -90f, 0f)),
            new CaptureTool.Pose("06_vers_l_entree", new Vector3(-53.5f, YH + 1.65f, ZA - 1.5f), new Vector3(-8f, 80f, 0f)),
            new CaptureTool.Pose("07_vestibule", new Vector3(-37.1f, 1.65f, ZA), new Vector3(8f, -90f, 0f)),
        };

        // =====================================================================  VESTIBULE ET DESCENTE
        static void BuildVestibule(Transform root)
        {
            var g = BuildKit.Group("Vestibule", root);
            float hV = YV - YH + 0.2f;

            BuildKit.Box("Sol_Vestibule", g, new Vector3(-38.6f, YH * 0.5f, ZA), new Vector3(4f, -YH, 6.8f), Pal.SolLosanges(4f, 6.8f));
            BuildKit.Box("Sol_Descente", g, new Vector3(-41.8f, YH - 0.1f, ZA), new Vector3(3f, 0.2f, 3.2f), Pal.MarbreRose);
            MeshKit.WBox("Mur_Vestibule_Sud", g, new Vector3(-38.8f, YH + hV * 0.5f, 74.8f), new Vector3(4.6f, hV, 0.4f), Pal.Chaux);
            MeshKit.WBox("Mur_Vestibule_Nord", g, new Vector3(-38.8f, YH + hV * 0.5f, 81.2f), new Vector3(4.6f, hV, 0.4f), Pal.Chaux);
            MeshKit.WBox("Mur_Vestibule_Ouest_S", g, new Vector3(-40.8f, YH + hV * 0.5f, 75.45f), new Vector3(0.4f, hV, 1.7f), Pal.Chaux);
            MeshKit.WBox("Mur_Vestibule_Ouest_N", g, new Vector3(-40.8f, YH + hV * 0.5f, 80.55f), new Vector3(0.4f, hV, 1.7f), Pal.Chaux);
            MeshKit.WBox("Facade_Est_Interieur", g, new Vector3(-36.3f, YH + hV * 0.5f, ZA), new Vector3(0.6f, hV, 6.8f), Pal.Chaux);
            MeshKit.WBox("Mur_Descente_Sud", g, new Vector3(-41.95f, YH + hV * 0.5f, 76.5f), new Vector3(2.7f, hV, 0.4f), Pal.Chaux);
            MeshKit.WBox("Mur_Descente_Nord", g, new Vector3(-41.95f, YH + hV * 0.5f, 79.5f), new Vector3(2.7f, hV, 0.4f), Pal.Chaux);
            BuildKit.Box("Plafond_Vestibule", g, new Vector3(-39.95f, YV + 0.1f, ZA), new Vector3(6.9f, 0.2f, 7f), Pal.Chaux, 0f, false);

            PropsHall.PorteCloutee(g, new Vector3(-36.6f, 0f, ZA), 90f, 2.6f, 3.4f);
            MeshKit.WBox("Lambris_Sud", g, new Vector3(-38.6f, 0.65f, 75.02f), new Vector3(4f, 1.3f, 0.04f), Pal.MarbreBlancPanneau, 0f, false);
            MeshKit.WBox("Lambris_Nord", g, new Vector3(-38.6f, 0.65f, 80.98f), new Vector3(4f, 1.3f, 0.04f), Pal.MarbreBlancPanneau, 0f, false);
            BuildKit.Arch("Arc_Vestibule", g, new Vector3(-39.2f, 0f, ZA), 90f, 5.4f, 1.6f, 0.4f, 0.25f, Pal.Chaux, 0f, 18, true);
            PropsHall.LanterneSuspendue(g, new Vector3(-38.4f, YV, ZA), 0.9f, 0.45f, 1.4f, 7f);

            // Descente : 8 marches de marbre rose, lambris rose à filet noir, arcs blancs
            BuildKit.Stairs("Descente_Marbre_Rose", g, new Vector3(-43f, YH, ZA), 90f, 2.6f, 8, Rise, Tread, Pal.MarbreRose);
            PropsInterieur.Panneau(g, "Lambris_Descente_Sud", new Vector3(-41.95f, (YH + 1.2f) * 0.5f, 76.72f), 2.7f, 1.2f - YH, 0f, Pal.MarbreRose, true);
            PropsInterieur.Panneau(g, "Lambris_Descente_Nord", new Vector3(-41.95f, (YH + 1.2f) * 0.5f, 79.28f), 2.7f, 1.2f - YH, 0f, Pal.MarbreRose, true);
            BuildKit.Arch("Arc_Descente_1", g, new Vector3(-41.4f, 0f, ZA), 90f, 2.6f, 3.0f, 0.3f, 0.2f, Pal.Chaux, 0f, 12, false);
            BuildKit.Arch("Arc_Descente_2", g, new Vector3(-42.6f, 0f, ZA), 90f, 2.6f, 3.0f, 0.3f, 0.2f, Pal.Chaux, 0f, 12, false);

            BuildKit.Info(g, new Vector3(-38.6f, 1.2f, ZA), new Vector3(3.5f, 2.4f, 5.5f), "Le vestibule",
                "Bienvenue dans le palais Ennejma Ezzahra, construit de 1912 à 1922 par le baron Rodolphe d'Erlanger, " +
                "peintre et musicologue.\nDescendez les marches de marbre rose vers la grande nef.");
        }

        // =====================================================================  GRANDE NEF (rez-de-chaussée)
        static void BuildNef(Transform root)
        {
            var g = BuildKit.Group("Grande_Nef", root);

            MeshKit.WBox("Sol_Nef", g, new Vector3(-49.3f, YH - 0.1f, ZA), new Vector3(12f, 0.2f, 7.5f), Pal.SolDamier);

            // Mur est : grand pan percé de l'arc de la descente (de z = 86 à z = 70)
            MeshKit.ArcadeWall("Mur_Est_Nef", g, new Vector3(-43f, YH, 86f), 90f, 16f, YC - YH + 0.3f, 0.6f, 1, 2.6f, 3.6f, 15f,
                               Pal.StucTreillis, Pal.Stuc, 0.3f);
            for (int s = -1; s <= 1; s += 2)
                PropsHall.ColonnesJumelees(g, new Vector3(-43.5f, YH, ZA + s * 1.6f), 90f, 3.6f, 0.1f, Pal.MarbreColonne, 0.3f);

            // Arcades basses nord et sud : 4 arcs outrepassés sur colonnes jumelées de marbre rose
            for (int s = -1; s <= 1; s += 2)
            {
                float z = ZA + s * 4f;
                var piers = MeshKit.ArcadeWall(s < 0 ? "Arcade_Basse_Sud" : "Arcade_Basse_Nord", g, new Vector3(XE, YH, z), 180f,
                                               12f, YG - YH, 0.5f, 4, 2.0f, 3.0f, 12f, Pal.StucTreillis, Pal.Stuc, 0.28f);
                foreach (float px in piers)
                {
                    float x = XE - px;
                    PropsHall.ColonnesJumelees(g, new Vector3(x, YH, z - s * 0.42f), 0f, 3.0f, 0.1f, Pal.MarbreColonne);
                }
            }

            // Rigole d'eau sous verre et vasque à veilleuse
            BuildKit.Box("Rigole", g, new Vector3(-47.7f, YH + 0.004f, ZA), new Vector3(7.5f, 0.008f, 0.22f), Pal.EauSombre, 0f, false);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Rigole_Bordure", g, new Vector3(-47.7f, YH + 0.005f, ZA + s * 0.17f), new Vector3(7.5f, 0.01f, 0.12f), Pal.Marbre, 0f, false);
            PropsHall.Vasque(g, new Vector3(-52.2f, YH, ZA));

            // Lanterne de bronze au centre de la nef
            PropsHall.LanterneSuspendue(g, new Vector3(-48.8f, YC, ZA), 4.2f, 0.9f, 2.2f, 16f, true);

            BuildKit.Info(g, new Vector3(-46.3f, YH + 1.2f, ZA), new Vector3(3f, 2.4f, 6f), "La grande nef",
                "Arcs outrepassés sur colonnes jumelées de marbre, murs couverts de stuc ciselé (naqch hadida), " +
                "lambris d'onyx et de marbre : le décor a été réalisé par des artisans venus du Maroc et de Tunisie. " +
                "Levez les yeux vers la galerie et le plafond de bois doré.");
        }

        // =====================================================================  GALERIE HAUTE ET PLAFOND DORÉ
        static void BuildGalerieEtPlafond(Transform root)
        {
            var g = BuildKit.Group("Galerie_Haute", root);

            for (int s = -1; s <= 1; s += 2)
            {
                float zArc = ZA + s * 4f;
                float zOut = ZA + s * 8f;
                // Plancher de la galerie (plafond des bas-côtés)
                MeshKit.WBox("Plancher_Galerie", g, new Vector3(-49.3f, YG - 0.15f, ZA + s * 6f), new Vector3(12f, 0.3f, 4f), Pal.Chaux, 0f, false);
                BuildKit.Box("Bandeau_Galerie", g, new Vector3(-49.3f, YG - 0.15f, zArc - s * 0.27f), new Vector3(12f, 0.34f, 0.04f), Pal.FriseCalligraphie(12f, 0.34f), 0f, false);
                // Arcade haute : 6 arcs sur colonnettes blanches, balustrades de bois tourné
                var piers = MeshKit.ArcadeWall(s < 0 ? "Arcade_Haute_Sud" : "Arcade_Haute_Nord", g, new Vector3(XE, YG, zArc), 180f,
                                               12f, YC - YG + 0.3f, 0.4f, 6, 1.3f, 2.3f, 0f, Pal.StucTreillis, Pal.Stuc, 0.16f, false);
                for (int i = 0; i < piers.Length; i++)
                {
                    float x = XE - piers[i];
                    PropsHall.ColonnesJumelees(g, new Vector3(x, YG, zArc - s * 0.3f), 0f, 2.3f, 0.06f, Pal.Marbre, 0.2f);
                    if (i < piers.Length - 1)
                    {
                        float x2 = XE - piers[i + 1];
                        PropsHall.Balustrade(g, new Vector3(x - 0.32f, YG, zArc), new Vector3(x2 + 0.32f, YG, zArc));
                    }
                }
                // Portes du fond de la galerie (le mur est celui des bas-côtés), plafond à lattes
                foreach (float x in new[] { -46.3f, -52.3f })
                    PropsInterieur.PorteSculptee(g, new Vector3(x, YG, zOut - s * 0.25f), s < 0 ? 180f : 0f, 1.5f, 2.4f);
                MeshKit.WBox("Plafond_Galerie", g, new Vector3(-49.3f, YGc + 0.1f, ZA + s * 6f), new Vector3(12f, 0.2f, 4f), Pal.Lattes, 0f, false);
                // Fenêtres hautes de la nef (claire-voie)
                for (int i = 0; i < 5; i++)
                    PropsHall.FenetreHaute(g, new Vector3(-45.3f - i * 2f, YGc + 0.45f, zArc - s * 0.2f), s < 0 ? 180f : 0f, 0.55f, 1.05f);
            }

            // Passerelle est, au-dessus de la descente
            MeshKit.WBox("Passerelle_Est", g, new Vector3(-44.1f, YG - 0.15f, ZA), new Vector3(1.6f, 0.3f, 7.5f), Pal.Chaux, 0f, false);
            BuildKit.Box("Bandeau_Passerelle", g, new Vector3(-44.92f, YG - 0.15f, ZA), new Vector3(0.04f, 0.34f, 7.5f), Pal.FriseCalligraphie(7.5f, 0.34f), 0f, false);
            PropsHall.Balustrade(g, new Vector3(-44.85f, YG, 74.4f), new Vector3(-44.85f, YG, 81.6f));

            // Plafond doré de la nef, frise dorée et frise de stuc calligraphiée sous le plafond
            BuildKit.Box("Plafond_Dore", g, new Vector3(-49.3f, YC + 0.05f, ZA), new Vector3(12f, 0.1f, 7.5f), Pal.PlafondDore(new Vector2(1.5f, 2.5f)), 0f, false);
            const float hf = 0.7f, hc = 0.34f;
            for (int s = -1; s <= 1; s += 2)
            {
                BuildKit.Box("Frise_Doree", g, new Vector3(-49.3f, YC - hf * 0.5f, ZA + s * 3.72f), new Vector3(12f, hf, 0.04f), Pal.FriseDoree(12f, hf), 0f, false);
                BuildKit.Box("Frise_Calligraphie", g, new Vector3(-49.3f, YC - hf - hc * 0.5f, ZA + s * 3.72f), new Vector3(12f, hc, 0.04f), Pal.FriseCalligraphie(12f, hc), 0f, false);
                float x = s < 0 ? XE - 0.02f : XW + 0.02f;
                BuildKit.Box("Frise_Doree", g, new Vector3(x, YC - hf * 0.5f, ZA), new Vector3(0.04f, hf, 7.5f), Pal.FriseDoree(7.5f, hf), 0f, false);
                BuildKit.Box("Frise_Calligraphie", g, new Vector3(x, YC - hf - hc * 0.5f, ZA), new Vector3(0.04f, hc, 7.5f), Pal.FriseCalligraphie(7.5f, hc), 0f, false);
            }
            BuildKit.PointLight("Jour_Claire_Voie_Nord", g, new Vector3(-49.3f, 8.4f, 80.6f), new Color(1f, 0.93f, 0.8f), 1.4f, 12f);
            BuildKit.PointLight("Jour_Claire_Voie_Sud", g, new Vector3(-49.3f, 8.4f, 75.4f), new Color(1f, 0.93f, 0.8f), 1.4f, 12f);
        }

        // =====================================================================  BAS-CÔTÉS (nord / sud) ET ESCALIERS
        static void BuildBasCote(Transform root, bool nord)
        {
            float s = nord ? 1f : -1f;
            var g = BuildKit.Group(nord ? "Bas_Cote_Nord" : "Bas_Cote_Sud", root);
            float zc = ZA + s * 6f;              // milieu du bas-côté
            float zWall = ZA + s * 8f;           // axe du mur extérieur
            float zFace = zWall - s * 0.25f;     // face intérieure du mur
            float zStair = zFace - s * 0.8f;     // axe de l'escalier (1,5 m de large)
            float hWall = YGc + 0.2f - YH;
            const float xs0 = -46.6f;            // pied de l'escalier
            float xs1 = xs0 - AisleSteps * Tread;   // −50,2 : haut de l'escalier
            const float xd0 = -50.3f, xd1 = -51.6f; // baie vers la cage d'escalier

            MeshKit.WBox("Sol", g, new Vector3(-49.3f, YH - 0.1f, zc), new Vector3(12f, 0.2f, 4f), Pal.SolDamier);

            // Mur extérieur percé d'une baie au niveau du palier
            MeshKit.WBox("Mur_Exterieur_E", g, new Vector3((XE + xd0) * 0.5f, YH + hWall * 0.5f, zWall), new Vector3(XE - xd0, hWall, 0.5f), Pal.Stuc);
            MeshKit.WBox("Mur_Exterieur_O", g, new Vector3((xd1 + XW) * 0.5f, YH + hWall * 0.5f, zWall), new Vector3(xd1 - XW, hWall, 0.5f), Pal.Stuc);
            MeshKit.WBox("Mur_Sous_Baie", g, new Vector3((xd0 + xd1) * 0.5f, (YH + YL) * 0.5f, zWall), new Vector3(xd0 - xd1, YL - YH, 0.5f), Pal.Stuc);
            float yLintel = YL + 2.5f;
            MeshKit.WBox("Linteau_Baie", g, new Vector3((xd0 + xd1) * 0.5f, (yLintel + YH + hWall) * 0.5f, zWall), new Vector3(xd0 - xd1, YH + hWall - yLintel, 0.5f), Pal.Stuc);

            // Lambris d'onyx rouge, bandeau de marbre vert sombre
            foreach (var (a, b) in new[] { (XE, xd0), (xd1, XW) })
            {
                float len = a - b, xm = (a + b) * 0.5f;
                MeshKit.WBox("Lambris_Onyx", g, new Vector3(xm, YH + 1.25f, zFace - s * 0.03f), new Vector3(len, 2.5f, 0.06f), Pal.OnyxRouge, 0f, false);
                MeshKit.WBox("Bandeau_Marbre_Vert", g, new Vector3(xm, YH + 2.62f, zFace - s * 0.035f), new Vector3(len, 0.24f, 0.07f), Pal.MarbreVert, 0f, false);
            }

            // Escalier de marbre rose (12 marches) et palier
            BuildKit.Stairs("Escalier_Marbre_Rose", g, new Vector3(xs0, YH, zStair), -90f, 1.5f, AisleSteps, Rise, Tread, Pal.MarbreRose);
            BuildKit.Box("Palier", g, new Vector3((xs1 + xd1 - 0.15f) * 0.5f, (YH + YL) * 0.5f, zStair), new Vector3(xs1 - xd1 + 0.15f, YL - YH, 1.5f), Pal.MarbreRose);
            float zPar = zStair - s * 0.82f;
            const int seg = 9;
            float runEnd = xd1 - 0.15f;
            float segLen = (xs0 - runEnd) / seg;
            for (int k = 0; k < seg; k++)
            {
                float xm = xs0 - (k + 0.5f) * segLen;
                float steps = Mathf.Clamp((xs0 - xm) / Tread, 0f, AisleSteps);
                float top = YH + steps * Rise + 0.95f;
                BuildKit.Box("Muret", g, new Vector3(xm, (YH + top) * 0.5f, zPar), new Vector3(segLen + 0.01f, top - YH, 0.16f), Pal.Marbre);
            }
            BuildKit.Box("Muret_Palier", g, new Vector3(runEnd - 0.08f, (YH + YL + 0.95f) * 0.5f, zStair), new Vector3(0.16f, YL + 0.95f - YH, 1.6f), Pal.Marbre);

            // Cage d'escalier derrière la baie (la suite de la montée), et transition vers l'étage
            float zCage = zWall + s * 1.1f;
            BuildKit.Box("Sol_Cage", g, new Vector3((xd0 + xd1) * 0.5f, (YH + YL) * 0.5f, zCage), new Vector3(xd0 - xd1 + 0.4f, YL - YH, 1.8f), Pal.MarbreRose);
            BuildKit.Stairs("Escalier_Suite", g, new Vector3((xd0 + xd1) * 0.5f, YL, zWall + s * 1.0f), nord ? 0f : 180f, 1.2f, 4, Rise, Tread, Pal.MarbreRose, false);
            MeshKit.WBox("Cage_Mur_E", g, new Vector3(xd0 + 0.2f, YL + 1.6f, zCage), new Vector3(0.4f, 3.2f, 2f), Pal.Chaux);
            MeshKit.WBox("Cage_Mur_O", g, new Vector3(xd1 - 0.2f, YL + 1.6f, zCage), new Vector3(0.4f, 3.2f, 2f), Pal.Chaux);
            MeshKit.WBox("Cage_Fond", g, new Vector3((xd0 + xd1) * 0.5f, YL + 1.6f, zWall + s * 2.2f), new Vector3(2.1f, 3.2f, 0.4f), Pal.Chaux);
            BuildKit.Box("Cage_Plafond", g, new Vector3((xd0 + xd1) * 0.5f, YL + 3.3f, zCage), new Vector3(2.1f, 0.2f, 2.2f), Pal.Chaux, 0f, false);
            PropsInterieur.LanterneMurale(g, new Vector3((xd0 + xd1) * 0.5f, YL + 2.4f, zWall + s * 2.0f), nord ? 0f : 180f, 1.2f, 5f);
            BuildKit.Transition(g, new Vector3((xd0 + xd1) * 0.5f, YL + 1.2f, zWall + s * 0.55f), new Vector3(1.2f, 2.4f, 0.5f), NextScene, nord ? "FromHall" : "FromHallSud");

            // Mobilier : divans rouges, tapis, niche de marbre noir (nord), lampes de bronze
            float yawWall = nord ? 0f : 180f;
            PropsInterieur.Divan(g, new Vector3(-45.3f, YH, zFace - s * 0.45f), yawWall, 2.4f, 0.8f, Pal.VeloursRouge);
            PropsInterieur.Tapis(g, new Vector3(-45f, YH, zc - s * 0.2f), 90f, 1.6f, 2.6f);
            if (nord)
                PropsHall.NicheMarbreNoir(g, new Vector3(-53.6f, YH, zFace), 0f);
            else
            {
                PropsInterieur.Divan(g, new Vector3(-53.5f, YH, zFace - s * 0.45f), yawWall, 2.8f, 0.8f, Pal.VeloursRouge);
                PropsInterieur.TableHexagonale(g, new Vector3(-55.05f, YH, zFace - s * 1.2f));
            }
            PropsInterieur.TableHexagonale(g, new Vector3(-43.75f, YH, zFace - s * 1.2f));
            foreach (float x in new[] { -45.2f, -53.6f })
                PropsInterieur.LustreDisque(g, new Vector3(x, YG - 0.3f, zc - s * 0.4f), 1.2f, 0.55f, 1.3f, 7f, false, Pal.Bronze);

            if (nord)
                BuildKit.Info(g, new Vector3(-45.8f, YH + 1.2f, zStair), new Vector3(1.4f, 2.4f, 1.5f), "Le baron d'Erlanger et la musique",
                    "Rodolphe d'Erlanger (1872-1932) a consacré sa vie à l'étude de la musique arabe : son grand ouvrage, " +
                    "« La Musique arabe », compte six volumes. Aujourd'hui, le palais abrite le Centre des musiques arabes " +
                    "et méditerranéennes.\nMontez l'escalier vers l'étage.");
        }

        // =====================================================================  FOND DE LA NEF : PORTE DU BIT GAÂD, ROSACES, MOUCHARABIEHS
        static void BuildFondOuest(Transform root)
        {
            var g = BuildKit.Group("Fond_Ouest", root);
            const float xw = -55.55f, xf = XW + 0.03f;
            const float dw = 2f, dh = 2.9f;

            // Mur bas (rez-de-chaussée), percé de la porte du bit gaâd
            MeshKit.WBox("Mur_Bas_S", g, new Vector3(xw, YH + (YG - YH) * 0.5f, (70f + ZA - dw * 0.5f) * 0.5f), new Vector3(0.5f, YG - YH, ZA - dw * 0.5f - 70f), Pal.StucTreillis);
            MeshKit.WBox("Mur_Bas_N", g, new Vector3(xw, YH + (YG - YH) * 0.5f, (86f + ZA + dw * 0.5f) * 0.5f), new Vector3(0.5f, YG - YH, 86f - ZA - dw * 0.5f), Pal.StucTreillis);
            MeshKit.WBox("Linteau", g, new Vector3(xw, (YH + dh + YG) * 0.5f, ZA), new Vector3(0.5f, YG - YH - dh, dw), Pal.StucTreillis);
            // Mur haut, de la galerie au plafond
            MeshKit.WBox("Mur_Haut", g, new Vector3(xw, (YG + YC + 0.3f) * 0.5f, ZA), new Vector3(0.5f, YC + 0.3f - YG, 16f), Pal.StucTreillis, 0f, false);

            // Encadrement de marbre, panneaux de marbre blanc, rosaces de stuc
            BuildKit.Box("Encadrement_G", g, new Vector3(xf - 0.03f, YH + dh * 0.5f, ZA - dw * 0.5f - 0.1f), new Vector3(0.08f, dh, 0.2f), Pal.Marbre, 0f, false);
            BuildKit.Box("Encadrement_D", g, new Vector3(xf - 0.03f, YH + dh * 0.5f, ZA + dw * 0.5f + 0.1f), new Vector3(0.08f, dh, 0.2f), Pal.Marbre, 0f, false);
            BuildKit.Box("Encadrement_H", g, new Vector3(xf - 0.03f, YH + dh + 0.1f, ZA), new Vector3(0.08f, 0.2f, dw + 0.4f), Pal.Marbre, 0f, false);
            for (int s = -1; s <= 1; s += 2)
            {
                float zp = ZA + s * (dw * 0.5f + 0.2f + 1.27f);
                MeshKit.WBox("Panneaux_Marbre", g, new Vector3(xf, YH + 1.2f, zp), new Vector3(0.04f, 2.4f, 2.55f), Pal.MarbreBlancPanneau, 0f, false);
                BuildKit.Box("Filet", g, new Vector3(xf - 0.01f, YH + 2.42f, zp), new Vector3(0.05f, 0.05f, 2.55f), Pal.MarbreVert, 0f, false);
                BuildKit.Box("Rosace", g, new Vector3(xf - 0.01f, YH + 3.5f, zp), new Vector3(1.7f, 1.7f, 0.03f), Pal.RosaceStuc, -90f, false);
            }
            // Poutre sculptée et galerie de moucharabiehs en bois rouge
            BuildKit.Box("Poutre_Sculptee", g, new Vector3(xf - 0.05f, YG - 0.2f, ZA), new Vector3(7.5f, 0.45f, 0.12f), Pal.FriseDoree(7.5f, 0.45f), -90f, false);
            BuildKit.Box("Moucharabieh", g, new Vector3(xf - 0.02f, YG + 0.25f + 1.7f, ZA), new Vector3(5.44f, 3.4f, 0.04f), Pal.MoucharabiehRouge, -90f, false);
            for (int s = -1; s <= 1; s += 2)
                BuildKit.Box("Moucharabieh_Cote", g, new Vector3(xf - 0.02f, YG + 0.25f + 1.7f, ZA + s * 3.25f), new Vector3(0.04f, 3.4f, 1.05f), Pal.BoisRouge, 0f, false);
            BuildKit.Box("Moucharabieh_Corniche", g, new Vector3(xf - 0.08f, YG + 3.75f, ZA), new Vector3(0.16f, 0.2f, 7.5f), Pal.BoisRouge, 0f, false);
            // Relief : montants sculptés en saillie, appui et balustrade de la loge
            for (int k = 0; k <= 6; k++)
                BuildKit.Box("Montant", g, new Vector3(xf - 0.07f, YG + 0.25f + 1.7f, ZA - 2.72f + k * (5.44f / 6f)), new Vector3(0.1f, 3.4f, 0.09f), Pal.BoisRouge, 0f, false);
            BuildKit.Box("Appui_Loge", g, new Vector3(xf - 0.15f, YG + 0.22f, ZA), new Vector3(0.3f, 0.1f, 7.5f), Pal.BoisRouge, 0f, false);
        }

        // =====================================================================  SALON « BIT GAÂD »
        static void BuildBitGaad(Transform root)
        {
            var g = BuildKit.Group("Bit_Gaad", root);
            const float x0 = -55.8f, x1 = -61.4f, z0 = 74.5f, z1 = 81.5f;
            float xc = (x0 + x1) * 0.5f, w = x0 - x1, d = z1 - z0;
            float h = YB - YH;

            BuildKit.Box("Sol", g, new Vector3(xc, YH - 0.1f, ZA), new Vector3(w, 0.2f, d), Pal.SolHexagones(w, d));
            MeshKit.WBox("Mur_Nord", g, new Vector3(xc, YH + (h + 0.6f) * 0.5f, z1 + 0.25f), new Vector3(w + 1f, h + 0.6f, 0.5f), Pal.StucTreillis);
            MeshKit.WBox("Mur_Sud", g, new Vector3(xc, YH + (h + 0.6f) * 0.5f, z0 - 0.25f), new Vector3(w + 1f, h + 0.6f, 0.5f), Pal.StucTreillis);
            MeshKit.WBox("Mur_Ouest", g, new Vector3(x1 - 0.3f, YH + (h + 0.6f) * 0.5f, ZA), new Vector3(0.6f, h + 0.6f, d + 1f), Pal.StucTreillis);
            PropsInterieur.PlafondCaissons(g, new Vector3(xc, YB, ZA), w, d, 1.4f);

            // Lambris de marbre blanc (panneaux) et filet sombre
            MeshKit.WBox("Lambris_N", g, new Vector3(xc, YH + 1.3f, z1 - 0.02f), new Vector3(w, 2.6f, 0.04f), Pal.MarbreBlancPanneau, 0f, false);
            MeshKit.WBox("Lambris_S", g, new Vector3(xc, YH + 1.3f, z0 + 0.02f), new Vector3(w, 2.6f, 0.04f), Pal.MarbreBlancPanneau, 0f, false);
            MeshKit.WBox("Lambris_O", g, new Vector3(x1 + 0.02f, YH + 1.3f, ZA), new Vector3(0.04f, 2.6f, d), Pal.MarbreBlancPanneau, 0f, false);
            BuildKit.Box("Filet_N", g, new Vector3(xc, YH + 2.63f, z1 - 0.03f), new Vector3(w, 0.06f, 0.05f), Pal.BoisSombre, 0f, false);
            BuildKit.Box("Filet_S", g, new Vector3(xc, YH + 2.63f, z0 + 0.03f), new Vector3(w, 0.06f, 0.05f), Pal.BoisSombre, 0f, false);
            BuildKit.Box("Filet_O", g, new Vector3(x1 + 0.03f, YH + 2.63f, ZA), new Vector3(0.05f, 0.06f, d), Pal.BoisSombre, 0f, false);

            // Fenêtre sur le jardin, vitrail en arc (qamariya) au-dessus
            PropsInterieur.FenetreInterieure(g, new Vector3(x1 + 0.01f, YH + 0.75f, ZA), -90f, 1.8f, 1.75f);
            PropsInterieur.Qamariya(g, new Vector3(x1 + 0.02f, YH + 2.7f, ZA), -90f, 1.6f, 0.8f);
            BuildKit.PointLight("Jour_Fenetre", g, new Vector3(x1 + 0.8f, YH + 1.8f, ZA), new Color(0.88f, 0.92f, 1f), 1.3f, 7f);

            // Colonnettes d'entrée, divans le long des murs, plateau de cuivre
            for (int s = -1; s <= 1; s += 2)
                PropsInterieur.ColonneMarbre(g, new Vector3(x0 - 0.35f, YH, ZA + s * 1.25f), 3.0f, 0.1f, Pal.Marbre);
            PropsInterieur.Divan(g, new Vector3(xc - 0.3f, YH, z1 - 0.45f), 0f, 4.4f, 0.85f);
            PropsInterieur.Divan(g, new Vector3(xc - 0.3f, YH, z0 + 0.45f), 180f, 4.4f, 0.85f);
            PropsInterieur.Divan(g, new Vector3(x1 + 0.45f, YH, ZA), -90f, 2.0f, 0.85f);
            PropsInterieur.TableHexagonale(g, new Vector3(x1 + 0.45f, YH, ZA - 1.45f));
            PropsInterieur.TableHexagonale(g, new Vector3(x1 + 0.45f, YH, ZA + 1.45f));
            PropsInterieur.Tapis(g, new Vector3(xc - 0.3f, YH, ZA), 90f, 2.4f, 3.4f);
            PropsInterieur.PlateauCuivre(g, new Vector3(xc - 0.4f, YH, ZA), 0.7f);
            PropsInterieur.LustreDisque(g, new Vector3(xc - 0.4f, YB, ZA), 1.2f, 0.6f, 1.6f, 8f);

            BuildKit.Info(g, new Vector3(xc + 1f, YH + 1.2f, ZA), new Vector3(2.5f, 2.4f, 5f), "Le bit gaâd",
                "Le « bit gaâd » est la pièce où l'on s'assoit pour recevoir : divans bas couverts de coussins, " +
                "grand plateau de cuivre ciselé, petites tables incrustées de nacre, lambris de marbre et vitrail en arc.");
        }
    }
}
