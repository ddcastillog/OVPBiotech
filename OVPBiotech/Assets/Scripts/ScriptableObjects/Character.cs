using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace OVPBiotechSpace
{
    // holds basic level information (label name, level number, scene name for loading, thumbnail graphic for display, etc.)
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Characters/Character", menuName = "UIToolkitDemo/Character", order = 11)]
    public class Character : ScriptableObject
    {
        public string name;
        public string description;
        public Sprite btnImage;
    }
}
