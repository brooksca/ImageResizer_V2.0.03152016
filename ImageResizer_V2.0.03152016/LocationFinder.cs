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
        public enum Locations { rcc=1, rbi=2, rsz=3, rpd=4, rlp=5, rwp=6, rsi=7, rho=8, rdz=9, rrs=10, rpo=11, sch=12, rem=15, tcw=16, mpc=17, mco=18, mmm=19, mcr=20, rbt=22, rda=26, rwm=23 };
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
