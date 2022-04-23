using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;


namespace ArthemyDevelopment.Localization
{
    [System.Serializable]
    public class ForCustomComponentClass : UnityEvent<string> { }

    public class LocalizationObject : MonoBehaviour
    {
        
        
        public LocalizationManager LM;

        public bool B_alreadyStart;

        [Tooltip("Select the object to localize. \nIf the object is a button you will have access to advanced options. If the object is an image or a texture, the localization text in the file must be the name of the image inside the Resources folder.")]
        public Components AffectedComponents = Components.Text;

        [Tooltip("Input the existing reference key in the file.")]
        public string key;

        [Tooltip("Select the strings to set in your custom component, for more information on how this option works see documentation")]
        public List<string> keys;

        [Tooltip("Toggle to use the advance option for the button object.")]
        public bool useAdvanceOptions;

        [Tooltip("Select which advance option to use.\nLanguage selection allows you to change the active language in the game with the button, in this case, a key will not be needed.\nCustom Events allow you to have other methods trigger when the button is press, to know the difference from the OnClick event see the documentation.")]
        public AdvanceOptions advanceOptions;

        [Tooltip("The index of the language that will be selected, must be the same as the LanguageManager component that holds the language files.")]
        public int LocalizationIndex;
        [Tooltip("Allow you to remember the selected language, it will trigger the OnLanguageSelection event automatically when the button is enable.\nIf you want to access the language selection screen again in-game see the documentation.")]
        public bool rememberConfiguration;
        [Tooltip("The events that will be triggered when the button is pressed, to know the difference with the OnClick event see the documentation.")]
        public UnityEvent OnLanguageSelection;

        [Tooltip("Allow you to set if the OnCustomEvent event will be triggered automatically when the button is enabled, to know the difference with the OnClick event see the documentation.")]
        public bool triggerAutomatically;

        [Tooltip("Set a specific index of the CustomEvent for the trigger automatically option, left at 0 for generic trigger")]
        public int triggerIndex;
        [Tooltip("The events that will be triggered when the button is pressed, to know the difference with the OnClick event see the documentation.")]
        public UnityEvent OnCustomEvent;

        [Tooltip("Allow you to set multiple strings if your component uses a collection, for more information on how this option works see the documentation.")]
        public bool multipleStrings = false;

        [Tooltip("Add to the event the methods of your custom component that store the text.")]
        public ForCustomComponentClass ForCustomComponent;


		private void Start()
		{
            LM = LocalizationManager.current;
            SetLocalizedObject();
            B_alreadyStart = true;
		}

		void OnEnable()
        {
            if(B_alreadyStart)
                SetLocalizedObject();
        }

        public void ClearLanguageSelection()
		{
            if(PlayerPrefs.HasKey("ADLocalizationIndex"))
			{
                 PlayerPrefs.DeleteKey("ADLocalizationIndex");
                PlayerPrefs.DeleteKey("ADLanguage");

			}
        }

        public void ClearCustomEventTrigger()
		{
            if (PlayerPrefs.HasKey("CustomEventTrigger" + triggerIndex))
                PlayerPrefs.DeleteKey("CustomEventTrigger"+triggerIndex);
		}

        void SetLocalizedObject()
		{
           
            
            TMP_Text Text;
            switch (AffectedComponents)
			{
                case Components.Text:
                        Text = GetComponent<TMP_Text>();
                        Text.text = LM.GetLocalizationValue(key);
                        break;

                case Components.ButtonUI:
                    
                    switch(advanceOptions)
					{
                        case AdvanceOptions.SetLanguage:
                            Text = transform.GetChild(0).GetComponent<TMP_Text>();
                            Text.text = LM.currentsLanguages[LocalizationIndex].S_Name;
                            Button tempBtn = GetComponent<Button>();
                            tempBtn.onClick.AddListener(() => LM.LoadTextFile(LM.currentsLanguages[LocalizationIndex].S_FileName));
                            tempBtn.onClick.AddListener(() => LM.SaveDefault(LocalizationIndex));
                            if (rememberConfiguration)
                            {

                                if (PlayerPrefs.HasKey("ADLocalizationIndex"))
                                {
                                    LM.LoadDefault();
                                    OnLanguageSelection.Invoke();


                                }
                                else
                                {

                                    tempBtn.onClick.AddListener(() => OnLanguageSelection.Invoke());
                                }
                            }
                            else
                            {

                                tempBtn.onClick.AddListener(() => OnLanguageSelection.Invoke());
                            }
                            
                            break;

                        case AdvanceOptions.CustomEvents:
                            Text = transform.GetChild(0).GetComponent<TMP_Text>();
                            Text.text = LM.GetLocalizationValue(key);
                            Button tempBtn2 = GetComponent<Button>();
                            tempBtn2.onClick.AddListener(() => OnCustomEvent.Invoke());
                            tempBtn2.onClick.AddListener(() => LM.CustomEventTrigger(triggerIndex));
                            if (triggerAutomatically && PlayerPrefs.HasKey("CustomEventTrigger"+triggerIndex))
                            {
                                OnCustomEvent.Invoke();
                            }                           

                            break;

                        case AdvanceOptions.none:
                            Text = transform.GetChild(0).GetComponent<TMP_Text>();
                            Text.text = LM.GetLocalizationValue(key);
                            break;
                    }
                    
                    break;

                case Components.Image:
                    Image _Image = GetComponent<Image>();
                    _Image.sprite = Resources.Load<Sprite>(LM.GetLocalizationValue(key));                    

                    break;

                case Components.CustomComponent:
                                        
                    if(!multipleStrings)
                        ForCustomComponent.Invoke(LM.GetLocalizationValue(key));
                    else if(multipleStrings)
					{                        
						for (int i = 0; i < keys.Count; i++)
						{
                            ForCustomComponent.Invoke(LM.GetLocalizationValue(keys[i]));
						}
					}
                    break;

            }

        }

    }

   

}
