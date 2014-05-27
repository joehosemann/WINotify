using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WINotify
{
    public class GetResults
    {
        private string connectionString
        {
            get { return Properties.Settings.Default.CLARIFYDWConnectionString; }
        }

        public List<SqlResultsModel> GetResultsFromSQL(List<string> casenumbers)
        {
            string sql = "select dsite.Name, c.CaseNumber,c.Title,dcontact.Email,dcontact.FirstName,dcontact.LastName,dcasetype.[Type],c.TopicCode,c.Condition,c.CaseAge,c.CRID,c.WIPBIN,c.CQTICKET,c.DSSTYPE,c.ClientTrackingIDfrom FACT_CASE as cinner join DIM_SITE as dsite on c.siteid = dsite.idinner join DIM_CASETYPE as dcasetype on c.CaseTypeID = dcasetype.IDinner join DIM_CONTACT as dcontact on c.ContactID = dcontact.IDwhere c.CustomerSupport = 1 ANDc.CaseStatusID != '268435674' ANDc.CaseNumber = '@CaseNumber'";
            var casesFromSql = new List<SqlResultsModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                foreach (var item in casenumbers)
                {
                    var caseResult = new SqlResultsModel();

                    var command = new SqlCommand(sql, connection);
                    command.Parameters.Add(new SqlParameter("CaseNumber", item));
                    connection.Open();
                    SqlDataReader sqlReader = command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        caseResult.OrgName = sqlReader[0].ToString();
                        caseResult.CaseNumbers = sqlReader[1].ToString();
                        caseResult.CaseTitle = sqlReader[2].ToString();
                        caseResult.ContactEmail = sqlReader[3].ToString();
                        caseResult.ContactFirstName = sqlReader[4].ToString();
                        caseResult.ContactLastName = sqlReader[5].ToString();
                        caseResult.Type = sqlReader[6].ToString();
                        caseResult.TopicCode = sqlReader[7].ToString();
                        caseResult.Condition = sqlReader[8].ToString();
                        caseResult.CaseAge = sqlReader[9].ToString();
                        caseResult.CRID = sqlReader[10].ToString();
                        caseResult.WIPBIN = sqlReader[11].ToString();
                        caseResult.CQTicket = sqlReader[12].ToString();
                        caseResult.DSSType = sqlReader[13].ToString();
                        caseResult.ClientTrackingID = sqlReader[14].ToString();
                    }
                    sqlReader.Close();
                    command.Dispose();

                    casesFromSql.Add(caseResult);
                }
                connection.Close();
            }
            return casesFromSql;
        }
    }

}
