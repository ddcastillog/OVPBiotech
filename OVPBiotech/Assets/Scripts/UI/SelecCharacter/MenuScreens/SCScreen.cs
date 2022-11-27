using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace OVPBiotechSpace
{
    public class SCScreen : MenuScreenSC
    {
        const string k_BtnStartGame = "btn-Start-game";
        Button m_BtnStartGame;
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_BtnStartGame = m_Root.Q<Button>(k_BtnStartGame);
            
        }

        protected override void RegisterButtonCallbacks()
        {
            m_BtnStartGame?.RegisterCallback<ClickEvent>(BtnStartGame);            
        }
        private void BtnStartGame(ClickEvent e)
        {
            SceneManager.LoadScene(2);
        }
    }
}
