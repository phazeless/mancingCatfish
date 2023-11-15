using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_StyleSheet : ScriptableObject
	{
		public static TMP_StyleSheet instance
		{
			get
			{
				if (TMP_StyleSheet.s_Instance == null)
				{
					TMP_StyleSheet.s_Instance = TMP_Settings.defaultStyleSheet;
					if (TMP_StyleSheet.s_Instance == null)
					{
						TMP_StyleSheet.s_Instance = (Resources.Load("Style Sheets/TMP Default Style Sheet") as TMP_StyleSheet);
					}
					if (TMP_StyleSheet.s_Instance == null)
					{
						return null;
					}
					TMP_StyleSheet.s_Instance.LoadStyleDictionaryInternal();
				}
				return TMP_StyleSheet.s_Instance;
			}
		}

		public static TMP_StyleSheet LoadDefaultStyleSheet()
		{
			return TMP_StyleSheet.instance;
		}

		public static TMP_Style GetStyle(int hashCode)
		{
			return TMP_StyleSheet.instance.GetStyleInternal(hashCode);
		}

		private TMP_Style GetStyleInternal(int hashCode)
		{
			TMP_Style result;
			if (this.m_StyleDictionary.TryGetValue(hashCode, out result))
			{
				return result;
			}
			return null;
		}

		public void UpdateStyleDictionaryKey(int old_key, int new_key)
		{
			if (this.m_StyleDictionary.ContainsKey(old_key))
			{
				TMP_Style value = this.m_StyleDictionary[old_key];
				this.m_StyleDictionary.Add(new_key, value);
				this.m_StyleDictionary.Remove(old_key);
			}
		}

		public static void RefreshStyles()
		{
			TMP_StyleSheet.s_Instance.LoadStyleDictionaryInternal();
		}

		private void LoadStyleDictionaryInternal()
		{
			this.m_StyleDictionary.Clear();
			for (int i = 0; i < this.m_StyleList.Count; i++)
			{
				this.m_StyleList[i].RefreshStyle();
				if (!this.m_StyleDictionary.ContainsKey(this.m_StyleList[i].hashCode))
				{
					this.m_StyleDictionary.Add(this.m_StyleList[i].hashCode, this.m_StyleList[i]);
				}
			}
		}

		private static TMP_StyleSheet s_Instance;

		[SerializeField]
		private List<TMP_Style> m_StyleList = new List<TMP_Style>(1);

		private Dictionary<int, TMP_Style> m_StyleDictionary = new Dictionary<int, TMP_Style>();
	}
}
