using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AzureFunctionCall
{
    public class AzureSqlStorage
    {
        private static string _connection = Environment.GetEnvironmentVariable("SqlConnection");

        public static int Save(Details details)
        {
            var query = "insert into Users values (@Id, @FirstName, @Surname, @FullName)";

            using (var connection = new SqlConnection(_connection))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", details.Id);
                command.Parameters.AddWithValue("@FirstName", details.FirstName);
                command.Parameters.AddWithValue("@Surname", details.Surname);
                command.Parameters.AddWithValue("@FullName", details.FullName);

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static IEnumerable<Details> Read<TSource>(Expression<Func<TSource, bool>> filter) 
        {
            var expression = Filter(filter);

            var query = $"select * from Users {expression.opsFilter}";

            var userDetails = new List<Details>();
            using (var connection = new SqlConnection(_connection))
            using (var command = new SqlCommand(query, connection))
            {
                foreach (var item in expression.param)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }                

                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var detail = new Details
                    {
                        Id = (string)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        Surname = (string)reader["Surname"],
                        FullName = (string)reader["FullName"],
                    };

                    userDetails.Add(detail);
                }
            }

            return userDetails;
        }

        private static (Dictionary<string, object> param, string opsFilter) Filter<TSource>(Expression<Func<TSource, bool>> filter)
        {
            var builder = new List<string>();
            var valueDictionary = new Dictionary<string, object>();

            var operators = new Dictionary<ExpressionType, string>
            {
                { ExpressionType.Equal, "=" },
                { ExpressionType.And, "and" },
                { ExpressionType.Or, "or" },
                { ExpressionType.AndAlso, "and" }
            };

            var binaryExpression = (BinaryExpression)filter.Body;
            (List<string> expressionKey, Dictionary<string, object> expressionValue) = ExpressionDetails(binaryExpression, builder, valueDictionary, operators);

            var filterString = new StringBuilder("where");

            var keyCount = expressionKey.Count();
            for (int i = 0; i < keyCount - 1; i++)
            {
                filterString.Append($" {expressionKey[i]} ");
            }

            return (expressionValue, filterString.ToString());
        }

        private static (List<string> expressionKey, Dictionary<string, object> expressionValue) ExpressionDetails(BinaryExpression binaryExpression,
            List<string> condition,
            Dictionary<string, object> dictionary,
            Dictionary<ExpressionType, string> operators,
            string nodeType = "")
        {
            if (binaryExpression.Right.GetType().Name == "PropertyExpression" && binaryExpression.Left.GetType().Name == "PropertyExpression")
            {
                (string expressionKey, object expressionValue) = ExpressionKeyValue(binaryExpression);

                if (!dictionary.ContainsKey(expressionKey))
                {
                    condition.Add(expressionKey);
                    condition.Add(operators[binaryExpression.NodeType]);
                    condition.Add($"@{expressionKey}");
                    condition.Add(nodeType);
                    dictionary.Add(expressionKey, expressionValue);
                }
            }

            if (binaryExpression.Right.GetType().Name == "MethodBinaryExpression")
            {
                ExpressionDetails((BinaryExpression)binaryExpression.Right, condition, dictionary, operators, operators[binaryExpression.NodeType]);
            }

            if (binaryExpression.Left.GetType().Name == "MethodBinaryExpression")
            {
                ExpressionDetails((BinaryExpression)binaryExpression.Left, condition, dictionary, operators, operators[binaryExpression.NodeType]);
            }

            if (binaryExpression.Left.GetType().Name == "LogicalBinaryExpression")
            {
                var aa = (BinaryExpression)binaryExpression.Left;
                ExpressionDetails((BinaryExpression)aa.Right, condition, dictionary, operators, operators[aa.NodeType]);
                ExpressionDetails((BinaryExpression)aa.Left, condition, dictionary, operators, operators[aa.NodeType]);
            }           

            return (condition, dictionary);
        }

        private static (string expressionKey, object expressionValue) ExpressionKeyValue(BinaryExpression binaryExpression)
        {
            var key = (MemberExpression)binaryExpression.Right;
            var memberExpression = (MemberExpression)key.Expression;
            var captureConst = (ConstantExpression)memberExpression.Expression;
            var details = ((FieldInfo)memberExpression.Member).GetValue(captureConst.Value);
            var value = ((PropertyInfo)key.Member).GetValue(details, null);

            return (key.Member.Name, value);
        }
    }
}
