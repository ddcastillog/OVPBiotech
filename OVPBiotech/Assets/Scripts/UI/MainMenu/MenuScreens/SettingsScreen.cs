using System;
using UnityEngine;
using UnityEngine.UIElements;

using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace OVPBiotechSpace
{ 

    // This controls general settings for the game. Many of these options are non-functional in this demo but
    // show how to sync data from a UI with the GameDataManager.
    public class SettingsScreen : MenuScreen
    {
        public static event Action SettingsShown;
        public static event Action<GameData> SettingsUpdated;        

        // string IDs
        const string k_PanelBackButton = "settings-screen__back-button";        
        const string k_music = "settings-screen-music";
        const string k_sfx = "settings-screen-sfx";
        const string k_iconMusic = "icon-music";
        const string k_iconSfx = "icon-sfx";
        const string k_musicOff = "settings-music__off";
        const string k_SfxOff = "settings-sfx__off";        

        const string k_PanelActiveClass = "settings__panel";
        const string k_PanelInactiveClass = "settings__panel--inactive";        
        const string k_SettingsPanel = "settings__panel";
        const string k_SettingsOverlay = "settings-screen-overlay";
        

        
        Button m_Music;
        Button m_Sfx;
        VisualElement m_iconMusic;
        VisualElement m_iconSfx;
        VisualElement m_PanelBackButton;

        // root node for transitions
        VisualElement m_Panel;
        VisualElement m_SettingsOverlay;

        // temp storage to send back to GameDataManager
        GameData m_SettingsData;

        void OnEnable()
        {
            // sets m_SettingsData
            SaveManager.GameDataLoaded += OnGameDataLoaded;
        }

        void OnDisable()
        {
            SaveManager.GameDataLoaded -= OnGameDataLoaded;
        }
        public override void ShowScreen()
        {
            base.ShowScreen();

            // add active style
            m_Panel.RemoveFromClassList(k_PanelInactiveClass);
            m_Panel.AddToClassList(k_PanelActiveClass);

            // notify GameDataManager
            SettingsShown?.Invoke();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_PanelBackButton = m_Root.Q(k_PanelBackButton);
            m_Music = m_Root.Q<Button>(k_music);
            m_Sfx = m_Root.Q<Button>(k_sfx);
            m_iconMusic = m_Root.Q<VisualElement>(k_iconMusic);
            m_iconSfx = m_Root.Q<VisualElement>(k_iconSfx);
            m_Panel = m_Root.Q(k_SettingsPanel);
            m_SettingsOverlay = m_Root.Q(k_SettingsOverlay);
        }

        protected override void RegisterButtonCallbacks()
        {
            m_PanelBackButton?.RegisterCallback<ClickEvent>(ClosePanel);
            m_Music?.RegisterCallback<ClickEvent>(ChangeMusicVolume);
            m_Sfx?.RegisterCallback<ClickEvent>(ChangeSfxVolume);
            m_SettingsOverlay?.RegisterCallback<ClickEvent>(ClosePanel);
        }       

        void ChangeSfxVolume(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            m_SettingsData.sfxVolume = m_SettingsData.sfxVolume?false:true;
            if (m_SettingsData.sfxVolume)
            {
                m_iconSfx.RemoveFromClassList(k_SfxOff);
            }
            else
            {
                m_iconSfx.AddToClassList(k_SfxOff);
            }

            // notify the GameDataManager
            SettingsUpdated?.Invoke(m_SettingsData);
        }

        void ChangeMusicVolume(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            m_SettingsData.musicVolume = m_SettingsData.musicVolume ? false : true;
            if (m_SettingsData.musicVolume)
            {
                m_iconMusic.RemoveFromClassList(k_musicOff);
            }
            else
            {
                m_iconMusic.AddToClassList(k_musicOff);
            }

            // notify the GameDataManager
            SettingsUpdated?.Invoke(m_SettingsData);
        }
       
        void ClosePanel(ClickEvent evt)
        {
            m_Panel.RemoveFromClassList(k_PanelActiveClass);
            m_Panel.AddToClassList(k_PanelInactiveClass);

            AudioManager.PlayDefaultButtonSound();          
            
            SettingsUpdated?.Invoke(m_SettingsData);

            HideScreen();
        }       

        // syncs saved data from the GameDataManager to the UI elements
        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;

            m_SettingsData = gameData;
            if (!gameData.musicVolume)
            {
                m_iconMusic.AddToClassList(k_musicOff);
            }
            if (!gameData.sfxVolume)
            {
                m_iconSfx.AddToClassList(k_SfxOff);
            }     

            SettingsUpdated?.Invoke(m_SettingsData);
        }
    }
}