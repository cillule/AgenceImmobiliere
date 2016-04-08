using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.Tools;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.DataAccess
{
    [DataContract]
    public abstract class SearchCriteria : BaseNotifyPropertyChanged
    {
        protected internal class Query
        {
            internal string SqlQuery { get; set; }
            internal string SqlCountQuery { get; set; }
            internal long? CurrentPage { get; set; }
            internal long PagesCount { get; set; }
            internal long? ItemsCountOnPage { get; set; }
            internal long SelectedItemsCount { get; set; }
            internal long TotalItemsCount { get; set; }
            internal long CurrentItemIndex { get; set; }

            private Tools.ErrorsList _errors;
            internal Tools.ErrorsList Errors { get { return _errors; } }

            internal Query()
            {
                _errors = new Tools.ErrorsList();
            }
        }

        protected long _id;
        protected ObservableCollection<Sort> _tris;


        [DataMember]
        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        [DataMember]
        public ObservableCollection<Sort> Tris
        {
            get { return _tris; }
            private set { _tris = value; }
        }
        public Sort TriPrincipal
        {
            get
            {
                if (_tris == null)
                    return null;
                else if (_tris.Count <= 0)
                    _tris.Add(new Sort(Enums.ChampElement.Id, Enums.OrdreTri.Montant));

                return _tris[0];
            }
        }
        public abstract bool CriteresVides { get; }
        public abstract Array ListeChamps { get; }


        protected SearchCriteria()
        {
            ClearFilters();
        }
        protected SearchCriteria(SearchCriteria source)
        {
            CloneFilters(source);
        }

        public virtual void ClearFilters()
        {
            this._id = -1;
            this._tris = new ObservableCollection<Sort>();
            this._tris.Add(new Sort());
        }
        public virtual void CloneFilters(SearchCriteria source)
        {
            this._id = source.Id;
            this._tris = new ObservableCollection<Sort>();
            this._tris.AddRange(source.Tris);
        }

        protected override bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (base.SetProperty(ref field, value,propertyName))
            {
                base.OnPropertyChanged("CriteresVides");
                return true;
            }
            return false;
        }

        internal async Task<Query> GenereQuery<T>(long? currentPage, long? itemsCountOnPage, bool checkUserConnection = true) where T : Model.ModeleBase
        {
            Query query = null;
            string where = "";

            // Génération de l'expression SQL LIMIT et calcul des compteurs
            query = await GenereLimit<T>(currentPage, itemsCountOnPage, checkUserConnection);
            if (query == null) return null;
            if (!query.Errors.IsEmpty) return query;

            // Génération de la clause SQL WHERE selon les critères
            where = await GenereWhereCriteria();

            // Génération de la requête
            query.SqlQuery = await this.GenereQuery(Const.NomTableSelonType<T>(), query, where, GenereOrderBy());
            query.SqlCountQuery = await this.GenereCountQuery(Const.NomTableSelonType<T>(), where, GenereOrderBy());

            return query;
        }

        protected virtual async Task<string> GenereQuery(string tableName, Query limit, string where, string orderBy)
        {
            string query = "";

            query = string.Format("SELECT * FROM {0} {1} {2} {3}",
                                  tableName,
                                  where,
                                  limit.SqlQuery,
                                  orderBy);

            return query;
        }
        protected virtual async Task<string> GenereCountQuery(string tableName, string where, string orderBy)
        {
            string query = "";

            query = string.Format("SELECT COUNT(*) FROM {0} {1} {2}",
                                  tableName,
                                  where,
                                  orderBy);

            return query;
        }

        private async Task<Query> GenereLimit<T>(long? currentPage, long? itemsCountOnPage, bool checkUserConnection = true) where T : Model.ModeleBase
        {
            Query query = new Query();

            query.CurrentPage = currentPage;
            query.ItemsCountOnPage = itemsCountOnPage;
            query.TotalItemsCount = -1;
            query.PagesCount = 1;
            query.CurrentItemIndex = 0;

            // Lecture du nombre d'éléments total
            try
            {
                Connection conn = await Connection.GetCurrent();
                query.TotalItemsCount = await conn.SelectCount<T>(checkUserConnection);
            }
            catch (Exception ex)
            {
                query.Errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return query;
            }

            if (query.CurrentPage == null || query.ItemsCountOnPage == null || query.CurrentPage <= 0 || query.ItemsCountOnPage <= 0)
            {
                query.CurrentPage = 1;
                query.ItemsCountOnPage = query.TotalItemsCount;
                return query;
            }

            // Calcul des autres compteurs
            query.PagesCount = query.TotalItemsCount / (long)query.ItemsCountOnPage + ((query.TotalItemsCount % query.ItemsCountOnPage) > 0 ? 1 : 0);
            query.CurrentItemIndex = (long)query.ItemsCountOnPage * ((long)query.CurrentPage - 1);
            if (query.CurrentPage > query.PagesCount) query.CurrentPage = query.PagesCount;
            if (query.CurrentItemIndex >= query.TotalItemsCount) query.CurrentItemIndex = query.TotalItemsCount - 1;

            // Génération de l'expression SQL LIMIT
            query.SqlQuery = string.Format("LIMIT {0} OFFSET {1}", query.ItemsCountOnPage, query.CurrentItemIndex);

            return query;
        }

        private async Task<string> GenereWhereCriteria()
        {
            string where = "";

            where = await this.GenereWhere();

            if (where != "")
            {
                if (where.StartsWith(" AND ")) where = where.Remove(0, 5);
                where = "WHERE " + where;
            }

            return where;
        }
        protected virtual async Task<string> GenereWhere() {
            string where = "";
            where += GenereEqual(Const.DB_COMMON_ID_COLNAME, this._id, -1);
            return where;
        }
        protected string GenereEqual<T>(string fieldName, T value, T nullValue, string function = "", bool startsWith = false)
        {
            if (value == null) return "";
            if (EqualityComparer<T>.Default.Equals(value, nullValue)) return "";

            if (typeof(T) == typeof(string) && function == "")
            {
                return " AND " + fieldName + " LIKE '" + (value as string).Replace("'", "''") + (startsWith ? "%" : "") + "'";
            }

            return " AND " + fieldName + " = " + function + ((function == "") ? "" : "(") + Tools.Convert.FormatSQL(value) + ((function == "") ? "" : ")");
        }
        protected string GenereContains(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return " AND " + fieldName + " LIKE '% " + value.Replace("'", "''") + "%'";
        }
        protected string GenereBetween<T>(string fieldName, T value1, T value2, T nullValue, string function = "")
        {
            if (EqualityComparer<T>.Default.Equals(value1, nullValue) && EqualityComparer<T>.Default.Equals(value2, nullValue))
                return "";
            else if (EqualityComparer<T>.Default.Equals(value1, nullValue))
                return " AND " + fieldName + " <= " + function + ((function == "") ? "" : "(") + Tools.Convert.FormatSQL(value2) + ((function == "") ? "" : ")");
            else if (EqualityComparer<T>.Default.Equals(value2, nullValue))
                return " AND " + fieldName + " >= " + function + ((function == "") ? "" : "(") + Tools.Convert.FormatSQL(value1) + ((function == "") ? "" : ")");
            else if (EqualityComparer<T>.Default.Equals(value1, value2))
                return " AND " + fieldName + " = " + function + ((function == "") ? "" : "(") + Tools.Convert.FormatSQL(value1) + ((function == "") ? "" : ")");
            else
                return " AND (" + fieldName + " >= " + function + ((function == "") ? "" : "(") + Tools.Convert.FormatSQL(value1) + ((function == "") ? "" : ")") + " AND "
                                + fieldName + " <= " + function + ((function == "") ? "" : "(") + Tools.Convert.FormatSQL(value2) + ((function == "") ? "" : ")") + ")";
        }

        
        private string GenereOrderBy()
        {
            string orderby = "";

            if (this.TriPrincipal == null) return "";

            orderby = GenereOrderBy(this.TriPrincipal);
            if (orderby == "") return "";

            orderby = "ORDER BY " + orderby;

            if (this._tris != null)
            {
                for (int i = 1; i < this._tris.Count - 1; i++)
                {
                    orderby += ", " + GenereOrderBy(this._tris[i]);
                }
            }
            
            return orderby;
        }
        private string GenereOrderBy(Sort tri)
        {
            if (tri == null) return "";

            string orderby = Const.NomSelonChamp(tri.Field);
            if (orderby != "")
            {
                if (tri.Order == Enums.OrdreTri.Descendant)
                    orderby += " DESC";
                else
                    orderby += " ASC";
            }

            return orderby;
        }

    }
}
