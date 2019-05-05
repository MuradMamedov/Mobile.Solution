﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mobile.Solution.Infrastructure.Helpers;
using Syncfusion.Data.Extensions;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public static class ObjectExtensions
    {
        public static string ToString(this object obj, string properyName)
        {
            if (!string.IsNullOrEmpty(properyName) && obj != null)
            {
                return (obj.GetType().GetProperty(properyName).GetValue(obj) ?? string.Empty).ToString();
            }
            return (obj ?? string.Empty).ToString();
        }
    }

    public class Suggestion
    {
        public Suggestion(object x, string searchValue, string searchPath)
        {
            Value = x;
            Text = x.ToString(searchPath);

            FormattedText = new FormattedString();
            var v = Text.IndexOf(searchValue, StringComparison.CurrentCultureIgnoreCase);
            var str1 = Text.Substring(0, v);
            var str2 = Text.Substring(v, searchValue.Length);
            var str3 = Text.Substring(v + searchValue.Length, Text.Length - v - searchValue.Length);
            FormattedText.Spans.Add(new Span {Text = str1});
            FormattedText.Spans.Add(new Span {Text = str2, FontAttributes = FontAttributes.Bold});
            FormattedText.Spans.Add(new Span {Text = str3});
        }

        public FormattedString FormattedText { get; set; }

        public string Text { get; set; }

        public object Value { get; set; }
    }

    /// <summary>
    ///     Define the AutoCompleteView control.
    /// </summary>
    public class AutoCompleteView : ContentView
    {
        /// <summary>
        ///     The execute on suggestion click property.
        /// </summary>
        public static readonly BindableProperty ExecuteOnSuggestionClickProperty = BindableProperty.Create(
            nameof(ExecuteOnSuggestionClick), typeof(bool), typeof(AutoCompleteView), false);

        /// <summary>
        ///     The placeholder property.
        /// </summary>
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder),
            typeof(string), typeof(AutoCompleteView), string.Empty, BindingMode.TwoWay,
            propertyChanged: PlaceHolderChanged);

        /// <summary>
        ///     The search background color property.
        /// </summary>
        public static readonly BindableProperty SearchBackgroundColorProperty = BindableProperty.Create(
            nameof(SearchBackgroundColor), typeof(Color), typeof(AutoCompleteView), Color.Red, BindingMode.TwoWay,
            propertyChanged: SearchBackgroundColorChanged);

        /// <summary>
        ///     The search border color property.
        /// </summary>
        public static readonly BindableProperty SearchBorderColorProperty = BindableProperty.Create(
            nameof(SearchBorderColor), typeof(Color), typeof(AutoCompleteView), Color.White, BindingMode.TwoWay,
            propertyChanged: SearchBorderColorChanged);

        /// <summary>
        ///     The search border radius property.
        /// </summary>
        public static readonly BindableProperty SearchBorderRadiusProperty = BindableProperty.Create(
            nameof(SearchBorderRadius), typeof(int), typeof(AutoCompleteView), 0, BindingMode.TwoWay,
            propertyChanged: SearchBorderRadiusChanged);

        /// <summary>
        ///     The search border width property.
        /// </summary>
        public static readonly BindableProperty SearchBorderWidthProperty = BindableProperty.Create(
            nameof(SearchBorderWidth), typeof(int), typeof(AutoCompleteView), 1, BindingMode.TwoWay,
            propertyChanged: SearchBorderWidthChanged);

        /// <summary>
        ///     The search command property.
        /// </summary>
        public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create(nameof(SearchCommand),
            typeof(ICommand), typeof(AutoCompleteView));

        /// <summary>
        ///     The search horizontal options property
        /// </summary>
        public static readonly BindableProperty SearchHorizontalOptionsProperty = BindableProperty.Create(
            nameof(SearchHorizontalOptions), typeof(LayoutOptions), typeof(AutoCompleteView),
            LayoutOptions.FillAndExpand, BindingMode.TwoWay, propertyChanged: SearchHorizontalOptionsChanged);

        /// <summary>
        ///     The search text color property.
        /// </summary>
        public static readonly BindableProperty SearchTextColorProperty = BindableProperty.Create(
            nameof(SearchTextColor), typeof(Color), typeof(AutoCompleteView), Color.Red, BindingMode.TwoWay,
            propertyChanged: SearchTextColorChanged);

        /// <summary>
        ///     The search text property.
        /// </summary>
        public static readonly BindableProperty SearchTextProperty = BindableProperty.Create(nameof(SearchText),
            typeof(string), typeof(AutoCompleteView), "Search", BindingMode.TwoWay, propertyChanged: SearchTextChanged);

        /// <summary>
        ///     The search vertical options property
        /// </summary>
        public static readonly BindableProperty SearchVerticalOptionsProperty = BindableProperty.Create(
            nameof(SearchVerticalOptions), typeof(LayoutOptions), typeof(AutoCompleteView), LayoutOptions.Center,
            BindingMode.TwoWay, propertyChanged: SearchVerticalOptionsChanged);

        /// <summary>
        ///     The selected command property.
        /// </summary>
        public static readonly BindableProperty SelectedCommandProperty =
            BindableProperty.Create(nameof(SelectedCommand), typeof(ICommand), typeof(AutoCompleteView));

        /// <summary>
        ///     The selected item property.
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem),
            typeof(object), typeof(AutoCompleteView), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: SelectedItemPropertyChanged);

        /// <summary>
        ///     The show search property.
        /// </summary>
        public static readonly BindableProperty ShowSearchProperty = BindableProperty.Create(nameof(ShowSearchButton),
            typeof(bool), typeof(AutoCompleteView), true, BindingMode.TwoWay, propertyChanged: ShowSearchChanged);

        /// <summary>
        ///     The suggestion background color property.
        /// </summary>
        public static readonly BindableProperty SuggestionBackgroundColorProperty = BindableProperty.Create(
            nameof(SuggestionBackgroundColor), typeof(Color), typeof(AutoCompleteView), Color.Red, BindingMode.TwoWay,
            propertyChanged: SuggestionBackgroundColorChanged);

        /// <summary>
        ///     The suggestion item data template property.
        /// </summary>
        public static readonly BindableProperty SuggestionItemDataTemplateProperty = BindableProperty.Create(
            nameof(SuggestionItemDataTemplate), typeof(DataTemplate), typeof(AutoCompleteView),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SuggestionItemDataTemplateChanged);

        /// <summary>
        ///     The suggestion height request property.
        /// </summary>
        public static readonly BindableProperty SuggestionsHeightRequestProperty = BindableProperty.Create(
            nameof(SuggestionsHeightRequest), typeof(double), typeof(AutoCompleteView), 250.0, BindingMode.TwoWay,
            propertyChanged: SuggestionHeightRequestChanged);

        /// <summary>
        ///     The searchbar height request property.
        /// </summary>
        public static readonly BindableProperty SearchBarHeightRequestProperty = BindableProperty.Create(
            nameof(SearchBarHeightRequest), typeof(double), typeof(AutoCompleteView), 40.0, BindingMode.TwoWay,
            propertyChanged: SearchBarHeightRequestChanged);


        /// <summary>
        ///     The suggestions property.
        /// </summary>
        public static readonly BindableProperty SuggestionsProperty = BindableProperty.Create(nameof(Suggestions),
            typeof(IEnumerable), typeof(AutoCompleteView));
        
		public static readonly BindableProperty AvailableSuggestionsProperty = BindableProperty.Create(nameof(AvailableSuggestions),
           typeof(IEnumerable), typeof(AutoCompleteView), propertyChanged:AvailableSuggestionsChanged);
        /// <summary>
        ///     The text background color property.
        /// </summary>
        public static readonly BindableProperty TextBackgroundColorProperty = BindableProperty.Create(
            nameof(TextBackgroundColor), typeof(Color), typeof(AutoCompleteView), Color.Transparent, BindingMode.TwoWay,
            propertyChanged: TextBackgroundColorChanged);

        /// <summary>
        ///     The text color property.
        /// </summary>
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor),
            typeof(Color), typeof(AutoCompleteView), Color.Black, BindingMode.TwoWay,
            propertyChanged: TextColorChanged);

        /// <summary>
        ///     The placeholder color property.
        /// </summary>
        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create(nameof(PlaceHolderColor),
            typeof(Color), typeof(AutoCompleteView), Color.Gray, BindingMode.TwoWay,
            propertyChanged: PlaceHolderColorChanged);

        /// <summary>
        ///     The text horizontal options property
        /// </summary>
        public static readonly BindableProperty TextHorizontalOptionsProperty = BindableProperty.Create(
            nameof(TextHorizontalOptions), typeof(LayoutOptions), typeof(AutoCompleteView), LayoutOptions.FillAndExpand,
            BindingMode.TwoWay, propertyChanged: TextHorizontalOptionsChanged);

        /// <summary>
        ///     The text property.
        /// </summary>
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
            typeof(AutoCompleteView), string.Empty, BindingMode.TwoWay, propertyChanged: TextValueChanged);

        /// <summary>
        ///     The search path property.
        /// </summary>
        public static readonly BindableProperty SearchPathProperty = BindableProperty.Create(nameof(SearchPath),
            typeof(string),
            typeof(AutoCompleteView), null, BindingMode.OneWay);

        /// <summary>
        ///     The text vertical options property.
        /// </summary>
        public static readonly BindableProperty TextVerticalOptionsProperty = BindableProperty.Create(
            nameof(TextVerticalOptions), typeof(LayoutOptions), typeof(AutoCompleteView), LayoutOptions.Start,
            BindingMode.TwoWay, propertyChanged: TestVerticalOptionsChanged);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(AutoCompleteView),
                default(DataTemplate), propertyChanged: ItemTemplatePropertyChanged);

		private readonly Label _notFoundLabel;
		private readonly Button _btnSearch;
        internal SearchBar EntText { get; }
        private readonly StackLayoutList _lstSuggestions;
        private readonly StackLayout _stkBase;

        private List<Suggestion> _availableSuggestions = new List<Suggestion>();

        public bool SuppressTextChangedEvent = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoCompleteView" /> class.
        /// </summary>
        public AutoCompleteView()
		{
			_stkBase = new StackLayout();
			var innerLayout = new StackLayout();
			EntText = new SearchBar
			{
				HorizontalOptions = TextHorizontalOptions,
				VerticalOptions = TextVerticalOptions,
                WidthRequest=1,
				HeightRequest = SearchBarHeightRequest,
				TextColor = TextColor,
				SearchCommand =
					new Command(obj => SelectedItem = SelectedItem ?? _availableSuggestions.FirstOrDefault().Value),
				BackgroundColor = TextBackgroundColor
			};
			_btnSearch = new Button
			{
				VerticalOptions = SearchVerticalOptions,
				HorizontalOptions = SearchHorizontalOptions,
				Text = SearchText
			};
			var grid = new Grid();
            _notFoundLabel = new Label
            {
                Text = "Ничего не найдено",
                IsVisible = false,
                Margin=new Thickness(0,10,0,0),
                TextColor=Color.FromHex("#8C8C8C"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

			_lstSuggestions = new StackLayoutList
			{
				BindingContext = this,
			};

			grid.Children.Add(_notFoundLabel);
			grid.Children.Add(new ScrollView
			{
				Content = _lstSuggestions,
				Padding = new Thickness(0, 0, 0, 0),
				HeightRequest = SuggestionsHeightRequest,
				IsVisible = false
			});

			innerLayout.Children.Add(EntText);
			innerLayout.Children.Add(_btnSearch);
			innerLayout.Children.Add(grid);


			_stkBase.Children.Add(innerLayout);

			Content = _stkBase;

			EntText.Unfocused += (s, e) => { ShowHideListbox(false); };

			EntText.TextChanged += (s, e) =>
			{
				Text = e.NewTextValue;
				OnTextChanged(e);
			};
			_btnSearch.Clicked += (s, e) =>
			{
				if (SearchCommand != null && SearchCommand.CanExecute(Text))
				{
					SearchCommand.Execute(Text);
				}
			};
			_lstSuggestions.ItemSelected += (s, e) =>
			{
				if (e.SelectedItem == null)
					return;
				var item = e.SelectedItem as Suggestion;
				EntText.Text = item.Text;

				ShowHideListbox(false);
				OnSelectedItemChanged(item.Value);

				if (ExecuteOnSuggestionClick
					&& SearchCommand != null
					&& SearchCommand.CanExecute(Text))
				{
					SearchCommand.Execute(e);
				}
			};
			_lstSuggestions.ItemTemplate = new DataTemplate(() =>
			{
				var label = new Label { LineBreakMode = LineBreakMode.NoWrap };
				label.SetBinding(Label.FormattedTextProperty, "FormattedText");

				return label;
			});
		}

        /// <summary>
        ///     Gets or sets a value indicating whether [execute on sugestion click].
        /// </summary>
        /// <value><c>true</c> if [execute on sugestion click]; otherwise, <c>false</c>.</value>
        public bool ExecuteOnSuggestionClick
        {
            get => (bool) GetValue(ExecuteOnSuggestionClickProperty);
            set => SetValue(ExecuteOnSuggestionClickProperty, value);
        }

        /// <summary>
        ///     Gets or sets the placeholder.
        /// </summary>
        /// <value>The placeholder.</value>
        public string Placeholder
        {
            get => (string) GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        /// <summary>
        ///     Gets or sets the color of the search background.
        /// </summary>
        /// <value>The color of the search background.</value>
        public Color SearchBackgroundColor
        {
            get => (Color) GetValue(SearchBackgroundColorProperty);
            set => SetValue(SearchBackgroundColorProperty, value);
        }

        /// <summary>
        ///     Gets or sets the search border color.
        /// </summary>
        /// <value>The search border brush.</value>
        public Color SearchBorderColor
        {
            get => (Color) GetValue(SearchBorderColorProperty);
            set => SetValue(SearchBorderColorProperty, value);
        }

        /// <summary>
        ///     Gets or sets the search border radius.
        /// </summary>
        /// <value>The search border radius.</value>
        public int SearchBorderRadius
        {
            get => (int) GetValue(SearchBorderRadiusProperty);
            set => SetValue(SearchBorderRadiusProperty, value);
        }

        /// <summary>
        ///     Gets or sets the width of the search border.
        /// </summary>
        /// <value>The width of the search border.</value>
        public int SearchBorderWidth
        {
            get => (int) GetValue(SearchBorderWidthProperty);
            set => SetValue(SearchBorderWidthProperty, value);
        }

        /// <summary>
        ///     Gets or sets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public ICommand SearchCommand
        {
            get => (ICommand) GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the search horizontal options.
        /// </summary>
        /// <value>The search horizontal options.</value>
        public LayoutOptions SearchHorizontalOptions
        {
            get => (LayoutOptions) GetValue(SearchHorizontalOptionsProperty);
            set => SetValue(SearchHorizontalOptionsProperty, value);
        }

        /// <summary>
        ///     Gets or sets the search text.
        /// </summary>
        /// <value>The search text.</value>
        public string SearchText
        {
            get => (string) GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        /// <summary>
        ///     Gets or sets the color of the search text button.
        /// </summary>
        /// <value>The color of the search text.</value>
        public Color SearchTextColor
        {
            get => (Color) GetValue(SearchTextColorProperty);
            set => SetValue(SearchTextColorProperty, value);
        }

        /// <summary>
        ///     Gets or sets the search vertical options.
        /// </summary>
        /// <value>The search vertical options.</value>
        public LayoutOptions SearchVerticalOptions
        {
            get => (LayoutOptions) GetValue(SearchVerticalOptionsProperty);
            set => SetValue(SearchVerticalOptionsProperty, value);
        }


        /// <summary>
        ///     Gets or sets the selected command.
        /// </summary>
        /// <value>The selected command.</value>
        public ICommand SelectedCommand
        {
            get => (ICommand) GetValue(SelectedCommandProperty);
            set => SetValue(SelectedCommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [show search button].
        /// </summary>
        /// <value><c>true</c> if [show search button]; otherwise, <c>false</c>.</value>
        public bool ShowSearchButton
        {
            get => (bool) GetValue(ShowSearchProperty);
            set => SetValue(ShowSearchProperty, value);
        }

        /// <summary>
        ///     Gets or sets the color of the sugestion background.
        /// </summary>
        /// <value>The color of the sugestion background.</value>
        public Color SuggestionBackgroundColor
        {
            get => (Color) GetValue(SuggestionBackgroundColorProperty);
            set => SetValue(SuggestionBackgroundColorProperty, value);
        }

        /// <summary>
        ///     Gets or sets the suggestion item data template.
        /// </summary>
        /// <value>The sugestion item data template.</value>
        public DataTemplate SuggestionItemDataTemplate
        {
            get => (DataTemplate) GetValue(SuggestionItemDataTemplateProperty);
            set => SetValue(SuggestionItemDataTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the Suggestions.
        /// </summary>
        /// <value>The Suggestions.</value>
        public IEnumerable Suggestions
        {
            get => (IEnumerable) GetValue(SuggestionsProperty);
            set => SetValue(SuggestionsProperty, value);
        }

		public IEnumerable AvailableSuggestions
		{
			get => (IEnumerable)GetValue(AvailableSuggestionsProperty);
			set => SetValue(AvailableSuggestionsProperty, value);
		}

		/// <summary>
		///     Gets or sets the height of the suggestion.
		/// </summary>
		/// <value>The height of the suggestion.</value>
		public double SuggestionsHeightRequest
        {
            get => (double) GetValue(SuggestionsHeightRequestProperty);
            set => SetValue(SuggestionsHeightRequestProperty, value);
        }

        /// <summary>
        ///     Gets or sets the height of the SearchBar.
        /// </summary>
        /// <value>The height of the SearchBar.</value>
        public double SearchBarHeightRequest
        {
            get => (double) GetValue(SearchBarHeightRequestProperty);
            set => SetValue(SearchBarHeightRequestProperty, value);
        }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string SearchPath
        {
            get => (string) GetValue(SearchPathProperty);
            set => SetValue(SearchPathProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the color of the text background.
        /// </summary>
        /// <value>The color of the text background.</value>
        public Color TextBackgroundColor
        {
            get => (Color) GetValue(TextBackgroundColorProperty);
            set => SetValue(TextBackgroundColorProperty, value);
        }

        /// <summary>
        ///     Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor
        {
            get => (Color) GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        /// <summary>
        ///     Gets or sets the color of the placeholder.
        /// </summary>
        /// <value>The color of the placeholder.</value>
        public Color PlaceHolderColor
        {
            get => (Color) GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }

        /// <summary>
        ///     Gets or sets the text horizontal options.
        /// </summary>
        /// <value>The text horizontal options.</value>
        public LayoutOptions TextHorizontalOptions
        {
            get => (LayoutOptions) GetValue(TextHorizontalOptionsProperty);
            set => SetValue(TextHorizontalOptionsProperty, value);
        }

        /// <summary>
        ///     Gets or sets the text vertical options.
        /// </summary>
        /// <value>The text vertical options.</value>
        public LayoutOptions TextVerticalOptions
        {
            get => (LayoutOptions) GetValue(TextVerticalOptionsProperty);
            set => SetValue(TextVerticalOptionsProperty, value);
        }

        /// <summary>
        ///     Occurs when [selected item changed].
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        ///     Occurs when [text changed].
        /// </summary>
        public event EventHandler<TextChangedEventArgs> TextChanged;

        /// <summary>
        ///     Places the holder changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldPlaceHolderValue">The old place holder value.</param>
        /// <param name="newPlaceHolderValue">The new place holder value.</param>
        private static void PlaceHolderChanged(BindableObject obj, object oldPlaceHolderValue,
            object newPlaceHolderValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView.EntText.Placeholder = newPlaceHolderValue as string;
            }
        }

        /// <summary>
        ///     Searches the background color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchBackgroundColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._stkBase.BackgroundColor = (Color) newValue;
            }
        }

        /// <summary>
        ///     Searches the border color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchBorderColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.BorderColor = (Color) newValue;
            }
        }

        /// <summary>
        ///     Searches the border radius changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchBorderRadiusChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.BorderRadius = (int) newValue;
            }
        }

        /// <summary>
        ///     Searches the border width changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchBorderWidthChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.BorderWidth = (double) newValue;
            }
        }

        /// <summary>
        ///     Searches the horizontal options changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchHorizontalOptionsChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.HorizontalOptions = (LayoutOptions) newValue;
            }
        }

        /// <summary>
        ///     Searches the text changed.
        /// </summary>
        /// <param name="obj">The bindable.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchTextChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.Text = (string) newValue;
            }
        }

        /// <summary>
        ///     Searches the text color color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchTextColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.TextColor = (Color) newValue;
            }
        }

        /// <summary>
        ///     Searches the vertical options changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchVerticalOptionsChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.VerticalOptions = (LayoutOptions) newValue;
            }
        }

        /// <summary>
        ///     Shows the search changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldShowSearchValue">if set to <c>true</c> [old show search value].</param>
        /// <param name="newShowSearchValue">if set to <c>true</c> [new show search value].</param>
        private static void ShowSearchChanged(BindableObject obj, object oldShowSearchValue, object newShowSearchValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._btnSearch.IsVisible = (bool) newShowSearchValue;
            }
        }

        /// <summary>
        ///     Suggestions the background color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SuggestionBackgroundColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._lstSuggestions.BackgroundColor = (Color) newValue;
            }
        }

        /// <summary>
        ///     Suggestions the height changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SuggestionHeightRequestChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._lstSuggestions.HeightRequest = (double) newValue;
            }
        }

        /// <summary>
        ///     SearchBar the height changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void SearchBarHeightRequestChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView.EntText.HeightRequest = (double) newValue;
            }
        }


        /// <summary>
        ///     Suggestions the item data template changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldShowSearchValue">The old show search value.</param>
        /// <param name="newShowSearchValue">The new show search value.</param>
        private static void SuggestionItemDataTemplateChanged(BindableObject obj, object oldShowSearchValue,
            object newShowSearchValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._lstSuggestions.ItemTemplate = (DataTemplate) newShowSearchValue;
            }
        }

        /// <summary>
        ///     Tests the vertical options changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void TestVerticalOptionsChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView.EntText.VerticalOptions = (LayoutOptions) newValue;
            }
        }

        private static void ItemTemplatePropertyChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView._lstSuggestions.ItemTemplate = (DataTemplate) newValue;
            }
        }
	    
        private static void AvailableSuggestionsChanged(BindableObject obj, object oldValue, object newValue)
		{
			var autoCompleteView = obj as AutoCompleteView;
			if (autoCompleteView != null)
			{
                autoCompleteView._availableSuggestions = new List<Suggestion>();
                foreach(var suggestion in (IEnumerable) newValue)
                {
                    autoCompleteView._availableSuggestions.Add(new Suggestion(suggestion, string.Empty, autoCompleteView.SearchPath));
                }
                if (autoCompleteView._availableSuggestions.Count > 0)
                {
                    autoCompleteView.ShowHideListbox(true);
                    autoCompleteView._lstSuggestions.ItemsSource = autoCompleteView._availableSuggestions;
                }
			}
		}

        /// <summary>
        ///     Texts the background color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void TextBackgroundColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView.EntText.BackgroundColor = (Color) newValue;
            }
        }

        /// <summary>
        ///     Texts the color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void TextColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView.EntText.TextColor = (Color) newValue;
            }
        }

        /// <summary>
        ///     PlaceHolder's the color changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void PlaceHolderColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView.EntText.PlaceholderColor = (Color) newValue;
            }
        }

        /// <summary>
        ///     Texts the horizontal options changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void TextHorizontalOptionsChanged(BindableObject obj, object oldValue, object newValue)
        {
            var autoCompleteView = obj as AutoCompleteView;
            if (autoCompleteView != null)
            {
                autoCompleteView.EntText.VerticalOptions = (LayoutOptions) newValue;
            }
        }

        private static void SelectedItemPropertyChanged(BindableObject obj, object oldValue, object newValue)
        {
            var control = obj as AutoCompleteView;
            if (control == null)
                return;
            control.SuppressTextChangedEvent = true;
            control.EntText.Text = newValue?.ToString(control.SearchPath);
            control.EntText.Unfocus();
            control.SuppressTextChangedEvent = false;
        }

        /// <summary>
        ///     Texts the changed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="oldPlaceHolderValue">The old place holder value.</param>
        /// <param name="newPlaceHolderValue">The new place holder value.</param>
        private static async void TextValueChanged(BindableObject obj, object oldPlaceHolderValue,
            object newPlaceHolderValue)
        {
            var control = obj as AutoCompleteView;
            control.EntText.Text = newPlaceHolderValue?.ToString();
            if (control != null)
            {
                if (control.SuppressTextChangedEvent)
                    return;
                control._btnSearch.IsEnabled = !string.IsNullOrEmpty((string) newPlaceHolderValue);

                var cleanedValue = ((string) newPlaceHolderValue ?? string.Empty).ToLowerInvariant();

                if (!string.IsNullOrEmpty(cleanedValue) && control.Suggestions != null)
                {
                    try
                    {
                        var filteredSuggestions = (from x in control.Suggestions.Cast<object>()
                                where (x ?? string.Empty).ToString(control.SearchPath)
                                .ToLowerInvariant()
                                .Contains(cleanedValue)
                                orderby (x ?? string.Empty).ToString(control.SearchPath)
                                .IndexOf(cleanedValue, StringComparison.CurrentCultureIgnoreCase), (x ?? string.Empty)
                                .ToString(control.SearchPath)
                                .Length
                                select new Suggestion(x, cleanedValue, control.SearchPath)).Take(10)
                            .ToList();

                        control._availableSuggestions = new List<Suggestion>();
                        if (filteredSuggestions.Count > 0)
                        {
                            foreach (var suggestion in filteredSuggestions)
                            {
                                control._availableSuggestions.Add(suggestion);
                            }
                            control.ShowHideListbox(true);
                        }
                        else
                        {
                            control.ShowHideListbox(false);
							control.ShowHideNotFoundLabel(true);
						}
                    }
                    catch (Exception ex)
                    {
                        var newExc = new Exception($"AutoCompleteException cleanedValue - {cleanedValue} ", ex);
                        await newExc.ToLogUnhandledException();
                    }
                }
                else
                {
                    control.EntText.Text = (string) newPlaceHolderValue;
                    if (control._availableSuggestions.Count > 0)
                    {
                        control._availableSuggestions = new List<Suggestion>();
                        control.ShowHideListbox(false);
                    }
                }
                control._lstSuggestions.ItemsSource = control._availableSuggestions;
            }
        }

        /// <summary>
        ///     Called when [selected item changed].
        /// </summary>
        /// <param name="selectedItem">The selected item.</param>
        private void OnSelectedItemChanged(object selectedItem)
        {
            SelectedItem = selectedItem;

            SelectedCommand?.Execute(selectedItem);

            var handler = SelectedItemChanged;
            handler?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        /// <summary>
        ///     Handles the <see cref="E:TextChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void OnTextChanged(TextChangedEventArgs e)
        {
            var handler = TextChanged;
            handler?.Invoke(this, e);
        }

        /// <summary>
        ///     Shows the hide listbox.
        /// </summary>
        /// <param name="show">if set to <c>true</c> [show].</param>
        private void ShowHideListbox(bool show)
        {
            (_lstSuggestions.Parent as ScrollView).IsVisible = show;
            if (show)
                ShowHideNotFoundLabel(!show);
        }
		private void ShowHideNotFoundLabel(bool show)
        {
			_notFoundLabel.IsVisible = show;
		}

	}
}