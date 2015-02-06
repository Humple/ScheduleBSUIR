using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ScheduleBSUIR.Models;

namespace ScheduleBSUIR.ViewModels
{
    [DebuggerDisplay("{ItemTitle}")]
    public class DayViewModel: ViewModelBase, IChainedItem
    {
        private ObservableCollection<SubjectViewModel> subjects;
        private string itemTitle;

        public ObservableCollection<SubjectViewModel> Subjects
        {
            get { return subjects; }
            set
            {
                subjects = value;
                NotifyPropertyChanged("Subjects");
            }
        }

        public IScheduleService DaySchedule { get; set; }

        public DateTime Date { get; set; }

        public string ItemTitle
        {
            get { return itemTitle; }
            set
            {
                itemTitle = TitleLengthHack( value );
                NotifyPropertyChanged("ItemTitle");
            }
        }

        public IChainedItem Next()
        {
           return DaySchedule.Get(Date.AddDays(1));           
        }

        public IChainedItem Previous()
        {
            return DaySchedule.Get(Date.AddDays(-1));            
        }

        public void UpdateContent()
        {
            DaySchedule.UpdateContent(this);
        }

        public void UpdateFrom(DayViewModel d)
        {
            Subjects = d.Subjects;
            ItemTitle = d.ItemTitle;
            Date = d.Date;
        }

        /// <summary> Hack for a title width. </summary>
        /// <param name="t"></param>
        /// <returns>Constant width string.</returns>
        private static string TitleLengthHack(string t)
        {
            const int titleWidth = 12;

            if (t.Length < titleWidth)
            {
                return t + new string(' ', titleWidth - t.Length);
            }

            return t;
        }
    }
}
