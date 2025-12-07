using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq;
using TMPro;  

public class GameOptionManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider voiceSlider;

    public TMP_Dropdown resolutionDropdown;  
    private Resolution[] resolutions;

    public Image menuBackgroundImage;
    public Sprite pastBackgroundSprite;
    public Sprite futureBackgroundSprite;

    private const string KEY_MASTER = "MasterVol";
    private const string KEY_BGM = "BGMVol";
    private const string KEY_VOICE = "VoiceVol";
    private const string KEY_RES_WIDTH = "ResWidth";
    private const string KEY_RES_HEIGHT = "ResHeight";

    void Start()
    {
        InitResolutionOptions();
        LoadSettings();

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void OnEnable()
    {
        UpdateRoleImage();
    }

    public void SetMasterVolume(float value)
    {
        float db = Mathf.Log10(value) * 20;
        mainMixer.SetFloat("MasterVolume", db);
        PlayerPrefs.SetFloat(KEY_MASTER, value);
    }

    public void SetBGMVolume(float value)
    {
        float db = Mathf.Log10(value) * 20;
        mainMixer.SetFloat("BGMVolume", db);
        PlayerPrefs.SetFloat(KEY_BGM, value);
    }

    public void SetVoiceVolume(float value)
    {
        float db = Mathf.Log10(value) * 20;
        mainMixer.SetFloat("VoiceVolume", db);
        PlayerPrefs.SetFloat(KEY_VOICE, value);
    }

    void InitResolutionOptions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt(KEY_RES_WIDTH, resolution.width);
        PlayerPrefs.SetInt(KEY_RES_HEIGHT, resolution.height);
    }

    public void UpdateRoleImage()
    {
        if (menuBackgroundImage == null) return;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("character", out object characterValue))
        {
            string role = (string)characterValue;
            if (role == "Past" && pastBackgroundSprite != null)
            {
                menuBackgroundImage.sprite = pastBackgroundSprite;
            }
            else if (role == "Future" && futureBackgroundSprite != null)
            {
                menuBackgroundImage.sprite = futureBackgroundSprite;
            }
        }
    }

    void LoadSettings()
    {
        float mVol = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        float bVol = PlayerPrefs.GetFloat(KEY_BGM, 1f);
        float vVol = PlayerPrefs.GetFloat(KEY_VOICE, 1f);

        masterSlider.value = mVol;
        bgmSlider.value = bVol;
        voiceSlider.value = vVol;

        SetMasterVolume(mVol);
        SetBGMVolume(bVol);
        SetVoiceVolume(vVol);

        if (PlayerPrefs.HasKey(KEY_RES_WIDTH))
        {
            int width = PlayerPrefs.GetInt(KEY_RES_WIDTH);
            int height = PlayerPrefs.GetInt(KEY_RES_HEIGHT);
            Screen.SetResolution(width, height, Screen.fullScreen);
        }
    }
}