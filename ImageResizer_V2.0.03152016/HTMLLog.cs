using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
        public static TimeSpan StartTime;
        private static HtmlDocument HtmlLog;
        private static HtmlNode Head;
        private static HtmlNode Body;
        private static HtmlNode Table;
        public static void BeginLog()
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            StartTime = DateTime.Now.TimeOfDay;
            BeginHtml();           
        }

        private static void BeginHtml()
        {
            HtmlLog =                       new HtmlDocument();
            HtmlNode html =                 HtmlNode.CreateNode("<html><head></head><body></body></html>");
            Head =                          html.SelectSingleNode("//head");
            Body =                          html.SelectSingleNode("//body");

            HtmlNode title =                HtmlNode.CreateNode("<title>ImageResizer Log - " + StartTime + "</title>");
            HtmlNode metaName =             HtmlNode.CreateNode("<meta name = \"viewport\" content=\"width=device-width, initial-scale=1.0, maximum-scaled=1\">");
            HtmlNode metaCompatibility =    HtmlNode.CreateNode("<meta http-equiv=\'X-UA-Compatible\' content=\'IE=edge, chrome=1\'>");
            HtmlNode metaContentType =      HtmlNode.CreateNode("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            HtmlNode metaCopyright =        HtmlNode.CreateNode("<meta name=\"copyright\" content=\"&copy; " + DateTime.Now.Year + " Allen Brooks. All Rights Reserved.\">");
            HtmlNode metaLanguage =         HtmlNode.CreateNode("<meta http-equiv=\"content-language\" content=\"en\">");

            HtmlLog.DocumentNode.AppendChild(html);
            Head.AppendChild(title);
            Head.AppendChild(metaName);
            Head.AppendChild(metaCompatibility);
            Head.AppendChild(metaContentType);
            Head.AppendChild(metaCopyright);
            Head.AppendChild(metaLanguage);

            CreateTable();
        }

        private static void CreateTable()
        {
            Table =                         HtmlNode.CreateNode("<table border=\"1\" cellpadding=\"5\" cellspacing=\"5\"></table>");
            HtmlNode tableHeadRow =         HtmlNode.CreateNode("<tr></tr>");
            HtmlNode tableHeadTime =        HtmlNode.CreateNode("<th>Time</th><th>Type</th><th>Message</th><th>Task</th><th>Source</th><th>Destination</th><th>Details</th>");
            HtmlNode tableHeadType =        HtmlNode.CreateNode("<th>Type</th>");
            HtmlNode tableHeadMessage =     HtmlNode.CreateNode("<th>Message</th>");
            HtmlNode tableHeadTask =        HtmlNode.CreateNode("<th>Task</th>");
            HtmlNode tableHeadSource =      HtmlNode.CreateNode("<th>Source</th>");
            HtmlNode tableHeadDestination = HtmlNode.CreateNode("<th>Destination</th>");
            HtmlNode tableHeadDetails =     HtmlNode.CreateNode("<th>Details</th>");

            tableHeadRow.AppendChild(tableHeadTime);
            tableHeadRow.AppendChild(tableHeadType);
            tableHeadRow.AppendChild(tableHeadMessage);
            tableHeadRow.AppendChild(tableHeadTask);
            tableHeadRow.AppendChild(tableHeadSource);
            tableHeadRow.AppendChild(tableHeadDestination);
            tableHeadRow.AppendChild(tableHeadDetails);
            Table.AppendChild(tableHeadRow);
        }

        public static void AddToLog(LogMessage logMessage, FileToTransfer file)
        {
            HtmlNode tableRow = HtmlNode.CreateNode("<tr></tr>");

            HtmlNode timeCell =             HtmlNode.CreateNode("<td>" + logMessage.Time + "</td>");
            HtmlNode typeCell =             HtmlNode.CreateNode("<td>" + logMessage.Type + "</td>");
            HtmlNode messageCell =          HtmlNode.CreateNode("<td>" + logMessage.Message + "</td>");
            HtmlNode taskCell =             HtmlNode.CreateNode("<td>" + logMessage.Task + "</td>");
            HtmlNode sourceCell =           HtmlNode.CreateNode("<td>" + file.SourcePath??"not available" + "</td>");
            HtmlNode destinationCell =      HtmlNode.CreateNode("<td>" + file.DestinationPath??"not available" + "</td>");
            HtmlNode detailsCell =          HtmlNode.CreateNode("<td>" + logMessage.Message??"not available" + "</td>");

            tableRow.AppendChild(timeCell);
            tableRow.AppendChild(typeCell);
            tableRow.AppendChild(messageCell);
            tableRow.AppendChild(taskCell);
            tableRow.AppendChild(sourceCell);
            tableRow.AppendChild(destinationCell);
            tableRow.AppendChild(detailsCell);

            Table.AppendChild(tableRow);
        }

        public static void SaveLog()
        {
            Body.AppendChild(Table);
            StreamWriter writer = new StreamWriter(FullLogPath);
            HtmlLog.Save(writer);
        }
    }
}
