using System;
using System.Globalization;
using System.Web.Mvc;

namespace Passengers.Helper
{
    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            //var result = value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
            //return result;
            
            return ParseDate(value.AttemptedValue);
        }

        public DateTime ParseDate(string dateText)
        {
            DateTime date;
            CultureInfo arSy = new CultureInfo("ar-SY");

            if (DateTime.TryParseExact(dateText, "yyyy/MM/dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy/M/dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy/MM/d", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy/M/d", arSy, DateTimeStyles.None, out date) ||

                DateTime.TryParseExact(dateText, "dd/MM/yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "dd/M/yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d/MM/yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d/M/yyyy", arSy, DateTimeStyles.None, out date) ||

                DateTime.TryParseExact(dateText, "yyyy-MM-dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy-M-dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy-MM-d", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy-M-d", arSy, DateTimeStyles.None, out date) ||

                DateTime.TryParseExact(dateText, "dd-MM-yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "dd-M-yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d-MM-yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d-M-yyyy", arSy, DateTimeStyles.None, out date)
                )
            {
            }
            return date;
        }
    }

    public class NullableDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            var dateText = value.AttemptedValue;

            try
            {
                var result = value == null
                ? null
                : ParseDate(dateText);// value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
                return result;
            }
            catch (Exception)
            {
                var result = value == null ? null : ParseDate(dateText);
                return result;
            }
        }


        public DateTime? ParseDate(string dateText)
        {
            DateTime date;
            CultureInfo arSy = new CultureInfo("ar-SY");

            if (DateTime.TryParseExact(dateText, "yyyy/MM/dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy/M/dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy/MM/d", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy/M/d", arSy, DateTimeStyles.None, out date) ||

                DateTime.TryParseExact(dateText, "dd/MM/yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "dd/M/yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d/MM/yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d/M/yyyy", arSy, DateTimeStyles.None, out date) ||

                DateTime.TryParseExact(dateText, "yyyy-MM-dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy-M-dd", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy-MM-d", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "yyyy-M-d", arSy, DateTimeStyles.None, out date) ||

                DateTime.TryParseExact(dateText, "dd-MM-yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "dd-M-yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d-MM-yyyy", arSy, DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(dateText, "d-M-yyyy", arSy, DateTimeStyles.None, out date)
                )
            {
                return date;
            }
            else
            {
                return null;
            }
        }
    }
}