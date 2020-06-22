using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Roomsizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public StorageFile RoomFile { get; set; }

        public JObject WorkingRoomJson { get; set; }

        public enum AnchorDirection {
            TL,
            T,
            TR,
            L,
            C,
            R,
            BL,
            B,
            BR
        }

        public AnchorDirection Anchor = AnchorDirection.C;

        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(400, 350);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(400, 350));
        }

        private void Resize_Click(object sender, RoutedEventArgs e) {

        }

        private async void Browser_Click(object sender, RoutedEventArgs e) {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            filePicker.FileTypeFilter.Add(".yy");

            Windows.Storage.StorageFile file = await filePicker.PickSingleFileAsync();
            if (file != null) {
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("MostRecentRoom", file);
                PathTextBox.Text = file.DisplayName;
                RoomFile = file;

                var text = await FileIO.ReadTextAsync(file);
                WorkingRoomJson = JObject.Parse(text);
                WidthBox.Text = (string)WorkingRoomJson["roomSettings"]["Width"];
                HeightBox.Text = (string)WorkingRoomJson["roomSettings"]["Height"];
            }
        }

        private void AnchorTL_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.TL;
            AnchorTL.IsChecked = true;
        }
        private void AnchorT_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.T;
            AnchorT.IsChecked = true;
        }
        private void AnchorTR_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.TR;
            AnchorTR.IsChecked = true;
        }
        private void AnchorL_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.L;
            AnchorL.IsChecked = true;
        }
        private void AnchorC_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.C;
            AnchorC.IsChecked = true;
        }
        private void AnchorR_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.R;
            AnchorR.IsChecked = true;
        }
        private void AnchorBL_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.BL;
            AnchorBL.IsChecked = true;
        }
        private void AnchorB_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.B;
            AnchorB.IsChecked = true;
        }
        private void AnchorBR_Checked(object sender, RoutedEventArgs e) {
            UncheckAllBoxes();
            Anchor = AnchorDirection.BR;
            AnchorBR.IsChecked = true;
        }

        private void UncheckAllBoxes() {
            AnchorTL.IsChecked = false;
            AnchorT.IsChecked = false;
            AnchorTR.IsChecked = false;
            AnchorL.IsChecked = false;
            AnchorC.IsChecked = false;
            AnchorR.IsChecked = false;
            AnchorBL.IsChecked = false;
            AnchorB.IsChecked = false;
            AnchorBR.IsChecked = false;
        }
    }
}
