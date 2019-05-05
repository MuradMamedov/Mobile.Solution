﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    /// <summary>
    ///     Class TemplateSelector.
    /// </summary>
    [ContentProperty("Templates")]
    public class TemplateSelector : BindableObject
    {
        /// <summary>
        ///     Property definition for the <see cref="Templates" /> Bindable Property
        /// </summary>
        public static BindableProperty TemplatesProperty =
            BindableProperty.Create(nameof(Templates), typeof(DataTemplateCollection), typeof(TemplateSelector),
                default(DataTemplateCollection), BindingMode.OneWay, null, TemplatesChanged);

        /// <summary>
        ///     Property definition for the <see cref="SelectorFunction" /> Bindable Property
        /// </summary>
        public static BindableProperty SelectorFunctionProperty = BindableProperty.Create(nameof(SelectorFunction),
            typeof(Func<Type, DataTemplate>), typeof(TemplateSelector));

        /// <summary>
        ///     Property definition for the <see cref="ExceptionOnNoMatch" /> Bindable Property
        /// </summary>
        public static BindableProperty ExceptionOnNoMatchProperty = BindableProperty.Create(nameof(ExceptionOnNoMatch),
            typeof(bool), typeof(TemplateSelector), true);

        /// <summary>
        ///     Initialize the TemplateCollections so that each
        ///     instance gets it's own collection
        /// </summary>
        public TemplateSelector()
        {
            Templates = new DataTemplateCollection();
        }

        /// <summary>
        ///     Private cache of matched types with datatemplates
        ///     The cache is reset on any change to <see cref="Templates" />
        /// </summary>
        private Dictionary<Type, DataTemplate> Cache { get; set; }

        /// <summary>
        ///     Bindable property that allows the user to
        ///     there is no matching template found
        /// </summary>
        public bool ExceptionOnNoMatch
        {
            get => (bool) GetValue(ExceptionOnNoMatchProperty);
            set => SetValue(ExceptionOnNoMatchProperty, value);
        }

        /// <summary>
        ///     The collection of DataTemplates
        /// </summary>
        public DataTemplateCollection Templates
        {
            get => (DataTemplateCollection) GetValue(TemplatesProperty);
            set => SetValue(TemplatesProperty, value);
        }

        /// <summary>
        ///     A user supplied function of type
        ///     <code>Func<typeparamname name="Type"></typeparamname>,<typeparamname name="DataTemplate"></typeparamname></code>
        ///     If this function has been supplied it is always called first in the match
        ///     process.
        /// </summary>
        public Func<Type, DataTemplate> SelectorFunction
        {
            get => (Func<Type, DataTemplate>) GetValue(SelectorFunctionProperty);
            set => SetValue(SelectorFunctionProperty, value);
        }

        /// <summary>
        ///     Clears the cache when the set of templates change
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="oldval"></param>
        /// <param name="newval"></param>
        public static void TemplatesChanged(BindableObject bo, object oldval, object newval)
        {
            var ts = bo as TemplateSelector;
            if (ts == null)
                return;
            if (oldval != null)
                ((DataTemplateCollection) oldval).CollectionChanged -= ts.TemplateSetChanged;
            ((DataTemplateCollection) newval).CollectionChanged += ts.TemplateSetChanged;
            ts.Cache = null;
        }

        /// <summary>
        ///     Clear the cache on any template set change
        ///     If needed this could be optimized to care about the specific
        ///     change but I doubt it would be worthwhile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateSetChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Cache = null;
        }


        /// <summary>
        ///     Matches a type with a datatemplate
        ///     Order of matching=>
        ///     SelectorFunction,
        ///     Cache,
        ///     SpecificTypeMatch,
        ///     InterfaceMatch,
        ///     BaseTypeMatch
        ///     DefaultTempalte
        /// </summary>
        /// <param name="type">Type object type that needs a datatemplate</param>
        /// <returns>
        ///     The DataTemplate from the WrappedDataTemplates Collection that closest matches
        ///     the type paramater.
        /// </returns>
        /// <exception cref="Exception"></exception>
        /// Thrown if there is no datatemplate that matches the supplied type
        public DataTemplate TemplateFor(Type type)
        {
            var typesExamined = new List<Type>();
            var template = TemplateForImpl(type, typesExamined);
            if (template == null && ExceptionOnNoMatch)
                throw new Exception("NoDataTemplateMatchException");
            return template;
        }

        /// <summary>
        ///     Interal implementation of <see cref="TemplateFor" />.
        /// </summary>
        /// <param name="type">The type to match on</param>
        /// <param name="examined">A list of all types examined during the matching process</param>
        /// <returns>A DataTemplate or null</returns>
        private DataTemplate TemplateForImpl(Type type, ICollection<Type> examined)
        {
            if (type == null)
                return null; //This can happen when we recusively check base types (object.BaseType==null)
            examined.Add(type);
            Contract.Assert(Templates != null, "Templates cannot be null");

            Cache = Cache ?? new Dictionary<Type, DataTemplate>();
            DataTemplate retTemplate = null;

            //Prefer the selector function if present
            //This has been moved before the cache check so that
            //the user supplied function has an opportunity to 
            //Make a decision with more information than simply
            //the requested type (perhaps the Ux or Network states...)
            SelectorFunction?.Invoke(type);

            //Happy case we already have the type in our cache
            if (Cache.ContainsKey(type))
                return Cache[type];


            //check our list
            retTemplate = Templates.Where(x => x.Type == type).Select(x => x.WrappedTemplate).FirstOrDefault();
            //Check for interfaces
            retTemplate = retTemplate ??
                          type.GetTypeInfo()
                              .ImplementedInterfaces.Select(x => TemplateForImpl(x, examined))
                              .FirstOrDefault();
            //look at base types
            retTemplate = retTemplate ?? TemplateForImpl(type.GetTypeInfo().BaseType, examined);
            //If all else fails try to find a Default Template
            retTemplate = retTemplate ??
                          Templates.Where(x => x.IsDefault).Select(x => x.WrappedTemplate).FirstOrDefault();

            Cache[type] = retTemplate;
            return retTemplate;
        }

        /// <summary>
        ///     Finds a template for the type of the passed in item (<code>item.GetType()</code>)
        ///     and creates the content and sets the Binding context of the View
        ///     Currently the root of the DataTemplate must be a ViewCell.
        /// </summary>
        /// <param name="item">The item to instantiate a DataTemplate for</param>
        /// <returns>a View with it's binding context set</returns>
        /// <exception cref="Exception"></exception>
        /// Thrown when the matched datatemplate inflates to an object not derived from either
        /// <see cref="Xamarin.Forms.View" />
        /// or
        /// <see cref="Xamarin.Forms.ViewCell" />
        public View ViewFor(object item)
        {
            if (item == null)
                return null;

            var template = TemplateFor(item.GetType());
            var content = template.CreateContent();
            if (!(content is View) && !(content is ViewCell))
                throw new Exception("InvalidVisualObjectException");

            var view = content is View ? content as View : ((ViewCell) content).View;
            view.BindingContext = item;
            return view;
        }
    }

    /// <summary>
    ///     Interface to enable DataTemplateCollection to hold
    ///     typesafe instances of DataTemplateWrapper
    /// </summary>
    public interface IDataTemplateWrapper
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        bool IsDefault { get; set; }

        /// <summary>
        ///     Gets or sets the wrapped template.
        /// </summary>
        /// <value>The wrapped template.</value>
        DataTemplate WrappedTemplate { get; set; }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>The type.</value>
        Type Type { get; }
    }

    /// <summary>
    ///     Wrapper for a DataTemplate.
    ///     Unfortunately the default constructor for DataTemplate is internal
    ///     so I had to wrap the DataTemplate instead of inheriting it.
    /// </summary>
    /// <typeparam name="T">The object type that this DataTemplateWrapper matches</typeparam>
    [ContentProperty("WrappedTemplate")]
    public class DataTemplateWrapper<T> : BindableObject, IDataTemplateWrapper
    {
        /// <summary>
        ///     The wrapped template property
        /// </summary>
        public static readonly BindableProperty WrappedTemplateProperty =
            BindableProperty.Create(nameof(WrappedTemplate), typeof(DataTemplate), typeof(DataTemplateWrapper<T>));

        /// <summary>
        ///     The is default property
        /// </summary>
        public static readonly BindableProperty IsDefaultProperty =
            BindableProperty.Create(nameof(IsDefault), typeof(bool), typeof(DataTemplateWrapper<T>), false);

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public bool IsDefault
        {
            get => (bool) GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        /// <summary>
        ///     Gets or sets the wrapped template.
        /// </summary>
        /// <value>The wrapped template.</value>
        public DataTemplate WrappedTemplate
        {
            get => (DataTemplate) GetValue(WrappedTemplateProperty);
            set => SetValue(WrappedTemplateProperty, value);
        }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type => typeof(T);
    }

    /// <summary>
    ///     Collection class of IDataTemplateWrapper
    ///     Enables xaml definitions of collections.
    /// </summary>
    public class DataTemplateCollection : ObservableCollection<IDataTemplateWrapper>
    {
    }
}