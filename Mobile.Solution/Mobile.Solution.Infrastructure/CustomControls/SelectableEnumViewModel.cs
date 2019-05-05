using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using Syncfusion.Data.Extensions;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class SelectableEnumViewModel<T> : Dictionary<T, SelectableViewModel>, INotifyPropertyChanged
        where T : struct
    {
        private readonly Action<T> _callback;

        private KeyValuePair<T, SelectableViewModel> _selectedValue;

        public SelectableEnumViewModel(T? @enum, Action<T> callback, ResourceManager resourceManager)
        {
            _callback = callback;
            var enumType = typeof(T);
            if (!enumType.IsEnum())
                throw new Exception("Класс ObservableEnum может использоваться только для отображения перечислений!");

            foreach (T enumValue in Enum.GetValues(enumType))
                Add(enumValue,
                    new SelectableViewModel {Header = resourceManager.GetString(Enum.GetName(enumType, enumValue))});
            if (@enum.HasValue)
                SelectedValue = new KeyValuePair<T, SelectableViewModel>(@enum.Value, this[@enum.Value]);
        }

        public KeyValuePair<T, SelectableViewModel> SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (value.Value != null)
                {
                    if (!_selectedValue.Equals(value))
                    {
                        if (_selectedValue.Value != null)
                            _selectedValue.Value.IsSelected = false;

                        _selectedValue = value;
                        _callback?.Invoke(value.Key);
                    }
                    value.Value.IsSelected = true;
                }
                else
                {
                    if (_selectedValue.Value != null)
                    {
                        _selectedValue.Value.IsSelected = false;
                        _selectedValue = value;
                        OnPropertyChanged(nameof(SelectedValue));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetValue(T? key)
        {
            if (key.HasValue)
                SelectedValue = this.FirstOrDefault(v => v.Key.Equals(key));
            else
                SelectedValue = new KeyValuePair<T, SelectableViewModel>(default(T), null);
        }

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}