namespace ScheduleBSUIR
{
    using ViewModels;
    using Models;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System.IO.IsolatedStorage;
    using System.IO;
    using Resources;
    using ScheduleParser;

    public partial class MainPage : PhoneApplicationPage
    {
        private const int PagesCount = 5;
        private Schedule schedule;
        private string currentGroup;
        private IScheduleService scheduleService;
        private bool processingSelectionChange;

        public MainPage()
        {
            InitializeComponent();
            
            ((ApplicationBarIconButton) ApplicationBar.Buttons[0]).Text = AppResources.Today;
            ((ApplicationBarIconButton) ApplicationBar.Buttons[1]).Text = AppResources.Options;
            ((ApplicationBarIconButton) ApplicationBar.Buttons[2]).Text = AppResources.ChooseDate;

            DataContext = App.ViewModel;
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SetupPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                // Попытка загрузить последнее расписание
                string groupname = (string) settings["groupname"];
                string filename = (string) settings["filename"];
                if (currentGroup != groupname)
                {
                    currentGroup = groupname;
                    try
                    {
                        schedule = SchemaToObjectTree.ParseDocument<Schedule>(
                            IsolatedStorageFile.GetUserStoreForApplication()
                                .OpenFile(filename, FileMode.Open));

                        scheduleService = new ScheduleService(schedule);
                    }
                    catch
                    {
                        MessageBox.Show(AppResources.error_LoadFile);
                        OptionsButton_Click(this, null);
                    }
                }
                App.ViewModel.LoadData(scheduleService.GetSequence(App.ViewModel.CurrentDate.AddDays(-PagesCount/2), PagesCount));
            }
            catch
            {
                OptionsButton_Click(this, null);
            }

            if (settings.Contains("DateTimePickerPage_rv"))
            {
                // Проверка перехода со страницы выбора даты                
                DateTime date = DateTime.Parse((string) settings["DateTimePickerPage_rv"]);
                settings.Remove("DateTimePickerPage_rv");
                App.ViewModel.LoadData(scheduleService.GetSequence(date.AddDays(-PagesCount/2), PagesCount));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.ViewModel.ShowedDays = null;
        }

        private void DateCoiceButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(
                new Uri(
                    "/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml?date=" +
                    App.ViewModel.CurrentDate.ToShortDateString(), UriKind.Relative));
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!processingSelectionChange && App.ViewModel.IsDataLoaded)
            {
                Pivot pivot = sender as Pivot;
                int nextSelectedIndex = pivot.SelectedIndex;

                MainViewModel vmCopy = App.ViewModel;

                int baseIndex = vmCopy.ShowedDays.Count;
                int deltaMax = baseIndex - 1;

                processingSelectionChange = true;
                try
                {
                    int delta = pivot.SelectedIndex - vmCopy.PreviousSelectedIndex;
                    if ((delta > 0 && delta != deltaMax) || delta == -deltaMax)
                    {
                        // going to the right
                        vmCopy.ShowedDays[
                            GetNormalizedIndex(vmCopy.PreviousSelectedIndex - 2, baseIndex)]
                            .UpdateFrom(
                            (DayViewModel)vmCopy.ShowedDays[
                            GetNormalizedIndex(vmCopy.PreviousSelectedIndex + 2, baseIndex)]
                            .Next());
                    }
                    else if ((delta < 0 && delta != -deltaMax) || delta == deltaMax)
                    {
                        // going to the left
                        vmCopy.ShowedDays[GetNormalizedIndex(vmCopy.PreviousSelectedIndex + 2, baseIndex)]
                            .UpdateFrom(
                            (DayViewModel)vmCopy.ShowedDays[GetNormalizedIndex(vmCopy.PreviousSelectedIndex - 2, baseIndex)]
                            .Previous());
                    }

                    if (delta != 0)
                    {
                        vmCopy.PreviousSelectedIndex = nextSelectedIndex;
                        vmCopy.ShowedDays[nextSelectedIndex].UpdateContent();
                    }
                }
                finally
                {
                    processingSelectionChange = false;
                }
            }
        }

        private static int GetNormalizedIndex(int index, int baseIndex)
        {
            if (index < 0)
            {
                return baseIndex + index;
            }

            if (index >= baseIndex)
            {
                return index - baseIndex;
            }

            return index;
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.LoadData(scheduleService.GetSequence(DateTime.Today.AddDays(-PagesCount/2), PagesCount));
        }
    }
}