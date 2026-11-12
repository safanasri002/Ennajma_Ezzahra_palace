using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ennejma
{
    /// <summary>
    /// Fondu au noir entre les scènes. Se crée tout seul la première fois qu'on l'utilise
    /// et persiste entre les scènes (le chargement continue même si la scène d'origine disparaît).
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        static ScreenFader instance;
        CanvasGroup group;
        bool busy;

        public static ScreenFader Instance
        {
            get
            {
                if (instance == null) Create();
                return instance;
            }
        }

        static void Create()
        {
            var go = new GameObject("ScreenFader");
            DontDestroyOnLoad(go);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            instance = go.AddComponent<ScreenFader>();
            instance.group = go.AddComponent<CanvasGroup>();
            instance.group.blocksRaycasts = false;
            instance.group.alpha = 0f;

            var img = new GameObject("Noir").AddComponent<Image>();
            img.transform.SetParent(go.transform, false);
            img.color = Color.black;
            var rt = img.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public IEnumerator Fade(float target, float duration)
        {
            float start = group.alpha, t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(start, target, t / duration);
                yield return null;
            }
            group.alpha = target;
        }

        public void LoadScene(string sceneName, string spawnId, float fadeDuration = 0.8f)
        {
            if (busy) return;
            StartCoroutine(LoadRoutine(sceneName, spawnId, fadeDuration));
        }

        IEnumerator LoadRoutine(string sceneName, string spawnId, float fadeDuration)
        {
            busy = true;
            GameState.NextSpawnId = spawnId;
            yield return Fade(1f, fadeDuration);
            var op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone) yield return null;
            yield return null;
            yield return Fade(0f, fadeDuration);
            busy = false;
        }
    }
}
