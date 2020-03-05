using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace COASqlQuery
{
    public class ReferanceAllTypes : ExpressionVisitor
    {
        private readonly Type _ownerType;
        public List<string> Ad = new List<string>();
        public List<string> Valuee = new List<string>();
        public List<string> EqualType = new List<string>();
        public List<string> AndAlsoOrOr = new List<string>();
        public List<Type> type = new List<Type>();

        public ReferanceAllTypes(Type ownerType)
        {
            _ownerType = ownerType;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (Ad.Count != Valuee.Count)
            {
                type.Add(node.Type);
                if (node.Type == typeof(string))
                    Valuee.Add("'" + node.Value.ToString() + "'");
                else
                    Valuee.Add(node.Value.ToString());


            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var iteme = node.Method.Name;
            EqualType.Add(iteme);
            return base.VisitMethodCall(node);
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal
                   | node.NodeType == ExpressionType.NotEqual
                   | node.NodeType == ExpressionType.GreaterThan
                   | node.NodeType == ExpressionType.LessThan
                   | node.NodeType == ExpressionType.GreaterThanOrEqual
                   | node.NodeType == ExpressionType.LessThanOrEqual
                   )
            {
                EqualType.Add(node.NodeType.ToString());

            }
            else if (node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.Or || node.NodeType == ExpressionType.OrElse)
            {
                AndAlsoOrOr.Add(node.NodeType.ToString());
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {


            if (node.NodeType == ExpressionType.MemberAccess && node.Expression.NodeType == ExpressionType.Constant)
            {
                var cleanNode = GetMemberConstant(node);
                if (cleanNode.Type.Namespace == "System")
                {
                    type.Add(cleanNode.Type);
                    if (cleanNode.Type == typeof(string))
                        Valuee.Add("'" + cleanNode.Value.ToString() + "'");
                    else
                        Valuee.Add(cleanNode.Value.ToString());
                }

            }
            if (node.NodeType == ExpressionType.MemberAccess && node.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var cleanNode = GetMemberConstant(node);
                if (cleanNode.Type.Namespace == "System")
                {
                    type.Add(cleanNode.Type);

                    if (cleanNode.Type == typeof(string))
                        Valuee.Add("'" + cleanNode.Value.ToString() + "'");
                    else
                        Valuee.Add(cleanNode.Value.ToString());
                }


            }

            var propertyInfo = node.Member as PropertyInfo;

            if (propertyInfo != null && _ownerType.IsAssignableFrom(propertyInfo.DeclaringType) && node.Expression.NodeType == ExpressionType.Parameter)
            {
                Ad.Add(propertyInfo.Name);
            }


            if (node.Member.DeclaringType.IsDefined(typeof(CompilerGeneratedAttribute), true))
            {
                object target = ((ConstantExpression)node.Expression).Value, value;
                switch (node.Member.MemberType)
                {
                    case MemberTypes.Property:
                        value = ((PropertyInfo)node.Member).GetValue(target, null);
                        break;
                    case MemberTypes.Field:
                        value = ((FieldInfo)node.Member).GetValue(target);
                        break;
                    default:
                        value = target = null;
                        break;
                }
                if (target != null) return Expression.Constant(value, node.Type);
            }

            return base.VisitMember(node);
        }

        private static ConstantExpression GetMemberConstant(MemberExpression node)
        {
            object value;

            if (node.Member.MemberType == MemberTypes.Field)
            {
                value = GetFieldValue(node);
            }
            else if (node.Member.MemberType == MemberTypes.Property)
            {
                value = GetPropertyValue(node);
            }
            else
            {
                throw new NotSupportedException();
            }

            return Expression.Constant(value, node.Type);
        }
        private static object GetFieldValue(MemberExpression node)
        {
            var fieldInfo = (FieldInfo)node.Member;

            var instance = (node.Expression == null) ? null : TryEvaluate(node.Expression).Value;

            return fieldInfo.GetValue(instance);
        }

        private static object GetPropertyValue(MemberExpression node)
        {
            var propertyInfo = (PropertyInfo)node.Member;

            var instance = (node.Expression == null) ? null : TryEvaluate(node.Expression).Value;

            return propertyInfo.GetValue(instance, null);
        }
        public bool RepeatProtoc = false;
        private static ConstantExpression TryEvaluate(Expression expression)
        {

            if (expression.NodeType == ExpressionType.Constant)
            {
                return (ConstantExpression)expression;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {

                return GetMemberConstant((MemberExpression)expression);
            }
            throw new NotSupportedException();

        }
    }
}
