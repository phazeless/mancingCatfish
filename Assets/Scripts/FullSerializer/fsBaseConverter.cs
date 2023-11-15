using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer.Internal;

namespace FullSerializer
{
	public abstract class fsBaseConverter
	{
		public virtual object CreateInstance(fsData data, Type storageType)
		{
			if (this.RequestCycleSupport(storageType))
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					"Please override CreateInstance for ",
					base.GetType().FullName,
					"; the object graph for ",
					storageType,
					" can contain potentially contain cycles, so separated instance creation is needed"
				}));
			}
			return storageType;
		}

		public virtual bool RequestCycleSupport(Type storageType)
		{
			return storageType != typeof(string) && (storageType.Resolve().IsClass || storageType.Resolve().IsInterface);
		}

		public virtual bool RequestInheritanceSupport(Type storageType)
		{
			return !storageType.Resolve().IsSealed;
		}

		public abstract fsResult TrySerialize(object instance, out fsData serialized, Type storageType);

		public abstract fsResult TryDeserialize(fsData data, ref object instance, Type storageType);

		protected fsResult FailExpectedType(fsData data, params fsDataType[] types)
		{
			object[] array = new object[7];
			array[0] = base.GetType().Name;
			array[1] = " expected one of ";
			array[2] = string.Join(", ", (from t in types
			select t.ToString()).ToArray<string>());
			array[3] = " but got ";
			array[4] = data.Type;
			array[5] = " in ";
			array[6] = data;
			return fsResult.Fail(string.Concat(array));
		}

		protected fsResult CheckType(fsData data, fsDataType type)
		{
			if (data.Type != type)
			{
				return fsResult.Fail(string.Concat(new object[]
				{
					base.GetType().Name,
					" expected ",
					type,
					" but got ",
					data.Type,
					" in ",
					data
				}));
			}
			return fsResult.Success;
		}

		protected fsResult CheckKey(fsData data, string key, out fsData subitem)
		{
			return this.CheckKey(data.AsDictionary, key, out subitem);
		}

		protected fsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem)
		{
			if (!data.TryGetValue(key, out subitem))
			{
				return fsResult.Fail(string.Concat(new object[]
				{
					base.GetType().Name,
					" requires a <",
					key,
					"> key in the data ",
					data
				}));
			}
			return fsResult.Success;
		}

		protected fsResult SerializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, T value)
		{
			fsData value2;
			fsResult result = this.Serializer.TrySerialize(typeof(T), overrideConverterType, value, out value2);
			if (result.Succeeded)
			{
				data[name] = value2;
			}
			return result;
		}

		protected fsResult DeserializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, out T value)
		{
			fsData data2;
			if (!data.TryGetValue(name, out data2))
			{
				value = default(T);
				return fsResult.Fail("Unable to find member \"" + name + "\"");
			}
			object obj = null;
			fsResult result = this.Serializer.TryDeserialize(data2, typeof(T), overrideConverterType, ref obj);
			value = (T)((object)obj);
			return result;
		}

		public fsSerializer Serializer;
	}
}
