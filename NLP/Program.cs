// See https://aka.ms/new-console-template for more information
using Azure.AI.TextAnalytics;
using Azure;
using System;
using System.Data;
 
using System.Reflection;
using System.Security.Cryptography;
using CsvHelper;
using System.Globalization;
using NLP;
using System.Reflection.PortableExecutable;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Dynamic;
using CsvHelper.Configuration;
using System.IO;

namespace NLP
{
    public static class Program
    {



        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
 


         static List<csvData>  LoadCSV()
        {
            DataTable dt = new DataTable();
            List<csvData> data = new List<csvData>();
            using (var reader = new StreamReader(Path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    dt = new DataTable();
                    dt.Columns.Add("From", typeof(string));
                    dt.Columns.Add("Subject", typeof(string));
                    dt.Columns.Add("Body", typeof(string));
                    dt.Load(dr);

                }
 
            }
            
            return ConvertDataTable<csvData>(dt);
        }

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


static string languageKey = "4Dn6NHj5MR04IEzv1Pa2HjK0lbhVjw2GZCir3gPSfE3w0djb6dkBJQQJ99ALACLArgHXJ3w3AAAEACOGl7tU";
        static string languageEndpoint = "https://emailanonymous.cognitiveservices.azure.com/";
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(languageKey);
        private static readonly Uri endpoint = new Uri(languageEndpoint);
        static string Path = @"c:\users\jhayes\documents\DocumentInbox.csv";
        static int counter = 0;
        // Example method for detecting sensitive information (PII) from text 
        static PiiEntityCollection RecognizePII(TextAnalyticsClient client, string StringToTest, string Source)
        {
            string document = StringToTest;
            PiiEntityCollection entities = null;
            counter += 1;
            try
            {
                if (!string.IsNullOrEmpty(document))
                {

                    RecognizePiiEntitiesOptions options = new() { CategoriesFilter = { PiiEntityCategory.Email, PiiEntityCategory.Address , PiiEntityCategory.PhoneNumber} };
 

                    entities = client.RecognizePiiEntities(document, options: options).Value;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());   
            }
         //   Console.WriteLine("TestingString " + document.Substring(0,10));        
            //if (entities.Count > 0)
            //{
            //    Console.WriteLine($"Recognized {entities.Count} PII entit{(entities.Count > 1 ? "ies" : "y")}:");
            //    //foreach (PiiEntity entity in entities)
            //    //{
            //    //    if (entity.Category == "email")// | entity.Category.ToString() == "address" | entity.Category.ToString() == "phone" )
            //    //    {
            //    //                 Console.WriteLine($"Source: {Source}  Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
            //    //    }


            //    //  //  Console.WriteLine($"Id: {Id} Source: {Source}  Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
            //    //}
            //}
            //else
            //{
            //    Console.WriteLine("No entities were found.");
            //}
            return entities;
        }

        static void Main(string[] args)
        {
            DAL SFAUDIT = new DAL("Data Source=" + Config.ServerName.Value + ";Initial Catalog=" + Config.DatabaseName.Value + ";Integrated Security=True;Connection Timeout=30;");

            DataSet dsInbox = SFAUDIT.ExecuteSQL(Config.SelectStatement.Value, true);
            DataTable dtInbox = dsInbox.Tables[0];
            List<Inbox> lstInbox = new List<Inbox>();
            lstInbox = ConvertDataTable<Inbox>(dtInbox);
            var client = new TextAnalyticsClient(endpoint, credentials);
 
            DataTable goodEmail = MakeTable();
            DataRow row;
            foreach (Inbox ibox in lstInbox)
            {
                string body = ibox.Body.Length > 5000 ? ibox.Body.Substring(0, 5000) : ibox.Body;
                Console.WriteLine($"ID = {ibox.Id} ");
                // PiiEntityCollection SubjectLineEntities = RecognizePII(client, ibox.Subject, ibox.Id, "SUB");
                PiiEntityCollection BodyEntities = RecognizePII(client, body, "BOD");
                //    // only get non-fi emails
                if (BodyEntities != null)
                {
                    string Redacted = ibox.Body;
                    string replace = "";
                    foreach (var xxx in BodyEntities) //{ Console.WriteLine($" {xxx.Category}   {xxx.Text}"); }
                    {                                   //var csv = BodyEntities.AsQueryable().Where(c => c.Category == "Email" && !c.Text.Contains("fi.com")).Select(c => new { c.Text, c.Category }).Distinct();


                        if (xxx.Category == PiiEntityCategory.Address)
                        {
                            replace = "address here";
                        }
                        else if (xxx.Category == PiiEntityCategory.Email)
                        {
                            replace = "email@email.com";
                        }
                        else if (xxx.Category == PiiEntityCategory.PhoneNumber)
                        {
                            replace = "650-111-1234";
                        }
                        Redacted = Redacted.Replace(xxx.Text, replace);
                    }
 
                    row = goodEmail.NewRow();          
                    row["from"] = ibox.FromPerson;
                    row["subject"] = ibox.Subject;
                    row["body"] = Redacted;
                    goodEmail.Rows.Add(row);
                }


            }

                DataView view = new DataView(goodEmail);

            var filepath = @"c:\users\jhayes\documents\redactedemailsTeam.csv";
            using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
                //using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "|" }))

            {
                writer.WriteLine("\"From\"\t\"Subject\"\t\"Body\"");
                for (int i = 0; i < view.Count; i++)
                {

                    writer.WriteLine("\"" +  view[i]["from"] + "\"\t" + "\"" + view[i]["subject"] + "\"\t" + "\"" + view[i]["body"]  + "\"");
                }
 
            }

        

            Console.Write("Press any key to exit.");
            Console.ReadKey();


      
        }





















        //var client = new TextAnalyticsClient(endpoint, credentials);
        //List<csvData> listData = LoadCSV();

        //List<string> goodEmail = new List<string>();

        //foreach (csvData data in listData) 
        //{
        //    string body = data.Body.Length > 5000 ? data.Body.Substring(0, 5000) : data.Body;
        //    //  PiiEntityCollection SubjectLineEntities = RecognizePII(client, data.Subject,  "SUB");
        //    PiiEntityCollection BodyEntities = RecognizePII(client, body,  "BOD");
        //    // only get non-fi emails
        //    if (BodyEntities != null)
        //    {
        //        var csv = BodyEntities.AsQueryable().Where(c => c.Category == "Email" && !c.Text.Contains("fi.com")).Select(c => new { c.Text, c.Category });
        //        foreach (var email in csv)
        //        {
        //            goodEmail.Add(email.Text);              //retain all the source emails to change
        //        }
        //    }
        //}
    }

    
}