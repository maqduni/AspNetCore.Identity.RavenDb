using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Maqduni.RavenDb.Extensions
{
    public static class RavenDbDocumentSessionExtensions
    {
        public static async Task<T> LoadByUniqueExchangeValueAsync<T>(this IAsyncDocumentSession session, Expression<Func<T, object>> keySelector, object value)
        {
            if (value == null) throw new ArgumentNullException("value", "The unique value cannot be null");
            var typeName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(T));
            var body = GetMemberExpression(keySelector);
            var propertyName = body.Member.Name;
            var att = (UniqueExchangeValueAttribute)body.Member.GetCustomAttribute(typeof(UniqueExchangeValueAttribute));

            var escapedValue = EscapeUniqueValue(value, att.CaseInsensitive);
            var uniqueId = "UniqueExchangeValue/" + typeName.ToLowerInvariant() + "/" + propertyName.ToLowerInvariant() + "/" + escapedValue;

            var getOperation = new GetCompareExchangeValueOperation<string>(uniqueId);
            var getResult = await session.Advanced.DocumentStore.Operations.SendAsync(getOperation);

            if (getResult.Value == null)
                return default(T);

            session.Advanced.Evict(getResult.Value);

            return await session.LoadAsync<T>(getResult.Value).ConfigureAwait(false);
        }

        private static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> keySelector)
        {
            MemberExpression body;
            if (keySelector.Body is MemberExpression)
            {
                body = ((MemberExpression)keySelector.Body);
            }
            else
            {
                var op = ((UnaryExpression)keySelector.Body).Operand;
                body = ((MemberExpression)op);
            }

            var isDef = body.Member.IsDefined(typeof(UniqueExchangeValueAttribute));

            if (isDef == false)
            {
                var msg = string.Format(
                    "You are calling LoadByUniqueConstraints on {0}.{1}, but you haven't marked this property with [UniqueConstraint]",
                    body.Member.DeclaringType.Name, body.Member.Name);
                throw new InvalidOperationException(msg);
            }
            return body;
        }

        public static string EscapeUniqueValue(object value, bool caseInsensitive = false)
        {
            var stringToEscape = value.ToString();
            if (caseInsensitive)
                stringToEscape = stringToEscape.ToLowerInvariant();
            var escapeDataString = Uri.EscapeDataString(stringToEscape);
            if (stringToEscape == escapeDataString)
                return stringToEscape;
            // to avoid issues with ids, we encode the entire thing as safe Base64
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(stringToEscape));
        }
    }
}
