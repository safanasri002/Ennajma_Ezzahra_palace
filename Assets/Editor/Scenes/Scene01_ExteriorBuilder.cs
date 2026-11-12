using UnityEditor;
using UnityEngine;

namespace Ennejma.EditorTools
{
    /// <summary>
    /// SCÈNE 1 — L'EXTÉRIEUR DU PALAIS ENNEJMA EZZAHRA — VERSION V1 (CARCASSE)
    ///
    /// Maquette de base : uniquement les volumes principaux et quelques éléments
    /// représentatifs. Même parcours et mêmes coordonnées que la V2 :
    ///   Rue → Porte jaune (z=0) → Allée (z 0→55) → Esplanade → Façade du palais (x=-36)
    ///   → Coin de la fontaine → Porte du palais (transition vers la scène 2)
    ///
    /// Repère : +Z = sens de la visite, +X = vers la mer.
    /// </summary>
    public static class Scene01_ExteriorBuilder
    {
        public const string SceneName = "01_Exterieur";
        public const string NextScene = "02_Hall_Entree";

        const float SlopeDeg = 12f;

        [MenuItem("Ennejma/Phase 1/Scène 1 - Extérieur V1 (carcasse)", priority = 1)]
        public static void Build()
        {
            var scene = BuildKit.NewScene();
            if (!scene.IsValid()) return;
            BuildKit.BeginBuild(1912);

            BuildKit.SetupDaylight(new Vector3(42f, -115f, 0f), 1.3f);

            var root = new GameObject("=== EXTERIEUR (V1 carcasse) ===").transform;
            BuildGround(root);
            BuildEnclosure(root);
            BuildPalace(root);
            BuildFewProps(root);

            var gameplay = new GameObject("=== GAMEPLAY ===").transform;
            BuildKit.Spawn(gameplay, new Vector3(0f, 0.1f, -7f), 0f, "Default");
            BuildKit.Spawn(gameplay, new Vector3(-33f, 0.2f, 78f), 90f, "FromInterior");
            BuildKit.CreatePlayer(new Vector3(0f, 0.1f, -7f), 0f);

            BuildKit.SaveScene(scene, SceneName);
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Ennejma Ezzahra",
                    "Scène 1 (V1 carcasse) construite !\n\nAppuyez sur Play : ZQSD pour marcher, souris pour regarder, Maj pour courir, Échap pour libérer la souris.",
                    "OK");
        }

        // =====================================================================  SOL : zones du parcours + mer
        static void BuildGround(Transform root)
        {
            var g = BuildKit.Group("Sol", root);

            BuildKit.Box("Sol_Domaine", g, new Vector3(-27f, -0.5f, 44f), new Vector3(70f, 1f, 112f), Pal.Herbe);
            BuildKit.Box("Rue", g, new Vector3(-6f, 0.025f, -6f), new Vector3(28f, 0.05f, 12f), Pal.Pave);
            BuildKit.Box("Allee", g, new Vector3(0f, 0.025f, 27.5f), new Vector3(4.4f, 0.05f, 55f), Pal.Pave);
            BuildKit.Box("Esplanade", g, new Vector3(-16f, 0.025f, 75.75f), new Vector3(40f, 0.05f, 41.5f), Pal.Dallage);
            BuildKit.Box("Coin_Fontaine", g, new Vector3(-49f, 0.03f, 51.75f), new Vector3(22f, 0.06f, 15.5f), Pal.Dallage);

            // Pente vers la mer et la mer (simples plans)
            float half = 100f, a = SlopeDeg * Mathf.Deg2Rad;
            var slopeCenter = new Vector3(8f + half * Mathf.Cos(a), -1.5f - half * Mathf.Sin(a) - 0.5f, 50f);
            BuildKit.BoxE("Pente_vers_la_mer", g, slopeCenter, new Vector3(200f, 1f, 320f), Pal.Colline, new Vector3(0f, 0f, -SlopeDeg), false);
            BuildKit.Box("Golfe_de_Tunis", g, new Vector3(420f, -36f, 40f), new Vector3(800f, 1f, 1000f), Pal.Mer, 0f, false);

            BuildKit.InvisibleWall("Limite_Est", g, new Vector3(8.3f, 1.5f, 44f), new Vector3(0.4f, 4f, 112f));
            BuildKit.InvisibleWall("Limite_Nord", g, new Vector3(-27f, 1.5f, 100.2f), new Vector3(70f, 4f, 0.4f));
            BuildKit.InvisibleWall("Limite_Ouest", g, new Vector3(-62.2f, 1.5f, 44f), new Vector3(0.4f, 4f, 112f));
            BuildKit.InvisibleWall("Limite_Rue_Sud", g, new Vector3(-6f, 1.5f, -12.2f), new Vector3(28f, 4f, 0.4f));
            BuildKit.InvisibleWall("Limite_Rue_Ouest", g, new Vector3(-20.2f, 1.5f, -6f), new Vector3(0.4f, 4f, 12f));
        }

        // =====================================================================  ENCEINTE, PORTE JAUNE, PARAPETS
        static void BuildEnclosure(Transform root)
        {
            var g = BuildKit.Group("Enceinte", root);

            BuildKit.Wall("Enceinte_Sud_Ouest", g, new Vector3(-62f, 0f, 0f), new Vector3(-3f, 0f, 0f), 3.2f, 0.6f, Pal.Chaux);
            BuildKit.Wall("Enceinte_Sud_Est", g, new Vector3(3f, 0f, 0f), new Vector3(4.5f, 0f, 0f), 3.2f, 0.6f, Pal.Chaux);
            BuildKit.Wall("Enceinte_Ouest", g, new Vector3(-62f, 0f, 0f), new Vector3(-62f, 0f, 60f), 3.2f, 0.6f, Pal.Chaux);

            // Porte jaune : l'arc et deux battants unis
            BuildKit.WallWithArch("Portail", g, Vector3.zero, 0f, 6f, 5.2f, 0.8f, 3.0f, 2.7f, Pal.Chaux, Pal.Pierre, 18f);
            for (int s = -1; s <= 1; s += 2)
            {
                var hinge = BuildKit.Group(s < 0 ? "Battant_Gauche" : "Battant_Droit", g,
                                           new Vector3(s * 1.42f, 0.05f, 0.45f), new Vector3(0f, s * 75f, 0f));
                BuildKit.Box("Vantail", hinge, new Vector3(-s * 0.71f, 1.35f, 0f), new Vector3(1.42f, 2.7f, 0.1f), Pal.JaunePorte);
            }

            // Parapets côté mer (allée + esplanade)
            BuildKit.Wall("Parapet_Allee", g, new Vector3(2.5f, 0f, 0.4f), new Vector3(2.5f, 0f, 55f), 0.85f, 0.4f, Pal.Chaux);
            BuildKit.Wall("Parapet_Coude", g, new Vector3(2.5f, 0f, 55f), new Vector3(4.4f, 0f, 55f), 0.85f, 0.4f, Pal.Chaux);
            BuildKit.Wall("Parapet_Est", g, new Vector3(4.4f, 0f, 55f), new Vector3(4.4f, 0f, 96.7f), 0.85f, 0.4f, Pal.Chaux);
            BuildKit.Wall("Parapet_Nord", g, new Vector3(-36f, 0f, 96.7f), new Vector3(4.4f, 0f, 96.7f), 0.85f, 0.4f, Pal.Chaux);

            BuildKit.Info(g, new Vector3(0f, 1.5f, -2.5f), new Vector3(6f, 3f, 3f), "La porte jaune",
                "Bienvenue au palais Ennejma Ezzahra (1912-1922), demeure du baron Rodolphe d'Erlanger.\nAvancez dans l'allée.");
        }

        // =====================================================================  LE PALAIS : volume + porte
        static void BuildPalace(Transform root)
        {
            var g = BuildKit.Group("Palais", root);
            const float doorZ = 78f;

            BuildKit.Box("Volume_Principal", g, new Vector3(-49.3f, 6.25f, 78f), new Vector3(25.4f, 12.5f, 36f), Pal.Chaux);
            BuildKit.WallWithArch("Facade_Est", g, new Vector3(-36.3f, 0f, doorZ), -90f, 36f, 12.5f, 0.6f, 3.2f, 3.4f, Pal.Chaux, Pal.Marbre, 20f);
            BuildKit.Box("Porte_Bois", g, new Vector3(-36.63f, 2.8f, doorZ), new Vector3(0.1f, 5.6f, 3.4f), Pal.BoisSombre);
            BuildKit.Box("Marche", g, new Vector3(-35.3f, 0.1f, doorZ), new Vector3(1.4f, 0.2f, 4.6f), Pal.Marbre);

            BuildKit.Info(g, new Vector3(-33.5f, 1.5f, doorZ), new Vector3(3f, 3f, 6f), "Entrée principale du palais",
                "Franchissez la grande porte pour entrer dans le palais.");
            BuildKit.Transition(g, new Vector3(-36.25f, 1.6f, doorZ), new Vector3(0.5f, 2.4f, 2.8f), NextScene, "FromExterior");
        }

        // =====================================================================  QUELQUES ÉLÉMENTS REPRÉSENTATIFS
        static void BuildFewProps(Transform root)
        {
            var g = BuildKit.Group("Elements", root);

            // Jardin : un arbre de chaque espèce
            Props.Palm(g, new Vector3(-12f, 0f, 15f));
            Props.Cypress(g, new Vector3(-22f, 0f, 30f));
            Props.OrangeTree(g, new Vector3(-35f, 0f, 20f));
            Props.OliveTree(g, new Vector3(-48f, 0f, 32f));

            // Esplanade : un banc face à la mer
            Props.Bench(g, new Vector3(3.3f, 0.05f, 76f), -90f);

            // Coin de la fontaine
            Props.Fountain(g, new Vector3(-49f, 0f, 51.5f));

            // Sidi Bou Saïd : quelques maisons sur la pente
            var viewer = new Vector3(0f, 0f, 45f);
            float tan = Mathf.Tan(SlopeDeg * Mathf.Deg2Rad);
            foreach (var xz in new[] { new Vector2(25f, 10f), new Vector2(35f, 40f), new Vector2(30f, 75f), new Vector2(50f, 25f), new Vector2(55f, 60f) })
            {
                var p = new Vector3(xz.x, -1.5f - (xz.x - 8f) * tan - 0.8f, xz.y);
                Props.SbsHouse(g, p, BuildKit.FaceYaw(p, viewer));
            }
        }
    }
}
