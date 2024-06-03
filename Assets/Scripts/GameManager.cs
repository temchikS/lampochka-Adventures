using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject panelSettings;
    public GameObject deathPanel;

    private void Awake()
    {
        instance = this;
    }
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
    public void PlayAgain()
    {
        NPCSpawner.instance.DespawnAllMobs();
        Player.instance.RevivePlayer();
        CaveGenerator.instance.GenerateDungeon();
        deathPanel.SetActive(false);
    }
    public void DeathPanel()
    {
        deathPanel.SetActive(true);
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Settings()
    {
        panelSettings.SetActive(!panelSettings.activeSelf);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
