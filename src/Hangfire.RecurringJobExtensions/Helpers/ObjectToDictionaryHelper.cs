using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Hangfire.RecurringJobExtensions.Helpers
{
    /// <summary>   An object to dictionary helper. </summary>
    public static class ObjectToDictionaryHelper
    {
        //private static readonly MethodInfo AddToDicitonaryMethod = typeof(IDictionary<string, object>).GetMethod("Add");
        //private static readonly ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>> Converters = new ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>>();
        //private static readonly ConstructorInfo DictionaryConstructor = typeof(Dictionary<string, object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());

        ////todo : Has bug
        //public static IDictionary<string, object> ToDictionaryEnhance(this object obj)
        //{
        //    if (obj == null)
        //        return null;
        //    else
        //        return Converters.GetOrAdd(obj.GetType(), o =>
        //        {
        //            var outputType = typeof(IDictionary<string, object>);
        //            var inputType = obj.GetType();
        //            var inputExpression = Expression.Parameter(typeof(object), "input");
        //            var typedInputExpression = Expression.Convert(inputExpression, inputType);
        //            var outputVariable = Expression.Variable(outputType, "output");
        //            var returnTarget = Expression.Label(outputType);
        //            var body = new List<Expression>
        //            {
        //                Expression.Assign(outputVariable, Expression.New(DictionaryConstructor))
        //            };
        //            body.AddRange(
        //                from prop in inputType.GetProperties(
        //                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
        //                where prop.CanRead && (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
        //                let getExpression = Expression.Property(typedInputExpression, prop.GetMethod)
        //                select Expression.Call(outputVariable, AddToDicitonaryMethod, Expression.Constant(prop.Name),
        //                    getExpression));
        //            body.Add(Expression.Return(returnTarget, outputVariable));
        //            body.Add(Expression.Label(returnTarget, Expression.Constant(null, outputType)));

        //            var lambdaExpression = Expression.Lambda<Func<object, IDictionary<string, object>>>(
        //                Expression.Block(new[] { outputVariable }, body),
        //                inputExpression);

        //            return lambdaExpression.Compile();
        //        })(obj);
        //}

        /// <summary>   An object extension method that converts a source to a dictionary. </summary>
        /// <param name="source">   The source to act on. </param>
        /// <returns>   Source as an IDictionary&lt;string,object&gt; </returns>
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        /// <summary>   An object extension method that converts a source to a dictionary. </summary>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="source">   The source to act on. </param>
        /// <returns>   Source as an IDictionary&lt;string,object&gt; </returns>
        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null) ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                object value = property.GetValue(source);
                if (IsOfType<T>(value))
                {
                    dictionary.Add(property.Name, (T)value);
                }
            }
            return dictionary;
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new NullReferenceException("Unable to convert anonymous object to a dictionary. The source anonymous object is null.");
        }

        /// <summary>
        /// Extension method that turns a dictionary of string and object to an ExpandoObject
        ///https://theburningmonk.com/2011/05/idictionarystring-object-to-expandoobject-extension-method/
        /// </summary>
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;
            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries

                    // along the way into expando objects

                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }
                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }
            return expando;
        }
    }
}