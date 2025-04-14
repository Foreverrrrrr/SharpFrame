using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.AuthorizedRegistrationViews;
using System;
using System.Linq;

namespace SharpFrame.ViewModels
{
    public class SystemLogInViewModel : BindableBase
    {
        public static event Action<string, string> LogInLoad;

        public DelegateCommand<string> Login_button { get; set; }

        public DelegateCommand Close { get; set; }

        public SystemLogInViewModel(IEventAggregator aggregators)
        {
            Login_button = new DelegateCommand<string>((x) =>
            {
                LogInLoad?.Invoke(Username, PassWord);
            });
            Close = new DelegateCommand(() =>
            {
                System.Environment.Exit(0);
            });
            AuthorizationShow = new DelegateCommand(() =>
            {
                AuthorizedRegistrationView registrationView = new AuthorizedRegistrationView(aggregators);
                registrationView.Show();
            });
        }

        public DelegateCommand AuthorizationShow { get; set; }

        private string _longin_log;

        public string Lonin_Log
        {
            get { return _longin_log; }
            set { SetProperty(ref _longin_log, value); }
        }

        private string _title = "Sharp Frame";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _password = "666888";

        public string PassWord
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private bool _client_on_line;

        public bool Client_On_Line
        {
            get { return _client_on_line; }
            set { SetProperty(ref _client_on_line, value); }
        }

        private object _warranty = null;

        public object Warranty
        {
            get { return _warranty; }
            set { SetProperty(ref _warranty, value); }
        }
    }
}
