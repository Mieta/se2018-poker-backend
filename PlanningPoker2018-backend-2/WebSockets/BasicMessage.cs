using Newtonsoft.Json;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class BasicMessage
    {
        public string message { get; set; }
        public string type { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}