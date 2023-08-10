using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    public TMP_InputField _nickNameInputField;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
            _nickNameInputField.text = PlayerPrefs.GetString("PlayerNickname");
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString("PlayerNickname", _nickNameInputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Game");
    }
}
