using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

using NetflixComplete.Portable;

namespace NetflixComplete.DroidTV
{
    [Activity(Label = "Netflix Complete!", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity, Java.Lang.Thread.IUncaughtExceptionHandler, View.IOnClickListener
    {
        EditText emailEditText, passwordEditText;
        Button signInButton;

        public void UncaughtException(Thread thread, Throwable ex)
        {
            Android.Util.Log.Error("LoginActivity.UncaughtException", ex.Message);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            Java.Lang.Thread.DefaultUncaughtExceptionHandler = this;

            SetContentView(Resource.Layout.Login);

            var preferences = this.GetPreferences(FileCreationMode.Private);

            emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            signInButton = FindViewById<Button>(Resource.Id.signInButton);

            var progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            emailEditText.Text = preferences.GetString("emailAddress", "");
            passwordEditText.Text = preferences.GetString("password", "");

            ValidateLogin();

            signInButton.Click += async (sender, args) =>
            {
                string errorMessage = "Netflix could not authenticate you. Check your credentials.";
                bool isAuthenticated = false;
                progressBar.Visibility = ViewStates.Visible;
                try
                {
                    isAuthenticated = await NetflixSession.Get().AuthenticateAsync(emailEditText.Text, passwordEditText.Text);
                }
                catch(System.Exception ex)
                {
                    errorMessage = "Please check your network connection and try again.";
                    Android.Util.Log.Error("Login", ex.Message);
                }
                finally
                {
                    progressBar.Visibility = ViewStates.Invisible;
                }
                
                if (isAuthenticated)
                {
                    var editor = preferences.Edit();
                    editor.PutString("emailAddress", emailEditText.Text);
                    editor.PutString("password", passwordEditText.Text);
                    editor.Commit();

                    StartActivity(typeof(MainActivity));
                }
                else
                {
                    errorFragment.Title = "Oops!";
                    errorFragment.Message = errorMessage;
                    errorFragment.ButtonText = "Dismiss";
                    errorFragment.ButtonClickListener = this;
                    FragmentManager.BeginTransaction().Add(Resource.Id.error_fragment, errorFragment).Commit();
                }
            };

            emailEditText.TextChanged += (sender, args) =>
            {
                ValidateLogin();
            };
            passwordEditText.TextChanged += (sender, args) =>
            {
                ValidateLogin();
            };
        }

        // ErrorFragment.IOnClickListener
        private Android.Support.V17.Leanback.App.ErrorFragment errorFragment = new Android.Support.V17.Leanback.App.ErrorFragment();
        public void OnClick(View v)
        {
            this.FragmentManager.BeginTransaction().Remove(errorFragment).Commit();
        }

        private void ValidateLogin()
        {
            bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(emailEditText.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            signInButton.Enabled = passwordEditText.Text.Length >= 4 && isEmail;
        }
    }

    public static class NetflixSession
    {
        private static NetflixApi _netflixApi;
        public static NetflixApi Get()
        {
            if (_netflixApi == null)
            {
                _netflixApi = new NetflixApi();
            }
            return _netflixApi;
        }
    }
}