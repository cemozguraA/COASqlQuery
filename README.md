## COASqlQuery


#### Description
- Convert your class and predicate into SqlQuery. Work with dapper.

### NugetPackage
[https://www.nuget.org/packages/COASqlQuery]
(https://www.nuget.org/packages/COASqlQuery)

#### Usage

- **Step1**

Add the following NuGet package to your solution.

- **Step2**

You must call class like this.
 ```csharp
COASqlQuery.COASqlQuery<TData>() strQuery = new COASqlQuery.COASqlQuery<TData>();
//If the ClassName is different from the TableName in your DataBase. You can set your tablename like this.
COASqlQuery.COASqlQuery<TData>() strQuery = new COASqlQuery.COASqlQuery<TData>("TestTable");
```
#### Propertys
| Property  | What it does |
| ------------- | ------------|
| PrimaryKeyName  | Set your primarykey to not any changes.|
| Oracle  | If your datatable is oracle set this property true|

#### Methods
###### GenerateInsertQuery  
- Convert your class to insert sqlquery
