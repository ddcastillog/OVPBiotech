
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace OVPBiotechSpace
{
    // high-level manager for the various parts of the Main Menu UI. Here we use one master UXML and one UIDocument.
    // We allow the individual parts of the user interface to have separate UIDocuments if needed (but not shown in this example).
    
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUIManagerQG : MonoBehaviour
    {
        [Header("Modal Menu Screens")]
        [Tooltip("Only one modal interface can appear on-screen at a time.")]
        [SerializeField] QGScreen m_QGScreen;

        [Header("Toolbars")]
        [Tooltip("Toolbars remain active at all times unless explicitly disabled.")]
        [SerializeField] OptionsBarQG m_OptionsToolbar;

        [Header("Full-screen overlays")]
        [Tooltip("Full-screen overlays block other controls until dismissed.")]        
        [SerializeField] PauseScreenQG m_PauseScreen;
        UIDocument m_MainMenuDocument;
        List<MenuScreenQG> m_AllModalScreens = new List<MenuScreenQG>();
        public UIDocument MainMenuDocument => m_MainMenuDocument;
        void OnEnable()
        {
            m_MainMenuDocument = GetComponent<UIDocument>();
            SetupModalScreens();
            ShowSCScreen();
        }
        void SetupModalScreens()
        {
            if (m_QGScreen != null)
                m_AllModalScreens.Add(m_QGScreen);
           
        }
        // shows one screen at a time
        void ShowModalScreen(MenuScreenQG modalScreen)
        {
            foreach (MenuScreenQG m in m_AllModalScreens)
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
            ShowModalScreen(m_QGScreen);
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