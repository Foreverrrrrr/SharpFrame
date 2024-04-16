using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpFrame.ViewModels.ToolViewModels
{
    public class NewParameterModelViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        public NewParameterModelViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            Close = new DelegateCommand(() =>
            {
                System_AddView currentWindow = System.Windows.Application.Current.Windows.OfType<System_AddView>().SingleOrDefault(w => w.IsActive);
                currentWindow.Close();
            });
            OkButton = new DelegateCommand(() =>
            {

            });
        }

        public DelegateCommand Close { get; set; }

        public DelegateCommand OkButton { get; set; }
    }
}
