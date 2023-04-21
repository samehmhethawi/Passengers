
//namespace ProcedBase.Site.Helper
//{
//    public class ProcedConditionsHelper
//    {
//        private readonly ProcedContext Context;
//        string carForeignKeyName = "CARNB",
//            ownerForeignKeyName = "OWNERNB",
//            htmlNewLine = "<br />";
//        private Dictionary<string, string> queriesValues = new Dictionary<string, string>();
//        public ProcedConditionsHelper(ProcedContext context)
//        {
//            this.Context = context;
//        }

//        public Dictionary<TAX, List<TAXES_VALUES>> GetTaxesValues(CAR car, CARPROCED carProced, long? ownerNb, DateTime date, List<TAX> carProcedTaxesOfType, out List<string> errors)
//        {
//            errors = new List<string>();

//            var allZProcedTaxes = carProcedTaxesOfType.ToList();

//            List<TAX> model = new List<TAX>();

//            foreach (var tax in allZProcedTaxes)
//            {
//                if (ZTaxesNbs.SeiezDept == tax.NB)
//                {
//                    if (!(carProced.CARPROCED_SEIZS != null && carProced.CARPROCED_SEIZS.Any(cps => cps.DEPTVALE > 0)))
//                    {
//                        continue;
//                    }
//                }
//                if (!tax.TAX_PERIODS.Any())
//                {
//                    model.Add(tax);
//                }
//                else if (tax.TAX_PERIODS.Any(sp => (sp.TO_DATE.HasValue && date >= sp.FROM_DATE && date <= sp.TO_DATE) || (date >= sp.FROM_DATE && sp.TO_DATE == null)))
//                {
//                    var periods = tax.TAX_PERIODS.OrderBy(p => p.FROM_DATE).ThenBy(p => p.TO_DATE);
//                    var texesValuesInThisDate = periods.FirstOrDefault(sp => ((!sp.TO_DATE.HasValue && date >= sp.FROM_DATE) || (sp.TO_DATE.HasValue && date >= sp.FROM_DATE && date <= sp.TO_DATE)) && sp.TAXES_VALUES.Any());

//                    if (texesValuesInThisDate != null)
//                        model.Add(tax);
//                }
//            }

//            Dictionary<TAX, List<TAXES_VALUES>> taxesValues = new Dictionary<TAX, List<TAXES_VALUES>>();
//            foreach (var tax in model)
//            {
//                if (tax.NB == ZTaxesNbs.SocialTaamint)
//                {
//                    var tameenStep = carProced.CARPROCEDSTEPS.FirstOrDefault(cps => cps.STEPNB == ProcedBase.ZStepTypes.INSURANCE);
//                    if (tameenStep != null && tameenStep.NOTE != null)
//                    {
//                        int value = -1;
//                        if (int.TryParse(tameenStep.NOTE, out value) && value >= 0)
//                        {
//                            if (!taxesValues.ContainsKey(tax))
//                            {
//                                List<TAXES_VALUES> t = new List<TAXES_VALUES>();
//                                var TV = new TAXES_VALUES()
//                                {
//                                    VALUE = value,
//                                    TAX_TYPE_NB = 1,
//                                    CONDITIONS = new List<CONDITION>(),
//                                    ConditionsAreValid = true,
//                                    ValueAreValid = true,
//                                };
//                                t.Add(TV);
//                                taxesValues.Add(tax, t);
//                            }
//                            else
//                            {
//                                var t = taxesValues.GetValueOrNull(tax);
//                                var TV = new TAXES_VALUES()
//                                {
//                                    VALUE = value,
//                                    TAX_TYPE_NB = 1,
//                                    CONDITIONS = new List<CONDITION>(),
//                                    ConditionsAreValid = true,
//                                    ValueAreValid = true,
//                                };
//                                t.Add(TV);
//                                taxesValues.Remove(tax);
//                                taxesValues.Add(tax, t);
//                            }
//                        }
//                    }
//                    continue;
//                }
//                var conditions = tax.TAX_PERIODS.SelectMany(p => p.TAXES_VALUES).SelectMany(m => m.CONDITIONS)/*.OrderBy(c => c.ORDR)*/.ToList();
//                if (!conditions.Any())
//                {
//                    try
//                    {
//                        if (tax.TAX_PERIODS.ElementAt(0) != null)
//                            taxesValues.Add(tax, tax.TAX_PERIODS.ElementAt(0)?.TAXES_VALUES.ToList());
//                    }
//                    catch (Exception) { }
//                    continue;
//                }
//                string carForeignKeyName = "CARNB",
//                    ownerForeignKeyName = "OWNERNB";

//                bool isValid = true;
//                var periods = tax.TAX_PERIODS.OrderBy(p => p.FROM_DATE).ThenBy(p => p.TO_DATE);
//                var texesValuesInThisDate = periods.FirstOrDefault(sp => ((!sp.TO_DATE.HasValue && date >= sp.FROM_DATE) || (sp.TO_DATE.HasValue && date >= sp.FROM_DATE && date <= sp.TO_DATE)) && sp.TAXES_VALUES.Any());

//                var taxValues = texesValuesInThisDate.TAXES_VALUES.ToList();
//                foreach (var value in taxValues)
//                {
//                    value.ConditionsAreValid = true;
//                    isValid = true;
//                    conditions = value.CONDITIONS/*.OrderBy(c => c.ORDR)*/.ToList();
//                    foreach (var condition in conditions)
//                    {
//                        var cond = condition;
//                        var feild = cond.ZTAX_FEILDS;
//                        if (feild.TABLE_NAME == "CAROWNERS" && !ownerNb.HasValue)
//                        {
//                            break;
//                        }
//                        else if (feild.TABLE_NAME == "CAROWNERS" && ownerNb.HasValue)
//                        {
//                            ownerForeignKeyName = "NB";
//                        }
//                        string pkName = (feild.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                        if (feild.TABLE_NAME == "CARS")
//                        {
//                            pkName = "NB";
//                        }
//                        long? pkValue = (feild.TABLE_NAME == "CAROWNERS") ? ownerNb : car.NB;
//                        if (feild.TABLE_NAME == "CARPROCED_SEIZS" && feild.FEILD_NAME == "DEPTVALE")
//                        {
//                            pkName = "CARPROCEDNB";
//                            pkValue = carProced.NB;
//                        }
//                        var sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                        string query = "";
//                        if (!queriesValues.ContainsKey(sql))
//                        {
//                            if (feild.DATA_TYPE == DataTypes.NVARCHAR2 || feild.DATA_TYPE == DataTypes.VARCHAR2)
//                            {
//                                string queryResult = "";
//                                try
//                                {
//                                    queryResult = Context.Database.SqlQuery<string>(sql).FirstOrDefault();
//                                }
//                                catch (Exception)
//                                {
//                                    queryResult = "" + Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                }
//                                if (string.IsNullOrEmpty(queryResult) && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR" ))
//                                {
//                                    queryResult = "1";
//                                }
//                                else if (queryResult == null)
//                                {
//                                    var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";

//                                    string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                    addError(errors, error);
//                                    value.ConditionsAreValid = false;

//                                    break;
//                                }
//                                query = queryResult;
//                            }
//                            else if (feild.DATA_TYPE == DataTypes.Char)
//                            {
//                                var queryResult = Context.Database.SqlQuery<char?>(sql).FirstOrDefault();
//                                if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                {
//                                    queryResult = '1';
//                                }
//                                else if (queryResult == null)
//                                {
//                                    var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                    string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                    value.ConditionsAreValid = false;

//                                    //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                    addError(errors, error);
//                                    break;
//                                }
//                                query = "" + queryResult;
//                            }
//                            else if (feild.DATA_TYPE == DataTypes.Number)
//                            {
//                                try
//                                {
//                                    var queryResult = Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
//                                    if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                    {
//                                        queryResult = 1;
//                                    }
//                                    else if (queryResult == null)
//                                    {
//                                        var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                        string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                        //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                        addError(errors, error);
//                                        value.ConditionsAreValid = false;
//                                        break;
//                                    }
//                                    query = "" + queryResult;
//                                }
//                                catch (Exception)
//                                {
//                                    try
//                                    {
//                                        var queryResult = Context.Database.SqlQuery<double?>(sql).FirstOrDefault();
//                                        if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                        {
//                                            queryResult = 1;
//                                        }
//                                        else if (queryResult == null)
//                                        {
//                                            var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                            string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                            //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                            value.ConditionsAreValid = false;
//                                            addError(errors, error);
//                                            break;
//                                        }
//                                        query = "" + queryResult;
//                                    }
//                                    catch (Exception)
//                                    {
//                                        try
//                                        {
//                                            var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                            if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                            {
//                                                queryResult = 1;
//                                            }
//                                            else if (queryResult == null)
//                                            {
//                                                var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                                string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                                //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                                value.ConditionsAreValid = false;
//                                                addError(errors, error);
//                                                break;
//                                            }
//                                            query = "" + queryResult;
//                                        }
//                                        catch (Exception)
//                                        {
//                                            try
//                                            {
//                                                var queryResult = Context.Database.SqlQuery<int?>(sql).FirstOrDefault();
//                                                if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                                {
//                                                    queryResult = 1;
//                                                }
//                                                else if (queryResult == null)
//                                                {
//                                                    string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME);
//                                                    value.ConditionsAreValid = false;
//                                                    addError(errors, error);
//                                                    break;
//                                                }
//                                                query = "" + queryResult;
//                                            }
//                                            catch (Exception)
//                                            {

//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                            else if (feild.DATA_TYPE == DataTypes.Long)
//                            {
//                                var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                {
//                                    queryResult = 1;
//                                }
//                                else if (queryResult == null)
//                                {
//                                    var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                    string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                    //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                    value.ConditionsAreValid = false;
//                                    addError(errors, error);
//                                    break;
//                                }
//                                query = "" + queryResult;
//                            }
//                            if (!string.IsNullOrEmpty(query))
//                            {
//                                if ((cond?.ZTAX_FEILDS?.TAX_FEILDS_REFFERENCES?.Any()).GetValueOrDefault(false))
//                                {
//                                    var firstReff = cond?.ZTAX_FEILDS.TAX_FEILDS_REFFERENCES.ElementAt(0);
//                                    var sql2 = string.Format("SELECT {0} FROM {1} WHERE NB={2}", firstReff.REF_FEILD_NAME, firstReff.REF_TABLE_NAME, query);
//                                    if (queriesValues.ContainsKey(sql2))
//                                    {
//                                        queriesValues.TryGetValue(sql2, out query);
//                                    }
//                                    else
//                                    {
//                                        try
//                                        {
//                                            query = Context.Database.SqlQuery<string>(sql2).FirstOrDefault();
//                                        }
//                                        catch (Exception)
//                                        {
//                                        }
//                                    }
//                                }
//                            }
//                            try
//                            {
//                                queriesValues.Add(sql, query);
//                            }
//                            catch (Exception)
//                            {
//                            }
//                        }
//                        else
//                        {
//                            queriesValues.TryGetValue(sql, out query);
//                        }

//                        if (cond.OPERATOR_NB == (long)MathematicalOperators.Equal)//Equal
//                        {
//                            isValid = cond.VALUE1 == (query + "");
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotEqual)//Not Equal
//                        {
//                            isValid = cond.VALUE1 != (query + "");
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.Between)//between
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1)) && ((decimal.Parse(query) <= decimal.Parse(cond.VALUE2)));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotBetween)//not between
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1)) || ((decimal.Parse(query) > decimal.Parse(cond.VALUE2)));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.GreaterOrEqual)//Greater Or Equal
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.Greater)//Greater
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) > decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.LessOrEqual)//Less Or Equal
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) <= decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.Less)//Less
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }

//                        if (!isValid)
//                        {
//                            break;//break conditions loop
//                        }
//                    }
//                    if (isValid)
//                    {
//                        if (!taxesValues.ContainsKey(tax))
//                        {
//                            List<TAXES_VALUES> t = new List<TAXES_VALUES>();
//                            t.Add(value);
//                            taxesValues.Add(tax, t);
//                        }
//                        else
//                        {
//                            var t = taxesValues.GetValueOrNull(tax);
//                            t.Add(value);
//                            taxesValues.Remove(tax);
//                            taxesValues.Add(tax, t);
//                        }
//                    }
//                }
//            }
//            Dictionary<TAX, List<TAXES_VALUES>> result = new Dictionary<TAX, List<TAXES_VALUES>>();
//            var hasError = false;
//            if (errors != null && errors.Any() &&
//                taxesValues != null && taxesValues.Any())
//            {
//                foreach (var item in taxesValues)
//                {
//                    if (item.Value == null || !item.Value.Any(m => m.ConditionsAreValid == true))
//                    {
//                        hasError = true;
//                        break;
//                    }
//                    else if(item.Value != null && item.Value.Any())
//                    {
//                        var validValues = item.Value.Where(m => m.ConditionsAreValid == true).ToList();
//                        //result.Remove(item.Key);
//                        result.Add(item.Key, validValues);
//                    }
//                }
//            }
//            else if(taxesValues != null && taxesValues.Any())
//            {
//                foreach (var item in taxesValues)
//                {
//                    if (item.Value == null || !item.Value.Any(m => m.ConditionsAreValid == true))
//                    {
//                        hasError = true;
//                        break;
//                    }
//                    else if (item.Value != null && item.Value.Any())
//                    {
//                        var validValues = item.Value.Where(m => m.ConditionsAreValid == true).ToList();
//                        //result.Remove(item.Key);
//                        result.Add(item.Key, validValues);
//                    }
//                }
//            }
//            if (!hasError)
//            {
//                errors = new List<string>();
//            }
//            return result;
//        }

//        public List<CARPROCEDDOC> GetRequiredDocumentsOfCarProced(CAR car, DateTime date, long? carProcedNb, long? carProcedTypeNb, long? NewCarCategoryId, long? ZREGSId, long? CUSTTYP, long? TABTYP, long? BaseRegId, List<long> carOwners)
//        {
//            List<CARPROCEDDOC> carProcedDocs = null;
//            List<ZPROCEDDOC> model1 = new List<ZPROCEDDOC>();
//            int? carReg = null, reg = null;
//            if (car != null)
//            {
//                //ZREGSId, long? CUSTTYP, long? TABTYP, long? BaseRegId
//                ZREGSId = car.CARREGNB;
//                BaseRegId = car.BASREG;
//                CUSTTYP = car.CUSTTYP;
//                NewCarCategoryId = car.CARCATNB;

//                carReg = car.CARREG;
//                reg = car.REG;

//                //TABTYP = car.TABNB;
//                var owners = car?.CAROWNS.ToList();
//                if (owners != null && owners.Any())
//                {
//                    List<ZPROCEDDOC> allDocs = new List<ZPROCEDDOC>();
//                    foreach (var owner in owners)
//                    {
//                        var docs = FilterProcedDocuments(car, owner?.OWNERNB, carProcedTypeNb, date);
//                        if (docs != null && docs.Any())
//                        {
//                            docs = docs.Where(d => d.IS_REQUIRED == true).ToList();
//                        }
//                        allDocs.AddRange(docs);
//                    }
//                    if (allDocs.Any())
//                    {
//                        model1 = allDocs/*.DistinctBy(d => d.DOCNB)*/.ToList();
//                        carProcedDocs = model1.Select(m => new CARPROCEDDOC()
//                        {
//                            ZDOCTYP = m.ZDOCTYP,
//                            DOCTYPNB = m.ZDOCTYP.NB,
//                        }).ToList();
//                    }
//                }
//            }
//            else if (carProcedNb.HasValue)
//            {
//                var carProced = Context.CARPROCEDS.Where(cp => cp.NB == carProcedNb.Value).Include(cp => cp.CARPROCEDDOCS).FirstOrDefault();
//                if (carProced != null)
//                {
//                    carProcedDocs = carProced.CARPROCEDDOCS.ToList();
//                }
//            }
//            else if (carProcedTypeNb.HasValue)
//            {
//                var carProcedType = Context.ZPROCEDTYPS.Where(m => m.NB == carProcedTypeNb.Value).Include(m => m.ZPROCEDSTEPS).FirstOrDefault();
//                if (carProcedType != null)
//                {
//                    model1 = carProcedType.ZPROCEDSTEPS
//                                          .SelectMany(m => m.ZPROCEDDOCS)
//                                          .Where(m => m.IS_REQUIRED == true)
//                                          //.DistinctBy(d => d.DOCNB)
//                                          .ToList();
//                }
//            }
//            if (model1.Any())
//            {
//                List<ZPROCEDDOC> model = new List<ZPROCEDDOC>();
//                foreach (var zProcedDoc in model1)
//                {
//                    if (zProcedDoc.ZPROCEDDOCS_PERIODS.Any(p => (p.TO != null && p.FROM <= date && date <= p.TO) || (p.TO == null && p.FROM <= date)))
//                    {
//                        var period = zProcedDoc.ZPROCEDDOCS_PERIODS.FirstOrDefault(p => (p.TO != null && p.FROM <= date && date <= p.TO) || (p.TO == null && p.FROM <= date));
//                        if (period != null)
//                        {
//                            var docPeriodConditions = period.ZPROCEDDOCS_PERIODS_CONDS.ToList();
//                            if (!docPeriodConditions.Any())
//                            {
//                                if (model != null && !model.Any(m => m.DOCNB == zProcedDoc.DOCNB))
//                                    model.Add(zProcedDoc);
//                            }
//                            else
//                            {
//                                var isValid = true;
//                                foreach (var condition in docPeriodConditions)
//                                {
//                                    var cond = condition.ZPROCEDTYP_CONDITIONS;
//                                    if (cond != null)
//                                    {
//                                        var feild = cond.ZTAX_FEILDS;
//                                        if (feild.TABLE_NAME == "CARS" && feild.FEILD_NAME == "CUSTTYP")
//                                        {
//                                            var ZCUSTTYP = Context.ZCUSTTYPS.Find(CUSTTYP);
//                                            string val1 = ZCUSTTYP?.NAME;
//                                            isValid = checkConditionValue(cond, val1);
//                                        }
//                                        else if (feild.TABLE_NAME == "CARS" && feild.FEILD_NAME == "CARCATNB")
//                                        {
//                                            var ZCARCATEGORY = Context.ZCARCATEGORYS.Find(NewCarCategoryId);
//                                            string val1 = ZCARCATEGORY?.NAME;
//                                            isValid = checkConditionValue(cond, val1);
//                                        }

//                                        else if (feild.TABLE_NAME == "CARS" && feild.FEILD_NAME == "REG")
//                                        {
//                                            var ZCarKind = Context.ZCARKINDS.Find(reg);
//                                            string val1 = ZCarKind?.NAME;
//                                            isValid = checkConditionValue(cond, val1);
//                                        }

//                                        else if (feild.TABLE_NAME == "CARS" && feild.FEILD_NAME == "CARREG")
//                                        {
//                                            var ZReg = Context.ZREGS.Find(carReg);
//                                            string val1 = ZReg?.NAME;
//                                            isValid = checkConditionValue(cond, val1);
//                                        }

//                                        else if (feild.TABLE_NAME == "CARS" && feild.FEILD_NAME == "BASREG")
//                                        {
//                                            var ZREGTYP = Context.ZREGTYPS.Find(BaseRegId);
//                                            string val1 = ZREGTYP?.NAME;
//                                            isValid = checkConditionValue(cond, val1);
//                                        }
//                                        else if (feild.TABLE_NAME == "CARPROCEDS" && feild.FEILD_NAME == "TABTYP")
//                                        {
//                                            var zTableauModel = Context.TABLEAUMODELs.Find(TABTYP);
//                                            string val1 = zTableauModel?.NAME;
//                                            isValid = checkConditionValue(cond, val1);
//                                        }
//                                        else if (feild.TABLE_NAME == "CARS" && feild.FEILD_NAME == "CARREGNB")
//                                        {
//                                            var ZCARREG = Context.ZCARREGS.Find(ZREGSId);
//                                            string val1 = ZCARREG?.NAME;
//                                            isValid = checkConditionValue(cond, val1);
//                                        }
//                                        else if (feild.TABLE_NAME == "CAROWNERS")
//                                        {
//                                            try
//                                            {
//                                                foreach (var carOwnerNb in carOwners)
//                                                {
//                                                    var carOwner = Context.CAROWNERS.Find(carOwnerNb);
//                                                    string val1 = carOwner.GetType().GetProperty(feild.FEILD_NAME).GetValue(carOwner, null) + "";
//                                                    if ((feild.TAX_FEILDS_REFFERENCES?.Any()).GetValueOrDefault(false))
//                                                    {
//                                                        var firstReff = cond?.ZTAX_FEILDS.TAX_FEILDS_REFFERENCES.ElementAt(0);
//                                                        var sql = string.Format("SELECT {0} FROM {1} WHERE NB={2}", firstReff.REF_FEILD_NAME, firstReff.REF_TABLE_NAME, val1);
//                                                        try
//                                                        {
//                                                            val1 = Context.Database.SqlQuery<string>(sql).FirstOrDefault();

//                                                        }
//                                                        catch (Exception)
//                                                        {
//                                                        }
//                                                    }
//                                                    isValid = checkConditionValue(cond, val1);
//                                                    if (isValid)
//                                                    {
//                                                        break;
//                                                    }
//                                                }
//                                            }
//                                            catch (Exception ex)
//                                            {
//                                            }
//                                        }
//                                        else
//                                        {
//                                            isValid = true;
//                                        }
//                                    }
//                                    if (!isValid)
//                                        break;
//                                }
//                                if (isValid)
//                                {
//                                    if (model != null && !model.Any(m => m.DOCNB == zProcedDoc.DOCNB))
//                                        model.Add(zProcedDoc);
//                                }
//                            }
//                        }
//                        else
//                        {
//                            if (model != null && !model.Any(m => m.DOCNB == zProcedDoc.DOCNB))
//                                model.Add(zProcedDoc);
//                        }
//                    }
//                    else if (!zProcedDoc.ZPROCEDDOCS_PERIODS.Any(p => (p.TO != null && p.FROM <= date && date <= p.TO) || (p.TO == null && p.FROM <= date)))
//                    {
//                        if (model != null && !model.Any(m => m.DOCNB == zProcedDoc.DOCNB))
//                            model.Add(zProcedDoc);
//                    }
//                }
//                carProcedDocs = model.Select(m => new CARPROCEDDOC()
//                {
//                    ZDOCTYP = m.ZDOCTYP,
//                    DOCTYPNB = m.ZDOCTYP.NB,
//                }).ToList();
//                //}
//            }
//            return carProcedDocs;
//        }

//        private bool checkConditionValue(ZPROCEDTYP_CONDITIONS cond, string val1)
//        {
//            bool isValid = false;
//            MathematicalOperators operatorType = (MathematicalOperators)cond.OPERATOR_NB;
//            switch (operatorType)
//            {
//                case MathematicalOperators.Equal:
//                    isValid = cond.VALUE1 == val1;
//                    break;
//                case MathematicalOperators.NotEqual:
//                    isValid = cond.VALUE1 != val1;
//                    break;
//                case MathematicalOperators.Between:
//                    isValid = cond.VALUE1 == val1;
//                    break;
//                case MathematicalOperators.NotBetween:
//                    isValid = cond.VALUE1 == val1;
//                    break;
//                case MathematicalOperators.GreaterOrEqual:
//                    isValid = cond.VALUE1 == val1;
//                    break;
//                case MathematicalOperators.Greater:
//                    isValid = cond.VALUE1 == val1;
//                    break;
//                case MathematicalOperators.LessOrEqual:
//                    isValid = cond.VALUE1 == val1;
//                    break;
//                case MathematicalOperators.Less:
//                    isValid = cond.VALUE1 == val1;
//                    break;
//            }
//            return isValid;
//        }

//        private void addError(List<string> errors, string error)
//        {
//            if (errors != null && !errors.Any(e => e.Equals(error)))
//            {
//                errors.Add(error);
//            }
//        }

//        public Dictionary<TAX, List<TAXES_VALUES>> GetProcedTaxesValues(long carNb, long? ownerNb, long? zCarProcedTypeNb, DateTime date, List<ZPROCEDTAX> inputTaxes, ref List<string> errors, CARPROCED carProced)
//        {
//            if (errors == null)
//            {
//                errors = new List<string>();
//            }
//            CAR car = Context.CARS.Find(carNb);
//            if (car == null)
//            {
//                return null;
//            }
//            var allZProcedTaxes = inputTaxes.Where(M => M.TAX != null).Select(s => s.TAX).ToList();
//            if (carProced != null)
//            {
//                var carProcesSteps = carProced.CARPROCEDSTEPS.ToList();
//                var stepsRelatedTaxes = carProcesSteps.Where(cps => cps.ZSTEPTYP != null).SelectMany(cps => cps.ZSTEPTYP.RelatedTaxes).ToList();
//                foreach (var stepTax in stepsRelatedTaxes)
//                {
//                    if (!allZProcedTaxes.Any(t => t.NB == stepTax.NB))
//                    {
//                        allZProcedTaxes.Add(stepTax);
//                    }
//                }
//                List<TAX> newTaxes = new List<TAX>();
//                var TaxesrelatedToSteps = allZProcedTaxes.Where(t => t.ZSTEPTYPNB != null).ToList();
//                foreach (var tax in TaxesrelatedToSteps)
//                {
//                    var step = tax.RelatedZStepType ?? Context.ZSTEPTYPS.Find(tax.ZSTEPTYPNB);
//                    if (!carProcesSteps.Any(cps => cps.STEPNB == step.NB))
//                    {
//                        newTaxes.Add(tax);
//                    }
//                }
//                if (newTaxes.Any())
//                {
//                    var existedTaxIds = newTaxes.Select(t => t.NB).ToList();
//                    allZProcedTaxes = allZProcedTaxes.Where(t => !existedTaxIds.Contains(t.NB)).ToList();
//                }

//                List<TAX> oneTime = new List<TAX>();

//                try
//                {
//                    if (allZProcedTaxes.Any(t => t.ONETIME_IN_YEAR == true))
//                    {
//                        var oneTimeTaxes = allZProcedTaxes.Where(t => t.ONETIME_IN_YEAR == true).ToList();
//                        var oneTimeTaxesIds = oneTimeTaxes.Select(m => m.NB).ToList();
//                        var carTaxes = Context.CARTAXS.Where(ct => ct.CARNB == carProced.CARNB).OrderByDescending(ct => ct.TAXDATE).ThenBy(ct => ct.NB).ToList();
//                        foreach (var oneTimeTax in oneTimeTaxes)
//                        {
//                            foreach (var item in carTaxes)
//                            {
//                                if (item.CARTAXITEMS.Any(cti => cti.TAXNB == oneTimeTax.NB))
//                                {
//                                    if (!item.TAXDATE.HasValue)
//                                    {
//                                        break;
//                                    }
//                                    if (item.TAXDATE.Value.AddYears(1).Date >= date.Date)
//                                    {
//                                        oneTime.Add(oneTimeTax);
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                        if (oneTime.Any())
//                        {
//                            var existedTaxIds = oneTime.Select(t => t.NB).ToList();
//                            allZProcedTaxes = allZProcedTaxes.Where(t => !existedTaxIds.Contains(t.NB)).ToList();
//                        }
//                    }
//                }
//                catch (Exception ex){}
//            }
//            try
//            {
//                allZProcedTaxes = allZProcedTaxes.DistinctBy(c => c.NB).ToList();
//            }
//            catch (Exception)
//            {
//            }
//            List<TAX> model = new List<TAX>();

//            foreach (var tax in allZProcedTaxes)
//            {
//                if (!tax.TAX_PERIODS.Any())
//                {
//                    model.Add(tax);
//                }
//                else if (tax.TAX_PERIODS.Any(sp => (sp.TO_DATE.HasValue && date >= sp.FROM_DATE && date <= sp.TO_DATE) || (date >= sp.FROM_DATE && sp.TO_DATE == null)))
//                {
//                    var texesValuesInThisDate = tax.TAX_PERIODS.Where(sp => ((!sp.TO_DATE.HasValue && date >= sp.FROM_DATE) || (sp.TO_DATE.HasValue && date >= sp.FROM_DATE && date <= sp.TO_DATE)) && sp.TAXES_VALUES.Any());

//                    if (texesValuesInThisDate.Any())
//                        model.Add(tax);
//                }
//            }

//            Dictionary<TAX, List<TAXES_VALUES>> taxesValues = new Dictionary<TAX, List<TAXES_VALUES>>();
//            foreach (var tax in model)
//            {
//                if (tax.NB == ZTaxesNbs.SocialTaamint)
//                {
//                    var tameenStep = carProced.CARPROCEDSTEPS.FirstOrDefault(cps => cps.STEPNB == ProcedBase.ZStepTypes.INSURANCE);
//                    if (tameenStep != null && tameenStep.NOTE != null)
//                    {
//                        int value = -1;
//                        if (int.TryParse(tameenStep.NOTE, out value) && value >= 0)
//                        {
//                            if (!taxesValues.ContainsKey(tax))
//                            {
//                                List<TAXES_VALUES> t = new List<TAXES_VALUES>();
//                                var TV = new TAXES_VALUES()
//                                {
//                                    VALUE = value,
//                                    TAX_TYPE_NB = 1,
//                                    CONDITIONS = new List<CONDITION>()
//                                };
//                                t.Add(TV);
//                                taxesValues.Add(tax, t);
//                            }
//                            else
//                            {
//                                var t = taxesValues.GetValueOrNull(tax);
//                                var TV = new TAXES_VALUES()
//                                {
//                                    VALUE = value,
//                                    TAX_TYPE_NB = 1,
//                                    CONDITIONS = new List<CONDITION>()
//                                };
//                                t.Add(TV);
//                                taxesValues.Remove(tax);
//                                taxesValues.Add(tax, t);
//                            }
//                            //taxesValues.Add(tax, new List<TAXES_VALUES> { new TAXES_VALUES() { VALUE = value, IsRelateToProcedTax = false, } });
//                        }
//                    }
//                    continue;
//                }
//                var conditions = tax.TAX_PERIODS.SelectMany(p => p.TAXES_VALUES).SelectMany(m => m.CONDITIONS)/*.OrderBy(c => c.ORDR)*/.ToList();
//                if (!conditions.Any())
//                {
//                    try
//                    {
//                        if (tax.TAX_PERIODS.ElementAt(0) != null)
//                            taxesValues.Add(tax, tax.TAX_PERIODS.ElementAt(0)?.TAXES_VALUES.ToList());
//                    }
//                    catch (Exception)
//                    {
//                    }

//                    continue;
//                }
//                string carForeignKeyName = "CARNB",
//                    ownerForeignKeyName = "OWNERNB";

//                bool isValid = true;
//                var taxValues = tax.TAX_PERIODS.SelectMany(p => p.TAXES_VALUES).ToList();
//                foreach (var value in taxValues)
//                {
//                    isValid = true;
//                    conditions = value.CONDITIONS/*.OrderBy(c => c.ORDR)*/.ToList();
//                    foreach (var condition in conditions)
//                    {
//                        var cond = condition;
//                        var feild = cond.ZTAX_FEILDS;
//                        if (feild.TABLE_NAME == "CAROWNERS" && !ownerNb.HasValue)
//                        {
//                            break;
//                        }
//                        else if (feild.TABLE_NAME == "CAROWNERS" && ownerNb.HasValue)
//                        {
//                            ownerForeignKeyName = "NB";
//                        }
//                        string pkName = (feild.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                        if (feild.TABLE_NAME == "CARS")
//                        {
//                            pkName = "NB";
//                        }

//                        long? pkValue = (feild.TABLE_NAME == "CAROWNERS") ? ownerNb : car.NB;
//                        if (feild.TABLE_NAME == "CARPROCED_SEIZS" && feild.FEILD_NAME == "DEPTVALE")
//                        {
//                            pkName = "CARPROCEDNB";
//                            pkValue = carProced.NB;
//                        }
//                        var sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                        string query = "";

//                        if (feild.DATA_TYPE == DataTypes.NVARCHAR2 || feild.DATA_TYPE == DataTypes.VARCHAR2)
//                        {
//                            string queryResult = "";
//                            try
//                            {
//                                queryResult = Context.Database.SqlQuery<string>(sql).FirstOrDefault();
//                            }
//                            catch (InvalidOperationException)
//                            {
//                                queryResult = "" + Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
//                            }
//                            catch (SystemException)
//                            {
//                                queryResult = "" + Context.Database.SqlQuery<double?>(sql).FirstOrDefault();
//                            }
//                            catch (Exception)
//                            {
//                                queryResult = "" + Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                            }
//                            finally
//                            {
//                                try
//                                {
//                                    queryResult = "" + Context.Database.SqlQuery<int?>(sql).FirstOrDefault();
//                                }
//                                catch { }
//                            }
//                            if (string.IsNullOrEmpty(queryResult) && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                            {
//                                queryResult = "1";
//                            }

//                            else if (queryResult == null)
//                            {
//                                var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                addError(errors, error);
//                                break;
//                            }
//                            query = queryResult;
//                        }
//                        else if (feild.DATA_TYPE == DataTypes.Char)
//                        {
//                            var queryResult = Context.Database.SqlQuery<char?>(sql).FirstOrDefault();
//                            if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                            {
//                                queryResult = '1';
//                            }
//                            else if (queryResult == null)
//                            {
//                                var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                addError(errors, error);
//                                break;
//                            }
//                            query = "" + queryResult;
//                        }
//                        else if (feild.DATA_TYPE == DataTypes.Number)
//                        {
//                            try
//                            {
//                                var queryResult = Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
//                                if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                {
//                                    queryResult = 1;
//                                }
//                                else if (queryResult == null)
//                                {
//                                    var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                    string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                    //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                    addError(errors, error);
//                                    break;
//                                }
//                                query = "" + queryResult;
//                            }
//                            catch (Exception)
//                            {
//                                try
//                                {
//                                    var queryResult = Context.Database.SqlQuery<double?>(sql).FirstOrDefault();
//                                    if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                    {
//                                        queryResult = 1;
//                                    }
//                                    else if (queryResult == null)
//                                    {
//                                        var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                        string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                        //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                        addError(errors, error);
//                                        break;
//                                    }
//                                    query = "" + queryResult;
//                                }
//                                catch (Exception)
//                                {
//                                    try
//                                    {
//                                        var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                        if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                        {
//                                            queryResult = 1;
//                                        }
//                                        else if (queryResult == null)
//                                        {
//                                            var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                            string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                            //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                            addError(errors, error);
//                                            break;
//                                        }
//                                        query = "" + queryResult;
//                                    }
//                                    catch (Exception)
//                                    {
//                                        try
//                                        {
//                                            var queryResult = Context.Database.SqlQuery<int?>(sql).FirstOrDefault();
//                                            if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                                            {
//                                                queryResult = 1;
//                                            }
//                                            else if (queryResult == null)
//                                            {
//                                                var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                                string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                                //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                                addError(errors, error);
//                                                break;
//                                            }
//                                            query = "" + queryResult;
//                                        }
//                                        catch (Exception)
//                                        {

//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        else if (feild.DATA_TYPE == DataTypes.Long)
//                        {
//                            var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                            if (queryResult == null && feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                            {
//                                queryResult = 1;
//                            }
//                            else if (queryResult == null)
//                            {
//                                var taxName = cond.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1} . الرسم: {2}.)", feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName);
//                                //string error = string.Format("لم يتم تحديد قيمة الواصفة ({0}) للمركبة/المالك ({1})", feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME);
//                                addError(errors, error);
//                                break;
//                            }
//                            query = "" + queryResult;
//                        }
//                        if (!string.IsNullOrEmpty(query))
//                        {
//                            if ((cond?.ZTAX_FEILDS?.TAX_FEILDS_REFFERENCES?.Any()).GetValueOrDefault(false))
//                            {
//                                var firstReff = cond?.ZTAX_FEILDS.TAX_FEILDS_REFFERENCES.ElementAt(0);
//                                sql = string.Format("SELECT {0} FROM {1} WHERE NB={2}", firstReff.REF_FEILD_NAME, firstReff.REF_TABLE_NAME, query);
//                                try
//                                {
//                                    query = Context.Database.SqlQuery<string>(sql).FirstOrDefault();

//                                }
//                                catch (Exception)
//                                {
//                                }
//                            }
//                        }

//                        //else 
//                        if (cond.OPERATOR_NB == (long)MathematicalOperators.Equal)//Equal
//                        {
//                            isValid = cond.VALUE1 == (query + "");
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotEqual)//Not Equal
//                        {
//                            isValid = cond.VALUE1 != (query + "");
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.Between)//between
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1)) && ((decimal.Parse(query) <= decimal.Parse(cond.VALUE2)));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotBetween)//not between
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1)) || ((decimal.Parse(query) > decimal.Parse(cond.VALUE2)));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.GreaterOrEqual)
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.Greater)
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) > decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.LessOrEqual)
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) <= decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }
//                        else if (cond.OPERATOR_NB == (long)MathematicalOperators.Less)
//                        {
//                            try
//                            {
//                                isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1));
//                            }
//                            catch (Exception)
//                            {
//                                isValid = false;
//                            }
//                        }

//                        if (!isValid)
//                        {
//                            break;
//                        }
//                    }
//                    if (isValid)
//                    {
//                        if (!taxesValues.ContainsKey(tax))
//                        {
//                            List<TAXES_VALUES> t = new List<TAXES_VALUES>();
//                            t.Add(value);
//                            taxesValues.Add(tax, t);
//                        }
//                        else
//                        {
//                            var t = taxesValues.GetValueOrNull(tax);
//                            t.Add(value);
//                            taxesValues.Remove(tax);
//                            taxesValues.Add(tax, t);
//                        }
//                    }
//                }
//            }
//            return taxesValues;
//        }

//        public List<ZPROCEDTAX> FilterProcedTaxes(long carNb, long? ownerNb, long? zCarProcedTypeNb, DateTime date)
//        {
//            var zCarProcedType = Context.ZPROCEDTYPS.Where(pc => pc.NB == zCarProcedTypeNb).Include(m => m.ZPROCEDTAXS).FirstOrDefault();
//            if (zCarProcedType == null)
//            {
//                return null;
//            }
//            var allZProcedTaxes = zCarProcedType.ZPROCEDTAXS.ToList();
//            CAR car = Context.CARS.Find(carNb);
//            if (car == null)
//            {
//                return allZProcedTaxes;
//            }

//            List<ZPROCEDTAX> model = new List<ZPROCEDTAX>();
//            foreach (var tax in allZProcedTaxes)
//            {
//                if (!tax.ZPROCEDTAXS_PERIODS.Any() || tax.ZPROCEDTAXS_PERIODS.Any(sp => date >= sp.FROM && date <= sp.TO))
//                {
//                    model.Add(tax);
//                }
//            }

//            List<ZPROCEDTAX> result = new List<ZPROCEDTAX>();
//            foreach (var item in model)
//            {
//                if (!item.ZPROCEDTAXS_PERIODS.Any())
//                {
//                    result.Add(item);
//                    continue;
//                }
//                var conditions = item.ZPROCEDTAXS_PERIODS.SelectMany(p => p.ZPROCEDTAXS_PERIODS_CONDS).ToList();
//                if (!conditions.Any())
//                {
//                    result.Add(item);
//                    continue;
//                }
//                string carForeignKeyName = "CARNB",
//                    ownerForeignKeyName = "OWNERNB";

//                bool isValid = true;
//                foreach (var condition in conditions)
//                {
//                    var cond = condition.ZPROCEDTYP_CONDITIONS;
//                    var feild = cond.ZTAX_FEILDS;
//                    var tablename = feild.TABLE_NAME;
//                    var feildname = feild.FEILD_NAME;
//                    if (feild.TABLE_NAME == "CAROWNERS" && !ownerNb.HasValue)
//                    {
//                        break;
//                    }
//                    else if (feild.TABLE_NAME == "CAROWNERS" && ownerNb.HasValue)
//                    {
//                        ownerForeignKeyName = "NB";
//                    }
//                    string pkName = (feild.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                    if (feild.TABLE_NAME == "CARS")
//                    {
//                        pkName = "NB";
//                    }
//                    long? pkValue = (feild.TABLE_NAME == "CAROWNERS") ? ownerNb : car.NB;
//                    var sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                    string query = "";
//                    if (feild.DATA_TYPE == DataTypes.NVARCHAR2 || feild.DATA_TYPE == DataTypes.VARCHAR2)
//                    {
//                        var queryResult = Context.Database.SqlQuery<string>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = queryResult;
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Char)
//                    {
//                        var queryResult = Context.Database.SqlQuery<char?>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = "" + queryResult;
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Number)
//                    {
//                        try
//                        {
//                            var queryResult = Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
//                            if (queryResult == null)
//                                break;
//                            query = "" + queryResult;
//                        }
//                        catch (Exception)
//                        {
//                            try
//                            {
//                                var queryResult = Context.Database.SqlQuery<double?>(sql).FirstOrDefault();
//                                if (queryResult == null)
//                                    break;
//                                query = "" + queryResult;
//                            }
//                            catch (Exception)
//                            {
//                                try
//                                {
//                                    var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                    if (queryResult == null)
//                                        break;
//                                    query = "" + queryResult;
//                                }
//                                catch (Exception)
//                                {
//                                    try
//                                    {
//                                        var queryResult = Context.Database.SqlQuery<int?>(sql).FirstOrDefault();
//                                        if (queryResult == null)
//                                            break;
//                                        query = "" + queryResult;
//                                    }
//                                    catch (Exception)
//                                    {

//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Long)
//                    {
//                        var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = "" + queryResult;
//                    }
//                    if (!string.IsNullOrEmpty(query))
//                    {
//                        if ((cond?.ZTAX_FEILDS?.TAX_FEILDS_REFFERENCES?.Any()).GetValueOrDefault(false))
//                        {
//                            var firstReff = cond?.ZTAX_FEILDS.TAX_FEILDS_REFFERENCES.ElementAt(0);
//                            sql = string.Format("SELECT {0} FROM {1} WHERE NB={2}", firstReff.REF_FEILD_NAME, firstReff.REF_TABLE_NAME, query);
//                            try
//                            {
//                                query = Context.Database.SqlQuery<string>(sql).FirstOrDefault();

//                            }
//                            catch (Exception)
//                            {
//                            }
//                        }
//                    }
//                    if (cond.OPERATOR_NB == (long)MathematicalOperators.Equal)//Equal
//                    {
//                        isValid = cond.VALUE1 == (query + "");
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotEqual)//Not Equal
//                    {
//                        isValid = cond.VALUE1 != (query + "");
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Between)//between
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1)) && ((decimal.Parse(query) <= decimal.Parse(cond.VALUE2)));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotBetween)//not between
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1)) || ((decimal.Parse(query) > decimal.Parse(cond.VALUE2)));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.GreaterOrEqual)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Greater)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) > decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.LessOrEqual)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) <= decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Less)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }

//                    if (!isValid)
//                    {
//                        break;
//                    }
//                }
//                if (isValid)
//                {
//                    result.Add(item);
//                }
//            }
//            return result;
//        }

//        public List<ZPROCEDSTEP> FilterProcedSteps(long? carNb, long? ownerNb, long? zCarProcedTypeNb, DateTime date)
//        {
//            var zCarProcedType = Context.ZPROCEDTYPS.Where(pc => pc.NB == zCarProcedTypeNb).Include(m => m.ZPROCEDTAXS).FirstOrDefault();
//            if (zCarProcedType == null)
//            {
//                return null;
//            }
//            var allZProcedSteps = zCarProcedType.ZPROCEDSTEPS.ToList();

//            CAR car = Context.CARS.Find(carNb);
//            if (car == null)
//            {
//                return allZProcedSteps;
//            }
//            List<ZPROCEDSTEP> model = new List<ZPROCEDSTEP>();
//            foreach (var step in allZProcedSteps)
//            {
//                if (!step.ZPROCEDSTEPS_PERIODS.Any() || step.ZPROCEDSTEPS_PERIODS.Any(sp => date >= sp.FROM && date <= sp.TO))
//                {
//                    model.Add(step);
//                }
//            }
//            List<ZPROCEDSTEP> result = new List<ZPROCEDSTEP>();
//            foreach (var item in model)
//            {
//                if (!item.ZPROCEDSTEPS_PERIODS.Any())
//                {
//                    result.Add(item);
//                    continue;
//                }
//                var conditions = item.ZPROCEDSTEPS_PERIODS.SelectMany(p => p.ZPROCEDSTEPS_PERIODS_CONDS).ToList();
//                if (!conditions.Any())
//                {
//                    result.Add(item);
//                    continue;
//                }
//                string carForeignKeyName = "CARNB",
//                    ownerForeignKeyName = "OWNERNB";

//                bool isValid = true;
//                foreach (var condition in conditions)
//                {
//                    var cond = condition.ZPROCEDTYP_CONDITIONS;
//                    var feild = cond.ZTAX_FEILDS;
//                    if (feild.TABLE_NAME == "CAROWNERS" && !ownerNb.HasValue)
//                    {
//                        break;
//                    }
//                    else if (feild.TABLE_NAME == "CAROWNERS" && ownerNb.HasValue)
//                    {
//                        ownerForeignKeyName = "NB";
//                    }
//                    string pkName = (feild.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                    if (feild.TABLE_NAME == "CARS")
//                    {
//                        pkName = "NB";
//                    }
//                    long? pkValue = (feild.TABLE_NAME == "CAROWNERS") ? ownerNb : car.NB;
//                    var sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                    string query = "";
//                    if (feild.DATA_TYPE == DataTypes.NVARCHAR2 || feild.DATA_TYPE == DataTypes.VARCHAR2)
//                    {
//                        var queryResult = Context.Database.SqlQuery<string>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = queryResult;
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Char)
//                    {
//                        var queryResult = Context.Database.SqlQuery<char?>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = "" + queryResult;
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Number)
//                    {
//                        try
//                        {
//                            var queryResult = Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
//                            if (queryResult == null)
//                                break;
//                            query = "" + queryResult;
//                        }
//                        catch (Exception)
//                        {
//                            try
//                            {
//                                var queryResult = Context.Database.SqlQuery<double?>(sql).FirstOrDefault();
//                                if (queryResult == null)
//                                    break;
//                                query = "" + queryResult;
//                            }
//                            catch (Exception)
//                            {
//                                try
//                                {
//                                    var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                    if (queryResult == null)
//                                        break;
//                                    query = "" + queryResult;
//                                }
//                                catch (Exception)
//                                {
//                                    try
//                                    {
//                                        var queryResult = Context.Database.SqlQuery<int?>(sql).FirstOrDefault();
//                                        if (queryResult == null)
//                                            break;
//                                        query = "" + queryResult;
//                                    }
//                                    catch (Exception)
//                                    {

//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Long)
//                    {
//                        var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = "" + queryResult;
//                    }
//                    if (!string.IsNullOrEmpty(query))
//                    {
//                        if ((cond?.ZTAX_FEILDS?.TAX_FEILDS_REFFERENCES?.Any()).GetValueOrDefault(false))
//                        {
//                            var firstReff = cond?.ZTAX_FEILDS.TAX_FEILDS_REFFERENCES.ElementAt(0);
//                            sql = string.Format("SELECT {0} FROM {1} WHERE NB={2}", firstReff.REF_FEILD_NAME, firstReff.REF_TABLE_NAME, query);
//                            try
//                            {
//                                query = Context.Database.SqlQuery<string>(sql).FirstOrDefault();

//                            }
//                            catch (Exception)
//                            {
//                            }
//                        }
//                    }
//                    if (cond.OPERATOR_NB == (long)MathematicalOperators.Equal)//Equal
//                    {
//                        isValid = cond.VALUE1 == (query + "");
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotEqual)//Not Equal
//                    {
//                        isValid = cond.VALUE1 != (query + "");
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Between)//between
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1)) && ((decimal.Parse(query) <= decimal.Parse(cond.VALUE2)));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotBetween)//not between
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1)) || ((decimal.Parse(query) > decimal.Parse(cond.VALUE2)));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.GreaterOrEqual)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Greater)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) > decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.LessOrEqual)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) <= decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Less)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }

//                    if (!isValid)
//                    {
//                        break;
//                    }
//                }
//                if (isValid)
//                {
//                    result.Add(item);
//                }
//            }
//            return result;
//        }


//        public List<ZPROCEDDOC> FilterProcedDocuments(CAR car, long? ownerNb, long? zCarProcedTypeNb, DateTime date)
//        {
//            date = date.Date;
//            var zCarProcedType = Context.ZPROCEDTYPS.Where(pc => pc.NB == zCarProcedTypeNb).Include(m => m.ZPROCEDTAXS).FirstOrDefault();
//            if (zCarProcedType == null)
//            {
//                return null;
//            }
//            var allZProcedDocs = zCarProcedType.ZPROCEDSTEPS.SelectMany(m => m.ZPROCEDDOCS).Distinct().ToList();
//            List<ZPROCEDDOC> result = new List<ZPROCEDDOC>();
//            foreach (var item in allZProcedDocs)
//            {
//                if (item.DOCNB == 33)
//                {

//                }
//                var period = item.ZPROCEDDOCS_PERIODS.Where(p => (p.TO != null && p.FROM <= date && date <= p.TO) || (p.TO == null && p.FROM <= date)).ToList();
//                if (!period.Any())
//                {
//                    result.Add(item);
//                    continue;
//                }
//                var conditions = period.SelectMany(p => p.ZPROCEDDOCS_PERIODS_CONDS).ToList();
//                if (!conditions.Any())
//                {
//                    result.Add(item);
//                    continue;
//                }
//                string carForeignKeyName = "CARNB",
//                    ownerForeignKeyName = "OWNERNB";

//                bool isValid = true;
//                foreach (var condition in conditions)
//                {

//                    var cond = condition.ZPROCEDTYP_CONDITIONS;

//                    var feild = cond.ZTAX_FEILDS;
//                    if (feild.TABLE_NAME == "CAROWNERS" && !ownerNb.HasValue)
//                    {
//                        break;
//                    }
//                    else if (feild.TABLE_NAME == "CAROWNERS" && ownerNb.HasValue)
//                    {
//                        ownerForeignKeyName = "NB";
//                    }
//                    string pkName = (feild.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                    if (feild.TABLE_NAME == "CARS")
//                    {
//                        pkName = "NB";
//                    }
//                    long? pkValue = (feild.TABLE_NAME == "CAROWNERS") ? ownerNb : car.NB;
//                    var sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                    string query = "";
//                    if (feild.DATA_TYPE == DataTypes.NVARCHAR2 || feild.DATA_TYPE == DataTypes.VARCHAR2)
//                    {
//                        sql = string.Format("SELECT TO_CHAR({0}) FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                        var queryResult = Context.Database.SqlQuery<string>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = queryResult;
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Char)
//                    {
//                        var queryResult = Context.Database.SqlQuery<char?>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = "" + queryResult;
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Number)
//                    {
//                        try
//                        {
//                            var queryResult = Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
//                            if (queryResult == null)
//                                break;
//                            query = "" + queryResult;
//                        }
//                        catch (Exception)
//                        {
//                            try
//                            {
//                                var queryResult = Context.Database.SqlQuery<double?>(sql).FirstOrDefault();
//                                if (queryResult == null)
//                                    break;
//                                query = "" + queryResult;
//                            }
//                            catch (Exception)
//                            {
//                                try
//                                {
//                                    var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                    if (queryResult == null)
//                                        break;
//                                    query = "" + queryResult;
//                                }
//                                catch (Exception)
//                                {
//                                    try
//                                    {
//                                        var queryResult = Context.Database.SqlQuery<int?>(sql).FirstOrDefault();
//                                        if (queryResult == null)
//                                            break;
//                                        query = "" + queryResult;
//                                    }
//                                    catch (Exception)
//                                    {

//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else if (feild.DATA_TYPE == DataTypes.Long)
//                    {
//                        var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                        if (queryResult == null)
//                            break;
//                        query = "" + queryResult;
//                    }
//                    if (!string.IsNullOrEmpty(query))
//                    {
//                        if ((cond?.ZTAX_FEILDS?.TAX_FEILDS_REFFERENCES?.Any()).GetValueOrDefault(false))
//                        {
//                            var firstReff = cond?.ZTAX_FEILDS.TAX_FEILDS_REFFERENCES.ElementAt(0);
//                            sql = string.Format("SELECT {0} FROM {1} WHERE NB={2}", firstReff.REF_FEILD_NAME, firstReff.REF_TABLE_NAME, query);
//                            try
//                            {
//                                query = Context.Database.SqlQuery<string>(sql).FirstOrDefault();

//                            }
//                            catch (Exception)
//                            {
//                            }
//                        }
//                    }
//                    else
//                    {
//                        isValid = false;
//                        break;
//                    }
//                    if (cond.OPERATOR_NB == (long)MathematicalOperators.Equal)//Equal
//                    {
//                        isValid = cond.VALUE1 == (query + "");
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotEqual)//Not Equal
//                    {
//                        isValid = cond.VALUE1 != (query + "");
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Between)//between
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1)) && ((decimal.Parse(query) <= decimal.Parse(cond.VALUE2)));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.NotBetween)//not between
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1)) || ((decimal.Parse(query) > decimal.Parse(cond.VALUE2)));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.GreaterOrEqual)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) >= decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Greater)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) > decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.LessOrEqual)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) <= decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }
//                    else if (cond.OPERATOR_NB == (long)MathematicalOperators.Less)
//                    {
//                        try
//                        {
//                            isValid = (decimal.Parse(query) < decimal.Parse(cond.VALUE1));
//                        }
//                        catch (Exception)
//                        {
//                            isValid = false;
//                        }
//                    }

//                    if (!isValid)
//                    {
//                        break;
//                    }
//                }
//                if (isValid)
//                {
//                    result.Add(item);
//                }
//            }
//            return result;
//        }

//        #region GetValue
//        public double? GetActualValue(TAXES_VALUES taxValue, CARPROCED carProced, List<KeyValuePair<TAX, List<TAXES_VALUES>>> procedTaxes, List<long> carOwnersIds, ref string Errors, bool isRecursive = false)
//        {
//            if (taxValue.DescriptionNotMapped == null)
//            {
//                taxValue.DescriptionNotMapped = "";
//            }
//            List<string> err = new List<string>();
//            if (taxValue?.TAX_TYPE_NB == 1 || taxValue?.TAX_TYPE_NB == 4) //ثابتة أومن ملف
//            {
//                if (isRecursive)
//                {
//                    taxValue.DescriptionNotMapped += "من الرسم ثابتة = " + taxValue.VALUE + " ، الشروط: {" + getTaxValueConditions(taxValue) + "}";
//                }
//                else
//                {
//                    taxValue.DescriptionNotMapped += "ثابتة = " + taxValue.VALUE + " ، الشروط: {" + getTaxValueConditions(taxValue) + "}";
//                }
//                return taxValue.VALUE;
//            }
//            #region نسبة
//            else if (taxValue?.TAX_TYPE_NB == 2) //نسبة
//            {
//                var errorIndex = 1;
//                var procedTaxesIds = procedTaxes.Where(m => m.Key.NB != taxValue.TAX_PERIODS.TAX_NB).Select(m => m.Key.NB).ToList();
//                var relatedTaxes = taxValue.TAXES_CONNECTIONS.Where(m => m.TAX_VALUE_NB == taxValue.NB).ToList()
//                                                             .Where(t => t.RELATED_TAX_NB.HasValue && procedTaxesIds.Contains(t.RELATED_TAX_NB.Value)).ToList();

//                var relatedAttr = taxValue.TAXES_CONNECTIONS.Where(m => m.TAX_VALUE_NB == taxValue.NB).ToList()
//                                                             .Where(t => !t.RELATED_TAX_NB.HasValue).ToList();

//                var relatedTaxesIds = relatedTaxes.Select(tc => tc.RELATED_TAX_NB).ToList();
//                var tmp = procedTaxes.Where(m => relatedTaxesIds.Contains(m.Key.NB)).ToList();
//                var relatedTaxesValues = tmp.SelectMany(m => m.Value).DistinctBy(m => m.TAXPERIOD_NB).ToList();
//                double? accumaletedValuesOfRelatedTaxes = 0;


//                foreach (var tax in relatedTaxesValues)
//                {
//                    accumaletedValuesOfRelatedTaxes += GetActualValue(tax, carProced, tmp, carOwnersIds, ref Errors, true);
//                }

//                string relatedFeildValue = string.Empty;
//                foreach (var item in relatedAttr)
//                {
//                    var feild = item.ZTAX_FEILDS;
//                    if (feild == null)
//                    {

//                    }
//                    string pkName = (feild.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                    if (feild.TABLE_NAME == "CARTAXS" && feild.FEILD_NAME == "TOTVAL")
//                    {
//                        taxValue.IsRelateToProcedTax = true;
//                        return 0d;
//                    }
//                    else if (feild.TABLE_NAME == "CARS")
//                    {
//                        pkName = "NB";
//                    }
//                    else if (feild.TABLE_NAME == "CARPROCED_SEIZS" && feild.FEILD_NAME == "DEPTVALE")
//                    {
//                        pkName = "CARPROCEDNB";
//                        var pkValue = carProced.NB;
//                        var sql = string.Format("SELECT NVL(SUM({0}),0) FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                        relatedFeildValue = getFeildValue(feild, sql);
//                        if (string.IsNullOrEmpty(relatedFeildValue))
//                        {
//                            var taxName = item.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                            Errors += string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2}) - الرسم: {3}", errorIndex++, feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName) + htmlNewLine;
//                        }
//                        else
//                        {
//                            try
//                            {
//                                accumaletedValuesOfRelatedTaxes += double.Parse(relatedFeildValue);
//                            }
//                            catch (Exception)
//                            {
//                            }
//                        }
//                    }

//                    else if (feild.TABLE_NAME == "CAROWNERS")
//                    {
//                        foreach (var carOwnerId in carOwnersIds)
//                        {
//                            string pkValue = carOwnerId + "";
//                            var sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                            relatedFeildValue = getFeildValue(feild, sql);

//                            if (string.IsNullOrEmpty(relatedFeildValue))
//                            {
//                                var taxName = item.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                Errors += string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2}) - الرسم: {3}", errorIndex++, feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName) + htmlNewLine;
//                            }
//                            else
//                            {
//                                try
//                                {
//                                    accumaletedValuesOfRelatedTaxes += double.Parse(relatedFeildValue);
//                                }
//                                catch (Exception)
//                                {
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {

//                        string pkValue = carProced.CARNB + "";
//                        if (feild.TABLE_NAME == "CARPROCED_SRCS")
//                        {
//                            pkName = "CARPROCEDNB";
//                            pkValue = "" + carProced.NB;
//                        }
//                        var sql = string.Format("SELECT NVL(SUM({0}),0) FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                        relatedFeildValue = getFeildValue(feild, sql);

//                        if (string.IsNullOrEmpty(relatedFeildValue))
//                        {
//                            var taxName = item.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                            Errors += string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2}) - الرسم: {3}", errorIndex++, feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName) + htmlNewLine;
//                            //Errors +=string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2})", errorIndex++, feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME)+htmlNewLine;
//                        }
//                        else
//                        {
//                            try
//                            {
//                                accumaletedValuesOfRelatedTaxes += double.Parse(relatedFeildValue);
//                            }
//                            catch (Exception)
//                            {
//                            }
//                        }
//                    }

//                }

//                var percent = (taxValue.VALUE.GetValueOrDefault(0) / (double)taxValue.DENOMINATOR);
//                taxValue.DescriptionNotMapped += "نسبة = " + percent + " x " + accumaletedValuesOfRelatedTaxes + " ، الشروط: {" + getTaxValueConditions(taxValue) + "}";
//                return percent * accumaletedValuesOfRelatedTaxes;
//            }
//            #endregion
//            else if (taxValue?.TAX_TYPE_NB == 3) // متغيرة
//            {
//                bool hasErrors = false;
//                var procedTaxesIds = procedTaxes.Where(m => m.Key.NB != taxValue.TAX_PERIODS.TAX_NB).Select(m => m.Key.NB).ToList();
//                var relatedTaxes = taxValue.TAXES_CONNECTIONS.Where(m => m.TAX_VALUE_NB == taxValue.NB).ToList();
//                double? value = taxValue.VALUE;
//                var constVal = relatedTaxes.FirstOrDefault(m => m.IS_PERCENTAGE_OF == false && m.CONST_VALUE.HasValue && m.ZFEILD_NB == null);

//                if (relatedTaxes.Any(m => m.IS_PERCENTAGE_OF == true))
//                {
//                    taxValue.DescriptionNotMapped += "متغيرة = " + taxValue.VALUE + "/" + taxValue.DENOMINATOR + " ، الشروط: {" + getTaxValueConditions(taxValue) + "}";
//                    #region Percentage Changable Tax
//                    var percentOf = relatedTaxes.FirstOrDefault(m => m.IS_PERCENTAGE_OF == true);
//                    var feild = percentOf.ZTAX_FEILDS;
//                    string pkName = (feild.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                    long? pkValue = (feild.TABLE_NAME == "CAROWNERS") ? carProced.OWNERNB : carProced.CARNB;

//                    if (feild.TABLE_NAME == "CARS")
//                    {
//                        pkName = "NB";
//                    }
//                    var sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);

//                    if (feild.TABLE_NAME == "CARPROCED_SRCS")
//                    {
//                        pkName = "CARPROCEDNB";
//                        pkValue = carProced.NB;
//                        sql = string.Format("SELECT NVL(SUM({0}), 0) FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                    }


//                    if (feild.TABLE_NAME == "CARVALUES")
//                    {
//                        pkName = "CARPROCEDNB";
//                        pkValue = carProced.NB;
//                        sql = string.Format("SELECT NVL(SUM({0}*FACTOR),0) FROM {1} WHERE {2}={3} AND EDATE IS NULL", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                    }
//                    if (feild.TABLE_NAME == "CARPROCEDS" && (feild.FEILD_NAME == "TABCOUNT" || feild.FEILD_NAME == "FOLDER" || feild.FEILD_NAME == "SELLTAXFACTOR"))
//                    {
//                        pkName = "NB";
//                        pkValue = carProced.NB;
//                        sql = string.Format("SELECT NVL({0}, 1) FROM {1} WHERE {2}={3}", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                    }
//                    string relatedFeildValue = string.Empty;
//                    relatedFeildValue = getFeildValue(feild, sql);
//                    var errorIndex = 1;
//                    if (string.IsNullOrEmpty(relatedFeildValue))
//                    {
//                        //{0}{1}
//                        hasErrors = true;
//                        var taxName = percentOf.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                        Errors += string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2}) - الرسم: {3}", errorIndex++, feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName) + htmlNewLine;
//                        //Errors+=string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2})", errorIndex++, feild.DESCRIPTION, feild.TABLE_NAME+" -> "+feild.FEILD_NAME)+htmlNewLine;
//                    }
//                    var relatedFeilds = relatedTaxes.Where(rt => (rt.IS_PERCENTAGE_OF == null || rt.IS_PERCENTAGE_OF == false)/*Not Percentage Rows*/ && (rt.CONST_VALUE == null)/*Not Const Value Row*/).ToList();
//                    List<string> relatedFeildsValues = new List<string>();
//                    if (relatedFeilds.Any())
//                    {
//                        foreach (var item in relatedFeilds)
//                        {
//                            pkName = (item.ZTAX_FEILDS.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                            pkValue = (item.ZTAX_FEILDS.TABLE_NAME == "CAROWNERS") ? carProced.OWNERNB : carProced.CARNB;


//                            if (item.ZTAX_FEILDS.TABLE_NAME == "CARS")
//                            {
//                                pkName = "NB";
//                            }
//                            sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", item.ZTAX_FEILDS.FEILD_NAME, item.ZTAX_FEILDS.TABLE_NAME, pkName, pkValue);
//                            if (item.ZTAX_FEILDS.TABLE_NAME == "CARPROCED_SRCS")
//                            {
//                                pkName = "CARPROCEDNB";
//                                pkValue = carProced.NB;
//                                sql = string.Format("SELECT NVL(SUM({0}), 0) FROM {1} WHERE {2}={3}", item.ZTAX_FEILDS.FEILD_NAME, item.ZTAX_FEILDS.TABLE_NAME, pkName, pkValue);
//                            }
//                            if (item.ZTAX_FEILDS.TABLE_NAME == "CARPROCEDS" && (item.ZTAX_FEILDS.FEILD_NAME == "TABCOUNT" || item.ZTAX_FEILDS.FEILD_NAME == "FOLDER" || item.ZTAX_FEILDS.FEILD_NAME == "SELLTAXFACTOR"))
//                            {
//                                pkName = "NB";
//                                pkValue = carProced.NB;
//                                sql = string.Format("SELECT NVL({0}, 1) FROM {1} WHERE {2}={3}", item.ZTAX_FEILDS.FEILD_NAME, item.ZTAX_FEILDS.TABLE_NAME, pkName, pkValue);
//                            }
//                            if (feild.TABLE_NAME == "CARVALUES")
//                            {
//                                pkName = "CARPROCEDNB";
//                                pkValue = carProced.NB;
//                                sql = string.Format("SELECT NVL(SUM({0}*FACTOR), 0) FROM {1} WHERE {2}={3} AND EDATE IS NULL", feild.FEILD_NAME, feild.TABLE_NAME, pkName, pkValue);
//                            }
//                            var relatedFeildsValue = getFeildValue(item.ZTAX_FEILDS, sql);
//                            if (string.IsNullOrEmpty(relatedFeildsValue))
//                            {
//                                hasErrors = true;
//                                var taxName = item.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                Errors += string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2}) - الرسم: {3}", errorIndex++, feild.DESCRIPTION, feild.TABLE_NAME + " -> " + feild.FEILD_NAME, taxName) + htmlNewLine;
//                                //Errors +=string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2})", errorIndex++, item.ZTAX_FEILDS.DESCRIPTION, item.ZTAX_FEILDS.TABLE_NAME+" -> "+item.ZTAX_FEILDS.FEILD_NAME)+htmlNewLine;
//                            }
//                            else
//                            {
//                                relatedFeildsValues.Add(relatedFeildsValue);
//                            }
//                        }
//                    }
//                    if (!hasErrors && constVal != null && constVal.CONST_VALUE.HasValue)
//                    {
//                        double relatedFeildValue_;
//                        double multiplicityBraceValue = (double)constVal.CONST_VALUE.Value;
//                        double percentOperand = 0;
//                        if (double.TryParse(relatedFeildValue, out relatedFeildValue_))
//                        {
//                            percentOperand = relatedFeildValue_ * (taxValue.VALUE.Value / (double)taxValue.DENOMINATOR);
//                            foreach (var item in relatedFeildsValues)
//                            {
//                                try
//                                {
//                                    multiplicityBraceValue += double.Parse(item);
//                                }
//                                catch (Exception)
//                                {
//                                    hasErrors = true;
//                                    Errors += string.Format("{0}- لا يمكن قسر قيمة ({1}) لقيمة عددية", errorIndex++, item) + htmlNewLine;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            hasErrors = true;
//                            Errors += string.Format("{0}- لا يمكن قسر قيمة ({1}) لقيمة عددية", errorIndex++, relatedFeildValue) + htmlNewLine;
//                        }
//                        if (!hasErrors)
//                        {
//                            return (multiplicityBraceValue * percentOperand);
//                        }
//                    }
//                    else
//                    {
//                        double relatedFeildValue_;
//                        double multiplicityBraceValue = 0;
//                        double percentOperand = 0;
//                        if (double.TryParse(relatedFeildValue, out relatedFeildValue_))
//                        {
//                            percentOperand = relatedFeildValue_ * (taxValue.VALUE.Value / (double)taxValue.DENOMINATOR);
//                            foreach (var item in relatedFeildsValues)
//                            {
//                                try
//                                {
//                                    multiplicityBraceValue += double.Parse(item);
//                                }
//                                catch (Exception)
//                                {
//                                    hasErrors = true;
//                                    Errors += string.Format("{0}- لا يمكن قسر قيمة ({1}) لقيمة عددية", errorIndex++, item) + htmlNewLine;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            hasErrors = true;
//                            Errors += string.Format("{0}- لا يمكن قسر قيمة ({1}) لقيمة عددية", errorIndex++, relatedFeildValue) + htmlNewLine;
//                        }
//                        if (!hasErrors)
//                        {
//                            return (multiplicityBraceValue * percentOperand);
//                        }
//                    }
//                    #endregion
//                }
//                else
//                {
//                    #region Fixed Changable Tax
//                    int errorIndex = 1;
//                    var relatedFeilds = relatedTaxes.Where(rt => (rt.IS_PERCENTAGE_OF == null || rt.IS_PERCENTAGE_OF == false)/*Not Percentage Rows*/ && (rt.CONST_VALUE == null)/*Not Const Value Row*/).ToList();
//                    List<string> relatedFeildsValues = new List<string>();
//                    if (relatedFeilds.Any())
//                    {
//                        foreach (var item in relatedFeilds)
//                        {
//                            string pkName = (item.ZTAX_FEILDS.TABLE_NAME == "CAROWNERS") ? ownerForeignKeyName : carForeignKeyName;
//                            if (item.ZTAX_FEILDS.TABLE_NAME == "CARS")
//                            {
//                                pkName = "NB";
//                            }

//                            long? pkValue = (item.ZTAX_FEILDS.TABLE_NAME == "CAROWNERS") ? carProced.OWNERNB : carProced.CARNB;
//                            string sql = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", item.ZTAX_FEILDS.FEILD_NAME, item.ZTAX_FEILDS.TABLE_NAME, pkName, pkValue);
//                            if (item.ZTAX_FEILDS.TABLE_NAME == "CARVALUES")
//                            {
//                                pkName = "CARPROCEDNB";
//                                pkValue = carProced.NB;
//                                sql = string.Format("SELECT NVL(SUM({0}*FACTOR),0) FROM {1} WHERE {2}={3} AND EDATE IS NULL", item.ZTAX_FEILDS.FEILD_NAME, item.ZTAX_FEILDS.TABLE_NAME, pkName, pkValue);
//                            }
//                            else if (item.ZTAX_FEILDS.TABLE_NAME == "CARPROCED_SRCS")
//                            {
//                                pkName = "CARPROCEDNB";
//                                pkValue = carProced.NB;
//                                sql = string.Format("SELECT NVL(SUM({0}), 0) FROM {1} WHERE {2}={3}", item.ZTAX_FEILDS.FEILD_NAME, item.ZTAX_FEILDS.TABLE_NAME, pkName, pkValue);
//                            }
//                            else if (item.ZTAX_FEILDS.TABLE_NAME == "CARPROCEDS" && (item.ZTAX_FEILDS.FEILD_NAME == "TABCOUNT" || item.ZTAX_FEILDS.FEILD_NAME == "FOLDER" || item.ZTAX_FEILDS.FEILD_NAME == "SELLTAXFACTOR"))
//                            {
//                                pkName = "NB";
//                                pkValue = carProced.NB;
//                                sql = string.Format("SELECT NVL({0}, 1) FROM {1} WHERE {2}={3}", item.ZTAX_FEILDS.FEILD_NAME, item.ZTAX_FEILDS.TABLE_NAME, pkName, pkValue);
//                            }
//                            var relatedFeildsValue = getFeildValue(item.ZTAX_FEILDS, sql);
//                            if (string.IsNullOrEmpty(relatedFeildsValue))
//                            {
//                                hasErrors = true;
//                                var taxName = item.TAXES_VALUES?.TAX_PERIODS?.TAX?.NAME ?? "~";
//                                Errors += string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2}) - الرسم: {3}", errorIndex++, item.ZTAX_FEILDS.DESCRIPTION, item.ZTAX_FEILDS.TABLE_NAME + " -> " + item.ZTAX_FEILDS.FEILD_NAME, taxName) + htmlNewLine;
//                                //Errors+=string.Format("{0}- لم يتم تحديد قيمة الواصفة ({1}) للمركبة/المالك ({2})", errorIndex++, item.ZTAX_FEILDS.DESCRIPTION, item.ZTAX_FEILDS.TABLE_NAME+" -> "+item.ZTAX_FEILDS.FEILD_NAME)+htmlNewLine;
//                            }
//                            else
//                            {
//                                relatedFeildsValues.Add(relatedFeildsValue);
//                            }
//                        }
//                    }
//                    if (!hasErrors && constVal != null && constVal.CONST_VALUE.HasValue)
//                    {
//                        double multiplicityBraceValue = (double)constVal.CONST_VALUE.Value;
//                        double operand = taxValue.VALUE.Value / (taxValue.DENOMINATOR != 0 ? taxValue.DENOMINATOR : 1);
//                        foreach (var item in relatedFeildsValues)
//                        {
//                            try
//                            {
//                                multiplicityBraceValue += double.Parse(item);
//                            }
//                            catch (Exception)
//                            {
//                                hasErrors = true;
//                                Errors += string.Format("{0}- لا يمكن قسر قيمة ({1}) لقيمة عددية", errorIndex++, item) + htmlNewLine;
//                            }
//                        }
//                        if (!hasErrors)
//                        {
//                            return (multiplicityBraceValue * operand);
//                        }
//                    }
//                    else if (!hasErrors)
//                    {
//                        double multiplicityBraceValue = 0;
//                        double operand = ((double)taxValue.VALUE.Value) / (taxValue.DENOMINATOR != 0 ? taxValue.DENOMINATOR : 1);
//                        foreach (var item in relatedFeildsValues)
//                        {
//                            try
//                            {
//                                multiplicityBraceValue += double.Parse(item);
//                            }
//                            catch (Exception)
//                            {
//                                hasErrors = true;
//                                Errors += string.Format("{0}- لا يمكن قسر قيمة ({1}) لقيمة عددية", errorIndex++, item) + htmlNewLine;
//                            }
//                        }
//                        if (!hasErrors)
//                        {
//                            return (multiplicityBraceValue * operand);
//                        }
//                    }
//                    #endregion
//                }
//            }
//            return 0;
//        }

//        private string getFeildValue(ZTAX_FEILDS feild, string sql)
//        {
//            string relatedFeildValue = "";
//            if (queriesValues.ContainsKey(sql))
//            {
//                queriesValues.TryGetValue(sql, out relatedFeildValue);
//            }
//            else
//            {
//                if (feild.DATA_TYPE == DataTypes.NVARCHAR2 || feild.DATA_TYPE == DataTypes.VARCHAR2)
//                {
//                    var queryResult = Context.Database.SqlQuery<string>(sql).FirstOrDefault();
//                    if (queryResult != null)
//                        relatedFeildValue = queryResult;
//                }
//                else if (feild.DATA_TYPE == DataTypes.Char)
//                {
//                    var queryResult = Context.Database.SqlQuery<char?>(sql).FirstOrDefault();
//                    if (queryResult != null)
//                        relatedFeildValue = "" + queryResult;
//                }
//                else if (feild.DATA_TYPE == DataTypes.Number)
//                {
//                    try
//                    {
//                        var queryResult = Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
//                        if (queryResult != null)
//                            relatedFeildValue = "" + queryResult;
//                    }
//                    catch (Exception)
//                    {
//                        try
//                        {
//                            var queryResult = Context.Database.SqlQuery<double?>(sql).FirstOrDefault();
//                            if (queryResult != null)
//                                relatedFeildValue = "" + queryResult;
//                        }
//                        catch (Exception)
//                        {
//                            try
//                            {
//                                var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                                if (queryResult != null)
//                                    relatedFeildValue = "" + queryResult;
//                            }
//                            catch (Exception)
//                            {
//                                try
//                                {
//                                    var queryResult = Context.Database.SqlQuery<int?>(sql).FirstOrDefault();
//                                    if (queryResult != null)
//                                        relatedFeildValue = "" + queryResult;
//                                }
//                                catch (Exception)
//                                {

//                                }
//                            }
//                        }
//                    }
//                }
//                else if (feild.DATA_TYPE == DataTypes.Long)
//                {
//                    var queryResult = Context.Database.SqlQuery<long?>(sql).FirstOrDefault();
//                    if (queryResult != null)
//                        relatedFeildValue = "" + queryResult;
//                }
//                try
//                {
//                    queriesValues.Add(sql, relatedFeildValue);
//                }
//                catch (Exception)
//                {
//                }
//            }
//            return relatedFeildValue;
//        }

//        private string getTaxValueConditions(TAXES_VALUES taxValue)
//        {
//            var str_conditions = "";
//            var conditions = taxValue.CONDITIONS/*.OrderBy(c => c.ORDR)*/.ToList();
//            if (conditions != null && conditions.Count() > 0)
//            {
//                foreach (CONDITION condition in conditions)
//                {

//                    string feildName = condition.ZTAX_FEILDS?.DESCRIPTION;
//                    if (condition?.OPERATOR?.OPERANDS_COUNT == 2)
//                    {
//                        str_conditions += "(" + feildName + " " + condition?.OPERATOR.NAME_AR + " " + condition?.VALUE1 + HelperLib.ControlLabelsAR.AND + condition.VALUE2 + "), ";
//                    }
//                    else
//                    {
//                        str_conditions += "(" + feildName + " " + condition?.OPERATOR?.NAME_AR + " " + condition.VALUE1 + "), ";
//                    }
//                }
//                str_conditions = str_conditions.Substring(0, str_conditions.Length - 2);
//                str_conditions += " للفترة (" + taxValue.TAX_PERIODS.FROM_DATE?.ToString("yyyy/MM/dd") + " - " + taxValue.TAX_PERIODS.TO_DATE?.ToString("yyyy/MM/dd");
//            }
//            else
//            {
//                str_conditions = "لا يوجد شروط";
//            }
//            return str_conditions;
//        }
//        #endregion
//    }
//}