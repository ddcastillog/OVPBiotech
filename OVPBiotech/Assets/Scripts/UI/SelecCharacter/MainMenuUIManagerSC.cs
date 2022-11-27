
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace OVPBiotechSpace
{
    // high-level manager for the various parts of the Main Menu UI. Here we use one master UXML and one UIDocument.
    // We allow the individual parts of the user interface to have separate UIDocuments if needed (but not shown in this example).
    
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUIManagerSC : MonoBehaviour
    {
        [Header("Modal Menu Screens")]
        [Tooltip("Only one modal interface can appear on-screen at a time.")]
        [SerializeField] SCScreen m_SCScreen;

        [Header("Toolbars")]
        [Tooltip("Toolbars remain active at all times unless explicitly disabled.")]
        [SerializeField] OptionsBarSC m_OptionsToolbar;

        [Header("Full-screen overlays")]
        [Tooltip("Full-screen overlays block other controls until dismissed.")]        
        [SerializeField] PauseScreenSC m_PauseScreen;
        UIDocument m_MainMenuDocument;
        List<MenuScreenSC> m_AllModalScreens = new List<MenuScreenSC>();
        public UIDocument MainMenuDocument => m_MainMenuDocument;
        void OnEnable()
        {
            m_MainMenuDocument = GetComponent<UIDocument>();
            SetupModalScreens();
            ShowSCScreen();
        }
        void SetupModalScreens()
        {
            if (m_SCScreen != null)
                m_AllModalScreens.Add(m_SCScreen);
           
        }
        // shows one screen at a time
        void ShowModalScreen(MenuScreenSC modalScreen)
        {
            foreach (MenuScreenSC m in m_AllModalScreens)
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
        public void ShowSCScreen()
        {
            ShowModalScreen(m_SCScreen);
        }
        void Start()
        {
            Time.timeScale = 1f;
        }             
        // overlay screen methods
        public void ShowSettingsScreen()
        {
            m_PauseScreen?.ShowScreen();
        }
    }     
    
}