using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Platform.Services
{
    public static class TypeBroker
    {
        //private static IResponseFormatter formatter = new TextResponseFormatter();
        // replacing the implementation in the broker class
        private static IResponseFormatter formatter = new HtmlResponseFormatter();

        public static IResponseFormatter Formatter => formatter;
    }
}
