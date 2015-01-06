namespace ScheduleBSUIR.Models
{
    public class SubgroupScope
    {
        public const string All = "012";

        public const string First = "01";

        public const string Second = "02";


        public static string ForSubgroupNumber(int number)
        {
            switch (number)
            {
                case 0:
                    return All;
                case 1:
                    return First;
                case 2:
                    return Second;
                default:
                    return All;
            }
        }
    }
}
