using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBSUIR.ViewModels;
using ScheduleParser;

namespace ScheduleBSUIR.Models
{
    public interface IScheduleService
    {
        DayViewModel Get();

        DayViewModel Get(DateTime date);

        IEnumerable<DayViewModel> GetSequence(DateTime date, int number);
        void UpdateContent(DayViewModel dayViewModel);
    }
}
