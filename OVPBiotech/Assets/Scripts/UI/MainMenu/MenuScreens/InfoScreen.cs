using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OVPBiotechSpace
{

    // links to additional UI Toolkit resources
    public class InfoScreen : MenuScreen
    {
        [Header("URLs")]
        [SerializeField] string m_GetInfoURL = "https://biotecnologia.utn.edu.ec/";
        [SerializeField] string m_DocsURL = "https://biotecnologia.utn.edu.ec/";
        [SerializeField] string m_ForumURL = "https://biotecnologia.utn.edu.ec/";
        [SerializeField] string m_BlogURL = "https://biotecnologia.utn.edu.ec/";
        [SerializeField] string m_AssetStoreURL = "https://biotecnologia.utn.edu.ec/";

        const string k_GetInfoButton = "info-signup__button";
        const string k_DocsButton = "info-content__docs-button";
        const string k_ForumButton = "info-content__forum-button";
        const string k_BlogButton = "info-content__blog-button";
        const string k_AssetStoreButton = "info-content__asset-button";

        Button m_GetInfoButton;
        Button m_DocsButton;
        Button m_ForumButton;
        Button m_BlogButton;
        Button m_AssetStoreButton;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_GetInfoButton = m_Root.Q<Button>(k_GetInfoButton);
            m_DocsButton = m_Root.Q<Button>(k_DocsButton);
            m_ForumButton = m_Root.Q<Button>(k_ForumButton);
            m_BlogButton = m_Root.Q<Button>(k_BlogButton);
            m_AssetStoreButton = m_Root.Q<Button>(k_AssetStoreButton);
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();
            m_GetInfoButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_GetInfoURL));
            m_DocsButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_DocsURL));
            m_ForumButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_ForumURL));
            m_BlogButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_BlogURL));
            m_AssetStoreButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_AssetStoreURL));

        }

        static void OpenURL(string URL)
        {
            AudioManager.PlayDefaultButtonSound();
            Application.OpenURL(URL);
        }
    }
}