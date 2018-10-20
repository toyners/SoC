using System;
using System.Collections.Generic;
using System.ComponentModel;
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
  public partial class ResourceButtonControl : UserControl, INotifyPropertyChanged
  {
    private string imagePath;
    private bool isSelected;
    private Action<ResourceButtonControl> clickEventHandler;
    private PropertyChangedEventArgs imagePathChanged = new PropertyChangedEventArgs("ImagePath");

    public ResourceButtonControl(Action<ResourceButtonControl> clickEventHandler)
    {
      this.DataContext = this;
      this.InitializeComponent();
      this.clickEventHandler = clickEventHandler;
    }

    public string ImagePath {
      get { return this.imagePath; }
      set {
        this.imagePath = value;
        this.PropertyChanged?.Invoke(this, this.imagePathChanged);
      }
    }
    public string OriginalImagePath { get; set; }
    public string SelectedImagePath { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (this.isSelected)
      {
        this.isSelected = false;
        this.ImagePath = this.OriginalImagePath;
      }
      else
      {
        this.isSelected = true;
        this.ImagePath = this.SelectedImagePath;
      }

      this.clickEventHandler?.Invoke(this);
    }
  }
}
