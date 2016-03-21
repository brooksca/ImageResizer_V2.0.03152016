using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace ImageResizer_V2._0._03152016
{
    class HTMLLog
    {
#if DEBUG
        private static string LogDirectory = @"C:\ImageResizer";
#else
        private static string LogDirectory = @"D:\ImageResizer";
#endif
        public static string FullLogPath = Path.Combine(LogDirectory, "ImageResizeLog.html");
        private static DateTime StartTime;
        public static LocationFinder Location { get; set; }
        private static HtmlDocument HtmlLog;
        private static HtmlNode Head;
        private static HtmlNode Body;
        private static HtmlNode Table;
        private static HtmlNode Html;
        private static int Errors;
        public static void BeginLog()
        {
            if (!Directory.Exists(LogDirectory))
                try
                {
                    Directory.CreateDirectory(LogDirectory);
                }
                catch (Exception ex)
                {
                    AddToLog(new LogMessage()
                    {
                        Message = "Could not create log directory",
                        Time = DateTime.Now,
                        Type = MessageType.error,
                        Details = ex.Message
                    });
                    return;
                }
            StartTime = DateTime.Now;
            HtmlLog = null;
            Head = null;
            Body = null;
            BeginHtml();           
        }

        private static void BeginHtml()
        {
            HtmlLog =                       new HtmlDocument();
            Html =                          HtmlNode.CreateNode("<html></html>");
            Head =                          HtmlNode.CreateNode("<head></head>");
            Body =                          HtmlNode.CreateNode("<body></body>");
            Table =                         HtmlNode.CreateNode("<table></table>");
            HtmlNode cssLink = HtmlLog.CreateElement("link");

            Body.AppendChild(HtmlNode.CreateNode("<h2>ImageResizer</h2>"));
            Body.AppendChild(HtmlNode.CreateNode("<h6>Version 2.1.03192016"));
            Body.AppendChild(HtmlNode.CreateNode("<h5>Report from " + DateTime.Now + "</h5>"));
            Body.SetAttributeValue("class", "container-fluid");
            Head.AppendChild(cssLink);
            Table.SetAttributeValue("class", "table table-bordered table-condensed");
            Table.SetAttributeValue("style", "font-size:10px");
            cssLink.SetAttributeValue("rel", "stylesheet");
            cssLink.SetAttributeValue("type", "text/css");
            cssLink.SetAttributeValue("href", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css");

            HtmlNode title =                HtmlNode.CreateNode("<title>ImageResizer Log - " + StartTime + "</title>");
            HtmlNode metaName =             HtmlNode.CreateNode("<meta name = \"viewport\" content=\"width=device-width, initial-scale=1.0, maximum-scaled=1\">");
            HtmlNode metaCompatibility =    HtmlNode.CreateNode("<meta http-equiv=\'X-UA-Compatible\' content=\'IE=edge, chrome=1\'>");
            HtmlNode metaContentType =      HtmlNode.CreateNode("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            HtmlNode metaCopyright =        HtmlNode.CreateNode("<meta name=\"copyright\" content=\"&copy; " + DateTime.Now.Year + " Allen Brooks. All Rights Reserved.\">");
            HtmlNode metaLanguage =         HtmlNode.CreateNode("<meta http-equiv=\"content-language\" content=\"en\">");

            Body.AppendChild(Table);
            Head.AppendChild(title);
            Head.AppendChild(metaName);
            Head.AppendChild(metaCompatibility);
            Head.AppendChild(metaContentType);
            Head.AppendChild(metaCopyright);
            Head.AppendChild(metaLanguage);
            Html.AppendChild(Head);
            Html.AppendChild(Body);
            HtmlLog.DocumentNode.AppendChild(Html);

            CreateTable();
        }

        private static void CreateTable()
        {
            HtmlNode tableHeadRow =         HtmlNode.CreateNode("<tr></tr>");
            HtmlNode tableHeadTime =        HtmlNode.CreateNode("<th>Time</th><th>Type</th><th>Message</th><th>Source</th><th>Destination</th><th>Details</th>");
            HtmlNode tableHeadType =        HtmlNode.CreateNode("<th>Type</th>");
            HtmlNode tableHeadMessage =     HtmlNode.CreateNode("<th>Message</th>");
            HtmlNode tableHeadSource =      HtmlNode.CreateNode("<th>Source</th>");
            HtmlNode tableHeadDestination = HtmlNode.CreateNode("<th>Destination</th>");
            HtmlNode tableHeadDetails =     HtmlNode.CreateNode("<th>Details</th>");

            tableHeadRow.AppendChild(tableHeadTime);
            tableHeadRow.AppendChild(tableHeadType);
            tableHeadRow.AppendChild(tableHeadMessage);
            tableHeadRow.AppendChild(tableHeadSource);
            tableHeadRow.AppendChild(tableHeadDestination);
            tableHeadRow.AppendChild(tableHeadDetails);
            Table.AppendChild(tableHeadRow);
        }

        public static void AddToLog(LogMessage logMessage)
        {
            HtmlNode tableRow = HtmlNode.CreateNode("<tr></tr>");
            HtmlNode timeCell =             HtmlNode.CreateNode("<td>" + logMessage.Time + "</td>");
            HtmlNode typeCell =             HtmlNode.CreateNode("<td>" + logMessage.Type + "</td>");
            HtmlNode messageCell =          HtmlNode.CreateNode("<td>" + logMessage.Message + "</td>");
            HtmlNode sourceCell =           HtmlNode.CreateNode("<td>not availabe</td>");
            HtmlNode destinationCell =      HtmlNode.CreateNode("<td>not available</td>");
            HtmlNode detailsCell =          HtmlNode.CreateNode("<td>" + logMessage.Details??"no details" + "</td>");
            tableRow.AppendChild(timeCell);
            tableRow.AppendChild(typeCell);
            tableRow.AppendChild(messageCell);
            tableRow.AppendChild(sourceCell);
            tableRow.AppendChild(destinationCell);
            tableRow.AppendChild(detailsCell);
            tableRow.SetAttributeValue("class", (logMessage.Type == MessageType.error ? "danger" : logMessage.Type.ToString()));
            Table.AppendChild(tableRow);

        }

        public static void AddToLog(LogMessage logMessage, FileToTransfer file)
        {
            HtmlNode tableRow = HtmlNode.CreateNode("<tr></tr>");
            HtmlNode timeCell =             HtmlNode.CreateNode("<td>" + logMessage.Time + "</td>");
            HtmlNode typeCell =             HtmlNode.CreateNode("<td>" + logMessage.Type + "</td>");
            HtmlNode messageCell =          HtmlNode.CreateNode("<td>" + logMessage.Message + "</td>");
            HtmlNode sourceCell =           HtmlNode.CreateNode("<td></td>");
            HtmlNode sourceLink =           HtmlNode.CreateNode("<a>" + file.SourcePath + "</a>");
            HtmlNode destinationCell =      HtmlNode.CreateNode("<td></td>");
            HtmlNode destinationLink =      HtmlNode.CreateNode("<a>" + file.DestinationPath + "</a>");
            HtmlNode detailsCell =          HtmlNode.CreateNode("<td>" + logMessage.Details??"no details" + "</td>");
            sourceLink.SetAttributeValue("href", "file:///" + file.SourcePath);
            destinationLink.SetAttributeValue("href", "file:///" + file.DestinationPath);
            sourceCell.AppendChild(sourceLink);
            destinationCell.AppendChild(destinationLink);
            tableRow.AppendChild(timeCell);
            tableRow.AppendChild(typeCell);
            tableRow.AppendChild(messageCell);
            tableRow.AppendChild(sourceCell);
            tableRow.AppendChild(destinationCell);
            tableRow.AppendChild(detailsCell);
            tableRow.SetAttributeValue("class", (logMessage.Type == MessageType.error ? "danger" : logMessage.Type.ToString()));
            Table.AppendChild(tableRow);
        }

        public static void GenerateReportPanel(int errors, int skipped, int success)
        {
            Errors = errors;
            HtmlNode resultsPanel = HtmlNode.CreateNode("<div></div>");
            resultsPanel.SetAttributeValue("class", errors == 0 ? "panel panel-success" : "panel panel-danger");

            HtmlNode panelHead = HtmlNode.CreateNode("<div></div>");
            panelHead.SetAttributeValue("class", "panel-heading");

            HtmlNode panelIcon = HtmlNode.CreateNode("<span></span>");
            panelIcon.SetAttributeValue("class", "glyphicon " + (errors == 0 ? "glyphicon-ok" : "glyphicon-remove"));

            HtmlNode panelTitle = HtmlNode.CreateNode("<h3></h3>");
            panelTitle.SetAttributeValue("class", "panel-title");
            panelTitle.InnerHtml = errors == 0 ? " Success" : " Fail";
            panelTitle.PrependChild(panelIcon);

            HtmlNode panelBody = HtmlNode.CreateNode("<div></div>");
            panelBody.SetAttributeValue("class", "panel-body");
            TimeSpan processingTime = DateTime.Now.Subtract(StartTime);
            panelBody.AppendChild(HtmlNode.CreateNode("<p>Server name: " + Location.ServerName.ToLower() + "</p>"));
            panelBody.AppendChild(HtmlNode.CreateNode("<p>Firm ID: " + Location.FirmID + "</p>"));
            panelBody.AppendChild(HtmlNode.CreateNode("<p>Successful " + success + " | Skipped " + skipped + " | Errors " + errors + " | Processing time " + processingTime.ToString(@"hh\:mm\:ss\.ff") + "</p>"));

            panelHead.AppendChild(panelTitle);
            resultsPanel.AppendChild(panelHead);
            resultsPanel.AppendChild(panelBody);
            Body.InsertBefore(resultsPanel, Table);
        }

        public static void SaveAndClose()
        {
            HtmlNode bootstrapJavascript = HtmlLog.CreateElement("script");
            bootstrapJavascript.SetAttributeValue("type", "text/javascript");
            bootstrapJavascript.SetAttributeValue("href", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js");
            StreamWriter writer = new StreamWriter(FullLogPath);
            HtmlLog.Save(writer);
            if (!EventLog.SourceExists("Image Resizer"))
                EventLog.CreateEventSource("Image Resizer", "Application");
            string message = "Image Resizer completed " + (Errors == 0 ? "successfully" : "with errors") + " at " + DateTime.Now
                + Environment.NewLine + "Log located at " + FullLogPath;
            EventLog.WriteEntry("Image Resizer", message, (Errors == 0 ? EventLogEntryType.Information : EventLogEntryType.Error));
        }
    }
}
