 using Proced.DataAccess.Models.CF;

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


    }


}