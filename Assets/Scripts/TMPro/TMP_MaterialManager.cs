using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public static class TMP_MaterialManager
	{
		public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
		{
			if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
			{
				UnityEngine.Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
				return baseMaterial;
			}
			int instanceID = baseMaterial.GetInstanceID();
			for (int i = 0; i < TMP_MaterialManager.m_materialList.Count; i++)
			{
				if (TMP_MaterialManager.m_materialList[i].baseMaterial.GetInstanceID() == instanceID && TMP_MaterialManager.m_materialList[i].stencilID == stencilID)
				{
					TMP_MaterialManager.m_materialList[i].count++;
					return TMP_MaterialManager.m_materialList[i].stencilMaterial;
				}
			}
			Material material = new Material(baseMaterial);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.shaderKeywords = baseMaterial.shaderKeywords;
			ShaderUtilities.GetShaderPropertyIDs();
			material.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
			material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
			TMP_MaterialManager.MaskingMaterial maskingMaterial = new TMP_MaterialManager.MaskingMaterial();
			maskingMaterial.baseMaterial = baseMaterial;
			maskingMaterial.stencilMaterial = material;
			maskingMaterial.stencilID = stencilID;
			maskingMaterial.count = 1;
			TMP_MaterialManager.m_materialList.Add(maskingMaterial);
			return material;
		}

		public static void ReleaseStencilMaterial(Material stencilMaterial)
		{
			int instanceID = stencilMaterial.GetInstanceID();
			for (int i = 0; i < TMP_MaterialManager.m_materialList.Count; i++)
			{
				if (TMP_MaterialManager.m_materialList[i].stencilMaterial.GetInstanceID() == instanceID)
				{
					if (TMP_MaterialManager.m_materialList[i].count > 1)
					{
						TMP_MaterialManager.m_materialList[i].count--;
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(TMP_MaterialManager.m_materialList[i].stencilMaterial);
						TMP_MaterialManager.m_materialList.RemoveAt(i);
						stencilMaterial = null;
					}
					break;
				}
			}
		}

		public static Material GetBaseMaterial(Material stencilMaterial)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				return null;
			}
			return TMP_MaterialManager.m_materialList[num].baseMaterial;
		}

		public static Material SetStencil(Material material, int stencilID)
		{
			material.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
			if (stencilID == 0)
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 8f);
			}
			else
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
			}
			return material;
		}

		public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				TMP_MaterialManager.MaskingMaterial maskingMaterial = new TMP_MaterialManager.MaskingMaterial();
				maskingMaterial.baseMaterial = baseMaterial;
				maskingMaterial.stencilMaterial = stencilMaterial;
				maskingMaterial.stencilID = stencilID;
				maskingMaterial.count = 1;
				TMP_MaterialManager.m_materialList.Add(maskingMaterial);
			}
			else
			{
				stencilMaterial = TMP_MaterialManager.m_materialList[num].stencilMaterial;
				TMP_MaterialManager.m_materialList[num].count++;
			}
		}

		public static void RemoveStencilMaterial(Material stencilMaterial)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num != -1)
			{
				TMP_MaterialManager.m_materialList.RemoveAt(num);
			}
		}

		public static void ReleaseBaseMaterial(Material baseMaterial)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.baseMaterial == baseMaterial);
			if (num == -1)
			{
				UnityEngine.Debug.Log("No Masking Material exists for " + baseMaterial.name);
			}
			else if (TMP_MaterialManager.m_materialList[num].count > 1)
			{
				TMP_MaterialManager.m_materialList[num].count--;
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"Removed (1) reference to ",
					TMP_MaterialManager.m_materialList[num].stencilMaterial.name,
					". There are ",
					TMP_MaterialManager.m_materialList[num].count,
					" references left."
				}));
			}
			else
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"Removed last reference to ",
					TMP_MaterialManager.m_materialList[num].stencilMaterial.name,
					" with ID ",
					TMP_MaterialManager.m_materialList[num].stencilMaterial.GetInstanceID()
				}));
				UnityEngine.Object.DestroyImmediate(TMP_MaterialManager.m_materialList[num].stencilMaterial);
				TMP_MaterialManager.m_materialList.RemoveAt(num);
			}
		}

		public static void ClearMaterials()
		{
			if (TMP_MaterialManager.m_materialList.Count<TMP_MaterialManager.MaskingMaterial>() == 0)
			{
				UnityEngine.Debug.Log("Material List has already been cleared.");
				return;
			}
			for (int i = 0; i < TMP_MaterialManager.m_materialList.Count<TMP_MaterialManager.MaskingMaterial>(); i++)
			{
				Material stencilMaterial = TMP_MaterialManager.m_materialList[i].stencilMaterial;
				UnityEngine.Object.DestroyImmediate(stencilMaterial);
				TMP_MaterialManager.m_materialList.RemoveAt(i);
			}
		}

		public static int GetStencilID(GameObject obj)
		{
			int num = 0;
			List<Mask> list = TMP_ListPool<Mask>.Get();
			obj.GetComponentsInParent<Mask>(false, list);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].MaskEnabled())
				{
					num++;
				}
			}
			TMP_ListPool<Mask>.Release(list);
			return Mathf.Min((1 << num) - 1, 255);
		}

		public static Material GetFallbackMaterial(Material sourceMaterial, Material targetMaterial)
		{
			int instanceID = sourceMaterial.GetInstanceID();
			Texture texture = targetMaterial.GetTexture(ShaderUtilities.ID_MainTex);
			int instanceID2 = texture.GetInstanceID();
			long num = (long)instanceID << 32 | (long)((ulong)instanceID2);
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterials.TryGetValue(num, out fallbackMaterial))
			{
				return fallbackMaterial.fallbackMaterial;
			}
			Material material = new Material(sourceMaterial);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.SetTexture(ShaderUtilities.ID_MainTex, texture);
			material.SetFloat(ShaderUtilities.ID_GradientScale, targetMaterial.GetFloat(ShaderUtilities.ID_GradientScale));
			material.SetFloat(ShaderUtilities.ID_TextureWidth, targetMaterial.GetFloat(ShaderUtilities.ID_TextureWidth));
			material.SetFloat(ShaderUtilities.ID_TextureHeight, targetMaterial.GetFloat(ShaderUtilities.ID_TextureHeight));
			material.SetFloat(ShaderUtilities.ID_WeightNormal, targetMaterial.GetFloat(ShaderUtilities.ID_WeightNormal));
			material.SetFloat(ShaderUtilities.ID_WeightBold, targetMaterial.GetFloat(ShaderUtilities.ID_WeightBold));
			fallbackMaterial = new TMP_MaterialManager.FallbackMaterial();
			fallbackMaterial.baseID = instanceID;
			fallbackMaterial.baseMaterial = sourceMaterial;
			fallbackMaterial.fallbackMaterial = material;
			fallbackMaterial.count = 0;
			TMP_MaterialManager.m_fallbackMaterials.Add(num, fallbackMaterial);
			TMP_MaterialManager.m_fallbackMaterialLookup.Add(material.GetInstanceID(), num);
			return material;
		}

		public static void AddFallbackMaterialReference(Material targetMaterial)
		{
			if (targetMaterial == null)
			{
				return;
			}
			int instanceID = targetMaterial.GetInstanceID();
			long key;
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterialLookup.TryGetValue(instanceID, out key) && TMP_MaterialManager.m_fallbackMaterials.TryGetValue(key, out fallbackMaterial))
			{
				fallbackMaterial.count++;
			}
		}

		public static void RemoveFallbackMaterialReference(Material targetMaterial)
		{
			if (targetMaterial == null)
			{
				return;
			}
			int instanceID = targetMaterial.GetInstanceID();
			long num;
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterialLookup.TryGetValue(instanceID, out num) && TMP_MaterialManager.m_fallbackMaterials.TryGetValue(num, out fallbackMaterial))
			{
				fallbackMaterial.count--;
				if (fallbackMaterial.count < 1)
				{
					TMP_MaterialManager.m_fallbackCleanupList.Add(num);
				}
			}
		}

		public static void CleanupFallbackMaterials()
		{
			for (int i = 0; i < TMP_MaterialManager.m_fallbackCleanupList.Count; i++)
			{
				long key = TMP_MaterialManager.m_fallbackCleanupList[i];
				TMP_MaterialManager.FallbackMaterial fallbackMaterial;
				if (TMP_MaterialManager.m_fallbackMaterials.TryGetValue(key, out fallbackMaterial) && fallbackMaterial.count < 1)
				{
					Material fallbackMaterial2 = fallbackMaterial.fallbackMaterial;
					UnityEngine.Object.DestroyImmediate(fallbackMaterial2);
					TMP_MaterialManager.m_fallbackMaterials.Remove(key);
					TMP_MaterialManager.m_fallbackMaterialLookup.Remove(fallbackMaterial2.GetInstanceID());
				}
			}
		}

		public static void ReleaseFallbackMaterial(Material fallackMaterial)
		{
			if (fallackMaterial == null)
			{
				return;
			}
			int instanceID = fallackMaterial.GetInstanceID();
			long key;
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterialLookup.TryGetValue(instanceID, out key) && TMP_MaterialManager.m_fallbackMaterials.TryGetValue(key, out fallbackMaterial))
			{
				if (fallbackMaterial.count > 1)
				{
					fallbackMaterial.count--;
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(fallbackMaterial.fallbackMaterial);
					TMP_MaterialManager.m_fallbackMaterials.Remove(key);
					TMP_MaterialManager.m_fallbackMaterialLookup.Remove(instanceID);
					fallackMaterial = null;
				}
			}
		}

		public static void CopyMaterialPresetProperties(Material source, Material destination)
		{
			Texture texture = destination.GetTexture(ShaderUtilities.ID_MainTex);
			float @float = destination.GetFloat(ShaderUtilities.ID_GradientScale);
			float float2 = destination.GetFloat(ShaderUtilities.ID_TextureWidth);
			float float3 = destination.GetFloat(ShaderUtilities.ID_TextureHeight);
			float float4 = destination.GetFloat(ShaderUtilities.ID_WeightNormal);
			float float5 = destination.GetFloat(ShaderUtilities.ID_WeightBold);
			destination.CopyPropertiesFromMaterial(source);
			destination.shaderKeywords = source.shaderKeywords;
			destination.SetTexture(ShaderUtilities.ID_MainTex, texture);
			destination.SetFloat(ShaderUtilities.ID_GradientScale, @float);
			destination.SetFloat(ShaderUtilities.ID_TextureWidth, float2);
			destination.SetFloat(ShaderUtilities.ID_TextureHeight, float3);
			destination.SetFloat(ShaderUtilities.ID_WeightNormal, float4);
			destination.SetFloat(ShaderUtilities.ID_WeightBold, float5);
		}

		private static List<TMP_MaterialManager.MaskingMaterial> m_materialList = new List<TMP_MaterialManager.MaskingMaterial>();

		private static Dictionary<long, TMP_MaterialManager.FallbackMaterial> m_fallbackMaterials = new Dictionary<long, TMP_MaterialManager.FallbackMaterial>();

		private static Dictionary<int, long> m_fallbackMaterialLookup = new Dictionary<int, long>();

		private static List<long> m_fallbackCleanupList = new List<long>();

		private class FallbackMaterial
		{
			public int baseID;

			public Material baseMaterial;

			public Material fallbackMaterial;

			public int count;
		}

		private class MaskingMaterial
		{
			public Material baseMaterial;

			public Material stencilMaterial;

			public int count;

			public int stencilID;
		}
	}
}
