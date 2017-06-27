using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DangerousReflection {
	/// <summary>
	/// Cache for MethodInfo
	/// </summary>
	internal class MethodInfoCache {
		public Func<object, object[], object> Invoker => _invoker;
		private Func<object, object[], object> _invoker;

		/// <summary>
		/// Create dynamic delegate from method info
		/// </summary>
		public static Func<object, object[], object> LambdaMakeInvoker(MethodInfo methodInfo) {
			var instanceParameter = Expression.Parameter(typeof(object), "instance");
			var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");
			var parameterExpressions = new List<Expression>();
			var paramInfos = methodInfo.GetParameters();
			for (int i = 0; i < paramInfos.Length; i++) {
				var valueObj = Expression.ArrayIndex(
					parametersParameter, Expression.Constant(i));
				var valueCast = Expression.Convert(
					valueObj, paramInfos[i].ParameterType);
				parameterExpressions.Add(valueCast);
			}
			var instanceCast = methodInfo.IsStatic ? null :
				Expression.Convert(instanceParameter, methodInfo.DeclaringType);
			var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);
			if (methodCall.Type == typeof(void)) {
				var lambda = Expression.Lambda<Action<object, object[]>>(
						methodCall, instanceParameter, parametersParameter);
				Action<object, object[]> execute = lambda.Compile();
				return (instance, parameters) => {
					execute(instance, parameters);
					return null;
				};
			} else {
				var castMethodCall = Expression.Convert(methodCall, typeof(object));
				var lambda = Expression.Lambda<Func<object, object[], object>>(
					castMethodCall, instanceParameter, parametersParameter);
				return lambda.Compile();
			}
		}

		/// <summary>
		/// Initialize
		/// </summary>
		public MethodInfoCache(MethodInfo methodInfo) {
			_invoker = LambdaMakeInvoker(methodInfo);
		}
	}
}
