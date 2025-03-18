using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button settingsBack;
    public Button restart;
    public GameObject mainMenu;
    public GameObject gui;
    public GameObject settingsUI;
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        Button start = startButton.GetComponent<Button>();
        Button settings = settingsButton.GetComponent<Button>();
        Button backSetting = settingsBack.GetComponent<Button>();
        Button res = restart.GetComponent<Button>();

        start.onClick.AddListener(StartButton);
        settings.onClick.AddListener(SettingButton);
        backSetting.onClick.AddListener(SettingBack);
        res.onClick.AddListener(Restart);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton() 
    {
        mainMenu.SetActive(false);
        gui.SetActive(true);
        player.immobile = false;
        player.RemainingTime = player.startingTime;
    }

    private void SettingButton()
    {
        mainMenu.SetActive(false);
        settingsUI.SetActive(true);
    }

    private void SettingBack()
    {
        mainMenu.SetActive(true);
        settingsUI.SetActive(false);
    }

    private void Restart()
    {
        SceneManager.LoadScene("Level1 Alternate", LoadSceneMode.Single);
    }
}
