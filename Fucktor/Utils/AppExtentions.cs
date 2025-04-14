using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Linq.Expressions;

namespace Fucktor.Utils
{
    public static class AppExtentions
    {
        public static string NumberInLetters(this decimal number)
        {
            string[] yekans = { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
            string[] dahgan10 = { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
            string[] dahgans = { "", "ده", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
            string[] sadgans = { "", "صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
            string[] sep = { "", "هزار", "ملیون", "میلیارد", "تریلیارد", "هزار تریلیارد" };

            var parts = new List<int>();
            do
            {
                parts.Add((int)number % 1000);
                number /= 1000;
            } while (number >= 1);

            if (parts.Count == 1 && parts[0] == 0)
            {
                return yekans[0];
            }
            var numbersInLetters = new List<string>();
            for (var i = 0; i < parts.Count; i++)
            {
                var number3 = "";
                var yekan = parts[i] % 10;
                parts[i] /= 10;
                var dahgan = parts[i] % 10;
                parts[i] /= 10;
                var sadgan = parts[i] % 10;
                parts[i] /= 10;

                if (sadgan != 0)
                {
                    number3 += sadgans[sadgan];
                    if (dahgan != 0 || yekan != 0)
                    {
                        number3 += " و ";
                    }
                }
                if (dahgan == 1)
                {
                    number3 += dahgan10[yekan];
                }
                else
                {
                    if (dahgan != 0)
                    {
                        number3 += dahgans[dahgan];
                        if (yekan != 0)
                        {
                            number3 += " و ";
                        }
                    }
                    if (yekan != 0)
                    {
                        number3 += yekans[yekan];
                    }
                }
                numbersInLetters.Add(number3);
            }

            var numberInLetters = "";
            for (var i = numbersInLetters.Count - 1; i >= 0; i--)
            {
                if (numberInLetters.Length > 0 && !string.IsNullOrEmpty(numbersInLetters[i]))
                {
                    numberInLetters += " و ";
                }
                numberInLetters += numbersInLetters[i];
                if (!string.IsNullOrEmpty(numbersInLetters[i]))
                {
                    numberInLetters += " " + sep[i];
                }
            }
            return numberInLetters;
        }

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
