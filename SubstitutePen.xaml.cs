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

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for SubstitutePen.xaml
    /// </summary>
    public partial class SubstitutePen : Window
    {
        public SubstitutePen(object dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }
    }
}
