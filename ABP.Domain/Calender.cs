using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain
{
    public class Calender
    {
        public DateTime EventStartDateTime
        {
            get;
            set;
        }
        public DateTime EventEndDateTime
        {
            get;
            set;
        }
        public DateTime EventTimeStamp
        {
            get;
            set;
        }
        public DateTime EventCreatedDateTime
        {
            get;
            set;
        }
        public DateTime EventLastModifiedTimeStamp
        {
            get;
            set;
        }
        public string UID
        {
            get;
            set;
        }
        public string EventDescription
        {
            get;
            set;
        }
        public string EventLocation
        {
            get;
            set;
        }
        public string EventSummary
        {
            get;
            set;
        }
        public string AlarmTrigger
        {
            get;
            set;
        }
        public string AlarmRepeat
        {
            get;
            set;
        }
        public string AlarmDuration
        {
            get;
            set;
        }
        public string AlarmDescription
        {
            get;
            set;
        }
        public string ToMail { get; set; }
        public Calender()
        {
            EventTimeStamp = DateTime.Now;
            EventCreatedDateTime = EventTimeStamp;
            EventLastModifiedTimeStamp = EventTimeStamp;
        }
        public class Registration
        {
            public List<Registration> Reservations { get; set; }

            public string BeginDate { get; set; }
            public string EndDate { get; set; }
            public string DetailsHTML { get; set; }
            public string Location { get; set; }
            public string Summary { get; set; }
            public string Details { get; set; }

        }
    }
}
