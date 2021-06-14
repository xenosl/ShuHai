using System;
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
            AddColor("--unity-colors-box-background", 0x28282840, 0x898989E6, 0xDEDEDEFF);
            AddColor("--unity-colors-box-border", 0x00000000, 0x00000000, 0x555555FF);

            // button
            AddColor("--unity-colors-button-background", 0x585858FF, 0xE4E4E4FF, 0xE4E4E4FF);
            AddColor("--unity-colors-button-background-focus", 0x6E6E6EFF, 0xBEBEBEFF);
            AddColor("--unity-colors-button-background-hover", 0x676767FF, 0xECECECFF, 0xECECECFF);
            AddColor("--unity-colors-button-background-pressed", 0x353535FF, 0xB1B1B1FF, 0x6F6F6FFF);
            AddColor("--unity-colors-button-border", 0x303030FF, 0xB2B2B2FF);
            AddColor("--unity-colors-button-border_accent", 0x242424FF, 0x939393FF);
            AddColor("--unity-colors-button-border-pressed", 0x0D0D0DFF, 0x707070FF);
            AddColor("--unity-colors-button-text", 0xEEEEEEFF, 0x090909FF, 0x101010FF);

            // default
            AddColor("--unity-colors-default-background", 0x282828FF, 0xA5A5A5FF);
            AddColor("--unity-colors-default-border", 0x232323FF, 0x999999FF);
            AddColor("--unity-colors-default-text", 0xD2D2D2FF, 0x090909FF, 0x101010FF);
            AddColor("--unity-colors-default-text-hover", 0xBDBDBDFF, 0x090909FF);

            // dropdown
            //AddColor("", );

            // error
            AddColor("--unity-colors-error-text", 0xD32222FF, 0x5A0000FF);

            // helpbox

            // highlight
            AddColor("--unity-colors-highlight-background", 0x2C5D87FF, 0x3A72B0FF);
            AddColor("--unity-colors-highlight-background-hover", 0xFFFFFF10, 0x00000010);
            AddColor("--unity-colors-highlight-background-inactive", 0x4D4D4DFF, 0xAEAEAEFF);
            AddColor("--unity-colors-highlight-text", 0x4C7EFFFF, 0x0032E6FF);
            AddColor("--unity-colors-highlight-text-inactive", 0xFFFFFFFF, 0xFFFFFFFF);

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
            AddIcon("--unity-icons-link",
                "Icons/d_UnLinked.png",
                "Icons/UnLinked.png");
            AddIcon("--unity-icons-link-checked",
                "Icons/d_Linked.png",
                "Icons/Linked.png");
            AddIcon("--unity-icons-lock",
                "Builtin Skins/DarkSkin/Images/IN LockButton.png",
                "Builtin Skins/LightSkin/Images/IN LockButton.png");
            AddIcon("--unity-icons-lock-checked",
                "Builtin Skins/DarkSkin/Images/IN LockButton on.png",
                "Builtin Skins/LightSkin/Images/IN LockButton on.png");
            AddIcon("--unity-icons-lock-checked_pressed",
                "Builtin Skins/DarkSkin/Images/IN LockButton on act.png",
                "Builtin Skins/LightSkin/Images/IN LockButton on act.png");
            AddIcon("--unity-icons-lock-pressed",
                "Builtin Skins/DarkSkin/Images/IN LockButton act.png",
                "Builtin Skins/LightSkin/Images/IN LockButton act.png");
            AddIcon("--unity-icons-minus",
                "UIPackageResources/Images/d_ol_minus.png",
                "UIPackageResources/Images/ol_minus.png");
            AddIcon("--unity-icons-minus-hover",
                "UIPackageResources/Images/d_ol_minus_act.png",
                "UIPackageResources/Images/ol_minus_act.png");
            AddIcon("--unity-icons-pane_options",
                "Builtin Skins/DarkSkin/Images/pane options.png",
                "Builtin Skins/LightSkin/Images/pane options.png");
            AddIcon("--unity-icons-picker",
                "UIPackageResources/Images/d_pick.png",
                "UIPackageResources/Images/pick.png");
            AddIcon("--unity-icons-plus",
                "UIPackageResources/Images/d_ol_plus.png",
                "UIPackageResources/Images/ol_plus.png");
            AddIcon("--unity-icons-plus-hover",
                "UIPackageResources/Images/d_ol_plus_act.png",
                "UIPackageResources/Images/ol_plus_act.png");
            AddIcon("--unity-icons-profiler_timeline_digdown_arrow",
                "Profiler/d_ProfilerTimelineDigDownArrow.png",
                "Profiler/ProfilerTimelineDigDownArrow.png");
            AddIcon("--unity-icons-profiler_timeline_rollup_arrow",
                "Profiler/d_ProfilerTimelineRollUpArrow.png",
                "Profiler/ProfilerTimelineRollUpArrow.png");
            AddIcon("--unity-icons-pulldown",
                "UIPackageResources/Images/d_dropdown.png",
                "UIPackageResources/Images/dropdown.png");
            AddIcon("--unity-icons-scroll_down",
                "UIPackageResources/Images/d_scrolldown.png",
                "UIPackageResources/Images/scrolldown.png");
            AddIcon("--unity-icons-scroll_left",
                "UIPackageResources/Images/d_scrollleft.png",
                "UIPackageResources/Images/scrollleft.png");
            AddIcon("--unity-icons-scroll_right",
                "UIPackageResources/Images/d_scrollright.png",
                "UIPackageResources/Images/scrollright.png");
            AddIcon("--unity-icons-scroll_up",
                "UIPackageResources/Images/d_scrollup.png",
                "UIPackageResources/Images/scrollup.png");
            AddIcon("--unity-icons-search",
                "UIPackageResources/Images/d_search_icon.png",
                "UIPackageResources/Images/search_icon.png");
            AddIcon("--unity-icons-search_menu",
                "UIPackageResources/Images/d_search_menu.png",
                "UIPackageResources/Images/search_menu.png");
            AddIcon("--unity-icons-shuriken_toggle-checked",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleNormalOn.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleNormalOn.png");
            AddIcon("--unity-icons-shuriken_toggle-checked_focus",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleFocusedOn.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleFocusedOn.png");
            AddIcon("--unity-icons-shuriken_toggle-checked_hover",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleHoverOn.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleHoverOn.png");
            AddIcon("--unity-icons-shuriken_toggle_bg	",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleNormal.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleNormal.png");
            AddIcon("--unity-icons-shuriken_toggle_bg-focus",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleFocused.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleFocused.png");
            AddIcon("--unity-icons-shuriken_toggle_bg-focus_mixed",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleFocusedMixed.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleFocusedMixed.png");
            AddIcon("--unity-icons-shuriken_toggle_bg-hover",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleHover.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleHover.png");
            AddIcon("--unity-icons-shuriken_toggle_bg-hover_mixed",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleHoverMixed.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleHoverMixed.png");
            AddIcon("--unity-icons-shuriken_toggle_bg-mixed",
                "Builtin Skins/DarkSkin/Images/ShurikenToggleNormalMixed.png",
                "Builtin Skins/LightSkin/Images/ShurikenToggleNormalMixed.png");
            AddIcon("--unity-icons-toggle-checked",
                "UIPackageResources/Images/d_toggle_on.png",
                "UIPackageResources/Images/toggle_on.png");
            AddIcon("--unity-icons-toggle-checked_focus",
                "UIPackageResources/Images/d_toggle_on_focus.png",
                "UIPackageResources/Images/toggle_on_focus.png");
            AddIcon("--unity-icons-toggle-checked_hover",
                "UIPackageResources/Images/d_toggle_on_hover.png",
                "UIPackageResources/Images/toggle_on_hover.png");
            AddIcon("--unity-icons-toggle-focus_mixed",
                "UIPackageResources/Images/d_toggle_on_focus.png",
                "UIPackageResources/Images/toggle_on_focus.png");
            AddIcon("--unity-icons-toggle-hover_mixed",
                "UIPackageResources/Images/d_toggle_on_hover.png",
                "UIPackageResources/Images/toggle_on_hover.png");
            AddIcon("--unity-icons-toggle-mixed",
                "UIPackageResources/Images/d_toggle_on.png",
                "UIPackageResources/Images/toggle_on.png");
            AddIcon("--unity-icons-toggle_bg",
                "UIPackageResources/Images/d_toggle_bg.png",
                "UIPackageResources/Images/toggle_bg.png");
            AddIcon("--unity-icons-toggle_bg-focus",
                "UIPackageResources/Images/d_toggle_bg_focus.png",
                "UIPackageResources/Images/toggle_bg_focus.png");
            AddIcon("--unity-icons-toggle_bg-focus_mixed",
                "UIPackageResources/Images/d_toggle_mixed_bg_focus.png",
                "UIPackageResources/Images/toggle_mixed_bg_focus.png");
            AddIcon("--unity-icons-toggle_bg-hover",
                "UIPackageResources/Images/d_toggle_bg_hover.png",
                "UIPackageResources/Images/toggle_bg_hover.png");
            AddIcon("--unity-icons-toggle_bg-hover_mixed",
                "UIPackageResources/Images/d_toggle_mixed_bg_hover.png",
                "UIPackageResources/Images/toggle_mixed_bg_hover.png");
            AddIcon("--unity-icons-toggle_bg-mixed",
                "UIPackageResources/Images/d_toggle_mixed_bg.png",
                "UIPackageResources/Images/toggle_mixed_bg.png");
            AddIcon("--unity-icons-window_button_close",
                "Icons/d_winbtn_win_close.png",
                "Icons/winbtn_win_close.png");
            AddIcon("--unity-icons-window_button_max",
                "Icons/d_winbtn_win_max.png",
                "Icons/winbtn_win_max.png");
            AddIcon("--unity-icons-window_button_restore",
                "Icons/d_winbtn_win_restore.png",
                "Icons/winbtn_win_restore.png");
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
            try
            {
                var icon = UnityEditor.Experimental.EditorResources.Load<Texture2D>(path);
                dict.Add(name, icon);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
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
