using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpFrame.ViewModels
{
    public class DebuggingViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        public DebuggingViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            aggregator.GetEvent<PageLoadEvent>().Subscribe(() =>
            {

            });
            aggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {

            });
        }
    }
}
