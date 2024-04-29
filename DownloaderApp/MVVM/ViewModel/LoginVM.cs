using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.Model;
using DownloaderApp.UserControls;
using DownloaderApp.Utils;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DownloaderApp.MVVM.ViewModel
{
    class LoginVM : InputOutputVM
    {
        public ICommand LoginButtonCommand { get; }

        private readonly LoginModel _loginModel;

        public string? Username
        {
            get { return _loginModel.Username; }
            set
            {
                if (value != _loginModel.Username)
                {
                    _loginModel.Username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Password
        {
            get { return _loginModel.Password; }
            set
            {
                if (value != _loginModel.Password)
                {
                    _loginModel.Password = value;
                    OnPropertyChanged();
                }
            }
        }

        public override ICommand TextChangedCommand => throw new NotImplementedException();
        public override ICommand ButtonClickCommand => throw new NotImplementedException();

        public LoginVM()
        {
            LoginButtonCommand = new RelayCommand(LoginButtonCommandExecute, LoginButtonCommandCanExecute);

            _loginModel = new LoginModel();
             
            Mediator.IInstaApiChanged += (sender, e) =>
            {
                if (sender != this)
                {
                    _loginModel.Api = e.Api;
                    InfoText = e.Api.IsUserAuthenticated ? "Logged" : "Not logged";
                }
            };
        }

        private async void LoginButtonCommandExecute(object? parameter)
        {
            IInstaApi instaApi = InstaApiBuilder
                .CreateBuilder()
                .SetSessionHandler(new FileSessionHandler { FilePath = InstagramModel.AccountSessionFilePath })
                .SetUser(new UserSessionData { UserName = Username, Password = Password })
                .Build();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Logging", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            var loginResult = await instaApi.LoginAsync();
            cancellationTokenSource.Cancel();

            if (!loginResult.Succeeded)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = loginResult.Info.ToString();
                return;
            }

            InfoSignState = InfoSignState.Success;
            InfoSignToolTip = "The user has successfully logged in";
            InfoText = "Logged";

            instaApi.SessionHandler.Save(false);
            _loginModel.Api = instaApi;

            Mediator.NotifyIInstaApiChanged(this, new InstaApiEventArgs(_loginModel.Api));
        }

        private bool LoginButtonCommandCanExecute(object? parameter)
        {
            if (_loginModel.Api?.IsUserAuthenticated ?? false)
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
