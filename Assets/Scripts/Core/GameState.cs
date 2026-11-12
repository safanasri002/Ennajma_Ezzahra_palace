using System.Collections.Generic;
using UnityEngine;

namespace Ennejma
{
    /// <summary>
    /// État global qui survit aux changements de scène.
    /// Phase 1 : sert à savoir où faire apparaître le visiteur.
    /// Phase 2 : retiendra les indices trouvés et la clé de la chambre secrète.
    /// </summary>
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        /// <summary>Identifiant du SpawnPoint où placer le visiteur dans la prochaine scène.</summary>
        public static string NextSpawnId;

        [Header("Chasse au trésor (phase 2)")]
        public bool huntStarted;
        public bool hasSecretKey;
        public readonly HashSet<string> cluesFound = new HashSet<string>();

        public static GameState Ensure()
        {
            if (Instance == null) new GameObject("GameState").AddComponent<GameState>();
            return Instance;
        }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
