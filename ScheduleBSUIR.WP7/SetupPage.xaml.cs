using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;
using ScheduleBSUIR.Resources;
using Microsoft.Phone.Tasks;

namespace ScheduleBSUIR
{
    public partial class OptionsPage : PhoneApplicationPage
    {
        private readonly string ScheduleUri = "http://www.bsuir.by/psched/rest/";
        private readonly string Extension = ".bsuir";
        public OptionsPage()
        {
            InitializeComponent();
            
        }

        private void ScheduleList_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshScheduleList();
        }
        void RefreshScheduleList()
        {
            ScheduleList.IsEnabled = false;
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            List<GroupScheduleLink> fileslist = new List<GroupScheduleLink>();
            foreach (string item in storage.GetFileNames("*" + Extension))
            {
                fileslist.Add(new GroupScheduleLink(item));
            }
            ScheduleList.ItemsSource = fileslist;
            ScheduleList.IsEnabled = true;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            GroupBox.IsEnabled = false;
            DownloadButton.IsEnabled = false;
            ScheduleList.IsEnabled = false;
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            IsolatedStorageSettings.ApplicationSettings.Remove("DownloadGroupName");
            IsolatedStorageSettings.ApplicationSettings.Add("DownloadGroupName", GroupBox.Text);
            wc.DownloadStringAsync(new Uri(ScheduleUri + GroupBox.Text));
            //wc.DownloadStringAsync(new Uri("https://dl.dropboxusercontent.com/u/46762801/Sites/velcom_tibo/050503.xml"));
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            GroupBox.IsEnabled = true;
            DownloadButton.IsEnabled = true;
            if (e.Error != null) { MessageBox.Show(AppResources.error_DownloadFile);}
            else
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream filestream = storage.CreateFile(IsolatedStorageSettings.ApplicationSettings["DownloadGroupName"] + Extension);

                StreamWriter sw = new StreamWriter(filestream);
                sw.Write(e.Result);
                sw.Close();

                filestream.Close();
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

        private void DeleteGroupScheduleLinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                if (((string)settings["filename"]) == ((List<GroupScheduleLink>)ScheduleList.ItemsSource)[ScheduleList.SelectedIndex].Filename)
                {
                    settings.Remove("filename");
                    settings.Save();
                }
            }
            catch (Exception) { }

            storage.DeleteFile(((List<GroupScheduleLink>)ScheduleList.ItemsSource)[ScheduleList.SelectedIndex].Filename);
            RefreshScheduleList();
        }

        private void GroupName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Remove("groupname");
            settings.Remove("filename");
            settings.Remove("subgroup");
            settings.Add("groupname", ((List<GroupScheduleLink>)ScheduleList.ItemsSource)[ScheduleList.SelectedIndex].Name);
            settings.Add("filename", ((List<GroupScheduleLink>)ScheduleList.ItemsSource)[ScheduleList.SelectedIndex].Filename);
            settings.Add("subgroup", 0);
            settings.Save();
            NavigationService.GoBack();
        }

        private void RefteshGroupScheduleLinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GroupBox.IsEnabled = false;
            DownloadButton.IsEnabled = false;
            ScheduleList.IsEnabled = false;
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            IsolatedStorageSettings.ApplicationSettings.Remove("DownloadGroupName");
            IsolatedStorageSettings.ApplicationSettings.Add("DownloadGroupName", ((List<GroupScheduleLink>)ScheduleList.ItemsSource)[ScheduleList.SelectedIndex].Name);
            wc.DownloadStringAsync(new Uri(ScheduleUri + ((List<GroupScheduleLink>)ScheduleList.ItemsSource)[ScheduleList.SelectedIndex].Name));
        }

        private void RefteshGroupScheduleLinkButton_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            var darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            if (darkVisibility == Visibility.Visible) 
            {
                bi.UriSource = new Uri("Icons/refresh.png", UriKind.Relative);
            }
            else 
            {
                bi.UriSource = new Uri("Icons/refresh_dark.png", UriKind.Relative);
            }
            ((Image)sender).Source = bi;
        }

        private void DeleteGroupScheduleLinkButton_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            var darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            if (darkVisibility == Visibility.Visible)
            {
                bi.UriSource = new Uri("Icons/delete.png", UriKind.Relative);
            }
            else
            {
                bi.UriSource = new Uri("Icons/delete_dark.png", UriKind.Relative);
            }
            ((Image)sender).Source = bi;
        }

        private void SubgroupSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                string groupname = (string)settings["groupname"];
                string filename = (string)settings["filename"];
                switch ((int)settings["subgroup"])
                {
                    case 0: BothGroupRadio.IsChecked = true; break;
                    case 1: FirstGroupRadio.IsChecked = true; break;
                    case 2: SecondGroupRadio.IsChecked = true; break;
                }
                SubgroupSwitch.Visibility = System.Windows.Visibility.Visible;
            }
            catch { SubgroupSwitch.Visibility = System.Windows.Visibility.Collapsed; }
        }

        private void BothGroupRadio_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("subgroup");
            IsolatedStorageSettings.ApplicationSettings.Add("subgroup", 0);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void FirstGroupRadio_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("subgroup");
            IsolatedStorageSettings.ApplicationSettings.Add("subgroup", 1);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void SecondGroupRadio_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("subgroup");
            IsolatedStorageSettings.ApplicationSettings.Add("subgroup", 2);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void GitHubLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("https://github.com/Humple/ScheduleBSUIR");
            wbt.Show();
        }
    }
}