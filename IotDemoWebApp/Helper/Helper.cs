using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IotDemoWebApp.Helper
{
    public static class Helper<T> where T:class
    {
        //public static IList<IList<T>> Split<T>(IList<T> source)
        //{
        //    return source
        //        .Select((x, i) => new { Index = i, Value = x })
        //        .GroupBy(x => x.Index / 3)
        //        .Select(x => x.Select(v => v.Value).ToList())
        //        .ToList();
        //}
    }
}