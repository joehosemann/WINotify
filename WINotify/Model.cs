using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WINotify
{
    public class Model
    {
        public string OrgName { get; set; }
        public StringBuilder CaseNumbers { get; set; }
        public StringBuilder CaseTitle { get; set; }
        public StringBuilder ContactEmail { get; set; }
        public StringBuilder ContactFirstName { get; set; }
        public StringBuilder ContactLastName { get; set; }
        public StringBuilder Type { get; set; }
        public StringBuilder TopicCode { get; set; }
        public StringBuilder Condition { get; set; }
        public StringBuilder CaseAge { get; set; }
        public StringBuilder CRID { get; set; }
        public StringBuilder WIPBIN { get; set; }
        public StringBuilder CQTicket { get; set; }
        public StringBuilder DSSType { get; set; }
        public StringBuilder ClientTrackingID { get; set; }

        public Model()
        {
            CaseNumbers = new StringBuilder();
            CaseTitle = new StringBuilder();
            ContactEmail = new StringBuilder();
            ContactFirstName = new StringBuilder();
            ContactLastName = new StringBuilder();
            Type = new StringBuilder();
            TopicCode = new StringBuilder();
            Condition = new StringBuilder();
            CaseAge = new StringBuilder();
            CRID = new StringBuilder();
            WIPBIN = new StringBuilder();
            CQTicket = new StringBuilder();
            DSSType = new StringBuilder();
            ClientTrackingID = new StringBuilder();
        }

    }

    public class SqlResultsModel
    {
        public string OrgName { get; set; }
        public string CaseNumbers { get; set; }
        public string CaseTitle { get; set; }
        public string ContactEmail { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Type { get; set; }
        public string TopicCode { get; set; }
        public string Condition { get; set; }
        public string CaseAge { get; set; }
        public string CRID { get; set; }
        public string WIPBIN { get; set; }
        public string CQTicket { get; set; }
        public string DSSType { get; set; }
        public string ClientTrackingID { get; set; }
    }
}
