using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Collections;

public class AuthRegister: MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Header("Register")]
    [SerializeField] private TMP_InputField usernameRegisterField;
    [SerializeField] private TMP_InputField emailRegisterField;
    [SerializeField] private TMP_InputField passwordRegisterField;
    [SerializeField] private TMP_InputField passwordRegisterVerifyField;
    [SerializeField] private TMP_Text warningRegisterText;


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
                Debug.Log("no se pudo resolver las dependencia del firebase" + dependencyStatus);
            }
        });
    }
    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
   
    private IEnumerator Register(string _email, string _password, string _userName)
    {
      
        if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Not Match";
        }
        else{
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);


            if (RegisterTask.Exception != null)
            {
                Debug.Log(message: $"Failed To register task with{RegisterTask.Exception}");

                FirebaseException firebaseEX = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEX.ErrorCode;

                string message = "Register Failed:" + errorCode;
               
                warningRegisterText.text = message;

            }
            else
            {

                Firebase.Auth.AuthResult result = RegisterTask.Result;

                user = result.User;

                if (user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _userName };

                    var ProfileTask = user.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                    if(ProfileTask.Exception != null)
                    {
                        Debug.Log(message: $"Failed To register task with{RegisterTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text ="Username Set Failed";
                    }

                }
                Debug.LogFormat("User Register in succesfully: {0} ({1}", user.DisplayName, user.Email);
        }
       
        }
    }
}
