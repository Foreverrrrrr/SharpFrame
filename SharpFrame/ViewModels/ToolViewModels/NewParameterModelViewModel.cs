using ImTools;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;

namespace SharpFrame.ViewModels.ToolViewModels
{
    public class NewParameterModelViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        public NewParameterModelViewModel(IEventAggregator aggregator, List<string> parameterslist)
        {
            this.eventAggregator = aggregator;
            for (int i = 0; i < parameterslist.Count; i++)
            {
                OptionalParametersList.Add(new ComboxList() { ID = i, Name = parameterslist[i].ToString() });
            }
            OptionalParametersList.Add(new ComboxList() { ID = OptionalParametersList.Count - 1, Name = "Null" });

            Close = new DelegateCommand(() =>
            {
                NewParameterModelView currentWindow = System.Windows.Application.Current.Windows.OfType<NewParameterModelView>().SingleOrDefault(w => w.IsActive);
                currentWindow.Close();
            });
            OkButton = new DelegateCommand(() =>
            {
                Add_Model modelEvent = new Add_Model();
                modelEvent.NewName = NewName;
                modelEvent.InitialParameter = OptionalParametersList[OptionalParametersIndexes].Name;
                eventAggregator.GetEvent<NewModelEvent>().Publish(modelEvent);
                NewParameterModelView currentWindow = System.Windows.Application.Current.Windows.OfType<NewParameterModelView>().SingleOrDefault(w => w.IsActive);
                currentWindow.Close();
            });
        }

        public DelegateCommand Close { get; set; }

        public DelegateCommand OkButton { get; set; }

        private string _newname;

        public string NewName
        {
            get { return _newname; }
            set { _newname = value; RaisePropertyChanged(); }
        }

        private int _optionalparametersindexes = 0;
        /// <summary>
        /// 可选初始值索引
        /// </summary>
        public int OptionalParametersIndexes
        {
            get { return _optionalparametersindexes; }
            set { _optionalparametersindexes = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ComboxList> _optionalparameterslist = new ObservableCollection<ComboxList>();
        /// <summary>
        /// 可选初始值集合
        /// </summary>
        public ObservableCollection<ComboxList> OptionalParametersList
        {
            get { return _optionalparameterslist; }
            set { _optionalparameterslist = value; RaisePropertyChanged(); }
        }
    }
}
