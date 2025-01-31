using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Linq.Expressions;

namespace Fucktor.Utils
{
    public static class AppExtentions
    {
        public static List<object> ToList(this Array array)
        {
            var list = new List<object>();
            foreach (var item in array)
            {
                list.Add(item);
            }
            return list;
        }
        public static string RemoveController(this string controllerName)
        {
            return controllerName.Replace("Controller", "");
        }
        public static string ToInputRecognizableDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.000", CultureInfo.InvariantCulture);
        }

        public static string ToPersianDateString(this DateTime dateTime,string format)
        {
            return dateTime.ToString(format, new CultureInfo("fa-IR"));
        }

        public static string ToLocalizedString<T>(this T obj, IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create(obj.GetType());
            return localizer[obj.ToString()].Value;
        }

        public static string DisplayNameFor<TModel, TResult>(this TModel obj, Expression<Func<TModel, TResult>> exp, IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create(obj.GetType());
            var memEx = (MemberExpression)exp.Body;
            if (memEx.Expression.NodeType == ExpressionType.Call)
            {
                localizer = localizerFactory.Create(memEx.Expression.Type);
            }
            return localizer[((MemberExpression)exp.Body).Member.Name].Value;
        }
    }
}
