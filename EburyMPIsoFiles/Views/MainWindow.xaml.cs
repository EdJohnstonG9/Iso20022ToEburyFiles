using EburyMPIsoFiles.Constants;
using MahApps.Metro.Controls;
using Prism.Regions;
using System.Windows;

namespace EburyMPIsoFiles.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            RegionManager.SetRegionName(hamburgerMenuContentControl, Regions.Main);
            RegionManager.SetRegionManager(hamburgerMenuContentControl, regionManager);
        }
    }
}
