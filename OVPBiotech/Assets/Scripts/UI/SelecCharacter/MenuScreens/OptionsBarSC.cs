using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace OVPBiotechSpace
{
    public class OptionsBarSC : MenuScreenSC
    {
        // string IDs
        const string k_OptionsButton = "options-bar__button"; 

        VisualElement m_OptionsButton;  

        // identify visual elements by name
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_OptionsButton = m_Root.Q(k_OptionsButton);           
        }

        // set up button click events
        protected override void RegisterButtonCallbacks()
        {
            m_OptionsButton?.RegisterCallback<ClickEvent>(ShowOptionsScreen);           
        }

        void ShowOptionsScreen(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            m_MainMenuUIManager?.ShowSettingsScreen();
        }       
    }
}