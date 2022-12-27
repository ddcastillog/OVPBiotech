using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;


namespace OVPBiotechSpace
{
    // stores consumable data (resources)
    [System.Serializable]
    public class CategorySave
    {   

        public string id;
        public bool IsSelect;       
        // constructor, starting values
        public CategorySave( String id)
        {   // settings
            this.IsSelect = true;
            this.id = id;
        }
        // constructor, starting values
        public CategorySave(String id,bool IsSelect)
        {   // settings
            this.IsSelect = IsSelect;
            this.id = id;
        }
    }
}