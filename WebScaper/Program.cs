using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using HtmlAgilityPack;

namespace WebScaper
{
    class Program
    {

        static void Main(string[] args)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

                Scrape(web, ConfigurationManager.AppSettings["SJS1"].ToString(), ConfigurationManager.AppSettings["SJS2"].ToString(), ConfigurationManager.AppSettings["SJS3"].ToString(), "SJS", "San Jose Sabercats");
                Scrape(web, ConfigurationManager.AppSettings["BH1"].ToString(), ConfigurationManager.AppSettings["BH2"].ToString(), ConfigurationManager.AppSettings["BH3"].ToString(), "BH", "Baltimore Hawks");
                Scrape(web, ConfigurationManager.AppSettings["CY1"].ToString(), ConfigurationManager.AppSettings["CY2"].ToString(), ConfigurationManager.AppSettings["CY3"].ToString(), "CY", "Colorado Yetis");
                Scrape(web, ConfigurationManager.AppSettings["PL1"].ToString(), ConfigurationManager.AppSettings["PL2"].ToString(), ConfigurationManager.AppSettings["PL3"].ToString(), "PL", "Philadelphia Liberty");
                Scrape(web, ConfigurationManager.AppSettings["YW1"].ToString(), ConfigurationManager.AppSettings["YW2"].ToString(), ConfigurationManager.AppSettings["YW3"].ToString(), "YW", "Yellowknife Wraiths");
                Scrape(web, ConfigurationManager.AppSettings["AO1"].ToString(), ConfigurationManager.AppSettings["AO2"].ToString(), ConfigurationManager.AppSettings["AO3"].ToString(), "AO", "Arizona Outlaws");
                Scrape(web, ConfigurationManager.AppSettings["NO1"].ToString(), ConfigurationManager.AppSettings["NO2"].ToString(), ConfigurationManager.AppSettings["NO3"].ToString(), "NO", "New Orleans Second Line");
                Scrape(web, ConfigurationManager.AppSettings["OCO1"].ToString(), ConfigurationManager.AppSettings["OCO2"].ToString(), ConfigurationManager.AppSettings["OCO3"].ToString(), "OCO", "Orange County Otters");

                Scrape(web, ConfigurationManager.AppSettings["FA1"].ToString(), ConfigurationManager.AppSettings["FA2"].ToString(), ConfigurationManager.AppSettings["FA3"].ToString(), "FA", "Free Agents");

            //Scrape(web, ConfigurationManager.AppSettings["KC1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "KC", "Kansas City Coyotes");
            //Scrape(web, ConfigurationManager.AppSettings["PO1"].ToString(), ConfigurationManager.AppSettings["PO2"].ToString(), ConfigurationManager.AppSettings["PO3"].ToString(), "PO", "Portland Pythons");
            //Scrape(web, ConfigurationManager.AppSettings["SA1"].ToString(), ConfigurationManager.AppSettings["SA2"].ToString(), ConfigurationManager.AppSettings["SA3"].ToString(), "SA", "San Antonio Marshals");
            //Scrape(web, ConfigurationManager.AppSettings["TL1"].ToString(), ConfigurationManager.AppSettings["TL2"].ToString(), ConfigurationManager.AppSettings["TL3"].ToString(), "TL", "Tijuana Luchadores");
            //Scrape(web, ConfigurationManager.AppSettings["PB1"].ToString(), ConfigurationManager.AppSettings["PB2"].ToString(), ConfigurationManager.AppSettings["PB3"].ToString(), "PB", "Palm Beach Solar Bears");
            //Scrape(web, ConfigurationManager.AppSettings["NS1"].ToString(), ConfigurationManager.AppSettings["NS2"].ToString(), ConfigurationManager.AppSettings["NS3"].ToString(), "NS", "Norfolk Seawolves");

            //Scrape(web, ConfigurationManager.AppSettings["QB1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "QB", "Quarterbacks");
            //Scrape(web, ConfigurationManager.AppSettings["RB1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "RB", "Runningbacks");
            //Scrape(web, ConfigurationManager.AppSettings["WR1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "WR", "Wide Receivers");
            //Scrape(web, ConfigurationManager.AppSettings["TE1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "TE", "Tight Ends");
            //Scrape(web, ConfigurationManager.AppSettings["OL1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "OL", "Offensive Line");
            //Scrape(web, ConfigurationManager.AppSettings["DE1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "DE", "Defensive Ends");
            //Scrape(web, ConfigurationManager.AppSettings["DT1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "DT", "Defensive Tackles");
            //Scrape(web, ConfigurationManager.AppSettings["LB1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "LB", "Linebackers");
            //Scrape(web, ConfigurationManager.AppSettings["CB1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "CB", "Cornerbacks");
            //Scrape(web, ConfigurationManager.AppSettings["SF1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "SF", "Safeties");
            //Scrape(web, ConfigurationManager.AppSettings["KP1"].ToString(), ConfigurationManager.AppSettings["KC2"].ToString(), ConfigurationManager.AppSettings["KC3"].ToString(), "KP", "Kickers and Punters");
            
            // store a timestamp in record.json, then clean up
            var dt = DateTime.UtcNow;
                System.IO.StreamWriter dtFile = new System.IO.StreamWriter(ConfigurationManager.AppSettings["LocalPath"] + "record.json");
                dtFile.WriteLine(dt.ToString("ddd, d MMMM yyyy HH:mm:ss zzz"));
                CloseAndDispose(dtFile);

        }

        private static void Scrape(HtmlWeb web,string d1, string d2, string d3, string teamAbrv, string teamName)
        {
            HtmlAgilityPack.HtmlDocument doc = web.Load(d1);
            HtmlAgilityPack.HtmlDocument doc2 = web.Load(d2);
            HtmlAgilityPack.HtmlDocument doc3 = null;
            int pagecount;
            HtmlNodeCollection pagination = doc.DocumentNode.SelectNodes(ConfigurationManager.AppSettings["PaginationPage"]);
            if (pagination == null)
            {
                pagecount = 1;
            }
            else
            {
                string page = pagination[0].InnerText.Split('(')[1];
                pagecount = (Convert.ToInt32(page.Split(')')[0]));
            }

            List<HtmlNode> PlayerNames = GetNodes(doc, ConfigurationManager.AppSettings["PlayerNodes"].ToString());

            PlayerNames.AddRange(GetNodes(doc2, ConfigurationManager.AppSettings["PlayerNodes"].ToString()).ToList());

            if (pagecount == 3)
            {
                doc3 = web.Load(d3);
                PlayerNames.AddRange(GetNodes(doc3, ConfigurationManager.AppSettings["PlayerNodes"].ToString()).ToList());
            }

            PlayerNames.ForEach(x => x.InnerHtml = x.InnerHtml.Replace("&#39;", "'"));
            RemoveRepliesFromPlayerNames(PlayerNames);

            var PlayerTPE = GetNodes(doc, ConfigurationManager.AppSettings["TPENodes"].ToString());
            PlayerTPE.AddRange(GetNodes(doc2, ConfigurationManager.AppSettings["TPENodes"].ToString()).ToList());

            if (pagecount == 3)
            {
                PlayerTPE.AddRange(GetNodes(doc3, ConfigurationManager.AppSettings["TPENodes"].ToString()).ToList());
            }

            foreach(var i in PlayerTPE)
            {
                if(i.InnerHtml.Length==0)
                {
                    i.InnerHtml = "TPE: 50";
                }
            }

            List<string> href1 = new List<string>();
            GetURLs(doc, href1);
            GetURLs(doc2, href1);
            if (pagecount == 3)
            {
                GetURLs(doc3, href1);
            }

            var NameAndTPE = PlayerNames.Zip(PlayerTPE, (n, t) => new { PlayerNames = n, PlayerTPE = t });
            var NameAndTPEAndURL = NameAndTPE.Zip(href1, (x, y) => new { NameAndTPE = x, href1 = y });

            System.IO.StreamWriter file1 = new System.IO.StreamWriter(ConfigurationManager.AppSettings["LocalPath"] + teamAbrv+"Players.txt");
            foreach (var nt in NameAndTPEAndURL)
            {

                // convert other dash types as we go...
                string sName = nt.NameAndTPE.PlayerNames.InnerText;
                sName.Replace("–", "-");

                // construct player line and output to text file
                file1.WriteLine(sName + "," + nt.NameAndTPE.PlayerTPE.InnerText + ", " + nt.href1.ToString() + ",");

            }

            CloseAndDispose(file1);

            string[] SJSTPETotal = System.IO.File.ReadAllLines(ConfigurationManager.AppSettings["LocalPath"] + teamAbrv+"Players.txt");
            List<int> Total = new List<int>();
            foreach (string line in SJSTPETotal)
            {
                string[] splitwords = line.Split(',');
                string[] splitAgain = splitwords[1].Split(':');
                Total.Add(Convert.ToInt32(splitAgain[1]));
            }

            System.IO.StreamWriter file2 = new System.IO.StreamWriter(ConfigurationManager.AppSettings["LocalPath"] + teamAbrv+"Team.txt");

            file2.WriteLine(teamName+"' TPE: " + Total.Sum());
            CloseAndDispose(file2);
        }

        private static void CloseAndDispose(System.IO.StreamWriter file1)
        {
            file1.Close();
            file1.Dispose();
        }

        private static List<HtmlNode> GetNodes(HtmlDocument document, string config)
        {
            return document.DocumentNode
                                .SelectNodes(config.ToString()).ToList();
        }

        private static void GetURLs(HtmlDocument document, List<string> href1)
        {
            int switcher = 1;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes(ConfigurationManager.AppSettings["PlayerURL"]))
            {
                if (switcher == 1)
                {
                    String[] x = node.OuterHtml.Split('"');

                    var playerURL = x[1].Replace("&amp;", "&");
                    playerURL = "http://nsfl.jcink.net/index.php?" + playerURL.Split('&').Last();
                    href1.Add(playerURL.ToString());
                    switcher *= -1;
                }
                else
                {
                    switcher *= -1;
                }
            }
        }

        private static void RemoveRepliesFromPlayerNames(List<HtmlAgilityPack.HtmlNode> PlayerNames)
        {
            int pos = 0;
            for (int i = 0; i < PlayerNames.Count; i += 2, pos++)
            {
                PlayerNames[pos] = PlayerNames[i];
            }
            PlayerNames.RemoveRange(pos, PlayerNames.Count - pos);
        }
    }
}
