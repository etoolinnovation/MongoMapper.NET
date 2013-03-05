using System;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public static class MongoQuery<T>
    {
        #region Public Methods

        public static IMongoQuery Eq(Expression<Func<T, string>> Field, object Value)
        {
            return MongoQuery.Eq(typeof(T).Name, ReflectionUtility.GetPropertyName(Field), Value);
        }

        public static IMongoQuery Eq(Expression<Func<T, object>> Field, object Value)
        {
            return MongoQuery.Eq(typeof(T).Name, ReflectionUtility.GetPropertyName(Field), Value);
        }
    
        public static IMongoQuery Gt(Expression<Func<T, object>> Field, object Value)
        {
            return MongoQuery.Gt(typeof(T).Name, ReflectionUtility.GetPropertyName(Field), Value);
        }
        
        public static IMongoQuery Gte(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery.Gte(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }
        
        public static IMongoQuery Lt(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery.Lt(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }
        
        public static IMongoQuery Lte(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery.Lte(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }
        
        public static IMongoQuery Ne(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery.Ne(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }
        
        public static IMongoQuery RegEx(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery.RegEx(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value.ToString());
        }

        #endregion
    }

    public static class MongoQuery
    {
        #region Public Methods

        internal static IMongoQuery Eq(string ObjName, string FieldName, object Value)
        {

            object defaultValue = MongoMapperHelper.GetFieldDefaultValue(ObjName, FieldName);
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

            IMongoQuery query = null;
            Type type = Value.GetType();

            if (type == typeof(string))
            {
                bool isLike = false;
                string txtValue = Value.ToString();
                if (txtValue.StartsWith("%") && txtValue.EndsWith("%"))
                {
                    Value = String.Format("/{0}/", txtValue);
                    isLike = true;
                }
                else if (txtValue.StartsWith("%"))
                {
                    Value = String.Format("/{0}^/", txtValue);
                    isLike = true;
                }
                else if (txtValue.EndsWith("%"))
                {
                    Value = String.Format("/^{0}/", txtValue);
                    isLike = true;
                }

                if (isLike)
                {
                    query = Query.Matches(FieldName, Value.ToString().Replace("%", ""));
                    return query;
                }
            }

            if (type == typeof(DateTime))
            {
                query = Query.EQ(FieldName, (DateTime)Value);
            }
            else if (type == typeof(int))
            {
                query = Query.EQ(FieldName, (int)Value);
            }
            else if (type == typeof(string))
            {
                query = Query.EQ(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Query.EQ(FieldName, (long)Value);
            }
            else if (type == typeof(bool))
            {
                query = Query.EQ(FieldName, (bool)Value);               
                if (defaultValue != null && (bool)defaultValue == (bool)Value)
                    query = Query.Or(query, Query.NotExists(MongoMapperHelper.ConvertFieldName(ObjName, FieldName)));

            }
            else if (type == typeof(double))
            {
                query = Query.EQ(FieldName, (double)Value);
            }
            else if (type == typeof(Guid))
            {
                query = Query.EQ(FieldName, (Guid)Value);
            }

            return query;
        }
      

        public static IMongoQuery Gt(string ObjName, string FieldName, object Value)
        {

            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

            IMongoQuery query = null;
            Type type = Value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.GT(FieldName, (DateTime)Value);
            }
            else if (type == typeof(int))
            {
                query = Query.GT(FieldName, (int)Value);
            }
            else if (type == typeof(string))
            {
                query = Query.GT(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Query.GT(FieldName, (long)Value);
            }
            else if (type == typeof(bool))
            {
                query = Query.GT(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Query.GT(FieldName, (double)Value);
            }

            return query;
        }


        public static IMongoQuery Gte(string ObjName, string FieldName, object Value)
        {

            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);
            IMongoQuery query = null;
            Type type = Value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.GTE(FieldName, (DateTime)Value);
            }
            else if (type == typeof(int))
            {
                query = Query.GTE(FieldName, (int)Value);
            }
            else if (type == typeof(string))
            {
                query = Query.GTE(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Query.GTE(FieldName, (long)Value);
            }
            else if (type == typeof(bool))
            {
                query = Query.GTE(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Query.GTE(FieldName, (double)Value);
            }

            return query;
        }



        public static IMongoQuery Lt(string ObjName, string FieldName, object Value)
        {
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

            IMongoQuery query = null;
            Type type = Value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.LT(FieldName, (DateTime)Value);
            }
            else if (type == typeof(int))
            {
                query = Query.LT(FieldName, (int)Value);
            }
            else if (type == typeof(string))
            {
                query = Query.LT(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Query.LT(FieldName, (long)Value);
            }
            else if (type == typeof(bool))
            {
                query = Query.LT(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Query.LT(FieldName, (double)Value);
            }

            return query;
        }



        public static IMongoQuery Lte(string ObjName, string FieldName, object Value)
        {
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

            IMongoQuery query = null;
            Type type = Value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.LTE(FieldName, (DateTime)Value);
            }
            else if (type == typeof(int))
            {
                query = Query.LTE(FieldName, (int)Value);
            }
            else if (type == typeof(string))
            {
                query = Query.LTE(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Query.LTE(FieldName, (long)Value);
            }
            else if (type == typeof(bool))
            {
                query = Query.LTE(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Query.LTE(FieldName, (double)Value);
            }

            return query;
        }



        public static IMongoQuery Ne(string ObjName, string FieldName, object Value)
        {

            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

            IMongoQuery query = null;
            Type type = Value.GetType();

            if (type == typeof(DateTime))
            {
                query = Query.NE(FieldName, (DateTime)Value);
            }
            else if (type == typeof(int))
            {
                query = Query.NE(FieldName, (int)Value);
            }
            else if (type == typeof(string))
            {
                query = Query.NE(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Query.NE(FieldName, (long)Value);
            }
            else if (type == typeof(bool))
            {
                query = Query.NE(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Query.NE(FieldName, (double)Value);
            }

            return query;
        }



        public static IMongoQuery RegEx(string ObjName, string FieldName, string expresion)
        {
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);
            return Query.Matches(FieldName, expresion);
        }

      

        #endregion
    }
}