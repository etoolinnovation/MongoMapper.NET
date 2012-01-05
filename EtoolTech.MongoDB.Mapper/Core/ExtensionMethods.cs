using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ExtensionMethods
    {
        public static T FindByKey<T>(this T o, params object[] values)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotSupportedException();
            }
            return Finder.Instance.FindByKey<T>(values);

            //object result = Finder.Instance.FindByKey<T>(values);
            //Conversion<T,T>.From((T) result, ref o); 
        }

        public static void MongoFind<T>(this List<T> list, QueryComplete query = null)
        {
            if (typeof (T).BaseType != typeof (MongoMapper))
            {
                throw new NotSupportedException();
            }
            list.Clear();
            list.AddRange(query == null ? Finder.Instance.AllAsList<T>() : Finder.Instance.FindAsList<T>(query));
        }

        public static void MongoFind<T>(this List<T> list, string fieldName, object value)
        {
            if (typeof (T).BaseType != typeof (MongoMapper))
            {
                throw new NotSupportedException();
            }
            list.Clear();
            list.AddRange(Finder.Instance.FindAsList<T>(MongoQuery.Eq(fieldName, value)));
        }

        public static void MongoFind<T>(this List<T> list, Expression<Func<T, object>> exp)
        {
            if (typeof (T).BaseType != typeof (MongoMapper))
            {
                throw new NotSupportedException();
            }
            list.Clear();
            list.AddRange(Finder.Instance.FindAsList(exp));
        }
    }

    //Basado en: http://www.differentpla.net/content/2009/09/using-expressiont-compiler-avoid-writing-conversion-code
    static class Conversion<TInput, TOutput>
    {
        private static readonly Func<TInput, TOutput> Converter;
        private static object obj = null;

        static Conversion()
        {
            Converter = CreateConverter(obj);
        }

        static Func<TInput, TOutput> CreateConverter(object o = null)
        {
            var input = Expression.Parameter(typeof(TInput), "input");
            
            var destinationProperties = typeof(TOutput)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.CanWrite);
            var sourceProperties = typeof(TInput)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.CanRead);

            var memberBindings = sourceProperties.Join(destinationProperties,
                sourceProperty => sourceProperty.Name,
                destinationProperty => destinationProperty.Name,
                (sourceProperty, destinationProperty) =>
                    (MemberBinding)Expression.Bind(destinationProperty,
                        Expression.Property(input, sourceProperty)));

            MemberInitExpression body = Expression.MemberInit(Expression.New(typeof(TOutput)), memberBindings);
            var lambda = Expression.Lambda<Func<TInput, TOutput>>(body, input);
            return lambda.Compile();            
        }

        public static TOutput From(TInput input)
        {
            return Converter(input);
        }

        public static void From(TInput input, ref TOutput output)
        {
            obj = output;
            Converter(input);           
        }
    }

}

