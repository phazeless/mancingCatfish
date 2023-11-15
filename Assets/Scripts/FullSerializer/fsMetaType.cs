using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using FullSerializer.Internal;
using UnityEngine;

namespace FullSerializer
{
	public class fsMetaType
	{
		private fsMetaType(fsConfig config, Type reflectedType)
		{
			this.ReflectedType = reflectedType;
			List<fsMetaProperty> list = new List<fsMetaProperty>();
			fsMetaType.CollectProperties(config, list, reflectedType);
			this.Properties = list.ToArray();
		}

		public static fsMetaType Get(fsConfig config, Type type)
		{
            Dictionary<Type, fsMetaType> metaTypes;
            lock (typeof(fsMetaType))
            {
                if (_configMetaTypes.TryGetValue(config, out metaTypes) == false)
                    metaTypes = _configMetaTypes[config] = new Dictionary<Type, fsMetaType>();
            }

            fsMetaType metaType;
            if (metaTypes.TryGetValue(type, out metaType) == false)
            {
                metaType = new fsMetaType(config, type);
                metaTypes[type] = metaType;
            }

            return metaType;
        }

		public static void ClearCache()
		{
			object typeFromHandle = typeof(fsMetaType);
			lock (typeFromHandle)
			{
				fsMetaType._configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();
			}
		}

		private static void CollectProperties(fsConfig config, List<fsMetaProperty> properties, Type reflectedType)
		{
			bool flag = config.DefaultMemberSerialization == fsMemberSerialization.OptIn;
			bool flag2 = config.DefaultMemberSerialization == fsMemberSerialization.OptOut;
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(reflectedType);
			if (attribute != null)
			{
				flag = (attribute.MemberSerialization == fsMemberSerialization.OptIn);
				flag2 = (attribute.MemberSerialization == fsMemberSerialization.OptOut);
			}
			MemberInfo[] declaredMembers = reflectedType.GetDeclaredMembers();
			MemberInfo[] array = declaredMembers;
			for (int i = 0; i < array.Length; i++)
			{
				MemberInfo member = array[i];
				if (!config.IgnoreSerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t)))
				{
					PropertyInfo propertyInfo = member as PropertyInfo;
					FieldInfo fieldInfo = member as FieldInfo;
					if (propertyInfo != null || fieldInfo != null)
					{
						if (propertyInfo == null || config.EnablePropertySerialization)
						{
							if (!flag || config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t)))
							{
								if (!flag2 || !config.IgnoreSerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t)))
								{
									if (propertyInfo != null)
									{
										if (fsMetaType.CanSerializeProperty(config, propertyInfo, declaredMembers, flag2))
										{
											properties.Add(new fsMetaProperty(config, propertyInfo));
										}
									}
									else if (fieldInfo != null && fsMetaType.CanSerializeField(config, fieldInfo, flag2))
									{
										properties.Add(new fsMetaProperty(config, fieldInfo));
									}
								}
							}
						}
					}
				}
			}
			if (reflectedType.Resolve().BaseType != null)
			{
				fsMetaType.CollectProperties(config, properties, reflectedType.Resolve().BaseType);
			}
		}

		private static bool IsAutoProperty(PropertyInfo property, MemberInfo[] members)
		{
			return property.CanWrite && property.CanRead && fsPortableReflection.HasAttribute(property.GetGetMethod(), typeof(CompilerGeneratedAttribute), false);
		}

		private static bool CanSerializeProperty(fsConfig config, PropertyInfo property, MemberInfo[] members, bool annotationFreeValue)
		{
			if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
			{
				return false;
			}
			MethodInfo getMethod = property.GetGetMethod(false);
			MethodInfo setMethod = property.GetSetMethod(false);
			return (getMethod == null || !getMethod.IsStatic) && (setMethod == null || !setMethod.IsStatic) && property.GetIndexParameters().Length <= 0 && (config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(property, t)) || (property.CanRead && property.CanWrite && ((getMethod != null && (config.SerializeNonPublicSetProperties || setMethod != null) && (config.SerializeNonAutoProperties || fsMetaType.IsAutoProperty(property, members))) || annotationFreeValue)));
		}

		private static bool CanSerializeField(fsConfig config, FieldInfo field, bool annotationFreeValue)
		{
			return !typeof(Delegate).IsAssignableFrom(field.FieldType) && !field.IsDefined(typeof(CompilerGeneratedAttribute), false) && !field.IsStatic && (config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(field, t)) || annotationFreeValue || field.IsPublic);
		}

		public void EmitAotData(bool throwException)
		{
			fsAotCompilationManager.AotCandidateTypes.Add(this.ReflectedType);
			if (!throwException)
			{
				return;
			}
			for (int i = 0; i < this.Properties.Length; i++)
			{
				if (!this.Properties[i].IsPublic)
				{
					throw new fsMetaType.AotFailureException(this.ReflectedType.CSharpName(true) + "::" + this.Properties[i].MemberName + " is not public");
				}
				if (this.Properties[i].IsReadOnly)
				{
					throw new fsMetaType.AotFailureException(this.ReflectedType.CSharpName(true) + "::" + this.Properties[i].MemberName + " is readonly");
				}
			}
			if (!this.HasDefaultConstructor)
			{
				throw new fsMetaType.AotFailureException(this.ReflectedType.CSharpName(true) + " does not have a default constructor");
			}
		}

		public fsMetaProperty[] Properties { get; private set; }

		public bool HasDefaultConstructor
		{
			get
			{
				if (this._hasDefaultConstructorCache == null)
				{
					if (this.ReflectedType.Resolve().IsArray)
					{
						this._hasDefaultConstructorCache = new bool?(true);
						this._isDefaultConstructorPublicCache = new bool?(true);
					}
					else if (this.ReflectedType.Resolve().IsValueType)
					{
						this._hasDefaultConstructorCache = new bool?(true);
						this._isDefaultConstructorPublicCache = new bool?(true);
					}
					else
					{
						ConstructorInfo declaredConstructor = this.ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
						this._hasDefaultConstructorCache = new bool?(declaredConstructor != null);
						if (declaredConstructor != null)
						{
							this._isDefaultConstructorPublicCache = new bool?(declaredConstructor.IsPublic);
						}
					}
				}
				return this._hasDefaultConstructorCache.Value;
			}
		}

		public bool IsDefaultConstructorPublic
		{
			get
			{
				if (this._isDefaultConstructorPublicCache == null)
				{
					bool hasDefaultConstructor = this.HasDefaultConstructor;
				}
				return this._isDefaultConstructorPublicCache.Value;
			}
		}

		public object CreateInstance()
		{
			if (this.ReflectedType.Resolve().IsInterface || this.ReflectedType.Resolve().IsAbstract)
			{
				throw new Exception("Cannot create an instance of an interface or abstract type for " + this.ReflectedType);
			}
			if (typeof(ScriptableObject).IsAssignableFrom(this.ReflectedType))
			{
				return ScriptableObject.CreateInstance(this.ReflectedType);
			}
			if (typeof(string) == this.ReflectedType)
			{
				return string.Empty;
			}
			if (!this.HasDefaultConstructor)
			{
				return FormatterServices.GetSafeUninitializedObject(this.ReflectedType);
			}
			if (this.ReflectedType.Resolve().IsArray)
			{
				return Array.CreateInstance(this.ReflectedType.GetElementType(), 0);
			}
			object result;
			try
			{
				result = Activator.CreateInstance(this.ReflectedType, true);
			}
			catch (MissingMethodException innerException)
			{
				throw new InvalidOperationException("Unable to create instance of " + this.ReflectedType + "; there is no default constructor", innerException);
			}
			catch (TargetInvocationException innerException2)
			{
				throw new InvalidOperationException("Constructor of " + this.ReflectedType + " threw an exception when creating an instance", innerException2);
			}
			catch (MemberAccessException innerException3)
			{
				throw new InvalidOperationException("Unable to access constructor of " + this.ReflectedType, innerException3);
			}
			return result;
		}

		private static Dictionary<fsConfig, Dictionary<Type, fsMetaType>> _configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();

		public Type ReflectedType;

		private bool? _hasDefaultConstructorCache;

		private bool? _isDefaultConstructorPublicCache;

		public class AotFailureException : Exception
		{
			public AotFailureException(string reason) : base(reason)
			{
			}
		}
	}
}
