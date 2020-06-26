using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
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
            SetButtonsEnabled(false);
            ValidateResizeAllowed();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(400, 400));
            ApplicationView.PreferredLaunchViewSize = new Size(400, 400);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private async void Resize_Click(object sender, RoutedEventArgs e) {
            var tileSize = TileSizeCheckbox.IsChecked ?? true ? int.Parse(TileSizeBox.Text) : 1;
            try {
                WorkingRoomJson = RoomResizer.ResizeRoom(WorkingRoomJson, Int32.Parse(WidthBox.Text), Int32.Parse(HeightBox.Text), tileSize, Anchor);
                await FileIO.WriteTextAsync(RoomFile, WorkingRoomJson.ToString());
                WorkingRoomJson = null;
                SetButtonsEnabled(false);
            } catch (ArgumentException) {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)ResizeButton);
            }
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
                ReloadButton.IsEnabled = true;

                LoadFileData();
            }
        }
        private void ReloadButton_Click(object sender, RoutedEventArgs e) {
            LoadFileData();
        }

        private void ValidateResizeAllowed() {
            int errorCount = 0;
            if (!int.TryParse(WidthBox.Text, out int temp)) {
                errorCount += 1;
            } else {
                if (temp < 0) {
                    errorCount += 1;
                }
            }
            if (!int.TryParse(HeightBox.Text, out temp)) {
                errorCount += 1;
            } else {
                if (temp < 0) {
                    errorCount += 1;
                }
            }
            if (TileSizeCheckbox.IsChecked ?? true) {
                if (!int.TryParse(TileSizeBox.Text, out temp)) {
                    errorCount += 1;
                } else {
                    if (temp <= 0) {
                        errorCount += 1;
                    }
                }
            }
            ResizeButton.IsEnabled = (errorCount == 0);
        }

        private async void LoadFileData() {
            var text = await FileIO.ReadTextAsync(RoomFile);
            WorkingRoomJson = JObject.Parse(text);
            WidthBox.Text = (string)WorkingRoomJson["roomSettings"]["Width"];
            HeightBox.Text = (string)WorkingRoomJson["roomSettings"]["Height"];
            SetButtonsEnabled(true);
            ValidateResizeAllowed();
        }

        private void SetButtonsEnabled(bool enabled) {
            AnchorTL.IsEnabled = enabled;
            AnchorT.IsEnabled = enabled;
            AnchorTR.IsEnabled = enabled;
            AnchorL.IsEnabled = enabled;
            AnchorC.IsEnabled = enabled;
            AnchorR.IsEnabled = enabled;
            AnchorBL.IsEnabled = enabled;
            AnchorB.IsEnabled = enabled;
            AnchorBR.IsEnabled = enabled;

            HeightBox.IsEnabled = enabled;
            WidthBox.IsEnabled = enabled;
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

        private void TileSizeCheckbox_Click(object sender, RoutedEventArgs e) {
            ValidateResizeAllowed();
        }

        private void TextChanged(object sender, TextChangedEventArgs e) {
            ValidateResizeAllowed();
        }
    }
}
