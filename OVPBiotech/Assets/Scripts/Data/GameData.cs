using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using Firebase.Firestore;

namespace OVPBiotechSpace
{
    // stores consumable data (resources)
    [System.Serializable]
    public class GameData
    {   

        public string username;
        public int selectCharacter;
        public bool musicVolume;
        public bool sfxVolume;
        public List<CategorySave> categoryList;
        public string date_publish;


        // constructor, starting values
        public GameData()
        {   // settings
            this.musicVolume = true;
            this.sfxVolume = true;
            this.username = "GUEST_123456";
            this.selectCharacter = 0;
            this.categoryList = new List<CategorySave>();
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