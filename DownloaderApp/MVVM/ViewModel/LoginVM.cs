using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.Model;
using DownloaderApp.UserControls;
using DownloaderApp.Utils;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using System.Windows.Input;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class LoginVM : InputOutputVM
    {
        public ICommand LoginButtonCommand { get; }

        private IInstaApi? _api;

        private string? _username;
        public string? Username
        {
            get { return _username; }
            set { RaiseAndSetIfChanged(ref _username, value); }
        }

        private string? _password;
        public string? Password
        {
            get { return _password; }
            set { RaiseAndSetIfChanged(ref _password, value); }
        }

        public override ICommand TextChangedCommand => throw new NotImplementedException();
        public override ICommand ButtonClickCommand => throw new NotImplementedException();

        public LoginVM()
        {
            LoginButtonCommand = new RelayCommand(LoginButtonCommandExecute, LoginButtonCommandCanExecute);

            Mediator.InstaApiChanged += (sender, e) =>
            {
                if (sender != this)
                {
                    _api = e;
                    InfoText = e.IsUserAuthenticated ? "Logged" : "Not logged";
                }
            };
        }

        private async void LoginButtonCommandExecute(object? parameter)
        {
            if (_api is not null && _api.IsUserAuthenticated)
            {
                var mBoxResult = CustomMbox.ShowDialog("You already logged. Wanna proceed?",
                    "Login", CustomMbox.CustomMboxButtons.YesNo);

                if (mBoxResult != true)
                    return;
            }

            IInstaApi instaApi = InstaApiBuilder
                .CreateBuilder()
                .SetSessionHandler(new FileSessionHandler { FilePath = InstagramModel.AccountSessionFilePath })
                .SetUser(new UserSessionData { UserName = Username, Password = Password })
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Logging", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            var loginResult = await instaApi.LoginAsync();
            cancellationTokenSource.Cancel();

            if (!loginResult.Succeeded)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = loginResult.Info.Message;
                CustomMbox.ShowDialog(loginResult.Info.Message, "Login error");
                return;
            }

            InfoSignState = InfoSignState.Success;
            InfoSignToolTip = "User has successfully logged in";
            InfoText = "Logged";

            instaApi.SessionHandler.Save(false);
            _api = instaApi;

            Mediator.NotifyInstaApiChanged(this, _api);
        }

        private bool LoginButtonCommandCanExecute(object? parameter)
        {
            if (_api?.IsUserAuthenticated ?? false)
            {
                InfoSignState = InfoSignState.Success;
                InfoSignToolTip = "User logged";
            }
            else
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = "Username must be non empty field and password must contain at least 6 characters";
            }

            if (Password is null || Username is null || Password.Length < 6 || Username.Length < 1)
                return false;

            return true;
        }

        protected override void TextChangedCommandExecute(object? parameter)
        {
            throw new NotImplementedException();
        }
        protected override bool TextChangedCommandCanExecute(object? parameter)
        {
            throw new NotImplementedException();
        }
        protected override void ClickCommandExecute(object? parameter)
        {
            throw new NotImplementedException();
        }
        protected override bool ClickCommandCanExecute(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
