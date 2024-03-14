using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SharpFrame.ViewModels
{
    public class SystemLogInViewModel : BindableBase
    {
        public static event Action<string, string> LogInLoad;

        IEventAggregator eventAggregator;

        public DelegateCommand Login_button { get; set; }

        public DelegateCommand Close { get; set; }

        public SystemLogInViewModel(IEventAggregator aggregators)
        {
            this.eventAggregator = aggregators;
            Login_button = new DelegateCommand(() =>
            {
                LogInLoad?.Invoke(Username, PassWord);
            });
            Close = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<Close_MessageEvent>().Publish();
                System.Environment.Exit(0);
            });
        }

        private string _longin_log;

        public string Lonin_Log
        {
            get { return _longin_log; }
            set { _longin_log = value; RaisePropertyChanged(); }
        }

        private string _title = "瑞弘测控";
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }

        private string _password;

        public string PassWord
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged(); }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; RaisePropertyChanged(); }
        }

        private bool _client_on_line;

        public bool Client_On_Line
        {
            get { return _client_on_line; }
            set { _client_on_line = value; RaisePropertyChanged(); }
        }
    }

    public class Close_MessageEvent : PubSubEvent { }
}
