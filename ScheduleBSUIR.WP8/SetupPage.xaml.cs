using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using ScheduleBSUIR.Models;
using ScheduleBSUIR.Resources;
using ScheduleParser;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace ScheduleBSUIR
{
    public partial class OptionsPage : PhoneApplicationPage
    {
        private const string ScheduleUri = @"http://www.bsuir.by/schedule/rest/schedule/";
        private const string Extension = ".bsuir";

        public OptionsPage()
        {
            InitializeComponent();
        }

        private void ScheduleList_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshScheduleList();
        }

        private void RefreshScheduleList()
        {
            ScheduleList.IsEnabled = false;
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            List<GroupScheduleLink> fileslist = storage
                .GetFileNames("*" + Extension)
                .Select(item => new GroupScheduleLink(item))
                .ToList();

            ScheduleList.ItemsSource = fileslist;
            ScheduleList.IsEnabled = true;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            GroupBox.IsEnabled = false;
            DownloadButton.IsEnabled = false;
            ScheduleList.IsEnabled = false;

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;

            IsolatedStorageSettings.ApplicationSettings.Remove("DownloadGroupName");
            IsolatedStorageSettings.ApplicationSettings.Add("DownloadGroupName", GroupBox.Text);

            wc.DownloadStringAsync(new Uri(ScheduleUri + GroupBox.Text));
        }

        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            GroupBox.IsEnabled = true;
            DownloadButton.IsEnabled = true;
            if (e.Error != null)
            {
#if DEBUG 
                DemoSchedule demo = new DemoSchedule();
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

                using (StreamWriter sw = new StreamWriter(
                    storage.CreateFile(IsolatedStorageSettings.ApplicationSettings["DownloadGroupName"] + Extension)
                    ))
                {
                    sw.Write(demo.GetScheduleString());
                }                
#else
                MessageBox.Show(AppResources.error_DownloadFile
                                + Environment.NewLine + e.Error.Message);
#endif
            }
            else
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

                using (StreamWriter sw = new StreamWriter(
                    storage.CreateFile(IsolatedStorageSettings.ApplicationSettings["DownloadGroupName"] + Extension)
                    ))
                {
                    sw.Write(e.Result);
                }

            }

            IsolatedStorageSettings.ApplicationSettings.Remove("DownloadGroupName");
            RefreshScheduleList();
        }

        private void GroupBox_GotFocus(object sender, RoutedEventArgs e)
        {
            int tmp;
            if (int.TryParse(GroupBox.Text, out tmp) == false) GroupBox.Text = "";
            DownloadButton.IsEnabled = true;
        }

        private void DeleteGroupScheduleLinkButton_Tap(object sender, GestureEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            GroupScheduleLink linkItem = ((List<GroupScheduleLink>)ScheduleList.ItemsSource)[ScheduleList.SelectedIndex];
            try
            {
                if (((string) settings["filename"]) == linkItem.Filename)
                {
                    settings.Remove("filename");
                    settings.Save();
                }
            }
            catch (Exception)
            {
            }

            storage.DeleteFile(linkItem.Filename);
            RefreshScheduleList();
        }

        private void GroupName_Tap(object sender, GestureEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Remove("groupname");
            settings.Remove("filename");
            settings.Remove("subgroup");
            GroupScheduleLink linkItem = ((List<GroupScheduleLink>) ScheduleList.ItemsSource)[ScheduleList.SelectedIndex];
            settings.Add("groupname", linkItem.Name);
            settings.Add("filename", linkItem.Filename);
            settings.Add("subgroup", SubgroupScope.All);
            settings.Save();
            
            NavigationService.GoBack();
        }

        private void RefteshGroupScheduleLinkButton_Tap(object sender, GestureEventArgs e)
        {
            GroupBox.IsEnabled = false;
            DownloadButton.IsEnabled = false;
            ScheduleList.IsEnabled = false;
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            IsolatedStorageSettings.ApplicationSettings.Remove("DownloadGroupName");

            GroupScheduleLink linkItem = ((List<GroupScheduleLink>) ScheduleList.ItemsSource)[ScheduleList.SelectedIndex];
            IsolatedStorageSettings.ApplicationSettings.Add("DownloadGroupName", linkItem.Name);
            wc.DownloadStringAsync(new Uri(ScheduleUri + linkItem.Name));
        }

        private void RefteshGroupScheduleLinkButton_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateIcon(sender, "refresh");
        }

        private void DeleteGroupScheduleLinkButton_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateIcon(sender, "delete");
        }

        private void UpdateIcon(object sender, string name)
        {
            BitmapImage bi = new BitmapImage();
            Visibility darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            bi.UriSource = darkVisibility == Visibility.Visible
                ? new Uri(string.Format("Icons/{0}.png", name), UriKind.Relative)
                : new Uri(string.Format("Icons/{0}_dark.png", name), UriKind.Relative);
            ((Image)sender).Source = bi;
        }

        private void SubgroupSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                var subgroup = (string)settings["subgroup"];
                BothGroupRadio.IsChecked = subgroup.Equals(SubgroupScope.All);
                FirstGroupRadio.IsChecked = subgroup.Equals(SubgroupScope.First);
                SecondGroupRadio.IsChecked = subgroup.Equals(SubgroupScope.Second);                
                SubgroupSwitch.Visibility = Visibility.Visible;
            }
            catch
            {
                SubgroupSwitch.Visibility = Visibility.Collapsed;
            }
        }

        private void BothGroupRadio_Checked(object sender, RoutedEventArgs e)
        {
            SetSubgroup(SubgroupScope.All);
        }

        private void FirstGroupRadio_Checked(object sender, RoutedEventArgs e)
        {
            SetSubgroup(SubgroupScope.First);
        }

        private void SecondGroupRadio_Checked(object sender, RoutedEventArgs e)
        {
            SetSubgroup(SubgroupScope.Second);
        }

        private void SetSubgroup(string value)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("subgroup");
            IsolatedStorageSettings.ApplicationSettings.Add("subgroup", value);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void GitHubLink_Tap(object sender, GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("https://github.com/Humple/ScheduleBSUIR");
            wbt.Show();
        }
    }
}