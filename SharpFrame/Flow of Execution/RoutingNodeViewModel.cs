using Syncfusion.UI.Xaml.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SharpFrame.Flow_of_Execution
{
    public class RoutingNodeViewModel : NodeViewModel
    {
        //To hold the fill color of the nodes.
        private Brush fill = new SolidColorBrush(Colors.Black);

        /// <summary>
        /// Gets or sets the fill color to the nodes.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return fill;
            }

            set
            {
                if (fill != value)
                {
                    fill = value;
                    OnPropertyChanged("Fill");
                    OnFillChanged();
                }
            }
        }

        #region Methods

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);

            switch (name)
            {
                case "Fill":
                    OnFillChanged();
                    break;
            }
        }

        /// <summary>
        /// Updates the fill color, stroke color, stroke thickness, stroke dash values to the nodes.
        /// </summary>
        private void OnFillChanged()
        {
            Style s = new Style(typeof(System.Windows.Shapes.Path));
            if (Fill != null)
            {
                s.Setters.Add(new Setter(System.Windows.Shapes.Path.FillProperty, Fill));
                s.Setters.Add(new Setter(System.Windows.Shapes.Path.StretchProperty, Stretch.Fill));
                s.Setters.Add(new Setter(System.Windows.Shapes.Path.StrokeProperty, Fill));
            }
            ShapeStyle = s;
        }

        #endregion Methods
    }
}
