using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FullSerializer.Internal
{
	public static class fsTypeCache
	{
		static fsTypeCache()
		{
			object typeFromHandle = typeof(fsTypeCache);
			lock (typeFromHandle)
			{
				fsTypeCache._assembliesByName = new Dictionary<string, Assembly>();
				fsTypeCache._assembliesByIndex = new List<Assembly>();
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					fsTypeCache._assembliesByName[assembly.FullName] = assembly;
					fsTypeCache._assembliesByIndex.Add(assembly);
				}
				fsTypeCache._cachedTypes = new Dictionary<string, Type>();
				AppDomain currentDomain = AppDomain.CurrentDomain;
				if (fsTypeCache._003C_003Ef__mg_0024cache0 == null)
				{
					fsTypeCache._003C_003Ef__mg_0024cache0 = new AssemblyLoadEventHandler(fsTypeCache.OnAssemblyLoaded);
				}
				currentDomain.AssemblyLoad += fsTypeCache._003C_003Ef__mg_0024cache0;
			}
		}

		private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
			object typeFromHandle = typeof(fsTypeCache);
			lock (typeFromHandle)
			{
				fsTypeCache._assembliesByName[args.LoadedAssembly.FullName] = args.LoadedAssembly;
				fsTypeCache._assembliesByIndex.Add(args.LoadedAssembly);
				fsTypeCache._cachedTypes = new Dictionary<string, Type>();
			}
		}

		private static bool TryDirectTypeLookup(string assemblyName, string typeName, out Type type)
		{
			Assembly assembly;
			if (assemblyName != null && fsTypeCache._assembliesByName.TryGetValue(assemblyName, out assembly))
			{
				type = assembly.GetType(typeName, false);
				return type != null;
			}
			type = null;
			return false;
		}

		private static bool TryIndirectTypeLookup(string typeName, out Type type)
		{
			for (int i = 0; i < fsTypeCache._assembliesByIndex.Count; i++)
			{
				Assembly assembly = fsTypeCache._assembliesByIndex[i];
				type = assembly.GetType(typeName);
				if (type != null)
				{
					return true;
				}
			}
			for (int i = 0; i < fsTypeCache._assembliesByIndex.Count; i++)
			{
				Assembly assembly2 = fsTypeCache._assembliesByIndex[i];
				foreach (Type type2 in assembly2.GetTypes())
				{
					if (type2.FullName == typeName)
					{
						type = type2;
						return true;
					}
				}
			}
			type = null;
			return false;
		}

		public static void Reset()
		{
			fsTypeCache._cachedTypes = new Dictionary<string, Type>();
		}

		public static Type GetType(string name)
		{
			return fsTypeCache.GetType(name, null);
		}

		public static Type GetType(string name, string assemblyHint)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			object typeFromHandle = typeof(fsTypeCache);
			Type result;
			lock (typeFromHandle)
			{
				Type type;
				if (!fsTypeCache._cachedTypes.TryGetValue(name, out type))
				{
					if (fsTypeCache.TryDirectTypeLookup(assemblyHint, name, out type) || !fsTypeCache.TryIndirectTypeLookup(name, out type))
					{
					}
					fsTypeCache._cachedTypes[name] = type;
				}
				result = type;
			}
			return result;
		}

		private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

		private static Dictionary<string, Assembly> _assembliesByName;

		private static List<Assembly> _assembliesByIndex;

		[CompilerGenerated]
		private static AssemblyLoadEventHandler _003C_003Ef__mg_0024cache0;
	}
}
