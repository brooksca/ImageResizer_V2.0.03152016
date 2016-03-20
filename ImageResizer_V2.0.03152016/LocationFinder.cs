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
        public string FirmID { get; set; }
#if !DEBUG
        public enum Locations { sch, mco, mmm, mpc, rbi, rbt, rcc, rda, rdz, rem, rho, rlp, rpd, rpo, rrs, rsi, rsz, rwm, rwmk, rwp, tcw };
#else
        public enum Locations { abr, all, uss };
#endif

        public LocationFinder()
        {
            ServerName =        Environment.MachineName;
            LocationCode =      ServerName.Substring(0, 3).ToLower();
            try
            {
                FirmID = ((int)Enum.Parse(typeof(Locations), LocationCode)).ToString().PadLeft(2, '0');
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Firm ID set",
                    Time = DateTime.Now,
                    Type = MessageType.success,
                    Details = "Value= " + ServerName
                });
            }
            catch (Exception ex)
            {
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Location retrieval failed",
                    Time = DateTime.Now,
                    Type = MessageType.warning,
                    Details = ex.Message
                });

                GetLocationByHostname();
            }
            ("Running on: " + ServerName).WriteLine();
            ("Location code: " + LocationCode).WriteLine();
            ("Firm ID: " + FirmID).WriteLine();
            "".WriteLine();
        }

        private void GetLocationByHostname()
        {
            string hostname =       Dns.GetHostName();
            LocationCode =          hostname.Substring(0, 3);
            try
            {
                FirmID = ((int)Enum.Parse(typeof(Locations), LocationCode)).ToString().PadLeft(2, '0');
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Firm ID set",
                    Time = DateTime.Now,
                    Type = MessageType.success,
                    Details = "Value= " + hostname
                });
            }
            catch(Exception ex)
            {
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Location retrieval failed",
                    Time = DateTime.Now,
                    Type = MessageType.error,
                    Details = ex.Message
                });
            }
        }
    }
}
