using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Shared;

namespace SpiritTime.Frontend.Infrastructure.ElectronConfig
{
    public static class ElectronHelper
    {
        public static string GetDownloadInfo(ProgressInfo info)
        {
            var separator = "  /  ";
            var message1  = "Download speed: " + ConvertStringToByte(info.BytesPerSecond) + "/s" + separator;
            var message2  = "Downloaded " + ConvertPercentage(info.Percent) + "%"                + separator;
            var message3  = ConvertStringToByte(info.Transferred) + "/"                          + ConvertStringToByte(info.Total);

            return message1 + message2 + message3;
        }

        public static string ConvertPercentage(string percentage)
        {
            if (!percentage.Contains(".")) return percentage;
            var indexOfPercent = percentage.IndexOf('.');
            if (indexOfPercent != 0)
            {
                return percentage.Length - indexOfPercent > 2 ? percentage.Substring(0, indexOfPercent) + "." + percentage.Substring(indexOfPercent + 1, 2) : percentage;
            }

            return percentage;
        }

        public static string ConvertStringToByte(string input)
        {
            string output;
            if (input.Length > 9)
            {
                output = input.Substring(0, input.Length - 9) + "." + input.Substring(input.Length - 9, 2) + " GB";
            }
            else if (input.Length > 6)
            {
                output = input.Substring(0, input.Length - 6) + "." + input.Substring(input.Length - 6, 2) + " MB";
            }
            else if (input.Length > 3)
            {
                output = input.Substring(0, input.Length - 3) + "." + input.Substring(input.Length - 3, 2) + " KB";
            }
            else
            {
                output = input + " Byte";
            }

            return output;
        }
    }
}