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
using System.Windows.Data;

namespace SLProcessController
{

    public delegate void OnChangeStateHandler(ProcessInfoCtl ctl,ProcessService.ProcessInfo info, bool bPlay);
   
    public partial class ProcessInfoCtl : UserControl
    {
        public event OnChangeStateHandler OnChangeState;
        public ProcessInfoCtl()
        {
            InitializeComponent();
          //  this.LayoutRoot.DataContext = info;
            this.txtMax.Text = this.ProgressBar.Maximum.ToString();
            
        }

        public void SetProgressMaxValue(int maxValue)
        {
            this.ProgressBar.Maximum = maxValue;
            this.txtMax.Text = maxValue.ToString();
        }
      public void setDataContext(ProcessService.ProcessInfo info)
      {
          this.DataContext=info;
          if (info.IsAlive)
          {
              this.run.Stop();
              this.run.Begin();
          }
          else
          {
              this.run.Stop();
              this.stop.Stop();
              this.stop.Begin();
          }
      }

      public void RefreshState()
      {
          if (DataContext == null)
              return;
          ProcessService.ProcessInfo info = this.DataContext as ProcessService.ProcessInfo;
          
          if (info.IsAlive)
          {
              this.run.Stop();
              this.run.Begin();
          }
          else
          {
              this.run.Stop();
              this.stop.Stop();
              this.stop.Begin();
          }
      }

      private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
          if(this.OnChangeState!=null)
          this.OnChangeState(this,this.DataContext  as ProcessService.ProcessInfo, false);
      }

      private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
          if (this.OnChangeState != null)
              this.OnChangeState(this,this.DataContext as ProcessService.ProcessInfo, true);
      }
    }

  
    public   class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter 成員

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
             if(value==null)
                return    System.Windows.Visibility.Collapsed;
            if (System.Convert.ToBoolean(value))
            {
                
                return System.Windows.Visibility.Visible;
            }

            else
            {
                return System.Windows.Visibility.Collapsed;
            }

            
          //  throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class HostQueueCntProgressConverter : IValueConverter
    {
      

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //  throw new NotImplementedException();
            int cnt;
            if (int.TryParse(value.ToString(), out cnt))
            {
                if (cnt > 10000)
                    return 10000;
                else

                    return cnt;


            }
            else
                return 0;


        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotImplementedException();
      
        }
    }

        public class MFCCQueueCntProcessConverter : IValueConverter
        {
            #region IValueConverter 成員

            object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                int cnt;
                if (int.TryParse(value.ToString(), out cnt))
                {
                    if (cnt > 1000)
                        return 1000;
                    else

                        return cnt;


                }
                else
                    return 0;
            }


            object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
        public class InvertBooleanToVisibilityConverter : IValueConverter
        {
            #region IValueConverter 成員

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value == null)
                    return System.Windows.Visibility.Collapsed;

                if (System.Convert.ToBoolean(value))

                    return System.Windows.Visibility.Collapsed;

                else
                    return System.Windows.Visibility.Visible;


                //  throw new NotImplementedException();
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }

