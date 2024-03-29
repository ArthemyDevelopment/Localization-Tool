using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Events;
using System.IO;
using UnityEditor.SceneManagement;

namespace ArthemyDevelopment.Localization
{

	[CustomEditor(typeof(LocalizationManager))]
	public class LocalizationManagerEditor : Editor
	{
		private ReorderableList LanguageList;


		private void OnEnable()
		{
			LanguageList = new ReorderableList(serializedObject, serializedObject.FindProperty("currentsLanguages"), false, false, false, false);
			LanguageList.headerHeight = 0;
			LanguageList.onRemoveCallback += RemoveCallbacks;
			LanguageList.drawElementCallback += OnDrawCallbacks;
		}
		
		private void RemoveCallbacks(ReorderableList list)
		{
			if(EditorUtility.DisplayDialog("Are you sure?", "You are going to remove a language file from the list, the file in the StreamingAssets folder will still exist", "Remove", "Cancel"))
			{
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
			}
		}

		private void OnDrawCallbacks(Rect rect, int index, bool isActive, bool isFocus)
		{
			var item = LanguageList.serializedProperty.GetArrayElementAtIndex(index);

			//Rect lableKey = new Rect(rect.x + 10, rect.y, 60, EditorGUIUtility.singleLineHeight);
			Rect key = new Rect(rect.x + 20, rect.y, (EditorGUIUtility.currentViewWidth-55) , EditorGUIUtility.singleLineHeight);
			Rect lableValue = new Rect(rect.x + ((EditorGUIUtility.currentViewWidth - 160 - 30) ) + 70, rect.y, 100, EditorGUIUtility.singleLineHeight);
			//Rect value = new Rect(rect.x + ((EditorGUIUtility.currentViewWidth - 160) / 2)+ 120, rect.y, (EditorGUIUtility.currentViewWidth - 160 - 30) / 2, EditorGUIUtility.singleLineHeight);

			EditorGUI.LabelField(rect, index.ToString() + " ");

			//EditorGUI.LabelField(lableKey, "Language ");
			EditorGUI.PropertyField(key, item.FindPropertyRelative("S_Name"), GUIContent.none);			

			/*EditorGUI.LabelField(lableValue, "File Name ");
			EditorGUI.PropertyField(value, item.FindPropertyRelative("S_FileName"), GUIContent.none);*/
		}


		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.Space(10);
			
			#region Banner
			
			EditorGUILayout.BeginHorizontal();
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerStart33px"), new Vector2(32,33), 5f);
			
			GUIEditorWindow.ExtendableBannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerExt"),33f,new Vector2(37,37+(EditorGUIUtility.currentViewWidth-74-186)/2));
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerLanguageManager"), new Vector2(186,33), 38+(EditorGUIUtility.currentViewWidth-74-186)/2);
			
			GUIEditorWindow.ExtendableBannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerExt"),33f,new Vector2(37+ 185+(EditorGUIUtility.currentViewWidth-74-186)/2,32));
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerEnd"),new Vector2(32,33), EditorGUIUtility.currentViewWidth - 37f);
			
			EditorGUILayout.EndHorizontal();
			
			#endregion
			
			EditorGUILayout.Space(38);
			
			#region Properties

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("This manager contains the list of all language files used in the project\n\n" +
				"To assign a new language put the file in the StreamingAssets folder and drag it to the list of files, " +
				"you can also add a new language manually and set the language and the file name.", GUIEditorWindow.GuiMessageStyle);
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space(5);
			
			EditorGUI.BeginDisabledGroup(true);
			SerializedProperty LanguagesFileName = serializedObject.FindProperty("LanguagesFileName");
			EditorGUILayout.PropertyField(LanguagesFileName, true );
			EditorGUILayout.Space(5);
			GUILayout.Label("Available Languages", EditorStyles.boldLabel);

			LanguageList.DoLayoutList();
			EditorGUI.EndDisabledGroup();
			if(LanguageList.count == 0)
			{
				EditorGUILayout.HelpBox("At least 1 language file must exist in the language list, please add a new language", MessageType.Warning);
			}

			EditorGUILayout.HelpBox("To create a new file or edit an existing one you can use the Localization Editor Tool in ArthemyDevelopment/LocalizationTool\nYou can also use external tools and import the files into Unity.", MessageType.Info);
			#endregion
			EditorGUILayout.Space(15);
			GUILayout.FlexibleSpace();
			GUIEditorWindow.FooterLogo(new Vector2(107,143));
			EditorGUILayout.Space(143);

			DropGui();
			serializedObject.ApplyModifiedProperties();
		}

		private void DropGui()
		{
			var _event = Event.current.type;

			if(_event == EventType.DragUpdated)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			}
			else if(_event == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();
				foreach (Object dragged in DragAndDrop.objectReferences)
				{
					if(dragged is DefaultAsset)
					{
						DefaultAsset asset = dragged as DefaultAsset;
						LocalizationManager LM = target as LocalizationManager;
						string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(asset));
						LocalizationData localizationData= null;
						LM.LanguagesFileName = fileName;
						if (fileName.EndsWith(".json"))
						{
							string jsonData = File.ReadAllText(AssetDatabase.GetAssetPath(asset));
							localizationData = JsonUtility.FromJson<LocalizationData>(jsonData);
						}
						
						else if (fileName.EndsWith(".csv"))
						{
							List<LocalizationItem> localizedText = new List<LocalizationItem>();
							string file= File.ReadAllText(AssetDatabase.GetAssetPath(asset));
							string[] lines = file.Split('\n');
							for (int i = 0; i < lines.Length; i++)
							{
								string[] data = lines[i].Split(';');
								LocalizationItem tempItem = new LocalizationItem();
						
								for (int j = 0; j < data.Length; j++)
								{
									if (j == 0)
									{
										tempItem.key = data[j];
										continue;
									}
									tempItem.value.Add(data[j]);
							
								}
								localizedText.Add(tempItem);
							}

							localizationData = new LocalizationData();
							localizationData.LI_Items = new LocalizationItem[localizedText.Count];
							localizationData.LI_Items = localizedText.ToArray();
						}
						
						LM.currentsLanguages = new List<LanguageFile>();
						for (int i = 0; i < localizationData.LI_Items[0].value.Count; i++)
						{
							LanguageFile newLangTemp = new LanguageFile();
							newLangTemp.S_Name = localizationData.LI_Items[0].value[i];
							LM.currentsLanguages.Add(newLangTemp);
						}
						
						EditorUtility.SetDirty(LM);
						EditorSceneManager.MarkSceneDirty(LM.gameObject.scene);

					}
				}
			}
		}
	}
}