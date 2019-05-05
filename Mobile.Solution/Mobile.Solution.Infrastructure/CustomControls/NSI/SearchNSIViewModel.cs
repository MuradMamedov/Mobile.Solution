using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Requests.NSI;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;
using Newtonsoft.Json;
using Syncfusion.Data.Extensions;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class SearchNsiViewModel<T> : SelectableViewModel
        where T : class, INsiItem
    {
        private readonly Action<T> _callback;
        private SearchNsiViewModel<T> _parent;

        public SearchNsiViewModel(SearchNsiViewModel<T> parent, string title, string searchPath)
        {
            _parent = parent;
            PlaceHolder = _parent.PlaceHolder;
            Title = title;
            Items = _parent.Items.ToList();
            SearchPath = searchPath;
            AvailableSuggestions = ReadItemsFromFile();
			_searchText = _parent?.SelectedItem.ToString(SearchPath);
		}

        public SearchNsiViewModel(string placeHolder, Action<T> callback = null,
            string defaultSearchValue = null)
        {
            PlaceHolder = placeHolder;
            _callback = callback;
            SetItems();
            SetSelecetedItem(defaultSearchValue);
        }

        private void SetItems()
        {
            var prop = typeof(Dictionaries).GetProperties()
                .FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Length > 0 &&
                                     p.PropertyType.GenericTypeArguments[0] == typeof(T));
            Items = ((IEnumerable)prop.GetValue(Dictionaries.Instance)).Cast<T>().ToList();
        }

        private void SetSelecetedItem(string defaultSearchValue)
        {
            try
            {
                SelectedItem = Items.FirstOrDefault(x => x.ToString().ToLower().Contains(defaultSearchValue));
            }
            catch
            {
                // ignored
            }
        }

        #region Properties

        public string SearchText
        {
            get => _searchText;
            set
            {
                Set(() => SearchText, ref _searchText, value);
                if (string.IsNullOrEmpty(_searchText))
                {
                    _selectedItem = null;
					if (_parent != null)
					    _parent.SelectedItem = _selectedItem;
                    _callback?.Invoke(null);
                    RaisePropertyChanged(() => SelectedItem);
                }
            }
        }

        private string _searchText;

        public string PlaceHolder { get; }

        public string Title { get; }

        public string SearchPath { get; }

        public List<T> Items
        {
            get => _items;
            set { Set(() => Items, ref _items, value); }
        }

        private List<T> _items;

        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (Set(() => SelectedItem, ref _selectedItem, value))
                {
                    if (_parent != null)
                    {
                        _parent.SelectedItem = _selectedItem;
                        _parent.Navigation.PopAsync();
                        if (_selectedItem != null)
                        {
                            SaveItemToFile();
                        }
                    }
                    _callback?.Invoke(_selectedItem);
                    SearchText = _selectedItem?.ToString(SearchPath);
                }
            }
        }

        private T _selectedItem;

		public List<T> AvailableSuggestions { get; private set; }

        public INavigation Navigation { get; set; }

		#endregion Properties

		private List<T> ReadItemsFromFile()
        {
			var fileName = $"{typeof(T).Name}.sres";
			var fp = DependencyService.Get<IFilePreview>();
			var savedItemsString = fp.ReadStringFromFile(fileName);
			return savedItemsString != null ? JsonConvert.DeserializeObject<List<T>>(savedItemsString).ToList() : new List<T>();
		}

		private void SaveItemToFile()
        {
            var fileName = $"{typeof(T).Name}.sres";
			var fp = DependencyService.Get<IFilePreview>();
            var savedItems = ReadItemsFromFile();
            savedItems.Remove(savedItems.FirstOrDefault(si => si.ToString() == _selectedItem.ToString()));
            savedItems.Insert(0,_selectedItem);
            fp.SaveStringToFile(fileName, JsonConvert.SerializeObject(savedItems.Take(10)));
		}
    }
}