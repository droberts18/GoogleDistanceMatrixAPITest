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
        static string outputLocations(string[] locations)
        {
            string result ="";

            foreach(string s in locations)
            {
                result += s;
                if (s != locations[locations.Count() - 1])
                    result += "|";
            }
            return result;
        }

        static void testXMLRetrieval() {
            DistanceObj exObj = new DistanceObj();

            try
            {
                // Referencing App.config
                string distanceApiUrl = ConfigurationManager.AppSettings["GoogleDistanceMatrixApi"];
                string apiKey = ConfigurationManager.AppSettings["ApiKey"];

                string whitworthCoord = "47.754721,-117.417948";
                string arcOfSpokaneCoord = "47.654509,-117.405558";
                string holmesElementaryCoord = "47.669529,-117.449858";
                string ronaldHouseCoord = "47.651293,-117.427355";
                string rockwoodCoord = "47.651282,-117.423641";

                string[] locations = { whitworthCoord, arcOfSpokaneCoord, holmesElementaryCoord, ronaldHouseCoord, rockwoodCoord};
                string locationsUrlFormat = outputLocations(locations);

                string url = distanceApiUrl + "&origins=" + locationsUrlFormat +
                    "&destinations=" + locationsUrlFormat +
                    "&key=" + apiKey;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sreader = new StreamReader(dataStream);
                string responseReader = sreader.ReadToEnd();
                response.Close();
                DataSet ds = new DataSet();
                ds.ReadXml(new XmlTextReader(new StringReader(responseReader)));

                string test = ds.Tables["origin_address"].Rows[0].ItemArray[0].ToString();
                Console.WriteLine(test);
                //return;

                if (ds.Tables.Count > 0)
                {
                    // elementNum must increment, eventually reaching square of number of locations
                    // e.g. 5 locations means elementNum must reach 25
                    int elementNum = 0;

                    for (int rowNum = 0; rowNum < ds.Tables["row"].Rows.Count; rowNum++)
                    {
                        for(int destNum = 0; destNum < ds.Tables["row"].Rows.Count; destNum++)
                        {
                            if (ds.Tables["element"].Rows[rowNum]["status"].ToString() == "OK")
                            {
                                Console.WriteLine("ORIGIN: " + ds.Tables["origin_address"].Rows[rowNum].ItemArray[0]);
                                Console.WriteLine("DESTINATION: " + ds.Tables["origin_address"].Rows[destNum].ItemArray[0]);
                                exObj.duration = Convert.ToString(ds.Tables["duration"].Rows[elementNum]["text"].ToString().Trim());
                                exObj.distance = Convert.ToString(ds.Tables["distance"].Rows[elementNum]["text"].ToString().Trim());
                                Console.WriteLine(exObj.duration);
                                Console.WriteLine(exObj.distance + "\n");

                                elementNum++;
                            }
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
