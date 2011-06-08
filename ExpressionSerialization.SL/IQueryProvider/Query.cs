﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
namespace ExpressionSerialization
{
	public class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>
		, System.Collections.IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
	{
		QueryProvider provider;
		Expression expression;


		public Query(QueryProvider provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			this.provider = provider;
			this.expression =
				Expression.Constant(this);//this function implicitly calls the ToString method in Debug
		}

		public Query(QueryProvider provider, Expression expression)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
			{
				throw new ArgumentOutOfRangeException("expression");
			}
			this.provider = provider;
			this.expression = expression;
		}

		Expression IQueryable.Expression
		{
			get { return this.expression; }
		}

		Type IQueryable.ElementType
		{
			get { return typeof(T); }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return this.provider; }
		}

		/// <summary>
		/// on the call to any of the System.Linq extension methods on IEnumerable{T}, this
		/// method will get called. <see cref="http://msdn.microsoft.com/en-us/library/system.linq.enumerable.aspx"/>
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)this.provider.Execute(this.expression)).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
		}
		/// <summary>
		/// in Debug, this is called implicitly.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.provider.GetQueryText(this.expression);
		}
	}
}
