 using Proced.DataAccess.Models.CF;
using ProcedBase;
using System;
using System.Drawing;
using System.IO;
using ZXing;

namespace Passengers.Helper
{

    public static class Constants
    {
        public static string[] AdminUserNames = new string[] { "Admin", "دعم فني" };
        public const string GlobalDateFormat = "yyyy/MM/dd";
        public const string GlobalDateTimeFormat = "yyyy/MM/dd HH:mm:ss";
        public const string MazoutFuelType = "مازوت";

    }
    public sealed class ZPRICES
    {
        private ZPRICES() { }
        /// <summary>
        /// القيمة الأساسية
        /// </summary>
        public const int BaseValue = 1;

        /// <summary>
        /// الرسوم الجمركية
        /// </summary>
        public const int CustomTaxes = 2;

        /// <summary>
        /// عمولة أفتوماشين
        /// </summary>
        public const int OfftoMachineTaxes = 3;

        /// <summary>
        /// فارق عمولة أفتوماشين
        /// </summary>
        public const int OfftoMachineDiffTaxes = 4;


        /// <summary>
        /// قيمة قطع مستوردة
        /// </summary>
        public const int ImportedParts = 17;
        public const int RaFahia = 44;

        public sealed class BarCodeHelper
        {
            private BarCodeHelper() { }

            public static string GenerateParCodeForTax(object data, string prefix, int width = 120, int height = 80, BarcodeFormat format = BarcodeFormat.PDF_417)
            {
                var text = NewDate.getHash(prefix + data.ToString());
                return GenerateBarcode(text, width, height, format);
            }

            public static string GenerateBarcode(string text, int width = 120, int height = 80, BarcodeFormat format = BarcodeFormat.PDF_417)
            {
                Image img = null;
                using (var ms = new MemoryStream())
                {
                    var writer = new ZXing.BarcodeWriter() { Format = format };
                    writer.Options.Height = height;
                    writer.Options.Width = width;
                    writer.Options.PureBarcode = true;
                    img = writer.Write(text);
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    //The Image is finally converted to Base64 string.
                    return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }


            //public static string GenerateParCodeForCarProced(long? carProcedNB, int width = 280, int height = 80, BarcodeFormat format = BarcodeFormat.QR_CODE)
            //{
            //    Image img = null;
            //    using (var ms = new MemoryStream())
            //    {
            //        var writer = new ZXing.BarcodeWriter() { Format = format };
            //        writer.Options.Height = 80;
            //        writer.Options.Width = 280;
            //        writer.Options.PureBarcode = true;
            //        img = writer.Write("" + carProcedNB);
            //        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            //        //The Image is finally converted to Base64 string.
            //        return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            //    }
            //}

        }


           

    }


}