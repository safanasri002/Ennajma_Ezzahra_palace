using UnityEngine;

namespace Ennejma
{
    /// <summary>
    /// Zone d'information pour la visite : quand le visiteur entre dans la zone,
    /// un panneau s'affiche en bas de l'écran pendant quelques secondes.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InfoZone : MonoBehaviour
    {
        public string title;
        [TextArea(2, 6)] public string text;
        public float showSeconds = 7f;

        static InfoZone current;
        static float hideAt;
        static GUIStyle style;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            current = this;
            hideAt = Time.time + showSeconds;
        }

        void OnGUI()
        {
            if (current != this || Time.time > hideAt) return;
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 18,
                    wordWrap = true,
                    richText = true,
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(22, 22, 14, 14)
                };
                style.normal.textColor = Color.white;
            }
            string content = string.IsNullOrEmpty(title) ? text : $"<b><size=22>{title}</size></b>\n{text}";
            float w = Mathf.Min(760f, Screen.width - 40f);
            float h = style.CalcHeight(new GUIContent(content), w) + 4f;
            GUI.Box(new Rect((Screen.width - w) * 0.5f, Screen.height - h - 30f, w, h), content, style);
        }
    }
}
