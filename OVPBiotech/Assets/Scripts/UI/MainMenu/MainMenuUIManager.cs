using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Serialization;
using System;


namespace OVPBiotechSpace
{
    // high-level manager for the various parts of the Main Menu UI. Here we use one master UXML and one UIDocument.
    // We allow the individual parts of the user interface to have separate UIDocuments if needed (but not shown in this example).

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUIManager : MonoBehaviour
    {

        [Header("Modal Menu Screens")]
        [Tooltip("Only one modal interface can appear on-screen at a time.")]
        [SerializeField] HomeScreen m_HomeModalScreen;
        [SerializeField] InfoScreen m_InfoModalScreen;
        [SerializeField] CategoryScreen m_CategoryModalScreen;

        [Header("Toolbars")]
        [Tooltip("Toolbars remain active at all times unless explicitly disabled.")]
        [SerializeField] OptionsBar m_OptionsToolbar;
        [SerializeField] MenuBar m_MenuToolbar;

        [Header("Full-screen overlays")]
        [Tooltip("Full-screen overlays block other controls until dismissed.")]
        [SerializeField] SettingsScreen m_SettingsScreen;       


        List<MenuScreen> m_AllModalScreens = new();

        UIDocument m_MainMenuDocument;
        public UIDocument MainMenuDocument => m_MainMenuDocument;

        void OnEnable()
        {
            m_MainMenuDocument = GetComponent<UIDocument>();
            SetupModalScreens();            
            ShowHomeScreen();
        }

        void Start()
        {
            Time.timeScale = 1f;            
        }       
        void SetupModalScreens()
        {
            if (m_HomeModalScreen != null)
                m_AllModalScreens.Add(m_HomeModalScreen);
            if (m_InfoModalScreen != null)
                m_AllModalScreens.Add(m_InfoModalScreen);
            if (m_CategoryModalScreen != null)
                m_AllModalScreens.Add(m_CategoryModalScreen);
        }

        // shows one screen at a time
        void ShowModalScreen(MenuScreen modalScreen)
        {
            foreach (MenuScreen m in m_AllModalScreens)
            {
                if (m == modalScreen)
                {
                    m?.ShowScreen();
                }
                else
                {
                    m?.HideScreen();
                }
            }
        }


        // modal screen methods 
        public void ShowHomeScreen()
        {
            ShowModalScreen(m_HomeModalScreen);
        }

        public void ShowInfoScreen()
        {
            ShowModalScreen(m_InfoModalScreen);
        }
        public void ShowCategoryScreen()
        {
            ShowModalScreen(m_CategoryModalScreen);
        }

        // overlay screen methods
        public void ShowSettingsScreen()
        {
            m_SettingsScreen?.ShowScreen();
        }
        

    }

}