
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace OVPBiotechSpace
{
    // high-level manager for the various parts of the Main Menu UI. Here we use one master UXML and one UIDocument.
    // We allow the individual parts of the user interface to have separate UIDocuments if needed (but not shown in this example).
    
    [RequireComponent(typeof(UIDocument))]
    public class MenuUISelectCharacterManager : MonoBehaviour
    {   [Header("Toolbars")]
        [Tooltip("Toolbars remain active at all times unless explicitly disabled.")]
        [SerializeField] OptionsBar m_OptionsToolbar;

        [Header("Full-screen overlays")]
        [Tooltip("Full-screen overlays block other controls until dismissed.")]        
        [SerializeField] SettingsScreen m_SettingsScreen;
        UIDocument m_MainMenuDocument;
        public UIDocument MainMenuDocument => m_MainMenuDocument;
        void OnEnable()
        {
            m_MainMenuDocument = GetComponent<UIDocument>();     
        }

        void Start()
        {
            Time.timeScale = 1f;
        }             
        // overlay screen methods
        public void ShowSettingsScreen()
        {
            m_SettingsScreen?.ShowScreen();
        }

    }     
    
}