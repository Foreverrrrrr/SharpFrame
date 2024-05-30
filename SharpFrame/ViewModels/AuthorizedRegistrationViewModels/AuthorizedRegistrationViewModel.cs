using System.Linq;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.AuthorizedRegistrationViews;

namespace SharpFrame.ViewModels.AuthorizedRegistrationViewModels
{
    public class AuthorizedRegistrationViewModel : BindableBase
    {
        private int continuous = 0;
        private readonly IEventAggregator eventAggregator;

        public static ClientKeyMaker.ClientToken token;
        public AuthorizedRegistrationViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            GenerateRegistrationCode = new DelegateCommand(() =>
            {
                if (token != null)
                {
                    if (RegistrationCode == null)
                    {
                        RegistrationCode = token.Client_Token_Create(256);
                        Clipboard.SetText(RegistrationCode);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("已存在注册码，请勿重复生成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            });

            Accredit = new DelegateCommand(() =>
            {
                if (token != null)
                {
                    if (continuous < 3)
                    {
                        if (Password != null)
                        {
                            try
                            {
                                bool t = token.PassWordCheck(Password);
                                if (!t)
                                {
                                    System.Windows.Forms.MessageBox.Show("授权码校验错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                Password = null;
                                RegistrationCode = null;
                                AuthorizedRegistrationView currentWindow = System.Windows.Application.Current.Windows.OfType<AuthorizedRegistrationView>().SingleOrDefault(w => w.IsActive);
                                currentWindow.Close();
                            }
                            catch (System.Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("未输入授权密钥", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("授权验证超过三次,授权自动退出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Password = null;
                        RegistrationCode = null;
                        AuthorizedRegistrationView currentWindow = System.Windows.Application.Current.Windows.OfType<AuthorizedRegistrationView>().SingleOrDefault(w => w.IsActive);
                        currentWindow.Close();
                    }
                }
                continuous++;
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
