using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class InGameMenuController : MonoBehaviour
{
    // Class for the in game GUI based on UI Toolkit

    public Button resumeButton;
    public Button restartButton;
    public Button optionsButton;
    public Button mainMenuButton;
    public Button backButton;

    public VisualElement buttonsMenu;
    public VisualElement optionsMenu;
    public VisualElement inGameMenu;
    public VisualElement fireballIcon;
    public VisualElement inGameGUI;

    public Slider soundSlider;
    public Slider musicSlider;

    public Label waveLabel;
    public Label cashLabel;

    private Quaternion cameraRotation = Quaternion.identity;

    private MainManager manager;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        resumeButton = root.Q<Button>("Resume");
        restartButton = root.Q<Button>("Reset");
        optionsButton = root.Q<Button>("Options");
        mainMenuButton = root.Q<Button>("MainMenu");
        backButton = root.Q<Button>("Back");

        buttonsMenu = root.Q<VisualElement>("ButtonsMenu");
        optionsMenu = root.Q<VisualElement>("OptionsMenu");
        inGameMenu = root.Q<VisualElement>("InGameMenu");
        fireballIcon = root.Q<VisualElement>("Fireball");
        inGameGUI = root.Q<VisualElement>("InGameGUI");

        soundSlider = root.Q<Slider>("SoundVolume");
        musicSlider = root.Q<Slider>("MusicVolume");
        soundSlider.RegisterValueChangedCallback(OnSoundSliderValueChanged);
        musicSlider.RegisterValueChangedCallback(OnMusicSliderValueChanged);

        waveLabel = root.Q<Label>("Waves");
        cashLabel = root.Q<Label>("Cash");

        resumeButton.clicked += ResumeButtonPressed;
        restartButton.clicked += RestartButtonPressed;
        optionsButton.clicked += OptionsButtonPressed;
        mainMenuButton.clicked += MainMenuButtonPressed;
        backButton.clicked += BackButtonPressed;

        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MainManager>();
    }

    void Update()
    {
        if (cameraRotation != Quaternion.identity)
        {
            Camera.main.transform.rotation = cameraRotation;
        }
        if (Input.GetButtonDown("Cancel"))
        {
            if (inGameMenu.style.display == DisplayStyle.Flex)
            {
                ResumeButtonPressed();
            }
            else
            {
                // Make according menus visible and invisible
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                cameraRotation = Camera.main.transform.rotation;
                Time.timeScale = 0f;
                inGameMenu.style.display = DisplayStyle.Flex;
                inGameGUI.style.display = DisplayStyle.None;
            }
        }
    }

    void ResumeButtonPressed()
    {
        // Same as above, but reversed
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        Time.timeScale = 1f;
        inGameMenu.style.display = DisplayStyle.None;
        inGameGUI.style.display = DisplayStyle.Flex;
        cameraRotation = Quaternion.identity;
    }

    void RestartButtonPressed()
    {
        manager.ChangeScene("MainScene");
    }

    void OptionsButtonPressed()
    {
        buttonsMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.Flex;
    }

    void MainMenuButtonPressed()
    {
        manager.ChangeScene("MainMenu");
    }

    void BackButtonPressed()
    {
        buttonsMenu.style.display = DisplayStyle.Flex;
        optionsMenu.style.display = DisplayStyle.None;
    }

    void OnSoundSliderValueChanged(ChangeEvent<float> evt)
    {
        manager.soundVolume = evt.newValue;
    }

    void OnMusicSliderValueChanged(ChangeEvent<float> evt)
    {
        manager.musicVolume = evt.newValue;
    }

    public void UpdateCash(int value)
    {
        cashLabel.text = "Cash: " + value.ToString(); // Update cash value, either after an enemy kill, or after an ally buy
    }

    public void UpdateWave(int value)
    {
        // Updated via Enemy Spawners
        waveLabel.text = "Wave: " + value.ToString() + "/24";
        if (value == 25)
        {
            waveLabel.text = "Victory!";
        }
        else if (value == -1)
        {
            waveLabel.text = "You lost! Try again?";
        }
    }
}
