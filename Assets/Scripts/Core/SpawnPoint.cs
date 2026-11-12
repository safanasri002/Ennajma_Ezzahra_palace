using UnityEngine;

namespace Ennejma
{
    /// <summary>
    /// Point d'apparition du visiteur à l'arrivée dans une scène.
    /// Ex. : "FromExterior" dans le hall, "FromInterior" devant la façade.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        public string spawnId = "Default";

        public static void PlacePlayer(FirstPersonController player)
        {
            string wanted = GameState.NextSpawnId;
            if (string.IsNullOrEmpty(wanted)) return;
            foreach (var sp in FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None))
            {
                if (sp.spawnId != wanted) continue;
                player.Teleport(sp.transform.position, sp.transform.eulerAngles.y);
                break;
            }
            GameState.NextSpawnId = null;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f);
            Vector3 head = transform.position + Vector3.up * 0.9f;
            Gizmos.DrawWireSphere(head, 0.4f);
            Gizmos.DrawRay(head, transform.forward * 1.2f);
        }
    }
}
