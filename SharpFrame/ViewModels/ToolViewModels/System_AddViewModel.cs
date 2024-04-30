using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SharpFrame.ViewModels.ToolViewModels
{
    public class System_AddViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private SystemParameter this_checkdata;

        public System_AddViewModel(IEventAggregator aggregator, SystemParameter checkdata)
        {
            this.eventAggregator = aggregator;
            this.this_checkdata = checkdata;
            Close = new DelegateCommand(() =>
            {
                System_AddView currentWindow = System.Windows.Application.Current.Windows.OfType<System_AddView>().SingleOrDefault(w => w.IsActive);
                currentWindow.Close();
            });
            OkButton = new DelegateCommand(() =>
            {
                SystemParameter structure = new SystemParameter();
                structure.Name = Name;
                string input_value = Input_Value;
                switch (TypeModel)
                {
                    case 0:
                        if (input_value != "Null" || input_value != "" || input_value != null)
                        {
                            structure.Value = input_value;
                            aggregator.GetEvent<SystemParameterAddEvent>().Publish(new Add_SystemIns() { NewParameter = structure, InsertionParameter = this_checkdata });
                        }
                        else
                        {
                            MessageBox.Show("输入参数值与类型不符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;
                    case 1:
                        float float_value = 0f;
                        if (float.TryParse(input_value, out float_value))
                        {
                            structure.Value = float_value;
                            aggregator.GetEvent<SystemParameterAddEvent>().Publish(new Add_SystemIns() { NewParameter = structure, InsertionParameter = this_checkdata });
                        }
                        else
                        {
                            MessageBox.Show("输入参数值与类型不符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;
                    case 2:
                        double double_value = 0d;
                        if (double.TryParse(input_value, out double_value))
                        {
                            structure.Value = double_value;
                            aggregator.GetEvent<SystemParameterAddEvent>().Publish(new Add_SystemIns() { NewParameter = structure, InsertionParameter = this_checkdata });
                        }
                        else
                        {
                            MessageBox.Show("输入参数值与类型不符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;
                    case 3:
                        bool bool_value = false;
                        if (bool.TryParse(input_value, out bool_value))
                        {
                            structure.Value = bool_value;
                            aggregator.GetEvent<SystemParameterAddEvent>().Publish(new Add_SystemIns() { NewParameter = structure, InsertionParameter = this_checkdata });
                        }
                        else
                        {
                            MessageBox.Show("输入参数值与类型不符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;
                }
                System_AddView currentWindow = System.Windows.Application.Current.Windows.OfType<System_AddView>().SingleOrDefault(w => w.IsActive);
                currentWindow.Close();
            });
        }

        public DelegateCommand Close { get; set; }

        public DelegateCommand OkButton { get; set; }

        private string _name = "Null";

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        private string _inputvalue = "";
        public string Input_Value
        {
            get { return _inputvalue; }
            set { _inputvalue = value; RaisePropertyChanged(); }
        }

        private int _typemodel = 0;
        public int TypeModel
        {
            get { return _typemodel; }
            set { _typemodel = value; RaisePropertyChanged(); }
        }

        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }
    }
}
