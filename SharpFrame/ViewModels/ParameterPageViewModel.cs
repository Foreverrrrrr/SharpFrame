using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.ViewModels
{
    public class ParameterPageViewModel : BindableBase
    {
        IEventAggregator aggregator;

        public ParameterPageViewModel(IEventAggregator eventaggregator)
        {
            aggregator = eventaggregator;
            aggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {

            });
        }
    }
}
