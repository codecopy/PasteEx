﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace PasteEx.Util
{
    internal class I18n
    {
        public class Language
        {
            public int Index { get; set; }
            public string LocalName { get; set; }
            public string CultureInfoName { get; set; }
            public CultureInfo CultureInfo { get; set; }

            public Language(int index, string localName, string ciName)
            {
                Index = index;
                LocalName = localName;
                CultureInfoName = ciName;
                CultureInfo = new CultureInfo(ciName);
            }
        }

        public static List<Language> SupportLanguage = new List<Language>
        {
            new Language(0,"English","en-US"),
            new Language(1,"简体中文","zh-CN")
        };

        internal static Language FindLanguageByIndex(int index)
        {
            if (index >= 0 && index < SupportLanguage.Count)
            {
                return SupportLanguage[index];
            }
            return null;
        }

        internal static Language FindLanguageByLocalName(string localName)
        {
            foreach (Language lang in SupportLanguage)
            {
                if (lang.LocalName == localName)
                {
                    return lang;
                }
            }
            return null;
        }

        internal static Language FindLanguageByName(string name)
        {
            foreach (Language lang in SupportLanguage)
            {
                if (lang.CultureInfoName.Contains(name))
                {
                    return lang;
                }
            }
            return null;
        }

        internal static Language FindLanguageByCurrentThreadInfo()
        {
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentUICulture;
            if (ci.Name.Contains("zh"))
            {
                return SupportLanguage[1];
            }
            return SupportLanguage[0];
        }

        internal static void InitCurrentCulture()
        {
            int index = -1;
            try
            {
                if(!String.IsNullOrWhiteSpace(Properties.Settings.Default.language))
                {
                    index = Convert.ToInt32(Properties.Settings.Default.language);
                    Language language = FindLanguageByIndex(index);
                    if (language != null)
                    {
                        CultureInfo newCI = new CultureInfo(language.CultureInfoName);
                        System.Threading.Thread.CurrentThread.CurrentCulture = newCI;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = newCI;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        internal static void SetWinFormLanguage(string ciName)
        {
            CultureInfo newCI = new CultureInfo(ciName);
            System.Threading.Thread.CurrentThread.CurrentCulture = newCI;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCI;

            if (FormSetting.GetInstance() != null)
            {
                ApplyResourceToControl(FormSetting.GetInstance(), new ComponentResourceManager(typeof(FormSetting)), newCI);
            }
            if (FormMain.GetInstance() != null)
            {
                ApplyResourceToControl(FormMain.GetInstance(), new ComponentResourceManager(typeof(FormMain)), newCI);
            }
        }

        private static void ApplyResourceToControl(Control control, ComponentResourceManager cmp, CultureInfo cultureInfo)
        {
            cmp.ApplyResources(control, control.Name, cultureInfo);

            foreach (Control child in control.Controls)
            {
                ApplyResourceToControl(child, cmp, cultureInfo);
            }
        }
    }
}
