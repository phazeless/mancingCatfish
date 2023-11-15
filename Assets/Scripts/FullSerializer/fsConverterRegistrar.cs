using System;
using System.Collections.Generic;
using System.Reflection;
using FullSerializer.Internal;
using FullSerializer.Internal.DirectConverters;

namespace FullSerializer
{
	public class fsConverterRegistrar
	{
		static fsConverterRegistrar()
		{
			foreach (FieldInfo fieldInfo in typeof(fsConverterRegistrar).GetDeclaredFields())
			{
				if (fieldInfo.Name.StartsWith("Register_"))
				{
					fsConverterRegistrar.Converters.Add(fieldInfo.FieldType);
				}
			}
			foreach (MethodInfo methodInfo in typeof(fsConverterRegistrar).GetDeclaredMethods())
			{
				if (methodInfo.Name.StartsWith("Register_"))
				{
					methodInfo.Invoke(null, null);
				}
			}
			List<Type> list = new List<Type>(fsConverterRegistrar.Converters);
			foreach (Type type in fsConverterRegistrar.Converters)
			{
				object obj = null;
				try
				{
					obj = Activator.CreateInstance(type);
				}
				catch (Exception)
				{
				}
				fsIAotConverter fsIAotConverter = obj as fsIAotConverter;
				if (fsIAotConverter != null)
				{
					fsMetaType currentModel = fsMetaType.Get(new fsConfig(), fsIAotConverter.ModelType);
					if (!fsAotCompilationManager.IsAotModelUpToDate(currentModel, fsIAotConverter))
					{
						list.Remove(type);
					}
				}
			}
			fsConverterRegistrar.Converters = list;
		}

		public static AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;

		public static Bounds_DirectConverter Register_Bounds_DirectConverter;

		public static Gradient_DirectConverter Register_Gradient_DirectConverter;

		public static GUIStyle_DirectConverter Register_GUIStyle_DirectConverter;

		public static GUIStyleState_DirectConverter Register_GUIStyleState_DirectConverter;

		public static Keyframe_DirectConverter Register_Keyframe_DirectConverter;

		public static LayerMask_DirectConverter Register_LayerMask_DirectConverter;

		public static Rect_DirectConverter Register_Rect_DirectConverter;

		public static RectOffset_DirectConverter Register_RectOffset_DirectConverter;

		public static List<Type> Converters = new List<Type>();
	}
}
