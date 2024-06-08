﻿<!--  
  <auto-generated>   
    The contents of this file were generated by a tool.  
    Changes to this file may be list if the file is regenerated  
  </auto-generated>   
-->

# AaDataAccessLayer.ExecuteJsonObjectAsync Method

**Declaring Type:** [AaDataAccessLayer](../index.md)  
**Namespace:** [AaTools.DataLayer](../../index.md)  
**Assembly:** AaDataLayer  
**Assembly Version:** 1.0.0+5890e7dfdc873f8403b1635b629b41e8973b63a9

Asynchronously call stored procedure that returns Json Object T

```csharp
[AsyncStateMachine(AaTools.DataLayer.AaDataAccessLayer/<ExecuteJsonObjectAsync>d__5`1)]
[DebuggerStepThrough]
public static Task<T> ExecuteJsonObjectAsync<T>(string storedProcedureName, ParameterBuilder paramBuilder, string db, int commandTimeout = -1, bool throwError = false, bool logDebug = false);
```

## Type Parameters

`T`

Type of object to be returned

## Parameters

`storedProcedureName`  string

Name of the stored procedrue to be called

`paramBuilder`  [ParameterBuilder](../../ParameterBuilder/index.md)

ParamBuilder object with parameters to be passed to the stored procedure

`db`  string

Name of the database connection string, will check environmental variables first and then config files

`commandTimeout`  int

Optional number of seconds before stored procedure times out. Default if not provided or passed as \-1 is 30 seconds

`throwError`  bool

When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.

`logDebug`  bool

When true, will log debug messages

## Returns

Task\<T\>

Task type T

___

*Documentation generated by [MdDocs](https://github.com/ap0llo/mddocs)*