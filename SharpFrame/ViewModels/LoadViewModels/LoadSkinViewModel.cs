using System.Windows;
using Prism.Events;
using Prism.Mvvm;

namespace SharpFrame.ViewModels.LoadViewModels
{
    public class LoadSkinViewModel : BindableBase
    {
        private IEventAggregator eventAggregator;
        public LoadSkinViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            Height = SystemParameters.PrimaryScreenHeight - 45;
            Width = SystemParameters.PrimaryScreenWidth + 10;
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private double _width;

        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

    }
}