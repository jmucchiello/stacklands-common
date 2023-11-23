using System;
using System.Collections.Generic;
using System.Text;



#if false
        private void Fun()
        {
            Transform tb = GameCanvas.instance.transform.Find("ModDisablingScreen/Background/Scroll View/Viewport/Content/Buttons");
            for (int i = 0; i < tb.childCount; ++i) 
            {
                CustomButton cb = tb.GetChild(i).GetComponent<CustomButton>();
                if (cb.TextMeshPro.text.Contains("BetterSaves"))
                {
                    cb.TooltipText = "Hey, just so you're aware, if you disable Better Saves, you won't be able to access some of your saves.";
                }
            }
        }
#endif


#if false
    public class ConfigPushButtonEnum<T> : ConfigEntryHelper where T : Enum
    {
        private int content; // access via BoxedValue
        private int defaultValue; // access via BoxedValue
        private string[] EnumNames = new string[0];
        //        private CustomButton anchor;  // this holds the ModOptionsScreen text that is clicked to open the menu

        public delegate string OnDisplayText();
        public delegate string OnDisplayEnumText(T t);
        public OnDisplayText onDisplayText;
        public OnDisplayText onDisplayTooltip;
        public OnDisplayEnumText onDisplayEnumText;


        public virtual T DefaultValue { get => (T)(object)defaultValue; set => defaultValue = (int)(object)value; }
        public virtual T Value { get => (T)(object)content; set => content = (int)(object)value; }

        public override object BoxedValue
        {
            get => (T)(object)content;
            set => content = (int)value;
        }
    }
#endif


#if false
// STUFF FOR PEACEMODE MOD
        private void TimerText()
        {
            Transform tb = GameScreen.instance.transform.Find("TimeBackground");
            btnTimeText = tb.GetComponent<CustomButton>();
            if (defaultFontSize == 0) defaultFontSize = btnTimeText.TextMeshPro.fontSize;
            btnTimeText.TextMeshPro.fontSize = defaultFontSize * (I.WM.CurrentRunOptions.IsPeacefulMode ? 0.8f : 1f);
        }

        public void UpdateTimeText()
        {
            string text = I.Xlat(I.WM.CurrentRunOptions.IsPeacefulMode ? "label_timetext_peaceful" : "label_timetext", I.XParam("moon", I.WM.CurrentMonth));
            if (I.WM.CurrentBoard.Id != Board.Forest)
            {
                text += $" - {(int)(I.WM.MonthTime - I.WM.MonthTimer)}s";
            }
            btnTimeText.TextMeshPro.text = text;
        }


            RectTransform rt = tb.GetComponent<RectTransform>();
            rt.offsetMax
            Transform icon = null;
            for (int i = 0; i < tb.childCount; i++)
            {
                if (tb.GetChild(i).name == "SpeedIcon")
                {
                    icon = UnityEngine.Object.Instantiate(tb.GetChild(i));
                    break;
                }
            }
            if (icon != null)
            {
                icon.SetParent(tb);
                icon.localScale = Vector3.one;
                Sprite peace = ResourceHelper.LoadSpriteFromPath(Path + "/Icons/peace_symbol.png");
                Image image = icon.GetComponent<Image>();
                image.sprite = peace;
                CustomButton cb = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab);
                cb.transform.SetParent(icon);
                cb.transform.localScale = Vector3.one;
                cb.transform.localPosition = Vector3.zero;
                cb.Clicked += delegate {
                    I.WM.CurrentRunOptions.IsPeacefulMode = !I.WM.CurrentRunOptions.IsPeacefulMode;
                };
                cb.gameObject.SetActive(true);
            }

#endif