using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using GalaSoft.MvvmLight;
using Syncfusion.Data.Extensions;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    /// <summary>
    ///     Класс для хранения обычного и отображаемого значений перечислимого типа
    /// </summary>
    public class ObservableEnum<T> : ObservableObject
        where T : struct
    {
        /// <summary>
        ///     Функция, которая будет вызываться для обновления значения перечисления, на основе которого создан данный экземпляр
        ///     класса
        /// </summary>
        private readonly Action<T> _callback;

        /// <summary>
        ///     Словарь известных значений для различных перечислений
        /// </summary>
        private readonly Dictionary<T, string> _values;

        private KeyValuePair<T, string> _selectedValue;

        /// <summary>
        ///     Конструктор списка значений перечисления
        /// </summary>
        /// <param name="enum"></param>
        /// <param name="callback"></param>
        /// <param name="resourceManager"></param>
        public ObservableEnum(T @enum, Action<T> callback, ResourceManager resourceManager, bool silent = false)
        {
            _callback = callback;
            var enumType = @enum.GetType();
            if (!enumType.IsEnum())
                throw new Exception("Класс ObservableEnum может использоваться только для отображения перечислений!");
            _values = new Dictionary<T, string>();
            foreach (T enumValue in Enum.GetValues(enumType))
                _values.Add(enumValue,
                    resourceManager.GetString(Enum.GetName(enumType, enumValue)) ?? Enum.GetName(enumType, enumValue));
            if(silent)
				_selectedValue = ValuesList.FirstOrDefault(v => v.Key.Equals(@enum));
			else
				SelectedValue = ValuesList.FirstOrDefault(v => v.Key.Equals(@enum));
        }

        /// <summary>
        ///     Словарь доступных значений перечисления
        /// </summary>
        public Dictionary<T, string> ValuesList => _values;

        /// <summary>
        ///     Выбранное значение перечисления с текстом описания
        /// </summary>
        public KeyValuePair<T, string> SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (!_selectedValue.Equals(value))
                {
                    _selectedValue = value;
                    _callback?.Invoke(value.Key);
                    RaisePropertyChanged(() => SelectedValue);
                }
            }
        }

        public void DeleteItem(T item)
        {
            if (SelectedValue.Key.Equals(item))
                SelectedValue = _values.FirstOrDefault(v => !v.Key.Equals(item));
            _values.Remove(item);
            RaisePropertyChanged(() => ValuesList);
        }

        /// <summary>
        /// Sets the value from the ValuesList.
        /// </summary>
        /// <param name="key">Key.</param>
        public void SetValue(T key)
        {
            SelectedValue = ValuesList.FirstOrDefault(v => v.Key.Equals(key));
        }
    }
}