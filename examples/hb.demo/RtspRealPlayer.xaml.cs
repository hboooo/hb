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

namespace hb.demo
{
    /// <summary>
    /// RtspRealPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class RtspRealPlayer : Window
    {
        private RtspRealPlayManager f_RtspRealPlayManager;

        public RtspRealPlayer()
        {
            InitializeComponent();
            this.Loaded += RtspRealPlayer_Loaded;
            this.Closed += RtspRealPlayer_Closed;
        }

        private void RtspRealPlayer_Closed(object sender, EventArgs e)
        {
            f_RtspRealPlayManager?.Dispose();
        }

        private void RtspRealPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            f_RtspRealPlayManager = new RtspRealPlayManager();
        }

        public void Start()
        {
            f_RtspRealPlayManager?.Start(img, "rtsp://192.168.1.10");
        }

        public void Stop()
        {
            f_RtspRealPlayManager?.Stop();
        }
    }
}
