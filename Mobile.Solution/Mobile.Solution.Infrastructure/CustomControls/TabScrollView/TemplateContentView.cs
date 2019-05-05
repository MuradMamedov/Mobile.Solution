using System;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    /// <summary>
    ///     Class TemplateContentView.
    /// </summary>
    public class TemplateContentView : ContentView
    {
        /// <summary>
        ///     Clears the old Children
        ///     Creates the new View and adds it to the Children, and Invalidates the Layout
        /// </summary>
        /// <param name="newvalue"></param>
        private void ViewModelChangedImpl(object newvalue)
        {
            View newChild = null;
            // check ItemTemplateSelector first
            if (_currentItemSelector != null)
                newChild = this.ViewFor(newvalue, _currentItemSelector);

            newChild = newChild ?? TemplateSelector.ViewFor(newvalue);
            //Verify that newchild is a contentview
            Content = newChild;
        }

        #region Bindable Properties

        /// <summary>
        ///     Property definition for the <see cref="TemplateSelector" /> Property
        /// </summary>
        public static readonly BindableProperty TemplateSelectorProperty =
            BindableProperty.Create(nameof(TemplateSelector), typeof(TemplateSelector), typeof(TemplateContentView),
                default(TemplateSelector));

        /// <summary>
        ///     Property definition for the <see cref="ViewModel" /> Property
        /// </summary>
        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create(nameof(ViewModel), typeof(object), typeof(TemplateContentView), default(object),
                BindingMode.OneWay, null, ViewModelChanged);

        /// <summary>
        ///     The item template selector property
        /// </summary>
        public static readonly BindableProperty ItemTemplateSelectorProperty =
            BindableProperty.Create(nameof(ItemTemplateSelector), typeof(DataTemplateSelector),
                typeof(TemplateContentView), default(DataTemplateSelector),
                propertyChanged: OnDataTemplateSelectorChanged);

        private DataTemplateSelector _currentItemSelector;

        /// <summary>
        ///     Gets or sets the item template selector.
        /// </summary>
        /// <value>The item template selector.</value>
        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector) GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        private static void OnDataTemplateSelectorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((TemplateContentView) bindable).OnDataTemplateSelectorChanged(oldvalue as DataTemplateSelector,
                newvalue as DataTemplateSelector);
        }

        /// <summary>
        ///     Called when [data template selector changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataTemplateSelectorChanged(DataTemplateSelector oldValue,
            DataTemplateSelector newValue)
        {
            // cache value locally
            _currentItemSelector = newValue;
        }

        /// <summary>
        ///     Used to match a type with a datatemplate
        ///     <see cref="TemplateSelector" />
        /// </summary>
        public TemplateSelector TemplateSelector
        {
            get => (TemplateSelector) GetValue(TemplateSelectorProperty);
            set => SetValue(TemplateSelectorProperty, value);
        }

        /// <summary>
        ///     There is an argument to use 'object' rather than T
        ///     however you can specify T as object.  In addition
        ///     T allows the use of marker interfaces to enable
        ///     things like Ux Widgets while maintaining
        ///     some typesafety
        /// </summary>
        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        ///     Call down to the actual controls Implmentation
        ///     <see cref="ViewModelChangedImpl" />
        /// </summary>
        /// <param name="bindable">The TemplateContentView</param>
        /// <param name="oldValue">Ignored</param>
        /// <param name="newValue">Passed down to <see cref="ViewModelChangedImpl" /></param>
        /// <exception cref="Exception">Thrown if bindable is not in fact a TemplateContentView</exception>
        private static void ViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = bindable as TemplateContentView;
            if (layout == null)
                throw new Exception("InvalidBindableException");
            layout.ViewModelChangedImpl(newValue);
        }

        #endregion
    }
}