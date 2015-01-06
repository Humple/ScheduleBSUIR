using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBSUIR.Resources;
using ScheduleParser;

namespace ScheduleBSUIR.ViewModels
{
    public class SubjectViewModel : ViewModelBase
    {
        #region Static

        public static SubjectViewModel Empty = new SubjectViewModel()
        {
            Subject = AppResources.NoLessons
        };

        #endregion

        #region Private

        private string type;
        private string subject;
        private string place;
        private string time;
        private string number;
        private string lector;
        private string subgroup;
        private List<string> weeks;

        #endregion

        #region C-tor

        public SubjectViewModel(Lesson lesson)
        {
            Subject = lesson.Subject;
            Lector = Employee.ToShortName(lesson.Employee);
            Place = lesson.Auditory ?? string.Empty;
            Subgroup = lesson.Subgroup;
            Time = lesson.Time;
            Type = lesson.Type;
        }

        public SubjectViewModel()
        {            
        }

        #endregion

        #region Props

        public string Subject
        {
            get { return subject; }
            set
            {
                subject = value;
                NotifyPropertyChanged("Subject");
            }
        }

        public string Place
        {
            get { return place; }
            set
            {
                place = value;
                NotifyPropertyChanged("Place");
            }
        }

        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }

        public string Lector
        {
            get { return lector; }
            set
            {
                lector = value;
                NotifyPropertyChanged("Lector");
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value.ToLowerInvariant();
                NotifyPropertyChanged("Type");
            }
        }

        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                NotifyPropertyChanged("Number");
            }
        }

        public string Subgroup
        {
            get { return subgroup; }
            set
            {
                subgroup = value;
                NotifyPropertyChanged("Subgroup");
            }
        }

        public List<string> Weeks
        {
            get { return weeks; }
            set
            {
                weeks = value;
                NotifyPropertyChanged("Weeks");
            }
        }

        #endregion

    }
}
