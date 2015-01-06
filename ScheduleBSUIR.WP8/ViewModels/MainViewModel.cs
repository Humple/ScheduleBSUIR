using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScheduleBSUIR.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private DayViewModel selectedDay;
        private ObservableCollection<DayViewModel> showedDays;
        private int previousSelectedIndex;        

        public DateTime CurrentDate
        {
            get { return SelectedDay != null ? SelectedDay.Date : DateTime.Today; }
        }

        public bool IsDataLoaded { get; private set; }

        public void LoadData(IEnumerable<DayViewModel> days)
        {
            IsDataLoaded = false;

            ShowedDays = new ObservableCollection<DayViewModel>(days);
            PreviousSelectedIndex = ShowedDays.Count / 2;
            SelectedDay = ShowedDays[PreviousSelectedIndex];
            ShowedDays[PreviousSelectedIndex].UpdateContent();

            IsDataLoaded = true;            
        }
        
        public DayViewModel SelectedDay
        {
            get { return selectedDay; }
            set
            {
                selectedDay = value;
                NotifyPropertyChanged("SelectedDay");                
            }
        }

        public int PreviousSelectedIndex
        {
            get { return previousSelectedIndex; }
            set
            {
                if (previousSelectedIndex != value)
                {
                    previousSelectedIndex = value;
                    NotifyPropertyChanged("PreviousSelectedIndex");
                }
            }
        }

        public ObservableCollection<DayViewModel> ShowedDays
        {
            get { return showedDays; }
            set
            {
                showedDays = value;
                NotifyPropertyChanged("ShowedDays");                
            }
        }       
    }
}