using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AppLimit.NetSparkle;

namespace WINotify
{
    public partial class Form1 : Form
    {
        static List<Model> Cases = new List<Model>();
        static Dictionary<string, string> OrgEmailList = new Dictionary<string, string>();
        private Sparkle _sparkle;

        private string connectionString
        {
            get { return Properties.Settings.Default.CLARIFYDWConnectionString; }
        }

        public Form1()
        {
            CopyDatabase();
            InitializeComponent();

            _sparkle = new Sparkle("http://bbecweb/utilities/winotify/update.xml");
            _sparkle.StartLoop(true);
        }
         
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            label2.Text = @"Status: Running. Please wait.";
            var textbox = textBox1.Text;
            label2.Text = @"Status: Running. Please wait.";
            string ntext;
            if (textbox.Trim().Contains(','))
            {
                ntext = textbox.Replace(Environment.NewLine, string.Empty);
                ntext = Regex.Replace(ntext, @"\s", string.Empty);
            }
            else
            {
                ntext = textbox.Trim().Replace(Environment.NewLine, ",");
            }
            var textarray = ntext.Split(',');
            var test = textarray.ToList();

            try
            {
                //GetResultsFromSQL(textarray.ToList());
                Money(textarray.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            label2.Text = @"Status: Finished.";
            button1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        #region Methods

        public void CopyDatabase()
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\db.orig";
            string path2 = Path.GetDirectoryName(Application.ExecutablePath) + "\\db.mdb";

            try
            {
                File.Copy(path, path2, true);
            }
            catch
            {
                MessageBox.Show(@"Please run application as Administrator or turn off UAC.");
            }
        }

        void Money(List<string> casenumbers)
        {
            var dc = new CaseDataContext();

            var cases = from c in dc.FACT_CASEs
                        join s in dc.DIM_SITEs on c.SiteID equals s.ID
                        join t in dc.DIM_CASETYPEs on c.CaseTypeID equals t.ID
                        join co in dc.DIM_CONTACTs on c.ContactID equals co.ID
                        where
                        c.CustomerSupport == true &&
                        c.CaseStatusID != 268435674 &&
                        casenumbers.Contains(c.CaseNumber)

                        select new
                        {
                            OrgName = s.Name,
                            CaseNumbers = c.CaseNumber,
                            CaseTitle = c.Title,
                            ContactEmail = co.EMail,
                            ContactFirstName = co.FirstName,
                            ContactLastName = co.LastName,
                            Type = t.Type,
                            TopicCode = c.TopicCode,
                            Condition = c.Condition,
                            CaseAge = c.CaseAge,
                            CRID = c.CRID,
                            WIPBIN = c.WIPBIN,
                            CQTicket = c.CQTICKET,
                            DSSType = c.DSSTYPE,
                            ClientTrackingID = c.ClientTrackingID
                        };

            var groupedCases = cases.GroupBy(x => x.OrgName);
            //var groupedEmails = cases.GroupBy(x => x.ContactEmail);

            if (groupedCases.Count() == 0)
            {
                MessageBox.Show("Failed to return any results.");
            }

            foreach (var groupedCase in groupedCases)
            {
                var acase = new Model();
                acase.OrgName = quoteMe(groupedCase.Key);

                foreach (var entry in groupedCase)
                {
                    if (entry.CaseNumbers == null)
                        acase.CaseNumbers.Append(Environment.NewLine);
                    else
                        acase.CaseNumbers.Append(entry.CaseNumbers + Environment.NewLine);

                    if (entry.CaseTitle == null)
                        acase.CaseTitle.Append(Environment.NewLine);
                    else
                        acase.CaseTitle.Append(entry.CaseTitle + Environment.NewLine);

                    if (entry.ContactEmail == null)
                        acase.ContactEmail.Append(Environment.NewLine);
                    else
                        acase.ContactEmail.Append(entry.ContactEmail + Environment.NewLine);

                    if (entry.ContactFirstName == null)
                        acase.ContactFirstName.Append(Environment.NewLine);
                    else
                        acase.ContactFirstName.Append(entry.ContactFirstName + Environment.NewLine);

                    if (entry.ContactLastName == null)
                        acase.ContactLastName.Append(Environment.NewLine);
                    else
                        acase.ContactLastName.Append(entry.ContactLastName + Environment.NewLine);

                    if (entry.Type == null)
                        acase.Type.Append(Environment.NewLine);
                    else
                        acase.Type.Append(entry.Type + Environment.NewLine);

                    if (entry.TopicCode == null)
                        acase.TopicCode.Append(Environment.NewLine);
                    else
                        acase.TopicCode.Append(entry.TopicCode + Environment.NewLine);

                    if (entry.Condition == null)
                        acase.Condition.Append(Environment.NewLine);
                    else
                        acase.Condition.Append(entry.Condition + Environment.NewLine);

                    if (entry.CaseAge == null)
                        acase.CaseAge.Append(Environment.NewLine);
                    else
                        acase.CaseAge.Append(entry.CaseAge.ToString() + Environment.NewLine);

                    if (entry.CRID == null)
                        acase.CRID.Append(Environment.NewLine);
                    else
                        acase.CRID.Append(entry.CRID.ToString() + Environment.NewLine);

                    if (entry.WIPBIN == null)
                        acase.WIPBIN.Append(Environment.NewLine);
                    else
                        acase.WIPBIN.Append(entry.WIPBIN + Environment.NewLine);

                    if (entry.CQTicket == null)
                        acase.CQTicket.Append(Environment.NewLine);
                    else
                        acase.CQTicket.Append(entry.CQTicket + Environment.NewLine);

                    if (entry.DSSType == null)
                        acase.DSSType.Append(Environment.NewLine);
                    else
                        acase.DSSType.Append(entry.DSSType + Environment.NewLine);

                    if (entry.ClientTrackingID == null)
                        acase.ClientTrackingID.Append(Environment.NewLine);
                    else
                        acase.ClientTrackingID.Append(entry.ClientTrackingID + Environment.NewLine);

                    // Produces a list of non duplicated emails, used to send the mailing to.

                    if (!OrgEmailList.ContainsKey(entry.ContactEmail.ToString()) && (!string.IsNullOrEmpty(entry.ContactEmail.ToString())))
                        OrgEmailList.Add(entry.ContactEmail.ToString(), entry.OrgName.ToString());
                }
                Cases.Add(acase);
            }

            var myConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Path.GetDirectoryName(Application.ExecutablePath) + "\\db.mdb");

            foreach (var email in OrgEmailList)
            {
                foreach (var model in Cases.Where(x => x.ContactEmail.ToString().Replace("'", "").Contains(email.Key)))
                {
                    myConnection.Open();

                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append(quoteMe(email.Key) + ",");
                    stringBuilder.Append(quoteMe(model.OrgName) + ",");
                    stringBuilder.Append(quoteMe(model.CaseNumbers.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.CaseTitle.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.ContactEmail.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.ContactFirstName.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.ContactLastName.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.Type.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.TopicCode.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.Condition.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.CaseAge.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.CRID.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.WIPBIN.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.CQTicket.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.DSSType.ToString()) + ",");
                    stringBuilder.Append(quoteMe(model.ClientTrackingID.ToString()));

                    var myCommand = new OleDbCommand();
                    myCommand.Connection = myConnection;
                    myCommand.CommandText = "INSERT INTO contactlist (RecipientEmail, OrgName, CaseNumbers, CaseTitle, ContactEmail, ContactFirstName, ContactLastName, Type, TopicCode, Condition, CaseAge, CRID, WIPBIN, CQTicket, DSSType, ClientTrackingID) VALUES (" + stringBuilder + ");";
                    myCommand.ExecuteNonQuery();
                    myCommand.Connection.Close();
                }
            }
        }
       
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        string quoteMe(string input)
        {
            return "'" + input.Replace("'", "") + "'";
        }

        #endregion

    }
}
