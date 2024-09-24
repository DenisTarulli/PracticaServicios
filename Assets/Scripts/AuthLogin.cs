using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Collections;

public class AuthLogin : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    private FirebaseAuth auth;
    public FirebaseUser user;

    [Header("Login")]
    [SerializeField] private TMP_InputField emailLoginField;
    [SerializeField] private TMP_InputField passwordLoginField;
    [SerializeField] private TMP_Text warningLoginText;
    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject registerUI;


    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.Log("could not resolve firebase dependency" + dependencyStatus);
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        { 
            if (loginUI.activeInHierarchy)
            {
                loginUI.SetActive(false);
                registerUI.SetActive(true);
            }
            else
            {
                loginUI.SetActive(true);
                registerUI.SetActive(false);
            }
        }
    }


    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {

        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);


        if (LoginTask.Exception != null)
        {
            Debug.Log(message: $"Failed To register task with{LoginTask.Exception}");
            FirebaseException firebaseEX = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEX.ErrorCode;

            string message = "Login Failed:" + errorCode;
            warningLoginText.text = message;
        }
        else
        {
            //Firebase.Auth.AuthResult result = RegisterTask.Result;

            //user = result.User;
            //Debug.LogFormat("Firebase user sign-in successfully: {0} ({1})", user.DisplayName, user.Email);
        }
    }
}
