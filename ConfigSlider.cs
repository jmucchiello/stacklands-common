using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonModNS
{
    public class IntNormalization
    {
        public readonly int LowerBound, UpperBound, Step;
        public IntNormalization(int lowerBound, int upperBound, int step)
        {
            if (lowerBound == upperBound || step == 0) throw new ArgumentException("LowerBound cannot equal UpperBound. Step cannot be zero.");
            if (lowerBound > upperBound && step > 0) (lowerBound, upperBound) = (upperBound, lowerBound); // swap

            LowerBound = lowerBound;
            UpperBound = upperBound;
            Step = step;
        }

        public float Clamp(float value) { return Math.Clamp(value, 0f, 1f); }
        public int Clamp(int value) { return Math.Clamp(value, LowerBound, UpperBound); }

        public int Convert(float value)
        {
            return (int)(Clamp(value) * (UpperBound - LowerBound) + Step/2)/ Step * Step + LowerBound;
        }

        public float Convert(int value)
        {
            return (float)(Clamp(value) - LowerBound) / (float)(UpperBound - LowerBound);
        }
    }

    public class ConfigSlider : ConfigEntryHelper
    {
        public override object BoxedValue { get => Value; set => Value = (int)value; }
        public int Value { 
            get => setting;
            set {
                setting = Math.Clamp(value, LowerBound, UpperBound);
                Config.Data[Name] = setting;
                if (SGO != null)
                {
                    SGO.Value = ToFloat(setting);
                    SetText(setting);
                }
                OnValueChanged?.Invoke(setting);
            }
        }
        public readonly int DefaultValue;

        private int setting;
        private readonly IntNormalization norm;
        private float ToFloat(int value)
        {
            return norm.Convert(value);
        }
        private int ToInt(float value)
        {
            return norm.Convert(value);
        }

        //public float minimumDelta { get => (float)Step / (UpperBound - LowerBound) / 2; }
        public int LowerBound { get => norm.LowerBound; }
        public int UpperBound { get => norm.UpperBound; }
        public int Step { get => norm.Step; }

        public readonly List<string> QuickButtons = new List<string>();
        public int QuickButtonSize = 20;
        public delegate void OnQuickButton(string text);
        public OnQuickButton onQuickButton;

        public Action<int> OnValueChanged;

        public Color currentValueColor = Color.black;

        private SliderGameObject SGO = null;
        private bool InclHeading { get => Heading != null || HeadingTerm != null; }

        public string HeadingTerm, Heading;
        public string TooltipTerm, Tooltip;
        /**
         *  
         **/
        public ConfigSlider(string name, ConfigFile config, int lowerBound, int upperBound, int step = 1, int defValue = 0)
        {
            norm = new(lowerBound, upperBound, step);

            Name = name;
            Config = config;
            ValueType = typeof(int);

            DefaultValue = Math.Clamp(defValue, lowerBound, upperBound);
            Value = LoadConfigEntry<int>(name, defValue);

            UI = new ConfigUI()
            {
                Hidden = true,
                OnUI = delegate (ConfigEntryBase _)
                {
                    CustomButton newParent = null;
                    RectTransform parent = I.MOS.ButtonsParent;
                    if (InclHeading || QuickButtons.Count > 0)
                    {
                        newParent = DefaultButton(parent, null);
                        if (Tooltip != null || TooltipTerm != null) newParent.TooltipText = Tooltip ?? I.Xlat(TooltipTerm);

                        parent = newParent.RectTransform;
                        HorizontalLayoutGroup hlg = parent.GetComponent<HorizontalLayoutGroup>();
                        UnityEngine.Object.DestroyImmediate(hlg, true);
                        VerticalLayoutGroup vlg = parent.gameObject.AddComponent<VerticalLayoutGroup>();
                        vlg.padding = new RectOffset();

                        if (InclHeading)
                        {
                            CustomButton heading = DefaultButton(parent, AlignText(TextAlign.Center, Heading ?? I.Xlat(HeadingTerm ?? "")), Tooltip ?? I.Xlat(TooltipTerm ?? ""));
                            heading.EnableUnderline = false;
                        }
                    }
                    CustomButton sliderParent = DefaultButton(parent, "");
                    sliderParent.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset();
                    sliderParent.RectTransform.DetachChildren();
                    CustomButton left = DefaultButton(sliderParent.RectTransform, AlignText(TextAlign.Center, SizeText(20, "<")));
                    left.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset();
                    left.Clicked += () => { Value -= 5; };
                    left.TooltipText = I.Xlat(TooltipTerm);
                    CustomButton right = DefaultButton(sliderParent.RectTransform, AlignText(TextAlign.Center, SizeText(20, ">")));
                    right.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset();
                    right.Clicked += () => { Value += 5; };
                    right.TooltipText = I.Xlat(TooltipTerm);

                    SGO = new SliderGameObject(sliderParent.RectTransform, Name);
                    SGO.ShowTooltip.MyTooltipTerm = TooltipTerm;
                    SGO.OnChange = (float value) => { Value = ToInt(value); };
                    if (QuickButtons.Count > 0) 
                    {
                        CustomButton horizontal = DefaultButton(parent, null);
                        horizontal.RectTransform.DetachChildren();
                        HorizontalLayoutGroup hlg = horizontal.GetComponent<HorizontalLayoutGroup>();
                        hlg.padding = new RectOffset();
                        foreach (string text in QuickButtons)
                        {
                            CustomButton btn = DefaultButton(horizontal.RectTransform, AlignText(TextAlign.Center, SizeText(QuickButtonSize,text)));
                            btn.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset();
                            btn.TooltipText = I.Xlat(TooltipTerm);
                            btn.Clicked += delegate () {
                                onQuickButton?.Invoke(text);
                            };
                        }
                    }
                    Value = Value;
                }
            };
            config.Entries.Add(this);
        }
        private void SetText(int value)
        {
            if (SGO != null)
            {
                string btnText = $"{value}%";
                SGO.Text = ColorText(currentValueColor, btnText);
            }
        }

        public override void SetDefaults()
        {
            Value = DefaultValue;
        }

    }

    public class SliderGameObject
    {
        public delegate void OnChangeFunc(float value);
        public OnChangeFunc OnChange;

        public readonly GameObject Slider;
        public readonly CustomButton Button;
        public readonly Slider SliderControl;
        public readonly ShowTooltip ShowTooltip;

        public float Value { get => SliderControl.value; set => SliderControl.value = Math.Clamp(value, 0f, 1f); }
        public string Text { get => Button.TextMeshPro.text; set => Button.TextMeshPro.text = value; }

        public SliderGameObject(RectTransform parent, string Id)
        {
            try
            {
                OptionsScreen os = (OptionsScreen)GameCanvas.instance.GetScreen<OptionsScreen>();
                Transform SliderTransform = UnityEngine.Object.Instantiate(os.MusicSlider.transform.parent.transform);
                SliderTransform.SetParentClean(parent);
                SliderTransform.localPosition = Vector3.zero;
                SliderTransform.localScale = Vector3.one;
                SliderTransform.localRotation = Quaternion.identity;

#pragma warning disable 8602
                Slider = SliderTransform?.gameObject;
                Button = Slider.GetComponent<CustomButton>();
                ShowTooltip = Slider.AddComponent<ShowTooltip>();
                SliderControl = Slider.GetComponentInChildren<Slider>();

                SliderControl.onValueChanged.AddListener(delegate (float value)
                {
                    OnChange?.Invoke(value);
                });
                Slider.name = "Slider_" + Id;
#pragma warning restore 8602
            }
            catch (Exception ex)
            {
                I.Log(ex.ToString());
            }
        }
    }
}