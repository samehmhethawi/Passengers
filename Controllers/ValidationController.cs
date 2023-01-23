using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Passengers.Controllers
{
    
    public class ValidationController : Controller
    {
        private readonly ProcedContext db = new ProcedContext();
        public string OracleExceptionValidation(Exception ex)
        {
            string message;
            if (ex.InnerException == null)
            {
                message = ex.Message;
            }
            else
            {
                if (ex.InnerException.InnerException == null)
                {
                    message = ex.InnerException.Message;
                }
                else
                {
                    message = ex.InnerException.InnerException.Message;
                }
            }

            if (message.Contains("ORA"))
            {
                if (message.Contains("ORA-01400"))
                {
                    Dictionary<string, string> comment = GetColumnComment(message);

                    return "لا يجوز إدراج قيمة فارغة في " + comment["comment"] + "";
                }
                else
                {
                    if (message.Contains("ORA-00600"))
                    {
                        return "حصل خطأ في قاعدة البيانات";
                    }
                    else if (message.Contains("ORA-12154"))
                    {
                        return "فشل الاتصال بقاعدة البيانات";
                    }
                    else if (message.Contains("ORA-1722"))
                    {
                        Dictionary<string, string> comment = GetConstraintComment(message);
                        return "رقم غير صحيح عند " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-01000"))
                    {
                        return "تم تجاوز عدد المؤشرات المفتوحة في قاعدة البيانات";
                    }
                    else if (message.Contains("ORA-03113"))
                    {
                        return "تم قطع الاتصال مع قاعدة البيانات";
                    }
                    else if (message.Contains("ORA-06512"))
                    {
                        return "حصل خطأ غير متوقع في قاعدة البيانات";
                    }
                    else if (message.Contains("ORA-00942"))
                    {
                        return "خطأ في تعليمة التنفيذ";
                    }
                    else if (message.Contains("ORA-01017"))
                    {
                        return "اسم المستخدم او كلمة المرور غير صحيحة";
                    }
                    else if (message.Contains("ORA-00936"))
                    {
                        return "خطأ في تعليمة التنفيذ في قاعدة البينات";
                    }
                    else if (message.Contains("ORA-00911"))
                    {
                        Dictionary<string, string> comment = GetConstraintComment(message);
                        return "إدخال محرف غير صحيح عند " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-06502"))
                    {
                        Dictionary<string, string> comment = GetConstraintComment(message);
                        return "إدخال غير صحيح عند " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-00001"))
                    {
                        Dictionary<string, string> comment = GetConstraintComment(message);
                        return "تم انتهاك القيد الفريد عند " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-02290"))
                    {
                        Dictionary<string, string> comment = GetConstraintComment(message);
                        return "تم انتهاك قيد الاختبار يرجى التأكد من المدخل " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-01031"))
                    {
                        return "مستخدم الاوراكل لا يملك الصلاحيات";
                    }
                    else if (message.Contains("ORA-02292"))
                    {
                        Dictionary<string, string> comment = GetTableComment(message);
                        return "هذا السجل مرتبط بجدول  " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-00904"))
                    {
                        Dictionary<string, string> comment = GetColumnComment1(message);
                        return "ربط غير صالح مع   " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-02291"))
                    {
                        Dictionary<string, string> comment = GetConstraintComment(message);
                        return "المفتاح الأب غير موجود عند " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-12899"))
                    {
                        Dictionary<string, string> comment = GetColumnComment2(message);
                        return "القيمة كبيرة نوعا ما عند العمود " + comment["comment"] + "";
                    }
                    else if (message.Contains("ORA-00900"))
                    {
                        return "خطأ في تنفيذ التعليمة في قاعدة البيانات";
                    }
                    else if (message.Contains("ORA-01438"))
                    {
                        return "قيمة اكبر من الدقة المسموح بها";
                    }
                    else if (message.Contains("ORA-04063"))
                    {
                        return "خطأ في ال View في قاعدة البيانات";
                    }
                    else
                    {
                        return message;
                    }
                }
            }
            else
            {
                if (message.Contains("Unable to connect to the remote server"))
                {
                    return "خطأ لا يمكن الاتصال بالسيرفر, قد يكون السيرفر مطفأ او ليس لديك اتصال انترنت";
                }
                else if (message.Contains("Connection request timed out"))
                {
                    return "تجاوز مدة الاتصال المحددة";
                }
                else if (message.Contains("Unexpected character encountered while parsing value: <. Path '', line 0, position 0."))
                {
                    return "خطأ في التحقق من سجل المركبات";
                }
                else if (message.Contains("Store update, insert, or delete statement affected an unexpected number of rows (0)"))
                {
                    return "خطأ في العملية المطلوبة, لم يتمكن من الربط مع السجل المطلوب";
                }
                else
                {
                    return message;
                }
            }
        }

        private Dictionary<string, string> GetConstraintComment(string message)
        {
            Dictionary<string, string> comment = new Dictionary<string, string>();
            int pFrom = message.IndexOf("(") + "(".Length;
            int pTo = message.LastIndexOf(")");

            string constraint = message.Substring(pFrom, pTo - pFrom);
            constraint = constraint.Split('.')[1];
            comment["constraint"] = constraint;

            string sql1 = "SELECT TABLE_NAME FROM SYS.ALL_CONSTRAINTS WHERE OWNER = 'VEHICLES' AND CONSTRAINT_NAME = '" + constraint + "'";
            string table_name = db.Database.SqlQuery<string>(sql1).FirstOrDefaultAsync().Result;
            comment["table"] = table_name;
            string sql = "SELECT ACC.COLUMN_NAME FROM ALL_CONS_COLUMNS ACC WHERE ACC.OWNER = 'VEHICLES' AND ACC.TABLE_NAME = '" + table_name + "' AND ACC.CONSTRAINT_NAME = '" + constraint + "'";
            string column = db.Database.SqlQuery<string>(sql).FirstOrDefaultAsync().Result;
            comment["column"] = column;
            string sql2 = "SELECT COMMENTS FROM SYS.ALL_COL_COMMENTS WHERE OWNER = 'VEHICLES' AND TABLE_NAME = '" + table_name + "' AND COLUMN_NAME = '" + column + "'";
            string comment1 = db.Database.SqlQuery<string>(sql2).FirstOrDefaultAsync().Result;
            comment["comment"] = comment1;

            return comment;
        }
        private Dictionary<string, string> GetColumnComment(string message)
        {
            Dictionary<string, string> comment = new Dictionary<string, string>();
            int pFrom = message.IndexOf("(") + "(".Length;
            int pTo = message.LastIndexOf(")");

            string constraint = message.Substring(pFrom, pTo - pFrom);
            string[] temp = constraint.Split('.');
            string column = temp[2].Trim(new char[] { '"', '\\' });
            string table_name = temp[1].Trim(new char[] { '"', '\\' });

            string sql2 = "SELECT COMMENTS FROM SYS.ALL_COL_COMMENTS WHERE OWNER = 'VEHICLES' AND TABLE_NAME = '" + table_name + "' AND COLUMN_NAME = '" + column + "'";
            string comment1 = db.Database.SqlQuery<string>(sql2).FirstOrDefaultAsync().Result;
            comment["constraint"] = constraint;
            comment["table"] = table_name;
            comment["column"] = column;
            comment["comment"] = comment1;

            return comment;
        }
        private Dictionary<string, string> GetTableComment(string message)
        {
            Dictionary<string, string> comment = new Dictionary<string, string>();
            int pFrom = message.IndexOf("(") + "(".Length;
            int pTo = message.LastIndexOf(")");

            string constraint = message.Substring(pFrom, pTo - pFrom);
            string[] temp = constraint.Split('.');
            string constraint1 = temp[1];
            string sql1 = "SELECT TABLE_NAME FROM SYS.ALL_CONSTRAINTS WHERE OWNER = 'VEHICLES' AND CONSTRAINT_NAME = '" + constraint1 + "'";
            string table_name = db.Database.SqlQuery<string>(sql1).FirstOrDefaultAsync().Result;

            string sql2 = "SELECT COMMENTS FROM SYS.ALL_TAB_COMMENTS WHERE OWNER = 'VEHICLES' AND TABLE_NAME = '" + table_name + "'";
            string comment1 = db.Database.SqlQuery<string>(sql2).FirstOrDefaultAsync().Result;
            comment["constraint"] = constraint1;
            comment["table"] = table_name;
            comment["comment"] = comment1;

            return comment;
        }
        private Dictionary<string, string> GetColumnComment1(string message)
        {
            Dictionary<string, string> comment = new Dictionary<string, string>();

            string[] temp = message.Split('"');
            string table_name = temp[1].Trim(new char[] { '"', '\\' });
            string[] temp1 = table_name.Split('_');
            table_name = temp1[0];
            string column = temp1[1];
            string sql2 = "SELECT COMMENTS FROM SYS.ALL_TAB_COMMENTS WHERE OWNER = 'VEHICLES' AND TABLE_NAME = '" + table_name + "'";
            string comment1 = db.Database.SqlQuery<string>(sql2).FirstOrDefaultAsync().Result;

            comment["table"] = table_name;
            comment["column"] = column;
            comment["comment"] = comment1;

            return comment;
        }
        private Dictionary<string, string> GetColumnComment2(string message)
        {
            Dictionary<string, string> comment = new Dictionary<string, string>();

            string[] temp = message.Split('"');
            string table_name = temp[3].Trim(new char[] { '"', '\\' });
            string column = temp[5].Trim(new char[] { '"', '\\' });

            string sql2 = "SELECT COMMENTS FROM SYS.ALL_COL_COMMENTS WHERE OWNER = 'VEHICLES' AND TABLE_NAME = '" + table_name + "' AND COLUMN_NAME = '" + column + "'";
            string comment1 = db.Database.SqlQuery<string>(sql2).FirstOrDefaultAsync().Result;

            comment["table"] = table_name;
            comment["column"] = column;
            comment["comment"] = comment1;

            return comment;

        }
    }
}