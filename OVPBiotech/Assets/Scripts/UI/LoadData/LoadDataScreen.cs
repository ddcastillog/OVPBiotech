using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using UnityEngine.SceneManagement;
namespace OVPBiotechSpace
{
    public enum NumberScenes
    {
        LOAD_DATA,
        MAIN_MENU,
        SELECT_CHARACTER,
        QUESTIONGAME,

    }
    public class LoadDataScreen : MonoBehaviour
    {
        const string k_MessageLabel = "lbl_message";
        public static event Action<GameData> SettingsUpdated;
        Label m_MessageLabel;
        [Header("Path")]
        [SerializeField]
        private string pathquestion = "question";
        [SerializeField]
        private string pathdifficultyLevel = "difficultyLevel";
        [SerializeField]
        private string pathcategory = "category";
        [SerializeField]
        private string pathdPostChange = "PostChange";
        [SerializeField]
        private string pathdPostChangeID = "date-change";
        GameData m_SettingsData;
        VisualElement m_Root;
        protected  void SetVisualElements()
        {
            m_Root = GetComponent<UIDocument>().rootVisualElement;
            m_MessageLabel = m_Root.Q<Label>(k_MessageLabel);
        }
        void OnEnable()
        {
           
            SaveManager.GameDataLoaded += OnGameDataLoaded;
        }

        void OnDisable()
        {
            SaveManager.GameDataLoaded -= OnGameDataLoaded;
        }
        private void Awake()
        {
            SetVisualElements();
            StartCoroutine("AnimationText");
        }
        private void Start()
        {            
            GetData();
        }
        async void GetData()
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentSnapshot categorys;
            categorys = await db.Collection(pathdPostChange).Document(pathdPostChangeID).GetSnapshotAsync();
            if (categorys.Exists)
            {
                PostChange postChnage = categorys.ConvertTo<PostChange>();
                if (postChnage.date_publish.ToString() != m_SettingsData.date_publish)
                {
                    m_SettingsData.date_publish = postChnage.date_publish.ToString();
                    SettingsUpdated?.Invoke(m_SettingsData);
                    await db.TerminateAsync();
                    await db.ClearPersistenceAsync();
                    db = FirebaseFirestore.DefaultInstance;
                    await db.Collection(pathquestion).WhereEqualTo("published", true).GetSnapshotAsync();
                    await db.Collection(pathdifficultyLevel).GetSnapshotAsync();
                    await db.Collection(pathcategory).GetSnapshotAsync();
                }
                SceneManager.LoadSceneAsync((int)NumberScenes.MAIN_MENU);
            }
            else
            {
                StopCoroutine("AnimationText");
                m_MessageLabel.text = "<color=#ff0000ff>No hay Datos</color>";
            }

        }
        // syncs saved data from the GameDataManager to the UI elements
        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;
            m_SettingsData = gameData;
            SettingsUpdated?.Invoke(m_SettingsData);
        }
        IEnumerator AnimationText()
        {
            while (true)
            {
                m_MessageLabel.text = "Cargando ";
                yield return new WaitForSeconds(.5f);
                for (int i = 0; i < 4; i++)
                {
                    m_MessageLabel.text += ". ";
                    yield return new WaitForSeconds(.5f);
                }
            }                        
        }
    }
}
