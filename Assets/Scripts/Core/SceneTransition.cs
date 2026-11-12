using UnityEngine;

namespace Ennejma
{
    /// <summary>
    /// Zone déclencheur : quand le visiteur y entre, on passe à la scène suivante
    /// (fondu au noir) et il apparaît au SpawnPoint indiqué.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SceneTransition : MonoBehaviour
    {
        public string targetScene = "02_Hall_Entree";
        public string targetSpawnId = "Default";
        public float fadeDuration = 0.8f;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (!Application.CanStreamedLevelBeLoaded(targetScene))
            {
                Debug.LogWarning($"[Ennejma] La scène '{targetScene}' n'existe pas encore ou n'est pas dans File > Build Settings.");
                return;
            }
            ScreenFader.Instance.LoadScene(targetScene, targetSpawnId, fadeDuration);
        }

        void OnDrawGizmos()
        {
            var box = GetComponent<BoxCollider>();
            if (box == null) return;
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.35f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}
