using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GoogleDistanceMatrixAPITest
{
    class Program
    {
        static void testXMLRetrieval() {
            DistanceObj exObj = new DistanceObj();

            try
            {
                // Referencing App.config
                string distanceApiUrl = ConfigurationManager.AppSettings["GoogleDistanceMatrixApi"];
                string apiKey = ConfigurationManager.AppSettings["ApiKey"];

                string url = distanceApiUrl + "&origins=" + "40.741895,-73.989308|47.6694788,-117.44983669999999" +
                    "&destinations=" + "40.741895,-73.989308|47.6694788,-117.44983669999999" +
                    "&key=" + apiKey;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sreader = new StreamReader(dataStream);
                string responseReader = sreader.ReadToEnd();
                response.Close();
                DataSet ds = new DataSet();
                ds.ReadXml(new XmlTextReader(new StringReader(responseReader)));
                if (ds.Tables.Count > 0)
                {
                    for (int rowNum = 0; rowNum < ds.Tables["element"].Rows.Count; rowNum++)
                    {
                        if (ds.Tables["element"].Rows[rowNum]["status"].ToString() == "OK")
                        {
                            exObj.duration = Convert.ToString(ds.Tables["duration"].Rows[rowNum]["text"].ToString().Trim());
                            exObj.distance = Convert.ToString(ds.Tables["distance"].Rows[rowNum]["text"].ToString().Trim());
                            Console.WriteLine(exObj.duration);
                            Console.WriteLine(exObj.distance + "\n");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error calculating distance: " + ex.Message);
            }
        }

        static void Main(string[] args)
        {
            testXMLRetrieval();
        }
    }
}
