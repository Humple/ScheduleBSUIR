using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ScheduleBSUIR.Resources;
using ScheduleBSUIR.ViewModels;
using ScheduleParser;

namespace ScheduleBSUIR.Models
{
    public class ScheduleService: IScheduleService
    {
        private readonly SubjectListBuildingStrategy subjectBuilding;

        public ScheduleService(Schedule schedule)
        {
            EffectiveDate = DateTime.Today;
            subjectBuilding = new SubjectListBuildingStrategy(schedule);
        }

        public DateTime EffectiveDate { get; set; }

        public DayViewModel Get()
        {
            return Get(EffectiveDate);
        }

        public DayViewModel Get(DateTime date)
        {
            return new DayViewModel()
            {
                Date = date,
                DaySchedule = this,
                ItemTitle = GenerateDayName(date)
            };
        }

        public IEnumerable<DayViewModel> GetSequence(DateTime date, int number)
        {
            return Enumerable.Range(0, number)
                .Select(daySpan => Get(date.AddDays(daySpan)));
        }

        public void UpdateContent(DayViewModel dayViewModel)
        {
            dayViewModel.Subjects = new ObservableCollection<SubjectViewModel>(
                subjectBuilding.GenerateList(dayViewModel.Date));
        }

        private static string GenerateDayName(DateTime day)
        {
            string headerText;
            if (day == DateTime.Today)
            {
                headerText = AppResources.Today;
            }
            else if (day == DateTime.Today.AddDays(1))
            {
                headerText = AppResources.Tomorrow;
            }
            else if (day == DateTime.Today.AddDays(-1))
            {
                headerText = AppResources.Yesterday;
            }
            else
            {
                headerText = day.ToShortDateString();
            }

            return headerText;
        }
    }
}
