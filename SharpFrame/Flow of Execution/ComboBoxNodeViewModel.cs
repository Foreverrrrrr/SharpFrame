using SharpFrame.Flow_of_Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Flow_of_Execution
{
    public class ComboBoxNodeViewModel : RoutingNodeViewModel
    {
        private List<string> _items;
        private string _selectedItem;

        public new List<string> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public new string SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
    }
}
