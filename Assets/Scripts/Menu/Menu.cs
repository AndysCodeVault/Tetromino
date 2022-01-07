using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private GameObject m_playerControllerPanel;
    private GameObject m_player1Control;
    private GameObject m_player2Control;
    private GameObject m_player3Control;
    private GameObject m_player4Control;
    private GameObject m_overlay;

    private void Awake()
    {
        m_overlay = GameObject.Find("Overlay");

        // Find all player controls panels
        m_player1Control = GameObject.Find("Player 1 Controls");
        m_player2Control = GameObject.Find("Player 2 Controls");
        m_player3Control = GameObject.Find("Player 3 Controls");
        m_player4Control = GameObject.Find("Player 4 Controls");

        // Find all player controls dropdowns
        TMP_Dropdown playerMode = GameObject.Find("Select Player").GetComponent<TMP_Dropdown>();        
        TMP_Dropdown player1Controls = GameObject.Find("Player 1 Controls/Player Controls").GetComponent<TMP_Dropdown>();        
        TMP_Dropdown player2Controls = GameObject.Find("Player 2 Controls/Player Controls").GetComponent<TMP_Dropdown>();        
        TMP_Dropdown player3Controls = GameObject.Find("Player 3 Controls/Player Controls").GetComponent<TMP_Dropdown>();        
        TMP_Dropdown player4Controls = GameObject.Find("Player 4 Controls/Player Controls").GetComponent<TMP_Dropdown>();
        
        // Set the default values
        playerMode.value = (int)Settings.mode;
        player1Controls.value = (int)Settings.player1Controls;
        player2Controls.value = (int)Settings.player2Controls;
        player3Controls.value = (int)Settings.player3Controls;
        player4Controls.value = (int)Settings.player4Controls;
        
        // Get player Controls Panel
        m_playerControllerPanel = GameObject.Find("Player Controls");

        UpdateMenuDisplay();
    }

   private void UpdateMenuDisplay()
   {
        switch(Settings.mode)
        {
            case PlayerMode.Single:
                m_playerControllerPanel.SetActive(false);
                break;
            case PlayerMode.TwoPlayers:
                m_playerControllerPanel.SetActive(true);
                m_player1Control.SetActive(true);
                m_player2Control.SetActive(true);
                m_player3Control.SetActive(false);
                m_player4Control.SetActive(false);
                break;
            case PlayerMode.ThreePlayers:
                m_playerControllerPanel.SetActive(true);
                m_player1Control.SetActive(true);
                m_player2Control.SetActive(true);
                m_player3Control.SetActive(true);
                m_player4Control.SetActive(false);
                break;
            case PlayerMode.FourPlayers:            
                m_playerControllerPanel.SetActive(true);
                m_player1Control.SetActive(true);
                m_player2Control.SetActive(true);
                m_player3Control.SetActive(true);
                m_player4Control.SetActive(true);
                break;
        }
    }

    public void OnSelectPlayersChanged(TMP_Dropdown players)
    {
        PlayerMode mode = (PlayerMode)players.value;
        Settings.mode = mode;
        UpdateMenuDisplay();
    }

    public void OnPlayer1ControlsChanged(TMP_Dropdown control)
    {
        Settings.player1Controls = (ControlType)control.value;
    }

    public void OnPlayer2ControlsChanged(TMP_Dropdown control)
    {
        Settings.player2Controls = (ControlType)control.value;
    }

    public void OnPlayer3ControlsChanged(TMP_Dropdown control)
    {
        Settings.player3Controls = (ControlType)control.value;
    }

    public void OnPlayer4ControlsChanged(TMP_Dropdown control)
    {
        Settings.player4Controls = (ControlType)control.value;
        CheckSettings();
    }

    private void CheckSettings()
    {
        print(Settings.mode);
        print(Settings.player1Controls);
        print(Settings.player2Controls);
        print(Settings.player3Controls);
        print(Settings.player4Controls);
    }

    public void OnStartButtonClicked()
    {
        m_overlay.GetComponent<Fade>().BeginFadeOut();
        Invoke("StartNextScene", 1f);
    }

    private void StartNextScene()
    {        
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
