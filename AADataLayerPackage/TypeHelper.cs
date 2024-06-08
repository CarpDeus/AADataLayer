using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaTools.DataLayer
{

    /// <summary>
    /// Helper class for converting between SQL Server types and .NET types.
    /// </summary>
    internal sealed class TypeHelper
    {
        /// <summary>
        /// Get the .NET type for the given SQL Server type.
        /// </summary>
        /// <param name="sqlType">A valid SqlDbType</param>
        /// <returns>.NET Type</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Type GetClrType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long?);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool?);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime?);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal?);

                case SqlDbType.Float:
                    return typeof(double?);

                case SqlDbType.Int:
                    return typeof(int?);

                case SqlDbType.Real:
                    return typeof(float?);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid?);

                case SqlDbType.SmallInt:
                    return typeof(short?);

                case SqlDbType.TinyInt:
                    return typeof(byte?);

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);

                case SqlDbType.Structured:
                    return typeof(DataTable);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);

                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }

        /// <summary>
        /// Get the SQL Server type for the given .NET type.
        /// </summary>
        /// <param name="clrType">.NET Type</param>
        /// <returns>SqlDbType</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static SqlDbType GetSqlType(Type clrType)
        {
            if(clrType == typeof(long?) || clrType == typeof(long))
            { return SqlDbType.BigInt; }
            if(clrType == typeof(byte[]))
            { return SqlDbType.VarBinary; }
            if (clrType == typeof(bool?) || clrType == typeof(bool))
            { return SqlDbType.Bit; }
            if (clrType == typeof(string))
            { return SqlDbType.NVarChar; }
            if (clrType == typeof(DateTime?) || clrType == typeof(DateTime))
            { return SqlDbType.DateTime; }
            if (clrType == typeof(decimal?) || clrType == typeof(decimal))
            { return SqlDbType.Decimal; }
            if (clrType == typeof(double?) || clrType == typeof(double))
            { return SqlDbType.Float; }
            if (clrType == typeof(int?) || clrType == typeof(int))
            { return SqlDbType.Int; }
            if (clrType == typeof(float?) || clrType == typeof(float))
            { return SqlDbType.Real; }
            if (clrType == typeof(Guid?) || clrType == typeof(Guid))
            { return SqlDbType.UniqueIdentifier; }
            if (clrType == typeof(short?) || clrType == typeof(short))
            { return SqlDbType.SmallInt; }
            if (clrType == typeof(byte?) || clrType == typeof(byte))
            { return SqlDbType.TinyInt; }
            if (clrType == typeof(object))
            { return SqlDbType.Variant; }
            if (clrType == typeof(DataTable))
            { return SqlDbType.Structured; }
            if (clrType == typeof(DateTimeOffset?) || clrType == typeof(DateTimeOffset))
            { return SqlDbType.DateTimeOffset; }
            //TODO: Log errors
            throw new ArgumentOutOfRangeException("clrType");
            
        }
    }
}
