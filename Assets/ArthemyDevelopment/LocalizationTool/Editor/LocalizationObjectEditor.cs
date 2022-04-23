using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ArthemyDevelopment.Localization
{
	[CanEditMultipleObjects]
    [CustomEditor(typeof(LocalizationObject))]
    public class LocalizationObjectEditor : Editor
    {
		LocalizationObject LO;

		bool B_UseAdvance;
		AdvanceOptions advOpt;
		Components affComp;

		bool B_ShowSet = true;
		bool B_ShowEvents = true;

		GUIStyle customFoldout;

		public SerializedProperty
			LM_Prop,
			debugbool,
			key_Prop,
			keys_Prop,
			AffectedComponents_Prop,
			UseAdvance_Prop,
			AdvanceOpt_Prop,
			LocalizationIndex_Prop,
			RemConfig_Prop,
			LanguageEvents_Prop,
			Trigger_Prop,
			indexTrigger_Prop,
			CustomEvents_Prop,
			multipleStrings_Prop,
			ForCustomComp_Prop;
		




		void OnEnable()
		{
			LO = (LocalizationObject)target;

			LM_Prop = serializedObject.FindProperty("LM");
			debugbool = serializedObject.FindProperty("B_alreadyStart");
			key_Prop = serializedObject.FindProperty("key");
			keys_Prop = serializedObject.FindProperty("keys");
			AffectedComponents_Prop = serializedObject.FindProperty("AffectedComponents");
			UseAdvance_Prop = serializedObject.FindProperty("useAdvanceOptions");

			AdvanceOpt_Prop = serializedObject.FindProperty("advanceOptions");

			LocalizationIndex_Prop = serializedObject.FindProperty("LocalizationIndex");
			RemConfig_Prop = serializedObject.FindProperty("rememberConfiguration");
			LanguageEvents_Prop = serializedObject.FindProperty("OnLanguageSelection");
			
			Trigger_Prop = serializedObject.FindProperty("triggerAutomatically");
			indexTrigger_Prop = serializedObject.FindProperty("triggerIndex");
			CustomEvents_Prop = serializedObject.FindProperty("OnCustomEvent");

			multipleStrings_Prop = serializedObject.FindProperty("multipleStrings");
			ForCustomComp_Prop = serializedObject.FindProperty("ForCustomComponent");
			
		}

		public override void OnInspectorGUI()
		{
			if(customFoldout == null)
			{
				customFoldout = new GUIStyle(EditorStyles.foldout);
				customFoldout.padding.left = 20;
				customFoldout.margin.left = -5;
				customFoldout.fontStyle = FontStyle.Bold;

			}
			serializedObject.Update();


			B_UseAdvance = LO.useAdvanceOptions;
			advOpt = LO.advanceOptions;
			affComp = LO.AffectedComponents;
			

			EditorGUILayout.Space(10);

			#region Banner

			EditorGUILayout.BeginHorizontal();
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerStart33px"), new Vector2(32,33), 5f);
			
			GUIEditorWindow.ExtendableBannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerExt"),33f,new Vector2(37,37+(EditorGUIUtility.currentViewWidth-74-165-10)/2));
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerLocalizationObject"), new Vector2(165,33), 38+(EditorGUIUtility.currentViewWidth-74-165-10)/2);
			
			GUIEditorWindow.ExtendableBannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerExt"),33f,new Vector2(37+ 165+(EditorGUIUtility.currentViewWidth-74-165-10)/2,42));
			
			GUIEditorWindow.BannerLogo(Resources.Load<Texture>("ArthemyDevelopment/Editor/BannerEnd"),new Vector2(32,33), EditorGUIUtility.currentViewWidth - 47f);
			
			EditorGUILayout.EndHorizontal();

			#endregion

			EditorGUILayout.Space(38);
			
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.Space(3);
			
			
			EditorGUILayout.LabelField("To localize, select the type of object and the correct reference key\nIf the object is a button, you have access to advance options", GUIEditorWindow.GuiMessageStyle);

			EditorGUILayout.Space(3);
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space(10);

			
			EditorGUILayout.Space(3);
			
			#region Properties

			GUILayout.Label("Localization Info", EditorStyles.boldLabel);
			EditorGUILayout.Space(5);

			EditorGUILayout.PropertyField(AffectedComponents_Prop);
			if (affComp != Components.CustomComponent || !multipleStrings_Prop.boolValue)
			{
				if(affComp != Components.ButtonUI)
					EditorGUILayout.PropertyField(key_Prop);
				else if(affComp == Components.ButtonUI && advOpt != AdvanceOptions.SetLanguage)
					EditorGUILayout.PropertyField(key_Prop);

			}
			else if (affComp == Components.CustomComponent && multipleStrings_Prop.boolValue)
				EditorGUILayout.PropertyField(keys_Prop);
			
			EditorGUILayout.Space(5);

			if(affComp == Components.CustomComponent)
			{
				EditorGUILayout.PropertyField(multipleStrings_Prop);
				EditorGUILayout.Space(3);
				EditorGUILayout.PropertyField(ForCustomComp_Prop);
					
			}

			if(affComp == Components.ButtonUI)
			{
				GUILayout.Label("Advance options", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(UseAdvance_Prop);


				if(B_UseAdvance)
				{
					EditorGUILayout.PropertyField(AdvanceOpt_Prop);
					GUIStyle helpbox = EditorStyles.helpBox;
					EditorGUILayout.BeginHorizontal(helpbox);
					EditorGUILayout.BeginVertical();

					switch(advOpt)
					{
						case AdvanceOptions.SetLanguage:
							B_ShowSet = EditorGUILayout.Foldout(B_ShowSet, "Set Language properties", true, customFoldout);
							EditorGUILayout.Space(3);
							if(B_ShowSet)
							{
								GUIEditorWindow.HorizontalLine(1,EditorGUIUtility.currentViewWidth, new Vector2(0,0), new Vector2(18,37), new Color(0,0,0));
								EditorGUILayout.Space(2);
							
								EditorGUILayout.PropertyField(LocalizationIndex_Prop);							
								EditorGUILayout.PropertyField(RemConfig_Prop);							
								EditorGUILayout.PropertyField(LanguageEvents_Prop);							

							}
							break;

						case AdvanceOptions.CustomEvents:
							B_ShowEvents = EditorGUILayout.Foldout(B_ShowEvents, "Events properties",true, customFoldout);
							EditorGUILayout.Space(3);
							if (B_ShowEvents)
							{	
								GUIEditorWindow.HorizontalLine(1,EditorGUIUtility.currentViewWidth, new Vector2(0,0), new Vector2(18,37), new Color(0,0,0));
								EditorGUILayout.Space(2);
							
								EditorGUILayout.PropertyField(Trigger_Prop);
								EditorGUILayout.PropertyField(indexTrigger_Prop);
								EditorGUILayout.PropertyField(CustomEvents_Prop);
							}
							break;

						case AdvanceOptions.none:
							GUILayout.Label("None", EditorStyles.boldLabel);
							break;

					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space(5);



				}
				else
				{
					LO.advanceOptions = AdvanceOptions.none;
				
				}

			}
			
			#endregion

			EditorGUILayout.HelpBox("You can manage the keys of the object from the Localization Editor Tool in ArthemyDevelopment/LocalizationTool, it also allow you to add the component automatically", MessageType.Info);
			
			EditorGUILayout.Space(15);
			GUILayout.FlexibleSpace();
			GUIEditorWindow.FooterLogo(new Vector2(107,143));
			EditorGUILayout.Space(143);

			serializedObject.ApplyModifiedProperties();
		}
	}

}
