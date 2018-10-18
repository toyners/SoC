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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoC.Harness.Views
{
  /// <summary>
  /// Interaction logic for ResourceControl.xaml
  /// </summary>
  public partial class ResourceButtonControl : UserControl
  {
    private Action<ResourceButtonControl> clickEventHandler;

    public ResourceButtonControl(string imagePath, Action<ResourceButtonControl> clickEventHandler)
    {
      this.DataContext = this;
      this.InitializeComponent();
      this.clickEventHandler = clickEventHandler;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.clickEventHandler?.Invoke(this);
    }
  }
}
