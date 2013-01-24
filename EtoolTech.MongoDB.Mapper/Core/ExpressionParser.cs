using System;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class ExpressionParser
    {
        #region Public Methods

        public IMongoQuery ParseExpression<T>(Expression<Func<T, object>> exp)
        {
            throw new NotImplementedException();
            //Parse(exp);

            ////TODO temporal Solo AND;
            //return Query.And(_bufferOptions.Select(
            //    bufferOption =>
            //    Finder.Instance.GetQuery(bufferOption.FindCondition, bufferOption.Value, bufferOption.FieldName)).ToArray());
        }

        #endregion

        /*

        private void Parse<T>(Expression<Func<T, object>> exp)
        {
            var expression = (UnaryExpression) exp.Body;
            var operand = (BinaryExpression) expression.Operand;

            Parse(operand, operand.NodeType);
        }

        private void Parse(BinaryExpression op2, ExpressionType parentOperator)
        {
            bool isBinary = false;
            try
            {
                var o = (BinaryExpression) op2.Right;
                Parse(o, op2.NodeType);
                isBinary = true;
            }
            catch (InvalidCastException)
            {
            }

            try
            {
                var o = (BinaryExpression) op2.Left;
                Parse(o, op2.NodeType);
                isBinary = true;
            }
            catch (InvalidCastException)
            {
            }

            try
            {
                var o = (UnaryExpression) op2.Right;
                var operand = (MemberExpression) o.Operand;
                string name = operand.Member.Name;
                string typeName = operand.Member.DeclaringType.Name;
                SetValues(typeName, name, false, ExpressionType.Equal, op2.NodeType);
                isBinary = true;
            }
            catch (InvalidCastException)
            {
            }

            try
            {
                var o = (UnaryExpression) op2.Left;
                var operand = (MemberExpression) o.Operand;
                string name = operand.Member.Name;
                string typeName = operand.Member.DeclaringType.Name;
                SetValues(typeName, name, false, ExpressionType.Equal, op2.NodeType);
                isBinary = true;
            }
            catch (InvalidCastException)
            {
            }

            //MemberExpression
            try
            {
                var o = (MemberExpression) op2.Right;
                if (o.Type == typeof (bool))
                {
                    string name = o.Member.Name;
                    string typeName = o.Member.DeclaringType.Name;
                    SetValues(typeName, name, true, ExpressionType.Equal, parentOperator);
                    isBinary = true;
                }
            }
            catch (InvalidCastException)
            {
            }

            try
            {
                var o = (MemberExpression) op2.Left;
                if (o.Type == typeof (bool))
                {
                    string name = o.Member.Name;
                    string typeName = o.Member.DeclaringType.Name;
                    SetValues(typeName, name, true, ExpressionType.Equal, parentOperator);
                    isBinary = true;
                }
            }
            catch (InvalidCastException)
            {
            }

            if (isBinary) return;
            object value;
            //TODO: Probar si se controla bien cuando es una constante
            try
            {
                var right = (MemberExpression)op2.Right;
                value = GetValue(right);
            }
            catch(InvalidCastException)
            {
                try
                {
                    var right = (ConstantExpression)op2.Right;
                    value = right.Value;

                }
                catch (InvalidCastException)
                {
                    var right = (MethodCallExpression)op2.Right;
                    value = Expression.Lambda(right).Compile().DynamicInvoke();                    
                }
            }
            
            var left = (MemberExpression) op2.Left;

            SetValues(left.Member.DeclaringType.Name, left.Member.Name, value, op2.NodeType, parentOperator);
        }


        private static object GetValue(MemberExpression member)
        {
            UnaryExpression objectMember = Expression.Convert(member, typeof (object));

            Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);

            Func<object> getter = getterLambda.Compile();

            return getter();
        }

        private void SetValues(string className, string fieldName, object value, ExpressionType op,
                               ExpressionType globalop)
        {
     
            var mongoMapperCondition = new FindCondition();

            ExpressionType oper = op;
            switch (oper)
            {
                case ExpressionType.Equal:
                    {
                        mongoMapperCondition = FindCondition.Equal;
                        break;
                    }
                case ExpressionType.NotEqual:
                    {
                        mongoMapperCondition = FindCondition.NotEquals;
                        break;
                    }
                case ExpressionType.GreaterThan:
                    {
                        mongoMapperCondition = FindCondition.Greater;
                        break;
                    }
                case ExpressionType.GreaterThanOrEqual:
                    {
                        mongoMapperCondition = FindCondition.GreaterOrEqual;
                        break;
                    }
                case ExpressionType.LessThan:
                    {
                        mongoMapperCondition = FindCondition.Lees;
                        break;
                    }
                case ExpressionType.LessThanOrEqual:
                    {
                        mongoMapperCondition = FindCondition.LessOrEqueal;
                        break;
                    }
                default:
                    {
                        mongoMapperCondition = FindCondition.Equal;
                        break;
                    }
            }

            object pOp = globalop;

         
        }
         */
    }
}