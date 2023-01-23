using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace OVPBiotechSpace
{
    [RequireComponent(typeof(SaveManager))]
    public class GameDataManager : MonoBehaviour
    {
        public static event Action<GameData> PotionsUpdated;
        public static event Action<GameData> FundsUpdated;             
       

        [SerializeField] GameData m_GameData;
        public GameData GameData { set => m_GameData = value; get => m_GameData; }

        SaveManager m_SaveManager;        

        void OnEnable()
        {            
            SettingsScreen.SettingsUpdated += OnSettingsUpdated;            
        }

        void OnDisable()
        {            
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
        
        // update values from SettingsScreen
        void OnSettingsUpdated(GameData gameData)
        {

            if (gameData == null)
                return;

            m_GameData.sfxVolume = gameData.sfxVolume;
            m_GameData.musicVolume = gameData.musicVolume;          
           
        }
    }
}
