using UnityEngine;

namespace CommonModNS
{
    public class ConfigFreeText : ConfigEntryHelper
    {
        public TextAlign TextAlign { get; set; } = TextAlign.Center;
        public Color TextColor { get; set; } = Color.black;
        public string Text { get => Button?.TextMeshPro.text; set { if (Button != null) Button.TextMeshPro.text = ColorText(TextColor, AlignText(TextAlign, value)); } }
        public string Tooltip{ get => Button?.TooltipText; set { if (Button != null) Button.TooltipText = value; } }
        public override object BoxedValue { get => new object(); set => _ = value; }

        public Action<ConfigEntryBase, CustomButton> Clicked;
        public Action<ConfigFreeText, CustomButton> OnUI;

        private CustomButton Button;

        /**
         *  Create a header line in the config screen. Also useful for stuff like "Close" or "Reset Defaults"
         **/
        public ConfigFreeText(string name, ConfigFile config, string text, string tooltip = null)
        {
            Name = name;
            Config = config;
            ValueType = typeof(object);
            UI = new ConfigUI()
            {
                NameTerm = text,
                TooltipTerm = tooltip,
                Hidden = true,
                OnUI = delegate (ConfigEntryBase c)
                {
                    Button = UnityEngine.Object.Instantiate(I.PFM.ButtonPrefab, I.MOS.ButtonsParent);
                    Button.transform.localScale = Vector3.one;
                    Button.transform.localPosition = Vector3.zero;
                    Button.transform.localRotation = Quaternion.identity;
                    Button.TextMeshPro.text = GetText();
                    Button.TooltipText = GetTooltip();
                    Button.EnableUnderline = false;
                    Button.Clicked += delegate ()
                    {
                        Clicked?.Invoke(this, Button);
                    };
                    OnUI?.Invoke(this, Button);
                }
            };
            config.Entries.Add(this);
        }

        string GetText()
        {
            return UI.NameTerm == null ? "---MISSING---" : ColorText(TextColor, AlignText(TextAlign, I.Xlat(UI.NameTerm)));
        }
        string GetTooltip()
        {
            return UI.TooltipTerm == null ? null : I.Xlat(UI.TooltipTerm);
        }

        public void Update()
        {
            OnUI?.Invoke(this, Button);
        }
    }
}
