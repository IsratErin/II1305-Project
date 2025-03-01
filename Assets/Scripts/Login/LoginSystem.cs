using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class LoginSystem : MonoBehaviour
{

    EventSystem system;

    // Buttons
    public Button loginButton;
    public Button registerButton;
    public Button passwordResetButton;

    // InputFields
    public TMP_InputField mailInputField;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;


    public TMP_Text message;

    public static string EntityId;
    public static string SessionTicket;

    public static string PlayerUsername;

    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
        loginButton.onClick.AddListener(LoginButton);
        registerButton.onClick.AddListener(RegisterButton);
        passwordResetButton.onClick.AddListener(PasswordResetButton);
        PlayFabSettings.staticSettings.TitleId = "8E20E";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null) {
                next.Select();
            }

        } else if (Input.GetKeyDown(KeyCode.Return)) {
            loginButton.onClick.Invoke();
            Debug.Log("Pressed");
        }
    }

    // Button Handling Methods
    public void LoginButton() {
        var request = new LoginWithEmailAddressRequest {
            Email = mailInputField.text,
            Password = passwordInput.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
        
    }

    //NOT IN USE
    public void LoginByUsernameButton() {
        SceneManager.LoadScene (sceneName:"GameBoardScene");
        var request = new LoginWithPlayFabRequest { Username = usernameInput.text, Password = passwordInput.text };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }
    public void RegisterButton() {
        var request = new RegisterPlayFabUserRequest {
            Email = mailInputField.text,
            Username = usernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = true,
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);

    }
    public void PasswordResetButton() {
        var request = new SendAccountRecoveryEmailRequest {
            Email = mailInputField.text,
            TitleId = "8DC0F"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);

    }

    // Success and Error Messages
    void OnLoginSuccess (LoginResult result) {
        PlayerUsername = usernameInput.text;
        Debug.Log("Logged In");
        SceneManager.LoadScene (sceneName:"GameBoardScene");
        SessionTicket = result.SessionTicket;
        EntityId = result.EntityToken.Entity.Id;
    }
    void OnRegisterSuccess (RegisterPlayFabUserResult result) {
        SessionTicket = result.SessionTicket;
        EntityId = result.EntityToken.Entity.Id;
        message.SetText("Account Created");
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result){
        Debug.Log("Updated usernmae to display name");
    }
    void OnPasswordReset (SendAccountRecoveryEmailResult result) {
        message.SetText("Recovery Email Sent");
    }
    void OnError(PlayFabError error) {
        message.SetText(error.ErrorMessage);
    }


  


}

