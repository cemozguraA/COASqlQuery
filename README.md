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

# Methods

- **TestClass For Example**
```csharp
COASqlQuery<Ogrenci> Data = new COASqlQuery<Ogrenci>();
Data.PrimaryKeyName = "ID";
var ogrenci = new Ogrenci()
            {
                ID = 6,
                Ad = "Cem",
                SoyAd = "Aydın",
                Okul = "Yunus emre",
                Sınıf = "8/c",
                BasTarihi = DateTime.Now.AddDays(-1000)
            };
```
 
## GenerateInsertQuery  
- Convert your class to insert sqlquery 
 ```csharp
var InsertString = Data.GenerateInsertQuery();
//Return : INSERT INTO Ogrenci ([Ad],[SoyAd],[Sınıf],[BasTarihi],[Okul]) VALUES (@Ad,@SoyAd,@Sınıf,@BasTarihi,@Okul)
```

## GenerateUpdateQuery  
- Convert your class to update sqlquery 
 ```csharp
var updateString = Data.GenerateUpdateQuery(a => a.BasTarihi == ogrenci.BasTarihi && a.Ad == ogrenci.Ad);
//Return : UPDATE Ogrenci SET Ad=@Ad,SoyAd=@SoyAd,Sınıf=@Sınıf,BasTarihi=@BasTarihi,Okul=@Okul WHERE BasTarihi = '2017-06-09 09:37:54' AND Ad = 'Cem'
```
