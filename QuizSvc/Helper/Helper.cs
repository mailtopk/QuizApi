using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QuizHelper
{
    public static class Helper
    {
        public static Dictionary<string, Object> ExtractBindingsAndValues<T>(Expression<Func<T>> expression)
        {
            if( expression == null || expression.Body == null)
                throw new ArgumentException("Invalid Expression");

            var resultOfBindingAndValue = new Dictionary<string, Object>();
             // Member init expression
            var memberInitExp = expression.Body as MemberInitExpression;
            if(memberInitExp == null)
            {
                return resultOfBindingAndValue;
            }

            foreach(var binding in memberInitExp.Bindings)
            {
                var bindingName = binding.Member.Name;
                var memberAssignment = binding as MemberAssignment;

                var bindingValue = ExtractConstants(memberAssignment.Expression as MemberExpression);
                if(bindingName != null && bindingValue != null)
                {
                    // var fooObj = new foo{ Name = "bar" }
                    // (T) => new foo { Name = fooObj.Name }
                    // TODO - ExtractConstants is called multiple time to do same work - fix this
                    if(bindingValue.GetType().Equals(typeof(T)) )
                    {
                        bindingValue = GetFieldValue(bindingName, bindingValue);
                    }
                    resultOfBindingAndValue.Add(bindingName, bindingValue);
                }
            }
            return resultOfBindingAndValue;
        }

        public static object ExtractConstants(MemberExpression memberExpression)
        {
            if(memberExpression == null)
                return null;

            var constExpression = memberExpression.Expression as ConstantExpression;
            if(constExpression == null)
            {
                // var fooObj = new foo{ Name = "nameValue" }
                // (T) => new foo { Name = fooObj.Name }
                var innerMemberExpression = memberExpression.Expression as MemberExpression;
                if( innerMemberExpression != null)
                {
                    constExpression = innerMemberExpression.Expression as ConstantExpression;
                    if(constExpression == null)
                        throw new InvalidCastException("Can not convert to Constant Expression");

                    memberExpression = innerMemberExpression;
                }
            }
            if (constExpression != null)
            {
                var declaringType = constExpression.Type;
                var declaringObject = constExpression.Value;
                var memberType = memberExpression.Member.MemberType;
                
                if (memberType == MemberTypes.Field)
                {
                    var field = declaringType.GetRuntimeField(memberExpression.Member.Name);
                    return (((FieldInfo)field).GetValue(declaringObject));
                }
            }
            return null;
        }

        private static object GetFieldValue(string bindingName, object bindingValue)
        {
            if(bindingValue == null)
                return null;
            
            var propInfos = bindingValue.GetType().GetRuntimeProperty(bindingName);
            if(propInfos != null)
                return propInfos.GetValue(bindingValue);

            return null;
        }
    }
}
