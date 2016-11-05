using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.Expand
{
    public static class ExpandQueryable
    {
        private static readonly ConcurrentDictionary<string, LambdaExpression> Cache = new ConcurrentDictionary<string, LambdaExpression>();

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));
            return Queryable.OrderBy(source, keySelector);
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));
            return Queryable.OrderByDescending(source, keySelector);
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));

            return Queryable.ThenBy(source, keySelector);

        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));
            return Queryable.ThenByDescending(source, keySelector);
        }
        private static LambdaExpression GetLambdaExpression(string propertyName, Type type)
        {
            string key = type.FullName + "_" + propertyName;
            if (Cache.ContainsKey(key))
            {
                return Cache[key];
            }
            ParameterExpression param = Expression.Parameter(type);
            MemberExpression body = Expression.Property(param, propertyName);
            LambdaExpression keySelector = Expression.Lambda(body, param);
            Cache[key] = keySelector;
            return keySelector;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                           Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static DataTable CreateSchema<T>(this IQueryable<T> source) where T : class
        {
            var t = typeof(T);
            var ps = t.GetProperties();
            DataTable dt = new DataTable();
            foreach (var p in ps)
            {
                dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
            }
            return dt;
        }
        public static DataTable ToDataTable<T>(this IQueryable<T> source) where T : class
        {
            var t = typeof(T);
            var ps = t.GetProperties();
            DataTable dt = new DataTable();
            foreach (var p in ps)
            {
                dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
            }

            foreach (var item in source)
            {
                var row = dt.NewRow();

                foreach (var p in ps)
                {
                    row[p.Name] = p.GetValue(item,null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }


        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
