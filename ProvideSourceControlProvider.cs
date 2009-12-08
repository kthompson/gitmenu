using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Globalization;

namespace GitMenu
{
    /// <summary>
    /// This attribute registers the source control provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProvideSourceControlProvider : RegistrationAttribute
    {
        /// <summary>
        /// Get the friendly name of the provider (written in registry)
        /// </summary>
        public string RegName { get; private set; }

        /// <summary>
        /// Get the UI name of the provider (string resource ID)
        /// </summary>
        public string UIName { get; private set; }


        /// <summary>
        /// </summary>
        public ProvideSourceControlProvider(string regName, string uiName)
        {
            this.RegName = regName;
            this.UIName = uiName;
        }


        /// <summary>
        /// Get the unique guid identifying the provider
        /// </summary>
        public Guid RegGuid
        {
            get { return GuidList.GuidGitSccProvider; }
        }

        /// <summary>
        /// Get the package containing the UI name of the provider
        /// </summary>
        public Guid UINamePkg
        {
            get { return GuidList.GuidGitMenuPkg; }
        }

        /// <summary>
        /// Get the guid of the provider's service
        /// </summary>
        public Guid SccProviderService
        {
            get { return GuidList.GuidGitSccProvider; }
        }

        /// <summary>
        ///     Called to register this attribute with the given context.  The context
        ///     contains the location where the registration inforomation should be placed.
        ///     It also contains other information such as the type being registered and path information.
        /// </summary>
        public override void Register(RegistrationContext context)
        {
            // Write to the context's log what we are about to do
            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "SccProvider:\t\t{0}\n", RegName));

            // Declare the source control provider, its name, the provider's service 
            // and aditionally the packages implementing this provider
            using (var sccProviders = context.CreateKey("SourceControlProviders"))
            {
                using (var sccProviderKey = sccProviders.CreateSubkey(RegGuid.ToString("B")))
                {
                    sccProviderKey.SetValue("", RegName);
                    sccProviderKey.SetValue("Service", SccProviderService.ToString("B"));

                    using (var sccProviderNameKey = sccProviderKey.CreateSubkey("Name"))
                    {
                        sccProviderNameKey.SetValue("", UIName);
                        sccProviderNameKey.SetValue("Package", UINamePkg.ToString("B"));

                        sccProviderNameKey.Close();
                    }

                    // Additionally, you can create a "Packages" subkey where you can enumerate the dll
                    // that are used by the source control provider, something like "Package1"="SccProvider.dll"
                    // but this is not a requirement.
                    sccProviderKey.Close();
                }

                sccProviders.Close();
            }
        }

        /// <summary>
        /// Unregister the source control provider
        /// </summary>
        /// <param name="context"></param>
        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey("SourceControlProviders\\" + GuidList.GuidGitMenuPkg.ToString("B"));
        }
    }

}
