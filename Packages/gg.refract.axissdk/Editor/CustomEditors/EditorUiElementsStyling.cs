using UnityEngine;
using UnityEditor;

namespace Axis._Editor.Styling
{
    public static class EditorUiElementsStyling
    {
        public static int fontSize = 20;
        public static float connectButtonWidth = 250;
        public static GUIStyle notActiveButtonStyle;
        public static GUIStyle activeButtonStyle;

        private static Color darkGreen = new Color(0f, 0.6f, 0f);
        private static Color darkRed = new Color(0.4f, 0f, 0f);

        public static GUIStyle centeredText;
        public static GUIStyle centeredTitle;

        public static void InitStyles()
        {
            activeButtonStyle = new GUIStyle(GUI.skin.button);
            notActiveButtonStyle = new GUIStyle(GUI.skin.button);

            activeButtonStyle.fontSize = fontSize;
            notActiveButtonStyle.fontSize = fontSize;
            SetButtonColorsTo(notActiveButtonStyle, darkGreen);
            SetButtonColorsTo(activeButtonStyle, darkRed);

            centeredText = new GUIStyle(GUI.skin.label);
            centeredText.alignment = TextAnchor.UpperCenter;
            centeredTitle = new GUIStyle(centeredText);
            centeredTitle = new GUIStyle(centeredText)
            {
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = 15,
                fontStyle = FontStyle.Bold,
            };

            centeredTitle.hover.textColor = Color.cyan;
            centeredTitle.normal.textColor = Color.cyan;
        }

        private static void SetButtonColorsTo(GUIStyle buttonStyle, Color color)
        {
            buttonStyle.normal.textColor = color;
            buttonStyle.active.textColor = color;
            buttonStyle.hover.textColor = color;
        }

        public static GUIStyle GetButtonStyleFromConnectionStatus(bool isConnectedToAxis)
        {
            return isConnectedToAxis == true ?
                                    activeButtonStyle : notActiveButtonStyle;
        }

        public static GUIStyle GetBtnAsLabelStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            var border = style.border;
            border.left = 0;
            border.top = 0;
            border.right = 0;
            border.bottom = 0;
            style.alignment = TextAnchor.MiddleCenter;
            style.hover.textColor = Color.cyan;

            return style;
        }
    }
}
