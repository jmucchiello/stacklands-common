using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CommonModNS
{
    public enum TextAlign { Left, Center, Right }

    public abstract class ConfigEntryHelper : ConfigEntryBase
    {
        public virtual void SetDefaults() { }

        public static string AlignText(TextAlign align, string txt)
        {
            return $"<align={align.ToString().ToLower()}>{txt}</align>";
        }

        public static string CenterAlign(string txt)
        {
            return AlignText(TextAlign.Center, txt);
        }

        public static string RightAlign(string txt)
        {
            return AlignText(TextAlign.Right, txt);
        }

        public static string ColorText(string color, string txt)
        {
            return $"<color={color}>" + txt + "</color>";
        }

        public static string ColorText(Color color, string txt)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + txt + "</color>";
        }

        public static string SizeText(int pixels, string txt)
        {
            if (pixels <= 0) return txt;
            return $"<size={pixels}>" + txt + "</size>";
        }

        public CustomButton DefaultButton(RectTransform parent, string text, string tooltip = null)
        {
            CustomButton btn = UnityEngine.Object.Instantiate(I.PFM.ButtonPrefab);
            btn.transform.SetParent(parent);
            btn.transform.localPosition = Vector3.zero;
            btn.transform.localScale = Vector3.one;
            btn.transform.localRotation = Quaternion.identity;
            btn.TextMeshPro.text = text;
            btn.TooltipText = tooltip;
            return btn;
        }

        public T LoadConfigEntry<T>(string key, T defValue)
        {
            if (Config.Data.TryGetValue(key, out JToken value))
            {
                return value.Value<T>();
            }
            return defValue;
        }
    }

    public class ConfigEmptySpace : ConfigEntryBase
    {
        public override object BoxedValue { get => new object(); set => _ = value; }

        public ConfigEmptySpace(ConfigFile Config)
        {
            Name = "__ConfigEmptySpace__";
            ValueType = typeof(object);
            UI = new ConfigUI()
            {
                Hidden = true,
                OnUI = delegate {
                    _ = UnityEngine.Object.Instantiate(I.MOS.SpacerPrefab, I.MOS.ButtonsParent);
                    _ = UnityEngine.Object.Instantiate(I.MOS.SpacerPrefab, I.MOS.ButtonsParent);
                }
            };
            Config.Entries.Add(this);
        }
    }

    public class ConfigResetDefaults : ConfigFreeText
    {
        public ConfigResetDefaults(ConfigFile config, Action OnReset)
            : base("reset", config, "savehelper_reset", "savehelper_reset_tooltip")
        {
            TextAlign = TextAlign.Right;
            Clicked += delegate (ConfigEntryBase _, CustomButton _) {
                OnReset?.Invoke();
            };
        }
    }
}
