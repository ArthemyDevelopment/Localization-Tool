using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace ArthemyDevelopment.Localization
{

	[DefaultExecutionOrder(-5)]
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager current;
       
        [Tooltip("The name of the file on StreamingAssets that has the languages. To set the name you can write it manually or drag and drop the file from the project folder")]
        public string LanguagesFileName;
		[Header("Languages")][Tooltip("The list of languages the file has, to add or refresh the available languages click on 'Refresh file'")]
		public List<LanguageFile> currentsLanguages;
		

		private Dictionary<string, string> LocalizedText = new Dictionary<string, string>(); 
		private string S_MissingTextString = "<String Not Found>";


		private void Awake()
		{
			if(current == null)
			{
				current = this;
			}
			else if(current != this)
			{
				Destroy(this);
			}
			if(PlayerPrefs.HasKey("ADLocalizationIndex"))
			{
				LoadTextFile(LanguagesFileName,PlayerPrefs.GetInt("ADLocalizationIndex"));
			}
			DontDestroyOnLoad(gameObject);
			
			BetterStreamingAssets.Initialize();
		}

		#region Localization File


		public void LoadTextFile(string fileName, int LanguageIndex) 
		{
			if(fileName.EndsWith(".json"))
			{
				LoadLocalizationTextJson(fileName,LanguageIndex);
			}
			/*else if(fileName.EndsWith(".strings"))
			{
				LoadLocalizationTextStrings(fileName,LanguageIndex);
			}*/
			else if (fileName.EndsWith(".csv"))
			{
				LoadLocalizationTextCSV(fileName,LanguageIndex);
			}
			else
			{
				Debug.LogError("NO VALID FILE HAS BEEN FOUND");
			}
		}

		public void LoadLocalizationTextJson(string fileName, int LanguageIndex)
		{
			LocalizedText = new Dictionary<string, string>();
			
			if(BetterStreamingAssets.FileExists("/"+fileName))
			{
				
				string JsonData = BetterStreamingAssets.ReadAllText("/" + fileName);
				LocalizationData LoadedData = JsonUtility.FromJson<LocalizationData>(JsonData);

				for (int i = 0; i < LoadedData.LI_Items.Length; i++)
				{
					LocalizedText.Add(LoadedData.LI_Items[i].key, LoadedData.LI_Items[i].value[LanguageIndex]);
				} 

			}
			else
			{
				Debug.LogError("LocalizationData file not found: no Json file with the given name exist in the correct folder");
				
			}
		}


		/*public void LoadLocalizationTextStrings(string fileName, int LanguageIndex)
		{
			LocalizedText = new Dictionary<string, string>();
			
			if(BetterStreamingAssets.FileExists("/"+fileName))
			{
				
				StreamReader reader = BetterStreamingAssets.OpenText("/"+fileName);
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("\""))
					{
						string[] data = line.Split('"');
						LocalizedText.Add(data[1], data[(LanguageIndex*2)+1]);
					}
				}
			}
			else
			{
				Debug.LogError("LocalizationData file not found: no .String file with the given name exist in the correct folder");
			}
		}*/

		public void LoadLocalizationTextCSV(string fileName, int LanguageIndex)
		{
			LocalizedText = new Dictionary<string, string>();


			#if !UNITY_WEBGL
			BetterStreamingAssets.Initialize();
			if(BetterStreamingAssets.FileExists(fileName))
			{
				//Debug.Log(BetterStreamingAssets.OpenText(fileName));
				StreamReader reader = BetterStreamingAssets.OpenText(fileName);
				string file;

				while ((file = reader.ReadLine()) != null)
				{
					string[] lines = file.Split('\n');
					for (int i = 0; i < lines.Length; i++)
					{
						string[] data = lines[i].Split(';');
						LocalizedText.Add(data[0], data[LanguageIndex]);
					}


				}
			}
			else
			{
				Debug.LogError("LocalizationData file not found: no .CSV file with the given name exist in the correct folder");
				Debug.LogError(fileName);
			}
#elif UNITY_WEBGL

			TextAsset textFile = Resources.Load<TextAsset>(fileName.Replace(".csv", ""));
			string[] lines = textFile.text.Split(System.Environment.NewLine);
			
			for (int i = 0; i < lines.Length-1; i++)
			{
				string[] data = lines[i].Split(';');
				LocalizedText.Add(data[0], data[LanguageIndex]);
			}


#endif



		}

		#endregion


		#region Localization Value

		public string GetLocalizationValue(string key)
		{
			if(LocalizedText == null || LocalizedText.Count == 0 )
			{
				LoadDefault();
			}
			string result = S_MissingTextString;
			if(LocalizedText.ContainsKey(key))
			{
				result = LocalizedText[key];
			}
			return result;
		}

		public void LoadDefault()
		{
			if(PlayerPrefs.HasKey("ADLocalizationIndex"))
			{
				LoadTextFile(LanguagesFileName,PlayerPrefs.GetInt("ADLocalizationIndex")-1);;
			}
			else
			{
				LoadTextFile(LanguagesFileName,0);;
			}
		}

		public void SaveDefault(int pref)
		{
			PlayerPrefs.SetInt("ADLocalizationIndex", pref);
			PlayerPrefs.SetString("ADLanguage", currentsLanguages[pref-1].S_Name);
		}

		public void CustomEventTrigger(int i)
		{
			PlayerPrefs.SetInt("CustomEventTrigger" + i, 1);
		}

		#endregion
	}

	#region Serilized Classses

	[System.Serializable]
	public class LocalizationData
	{
		public LocalizationItem[] LI_Items= new LocalizationItem[1];
	}

	[System.Serializable]
	public class LocalizationItem
	{
		public string key;
		public List<string> value= new List<string>( );
	}

	[Serializable]
	public class LanguageFile
	{
		public string S_Name;
		public string S_FileName;
	}
	#endregion
}
