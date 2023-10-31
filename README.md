# Simple Book
## Task based round for the position of backend (Junior) ar NITEX

***

Simple Book is a web service that allows users to register, login, and perform CRUD operations on books. The service uses JWT-based authentication, role-based access control, and simple password reset mechanisms.


#### Features
+ User registration and login
+ User profile management
+ User password change and reset
+ Book creation, retrieval, update, and deletion
+ Admin role for deleting any book


#### Technologies
+ ASP.NET Core 7
+ Entity Framework Core 7
+ SQLite 3
+ JWT 2.0

<br>

#### Installation
To run the application, you need to have the following installed on your machine:
+ Dotnet SDK Version 7
```
git clone https://github.com/nayef-shuvo/SimpleBook.git
cd SimpleBook
dotnet build
dotnet run
```
The application will be available at http://localhost:5050

<br/>

#### Database Tables
**Table: Books**
```
Id               [string, auto-genrated unique identifier]
Username         [string, unique]
FullName         [string]
Email            [string, valid email format]
Password         [string, hashed value]
DateRegistered   [DateTime, date of registration]

```
**Table: Books**
>The description did not specify the fields for the Books table, so I added them based on my own assumptions.

```
Id               [string, auto-genrated unique identifier]
Isbn             [string, 13-digit International Standard Book Number]
Title            [string]
Author           [string]
Edition          [int]
Price            [decimal, range (0, 100,000)]
```
**Table: UserRoles**
```
Id               [string, foreign key referencing Users.Id]
Role             [Enum, two possible values: "Admin" or "User"]
```

<br>

**The API documentation is available at `http://localhost:5050/swagger.html`**
#### API Endpoints
<img src="./Images/Screenshot 2023-10-31 at 23-08-23 Swagger UI.png">

#### A Sample Request and Response
<img src="./Images/Screenshot 2023-10-31 at 23-30-52 Swagger UI.png">

#### A Reponse for Unauthorized Request
<img src="./Images/Screenshot 2023-10-31 at 23-34-13 Swagger UI.png">