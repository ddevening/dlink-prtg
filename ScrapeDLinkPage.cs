using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using HtmlAgilityPack;
using WatiN.Core;

namespace dlink_prtg
{
    public class ScrapeDLinkPage
    {
        string m_strALL_SENSOR_NR;
    ﻿  ﻿  string m_strALL_SENSOR_VAL;
    ﻿  ﻿  string m_strRetVal;﻿  
        string errorMsg;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeDLinkPage"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        public ScrapeDLinkPage(string[] args)
        {
            var dataResults = LoginPage(args);

            string xmlResults = BuildXmlResults(dataResults);

            Console.WriteLine(xmlResults);
            if (xmlResults.Contains("error"))
            {
                setExitCode(2);
            }
            else
            {
                setExitCode(0);
            }

            return;
        }

        /// <summary>
        /// Builds the XML results.
        /// </summary>
        /// <param name="dataResults">The data results.</param>
        /// <returns></returns>
        private string BuildXmlResults(ResultDTO dataResults)
        {
            string buildxml = "";
            XmlDocument doc = new XmlDocument();
            XmlNode prtg = doc.CreateElement("prtg");
            XmlNode result;

            if (dataResults.Error.Length == 0)
            {
                result = doc.CreateElement("result");
                result.AppendChild(GetNodeItemChannel(doc, "channel", "DLink - Temperature"));
                result.AppendChild(GetNodeItemChannel(doc, "unit", "Temperature"));
                result.AppendChild(GetNodeItemChannel(doc, "mode", "Absolute"));
                result.AppendChild(GetNodeItemChannel(doc, "showChart", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "showTable", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "warning", "140"));
                result.AppendChild(GetNodeItemChannel(doc, "float", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "value", dataResults.Temp.ToString()));
                prtg.AppendChild(result);

                result = doc.CreateElement("result");
                result.AppendChild(GetNodeItemChannel(doc, "channel", "Disk Space - Total"));
                result.AppendChild(GetNodeItemChannel(doc, "unit", "Custom"));
                result.AppendChild(GetNodeItemChannel(doc, "customUnit", "GB"));
                //result.AppendChild(GetNodeItemChannel(doc, "unit", "BytesDisk"));
                //result.AppendChild(GetNodeItemChannel(doc, "volumesize", "GigaByte"));
                result.AppendChild(GetNodeItemChannel(doc, "mode", "Absolute"));
                result.AppendChild(GetNodeItemChannel(doc, "showChart", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "showTable", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "warning", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "float", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "value", dataResults.TotalDiskSpace.ToString()));
                prtg.AppendChild(result);

                result = doc.CreateElement("result");
                result.AppendChild(GetNodeItemChannel(doc, "channel", "Disk Space - Used"));
                result.AppendChild(GetNodeItemChannel(doc, "unit", "Custom"));
                result.AppendChild(GetNodeItemChannel(doc, "customUnit", "GB"));
                //result.AppendChild(GetNodeItemChannel(doc, "unit", "BytesDisk"));
                //result.AppendChild(GetNodeItemChannel(doc, "volumesize", "GigaByte"));
                result.AppendChild(GetNodeItemChannel(doc, "mode", "Absolute"));
                result.AppendChild(GetNodeItemChannel(doc, "showChart", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "showTable", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "warning", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "float", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "value", dataResults.UsedDiskSpace.ToString()));
                prtg.AppendChild(result);

                result = doc.CreateElement("result");
                result.AppendChild(GetNodeItemChannel(doc, "channel", "Disk Space - UnUsed"));
                result.AppendChild(GetNodeItemChannel(doc, "unit", "Custom"));
                result.AppendChild(GetNodeItemChannel(doc, "customUnit", "GB"));
                //result.AppendChild(GetNodeItemChannel(doc, "unit", "BytesDisk"));
                //result.AppendChild(GetNodeItemChannel(doc, "volumesize", "GigaByte"));
                result.AppendChild(GetNodeItemChannel(doc, "mode", "Absolute"));
                result.AppendChild(GetNodeItemChannel(doc, "showChart", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "showTable", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "warning", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "float", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "value", dataResults.UnUsedDiskSpace.ToString()));
                prtg.AppendChild(result);

                result = doc.CreateElement("result");
                result.AppendChild(GetNodeItemChannel(doc, "channel", "Disk Space - Percent Used"));
                result.AppendChild(GetNodeItemChannel(doc, "unit", "Percent"));
                result.AppendChild(GetNodeItemChannel(doc, "mode", "Absolute"));
                result.AppendChild(GetNodeItemChannel(doc, "showChart", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "showTable", "1"));
                result.AppendChild(GetNodeItemChannel(doc, "warning", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "float", "0"));
                result.AppendChild(GetNodeItemChannel(doc, "value", dataResults.PercentUsedDiskSpace.ToString()));
                prtg.AppendChild(result);

                result = doc.CreateElement("text");
                result.InnerText = "Dlink Sensor";
                prtg.AppendChild(result);  
            }
            else
            {
                result = doc.CreateElement("error");
                result.InnerText = "1";
                prtg.AppendChild(result);  

                result = doc.CreateElement("text");
                result.InnerText = dataResults.Error;
                prtg.AppendChild(result);  
            }

            doc.AppendChild(prtg);
            return doc.OuterXml;
        }

        /// <summary>
        /// Gets the node item channel.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="nodename">The nodename.</param>
        /// <param name="nodevalue">The nodevalue.</param>
        /// <returns></returns>
        private static XmlNode GetNodeItemChannel(XmlDocument doc, string nodename, string nodevalue)
        {
            XmlNode channel = doc.CreateElement(nodename);
            channel.InnerText = nodevalue;
            return channel;
        }

        private void WatinLogin(string[] args)
        {
            try
            {
                using (IE browser = new IE("http://10.10.1.37"))
                {
                    browser.GoTo("http://10.10.1.37");
                    browser.TextField(Find.ByName("f_LOGIN_NAME")).Click();
                    browser.TextField(Find.ByName("f_LOGIN_NAME")).TypeText("admin");
                    browser.Button(Find.ByName("Config_Button")).Click();
                    //Thread.Sleep(1000);
                    browser.Link(Find.ByUrl("http://10.10.1.37/goform/adv_status")).Click();
                    //Thread.Sleep(1000);
                    //var tot = Find.ByText("HARD DRIVE INFO : ");
                    //ie.TableCell(Find.ByCustom("innertext", "Total Hard Drive Capacity:")).Click();
                    //ie.TableCell(Find.ByCustom("innertext", " 5900643 MB")).Click();
                    //ie.TableCell(Find.ByCustom("innertext", "Used Space:")).Click();
                    //ie.TableCell(Find.ByCustom("innertext", " 1677521 MB")).Click();
                    //ie.TableCell(Find.ByCustom("innertext", "Unused Space:")).Click();
                    //ie.TableCell(Find.ByCustom("innertext", " 4223121 MB")).Click();
                    //var t = Find.ByClass("AutoNumber1");
                    var tables = browser.TableRows;
                    foreach (var item in tables)
                    {
                        var tt11 = item.ToString();
                        if (tt11 != null)
                        {
                            if (tt11.Contains("HARD DRIVE INFO :"))
                            {
                                var driveSize = "";
                                break;
                            }
                        }
                    }
                    browser.AutoClose = true;
                }
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                
            }
        }

        /// <summary>
        /// Logins the page.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        private ResultDTO LoginPage(string[] args)
        {
            Uri address;
            int iTotalSpace = 0;
            int iUsedSpace = 0;
            int iUnUsedSpace = 0;
            int iTemp = 0;

            ResultDTO results = new ResultDTO
            {
                UsedDiskSpace = 0,
                UnUsedDiskSpace = 0,
                PercentUsedDiskSpace = 0,
                Temp = 0,
                TotalDiskSpace = 0,
                Error = ""
            };
            
            try
            {
                string strUrl = @"http://10.10.1.37/goform/formLogin";

                WebPostRequest myPost = new WebPostRequest(new Uri(strUrl));
                myPost.Add("f_LOGIN_NAME", "admin");
                myPost.Add("f_LOGIN_PASSWD", "");
                myPost.Add("f_login_type", "0");
                myPost.Add("f_url", "");                

                string webpage = myPost.GetResponse();

                if (webpage.ToLower().Contains("wizard"))
                {
                    //-- Need to test for valid login page
                    //-- before continuing on.

                    //-- now goto the http://10.10.1.37/goform/adv_status
                    WebPostRequest myPost2 = new WebPostRequest(new Uri("http://10.10.1.37/goform/adv_status"));

                    string webpage2 = myPost2.GetResponse();

                    if (webpage2.ToLower().Contains("temperture"))
                    {
                        HtmlDocument doc = new HtmlDocument();

                        //-- Removes all html tags
                        //webpage2 = Regex.Replace(webpage2, @"<(.|\n)*?>", String.Empty);
                        //webpage2 = Regex.Replace(webpage2, @"^\n*", String.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        //webpage2 = Regex.Replace(webpage2, @"\n*$", String.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        //webpage2 = webpage2.Replace("\n", " ");

                        doc.LoadHtml(webpage2);

                        // There are various options, set as needed
                        //doc.OptionFixNestedTags = true;
                        //doc.OptionAutoCloseOnEnd = true;
                        //doc.OptionCheckSyntax = true;

                        results.Temp = GetTemp(webpage2);

                        results.TotalDiskSpace = GetTotalSpace(webpage2);
                        results.TotalDiskSpace = results.TotalDiskSpace / 1024;

                        results.UsedDiskSpace  = GetUsedSpace(webpage2);
                        results.UsedDiskSpace = results.UsedDiskSpace / 1024;

                        results.UnUsedDiskSpace = GetUnUsedSpace(webpage2);
                        results.UnUsedDiskSpace = results.UnUsedDiskSpace / 1024;

                        if(results.TotalDiskSpace > 0)
                        {
                            var percentUsed = (Convert.ToDouble(results.UsedDiskSpace) / Convert.ToDouble(results.TotalDiskSpace)) * 100;
                            results.PercentUsedDiskSpace = Convert.ToInt32(percentUsed);
                        }
                    }
                    //-- logout
                    PageLogout(args);
                }
                else if (webpage.ToLower().Contains("another party"))
                {
                    results.Error = "Web page in use.";
                }
                else
                {
                    results.Error = "Unknow Error.";
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                results.Error = ex.Message;
            }
            finally
            {
                
            }
            return results;
        }

        /// <summary>
        /// Pages the logout.
        /// </summary>
        /// <param name="args">The args.</param>
        private void PageLogout(string[] args)
        {
            try
            {
                WebPostRequest myPost = new WebPostRequest(new Uri("http://10.10.1.37/goform/formLogout"));
                string webpage = myPost.GetResponse();
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                
            }
        }

        /// <summary>
        /// Gets the un used space.
        /// </summary>
        /// <param name="webpage2">The webpage2.</param>
        /// <returns></returns>
        private static int GetUnUsedSpace(string webpage2)
        {
            return GetDataFromKey(webpage2, "Unused Space:");
        }

        /// <summary>
        /// Gets the used space.
        /// </summary>
        /// <param name="webpage2">The webpage2.</param>
        /// <returns></returns>
        private static int GetUsedSpace(string webpage2)
        {
            return GetDataFromKey(webpage2, "Used Space:");

        }

        /// <summary>
        /// Gets the total space.
        /// </summary>
        /// <param name="webpage2">The webpage2.</param>
        /// <returns></returns>
        private static int GetTotalSpace(string webpage2)
        {
            return GetDataFromKey(webpage2, "Total Hard Drive Capacity:");
        }


        /// <summary>
        /// Gets the data from key.
        /// </summary>
        /// <param name="webpage2">The webpage2.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static int GetDataFromKey(string webpage2, string key)
        {
            int iResult = 0;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(webpage2);
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
            {
                var s1 = "Found: " + table.Id;
                var b1 = table.InnerText;
                var h1 = table.InnerHtml;
                if (b1.Contains("HARD DRIVE INFO :"))
                {
                    var ss11 = b1;

                    string[] lines = b1.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    for (int i = 0; i < lines.Count(); i++)
                    {
                        if (lines[i].Contains(key))
                        {
                            var totalspace = lines[i + 1];
                            totalspace = totalspace.Replace("&nbsp;", "");
                            totalspace = totalspace.Replace(" ", "");
                            totalspace = totalspace.Replace("MB", "");
                            iResult = Convert.ToInt32(totalspace);
                        }
                    }
                }
            }
            return iResult;
        }
        private int GetTemp(string html)
        {
            int temp = 0;
            if(html.ToLower().Contains("var temper="))
            {
                string[] lines = html.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                var myList = lines.ToList();
                var item = myList.Find(p => p.Contains("var temper="));
                if( item != null)
                {
                    string t = item.Replace("var temper=", "");
                    t = t.Replace("\"", "");
                    t = t.Replace("\t", "");
                    var split = t.Split(':');
                    if(split.Length == 2)
                    {
                        temp = Convert.ToInt32(split[0]);
                    }
                }        
            }
            return temp;
        }
        ﻿
        /// <summary>
        /// Reads the val.
        /// </summary>
        public void readVal()
        {
            try
            {
                //string strUrl = String.Format("http://{0}/xml", m_strALL_IP);
                string strUrl = @"http://10.10.1.37";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                //request.Credentials = New System.Net.NetworkCredential( strUser, strPassword);
                request.Method = WebRequestMethods.Http.Get;


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);
                string strResult = sr.ReadToEnd();
                strResult = strResult.Substring(strResult.IndexOf("<xml>"), strResult.IndexOf("</xml>") - strResult.IndexOf("<xml>") + 6);
                strResult.Replace("\n", "");
                strResult.Replace("\r", "");
                response.Close();

                //                string strResult = "<xml><data><devicename>MySensoren</devicename><n0>0</n0><t0> 32.15</t0><min0> 28.69</min0><max0> 32.60</max0><l0>0</l0><h0>100</h0><s0>65</s0><n1>1</n1><t1> 21.05</t1><min1> 20.18</min1><max1> 23.73</max1><l1>-40</l1><h1>40</h1><s1>3</s1><n2>2</n2><t2> 1020.88</t2><min2> 955.43</min2><max2>-20480.00</max2><l2>850</l2><h2>1035</h2><s2>40</s2><n3>3</n3><t3>-20480.00</t3><min3> 20480.00</min3><max3>-20480.00</max3><l3>-55</l3><h3>150</h3><s3>0</s3><n4>4</n4><t4>-20480.00</t4><min4> 20480.00</min4><max4>-20480.00</max4><l4>-55</l4><h4>150</h4><s4>0</s4><n5>5</n5><t5>-20480.00</t5><min5> 20480.00</min5><max5>-20480.00</max5><l5>-55</l5><h5>150</h5><s5>0</s5><n6>6</n6><t6>-20480.00</t6><min6> 20480.00</min6><max6>-20480.00</max6><l6>-55</l6><h6>150</h6><s6>0</s6><n7>7</n7><t7> 21.68</t7><min7> 19.87</min7><max7> 23.50</max7><l7>-40</l7><h7>40</h7><s7>2</s7><date>08.04.2006</date><time>08:20:32</time><ad>1</ad><i>60</i><f>0</f><sys>28409</sys><mem>31656</mem><fw>2.59</fw><dev>ALL4000</dev></data></xml>";               
                /*
                 * <n0>0</n0>
                 * <t0> 32.15</t0>
                 * <min0> 28.69</min0>
                 * <max0> 32.60</max0>
                 * <l0>0</l0>
                 * <h0>100</h0>
                 * <s0>65</s0>
                 */

                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                xml.LoadXml(strResult);

                string strTag = "";
                switch (m_strALL_SENSOR_VAL)
                {
                    case "MAX":
                        strTag = "max";
                        break;
                    case "MIN":
                        strTag = "min";
                        break;
                    case "CURRENT":
                        strTag = "t";
                        break;
                }
                m_strRetVal = xml.GetElementsByTagName(strTag + m_strALL_SENSOR_NR)[0].InnerText + ":" + xml.GetElementsByTagName("n" + m_strALL_SENSOR_NR)[0].InnerText;
                setExitCode(0);
            }
            catch (Exception ex)
            {
                m_strRetVal = "-3: Exception:" + ex.Message;
                setExitCode(2);
            }
        }
        /// <summary>
        /// Sets the exit code.
        /// </summary>
        /// <param name="nExitCode">The n exit code.</param>
        ﻿private static void setExitCode(int nExitCode)
        {
        ﻿  ﻿  ﻿  Environment.ExitCode = nExitCode;
        }
    }
}
