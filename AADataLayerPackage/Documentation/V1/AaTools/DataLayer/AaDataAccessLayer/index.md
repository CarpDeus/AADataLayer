﻿<!--  
  <auto-generated>   
    The contents of this file were generated by a tool.  
    Changes to this file may be list if the file is regenerated  
  </auto-generated>   
-->

# AaDataAccessLayer Class

**Namespace:** [AaTools.DataLayer](../index.md)  
**Assembly:** AaDataLayer  
**Assembly Version:** 1.0.0+5890e7dfdc873f8403b1635b629b41e8973b63a9

Class for all database calls.  This class is abstract and should not be instantiated.  It is used to provide common functionality for database calls.

```csharp
public abstract class AaDataAccessLayer
```

**Inheritance:** object → AaDataAccessLayer

## Methods

| Name                                                                                                                        | Description                                                                    |
| --------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| [EndExecuteNonQueryAsync(IAsyncResult)](methods/EndExecuteNonQueryAsync.md)                                                 | End of the ExecuteNonQueryAsync                                                |
| [ExecuteDataset(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteDataset.md)                              | Call a stored procedure and return a DataSet                                   |
| [ExecuteDatatable(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteDatatable.md)                          | Call a stored procedure and return a DataTable                                 |
| [ExecuteDatatableAsync(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteDatatableAsync.md)                | Return a database table asynchronously                                         |
| [ExecuteJsonObjectAsync\<T\>(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteJsonObjectAsync.md)         | Asynchronously call stored procedure that returns Json Object T                |
| [ExecuteJsonStringAsync(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteJsonStringAsync.md)              | Asynchronously call stored procedure that returns Json as String               |
| [ExecuteNonQuery(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteNonQuery.md)                            | Call a stored procedure that returns no data                                   |
| [ExecuteNonQueryAsync(string, ParameterBuilder, string, bool)](methods/ExecuteNonQueryAsync.md)                             | Asynchronously call a stored procedure that returns no data                    |
| [ExecuteObjectJson\<T\>(string, ParameterBuilder, string, string, int, bool, bool)](methods/ExecuteObjectJson.md)           | Return Json Object when stored procedure uses output parameter                 |
| [ExecuteObjectJsonAsync\<T\>(string, ParameterBuilder, string, string, bool)](methods/ExecuteObjectJsonAsync.md)            | Asynchronouslyl Return Json Object when stored procedure uses output parameter |
| [ExecuteScalar\<T\>(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteScalar.md)                           | Execute a stored procedure that returns a scalar object T                      |
| [ExecuteWithParamReturns(string, ParameterBuilder, string, int, bool, bool)](methods/ExecuteWithParamReturns.md)            |                                                                                |
| [ExecuteWithReturnParam\<T\>(string, ParameterBuilder, string, string, int, bool, bool)](methods/ExecuteWithReturnParam.md) | Execute a stored procedure with a return parameter of a simple object type     |

___

*Documentation generated by [MdDocs](https://github.com/ap0llo/mddocs)*