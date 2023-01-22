using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
namespace OVPBiotechSpace
{
    public class PlayGameManager : MonoBehaviour
    {
        public static PlayGameManager instance;
        static bool active = false;        
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            if (!active)
            {
                DontDestroyOnLoad(this);
                active = true;
            }
            else
            {
                Destroy(this);
            }
#if PLATFORM_ANDROID            
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate((bool succces) => { });            
#endif
        }        
        public void ShowLeaderboarScreen()
        {
#if PLATFORM_ANDROID
            if (Social.localUser.authenticated)
                Social.ShowLeaderboardUI();
#endif
        }
        public void sendScore(int score)
        {
#if PLATFORM_ANDROID
            if (Social.localUser.authenticated)
                Social.ReportScore(score, GPGSIds.leaderboard_clasificacin, (bool success) =>{                    
                });
#endif
        }
    }
}
