using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public static class MongoQuery<T>
    {
        #region Public Methods


        public static FilterDefinition<T> Eq(string ObjName, string FieldName, object Value)
        {

            object defaultValue = MongoMapperHelper.GetFieldDefaultValue(ObjName, FieldName);
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

            FilterDefinition<T> query = null;
            Type type = Value.GetType();

            if (type == typeof(BsonNull))
            {
                query = Builders<T>.Filter.Eq(FieldName, BsonNull.Value);
                return query;
            }

            if (type.BaseType != null && type.BaseType.Name == "Enum")
            {
                type = typeof(int);
                if ((int)Value < 0)
                {                    
                    return new BsonDocument();
                }
            }

            if (type == typeof(string))
            {
                bool isLike = false;
                string txtValue = Value.ToString();

                if (txtValue.Trim() == "%")
                {
                    return new BsonDocument();
                }

                if (txtValue.StartsWith("%") && txtValue.EndsWith("%"))
                {
                    Value = String.Format("/{0}/", txtValue);
                    isLike = true;
                }
                else if (txtValue.StartsWith("%"))
                {
                    Value = String.Format("/{0}$/", txtValue);
                    isLike = true;
                }
                else if (txtValue.EndsWith("%"))
                {
                    Value = String.Format("/^{0}/", txtValue);
                    isLike = true;
                }

                if (isLike)
                {                                        
                    query =  Builders<T>.Filter.Regex(FieldName, new BsonRegularExpression(string.Format("{0}i", Value.ToString().Replace("%", ""))));
                    return query;
                }
            }

            if (type == typeof(DateTime))
            {
                query = Builders<T>.Filter.Eq(FieldName, (DateTime)Value);
                if (defaultValue != null && (DateTime)defaultValue == (DateTime)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(int))
            {
                query = Builders<T>.Filter.Eq(FieldName, (int)Value);
                if (defaultValue != null && (int)defaultValue == (int)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(string))
            {
                query = Builders<T>.Filter.Eq(FieldName, (string)Value);
                if (defaultValue != null && (string)defaultValue == (string)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(long))
            {
                query = Builders<T>.Filter.Eq(FieldName, (long)Value);
                if (defaultValue != null && (long)defaultValue == (long)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(bool))
            {
                query = Builders<T>.Filter.Eq(FieldName, (bool)Value);
                if (defaultValue != null && (bool)defaultValue == (bool)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(double))
            {
                query = Builders<T>.Filter.Eq(FieldName, (double)Value);
                if (defaultValue != null && (double)defaultValue == (double)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(Guid))
            {
                query = Builders<T>.Filter.Eq(FieldName, (Guid)Value);
                if (defaultValue != null && (Guid)defaultValue == (Guid)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }

            return query;
        }

        public static FilterDefinition<T> Eq(Expression<Func<T, object>> Field, object Value)
        {
            return MongoQuery<T>.Eq(typeof(T).Name, ReflectionUtility.GetPropertyName(Field), Value);
        }
      
    
        public static FilterDefinition<T> Gt(Expression<Func<T, object>> Field, object Value)
        {
            return MongoQuery<T>.Gt(typeof(T).Name, ReflectionUtility.GetPropertyName(Field), Value);
        }


        public static FilterDefinition<T> Gt(string ObjName, string FieldName, object Value)
        {

            object defaultValue = MongoMapperHelper.GetFieldDefaultValue(ObjName, FieldName);
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

             FilterDefinition<T> query = null;
            Type type = Value.GetType();

            if (type.BaseType != null && type.BaseType.Name == "Enum") type = typeof(int);

            if (type == typeof(DateTime))
            {
                query = Builders<T>.Filter.Gt(FieldName, (DateTime)Value);
                if (defaultValue != null && (DateTime)defaultValue > (DateTime)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(int))
            {
                query = Builders<T>.Filter.Gt(FieldName, (int)Value);
                if (defaultValue != null && (int)defaultValue > (int)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(string))
            {
                query = Builders<T>.Filter.Gt(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Builders<T>.Filter.Gt(FieldName, (long)Value);
                if (defaultValue != null && (long)defaultValue > (long)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(bool))
            {
                query = Builders<T>.Filter.Gt(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Builders<T>.Filter.Gt(FieldName, (double)Value);
                if (defaultValue != null && (double)defaultValue > (double)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }

            return query;
        }

        public static FilterDefinition<T> Gte(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery<T>.Gte(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }


        public static FilterDefinition<T> Gte(string ObjName, string FieldName, object Value)
        {

            object defaultValue = MongoMapperHelper.GetFieldDefaultValue(ObjName, FieldName);
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

             FilterDefinition<T> query = null;
            Type type = Value.GetType();

            if (type.BaseType != null && type.BaseType.Name == "Enum") type = typeof(int);

            if (type == typeof(DateTime))
            {
                query = Builders<T>.Filter.Gte(FieldName, (DateTime)Value);
                if (defaultValue != null && (DateTime)defaultValue >= (DateTime)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(int))
            {
                query = Builders<T>.Filter.Gte(FieldName, (int)Value);
                if (defaultValue != null && (int)defaultValue >= (int)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(string))
            {
                query = Builders<T>.Filter.Gte(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Builders<T>.Filter.Gte(FieldName, (long)Value);
                if (defaultValue != null && (long)defaultValue >= (long)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(bool))
            {
                query = Builders<T>.Filter.Gte(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Builders<T>.Filter.Gte(FieldName, (double)Value);
                if (defaultValue != null && (double)defaultValue >= (double)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }

            return query;
        }

        public static FilterDefinition<T> Lt(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery<T>.Lt(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }

        public static FilterDefinition<T> Lt(string ObjName, string FieldName, object Value)
        {
            object defaultValue = MongoMapperHelper.GetFieldDefaultValue(ObjName, FieldName);
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

             FilterDefinition<T> query = null;
            Type type = Value.GetType();

            if (type.BaseType != null && type.BaseType.Name == "Enum") type = typeof(int);

            if (type == typeof(DateTime))
            {
                query = Builders<T>.Filter.Lt(FieldName, (DateTime)Value);
                if (defaultValue != null && (DateTime)defaultValue < (DateTime)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(int))
            {
                query = Builders<T>.Filter.Lt(FieldName, (int)Value);
                if (defaultValue != null && (int)defaultValue < (int)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(string))
            {
                query = Builders<T>.Filter.Lt(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Builders<T>.Filter.Lt(FieldName, (long)Value);
                if (defaultValue != null && (long)defaultValue < (long)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(bool))
            {
                query = Builders<T>.Filter.Lt(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Builders<T>.Filter.Lt(FieldName, (double)Value);
                if (defaultValue != null && (double)defaultValue < (double)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }

            return query;
        }

        public static FilterDefinition<T> Lte(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery<T>.Lte(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }

        public static FilterDefinition<T> Lte(string ObjName, string FieldName, object Value)
        {
            object defaultValue = MongoMapperHelper.GetFieldDefaultValue(ObjName, FieldName);
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

            FilterDefinition<T> query = null;
            Type type = Value.GetType();

            if (type.BaseType != null && type.BaseType.Name == "Enum") type = typeof(int);

            if (type == typeof(DateTime))
            {
                query = Builders<T>.Filter.Lte(FieldName, (DateTime)Value);
                if (defaultValue != null && (DateTime)defaultValue <= (DateTime)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(int))
            {
                query = Builders<T>.Filter.Lte(FieldName, (int)Value);
                if (defaultValue != null && (int)defaultValue <= (int)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(string))
            {
                query = Builders<T>.Filter.Lte(FieldName, (string)Value);
            }
            else if (type == typeof(long))
            {
                query = Builders<T>.Filter.Lte(FieldName, (long)Value);
                if (defaultValue != null && (long)defaultValue <= (long)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(bool))
            {
                query = Builders<T>.Filter.Lte(FieldName, (bool)Value);
            }
            else if (type == typeof(double))
            {
                query = Builders<T>.Filter.Lte(FieldName, (double)Value);
                if (defaultValue != null && (double)defaultValue <= (double)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }

            return query;
        }


        public static FilterDefinition<T> Ne(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery<T>.Ne(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value);
        }


        public static FilterDefinition<T> Ne(string ObjName, string FieldName, object Value)
        {

            object defaultValue = MongoMapperHelper.GetFieldDefaultValue(ObjName, FieldName);
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);

             FilterDefinition<T> query = null;
            Type type = Value.GetType();
            if (type.BaseType != null && type.BaseType.Name == "Enum") type = typeof(int);

            if (type == typeof(DateTime))
            {
                query = Builders<T>.Filter.Ne(FieldName, (DateTime)Value);
                if (defaultValue != null && (DateTime)defaultValue != (DateTime)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));
            }
            else if (type == typeof(int))
            {
                query = Builders<T>.Filter.Ne(FieldName, (int)Value);
                if (defaultValue != null && (int)defaultValue != (int)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(string))
            {
                query = Builders<T>.Filter.Ne(FieldName, (string)Value);
                if (defaultValue != null && (string)defaultValue != (string)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(long))
            {
                query = Builders<T>.Filter.Ne(FieldName, (long)Value);
                if (defaultValue != null && (long)defaultValue != (long)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(bool))
            {
                query = Builders<T>.Filter.Ne(FieldName, (bool)Value);
                if (defaultValue != null && (bool)defaultValue != (bool)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }
            else if (type == typeof(double))
            {
                query = Builders<T>.Filter.Ne(FieldName, (double)Value);
                if (defaultValue != null && (double)defaultValue != (double)Value)
                    query = Builders<T>.Filter.Or(query, Builders<T>.Filter.Exists(FieldName, false));

            }

            return query;
        }

        public static FilterDefinition<T> RegEx(Expression<Func<T, object>> field, object Value)
        {
            return MongoQuery<T>.RegEx(typeof(T).Name, ReflectionUtility.GetPropertyName(field), Value.ToString());
        }


        public static FilterDefinition<T> RegEx(string ObjName, string FieldName, string expresion)
        {
            FieldName = MongoMapperHelper.ConvertFieldName(ObjName, FieldName);
            return Builders<T>.Filter.Regex(FieldName, expresion);
        }


        #endregion
    }
}