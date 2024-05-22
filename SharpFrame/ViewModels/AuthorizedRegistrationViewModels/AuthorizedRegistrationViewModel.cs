using System.Linq;
using System.Windows;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.AuthorizedRegistrationViews;

namespace SharpFrame.ViewModels.AuthorizedRegistrationViewModels
{
    public class AuthorizedRegistrationViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        public static ClientKeyMaker.ClientToken token;
        public AuthorizedRegistrationViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            GenerateRegistrationCode = new DelegateCommand(() =>
            {
                if (token != null)
                {
                    RegistrationCode = token.Client_Token_Create(256);
                }
            });

            Accredit = new DelegateCommand(() =>
            {
                if (token != null)
                {
                    if (Password != null)
                    {
                        try
                        {
                            bool t = token.PassWordCheck(Password);
                            if (!t)
                            {
                                MessageBox.Show("授权码校验错误！");
                            }
                            Password = null;
                            RegistrationCode = null;
                            AuthorizedRegistrationView currentWindow = System.Windows.Application.Current.Windows.OfType<AuthorizedRegistrationView>().SingleOrDefault(w => w.IsActive);
                            currentWindow.Close();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("未输入授权密钥！");
                    }
                }
            });
        }

        /// <summary>
        /// 注册机生成
        /// </summary>
        public DelegateCommand GenerateRegistrationCode { get; set; }

        /// <summary>
        /// 授权码认证
        /// </summary>
        public DelegateCommand Accredit { get; set; }

        private string _registrationcode;

        public string RegistrationCode
        {
            get { return _registrationcode; }
            set { _registrationcode = value; RaisePropertyChanged(); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged(); }
        }
    }
}
