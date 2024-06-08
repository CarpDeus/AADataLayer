﻿<!--  
  <auto-generated>   
    The contents of this file were generated by a tool.  
    Changes to this file may be list if the file is regenerated  
  </auto-generated>   
-->

# ParameterBuilder.GetValue Method

**Declaring Type:** [ParameterBuilder](../index.md)  
**Namespace:** [AaTools.DataLayer](../../index.md)  
**Assembly:** AaDataLayer  
**Assembly Version:** 1.0.0+5890e7dfdc873f8403b1635b629b41e8973b63a9

## Overloads

| Signature                                 | Description                                                     |
| ----------------------------------------- | --------------------------------------------------------------- |
| [GetValue(string)](#getvaluestring)       | Retrieves parameterValue data from parameters by name           |
| [GetValue\<T\>(string)](#getvaluetstring) | Retrieves parameterValue from parametes by name, strongly typed |

## GetValue(string)

Retrieves parameterValue data from parameters by name 

```csharp
public object GetValue(string parameterName);
```

### Parameters

`parameterName`  string

Name of parameter

### Returns

object

Value of parameter

## GetValue\<T\>(string)

Retrieves parameterValue from parametes by name, strongly typed

```csharp
public T GetValue<T>(string parameterName);
```

### Type Parameters

`T`

### Parameters

`parameterName`  string

### Returns

T

___

*Documentation generated by [MdDocs](https://github.com/ap0llo/mddocs)*