using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    // المعاملات في الجلسة
    public class TRSESSIONS_PROCEDS_VM
    {
        public long? NB { get; set; }
        public string PROCEDNAME { get; set; }
        public int? PROCEDNB { get; set; }

        public long? SESSIONNB { get; set; }

        public long? CARPROCEDNB { get; set; }

        public DateTime RECDAT { get; set; }

        public long? PSTATUS { get; set; }

        public long? CARNB { get; set; }

        public long? ORDR { get; set; }

        public long? CARPROCEDSTEPNB { get; set; }

        public string TYPSNAMEAGR { get; set; }

        public long? ISDONE { get; set; }
        public string NOTES { get; set; }


    }



    // الطلبات في طباعة محضر الجلسة
    public class PROCEDS_Print_ALL
    {
        [Display(Name = "الرمز الالكتروني")]
        public long? NB { get; set; }


        [Display(Name = "رمز الجلسة")]
        public long? SESSIONNB { get; set; }


        [Display(Name = "رمز نوع المعاملة")]
        public long? PROCEDNB { get; set; }


        [Display(Name = "اسم المعاملة")]
        public string PROCEDNAME { get; set; }


        [Display(Name = "الترتيب")]
        public long? ORDR { get; set; }


        [Display(Name = "سنة الجلسة")]
        public long? RYEAR { get; set; }


        [Display(Name = " اسم الخط الجديد")]
        public string LINENAME { get; set; }


        [Display(Name = "نوع الخط")]
        public string TYP { get; set; }


        [Display(Name = "رمز الخط")]
        public long? LINENB1 { get; set; }


        [Display(Name = "اسم الخط")]
        public string LINENAME1 { get; set; }

        [Display(Name = "نوع الخط")]
        public string LINETYP1 { get; set; }


        [Display(Name = "رمز الخط")]
        public long? LINENB2 { get; set; }


        [Display(Name = "اسم الخط")]
        public string LINENAME2 { get; set; }


        [Display(Name = "نوع الخط")]
        public string LINETYP2 { get; set; }


        [Display(Name = "رمز المركبة")]
        public long? CARNB { get; set; }


        [Display(Name = "رقم اللوحة")]
        public string TABNB { get; set; }


        [Display(Name = "الصانع")]
        public string FACCOMPNAME { get; set; }


        [Display(Name = "سنة الصنع")]
        public long? FACTYY { get; set; }


        [Display(Name = "نوع الوقود")]
        public string ENGINEFEUL { get; set; }


        [Display(Name = "عدد الركاب")]
        public long? SITES { get; set; }


        [Display(Name = "رمز المعاملة")]
        public long? CARPROCEDNB { get; set; }


        [Display(Name = "قرار اللجنة")]
        public string PSTATUS { get; set; }


        [Display(Name = "محافظات الخط ")]
        public string NEWLINECITY { get; set; }


        [Display(Name = "محافظات الخط")]
        public string LINE1CITY { get; set; }


        [Display(Name = "محافظات الخط")]
        public string LINE2CITY { get; set; }


        [Display(Name = "عدد مركبات الخط")]
        public long? COUNTCARLINE1 { get; set; }


        [Display(Name = "عدد مركبات الخط")]
        public long? COUNTCARLINE2 { get; set; }



        [Display(Name = "رمز الخط المركبة")]
        public long? CARLINENB { get; set; }


        [Display(Name = "اسم خط المركبة")]
        public string CARLINENAME { get; set; }


        [Display(Name = "نوع خط  المركبة")]
        public string CARLINETYP { get; set; }


        [Display(Name = "محافظات خط المركبة")]
        public string CARLINECITY { get; set; }
        [Display(Name = "الموافقات الممنوحة في المعاملة")]
        public string AGREEMENTINPROCED { get; set; }



        [Display(Name = "مدة التعاقد (شهر)")]
        public long? CONTRACT_PERIOD { get; set; }

        [Display(Name = "الفترة الصباحية من الساعة")]
        public int? DAY_TRIP_FROM { get; set; }

        [Display(Name = "الفترة الصباحية الى الساعة")]
        public int? DAY_TRIP_TO { get; set; }

        [Display(Name = "الفترة المسائية من الساعة")]
        public int? NIGHT_TRIP_FROM { get; set; }

        [Display(Name = "الفترة المسائية الى الساعة")]
        public int? NIGHT_TRIP_TO { get; set; }

        [Display(Name = "مسار السفرات ")]
        public string TRIP_PATH { get; set; }

        [Display(Name = "تاريخ السفر")]
        public DateTime? TRAVEL_MONTH { get; set; }

        [Display(Name = "البلد الذي سيتم السفر له")]
        public string TRAVEL_COUNTRY { get; set; }

        [Display(Name = "المنفذ الحدودي")]
        public string TRAVEL_BOARDER { get; set; }

        [Display(Name = "نص طلب البريد")]
        public string POSTMAIL { get; set; }

        [Display(Name = "المرسل")]
        public string SENDER { get; set; }


        [Display(Name = "رقم الكتاب")]
        public string POSTNO { get; set; }


        [Display(Name = "تاريخ الكتاب")]
        public DateTime? POSTDATE { get; set; }


        [Display(Name = "رمز المركبة الثاني")]
        public long? CARNB2 { get; set; }

        [Display(Name = "ملاحظات")]
        public string NOTES { get; set; }

        [Display(Name = "سبب السفرة")]
        public string CAUSES { get; set; }






    }

    public class ListPROCEDS_Print_ALL
    {

        public List<PROCEDS_Print_ALL> pro { get; set; }
        public long pronb { get; set; }
        public string proname { get; set; }
    }

}