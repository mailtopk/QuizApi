using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QuizHealper
{
    public static class Healper
    {
        public static Dictionary<string, object> ExtractBindingsAndValues<T>(Expression<Func<T>> expression)
        {
            if( expression == null || expression.Body == null)
                throw new ArgumentException("Invalid Expression");
                
            var resultOfBindingAndValue = new Dictionary<string, Object>();
             // Member init expression
            var memberInitExp = expression.Body as MemberInitExpression;
            if(memberInitExp == null)
                return resultOfBindingAndValue;

            foreach(var binding in memberInitExp.Bindings)
            {
                var bindingName = binding.Member.Name;
                var memberAssignment = binding as MemberAssignment;

                var bindingValue = ExtractConstants(memberAssignment.Expression as MemberExpression);

                if(bindingName != null && bindingValue != null)
                {
                    resultOfBindingAndValue.Add(bindingName, bindingValue);
                }
            }

            return resultOfBindingAndValue;
        }
        public static object ExtractConstants(MemberExpression memberExpression)
        {
            var constExpression = (ConstantExpression)memberExpression.Expression;

            if (constExpression != null)
            {
                var declaringType = constExpression.Type;
                var declaringObject = constExpression.Value;
                var member = declaringType.GetRuntimeField(memberExpression.Member.Name);

                if (member.MemberType == MemberTypes.Field)
                    return (((FieldInfo)member).GetValue(declaringObject));
            }
            return null;
        }
    }
}
