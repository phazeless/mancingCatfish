using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SoftMasking
{
	public static class MaterialReplacer
	{
		public static IEnumerable<IMaterialReplacer> globalReplacers
		{
			get
			{
				if (MaterialReplacer._globalReplacers == null)
				{
					MaterialReplacer._globalReplacers = MaterialReplacer.CollectGlobalReplacers().ToList<IMaterialReplacer>();
				}
				return MaterialReplacer._globalReplacers;
			}
		}

		private static IEnumerable<IMaterialReplacer> CollectGlobalReplacers()
		{
			IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (MaterialReplacer._003C_003Ef__mg_0024cache0 == null)
			{
				MaterialReplacer._003C_003Ef__mg_0024cache0 = new Func<Assembly, IEnumerable<Type>>(MaterialReplacer.GetTypesSafe);
			}
			IEnumerable<Type> source = assemblies.SelectMany(MaterialReplacer._003C_003Ef__mg_0024cache0);
			if (MaterialReplacer._003C_003Ef__mg_0024cache1 == null)
			{
				MaterialReplacer._003C_003Ef__mg_0024cache1 = new Func<Type, bool>(MaterialReplacer.IsMaterialReplacerType);
			}
			IEnumerable<Type> source2 = source.Where(MaterialReplacer._003C_003Ef__mg_0024cache1);
			if (MaterialReplacer._003C_003Ef__mg_0024cache2 == null)
			{
				MaterialReplacer._003C_003Ef__mg_0024cache2 = new Func<Type, IMaterialReplacer>(MaterialReplacer.TryCreateInstance);
			}
			return from t in source2.Select(MaterialReplacer._003C_003Ef__mg_0024cache2)
			where t != null
			select t;
		}

		private static bool IsMaterialReplacerType(Type t)
		{
			return !(t is TypeBuilder) && !t.IsAbstract && t.IsDefined(typeof(GlobalMaterialReplacerAttribute), false) && typeof(IMaterialReplacer).IsAssignableFrom(t);
		}

		private static IMaterialReplacer TryCreateInstance(Type t)
		{
			IMaterialReplacer result;
			try
			{
				result = (IMaterialReplacer)Activator.CreateInstance(t);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogErrorFormat("Could not create instance of {0}: {1}", new object[]
				{
					t.Name,
					ex
				});
				result = null;
			}
			return result;
		}

		private static IEnumerable<Type> GetTypesSafe(this Assembly asm)
		{
			IEnumerable<Type> result;
			try
			{
				result = asm.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				result = from t in ex.Types
				where t != null
				select t;
			}
			return result;
		}

		private static List<IMaterialReplacer> _globalReplacers;

		[CompilerGenerated]
		private static Func<Assembly, IEnumerable<Type>> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<Type, bool> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Func<Type, IMaterialReplacer> _003C_003Ef__mg_0024cache2;
	}
}
