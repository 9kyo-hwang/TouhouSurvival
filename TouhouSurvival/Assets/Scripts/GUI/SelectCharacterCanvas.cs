using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SelectCharacterCanvas : UnchordCanvas
    {
        private Button _btnBack;
        private Button _btnStart;
        private Transform _characterButtonContainer;

        protected override void Awake()
        {
            base.Awake();

            _btnBack = transform.Find("DescriptionPanel/Navigators/BackButton").GetComponent<Button>();
            _btnStart = transform.Find("DescriptionPanel/Navigators/StartButton").GetComponent<Button>();
            _characterButtonContainer = transform.Find("CharacterPanel/CharacterButtons");

            _btnBack.onClick.AddListener(OnBackButtonClick);
            _btnStart.onClick.AddListener(OnStartButtonClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _btnStart.interactable = false;

            s_gameManager.LoadPlayerPrefabs();

            int pCount = s_gameManager.PlayerPrefabs.Length;
            int bCount = _characterButtonContainer.childCount;
            GameObject buttonPrefab = _characterButtonContainer.GetChild(0).gameObject;

            for (int i = 0; i < pCount; ++i)
            {
                if (bCount - 1 <= i)
                {
                    GameObject newButton = GameObject.Instantiate(buttonPrefab, _characterButtonContainer, false);
                    newButton.name = $"CharacterButtonBase ({i + 1})";
                }

                Transform child = _characterButtonContainer.GetChild(i + 1);
                Button button = child.Find("Button").GetComponent<Button>();
                TextMeshProUGUI textComponent = child.Find("Text").GetComponent<TextMeshProUGUI>();

                int playerIndex = i;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCharacterButtonClick(button, playerIndex));

                // button.image = playableCharacterResources[i].icon;
                textComponent.text = s_gameManager.PlayerPrefabs[i].name;
                child.gameObject.SetActive(true);
            }

            for (int i = pCount + 1; i < bCount; ++i)
            {
                _characterButtonContainer.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void OnBackButtonClick()
        {
            this.Hide();
            s_uiManager.LobbyCanvas.Show();
        }

        private void OnStartButtonClick()
        {
            this.Hide();
            s_gameManager.StartGame();
        }

        private void OnCharacterButtonClick(Button button, int playerIndex)
        {
            if (s_gameManager.PlayerPrefabIndex == playerIndex)
            {
                s_gameManager.PlayerPrefabIndex = -1;
                _btnStart.interactable = false;
            }
            else
            {
                s_gameManager.PlayerPrefabIndex = playerIndex;
                _btnStart.interactable = true;
            }
        }
    }
}