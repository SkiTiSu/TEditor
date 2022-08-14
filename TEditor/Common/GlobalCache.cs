using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;

namespace TEditor
{
    public class GlobalCache
    {
        public static string[] Args;

        public static List<FontFamily> LocalizedFontFamily = new();

        public static void Init()
        {
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                LocalizedFontFamily.Add(GetLocalizedFontFamily(font));
            }

            FontFamily GetLocalizedFontFamily(FontFamily font)
            {
                XmlLanguage currentXmlLang = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
                if (font.FamilyNames.ContainsKey(currentXmlLang))
                {
                    return new FontFamily(font.FamilyNames[currentXmlLang]);
                }
                else
                {
                    return font;

                }
            }
        }
    }
}
