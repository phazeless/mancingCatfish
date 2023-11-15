using System;
using System.Collections.Generic;
using System.IO;
using FullInspector;
using UnityEngine;

namespace ACE
{
	public abstract class BasicPluginSettings<T> : UnityBaseSettings<T> where T : BasicPluginSettings<T>
	{
		[InspectorShowIf("IsLibraryProject")]
		[InspectorButton]
		[FullInspector.InspectorName("Export library as .UnityPackage")]
		public void ExportAsUnityPackage()
		{
			if (!this.IsLibraryProject)
			{
				return;
			}
			File.WriteAllLines(this.GetPathToIndexingFile(), this.GetIndexedFilenames());
			List<string> list = new List<string>(File.ReadAllLines(this.GetPathToIndexingFile()));
			list = list.FindAll(delegate(string x)
			{
				foreach (string value in this.IgnoreFilesAndFoldersForExporting)
				{
					if (x.Contains(value))
					{
						return false;
					}
				}
				return true;
			});
		}

		[InspectorHideIf("IsLibraryProject")]
		[InspectorButton]
		[FullInspector.InspectorName("Remove library files")]
		public void RemoveLibrary()
		{
			if (this.IsLibraryProject)
			{
				return;
			}
			UnityEngine.Debug.Log("About to remove '" + this.GetMainFolderName() + "' library...");
			string[] array = File.ReadAllLines(this.GetPathToIndexingFile());
			string str = Application.dataPath.Replace("Assets", string.Empty);
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					string text = array[i];
					string path = str + text;
					if (!text.Contains(this.GetRelativeSettingsPath()))
					{
						foreach (string value in this.IgnoreFilesAndFoldersForIndexing)
						{
							if (text.Contains(value))
							{
							}
						}
						bool flag = this.IsDirectory(path);
						bool flag2 = flag && text.EndsWith(".framework");
						if (flag)
						{
							if (flag2 || this.IsDirectoryEmpty(path))
							{
								Directory.Delete(path);
							}
						}
						else
						{
							File.Delete(path);
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("Error when deleting library files: " + ex.Message);
				}
			}
			UnityEngine.Debug.Log("'" + this.GetMainFolderName() + "' library has been removed.");
		}

		private bool IsLibraryProject
		{
			get
			{
				return Application.productName.StartsWith(this.GetMainFolderName());
			}
		}

		private string GetPathToIndexingFile()
		{
			return Application.dataPath + "/" + this.GetRelativeSettingsPath() + BasicPluginSettings<T>.INDEXED_FILES_FILENAME;
		}

		private string[] GetIndexedFilenames()
		{
			string b = string.Empty;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>(Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories));
			for (int i = 0; i < list2.Count; i++)
			{
				list2[i] = list2[i].Replace(Application.dataPath, "Assets").Replace("\\", "/");
				string text = list2[i].Substring(0, list2[i].LastIndexOf("/"));
				if (text != b)
				{
					list.Insert(0, text);
					b = text;
				}
			}
			list2.AddRange(list);
			return list2.ToArray();
		}

		private bool IsDirectoryEmpty(string path)
		{
			string[] directories = Directory.GetDirectories(path);
			string[] files = Directory.GetFiles(path);
			return directories.Length == 0 && files.Length == 0;
		}

		private bool IsDirectory(string path)
		{
			FileAttributes attributes = File.GetAttributes(path);
			return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
		}

		private static readonly string INDEXED_FILES_FILENAME = "library_filenames.txt";

		private static readonly string EXPORTED_UNITYPACKAGE_FILENAME = "latest.unitypackage";

		[InspectorShowIf("IsLibraryProject")]
		[FullInspector.InspectorName("[For lib removal] Files & folders to ignore")]
		public string[] IgnoreFilesAndFoldersForIndexing = new string[0];

		[FullInspector.InspectorName("[For lib exporting] Files & folders to ignore")]
		[InspectorShowIf("IsLibraryProject")]
		public string[] IgnoreFilesAndFoldersForExporting = new string[0];
	}
}
