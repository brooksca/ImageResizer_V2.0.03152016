using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace ImageResizer_V2._0._03152016
{
    class LocationFinder
    {
        public string ServerName { get; set; }
        public string LocationCode { get; set; }
        public int FirmID { get; set; }
        private const int ERROR_INVALID_LOCATION = 0xA0;

        public enum Locations { sch, mco, mmm, mpc, rbi, rbt, rcc, rda, rdz, rem, rho, rlp, rpd, rpo, rrs, rsi, rsz, rwm, rwmk, rwp, tcw, uss };

        public LocationFinder()
        {
            ServerName = Environment.MachineName;
            LocationCode = ServerName.Substring(0, 3).ToLower();
            try
            {
                FirmID = (int)Enum.Parse(typeof(Locations), LocationCode);
            }
            catch (Exception ex)
            {
                Log.WriteToLog("Location retrieval by machine name failed: " + ex.Message);
                Log.WriteToEventLog("Location Retrieval Attempt Failed", EventLogEntryType.Warning);
                GetLocationByHostname();
            }
            ("Running on: " + ServerName).WriteLine();
            ("Location code: " + LocationCode).WriteLine();
            ("Firm ID: " + FirmID).WriteLine();
            "".WriteLine();
        }

        private void GetLocationByHostname()
        {
            Log.WriteToLog("Attempting location retrieval by hostname.");
            string hostname = Dns.GetHostName();
            Log.WriteToLog("Host name set to: " + hostname);
            LocationCode = hostname.Substring(0, 3);
            try
            {
                FirmID = (int)Enum.Parse(typeof(Locations), LocationCode);
            }
            catch(Exception ex)
            {
                Log.WriteToLog("Fatal Error: Location retrieval by host name failed. " + ex.Message);
                Log.WriteToEventLog("Location Retrieval Attempt Failed", EventLogEntryType.Error);
                Environment.Exit(ERROR_INVALID_LOCATION);
            }
        }
    }
}
