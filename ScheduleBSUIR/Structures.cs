using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;

namespace ScheduleBSUIR
{
    public class GroupScheduleLink
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public GroupScheduleLink(string filename)
        {
            Filename = filename;
            Name = filename.Substring(0, filename.LastIndexOf('.'));
        }
    }
    public class SubjectItemData
    {
        public string Subject { get; set; }
        public string Place { get; set; }
        public string Time { get; set; }
        public string Lector { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public string Subgroup { get; set; }
        public string NumberColor { get; set; }
        public SubjectItemData(string subject, string place, string time, string lector, string type, string subgroup, string number)
        {
            Subject = subject;
            Place = place;
            Time = time;
            Lector = lector;
            Type = type;
            Number = number;
            Subgroup = subgroup;
            switch (type)
            {
                case "лр": NumberColor = "Crimson"; break;
                case "пз": NumberColor = "LimeGreen"; break;
                case "лк": NumberColor = "DeepSkyBlue"; break;
                default: NumberColor = "White"; break;
            }
        }
    }
}
