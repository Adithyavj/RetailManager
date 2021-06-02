# Retail Manager
A Retail Management System

## BackEnd
### This is where the Data is Stored.
The Back End Project consists of an SQL Server Data Tools (SSDT) / SQL Server DataBase Project. The Application has 2 DataBases.
- A User DataBase
- A Login DataBase

## MiddleWare
### The DataAccess and BusinessLogic are handled here
The Middle Ware Project consists of an ASP.NET MVC WEBAPI Project(.NET Core 3.1) which was previously .NET Framework 4.7.2 and
A Class Library Project(.NET Standard 2.0) for handling API Logic and DB Access.
The project was designed like this so that the UI can change without changing much of the BusinessLogic.

## FrontEnd
### WPF Project
The Front End of this Application is a WPF Project (.NET Core 3.1) previously .NET Framework 4.7.2.
It has a Class Library Project(.NET Standard 2.0) for Consuming API.
