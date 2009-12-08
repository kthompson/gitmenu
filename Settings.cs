using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;

namespace GitMenu
{
    public class Settings : Singleton<Settings>
    {

        [DisplayName("Sh Binary Path"), Category("Main")]
        [Description("The full file path of the sh.exe executable.")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [DefaultValue(@"C:\Program Files\Git\bin\sh.exe")]
        public string ShPath { get; set; }

        [DisplayName("Git Binary Path"), Category("Main")]
        [Description("The full file path of the git.exe executable.")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [DefaultValue(@"C:\Program Files\Git\bin\git.exe")]
        public string GitPath { get; set; }

        public Settings()
        {
        }

        public void ResetSettings()
        {

        }

        public void LoadSettings(Package service)
        {
            if (service == null)
                return;

            using (var key = service.UserRegistryRoot)
            {
                var settingsRegistryPath = this.SettingsRegistryPath;
                var automationObject = this.AutomationObject;
                var key2 = key.OpenSubKey(settingsRegistryPath, false);
                if (key2 == null)
                {
                    this.LoadDefaultSettings();
                    return;
                }

                using (key2)
                {
                    var valueNames = key2.GetValueNames();
                    var properties = TypeDescriptor.GetProperties(automationObject);
                    foreach (var str2 in valueNames)
                    {
                        var text = key2.GetValue(str2).ToString();
                        var descriptor = properties[str2];
                        if ((descriptor != null) && descriptor.Converter.CanConvertFrom(typeof(string)))
                        {
                            descriptor.SetValue(automationObject, descriptor.Converter.ConvertFromInvariantString(text));
                        }
                    }
                }
            }
        }

        public void LoadDefaultSettings()
        {
            var automationObject = this.AutomationObject;
            var properties = TypeDescriptor.GetProperties(automationObject);


            foreach (PropertyDescriptor descriptor in properties)
            {
                var defaultValue = descriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
                if (defaultValue == null)
                    continue;

                if (descriptor.Converter.CanConvertFrom(defaultValue.Value.GetType()))
                    descriptor.SetValue(automationObject, descriptor.Converter.ConvertFrom(defaultValue.Value));
            }


        }

        public void SaveSettings(Package service)
        {
            if (service == null)
                return;

            using (var key = service.UserRegistryRoot)
            {
                var settingsRegistryPath = this.SettingsRegistryPath;
                var automationObject = this.AutomationObject;
                var key2 = key.OpenSubKey(settingsRegistryPath, true) ?? key.CreateSubKey(settingsRegistryPath);

                using (key2)
                {
                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(automationObject, new Attribute[] { DesignerSerializationVisibilityAttribute.Visible }))
                    {
                        var converter = descriptor.Converter;
                        if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                        {
                            key2.SetValue(descriptor.Name, converter.ConvertToInvariantString(descriptor.GetValue(automationObject)));
                        }
                    }
                }
            }
        }

        private string _settingsPath;
        protected string SettingsRegistryPath
        {
            get
            {
                return this._settingsPath ?? (this._settingsPath = @"DialogPage\" + this.AutomationObject.GetType().FullName);
            }
            set
            {
                this._settingsPath = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object AutomationObject
        {
            get
            {
                return this;
            }
        }
    }
}
