using UnityEditor;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// SCÈNE 2 — LE HALL D'ENTRÉE DU PALAIS ENNEJMA EZZAHRA
    ///
    /// Même repère que la scène 1 (+Z = nord, +X = vers la mer) : l'intérieur tient dans l'emprise du palais
    /// (x −61,7…−36,3 ; z 60,3…95,7). Parcours :
    ///   Porte (x = −36,3) → Vestibule (y = 0) → Descente de 8 marches en marbre rose
    ///   → Grand hall en contrebas (y = −1,28, plafond à caissons à y = 7,5)
    ///      ├─ au nord, derrière 3 arcs : salon « bit gaâd » (divans, plateau de cuivre)
    ///      ├─ au sud, derrière 3 arcs : salon de jeux (tables à tapis vert, chaises lyre)
    ///      └─ à l'ouest : escalier de marbre rose (14 marches) → palier → transition vers l'étage (scène 3)
    /// </summary>
    public static class Scene02_HallBuilder
    {
        public const string SceneName = "02_Hall_Entree";
        public const string PrevScene = "01_Exterieur";
        public const string NextScene = "03_Etage_Chambres";

        const float YH = -1.28f;          // sol du hall et des salons (8 marches de 0,16 m sous le vestibule)
        const float YC = 7.5f;            // plafond du hall (8,78 m sous plafond)
        const float YS = YH + 5.2f;       // plafond des salons
        const float YV = 4.5f;            // plafond du vestibule et de la descente
        const float ZA = 78f;             // axe est-ouest de la visite (porte → escalier)
        const float Rise = 0.16f, Tread = 0.30f;
        const float YP = YH + 14 * Rise;  // palier de l'escalier (+0,96)

        static readonly float[] ArcadeX = { -55.5f, -50.5f, -45.5f };

        [MenuItem("Ennejma/Phase 1/Scène 2 - Hall d'entrée", priority = 2)]
        public static void Build()
        {
            var scene = BuildKit.NewScene();
            if (!scene.IsValid()) return;
            BuildKit.BeginBuild(1913);

            BuildKit.SetupInterior(new Color(0.55f, 0.52f, 0.48f), new Color(0.42f, 0.38f, 0.33f), new Color(0.25f, 0.22f, 0.19f), 0.35f);

            var root = new GameObject("=== HALL D'ENTREE ===").transform;
            BuildFloorsAndWalls(root);
            BuildVestibule(root);
            BuildHall(root);
            BuildStaircase(root);
            BuildSalonNord(root);
            BuildSalonSud(root);

            var gameplay = new GameObject("=== GAMEPLAY ===").transform;
            var spawnPos = new Vector3(-38.2f, 0.05f, ZA);
            BuildKit.Spawn(gameplay, spawnPos, -90f, "FromExterior");
            BuildKit.Spawn(gameplay, new Vector3(-51.3f, YH + 0.05f, ZA), 90f, "FromEtage");
            BuildKit.Transition(gameplay, new Vector3(-36.85f, 1.5f, ZA), new Vector3(0.5f, 3f, 2.8f), PrevScene, "FromInterior");
            BuildKit.Transition(gameplay, new Vector3(-58.9f, YP + 1.2f, ZA), new Vector3(0.8f, 2.4f, 2.6f), NextScene, "FromHall");
            BuildKit.CreatePlayer(spawnPos, -90f);

            BuildKit.SaveScene(scene, SceneName);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Ennejma Ezzahra",
                    "Scène 2 (hall d'entrée) construite !\n\nAppuyez sur Play : descendez les marches de marbre rose vers le grand hall.",
                    "OK");
        }

        // =====================================================================  SOLS, MURS, PLAFONDS
        static void BuildFloorsAndWalls(Transform root)
        {
            var g = BuildKit.Group("Structure", root);
            float hFull = YC - YH + 0.5f;
            float hSalon = YS - YH + 0.3f;
            float hVest = YV - YH + 0.2f;

            // Sols
            BuildKit.Box("Sol_Hall", g, new Vector3(-50.5f, YH - 0.1f, ZA), new Vector3(15f, 0.2f, 20f), Pal.SolHexagones(15f, 20f));
            BuildKit.Box("Sol_Salon_Nord", g, new Vector3(-50.5f, YH - 0.1f, 91.85f), new Vector3(15f, 0.2f, 7.7f), Pal.SolHexagones(15f, 7.7f));
            BuildKit.Box("Sol_Salon_Sud", g, new Vector3(-50.5f, YH - 0.1f, 64.15f), new Vector3(15f, 0.2f, 7.7f), Pal.SolHexagones(15f, 7.7f));
            BuildKit.Box("Sol_Vestibule", g, new Vector3(-38.6f, YH * 0.5f, ZA), new Vector3(4f, -YH, 6.8f), Pal.SolLosanges(4f, 6.8f));
            BuildKit.Box("Sol_Descente", g, new Vector3(-41.8f, YH - 0.1f, ZA), new Vector3(3f, 0.2f, 3.2f), Pal.MarbreRose);

            // Hall : mur est (arc de la descente), mur ouest (arc de l'escalier), arcades nord et sud
            BuildKit.WallWithArch("Mur_Est_Hall", g, new Vector3(-43f, YH, ZA), 90f, 20.6f, hFull, 0.6f, 2.6f, 3.6f, Pal.Chaux, Pal.Chaux, 18f);
            BuildKit.WallWithArch("Mur_Ouest_Hall", g, new Vector3(-58f, YH, ZA), 90f, 20.6f, hFull, 0.6f, 2.8f, 4.2f, Pal.Chaux, Pal.Chaux, 18f);
            for (int i = 0; i < 3; i++)
            {
                BuildKit.WallWithArch("Arcade_Nord_" + (i + 1), g, new Vector3(ArcadeX[i], YH, 88f), 0f, 5f, hFull, 0.6f, 3.2f, 3.0f, Pal.Chaux, Pal.Chaux, 18f);
                BuildKit.WallWithArch("Arcade_Sud_" + (i + 1), g, new Vector3(ArcadeX[i], YH, 68f), 0f, 5f, hFull, 0.6f, 3.2f, 3.0f, Pal.Chaux, Pal.Chaux, 18f);
            }

            // Salons : murs latéraux et murs extérieurs
            BuildKit.Wall("Mur_Salon_Nord_Ouest", g, new Vector3(-58f, YH, 88f), new Vector3(-58f, YH, 96f), hSalon, 0.6f, Pal.Chaux);
            BuildKit.Wall("Mur_Salon_Nord_Est", g, new Vector3(-43f, YH, 88f), new Vector3(-43f, YH, 96f), hSalon, 0.6f, Pal.Chaux);
            BuildKit.Wall("Mur_Salon_Sud_Ouest", g, new Vector3(-58f, YH, 60f), new Vector3(-58f, YH, 68f), hSalon, 0.6f, Pal.Chaux);
            BuildKit.Wall("Mur_Salon_Sud_Est", g, new Vector3(-43f, YH, 60f), new Vector3(-43f, YH, 68f), hSalon, 0.6f, Pal.Chaux);
            BuildKit.Wall("Mur_Exterieur_Nord", g, new Vector3(-58.3f, YH, 95.7f), new Vector3(-42.7f, YH, 95.7f), hSalon, 0.6f, Pal.Chaux);
            BuildKit.Wall("Mur_Exterieur_Sud", g, new Vector3(-58.3f, YH, 60.3f), new Vector3(-42.7f, YH, 60.3f), hSalon, 0.6f, Pal.Chaux);

            // Vestibule et descente
            BuildKit.Wall("Mur_Vestibule_Sud", g, new Vector3(-41f, YH, 74.8f), new Vector3(-36.3f, YH, 74.8f), hVest, 0.4f, Pal.Chaux);
            BuildKit.Wall("Mur_Vestibule_Nord", g, new Vector3(-41f, YH, 81.2f), new Vector3(-36.3f, YH, 81.2f), hVest, 0.4f, Pal.Chaux);
            BuildKit.Wall("Mur_Vestibule_Ouest_S", g, new Vector3(-40.8f, YH, 74.6f), new Vector3(-40.8f, YH, 76.3f), hVest, 0.4f, Pal.Chaux);
            BuildKit.Wall("Mur_Vestibule_Ouest_N", g, new Vector3(-40.8f, YH, 79.7f), new Vector3(-40.8f, YH, 81.4f), hVest, 0.4f, Pal.Chaux);
            BuildKit.Wall("Facade_Est_Interieur", g, new Vector3(-36.3f, YH, 74.6f), new Vector3(-36.3f, YH, 81.4f), hVest, 0.6f, Pal.Chaux);
            BuildKit.Wall("Mur_Descente_Sud", g, new Vector3(-43.3f, YH, 76.5f), new Vector3(-40.6f, YH, 76.5f), hVest, 0.4f, Pal.Chaux);
            BuildKit.Wall("Mur_Descente_Nord", g, new Vector3(-43.3f, YH, 79.5f), new Vector3(-40.6f, YH, 79.5f), hVest, 0.4f, Pal.Chaux);

            // Alcôve de l'escalier (derrière l'arc ouest)
            BuildKit.Wall("Mur_Alcove_Sud", g, new Vector3(-58.3f, YH, 76.3f), new Vector3(-61.6f, YH, 76.3f), 6.5f, 0.4f, Pal.Chaux);
            BuildKit.Wall("Mur_Alcove_Nord", g, new Vector3(-58.3f, YH, 79.7f), new Vector3(-61.6f, YH, 79.7f), 6.5f, 0.4f, Pal.Chaux);
            BuildKit.Wall("Mur_Alcove_Fond", g, new Vector3(-61.6f, YH, 76.1f), new Vector3(-61.6f, YH, 79.9f), 6.5f, 0.4f, Pal.Chaux);

            // Plafonds
            BuildKit.Box("Plafond_Vestibule", g, new Vector3(-39.95f, YV + 0.1f, ZA), new Vector3(6.9f, 0.2f, 7f), Pal.Chaux, 0f, false);
            BuildKit.Box("Plafond_Alcove", g, new Vector3(-59.95f, 4.9f, ZA), new Vector3(3.8f, 0.2f, 3.8f), Pal.Chaux, 0f, false);
            PropsInterieur.PlafondCaissons(g, new Vector3(-50.5f, YC, ZA), 14.4f, 19.4f, 2.0f, 0.3f);
            PropsInterieur.PlafondCaissons(g, new Vector3(-50.5f, YS, 91.85f), 14.4f, 7.1f, 1.5f);
            PropsInterieur.PlafondCaissons(g, new Vector3(-50.5f, YS, 64.15f), 14.4f, 7.1f, 1.5f);
        }

        // =====================================================================  VESTIBULE ET DESCENTE
        static void BuildVestibule(Transform root)
        {
            var g = BuildKit.Group("Vestibule", root);

            PropsInterieur.PorteSculptee(g, new Vector3(-36.6f, 0f, ZA), 90f, 2.8f, 3.3f);
            PropsInterieur.Panneau(g, "Lambris_Sud", new Vector3(-38.6f, 0.6f, 75.02f), 4f, 1.2f, 0f, Pal.MarbreRose, true);
            PropsInterieur.Panneau(g, "Lambris_Nord", new Vector3(-38.6f, 0.6f, 80.98f), 4f, 1.2f, 0f, Pal.MarbreRose, true);
            Props.Bench(g, new Vector3(-38.6f, 0f, 75.45f), 180f);
            PropsInterieur.LanterneMurale(g, new Vector3(-38.6f, 2.7f, 75f), 180f);
            PropsInterieur.LanterneMurale(g, new Vector3(-38.6f, 2.7f, 81f), 0f);

            // Descente : 8 marches de marbre rose, lambris rose à filet noir, arcs blancs
            BuildKit.Stairs("Descente_Marbre_Rose", g, new Vector3(-43f, YH, ZA), 90f, 2.6f, 8, Rise, Tread, Pal.MarbreRose);
            PropsInterieur.Panneau(g, "Lambris_Descente_Sud", new Vector3(-41.95f, (YH + 1.2f) * 0.5f, 76.72f), 2.7f, 1.2f - YH, 0f, Pal.MarbreRose, true);
            PropsInterieur.Panneau(g, "Lambris_Descente_Nord", new Vector3(-41.95f, (YH + 1.2f) * 0.5f, 79.28f), 2.7f, 1.2f - YH, 0f, Pal.MarbreRose, true);
            BuildKit.Arch("Arc_Descente_1", g, new Vector3(-41.4f, 0f, ZA), 90f, 2.6f, 3.0f, 0.3f, 0.2f, Pal.Chaux, 0f, 12, false);
            BuildKit.Arch("Arc_Descente_2", g, new Vector3(-42.6f, 0f, ZA), 90f, 2.6f, 3.0f, 0.3f, 0.2f, Pal.Chaux, 0f, 12, false);

            BuildKit.Info(g, new Vector3(-38.6f, 1.2f, ZA), new Vector3(3.5f, 2.4f, 5.5f), "Le vestibule",
                "Bienvenue dans le palais Ennejma Ezzahra, « l'étoile de Vénus », construit de 1912 à 1922 par le baron " +
                "Rodolphe d'Erlanger, peintre et musicologue.\nDescendez les marches de marbre rose vers le grand hall.");
        }

        // =====================================================================  GRAND HALL
        static void BuildHall(Transform root)
        {
            var g = BuildKit.Group("Grand_Hall", root);

            // Frise de stuc ciselé sous le plafond (y 6,5 → 7,5)
            float yF = YC - 0.5f;
            PropsInterieur.FriseStuc(g, new Vector3(-50.5f, yF, 87.67f), 14.4f, 1f, 0f);
            PropsInterieur.FriseStuc(g, new Vector3(-50.5f, yF, 68.33f), 14.4f, 1f, 0f);
            PropsInterieur.FriseStuc(g, new Vector3(-43.33f, yF, ZA), 19.4f, 1f, 90f);
            PropsInterieur.FriseStuc(g, new Vector3(-57.67f, yF, ZA), 19.4f, 1f, 90f);

            // Qamariyas (vitraux en arc) au-dessus des arcs
            foreach (float x in ArcadeX)
            {
                PropsInterieur.Qamariya(g, new Vector3(x, 4.35f, 87.68f), 0f, 1.6f, 1.0f);
                PropsInterieur.Qamariya(g, new Vector3(x, 4.35f, 68.32f), 180f, 1.6f, 1.0f);
            }
            foreach (float z in new[] { 72f, ZA, 84f })
                PropsInterieur.Qamariya(g, new Vector3(-43.32f, 4.5f, z), 90f, 1.6f, 1.0f);
            foreach (float z in new[] { 72f, 84f })
                PropsInterieur.Qamariya(g, new Vector3(-57.68f, 4.5f, z), -90f, 1.6f, 1.0f);

            // Colonnes de marbre rose devant les arcades et de part et d'autre de la descente
            foreach (float x in ArcadeX)
                for (int s = -1; s <= 1; s += 2)
                {
                    PropsInterieur.ColonneMarbre(g, new Vector3(x + s * 1.76f, YH, 87.5f), 3.0f, 0.13f, Pal.MarbreRose);
                    PropsInterieur.ColonneMarbre(g, new Vector3(x + s * 1.76f, YH, 68.5f), 3.0f, 0.13f, Pal.MarbreRose);
                }
            for (int s = -1; s <= 1; s += 2)
                PropsInterieur.ColonneMarbre(g, new Vector3(-43.5f, YH, ZA + s * 1.46f), 3.6f, 0.13f, Pal.MarbreRose);

            // Lustre central, tapis, lumière du jour tombant des vitraux
            PropsInterieur.LustreDisque(g, new Vector3(-48.5f, YC, ZA), 3.8f, 1.3f, 3f, 20f, true);
            PropsInterieur.Tapis(g, new Vector3(-47.8f, YH, ZA), 90f, 3.2f, 7f);
            BuildKit.PointLight("Jour_Vitraux_Nord", g, new Vector3(-50.5f, 5.8f, 86f), new Color(0.85f, 0.9f, 1f), 1.2f, 12f);
            BuildKit.PointLight("Jour_Vitraux_Sud", g, new Vector3(-50.5f, 5.8f, 70f), new Color(0.85f, 0.9f, 1f), 1.2f, 12f);
            PropsInterieur.LanterneMurale(g, new Vector3(-43.3f, YH + 2.6f, 73f), 90f);
            PropsInterieur.LanterneMurale(g, new Vector3(-43.3f, YH + 2.6f, 83f), 90f);

            // Divans le long du mur ouest, de part et d'autre de l'escalier
            foreach (float z in new[] { 71.6f, 84.4f })
            {
                PropsInterieur.Divan(g, new Vector3(-57.25f, YH, z), -90f, 3f);
                PropsInterieur.TableHexagonale(g, new Vector3(-56.2f, YH, z + (z < ZA ? 2f : -2f)));
            }

            // Le coffre peint et le téléphone de campagne (futur premier indice), contre le pilier nord
            float top = PropsInterieur.CoffrePeint(g, new Vector3(-48f, YH, 87.1f), 0f);
            PropsInterieur.TelephoneCampagne(g, new Vector3(-47.75f, YH + top, 87.1f), 0f);
            PropsInterieur.ChandelierFer(g, new Vector3(-48.3f, YH + top, 87.18f));

            BuildKit.Info(g, new Vector3(-46.3f, YH + 1.2f, ZA), new Vector3(3f, 2.4f, 6f), "Le grand hall",
                "Levez les yeux : le plafond en bois sculpté à caissons et les stucs ciselés (naqch hadida) ont été réalisés " +
                "par des artisans venus du Maroc et de Tunisie. Les vitraux en arc, les qamariya, colorent la lumière du jour.");
        }

        // =====================================================================  ESCALIER DE MARBRE ROSE
        static void BuildStaircase(Transform root)
        {
            var g = BuildKit.Group("Escalier", root);
            const float x0 = -52.6f;

            BuildKit.Stairs("Escalier_Marbre_Rose", g, new Vector3(x0, YH, ZA), -90f, 2.4f, 14, Rise, Tread, Pal.MarbreRose);
            BuildKit.Box("Palier", g, new Vector3(-59.1f, (YH + YP) * 0.5f, ZA), new Vector3(4.6f, YP - YH, 3f), Pal.MarbreRose);
            BuildKit.Stairs("Escalier_Suite", g, new Vector3(-60f, YP, ZA), -90f, 2.4f, 4, Rise, Tread, Pal.MarbreRose, false);

            // Murets blancs à chaperon de marbre, qui suivent la pente
            const int seg = 9;
            float segLen = (x0 - (-57.7f)) / seg;
            for (int s = -1; s <= 1; s += 2)
            {
                float z = ZA + s * 1.4f;
                for (int k = 0; k < seg; k++)
                {
                    float xm = x0 - (k + 0.5f) * segLen;
                    float steps = Mathf.Clamp((x0 - xm) / Tread, 0f, 14f);
                    float top = YH + steps * Rise + 0.9f;
                    BuildKit.Box("Muret", g, new Vector3(xm, (YH + top) * 0.5f, z), new Vector3(segLen + 0.01f, top - YH, 0.2f), Pal.Chaux);
                    BuildKit.Box("Chaperon", g, new Vector3(xm, top + 0.025f, z), new Vector3(segLen + 0.01f, 0.05f, 0.26f), Pal.Marbre, 0f, false);
                }
            }

            // Arc d'entrée sur colonnes de marbre rose
            for (int s = -1; s <= 1; s += 2)
                PropsInterieur.ColonneMarbre(g, new Vector3(-52.4f, YH, ZA + s * 1.75f), 3.4f, 0.14f, Pal.MarbreRose);
            BuildKit.Arch("Arc_Escalier", g, new Vector3(-52.4f, YH, ZA), 90f, 3.5f, 3.4f, 0.35f, 0.3f, Pal.Chaux, 15f, 16, false);
            Props.Pot(g, new Vector3(-51.7f, YH, ZA - 2.4f), 1f, true);
            Props.Pot(g, new Vector3(-51.7f, YH, ZA + 2.4f), 1f, true);
            PropsInterieur.LanterneMurale(g, new Vector3(-61.4f, 3.4f, ZA), -90f, 1.4f, 7f);

            BuildKit.Info(g, new Vector3(-51.4f, YH + 1.2f, ZA), new Vector3(1.4f, 2.4f, 3f), "Le baron d'Erlanger et la musique",
                "Rodolphe d'Erlanger (1872-1932) a consacré sa vie à l'étude de la musique arabe : son grand ouvrage, " +
                "« La Musique arabe », compte six volumes. Aujourd'hui, le palais abrite le Centre des musiques arabes " +
                "et méditerranéennes.\nMontez l'escalier vers l'étage.");
        }

        // =====================================================================  SALON NORD : BIT GAÂD
        static void BuildSalonNord(Transform root)
        {
            var g = BuildKit.Group("Salon_Nord_Bit_Gaad", root);
            const float zc = 91.85f, zWall = 95.36f;

            PropsInterieur.Panneau(g, "Lambris_Nord", new Vector3(-50.5f, YH + 1.2f, zWall + 0.02f), 14.4f, 2.4f, 0f, Pal.MarbreVeine);
            PropsInterieur.Panneau(g, "Lambris_Ouest", new Vector3(-57.68f, YH + 1.2f, zc), 7.1f, 2.4f, 90f, Pal.MarbreVeine);
            PropsInterieur.Panneau(g, "Lambris_Est", new Vector3(-43.32f, YH + 1.2f, zc), 7.1f, 2.4f, 90f, Pal.MarbreVeine);
            float yF = YS - 0.35f;
            PropsInterieur.FriseStuc(g, new Vector3(-50.5f, yF, zWall - 0.01f), 14.4f, 0.7f, 0f);
            PropsInterieur.FriseStuc(g, new Vector3(-50.5f, yF, 88.33f), 14.4f, 0.7f, 0f);
            PropsInterieur.FriseStuc(g, new Vector3(-57.67f, yF, zc), 7.1f, 0.7f, 90f);
            PropsInterieur.FriseStuc(g, new Vector3(-43.33f, yF, zc), 7.1f, 0.7f, 90f);

            foreach (float x in new[] { -54f, -47f })
            {
                PropsInterieur.FenetreInterieure(g, new Vector3(x, YH + 0.75f, zWall - 0.01f), 0f, 1.6f, 1.9f);
                PropsInterieur.Qamariya(g, new Vector3(x, YH + 2.85f, zWall - 0.02f), 0f, 1.4f, 0.7f);
                PropsInterieur.Divan(g, new Vector3(x, YH, 94.92f), 0f, 2.6f);
                PropsInterieur.TableHexagonale(g, new Vector3(x - 1.62f, YH, 94.95f));
                PropsInterieur.TableHexagonale(g, new Vector3(x + 1.62f, YH, 94.95f));
                BuildKit.PointLight("Jour_Fenetre", g, new Vector3(x, YH + 1.9f, 94.3f), new Color(0.85f, 0.9f, 1f), 1f, 6f);
            }
            PropsInterieur.Divan(g, new Vector3(-57.25f, YH, zc), -90f, 3.5f);
            PropsInterieur.Divan(g, new Vector3(-43.75f, YH, zc), 90f, 3.5f);

            PropsInterieur.Tapis(g, new Vector3(-50.5f, YH, 91.7f), 0f, 5.5f, 3.6f);
            PropsInterieur.PlateauCuivre(g, new Vector3(-50.5f, YH, 91.7f), 0.8f);
            PropsInterieur.LustreDisque(g, new Vector3(-50.5f, YS, 91.7f), 1.3f, 0.7f, 1.8f, 10f);

            BuildKit.Info(g, new Vector3(-50.5f, YH + 1.2f, 90f), new Vector3(12f, 2.4f, 3f), "Le bit gaâd",
                "Le « bit gaâd » est la pièce où l'on s'assoit pour recevoir : divans bas couverts de coussins, " +
                "grand plateau de cuivre ciselé pour le thé, petites tables incrustées de nacre. On y écoutait aussi la musique.");
        }

        // =====================================================================  SALON SUD : TABLES DE JEU
        static void BuildSalonSud(Transform root)
        {
            var g = BuildKit.Group("Salon_Sud_Jeux", root);
            const float zc = 64.15f, zWall = 60.64f;

            PropsInterieur.Panneau(g, "Lambris_Sud", new Vector3(-50.5f, YH + 1.2f, zWall - 0.02f), 14.4f, 2.4f, 0f, Pal.MarbreVeine);
            PropsInterieur.Panneau(g, "Lambris_Ouest", new Vector3(-57.68f, YH + 1.2f, zc), 7.1f, 2.4f, 90f, Pal.MarbreVeine);
            float yF = YS - 0.35f;
            PropsInterieur.FriseStuc(g, new Vector3(-50.5f, yF, zWall + 0.01f), 14.4f, 0.7f, 0f);
            PropsInterieur.FriseStuc(g, new Vector3(-50.5f, yF, 67.67f), 14.4f, 0.7f, 0f);
            PropsInterieur.FriseStuc(g, new Vector3(-57.67f, yF, zc), 7.1f, 0.7f, 90f);
            PropsInterieur.FriseStuc(g, new Vector3(-43.33f, yF, zc), 7.1f, 0.7f, 90f);

            foreach (float x in new[] { -54f, -47f })
            {
                PropsInterieur.FenetreInterieure(g, new Vector3(x, YH + 0.75f, zWall + 0.01f), 180f, 1.6f, 1.9f);
                PropsInterieur.Qamariya(g, new Vector3(x, YH + 2.85f, zWall + 0.02f), 180f, 1.4f, 0.7f);
                BuildKit.PointLight("Jour_Fenetre", g, new Vector3(x, YH + 1.9f, 61.5f), new Color(0.85f, 0.9f, 1f), 1f, 6f);
            }

            PropsInterieur.Tapis(g, new Vector3(-50.5f, YH, zc), 90f, 3.6f, 9f);
            int v = 0;
            foreach (float x in new[] { -53.5f, -47.5f })
            {
                var t = new Vector3(x, YH, zc);
                PropsInterieur.TableDeJeu(g, t, 0f, v++);
                PropsInterieur.ChaiseLyre(g, t + new Vector3(0f, 0f, -0.68f), 180f);
                PropsInterieur.ChaiseLyre(g, t + new Vector3(0f, 0f, 0.68f), 0f);
                PropsInterieur.ChaiseLyre(g, t + new Vector3(-0.68f, 0f, 0f), -90f);
            }
            PropsInterieur.HorlogeComtoise(g, new Vector3(-57.5f, YH, zc), -90f);
            PropsInterieur.PorteSculptee(g, new Vector3(-43.3f, YH, zc), 90f, 1.8f, 2.6f);
            PropsInterieur.LustreDisque(g, new Vector3(-50.5f, YS, zc), 1.3f, 0.7f, 1.8f, 10f);

            BuildKit.Info(g, new Vector3(-50.5f, YH + 1.2f, 66f), new Vector3(12f, 2.4f, 3f), "Le salon de jeux",
                "Tables à tapis vert, chaises cannées à dossier en lyre, horloge comtoise : le palais était aussi une " +
                "demeure de famille où le baron recevait ses invités.");
        }
    }
}
