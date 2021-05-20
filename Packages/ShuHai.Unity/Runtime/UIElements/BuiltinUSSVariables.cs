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

        static BuiltinUSSVariables()
        {
            Metrics = InitializeMetrics();
            InitializeColors();
        }
    }
}
