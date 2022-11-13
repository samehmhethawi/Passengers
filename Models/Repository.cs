using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AppsManagerDataAccess.DAL
{


    public class Repository<T> where T : class
    {
        #region public Attributes
        #endregion

        #region private Attributes
        private DbContext db;
        #endregion

        #region Constructors
        public Repository(DbContext db)
        {
            this.db = db;
        }
        #endregion

        #region Public Methods
        public List<T> GetList(Func<T, bool> condition = null)
        {
            return (condition != null) ? db.Set<T>().Where(condition).ToList() : db.Set<T>().ToList();
        }

        public SelectList GetSelectList(Func<T, bool> condition = null, string dataValueField = "NB", string dataTextField = "NAME", object selectedValue = null)
        {
            var items = GetList(condition);
            if (items == null)
            {
                items = new List<T>();
            }
            return new SelectList(items, dataValueField, dataTextField, selectedValue);
        }

        public List<SelectListItem> GetSelectListItems(Func<T, bool> condition = null, string dataValueField = "NB", string dataTextField = "NAME", object selectedValue = null)
        {
            List<SelectListItem> ii = new List<SelectListItem>();
            //var items = GetList(condition);
            //if (items == null)
            //{
            //    items = new List<T>();
            //}
            var items = GetList(condition);
            if (items == null)
            {
                items = new List<T>();
            }

            var item = new SelectListItem
            {
                Text = dataTextField,
                Value = dataTextField
            };

            ii.Add(item);
            return ii;

        }


        public T GetById(int? id)
        {
            return db.Set<T>().Find(id);
        }

        public T Insert(T newObj, bool autoSave = true)
        {
            var added = db.Set<T>().Add(newObj);
            if (autoSave)
            {
                db.SaveChanges();
            }
            return added;
        }


        public void Delete(int id, bool autoSave = true)
        {
            var obj = this.GetById(id);
            this.Delete(obj, autoSave);
        }

        public void Delete(T obj, bool autoSave = true)
        {
            db.Set<T>().Attach(obj);
            db.Set<T>().Remove(obj);
            if (autoSave)
            {
                db.SaveChanges();
            }
        }

        public void Update(T newObj, bool autoSave = true)
        {
            db.Entry(newObj).State = EntityState.Modified;
            if (autoSave)
            {
                db.SaveChanges();
            }
        }

        // TODO:
        public override string ToString()
        {
            return "";
        }
        #endregion

        #region Private Methods
        #endregion
    }
}