using Azure;
using Azure.AI.TextAnalytics;
using CsvHelper;
using iText.Layout.Splitting;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static NLP.CsvHandler;

namespace NLP
{
    public static class CsvHandler
    {
        static DataTable MakeTable()
        {
            DataTable table = new DataTable();

            // Declare DataColumn and DataRow variables.
            DataColumn column;
            DataRow row;


            // Create new DataColumn, set DataType, ColumnName and add to DataTable.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "from";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "subject";
            table.Columns.Add(column);

            // Create third column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "body";
            table.Columns.Add(column);



            return table;
        }
        public static DataTable goodEmail = MakeTable();


        static string languageKey = "4Dn6NHj5MR04IEzv1Pa2HjK0lbhVjw2GZCir3gPSfE3w0djb6dkBJQQJ99ALACLArgHXJ3w3AAAEACOGl7tU";
        static string languageEndpoint = "https://emailanonymous.cognitiveservices.azure.com/";
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(languageKey);
        private static readonly Uri endpoint = new Uri(languageEndpoint);
        static string phonePattern = "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$";


        public class emails
        {
            public string From { get; set; }
            public string FromAddress { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }

        public class MyEntity
        {
            public string Category { get; set; }
            public double ConfidenceScore { get; set; }
            public int Length { get; set; }
            public int Offset { get; set; }
            public string SubCategory { get; set; }
            public string Text { get; set; }
        }
        private static int counter = 0;
        public static List<ReplaceData> listReplace = new List<ReplaceData>();

        /// <summary>
        /// Main block
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            DAL SFAUDIT = new DAL("Data Source=" + Config.ServerName.Value + ";Initial Catalog=" + Config.DatabaseName.Value + ";Integrated Security=True;Connection Timeout=30;");

            DataSet dsReplace = SFAUDIT.ExecuteSQL(Config.SelectReplacementData.Value, true);
            listReplace = dsReplace.Tables[0].AsEnumerable().Select(dtRow => new ReplaceData()
            {
                Row = Convert.ToInt32(dtRow["Row"]),
                Email = Convert.ToString(dtRow["Email"]),
                Phone = Convert.ToString(dtRow["Phone"]),
                Address = Convert.ToString(dtRow["Address"])
            }).ToList();

            Removal removal = new Removal();

            List<emails> records = new List<emails>();

            using (var reader = new StreamReader(@"c:\users\jhayes\documents\DocumentProcessingTeamInbox.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))

            {
                records = csv.GetRecords<emails>().ToList();
            }
            var client = new TextAnalyticsClient(endpoint, credentials);
            string body;
            Console.WriteLine("Number of emails: " + records.Count);
            int emailCounter = 0;
            foreach (emails email in records)
            {
                emailCounter++;
                if(emailCounter == 2500) { break; };            //only want 2500

                List<MyEntity> myEntities = new List<MyEntity>();
                string cleanedBody = email.Body.Replace(removal.NewLine, " ");               //has to go first
                //  Console.WriteLine(cleanedBody +  System.Environment.NewLine);
                cleanedBody = cleanedBody.Replace(removal.Caution, " ");
                cleanedBody = cleanedBody.Replace(removal.PleaseRead, " ");
                cleanedBody = cleanedBody.Replace(removal.Warning, " ");
                Console.WriteLine("Number : " + emailCounter + "   before: " + email.Body.Length + "  after: " + cleanedBody.Length);
                string[] clean = new string[] { null, null };

                try
                {
                    if (cleanedBody.Length < 10000 && emailCounter >= 2000)
                    {
                        if (cleanedBody.Length > 5000)
                        {
                            clean[0] = cleanedBody.Substring(0, 4999);
                            clean[1] = cleanedBody.Substring(5000, cleanedBody.Length - 5001);
                        }
                        else
                        {
                            clean[0] = cleanedBody;
                        }

                        RecognizePiiEntitiesOptions options = new() { CategoriesFilter = { PiiEntityCategory.Email, PiiEntityCategory.Address, PiiEntityCategory.PhoneNumber } };


                        PiiEntityCollection BodyEntities = client.RecognizePiiEntities(clean[0], options: options).Value;

                        foreach (PiiEntity entity in BodyEntities)
                        {
                            string cat = entity.Category.ToString();

                            MyEntity myentity = new MyEntity
                            {
                                Category = cat,
                                SubCategory = entity.SubCategory,
                                ConfidenceScore = entity.ConfidenceScore,
                                Offset = entity.Offset,
                                Length = entity.Length,
                                Text = entity.Text
                            };
                            myEntities.Add(myentity);
                        }
                        if (clean[1] != null)
                        {
                            BodyEntities = client.RecognizePiiEntities(clean[1], options: options).Value;
                            {

                                foreach (PiiEntity entity in BodyEntities)
                                {
                                    string cat = entity.Category.ToString();

                                    MyEntity myentity = new MyEntity
                                    {
                                        Category = cat,
                                        SubCategory = entity.SubCategory,
                                        ConfidenceScore = entity.ConfidenceScore,
                                        Offset = entity.Offset,
                                        Length = entity.Length,
                                        Text = entity.Text
                                    };
                                    myEntities.Add(myentity);
                                }

                            }
                        }

                        bool result = Redaction(email.Body, email.From, email.Subject, myEntities);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }


            DataView view = new DataView(goodEmail);

            var filepath = @"c:\users\jhayes\documents\redactedemailsTeam4.csv";
            using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
            //using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "|" }))

            {
                writer.WriteLine("\"From\"\t\"Subject\"\t\"Body\"");
                for (int i = 0; i < view.Count; i++)
                {

                    writer.WriteLine("\"" + view[i]["from"] + "\"\t" + "\"" + view[i]["subject"] + "\"\t" + "\"" + view[i]["body"] + "\"");
                }

            }
            Console.WriteLine("All dones with this shit");
            Console.ReadLine();
        }

        static bool Redaction(string body,string from, string subject,  List<MyEntity> Entities)
        {
            counter++;
            bool changeit = false; 
            DataRow row = null;
            string Redacted = body;
            string replace = "";
            foreach (MyEntity xxx in Entities) //{ Console.WriteLine($" {xxx.Category}   {xxx.Text}"); }
            {                                   //var csv = BodyEntities.AsQueryable().Where(c => c.Category == "Email" && !c.Text.Contains("fi.com")).Select(c => new { c.Text, c.Category }).Distinct();
                if (xxx.Category == PiiEntityCategory.Address)
                {
                    replace = listReplace[counter].Address; 
                    changeit = true;
                }
                else if (xxx.Category == PiiEntityCategory.Email && !xxx.Text.Contains("fi.com"))
                {
                    replace = listReplace[counter].Email;
                    changeit = true;
                }
                else if (xxx.Category == PiiEntityCategory.PhoneNumber && Regex.IsMatch(xxx.Text, phonePattern))
                {
                    replace = listReplace[counter].Phone;
                    changeit = true;
                }
                else
                {
                    changeit = false;
                }
                    if (changeit) { Redacted = Redacted.Replace(xxx.Text, replace); };
            }

            row = goodEmail.NewRow();
            row["from"] = from;
            row["subject"] = subject;
            row["body"] = Redacted;
            goodEmail.Rows.Add(row);
            return true;
        }
 

        public struct Removal
        {
            public string NewLine;
            public string PleaseRead;
            public string Warning;
            public string Caution;
            public string Address;
            public Removal()
            {
                NewLine = System.Environment.NewLine;
                PleaseRead = "PLEASE READ: The material contained in this email is confidential and solely for the use of the intended recipient. Please do not forward this email to others. Investment in securities involves the risk of loss. Past performance is no guarantee of future returns. Fisher Investments (“FI”) is not a broker-dealer and cannot provide assurances and makes no representations or warranties as to the timing or price terms for the purchase or sale of particular securities. Market conditions and liquidity constraints may make it difficult to execute certain trades and FI will attempt to do so on a best efforts basis. FI is not responsible for errors or omissions in links from non-affiliated websites or other publicly available third party material provided. Third party information provided by FI does not necessarily reflect the view of FI or its employees. All email sent to or from this address will be received or otherwise recorded by FI’s corporate email system and is subject to archival, monitoring or review by, and/or disclosure to, someone other than the recipient. FI's privacy policy can be found at https://www.fisherinvestments.com/en-us/privacy";
                Warning = "WARNING: For your own protection, avoid sending identifying information such as social security or account numbers to us or others via email. Do not send time-sensitive, action-oriented messages, such as transaction requests, via email as it is our policy not to accept such items electronically.";
                Caution = "*** CAUTION: This email originated from outside of the organization. Do not click link(s) or open attachment(s) unless you recognize the sender(s). ***";
                Address = "5525 NW Fisher Creek Drive Camas, WA 98607";
            }
        }

    }




//var csv = BodyEntities.AsQueryable().Where(c => c.Category == "Email" && !c.Text.Contains("fi.com")).Select(c => new { c.Text, c.Category }).Distinct();



//using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "|" }))
}