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

namespace Client.TestHarness
{
  /// <summary>
  /// Interaction logic for CircleButton.xaml
  /// </summary>
  public partial class CircleButton : UserControl
  {
    private Action<Int32> click;

    private Int32 id;

    public CircleButton(Action<Int32> click, Int32 id)
    {
      InitializeComponent();

      this.click = click;
      this.id = id;
    }

    private void Button_Click(Object sender, RoutedEventArgs e)
    {
      this.click(this.id);
      e.Handled = true;
    }
  }
}
