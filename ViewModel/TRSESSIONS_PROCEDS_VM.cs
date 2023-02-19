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



    }



    // الطلبات في طباعة محضر الجلسة
    public class PROCEDS_Print_ALL
    {
        [Display(Name = "الرمز الالكتروني")]
        public long? NB { get; set; }


        [Display(Name = "رمز الجلسة")]
        public string SESSIONNB { get; set; }


        [Display(Name = "رمز نوع المعاملة")]
        public string PROCEDNB { get; set; }


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
        public string FACTYY { get; set; }


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
        public string COUNTCARLINE1 { get; set; }


        [Display(Name = "عدد مركبات الخط")]
        public string COUNTCARLINE2 { get; set; }



        [Display(Name = "رمز الخط المركبة")]
        public long? CARLINENB { get; set; }


        [Display(Name = "اسم خط المركبة")]
        public string CARLINENAME { get; set; }


        [Display(Name = "نوع خط  المركبة")]
        public string CARLINETYP { get; set; }


        [Display(Name = "محافظات خط المركبة")]
        public string CARLINECITY { get; set; }

        

    }

    public class ListPROCEDS_Print_ALL
    {

        public List<PROCEDS_Print_ALL> pro { get; set; }
        public long pronb { get; set; }
        public string proname { get; set; }
    }

}