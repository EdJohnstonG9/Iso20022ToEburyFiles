using System;

using ConvertToMassPayments.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ConvertToMassPayments.Views
{
    public sealed partial class ConvertPage : Page
    {
        private ConvertViewModel ViewModel => DataContext as ConvertViewModel;

        public ConvertPage()
        {
            InitializeComponent();
        }
    }
}
