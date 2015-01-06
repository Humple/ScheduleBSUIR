namespace ScheduleBSUIR.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using ScheduleParser;
    using ViewModels;
    
    public class SubjectListBuildingStrategy
    {
        private readonly Schedule schedule;

        public SubjectListBuildingStrategy(Schedule schedule)
        {
            this.schedule = schedule;
        }

        public IList<SubjectViewModel> GenerateList(DateTime effectiveDate)
        {
            List<SubjectViewModel> result;

            string week = effectiveDate.ToBsuirWeek().ToString(CultureInfo.InvariantCulture);
            string weekDayName = effectiveDate.ToDayNameRu();
            string allowedSubgroups = ((string) IsolatedStorageSettings.ApplicationSettings["subgroup"]);

            Day day = schedule.Days.FirstOrDefault(d => d.Name.ToLowerInvariant() == weekDayName);

            if (day == null)
            {
                result = new List<SubjectViewModel>() { SubjectViewModel.Empty };
            }
            else
            {
                result = day
                    .Lessons
                    .Where(l =>
                        allowedSubgroups.Contains(l.Subgroup)
                        &&
                        l.Weeks.Contains(week))
                    .Select(SubjectDataFromLesson)
                    .ToList();

                if (!result.Any())
                {
                    result.Add(SubjectViewModel.Empty);
                }
            }

            return result;
        }

        private SubjectViewModel SubjectDataFromLesson(Lesson l)
        {
            return new SubjectViewModel()
            {
                Lector = Employee.ToShortName(l.Employee),
                Number = GetLessonOrder(l.Time),
                Subject = l.Subject,
                Subgroup = l.Subgroup,
                Place = l.Auditory ?? string.Empty,
                Time = l.Time,
                Type = l.Type,
                Weeks = l.Weeks
            };
        }
        
        private static string GetLessonOrder(string time)
        {
            return time.StartsWith("08:")
                ? "1"
                : time.StartsWith("09:")
                    ? "2"
                    : time.StartsWith("11:")
                        ? "3"
                        : time.StartsWith("13:")
                            ? "4"
                            : time.StartsWith("15:")
                                ? "5"
                                : time.StartsWith("17:")
                                    ? "6"
                                    : time.StartsWith("18:")
                                        ? "7"
                                        : time.StartsWith("20:")
                                            ? "8"
                                            : "";
        }        
    }
}