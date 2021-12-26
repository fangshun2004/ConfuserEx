using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Confuser.Core.Frameworks {
	internal sealed class DotNetFrameworkDiscovery : IFrameworkDiscovery {
		private List<IInstalledFramework> InstalledFrameworks { get; set; }

		public IEnumerable<IInstalledFramework> GetInstalledFrameworks()
			=> InstalledFrameworks ??= DiscoverFrameworks().ToList();

		private static IEnumerable<IInstalledFramework> DiscoverFrameworks() {
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				yield break;

			// http://msdn.microsoft.com/en-us/library/hh925568.aspx
			using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\")) {
				foreach (string versionKeyName in ndpKey.GetSubKeyNames()) {
					if (!versionKeyName.StartsWith("v"))
						continue;
					using RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
					var name = (string)versionKey.GetValue("Version", "");
					string sp = versionKey.GetValue("SP", "").ToString();
					string install = versionKey.GetValue("Install", "").ToString();
					if (install == "" || sp != "" && install == "1")
						yield return versionKeyName + "  " + name;

					if (name != "")
						continue;

					foreach (string subKeyName in versionKey.GetSubKeyNames()) {
						RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
						name = (string)subKey.GetValue("Version", "");
						if (name != "")
							sp = subKey.GetValue("SP", "").ToString();
						install = subKey.GetValue("Install", "").ToString();

						if (install == "")
							yield return versionKeyName + "  " + name;
						else if (install == "1")
							yield return "  " + subKeyName + "  " + name;
					}
				}
			}

			using (RegistryKey ndpKey =
				RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "")
					.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\")) {
				if (ndpKey.GetValue("Release") == null)
					yield break;
				var releaseKey = (int)ndpKey.GetValue("Release");
				yield return "v4.5 " + releaseKey;
			}
		}

		private sealed class InstalledDotNetFramework : IInstalledFramework {

		}
	}
}
