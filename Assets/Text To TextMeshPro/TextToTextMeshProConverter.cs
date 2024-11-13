using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace TextToTMPConverter
{
    public class TextToTextMeshProConverter : MonoBehaviour
    {
        // Serializable class to handle font replacements, allowing users to map old fonts to new TMP fonts
        [System.Serializable]
        public class FontReplacement
        {
            public Font oldFont; // Original Unity font
            public TMP_FontAsset newFont; // Corresponding TextMeshPro font
        }

        // Array of font replacements defined in the Unity Inspector
        public FontReplacement[] fontReplacements;

        // Adds a context menu item in Unity for converting Text to TextMeshProUGUI
        [MenuItem("CONTEXT/TextToTextMeshProConverter/Convert Text to TextMeshPro")]
        private static void ConvertTextToTextMeshPro(MenuCommand command)
        {
            // Get the converter attached to the object and call the conversion function
            TextToTextMeshProConverter converter = (TextToTextMeshProConverter)command.context;
            converter.ConvertTextComponents();
        }

        // Initiates the conversion of Text components to TextMeshProUGUI by processing the object and its children
        public void ConvertTextComponents() => ConvertTextInChildren(transform);

        // Recursively search for Text components in the GameObject's children and convert them to TextMeshProUGUI
        private void ConvertTextInChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                // Check if the GameObject has a Unity Text component
                Text textComponent = child.GetComponent<Text>();
                if (textComponent != null)
                {
                    // Store the values of the old Text component before destroying it
                    string originalText = textComponent.text;
                    int originalFontSize = textComponent.fontSize;
                    bool originalAutoSize = textComponent.resizeTextForBestFit;
                    int originalFontSizeMin = textComponent.resizeTextMinSize;
                    int originalFontSizeMax = textComponent.resizeTextMaxSize;
                    TextAnchor originalAlignment = textComponent.alignment;
                    FontStyle originalFontStyle = textComponent.fontStyle;
                    Color originalColor = textComponent.color;
                    bool originalRaycastTarget = textComponent.raycastTarget;
                    bool originalMaskable = textComponent.maskable;
                    Font originalFont = textComponent.font;
                    HorizontalWrapMode originalHorizontalOverflow = textComponent.horizontalOverflow;

                    // Remove the old Unity Text component
                    DestroyImmediate(textComponent);

                    // Add a new TextMeshProUGUI component
                    TextMeshProUGUI tmpComponent = child.gameObject.AddComponent<TextMeshProUGUI>();

                    // Copy the stored values to the new TextMeshProUGUI component
                    tmpComponent.text = originalText;
                    tmpComponent.fontSize = originalFontSize;
                    tmpComponent.enableAutoSizing = originalAutoSize;
                    tmpComponent.fontSizeMin = originalFontSizeMin;
                    tmpComponent.fontSizeMax = originalFontSizeMax;
                    tmpComponent.alignment = MapTextAnchorToTMPAlignment(originalAlignment);
                    tmpComponent.fontStyle = MapFontStyle(originalFontStyle);
                    tmpComponent.color = originalColor;
                    tmpComponent.raycastTarget = originalRaycastTarget;
                    tmpComponent.maskable = originalMaskable;
                    tmpComponent.enableWordWrapping = (originalHorizontalOverflow == HorizontalWrapMode.Wrap);

                    // Replace the font if a matching TMP_FontAsset is available
                    if (originalFont != null)
                    {
                        TMP_FontAsset replacementFont = GetReplacementFont(originalFont);
                        if (replacementFont != null)
                        {
                            tmpComponent.font = replacementFont;
                        }
                    }
                }

                // Recursively check the child objects
                if (child.childCount > 0)
                {
                    ConvertTextInChildren(child);
                }
            }
        }

        // Maps Unity's FontStyle to TextMeshPro's FontStyles (bold, italic, etc.)
        private FontStyles MapFontStyle(FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case FontStyle.Bold:
                    return FontStyles.Bold;
                case FontStyle.Italic:
                    return FontStyles.Italic;
                case FontStyle.BoldAndItalic:
                    return FontStyles.Bold | FontStyles.Italic;
                default:
                    return FontStyles.Normal; // Default to normal style if not bold or italic
            }
        }

        // Maps Unity's TextAnchor alignment to TextMeshPro's TextAlignmentOptions
        private TextAlignmentOptions MapTextAnchorToTMPAlignment(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter:
                    return TextAlignmentOptions.Top;
                case TextAnchor.UpperRight:
                    return TextAlignmentOptions.TopRight;
                case TextAnchor.MiddleLeft:
                    return TextAlignmentOptions.Left;
                case TextAnchor.MiddleCenter:
                    return TextAlignmentOptions.Center;
                case TextAnchor.MiddleRight:
                    return TextAlignmentOptions.Right;
                case TextAnchor.LowerLeft:
                    return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerCenter:
                    return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerRight:
                    return TextAlignmentOptions.BottomRight;
                default:
                    return TextAlignmentOptions.Center; // Default alignment
            }
        }

        // Searches for a TMP_FontAsset that corresponds to the old Unity font
        private TMP_FontAsset GetReplacementFont(Font oldFont)
        {
            foreach (FontReplacement fontReplacement in fontReplacements)
            {
                if (fontReplacement.oldFont == oldFont)
                {
                    return fontReplacement.newFont; // Return the new TMP font if a match is found
                }
            }
            return null; // Return null if no replacement font is found
        }
    }
}