using CsvHelper.Configuration;

namespace PlanningPoker2018_backend_2.Entities
{
    public class CsvRow
    {
        
        public string Summary { get; set; }
        public string StoryPoints { get; set; }
    }

    public sealed class CsvRowMap : ClassMap<CsvRow>
    {
        public CsvRowMap()
        {
            AutoMap();
            Map(m => m.StoryPoints).Name("Story Points");
        }
    }
}