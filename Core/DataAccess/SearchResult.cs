using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.DataAccess
{
    [DataContract]
    public class SearchResult<T> : BaseNotifyPropertyChanged
    {
        protected ObservableCollection<T> _items;
        protected long? _currentPage;
        protected long _pagesCount;
        protected long? _itemsCountOnPage;
        protected long _selectedItemsCount;
        protected long _totalItemsCount;
        protected long _currentItemIndex;


        [DataMember]
        public ObservableCollection<T> Items {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        [DataMember]
        public long? CurrentPage {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }
        [DataMember]
        public long PagesCount {
            get { return _pagesCount; }
            set { SetProperty(ref _pagesCount, value); }
        }
        [DataMember]
        public long? ItemsCountOnPage {
            get { return _itemsCountOnPage; }
            set { SetProperty(ref _itemsCountOnPage, value); }
        }
        [DataMember]
        public long SelectedItemsCount
        {
            get { return _selectedItemsCount; }
            set { SetProperty(ref _selectedItemsCount, value); }
        }
        [DataMember]
        public long TotalItemsCount {
            get { return _totalItemsCount; }
            set { SetProperty(ref _totalItemsCount, value); }
        }
        [DataMember]
        public long CurrentItemIndex {
            get { return _currentItemIndex; }
            set { SetProperty(ref _currentItemIndex, value); }
        }

        public SearchResult()
        {
            this._items = new ObservableCollection<T>();
            this._currentPage = null;
            this._pagesCount = 0;
            this._itemsCountOnPage = null;
            this._selectedItemsCount = 0;
            this._totalItemsCount = 0;
            this._currentItemIndex = -1;
        }
    }
}
