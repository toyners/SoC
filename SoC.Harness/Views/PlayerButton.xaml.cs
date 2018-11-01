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
  /// Interaction logic for PlayerButtonControl.xaml
  /// </summary>
  public partial class PlayerButton : UserControl, INotifyPropertyChanged
  {
    #region Fields
    private string imagePath;
    private PropertyChangedEventArgs imagePathChanged = new PropertyChangedEventArgs("ImagePath");
    private bool isSelected;
    private string playerName;
    private PropertyChangedEventArgs playerNameChanged = new PropertyChangedEventArgs("PlayerName");
    private string resourceCountMessage;
    private PropertyChangedEventArgs resourceCountMessageChanged = new PropertyChangedEventArgs("ResourceCountMessage");
    public Action<PlayerButton> ButtonClickEventHandler;
    #endregion

    #region Construction
    public PlayerButton()
    {
      this.DataContext = this;
      this.InitializeComponent();
    }
    #endregion

    #region Properties
    public string ImagePath
    {
      get { return this.imagePath; }
      set
      {
        this.imagePath = value;
        this.PropertyChanged?.Invoke(this, this.imagePathChanged);
      }
    }
    public bool IsSelected
    {
      get { return this.isSelected; }
      set
      {
        this.isSelected = value;
        if (this.isSelected)
        {
          this.ImagePath = this.SelectedImagePath;
        }
        else
        { 
          this.ImagePath = this.OriginalImagePath;
        }
      }
    }
    public string PlayerName
    {
      get { return this.playerName; }
      set
      {
        this.playerName = value;
        this.PropertyChanged?.Invoke(this, this.playerNameChanged);
      }
    }
    public string ResourceCountMessage
    {
      get { return this.resourceCountMessage; }
      set
      {
        this.resourceCountMessage = value;
        this.PropertyChanged?.Invoke(this, this.resourceCountMessageChanged);
      }
    }
    public string OriginalImagePath { get; set; }
    public string SelectedImagePath { get; set; }
    #endregion

    #region Events
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion

    #region Methods
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.IsSelected = !this.IsSelected;
      this.ButtonClickEventHandler?.Invoke(this);
    }
    #endregion
  }
}
