using System;
using System.Collections.Generic;
using System.Text;
using dnlib.DotNet;

namespace Confuser.Analysis {
	/// <summary>
	/// This class contains everything currently implemented to identify the framework version of an specific module.
	/// </summary>
	internal static class ModuleFrameworkAnalyzer {
		private const string _tfmFramework = ".NETFramework";
		private const string _tfmUwp = ".NETCore";
		private const string _tfmCore = ".NETCoreApp";
		private const string _tfmStandard = ".NETStandard";

		internal static (ModuleFramework Framework, Version? Version) IdenitfyFramework(ModuleDef moduleDef) {
			if (moduleDef is null) throw new ArgumentNullException(nameof(moduleDef));


			moduleDef.Assembly.TryGetOriginalTargetFrameworkAttribute
		}

		private static ModuleFramework TryIdentityByAttribute(ModuleDef moduleDef, out Version? version) {
			AssemblyDef asmDef = moduleDef.Assembly;
			if (asmDef is null || !asmDef.TryGetOriginalTargetFrameworkAttribute(out string frameworkMoniker, out version, out _)) {
				version = null;
				return ModuleFramework.Unknown;
			}

			switch (frameworkMoniker) {
				case _tfmFramework:	return ModuleFramework.DotNetFramework;
				case _tfmCore: return ModuleFramework.DotNet;
				case _tfmStandard: return ModuleFramework.DotNetStandard;
				case _tfmUwp: return ModuleFramework.Uwp;
			}

			return ModuleFramework.Unknown;
		}
	}
}
