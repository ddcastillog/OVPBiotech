using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(SaveManager))]
    public class GameDataManager : MonoBehaviour
    {
        public static event Action<GameData> PotionsUpdated;
        public static event Action<GameData> FundsUpdated;              
        public static event Action<string> HomeMessageShown;

        [SerializeField] GameData m_GameData;
        public GameData GameData { set => m_GameData = value; get => m_GameData; }

        SaveManager m_SaveManager;
        bool m_IsGameDataInitialized;

        void OnEnable()
        {
            HomeScreen.HomeScreenShown += OnHomeScreenShown;
            SettingsScreen.SettingsUpdated += OnSettingsUpdated;
            
        }

        void OnDisable()
        {
            HomeScreen.HomeScreenShown -= OnHomeScreenShown;
            SettingsScreen.SettingsUpdated -= OnSettingsUpdated;
           
        }

        void Awake()
        {
            m_SaveManager = GetComponent<SaveManager>();
        }

        void Start()
        {
            //if saved data exists, load saved data
            m_SaveManager?.LoadGame();

            // flag that GameData is loaded the first time
            m_IsGameDataInitialized = true;
            ShowWelcomeMessage();
            UpdateFunds();
            UpdatePotions();
        }

        // transaction methods 
        void UpdateFunds()
        {
            if (m_GameData != null)
                FundsUpdated?.Invoke(m_GameData);
        }

        void UpdatePotions()
        {
            if (m_GameData != null)
                PotionsUpdated?.Invoke(m_GameData);
        }
        
        void ShowWelcomeMessage()
        {
            string message = "Welcome "  + GameData.username;
            HomeMessageShown?.Invoke(message);
        }
        // update values from SettingsScreen
        void OnSettingsUpdated(GameData gameData)
        {

            if (gameData == null)
                return;

            m_GameData.sfxVolume = gameData.sfxVolume;
            m_GameData.musicVolume = gameData.musicVolume;          
           
        }       

        void OnHomeScreenShown()
        {
            if (m_IsGameDataInitialized)
            {
                ShowWelcomeMessage();
            }
        }     

    }
}
