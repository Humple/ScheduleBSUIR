using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Linq;
using ScheduleBSUIR.Resources;

namespace ScheduleBSUIR
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly int PagesCount = 5;
        XDocument document;
        private bool FirstShow = true;
        
        string currentGroup;
        DateTime currentDate;
        int prevIndex;

        List<SubjectItemData> Schedule = new List<SubjectItemData>();

        public MainPage()
        {
            InitializeComponent();

            //ApplicationBar localization
            ApplicationBar = GetValue(ApplicationBarProperty) as Microsoft.Phone.Shell.ApplicationBar;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.Today;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.Options;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).Text = AppResources.ChooseDate;
            //end ApplicationBar localization

            for (int i = 0; i < PagesCount; i++)
            {
                PivotItem pi = new PivotItem();
                pivot.Items.Add(pi);
            }
        }


        // Метод генерирует имена заголовков в pivot в зависимоти от переданного дня
        void GenerateDayNames(DateTime day)
        {
            currentDate = day;
            for (int i = 0; i < PagesCount; i++)
            {
                int delta = PagesCount - pivot.SelectedIndex + i;
                if (delta >= PagesCount) delta -= PagesCount;
                if (delta == PagesCount - 1) delta = -1;
                string HeaderText;
                if (delta == PagesCount - 2) delta = -2;
                if (day.AddDays(delta) == DateTime.Today) HeaderText = AppResources.Today;
                else if (day.AddDays(delta) == DateTime.Today.AddDays(1)) HeaderText = AppResources.Tomorrow;
                else if (day.AddDays(delta) == DateTime.Today.AddDays(-1)) HeaderText = AppResources.Yesterday;
                else HeaderText = day.AddDays(delta).ToShortDateString();
                ((PivotItem)pivot.Items[i]).Header = HeaderText;
            }
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SetupPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            try
            { // Попытка загрузить последнее расписание
                string groupname = (string)settings["groupname"];
                string filename = (string)settings["filename"];
                if (currentGroup != groupname)
                {
                    currentGroup = groupname;
                    try
                    {
                        document = XDocument.Load(
                        IsolatedStorageFile.GetUserStoreForApplication().
                        OpenFile(filename, FileMode.Open));
                    }
                    catch
                    {
                        MessageBox.Show(AppResources.error_LoadFile);
                        OptionsButton_Click(this, null);
                    }
                    GenerateDayNames(DateTime.Today);
                }
            }
            catch
            { // Показать страницу настроек если не удалось
                if (FirstShow) OptionsButton_Click(this, null);
                else NavigationService.GoBack();
            }

            if (settings.Contains("DateTimePickerPage_rv"))
            { // Проверка перехода со страницы выбора даты
                GenerateDayNames(DateTime.Parse((string)settings["DateTimePickerPage_rv"]));
                settings.Remove("DateTimePickerPage_rv");
            }

            if (!FirstShow) GenerateSchedule();
            FirstShow = false;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            for (int i = 0; i < PagesCount; i++)
            {
                ((PivotItem)pivot.Items[i]).Content = null;
            }
        }

        private void DateCoiceButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml" + "?date=" + currentDate.ToShortDateString(), UriKind.Relative));
        }


        private void pivot_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        { // асинзронная подгрузка расписания при перелистывании
            int tmp = pivot.SelectedIndex - prevIndex;
            if (tmp > 1) tmp = -1;
            if (tmp < -1) tmp = 1;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GenerateDayNames(currentDate.AddDays(tmp));
                GenerateSchedule();
            });;
        }

        private void pivot_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            prevIndex = pivot.SelectedIndex;
        }

        private void HomeButton_Click(object sender, EventArgs e)
        { // Возврат нa сегодняшнюю дату
            if (currentDate != DateTime.Today)
            {
                if (currentDate > DateTime.Today)
                {
                    if (pivot.SelectedIndex != 0) pivot.SelectedIndex -= 1;
                    else pivot.SelectedIndex = PagesCount - 1;
                }
                else
                {
                    if (pivot.SelectedIndex != PagesCount - 1) pivot.SelectedIndex += 1;
                    else pivot.SelectedIndex = 0;
                }
                GenerateDayNames(DateTime.Today);
                GenerateSchedule();
            }
        }

        string GetDayOfWeekName(DateTime date)
        {
            switch ((int)currentDate.DayOfWeek)
            {
                case 1: return "пн";
                case 2: return "вт";
                case 3: return "ср";
                case 4: return "чт";
                case 5: return "пт";
                case 6: return "сб";
                default: return "";
            }
        }

        int GetBSUIRWeekNumber(DateTime date)
        { // Расчет номера текущей недели БГУИР
            DateTime startdate;
            int daydelta, weeknum = 0;

            if (date.Month < 8) startdate = new DateTime(date.Year - 1, 9, 1);
            else startdate = new DateTime(date.Year, 9, 1);

            startdate = startdate.AddDays(-1 * (int)startdate.DayOfWeek);

            if (date.Month < 8) daydelta = Math.Abs(date.DayOfYear + (new DateTime(startdate.Year, 12, 31).DayOfYear - startdate.DayOfYear));
            else daydelta = Math.Abs(date.DayOfYear - startdate.DayOfYear);

            while (daydelta > 7)
            {
                daydelta -= 7;
                weeknum++;
            }
            weeknum++;
            while (weeknum > 4) weeknum -= 4;
            return weeknum;
        }
        string GetClassNumber(string time)
        {
            switch (time)
            {
                case "8:00-9:35": return "1";
                case "9:45-11:20": return "2";
                case "11:40-13:15": return "3";
                case "13:25-15:00": return "4";
                case "15:20-16:55": return "5";
                case "17:05-18:40": return "6";
                case "18:45-20:20": return "7";
                case "20:25-22:00": return "8";
                default: return "";
            }
        }

        delegate void MethodDelegate();

        void GenerateSchedule()
        { // Выборка из XML файла расписания на текущий день и загрузка его на экран
            if (document != null)
            {
                Schedule.Clear();
                string week = GetBSUIRWeekNumber(currentDate).ToString();
                string subgroup = ((int)IsolatedStorageSettings.ApplicationSettings["subgroup"]).ToString();
                foreach (XElement el in document.Root.Elements())
                {
                    if (el.Attribute("weekDay").Value == GetDayOfWeekName(currentDate))
                    {
                        if (el.Attribute("weekList").Value == "" ||
                            el.Attribute("weekList").Value.Split(',').Contains(week))
                        {
                            if (el.Attribute("subgroup").Value == "" ||
                                el.Attribute("subgroup").Value == subgroup ||
                                subgroup == "0")
                            {
                                string tmp = "";
                                if (el.Attribute("subgroup").Value != "") tmp = '(' + el.Attribute("subgroup").Value + ')';
                                Schedule.Add(new SubjectItemData(el.Attribute("subject").Value + " " + tmp,
                                                    el.Attribute("auditorium").Value,
                                                    el.Attribute("timePeriod").Value,
                                                    el.Attribute("teacher").Value,
                                                    el.Attribute("subjectType").Value,
                                                    el.Attribute("subgroup").Value,
                                                    GetClassNumber(el.Attribute("timePeriod").Value).ToString()
                                                    ));
                            }

                        }
                    }
                }
                ListBox lb = new ListBox();
                lb.ItemTemplate = (DataTemplate)Resources["SubjectItemDataTemplate"];
                lb.ItemsSource = Schedule;
                ((PivotItem)pivot.SelectedItem).Content = lb;
                TextBlock b;
            }
        }
    }
}