namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Linq.Expressions;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    public static class MongoQuery
    {
        #region Public Methods

        public static IMongoQuery Eq(string fieldName, object value)
        {
            IMongoQuery query = null;
            Type type = value.GetType();

            if (value is string)
            {
                bool IsLike = false;
                string txtValue = value.ToString();
                if (txtValue.StartsWith("%") && txtValue.EndsWith("%"))
                {
                    value = String.Format("/{0}/", txtValue);
                    IsLike = true;
                }
                else if (txtValue.StartsWith("%"))
                {
                    value = String.Format("/{0}^/", txtValue);
                    IsLike = true;
                }
                else if (txtValue.EndsWith("%"))
                {
                    value = String.Format("/^{0}/", txtValue);
                    IsLike = true;
                }

                if (IsLike)
                {
                    query = RegEx(fieldName, value.ToString());
                    return query;
                }
            }

            if (type == typeof(DateTime))
            {
                query = Query.EQ(fieldName, (DateTime)value);
            }
            else if (type == typeof(int))
            {
                query = Query.EQ(fieldName, (int)value);
            }
            else if (type == typeof(string))
            {
                query = Query.EQ(fieldName, (string)value);
            }
            else if (type == typeof(long))
            {
                query = Query.EQ(fieldName, (long)value);
            }
            else if (type == typeof(bool))
            {
                query = Query.EQ(fieldName, (bool)value);
            }
            else if (type == typeof(double))
            {
                query = Query.EQ(fieldName, (double)value);
            }
            else if (type == typeof(Guid))
            {
                query = Query.EQ(fieldName, (Guid)value);
            }

            return query;
        }

        public static IMongoQuery Eq<T>(Expression<Func<T, object>> field, object value)
        {
            return Eq(ReflectionUtility.GetPropertyName(field), value);
        }

        public static IMongoQuery Gt(string fieldName, object value)
        {
            IMongoQuery query = null;
            Type type = value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.GT(fieldName, (DateTime)value);
            }
            else if (type == typeof(int))
            {
                query = Query.GT(fieldName, (int)value);
            }
            else if (type == typeof(string))
            {
                query = Query.GT(fieldName, (string)value);
            }
            else if (type == typeof(long))
            {
                query = Query.GT(fieldName, (long)value);
            }
            else if (type == typeof(bool))
            {
                query = Query.GT(fieldName, (bool)value);
            }
            else if (type == typeof(double))
            {
                query = Query.GT(fieldName, (double)value);
            }

            return query;
        }

        public static IMongoQuery Gt<T>(Expression<Func<T, object>> field, object value)
        {
            return Gt(ReflectionUtility.GetPropertyName(field), value);
        }

        public static IMongoQuery Gte(string fieldName, object value)
        {
            IMongoQuery query = null;
            Type type = value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.GTE(fieldName, (DateTime)value);
            }
            else if (type == typeof(int))
            {
                query = Query.GTE(fieldName, (int)value);
            }
            else if (type == typeof(string))
            {
                query = Query.GTE(fieldName, (string)value);
            }
            else if (type == typeof(long))
            {
                query = Query.GTE(fieldName, (long)value);
            }
            else if (type == typeof(bool))
            {
                query = Query.GTE(fieldName, (bool)value);
            }
            else if (type == typeof(double))
            {
                query = Query.GTE(fieldName, (double)value);
            }

            return query;
        }

        public static IMongoQuery Gte<T>(Expression<Func<T, object>> field, object value)
        {
            return Gte(ReflectionUtility.GetPropertyName(field), value);
        }

        public static IMongoQuery Lt(string fieldName, object value)
        {
            IMongoQuery query = null;
            Type type = value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.LT(fieldName, (DateTime)value);
            }
            else if (type == typeof(int))
            {
                query = Query.LT(fieldName, (int)value);
            }
            else if (type == typeof(string))
            {
                query = Query.LT(fieldName, (string)value);
            }
            else if (type == typeof(long))
            {
                query = Query.LT(fieldName, (long)value);
            }
            else if (type == typeof(bool))
            {
                query = Query.LT(fieldName, (bool)value);
            }
            else if (type == typeof(double))
            {
                query = Query.LT(fieldName, (double)value);
            }

            return query;
        }

        public static IMongoQuery Lt<T>(Expression<Func<T, object>> field, object value)
        {
            return Eq(ReflectionUtility.GetPropertyName(field), value);
        }

        public static IMongoQuery Lte(string fieldName, object value)
        {
            IMongoQuery query = null;
            Type type = value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.LTE(fieldName, (DateTime)value);
            }
            else if (type == typeof(int))
            {
                query = Query.LTE(fieldName, (int)value);
            }
            else if (type == typeof(string))
            {
                query = Query.LTE(fieldName, (string)value);
            }
            else if (type == typeof(long))
            {
                query = Query.LTE(fieldName, (long)value);
            }
            else if (type == typeof(bool))
            {
                query = Query.LTE(fieldName, (bool)value);
            }
            else if (type == typeof(double))
            {
                query = Query.LTE(fieldName, (double)value);
            }

            return query;
        }

        public static IMongoQuery Lte<T>(Expression<Func<T, object>> field, object value)
        {
            return Eq(ReflectionUtility.GetPropertyName(field), value);
        }

        public static IMongoQuery Ne(string fieldName, object value)
        {
            IMongoQuery query = null;
            Type type = value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.NE(fieldName, (DateTime)value);
            }
            else if (type == typeof(int))
            {
                query = Query.NE(fieldName, (int)value);
            }
            else if (type == typeof(string))
            {
                query = Query.NE(fieldName, (string)value);
            }
            else if (type == typeof(long))
            {
                query = Query.NE(fieldName, (long)value);
            }
            else if (type == typeof(bool))
            {
                query = Query.NE(fieldName, (bool)value);
            }
            else if (type == typeof(double))
            {
                query = Query.NE(fieldName, (double)value);
            }

            return query;
        }

        public static IMongoQuery Ne<T>(Expression<Func<T, object>> field, object value)
        {
            return Eq(ReflectionUtility.GetPropertyName(field), value);
        }

        public static IMongoQuery RegEx(string fieldName, string expresion)
        {
            return Query.Matches(fieldName, expresion);
        }

        public static IMongoQuery RegEx<T>(Expression<Func<T, object>> field, object value)
        {
            return RegEx(ReflectionUtility.GetPropertyName(field), value.ToString());
        }

        #endregion
    }
}