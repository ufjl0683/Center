using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SLProcessController.ProcessService;

namespace SLProcessController
{

     public delegate void  OnHostChangeAllStateHandler (HostInfo hosytinfo,bool isAllPlay);

    public partial class AccordionHeader : UserControl
    {
        public event OnHostChangeAllStateHandler OnChangeHostAllState;
        public AccordionHeader()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnChangeHostAllState != null)
                this.OnChangeHostAllState((DataContext as ProcessService.HostInfo), true);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.OnChangeHostAllState != null)
                this.OnChangeHostAllState((DataContext as ProcessService.HostInfo), false);
        }
    }
}
