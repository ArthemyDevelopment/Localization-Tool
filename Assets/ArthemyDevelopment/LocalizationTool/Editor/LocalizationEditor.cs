using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
using System.IO;
using System;

namespace ArthemyDevelopment.Localization
{
    public class LocalizationEditor : EditorWindow
    {
		
		public LocalizationData localizationData;

		bool B_FileExist;
		bool B_isFiles = true;
		bool B_isDataManag = true;
		bool B_isDebugging = true;
		bool B_isSelected = true;
		bool B_updateKeysList;

		[SerializeField] int CustomTriggerIndex;

		AnimBool fileAnimBool = new AnimBool();
		AnimBool objetcAnimBool = new AnimBool();

		GUIStyle foldoutStyle = null;
		

		string defaultOption;

		Vector2 scrollPos;

		EditorWindow scrollWindow;

		public List<KeyData> keyData = null;

		ReorderableList keyList;

		public static GameObject selectedObject;

		SerializedObject serializedObject;

		public FileTypes FileFormat;
        [MenuItem("ArthemyDevelopment/LocalizationTool/EditorTool")]
        static void Init()
		{
			EditorWindow window = GetWindow(typeof(LocalizationEditor), false, "Localization Tool", true);

			window.Show();
			window.minSize = new Vector2(400, 500);
		
		}

		private void Awake()
		{
			scrollWindow = GetWindow(typeof(LocalizationEditor));
		}

		public void OnGUI()
		{
			if (foldoutStyle == null)
			{
				foldoutStyle = new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold};
				if (EditorGUIUtility.isProSkin)
					foldoutStyle.normal.textColor = Color.white;
				else
					foldoutStyle.normal.textColor = Color.black;
			}
			if(serializedObject == null)
			{
				serializedObject = new SerializedObject(this);
			}
			if(serializedObject != null)
				serializedObject.Update();

			
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
			
			EditorGUILayout.Space(10);

			#region Banner

			EditorGUILayout.BeginHorizontal();
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerStart50px"), new Vector2(48,50), 10f);
			
			GUIEditorWindow.ExtendableBannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerExt"),50f,new Vector2(58,58+(EditorGUIUtility.currentViewWidth-116-179)/2));
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerLocalizationTool"), new Vector2(179,50), 58+(EditorGUIUtility.currentViewWidth-116-179)/2);
			
			GUIEditorWindow.ExtendableBannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerExt"),50f,new Vector2(58+ 179+(EditorGUIUtility.currentViewWidth-116-179)/2,55));
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerEnd"),new Vector2(48,50), EditorGUIUtility.currentViewWidth - 58f);
			
			EditorGUILayout.EndHorizontal();

			#endregion

			#region FileManagement
			
			EditorGUILayout.Space(60);
			using (new GUIEditorWindow.FoldoutScope(fileAnimBool, out var shouldDraw, "File Management"))
			{
				if(shouldDraw)
				{
					
					EditorGUILayout.Space(3);
					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					EditorGUILayout.LabelField("Create or Load a language file. \nCreate or modify keys and values for each language you are localizing for and make sure the values are the specific items for that language", GUIEditorWindow.GuiMessageStyle);

					EditorGUILayout.Space(3);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space(5);

					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					EditorGUILayout.Space(3);

					B_isFiles = EditorGUILayout.Foldout(B_isFiles, "Add or modify a new file", true, foldoutStyle);

					if(B_isFiles)
					{						
						EditorGUILayout.Space(5);

						SerializedProperty fileFormatProperty = serializedObject.FindProperty("FileFormat");
						EditorGUILayout.PropertyField(fileFormatProperty,true);
						EditorGUILayout.BeginHorizontal();

						if(GUILayout.Button("Create New Language File"))
						{
							CreateNewData();
						}
						switch(FileFormat)
						{
							case FileTypes.Json:
								if(GUILayout.Button("Load Language Data"))
								{
									LoadJsonData();
								}
					

								break;

							case FileTypes.Strings:
								if (GUILayout.Button("Load Language Data"))
								{
									LoadStringData();
								}
					
								break;

							case FileTypes.CSV:
								if (GUILayout.Button("Load Language Data"))
								{
									LoadCSVData();
								}
					
								break;
						}

			
						EditorGUILayout.EndHorizontal();

					}

					EditorGUILayout.Space(3);
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(5);
					if (B_FileExist)
					{
						
						EditorGUILayout.BeginVertical(EditorStyles.helpBox);
						EditorGUILayout.Space(3);

						B_isDataManag = EditorGUILayout.Foldout(B_isDataManag, "Data Management", true,foldoutStyle);

						if(B_isDataManag)
						{
							EditorGUILayout.Space(5);
							SerializedProperty dataField = serializedObject.FindProperty("localizationData");
							EditorGUILayout.PropertyField(dataField, true);

							EditorGUILayout.BeginHorizontal();
							switch (FileFormat)
							{
								case FileTypes.Json:
									if (GUILayout.Button("Clear Language Values"))
									{
										ClearDataValues();
									}
									if (localizationData != null)
									{
										if (GUILayout.Button("Save Language Data"))
										{
											SaveJsonData();
										}
									}

									break;

								case FileTypes.Strings:
									if (GUILayout.Button("Clear Language Values"))
									{
										ClearDataValues();
									}
									if (localizationData != null)
									{
										if (GUILayout.Button("Save Language Data"))
										{
											SaveStringData();
										}
									}
									break;

								case FileTypes.CSV:
									if (GUILayout.Button("Clear Language Values"))
									{
										ClearDataValues();
									}
									if (localizationData != null)
									{
										if (GUILayout.Button("Save Language Data"))
										{
											SaveCSVData();
										}
									}
									break;
							}
							EditorGUILayout.EndHorizontal();

						}
						EditorGUILayout.Space(3);
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.Space(5);

					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					EditorGUILayout.Space(3);
					B_isDebugging = EditorGUILayout.Foldout(B_isDebugging, "Debugging",true, foldoutStyle);

					if(B_isDebugging)
					{
						EditorGUILayout.BeginVertical(EditorStyles.helpBox);
						EditorGUILayout.Space(3);
						EditorGUILayout.LabelField("For testing purposes you will need to reset the default Index language and the Custom Event Trigger setting, otherwise, if you set the modifier of remembering the language selection or the automatically trigger option, the custom events for the buttons will trigger automatically every time.", GUIEditorWindow.GuiMessageStyle);
						EditorGUILayout.Space(3);
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginHorizontal();
						if(GUILayout.Button("Clear Language index"))
							ClearPlayerPref();
						if (GUILayout.Button("Clear Custom Event trigger"))
							ClearCustomEventTrigger(CustomTriggerIndex);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space(5);
						if(PlayerPrefs.HasKey("ADLocalizationIndex"))
						{
							defaultOption = PlayerPrefs.GetString("ADLanguage");
							GUILayout.Label("Current default language option: " + defaultOption);
						}
						else
						{
							GUILayout.Label("Current default language option: none");
						}
						EditorGUILayout.Space(3);
						SerializedProperty AutomaticallyTriggerIndex = serializedObject.FindProperty("CustomTriggerIndex");
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Custom Trigger index", GUILayout.Width(120));
						EditorGUILayout.PropertyField(AutomaticallyTriggerIndex,GUIContent.none, GUILayout.Width(40));
						if (PlayerPrefs.HasKey("CustomEventTrigger" + CustomTriggerIndex))
							GUILayout.Label("Is custom event trigger: true");
						else
							GUILayout.Label("Is custom event trigger: false");
						EditorGUILayout.EndHorizontal();



					}
					EditorGUILayout.Space(3);
					EditorGUILayout.EndVertical();

					
					
				}
			}
			
			#endregion
			
			EditorGUILayout.Space(10);
			
			#region ObjectManagement
			
			if(B_updateKeysList)
			{
				B_updateKeysList = false;
				if(keyData!= null)
				{
					keyData.Clear();

				}
				else
				{
						
					keyData = new List<KeyData>();
				}
				if(localizationData.LI_Items.Length != 0)
				{
					for (int i = 0; i < localizationData.LI_Items.Length; i++)
					{			 
				
						KeyData tempData = new KeyData();
						tempData.key = localizationData.LI_Items[i].key;
						keyData.Add(tempData);
					}

				}
				keyList = new ReorderableList(serializedObject, serializedObject.FindProperty("keyData"), false, false, false, false);
				keyList.drawElementCallback += OnDrawCallbacks;
					
			}
			
			using (new GUIEditorWindow.FoldoutScope(objetcAnimBool, out var shouldDraw, "Object Management"))
			{
				if(shouldDraw)
				{
					EditorGUILayout.Space(3);
					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					EditorGUILayout.Space(3);
					EditorGUILayout.LabelField("Manage the LocalizationObject component of your scene objects \nConfigure the keys automatically and add the component if the object doesn't have it yet.", GUIEditorWindow.GuiMessageStyle);
					EditorGUILayout.Space(3);
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(3);
					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					EditorGUILayout.Space(3);

					B_isSelected = EditorGUILayout.Foldout(B_isSelected, "Keys list", true, foldoutStyle);

					if(B_isSelected)
					{
						EditorGUILayout.Space(3);

						if(GUILayout.Button("Refresh key list"))
						{
							B_updateKeysList = true;
						}
						if(keyData == null || keyList == null)
						{
							EditorGUILayout.Space(3);
							EditorGUILayout.HelpBox("Create or Load a language file first", MessageType.Warning);
						}
						if (keyData != null && keyList != null)
						{
							EditorGUILayout.Space(3);

							if(keyList!= null)
								keyList.DoLayoutList();

						}
					
					}

					EditorGUILayout.Space(3);
					EditorGUILayout.EndVertical();
					


				}

			}
			
			#endregion

			EditorGUILayout.Space(50);
			GUILayout.FlexibleSpace();
			GUIEditorWindow.FooterLogo(new Vector2(107,143));
			EditorGUILayout.Space(150);
			EditorGUILayout.EndScrollView();

			if (serializedObject != null)
				serializedObject.ApplyModifiedProperties();

		}

		void OnDrawCallbacks(Rect _position, int index, bool isActive, bool isFocus)
		{
			var property = keyList.serializedProperty.GetArrayElementAtIndex(index);

			Rect keyRect = new Rect(_position.x + 30, _position.y, ((EditorGUIUtility.currentViewWidth - 70) / 3) * 2, _position.height);
			Rect buttonRect = new Rect(_position.x + EditorGUIUtility.currentViewWidth - 210, _position.y, 150, _position.height);

			EditorGUI.LabelField(_position, "Key: ");
			EditorGUI.LabelField(keyRect, property.FindPropertyRelative("key").stringValue, EditorStyles.boldLabel);
			if (GUI.Button(buttonRect, "Assign key to object"))
			{
				LocalizationEditor.SetKeyDataToObject(property.FindPropertyRelative("key").stringValue);

			}
		}

		private void Update()
		{
			if (Selection.activeGameObject != null)
			{
				selectedObject = Selection.activeGameObject;
			}
			else
			{
				selectedObject = null;
			}


		}

		#region Load and Save Files

		void LoadJsonData()
		{

			string filePath = EditorUtility.OpenFilePanel("Select your localization file", Application.streamingAssetsPath, "json");

			if(!string.IsNullOrEmpty(filePath))
			{
				B_FileExist = true;
				string jsonData = File.ReadAllText(filePath);

				localizationData = JsonUtility.FromJson<LocalizationData>(jsonData);
			}
			B_updateKeysList = true;
		}

		void SaveJsonData()
		{
			string filePath = EditorUtility.SaveFilePanel("Save your localization file", Application.streamingAssetsPath, "New Language Localization", "json");


			if (!string.IsNullOrEmpty(filePath))
			{
				string jsonData = JsonUtility.ToJson(localizationData, true);
				File.WriteAllText(filePath, jsonData);
			}
		}

		void LoadStringData()
		{
			string filePath = EditorUtility.OpenFilePanel("Select your localization file", Application.streamingAssetsPath, "strings");


			if (!string.IsNullOrEmpty(filePath))
			{
				B_FileExist = true;
				StreamReader reader = File.OpenText(filePath);
				string file;
				List<LocalizationItem> localizedText = new List<LocalizationItem>();

				while((file = reader.ReadLine()) != null)
				{
					if(file.StartsWith("\""))
					{
						string[] data = file.Split('"');
						LocalizationItem item = new LocalizationItem();
						item.key = data[1];
						item.value = data[3];
						localizedText.Add(item);
					}
				}

				localizationData =  new LocalizationData();
				localizationData.LI_Items = new LocalizationItem[localizedText.Count];
				localizationData.LI_Items = localizedText.ToArray();
				reader.Close();
				B_updateKeysList = true;
			}
		}

		void SaveStringData()
		{
			string filePath = EditorUtility.SaveFilePanel("Save your localization file", Application.streamingAssetsPath, "New Language Localization", "strings");


			if (!string.IsNullOrEmpty(filePath))
			{
				using (StreamWriter file = new StreamWriter(filePath))
				{
					foreach (LocalizationItem item in localizationData.LI_Items)
					{						
						file.WriteLine(string.Format("\"{0}\" = \"{1}\";", item.key, item.value));
					}
					file.Close();
				}
				
			}
		}

		void LoadCSVData()
		{
			string filePath = EditorUtility.OpenFilePanel("Select your localization file", Application.streamingAssetsPath, "csv");

			if (!string.IsNullOrEmpty(filePath))
			{
				B_FileExist = true;
				StreamReader reader = File.OpenText(filePath);
				string file;
				List<LocalizationItem> localizedText = new List<LocalizationItem>();

				while((file = reader.ReadLine()) != null)
				{
					string[] lines = file.Split('\n');
					for (int i = 0; i < lines.Length; i++)
					{
						string[] data = lines[i].Split(';');
						LocalizationItem tempItem = new LocalizationItem();
						tempItem.key = data[0];
						tempItem.value = data[1];
						localizedText.Add(tempItem);
					}

					localizationData = new LocalizationData();
					localizationData.LI_Items = new LocalizationItem[localizedText.Count];
					localizationData.LI_Items = localizedText.ToArray();
				}
				reader.Close();
				B_updateKeysList = true;

			}
		}

		void SaveCSVData()
		{
			string filePath = EditorUtility.SaveFilePanel("Save your localization file", Application.streamingAssetsPath, "New Language Localization", "csv");

			if (!string.IsNullOrEmpty(filePath))
			{
				using (StreamWriter file = new StreamWriter(filePath))
				{
					foreach (LocalizationItem item in localizationData.LI_Items)
					{
						file.WriteLine(string.Format("{0};{1}", item.key, item.value));
					}
					file.Close();

				}
			}
		}
		#endregion


		#region UtilityFunctions
		void CreateNewData()
		{
			B_FileExist = true;
			localizationData = new LocalizationData();
			localizationData.LI_Items = new LocalizationItem[0];
			B_updateKeysList = true;
		}

		void ClearDataValues()
		{
			if(EditorUtility.DisplayDialog("Warning", "You are going to erase all values from the file, but the keys will remain the same, this option is recommended when you are creating a new file from an existing one, if you are working on the file is recommended to save it before clearing the values", "Clear values","Cancel"))
			{
				if(localizationData != null && localizationData.LI_Items != null)
					foreach (LocalizationItem item in localizationData.LI_Items)
					{
						item.value = "";
					}

			}
		}

		void ClearPlayerPref()
		{
			if(PlayerPrefs.HasKey("ADLocalizationIndex"))
			{
				PlayerPrefs.DeleteKey("ADLocalizationIndex");
				PlayerPrefs.DeleteKey("ADLanguage");
			}
		}

		void ClearCustomEventTrigger(int i)
		{
			if (PlayerPrefs.HasKey("CustomEventTrigger" + i))
			{
				PlayerPrefs.DeleteKey("CustomEventTrigger"+i);
				Debug.Log("Custom Trigger "+i+ " cleared");
			}
			else
			{
				Debug.Log("There is no Custom Trigger " + i + " in use");
			}
		}

		public static void SetKeyDataToObject(string key)
		{

			if(selectedObject != null)
			{
				LocalizationObject tempLO;
				selectedObject.TryGetComponent<LocalizationObject>(out tempLO);
				if(tempLO != null)
				{
					if(!tempLO.multipleStrings)
						tempLO.key = key;
					else if(tempLO.multipleStrings)
					{
						tempLO.keys.Add(key);
					}
				}
				else
				{
					tempLO = selectedObject.AddComponent<LocalizationObject>();
					if (!tempLO.multipleStrings)
						tempLO.key = key;
					else if (tempLO.multipleStrings)
					{
						tempLO.keys.Add(key);
					}
				}

			}
		}
		
		#endregion
	}
	
	[CustomPropertyDrawer(typeof(LocalizationItem))]
	public class LocalizationItemDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);


			Rect keyRect = new Rect(position.x + 30, position.y, ((EditorGUIUtility.currentViewWidth - 90)/3) - 10, position.height);
			Rect labelRect = new Rect(position.x + ((EditorGUIUtility.currentViewWidth - 90) / 3) +30, position.y, 60, position.height);
			Rect valueRect = new Rect(position.x + ((EditorGUIUtility.currentViewWidth - 90) / 3) + 75, position.y, (((EditorGUIUtility.currentViewWidth - 90) / 3)*2f)-75, position.height);

			EditorGUI.LabelField(position, "Key ");
			EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"), GUIContent.none);
			EditorGUI.LabelField(labelRect, "Value ");
			EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

			EditorGUI.EndProperty();
		}
	}
	
	[Serializable]
	public class KeyData
	{		
		public string key;
		
	}




}
