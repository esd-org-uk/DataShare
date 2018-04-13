using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DS.WebUI.Controllers.Extensions
{
    public static class RegExExtensions
    {
        private static Regex _regWhiteSpace = new Regex(@"\s");

        public static string RemoveWhiteSpaces(this String str)
        {
         
            return str != null ? _regWhiteSpace.Replace(str, " ") : "";
        }
    }
}