﻿using System;
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

namespace SoC.Harness
{
  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class SettlementButtonControl : UserControl
  {
    private Action<int> clickEventHandler;
    private int id;

    public SettlementButtonControl(int id, Action<int> clickEventHandler)
    {
      this.InitializeComponent();

      this.id = id;
      this.clickEventHandler = clickEventHandler;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.clickEventHandler?.Invoke(this.id);
    }
  }
}
