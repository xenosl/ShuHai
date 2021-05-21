using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.Unity.UIElements
{
    using ColorDict = Dictionary<string, Color>;
    using Color32Dict = Dictionary<string, Color32>;

    /// <summary>
    /// Values of Unity’s built-in style sheets define USS variables.
    /// See https://docs.unity3d.com/Manual/UIE-USS-UnityVariables.html
    /// </summary>
    public static class BuiltinUSSVariables
    {
        #region Metrics

        public static IReadOnlyDictionary<string, int> Metrics { get; }

        private static IReadOnlyDictionary<string, int> InitializeMetrics()
        {
            return new Dictionary<string, int>
            {
                ["--unity-metrics-default-border_radius"] = 3,
                ["--unity-metrics-inspector_titlebar-height"] = 22,
                ["--unity-metrics-toolbar-height"] = 21,
                ["--unity-metrics-toolbar_button-height"] = 20,
                ["--unity-metrics-single_line-height"] = 18,
                ["--unity-metrics-single_line_large-height"] = 20,
                ["--unity-metrics-single_line_small-height"] = 16
            };
        }

        #endregion Metrics

        #region Colors

        public static IReadOnlyDictionary<string, Color> Colors
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorGUIUtility.isProSkin ? DarkThemeColors : LightThemeColors;
#else
                return RuntimeColors;
#endif
            }
        }

        public static IReadOnlyDictionary<string, Color32> Colors32
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorGUIUtility.isProSkin ? DarkThemeColors32 : LightThemeColors32;
#else
                return RuntimeColors32;
#endif
            }
        }

        public static IReadOnlyDictionary<string, Color> RuntimeColors => _runtimeColors;
        public static IReadOnlyDictionary<string, Color32> RuntimeColors32 => _runtimeColors32;
        public static IReadOnlyDictionary<string, Color> DarkThemeColors => _darkThemeColors;
        public static IReadOnlyDictionary<string, Color32> DarkThemeColors32 => _darkThemeColors32;
        public static IReadOnlyDictionary<string, Color> LightThemeColors => _lightThemeColors;
        public static IReadOnlyDictionary<string, Color32> LightThemeColors32 => _lightThemeColors32;

        private static readonly ColorDict _runtimeColors = new ColorDict();
        private static readonly Color32Dict _runtimeColors32 = new Color32Dict();
        private static readonly ColorDict _darkThemeColors = new ColorDict();
        private static readonly Color32Dict _darkThemeColors32 = new Color32Dict();
        private static readonly ColorDict _lightThemeColors = new ColorDict();
        private static readonly Color32Dict _lightThemeColors32 = new Color32Dict();

        private static void InitializeColors()
        {
            // alternated_rows
            AddColor("--unity-colors-alternated_rows-background", 0x3F3F3FFF, 0xCACACAFF);

            // app_toolbar
            AddColor("--unity-colors-app_toolbar-background", 0x191919FF, 0x8A8A8AFF);

            // app_toolbar_button
            //AddColor("", );
            //AddColor("", );

            // box
            //AddColor("", );

            // button
            //AddColor("", );

            // default
            AddColor("--unity-colors-default-background", 0x282828FF, 0xA5A5A5FF);
            AddColor("--unity-colors-default-border", 0x232323FF, 0x999999FF);
            AddColor("--unity-colors-default-text", 0xD2D2D2FF, 0x090909FF, 0x101010FF);
            AddColor("--unity-colors-default-text-hover", 0xBDBDBDFF, 0x090909FF);

            // dropdown
            //AddColor("", );

            // error
            //AddColor("", );

            // helpbox

            // highlight

            // input_field

            // inspector_titlebar

            // label
            AddColor("--unity-colors-label-text", 0xC4C4C4FF, 0x090909FF, 0x3B3B3BFF);
            AddColor("--unity-colors-label-text-focus", 0x81B4FFFF, 0x003C88FF, 0x074492FF);

            // link
            AddColor("--unity-colors-link-text", 0x4C7EFFFF, 0x4C7EFFFF);

            // object_field

            // object_field_button

            // play_mode

            // popup

            // preview

            // preview_overlay

            // progress

            // scrollbar_groove

            // scrollbar_thumb

            // slider_groove

            // slider_thumb

            // slider_thumb_halo

            // tab

            // toolbar

            // toolbar_button

            // visited_link
            AddColor("--unity-colors-visited_link-text", 0xFF00FFFF, 0xAA00AAFF);

            // warning
            AddColor("--unity-colors-warning-text", 0xF4BC02FF, 0x333308FF);

            // window
            AddColor("--unity-colors-window-background", 0x383838FF, 0xC8C8C8FF);
            AddColor("--unity-colors-window-border", 0x242424FF, 0x939393FF);
            AddColor("--unity-colors-window-text", 0xBDBDBDFF, 0x090909FF, 0x101010FF);
        }

        private static void AddColor(string name, uint darkTheme, uint lightTheme, uint? runtime = null)
        {
            if (runtime.HasValue)
            {
                var runtimeColor = Color32(runtime.Value);
                _runtimeColors.Add(name, runtimeColor);
                _runtimeColors32.Add(name, runtimeColor);
            }

            var darkColor = Color32(darkTheme);
            _darkThemeColors.Add(name, darkColor);
            _darkThemeColors32.Add(name, darkColor);

            var lightColor = Color32(lightTheme);
            _lightThemeColors.Add(name, lightColor);
            _lightThemeColors32.Add(name, lightColor);
        }

        private static Color32 Color32(uint rgba) { return Unity.Colors.New32(rgba); }

        #endregion Colors

        #region Icons

        public static IReadOnlyDictionary<string, Texture2D> DarkThemeIcons => _darkThemeIcons;
        public static IReadOnlyDictionary<string, Texture2D> LightThemeIcons => _lightThemeIcons;

        private static readonly Dictionary<string, Texture2D> _darkThemeIcons = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Texture2D> _lightThemeIcons = new Dictionary<string, Texture2D>();

        private static void InitializeIcons()
        {
            AddIcon("--unity-icons-arrow_left",
                "Builtin Skins/DarkSkin/Images/ArrowNavigationLeft.png",
                "Builtin Skins/LightSkin/Images/ArrowNavigationLeft.png");
            AddIcon("--unity-icons-arrow_right",
                "Builtin Skins/DarkSkin/Images/ArrowNavigationRight.png",
                "Builtin Skins/LightSkin/Images/ArrowNavigationRight.png");
            AddIcon("--unity-icons-clear",
                "UIPackageResources/Images/d_clear.png",
                "UIPackageResources/Images/clear.png");
            AddIcon("--unity-icons-color_picker",
                "UIPackageResources/Images/d_color_picker.png",
                "UIPackageResources/Images/color_picker.png");
            AddIcon("--unity-icons-console_entry_error",
                "Icons/d_console.erroricon.png",
                "Icons/console.erroricon.png");
            AddIcon("--unity-icons-console_entry_error_small",
                "Icons/d_console.erroricon.sml.png",
                "Icons/console.erroricon.sml.png");
            AddIcon("--unity-icons-console_entry_info",
                "Icons/d_console.infoicon.png",
                "Icons/console.infoicon.png");
            AddIcon("--unity-icons-console_entry_info_small",
                "Icons/d_console.infoicon.sml.png",
                "Icons/console.infoicon.sml.png");
            AddIcon("--unity-icons-console_entry_warn",
                "Icons/d_console.warnicon.png",
                "Icons/console.warnicon.png");
            AddIcon("--unity-icons-console_entry_warn_small	",
                "Icons/d_console.warnicon.sml.png",
                "Icons/console.warnicon.sml.png");
            AddIcon("--unity-icons-dropdown",
                "UIPackageResources/Images/d_dropdown.png",
                "UIPackageResources/Images/dropdown.png");
            AddIcon("--unity-icons-dropdown_toggle",
                "UIPackageResources/Images/d_dropdown_toggle.png",
                "UIPackageResources/Images/dropdown_toggle.png");
            AddIcon("--unity-icons-foldout",
                "UIPackageResources/Images/d_IN_foldout.png",
                "UIPackageResources/Images/IN_foldout.png");
            AddIcon("--unity-icons-foldout-checked",
                "UIPackageResources/Images/d_IN_foldout_on.png",
                "UIPackageResources/Images/IN_foldout_on.png");
            AddIcon("--unity-icons-foldout-checked_focus",
                "UIPackageResources/Images/d_IN_foldout_focus_on.png",
                "UIPackageResources/Images/IN_foldout_focus_on.png");
            AddIcon("--unity-icons-foldout-checked_pressed",
                "UIPackageResources/Images/d_IN_foldout_act_on.png",
                "UIPackageResources/Images/IN_foldout_act_on.png");
            AddIcon("--unity-icons-foldout-focus",
                "UIPackageResources/Images/d_IN_foldout_focus.png",
                "UIPackageResources/Images/IN_foldout_focus.png");
            AddIcon("--unity-icons-foldout-pressed",
                "UIPackageResources/Images/d_IN_foldout_act.png",
                "UIPackageResources/Images/IN_foldout_act.png");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
            //AddIcon("",
            //    "",
            //    "");
        }

        private static void AddIcon(string name, string darkThemePath, string lightThemePath)
        {
#if UNITY_EDITOR
            AddEditorIcon(_darkThemeIcons, name, darkThemePath);
            AddEditorIcon(_lightThemeIcons, name, lightThemePath);
#endif
        }

#if UNITY_EDITOR
        private static void AddEditorIcon(IDictionary<string, Texture2D> dict, string name, string path)
        {
            var icon = UnityEditor.Experimental.EditorResources.Load<Texture2D>(path);
            if (!icon)
            {
                Debug.LogWarning($"Failed to load built-in icon '{path}'.");
                return;
            }
            dict.Add(name, icon);
        }
#endif

        #endregion Icons

        static BuiltinUSSVariables()
        {
            Metrics = InitializeMetrics();
            InitializeColors();
            InitializeIcons();
        }
    }
}
