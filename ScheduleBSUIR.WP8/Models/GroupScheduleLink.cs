namespace ScheduleBSUIR.Models
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
}
