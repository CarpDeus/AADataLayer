using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaTools.DataLayer
{

    /// <summary>
    /// Builds a collection of SQL parameters to pass into the BaseDb class methods
    /// </summary>
    public class ParameterBuilder
    {
        /// <summary>
        /// Private collection of parameters
        /// </summary>
        private Dictionary<string, SqlParameter> _params;


        /// <summary>
        /// Gets the number of parameters in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return (_params != null) ? _params.Count : 0;
            }
        }

        /// <summary>
        /// Array of parameters
        /// </summary>
        public SqlParameter[] Params
        {
            get
            {
                if (_params == null)
                {
                    return null;
                }
                else
                {
                    return _params.Values.ToArray();
                }
            }
        }


        /// <summary>
        /// Object to store sql parameters in for passing to the BaseDb class
        /// </summary>
        public ParameterBuilder()
        {
            _params = new Dictionary<string, SqlParameter>();
        }

        /// <summary>
        /// Creates and stores a parameter
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map</param>
        /// <param name="dbType">One of the System.Data.SqlDbType values</param>
        /// <param name="size">The length of the parameter</param>
        /// <param name="precision">The precision of the parameter</param>
        /// <param name="scale">The scale of the parameter</param>
        /// <param name="direction">One of the System.Data.ParameterDirection values</param>
        /// <param name="parameterValue">The parameterValue of the parameter</param>
        public void Add(string parameterName, SqlDbType dbType, int size, byte precision, byte scale, ParameterDirection direction, object parameterValue)
        {
            // Build parameter object...
            SqlParameter parameter = new SqlParameter(parameterName, dbType);

            if (size != 0)
                parameter.Size = size;

            parameter.Direction = direction;
            parameter.Precision = precision;
            parameter.Scale = scale;

            if (parameterValue == null)
            {
                parameter.Value = DBNull.Value;
            }

            _params.Add(parameterName, parameter);
        }


        /// <summary>
        /// Creates and stores a parameter using the type of the object to infer the SQL type
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map</param>
        /// <param name="parameterValue"></param>
        /// <param name="direction">One of the System.Data.ParameterDirection values</param>
        public void Add(string parameterName, object parameterValue, ParameterDirection direction)
        {
            SqlDbType sqlType = TypeHelper.GetSqlType(parameterValue.GetType());
            SqlParameter parameter = new SqlParameter(parameterName, sqlType);

            parameter.Direction = direction;
            switch(sqlType)
            {
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    parameter.Size = 4000;
                    break;
                case SqlDbType.NText:
                case SqlDbType.Text:
                    parameter.Size = -1;
                    break;
                default:
                    break;
            }

            if (parameterValue == null)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = parameterValue;

            _params.Add(parameterName, parameter);
        }


        /// <summary>
        /// Creates and stores an input parameter - infers the SQL type based on the passed in object
        /// </summary>
        /// <param name="parameterName">The name of the parameter to map</param>
        /// <param name="parameterValue"></param>
        public void Add(string parameterName, object parameterValue)
        {
            Add(parameterName, parameterValue, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates and stores an output parameter - infers the SQL type based on the passed in object
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void AddOut(string parameterName, object value)
        {
            Add(parameterName, value, ParameterDirection.Output);
        }

        /// <summary>
        /// Retrieves parameterValue data from parameters by name 
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <returns>Value of parameter</returns>
        public object GetValue(string parameterName)
        {
            object value = null;

            if (_params != null)
            {
                SqlParameter param;
                if (_params.TryGetValue(parameterName, out param))
                {
                    if (param != null)
                    {
                        value = param.Value;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Retrieves parameterValue from parametes by name, strongly typed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public T GetValue<T>(string parameterName)
        {
            T value = default(T);

            SqlParameter param;
            if (_params != null
                && _params.TryGetValue(parameterName, out param))
            {
                if (param != null
                    && param.Value != DBNull.Value)
                {
                    try
                    {
                        value = (T)Convert.ChangeType(param.Value, typeof(T));
                    }
                    catch
                    {
                        value = default(T);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets whether the parameter exists in the parameter collection.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public bool Contains(string parameterName)
        {
            bool result = false;

            if (_params != null)
            {
                result = _params.ContainsKey(parameterName);
            }

            return result;
        }

    }
}