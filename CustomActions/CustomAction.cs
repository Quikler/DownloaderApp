using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult FindBundleCachePathByUpgradeCode(Session session)
        {
            using (RegistryKey uninstall = Registry.LocalMachine
                .OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
            {
                if (uninstall is null)
                    return ActionResult.NotExecuted;

                // add a variable in wix (<?define UpgradeCode = "PUT-GUID-HERE" ?>) UpgradeCode to access it from session
                string targetValue = session["UpgradeCode"];

                // We go through each subkey and look for the desired value
                foreach (string subKeyName in uninstall.GetSubKeyNames())
                {
                    using (RegistryKey subKey = uninstall.OpenSubKey(subKeyName))
                    {
                        if (subKey != null)
                        {
                            // BundleUpgradeCode is a property in the registry equal to our UpgradeCode in burn bootstrapper
                            object value = subKey.GetValue("BundleUpgradeCode");
                            if (((string[])value)?.FirstOrDefault() == targetValue)
                            {
                                // BundleCachePath is a property in the registry equal to path of our burn setup.exe
                                session["BundleCachePath"] = (string)subKey.GetValue("BundleCachePath");
                                break;
                            }
                        }
                    }
                }

                return ActionResult.Success;
            }
        }
    }
}
