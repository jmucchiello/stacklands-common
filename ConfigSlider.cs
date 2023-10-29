using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CommonModNS
{
    public class ConfigSlider : ConfigEntryHelper
    {
        public override object BoxedValue { get => Value; set => Value = (int)value; }
        public int Value { get; set; }

        public readonly int DefaultValue;

        public readonly int LowerBound = 0, UpperBound = 1, Step = 1;

        private SliderGameObject SGO = null;

        /**
         *  
         **/
        public ConfigSlider(string name, ConfigFile config, Action<int> OnChange, int lowerBound, int upperBound, int step = 1, int defValue = 0)
        {
            if (lowerBound == upperBound || step == 0) throw new ArgumentException("LowerBound cannot equal UpperBound. Step cannot be zero.");

            Name = name;
            Config = config;
            ValueType = typeof(int);
            if (lowerBound > upperBound && step > 0) (lowerBound, upperBound) = (upperBound, lowerBound); // swap
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Step = step;

            DefaultValue = Math.Clamp(defValue, lowerBound, upperBound);
            Value = Math.Clamp(LoadConfigEntry<int>(name, defValue), lowerBound, upperBound);

            UI = new ConfigUI()
            {
                Hidden = true,
                OnUI = delegate (ConfigEntryBase _)
                {
                    SGO = new SliderGameObject(I.MOS.ButtonsParent, "EnemyStrength");
                    SGO.OnChange = SetText;
                    SGO.Value = ((float)Value - LowerBound) / (UpperBound - LowerBound);
                    SetText(SGO.Value);
                }
            };
            config.Entries.Add(this);
        }

        private void SetText(float value)
        {
            Value = (int)(value * (UpperBound - LowerBound) + Step/2) / Step * Step + LowerBound;
            Config.Data[Name] = Value;
            string btnText = $"{Value}%";
            SGO.Text = btnText;
        }

        public override void SetDefaults()
        {
            bool change = Value != DefaultValue;
            Value = DefaultValue;
            Config.Data[Name] = Value;
            SGO.Value = ((float)Value - LowerBound) / (UpperBound - LowerBound);
        }

    }

    [HarmonyPatch]
    public class SliderGameObject
    {
        public delegate void OnChangeFunc(float value);
        public OnChangeFunc OnChange;

        public readonly GameObject Slider;
        public readonly CustomButton Button;
        public readonly Slider SliderControl;

        public float Value { get => SliderControl.value; set => SliderControl.value = Math.Clamp(value, 0f, 1f); }
        public string Text { get => Button.TextMeshPro.text; set => Button.TextMeshPro.text = value; }

        public SliderGameObject(Transform parent, string Id)
        {
            try
            {
                OptionsScreen os = (OptionsScreen)GameCanvas.instance.GetScreen<OptionsScreen>();
                Transform SliderTransform = UnityEngine.Object.Instantiate(os.MusicSlider.transform.parent.transform);
                SliderTransform.SetParentClean(parent);
                SliderTransform.localPosition = Vector3.zero;
                SliderTransform.localScale = Vector3.one;

#pragma warning disable 8602
                Slider = SliderTransform?.gameObject;
                Button = Slider.GetComponent<CustomButton>();
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