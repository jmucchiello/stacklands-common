using UnityEngine;
using System.Reflection;
using HarmonyLib;
using UnityEngine.UI;
using System.Reflection.Emit;

namespace CommonModNS
{
    public static class HookRunOptions
    {
        public readonly static List<RunoptsBase> Options = new();

        public static void ApplyPatch(Harmony harmony)
        {
            if (Options.Count > 0)
            {
                MethodInfo miMine = AccessTools.Method(typeof(HookRunOptions), nameof(HookRunOptions.RunOptionsScreen_Start));
                MethodInfo miWM = AccessTools.Method(typeof(RunOptionsScreen), "Start");
                var x = harmony.Patch(original: miWM, postfix: new HarmonyMethod(miMine));
            }
        }

        private static void RunOptionsScreen_Start()
        {
            try
            {
                Transform rosEntriesParent = GameCanvas.instance.transform.Find("RunOptionsScreen/Background/Buttons");
                List<Transform> list = new List<Transform>();
                for (int i = 0; i < rosEntriesParent.childCount; ++i)
                {
                    list.Add(rosEntriesParent.GetChild(i));
                }
                rosEntriesParent.DetachChildren();
                //Transform spacer = null;
                Transform label = null;
                for (int i = 0; i < list.Count; ++i)
                {
                    Transform t = list[i];
                    if (i == 0)
                    {
                        label = UnityEngine.Object.Instantiate(t);
                        label.GetComponent<TMPro.TextMeshProUGUI>().text = I.Xlat("CommonNS_challenge_label");
                        label.name = "MyLabel";
                        ShowTooltip tooltip = label.gameObject.AddComponent<ShowTooltip>();
                        tooltip.MyTooltipTerm = "CommonNS_challenge_tooltip";
                    }
                    if (i == 4 && label != null)
                    {
                        if (t.gameObject.name == "MyLabel")
                            t.SetParent(rosEntriesParent);
                        else
                        {
                            label.SetParent(rosEntriesParent);
                            label.localPosition = Vector3.zero;
                            label.localScale = Vector3.one;
                            label.localRotation = Quaternion.identity;
                        }
                        AddOptions(rosEntriesParent);
                        //                        if (t.gameObject.name == "MyLabel")
                        //                            spacer.SetParent(rosEntriesParent);
                    }
                    else
                    {
                        t.SetParent(rosEntriesParent);
                    }
                }

            }
            catch (Exception ex)
            {
                I.Log($"Exception caught modifying RunOptionScreen.Start{ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void AddOptions(Transform parent)
        {
            foreach (var opt in Options)
            {
                CustomButton button = RunoptsBase.DefaultButton(parent);
                button.transform.DetachChildren();
                opt.Parent.SetParentClean(button.RectTransform);
                opt.Parent.localPosition = Vector3.zero;
                opt.Parent.localScale = Vector3.one;
                opt.Parent.localRotation = Quaternion.identity;
                opt.OnStart?.Invoke(opt);
                CustomButton runoptsSave = RunoptsBase.DefaultButton(parent);
                runoptsSave.transform.SetParentClean(button.RectTransform);
                runoptsSave.transform.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(10, 0, 0, 0);
                runoptsSave.TextMeshPro.text = $"<align=right><size=15>{(opt.Enabled ? I.Xlat("label_on") : I.Xlat("label_off"))}</size></align>";
                runoptsSave.TooltipText = I.Xlat("CommonNS_runopts_save_tooltip");
                runoptsSave.Clicked += delegate
                {
                    opt.Enabled = !opt.Enabled;
                    runoptsSave.TextMeshPro.text = $"<align=right><size=15>{(opt.Enabled ? I.Xlat("label_on") : I.Xlat("label_off"))}</size></align>";
                };
            }
        }
    }

    public abstract class RunoptsBase
    {
        public string NameTerm { get => nameTerm; set { nameTerm = value; SetText(); } }
        private string nameTerm;
        protected virtual void SetText() { }

        public string TooltipTerm { get => tooltipTerm; set { tooltipTerm = value; SetTooltip(); } }
        private string tooltipTerm;
        protected virtual void SetTooltip() { }

        public Color FontColor { get => fontcolor; set { fontcolor = value; SetText(); } }
        private Color fontcolor = Color.black;
        public int FontSize { get => fontsize; set { fontsize = value; SetText(); } }
        private int fontsize = 0;

        protected readonly GameObject Root;
        public readonly Transform Parent;

        public Action<RunoptsBase> OnStart;

        public bool Enabled { get => enabled; set { enabled = value; SetText(); } }
        private bool enabled = true;

        public abstract object BoxedValue { get; set; }

        protected RunoptsBase(string name)
        {
            CustomButton btn = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab);
            btn.EnableUnderline = false;
            Root = btn.gameObject;
            Parent = Root.transform;
            Parent.DetachChildren();
            Parent.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset();
            Parent.name = name ?? "unknown";
            HookRunOptions.Options.Add(this);
        }

        public static CustomButton DefaultButton(Transform parent)
        {
            CustomButton btn = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab);
            btn.transform.SetParentClean(parent);
            btn.transform.localPosition = Vector3.zero;
            btn.transform.localScale = Vector3.one;
            btn.transform.localRotation = Quaternion.identity;
            btn.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset();
            return btn;
        }
    }

    public class RunoptsEnum<T> : RunoptsBase where T : Enum
    {
        public string EnumTermPrefix { get => enumTermPrefix; set { enumTermPrefix = value; SetText(); } }
        private string enumTermPrefix;

        public T Value { get => (T)(object)actualValue; set { actualValue = (int)(object)value; SetText(); ; } }
        private int actualValue;
        private readonly int enumLength;

        private CustomButton Label;

        public override object BoxedValue { get => Value; set => Value = (T)value; }

        public RunoptsEnum(string LabelTerm, T defaultValue) : base(LabelTerm)
        {
            enumLength = Enum.GetValues(typeof(T)).Length;
            I.Log($"RunoptsEnum<{nameof(T)}> Length = {enumLength}");
            actualValue = Math.Clamp((int)(object)defaultValue, 0, enumLength - 1);
            I.Log($"RunoptsEnum<{nameof(T)}> actualValue = {actualValue}");
            OnStart += delegate {
                Label = DefaultButton(Parent);
                SetText();
                SetTooltip();
                Label.Clicked += delegate
                {
                    ++actualValue;
                    if (actualValue >= enumLength) { actualValue = 0; }
                    I.Log($"RunoptsEnum<{nameof(T)}> actualValue = {actualValue}");
                    SetText();
                };
            };
        }

        protected override void SetText()
        {
            if (Label != null)
            {
                string label = I.Xlat(NameTerm);
                string value = $"<color=#{ColorUtility.ToHtmlStringRGBA(FontColor)}>" + I.Xlat($"{EnumTermPrefix}{Value}") + "</color>";
                string text = label + ": " + value;
                if (FontSize > 0) text = $"<size={FontSize}>" + text + "</size>";
                Label.TextMeshPro.text = text;
            }
        }
        protected override void SetTooltip()
        {
            if (Label != null)
            {
                Label.TooltipText = I.Xlat(TooltipTerm);
            }
        }
    }

    public class RunoptsSlider : RunoptsBase
    {
        public int Value { get => actualValue; set { actualValue = value; SetText(); ; } }
        private int actualValue;

        private CustomButton button;
        private SliderGameObject sgo;
        private ShowTooltip showTooltip;

        public readonly int DefaultValue;
        public override object BoxedValue { get => Value; set => Value = (int)value; }
        private readonly IntNormalization norm;

        public RunoptsSlider(string LabelTerm, int defaultValue, int lowerBound, int upperBound, int step) : base(LabelTerm)
        {
            norm = new(lowerBound, upperBound, step);
            DefaultValue = norm.Clamp(defaultValue);

            OnStart += delegate
            {
                button = DefaultButton(Parent);
                sgo = new SliderGameObject(Parent.GetComponent<RectTransform>(), "enemydifficultymod_strength");
                sgo.Slider.transform.GetChild(1).gameObject.SetActive(false);
                HorizontalLayoutGroup hlg = sgo.Slider.GetComponent<HorizontalLayoutGroup>();
                hlg.padding = new RectOffset();
                hlg.childForceExpandHeight = true;
                showTooltip = sgo.Slider.transform.GetChild(0).gameObject.AddComponent<ShowTooltip>();
                SetTooltip();
                sgo.OnChange = delegate
                {
                    Value = norm.Convert(sgo.Value);
                    SetText();
                };
                sgo.Value = norm.Convert(DefaultValue);
                SetText();
            };
        }

        protected override void SetText()
        {
            if (sgo != null)
            {
                string text = I.Xlat(NameTerm) + ": " + $"<color=#{ColorUtility.ToHtmlStringRGBA(FontColor)}>{Value}%</color>";
                if (FontSize > 0) text = $"<size={FontSize}>{text}</size>";
                button.TextMeshPro.text = text;
            }
        }

        protected override void SetTooltip()
        {
            if (sgo != null)
            {
                button.TooltipText = I.Xlat(TooltipTerm);
                showTooltip.MyTooltipTerm = TooltipTerm;
            }
        }
    }
}
