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
## GenerateDeleteQuery  
- Convert your class to delete sqlquery 
 ```csharp
var DeleteString = Data.GenerateDeleteQuery(a => a.BasTarihi == ogrenci.BasTarihi);
//Return : DELETE FROM Ogrenci WHERE BasTarihi = '2017-06-09 09:46:28'
```

## GenerateSelectQuery  
- Convert your class to select sqlquery 
 ```csharp
 var SelectString = Data.GenerateSelectQuery();
//Return : SELECT * FROM Ogrenci


 var SelectString = Data.GenerateSelectQuery(a => new { a.Ad, a.SoyAd, a.Sınıf, a.BasTarihi });
 //Return : SELECT Ad,SoyAd,Sınıf,BasTarihi FROM Ogrenci
```
# Extensions

## SelecExtensions
- **Where**
 ```csharp
var SelectString = Data.GenerateSelectQuery().Where(a=> a.BasTarihi>=ogrenci.BasTarihi || a.SoyAd==ogrenci.SoyAd).OrderBy(a=> a.SoyAd);
//Return : SELECT * FROM Ogrenci WHERE BasTarihi >= '2017-06-09 11:47:12' OR SoyAd LIKE '%Cem%' OR SoyAd = 'Aydın'
```
- **OrderBy**
 ```csharp
var SelectString = Data.GenerateSelectQuery().Where(a=> a.BasTarihi>=ogrenci.BasTarihi || a.SoyAd==ogrenci.SoyAd).OrderBy(a=> a.SoyAd);
//Return : SELECT * FROM Ogrenci WHERE BasTarihi >= '2017-06-09 10:36:51' OR SoyAd = 'Aydın' ORDER BY SoyAd ASC
```
- **OrderByDescing**
 ```csharp
var SelectString = Data.GenerateSelectQuery().Where(a=> a.BasTarihi>=ogrenci.BasTarihi || a.SoyAd==ogrenci.SoyAd).OrderByDescing(a=> a. Ad);
//Return : SELECT * FROM Ogrenci WHERE BasTarihi >= '2017-06-09 10:36:51' OR SoyAd = 'Aydın' ORDER BY Ad DESC
```
- **Skip**
 ```csharp
var SelectString = Data.GenerateSelectQuery().Where(a=> a.BasTarihi>=ogrenci.BasTarihi || a.SoyAd==ogrenci.SoyAd).OrderBy(a=> a.SoyAd).Skip(10,20);
//Return : with dummyTable as (select ROW_NUMBER() over( ORDER BY SoyAd ASC) as RowNumber,* from Ogrenci WHERE BasTarihi >= '2017-06-09 10:39:53' OR SoyAd = 'Aydın') select top(10) ID,Ad,SoyAd,Sınıf,BasTarihi,Okul from dummyTable WHERE RowNumber > (20)
```
## UpdateExtensions

- **SelectColums**
 ```csharp
 var updateString = Data.GenerateUpdateQuery(a => a.BasTarihi == ogrenci.BasTarihi).SelectColums(a => a.SoyAd);
//Return : UPDATE Ogrenci SET SoyAd=@SoyAd WHERE BasTarihi = '2017-06-09 10:39:53'
```

- **RemoveColums**
 ```csharp
 var updateString =  Data.GenerateUpdateQuery(a => a.BasTarihi == ogrenci.BasTarihi).RemoveColums(a => a.SoyAd);
//Return : UPDATE Ogrenci SET Ad=@Ad,Sınıf=@Sınıf,BasTarihi=@BasTarihi,Okul=@Okul WHERE BasTarihi = '2017-06-09 10:39:53'
```
