﻿using Prism.Events;
using SharpFrame.ViewModels.ToolViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SharpFrame.Views.ToolViews
{
    /// <summary>
    /// NewParameterModelView.xaml 的交互逻辑
    /// </summary>
    public partial class NewParameterModelView : Window
    {
        public NewParameterModelView(IEventAggregator aggregator, List<string> parameterslist)
        {
            InitializeComponent();
            this.DataContext = new NewParameterModelViewModel(aggregator, parameterslist);

        }
    }
}