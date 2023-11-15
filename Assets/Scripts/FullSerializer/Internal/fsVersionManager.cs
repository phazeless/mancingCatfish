using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	public static class fsVersionManager
	{
		public static fsResult GetVersionImportPath(string currentVersion, fsVersionedType targetVersion, out List<fsVersionedType> path)
		{
			path = new List<fsVersionedType>();
			if (!fsVersionManager.GetVersionImportPathRecursive(path, currentVersion, targetVersion))
			{
				return fsResult.Fail(string.Concat(new string[]
				{
					"There is no migration path from \"",
					currentVersion,
					"\" to \"",
					targetVersion.VersionString,
					"\""
				}));
			}
			path.Add(targetVersion);
			return fsResult.Success;
		}

		private static bool GetVersionImportPathRecursive(List<fsVersionedType> path, string currentVersion, fsVersionedType current)
		{
			for (int i = 0; i < current.Ancestors.Length; i++)
			{
				fsVersionedType fsVersionedType = current.Ancestors[i];
				if (fsVersionedType.VersionString == currentVersion || fsVersionManager.GetVersionImportPathRecursive(path, currentVersion, fsVersionedType))
				{
					path.Add(fsVersionedType);
					return true;
				}
			}
			return false;
		}

		public static fsOption<fsVersionedType> GetVersionedType(Type type)
		{
            fsOption<fsVersionedType> optionalVersionedType;

            if (_cache.TryGetValue(type, out optionalVersionedType) == false)
            {
                var attr = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);

                if (attr != null)
                {
                    if (string.IsNullOrEmpty(attr.VersionString) == false || attr.PreviousModels != null)
                    {
                        // Version string must be provided
                        if (attr.PreviousModels != null && string.IsNullOrEmpty(attr.VersionString))
                        {
                            throw new Exception("fsObject attribute on " + type + " contains a PreviousModels specifier - it must also include a VersionString modifier");
                        }

                        // Map the ancestor types into versioned types
                        fsVersionedType[] ancestors = new fsVersionedType[attr.PreviousModels != null ? attr.PreviousModels.Length : 0];
                        for (int i = 0; i < ancestors.Length; ++i)
                        {
                            fsOption<fsVersionedType> ancestorType = GetVersionedType(attr.PreviousModels[i]);
                            if (ancestorType.IsEmpty)
                            {
                                throw new Exception("Unable to create versioned type for ancestor " + ancestorType + "; please add an [fsObject(VersionString=\"...\")] attribute");
                            }
                            ancestors[i] = ancestorType.Value;
                        }

                        // construct the actual versioned type instance
                        fsVersionedType versionedType = new fsVersionedType
                        {
                            Ancestors = ancestors,
                            VersionString = attr.VersionString,
                            ModelType = type
                        };

                        // finally, verify that the versioned type passes some
                        // sanity checks
                        VerifyUniqueVersionStrings(versionedType);
                        VerifyConstructors(versionedType);

                        optionalVersionedType = fsOption.Just(versionedType);
                    }
                }

                _cache[type] = optionalVersionedType;
            }

            return optionalVersionedType;
        }

		private static void VerifyConstructors(fsVersionedType type)
		{
			ConstructorInfo[] declaredConstructors = type.ModelType.GetDeclaredConstructors();
			for (int i = 0; i < type.Ancestors.Length; i++)
			{
				Type modelType = type.Ancestors[i].ModelType;
				bool flag = false;
				for (int j = 0; j < declaredConstructors.Length; j++)
				{
					ParameterInfo[] parameters = declaredConstructors[j].GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == modelType)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new fsMissingVersionConstructorException(type.ModelType, modelType);
				}
			}
		}

		private static void VerifyUniqueVersionStrings(fsVersionedType type)
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Queue<fsVersionedType> queue = new Queue<fsVersionedType>();
			queue.Enqueue(type);
			while (queue.Count > 0)
			{
				fsVersionedType fsVersionedType = queue.Dequeue();
				if (dictionary.ContainsKey(fsVersionedType.VersionString) && dictionary[fsVersionedType.VersionString] != fsVersionedType.ModelType)
				{
					throw new fsDuplicateVersionNameException(dictionary[fsVersionedType.VersionString], fsVersionedType.ModelType, fsVersionedType.VersionString);
				}
				dictionary[fsVersionedType.VersionString] = fsVersionedType.ModelType;
				foreach (fsVersionedType item in fsVersionedType.Ancestors)
				{
					queue.Enqueue(item);
				}
			}
		}

		private static readonly Dictionary<Type, fsOption<fsVersionedType>> _cache = new Dictionary<Type, fsOption<fsVersionedType>>();
	}
}
