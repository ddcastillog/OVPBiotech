using UnityEngine;
using UnityEngine.UIElements;
using System;


namespace UIToolkitDemo
{
    // stores consumable data (resources)
    [System.Serializable]
    public class GameData
    {   

        public string username;   

        public bool musicVolume;
        public bool sfxVolume;    

        // constructor, starting values
        public GameData()
        {   // settings
            this.musicVolume = true;
            this.sfxVolume = true;
            this.username = "GUEST_123456";           
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadJson(string jsonFilepath)
        {
            JsonUtility.FromJsonOverwrite(jsonFilepath, this);
        }
    }
}