// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Drawing;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.AppCenter.Utils
{

    /// <summary>
    /// Implements the abstract device information helper class
    /// </summary>
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        protected override string GetSdkName()
        {
            var sdkName = WpfHelper.IsRunningOnWpf ? "appcenter.wpf" : "appcenter.winforms";
#if NETCOREAPP3_0
            sdkName = $"{sdkName}.netcore";
#endif
            return sdkName;
        }

        protected override string GetDeviceModel()
        {
            var managementClass = new ManagementClass("Win32_ComputerSystem");
            foreach (var managementObject in managementClass.GetInstances())
            {
                var model = (string)managementObject["Model"];
                return (string.IsNullOrEmpty(model) || DefaultSystemProductName == model ? null : model);
            }
            return string.Empty;
        }

        protected override string GetAppNamespace()
        {
            return Assembly.GetEntryAssembly()?.EntryPoint.DeclaringType?.Namespace;
        }

        protected override string GetDeviceOemName()
        {
            var managementClass = new ManagementClass("Win32_ComputerSystem");
            foreach (var managementObject in managementClass.GetInstances())
            {
                var manufacturer = (string)managementObject["Manufacturer"];
                return (string.IsNullOrEmpty(manufacturer) || DefaultSystemManufacturer == manufacturer ? null : manufacturer);
            }
            return string.Empty;
        }

        protected override string GetOsName()
        {
            return "WINDOWS";
        }

        protected override string GetOsBuild()
        {
            using (var hklmKey = Win32.Registry.LocalMachine)
            using (var subKey = hklmKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                // CurrentMajorVersionNumber present in registry starting with Windows 10
                var majorVersion = subKey.GetValue("CurrentMajorVersionNumber");
                if (majorVersion != null)
                {
                    var minorVersion = subKey.GetValue("CurrentMinorVersionNumber", "0");
                    var buildNumber = subKey.GetValue("CurrentBuildNumber", "0");
                    var revisionNumber = subKey.GetValue("UBR", "0");
                    return $"{majorVersion}.{minorVersion}.{buildNumber}.{revisionNumber}";
                }
                else
                {
                    // If CurrentMajorVersionNumber not present in registry then use CurrentVersion
                    var version = subKey.GetValue("CurrentVersion", "0.0");
                    var buildNumber = subKey.GetValue("CurrentBuild", "0");
                    var buildLabEx = subKey.GetValue("BuildLabEx")?.ToString().Split('.');
                    var revisionNumber = buildLabEx?.Length >= 2 ? buildLabEx[1] : "0";
                    return $"{version}.{buildNumber}.{revisionNumber}";
                }
            }
        }

        protected override string GetOsVersion()
        {
            var managementClass = new ManagementClass("Win32_OperatingSystem");
            foreach (var managementObject in managementClass.GetInstances())
            {
                return (string)managementObject["Version"];
            }
            return string.Empty;
        }

        protected override string GetAppVersion()
        {
#if NET461
            // Get ClickOnce version or fall back to assembly file version. ClickOnce does not exist on .NET Core.
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
#endif
            return Application.ProductVersion;
        }

        protected override string GetAppBuild()
        {
            return GetAppVersion();
        }

        protected override string GetScreenSize()
        {
            const int DESKTOPVERTRES = 117;
            const int DESKTOPHORZRES = 118;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                var desktop = graphics.GetHdc();
                var height = GetDeviceCaps(desktop, DESKTOPVERTRES);
                var width = GetDeviceCaps(desktop, DESKTOPHORZRES);
                return $"{width}x{height}";
            }
        }

        /// <summary>
        /// Import GetDeviceCaps function to retreive scale-independent screen size.
        /// </summary>
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
    }
}
