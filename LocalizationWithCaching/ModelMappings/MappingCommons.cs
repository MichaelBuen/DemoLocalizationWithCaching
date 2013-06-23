using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalizationWithCaching.ModelMappings
{
    public static class MappingCommons
    {
        public const string Context = "convert(nvarchar, substring(context_info(), 5, convert(int, substring(context_info(), 1, 4)) )  )";
    }
}