using System;

namespace ScanIT.Models
{

    public class ScanHistoryItem
    {

        public string Code { get; set; }
        public string CodeType { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
    }

}