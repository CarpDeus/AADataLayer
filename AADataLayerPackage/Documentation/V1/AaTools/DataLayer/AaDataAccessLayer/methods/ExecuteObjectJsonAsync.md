﻿<!--  
  <auto-generated>   
    The contents of this file were generated by a tool.  
    Changes to this file may be list if the file is regenerated  
  </auto-generated>   
-->

# AaDataAccessLayer.ExecuteObjectJsonAsync Method

**Declaring Type:** [AaDataAccessLayer](../index.md)  
**Namespace:** [AaTools.DataLayer](../../index.md)  
**Assembly:** AaDataLayer  
**Assembly Version:** 1.0.0+5890e7dfdc873f8403b1635b629b41e8973b63a9

Asynchronouslyl Return Json Object when stored procedure uses output parameter

```csharp
[AsyncStateMachine(AaTools.DataLayer.AaDataAccessLayer/<ExecuteObjectJsonAsync>d__8`1)]
[DebuggerStepThrough]
public static Task<T> ExecuteObjectJsonAsync<T>(string storedProcedureName, ParameterBuilder paramBuilder, string OutputParamName, string db, bool logDebug = false);
```

## Type Parameters

`T`

Type of object to be returned

## Parameters

`storedProcedureName`  string

`paramBuilder`  [ParameterBuilder](../../ParameterBuilder/index.md)

`OutputParamName`  string

`db`  string

`logDebug`  bool

When true, will log debug messages

## Returns

Task\<T\>

___

*Documentation generated by [MdDocs](https://github.com/ap0llo/mddocs)*