using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;
namespace OVPBiotechSpace
{
    public class QGScoreScreen : MenuScreenQG
    {
        public static event Action ReplayGame;
        const string k_lblScore= "lbl-score";
        const string k_lblReplay = "btn-replay";
        const string k_lblBack = "btn-back";
        Label m_lblScore;
        Button m_lblReplay;
        Button m_lblBack;
        void OnEnable()
        {
            QGScreen.updateScore += setScore;
        }
        void OnDisable()
        {
            QGScreen.updateScore -= setScore;
        }
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_lblScore = m_Root.Q<Label>(k_lblScore);
            m_lblReplay = m_Root.Q<Button>(k_lblReplay);
            m_lblBack = m_Root.Q<Button>(k_lblBack);
        }
        void setScore(String score)
        {
            m_lblScore.text = score;
        }

        protected override void RegisterButtonCallbacks()
        {
            m_lblReplay?.RegisterCallback<ClickEvent>(Replay);
            m_lblBack?.RegisterCallback<ClickEvent>(back);
        }
        void back(ClickEvent e)
        {
            SceneManager.LoadScene(0);
        }
        void Replay(ClickEvent e)
        {
            ReplayGame.Invoke();
            HideScreen();
        }
    }
}
