using System.Collections.Generic;
using System.Xml.Serialization;

namespace ScheduleParser
{
    [XmlRoot(ElementName= "employeeXmlModels")]
    public class Employees
    {
        [XmlElement(ElementName = "employee")]
        public List<Employee> AllEmployees { get; set; } 
    }

    [XmlRoot(ElementName= "scheduleXmlModels")]
    public class Schedule
    {
        [XmlElement(ElementName = "scheduleModel")]
        public List<Day> Days { get; set; }        
    }

    public class Day
    {
        [XmlElement(ElementName = "schedule")]
        public List<Lesson> Lessons { get; set; }

        [XmlElement(ElementName = "weekDay")]
        public string Name { get; set; }
    }

    public class Lesson
    {
        [XmlElement(ElementName = "auditory", IsNullable = true)]
        public string Auditory { get; set; }

        [XmlElement(ElementName = "employee", Type = typeof(Employee))]
        public Employee Employee { get; set; }

        [XmlElement(ElementName = "lessonTime")]
        public string Time { get; set; }

        [XmlElement(ElementName = "lessonType")]
        public string Type { get; set; }

        [XmlElement(ElementName = "note", IsNullable = true)]
        public string Note { get; set; }

        [XmlElement(ElementName = "studentGroup")]
        public string Group { get; set; }

        [XmlElement(ElementName = "numSubgroup")]
        public string Subgroup { get; set; }

        [XmlElement(ElementName = "subject")]
        public string Subject { get; set; }

        [XmlElement(ElementName = "weekNumber")]
        public List<string> Weeks { get; set; }

    }

    public class Employee
    {

        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "academicDepartment")]
        public string Department { get; set; }

        [XmlElement(ElementName = "firstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "lastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "middleName")]
        public string MiddleName { get; set; }

        public static string ToShortName(Employee e)
        {
            return e != null 
                ? string.Format("{0} {1}.{2}.", e.LastName, e.FirstName.Substring(0, 1), e.MiddleName.Substring(0, 1)) 
                : string.Empty;
        }
    }
}