use erpsystem

CREATE TABLE Inventory (
    ProdId         INT             NOT NULL,
    ProdCode       AS              ('PRC'+right('00000'+CONVERT(varchar(5),ProdId),(5))) PERSISTED,
	InventoryID    INT IDENTITY(1,1),
    INVT_QOH       INT             NULL DEFAULT(0),
    ProdUnit       VARCHAR (50)    NOT NULL,
    ProdPriceUnit  DECIMAL (10, 2) NULL DEFAULT(0),
    ProdTotalPrice DECIMAL (10, 2) NULL DEFAULT(0),
    INVT_INDATEAT  DATETIME        DEFAULT (GETUTCDATE()) NULL,
    INVT_UPDATEAT  DATETIME        DEFAULT (GETUTCDATE()) NULL,
    INVT_ISACTIVE  INT             DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED (ProdId ASC),
    FOREIGN KEY (ProdId) REFERENCES Product (ProdId)
);


CREATE TABLE Product (
    ProdId          INT          IDENTITY (1, 1) NOT NULL,
    ProdName        VARCHAR (50) NOT NULL,
    ProdDescription VARCHAR (50) NULL,
    ProdCreatedAt   DATETIME     DEFAULT(GETUTCDATE()) NULL,
    ProdUpdatedAt   DATETIME     DEFAULT(GETUTCDATE()) NULL,
    ProdIsActive    INT          DEFAULT(1) NULL,
    ProdcategoryId  INT          NOT NULL,
    SupplierId      INT          NULL,
    PRIMARY KEY CLUSTERED (ProdId ASC),
    FOREIGN KEY (ProdcategoryId) REFERENCES ProductCategory (ProdcategoryId),
    FOREIGN KEY (SupplierId) REFERENCES Supplier (SuppId)
);

CREATE TABLE ProductCategory (
    ProdcategoryId   INT          IDENTITY (1, 1) NOT NULL,
    ProdcategoryName VARCHAR (50) NOT NULL,
    IsActive         INT          DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED (ProdcategoryId ASC)
);

CREATE TABLE RequisitionItem (
    ReqId       INT          NOT NULL,
    ProdId      INT          NOT NULL,
    ReqQuantity INT          NULL DEFAULT(0),
    ReqUnit     VARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED (ReqId ASC, ProdId ASC),
    FOREIGN KEY (ReqId) REFERENCES RequisitionForm (ReqId)
);

CREATE TABLE RequisitionForm (
    ReqId        INT          IDENTITY (1, 1) NOT NULL,
    ReqTotalItem INT          NULL DEFAULT(0),
    ReqStatus    VARCHAR (20) DEFAULT ('Pending') NULL,
    ReqCreatedAt DATETIME         DEFAULT (GETDATE()) NULL,
    ReqUpdated   DATETIME         DEFAULT (GETDATE()) NULL,
    ReqIsActive  INT          DEFAULT ((1)) NULL,
    AccId        INT          NULL,
    PRIMARY KEY CLUSTERED (ReqId ASC),
    FOREIGN KEY (AccId) REFERENCES Account (AccId)
);



--------------------------------------------------------------------------
create table Account(
AccId int identity,
AccUserName varchar(50),
AccPassword varchar(50),
AccFname varchar(50),
AccLname varchar(50),
AccMname varchar(50),
AccType varchar(20),
AccCreatedAt date default current_timestamp,
AccUpdatedAt date,
AccIsActive int,
Primary Key(AccId)
)


create table ProductCategory(
ProdcategoryId int identity,
ProdcategoryName varchar(50),
IsActive int default(1)
Primary key(ProdcategoryId)
)


create table Supplier(
SuppId int identity,
SuppName varchar(50),
SuppPhone varchar(50),
SuppCity varchar(50),
SuppMunicipality varchar(50),
SuppBarangay varchar(50),
SuppZipcode varchar(50),
SuppCreatedAt date,
SuppUpdatedAt date,
SuppIsActive int,
Primary Key(SuppId )
)


create table Product(
ProdId int identity(1,1),
ProdName varchar(50),
ProdDescription varchar(50),
ProdCreatedAt date,
ProdUpdatedAt date,
ProdIsActive int,
ProdcategoryId int,
SupplierId int,
Primary Key(ProdId),
Foreign Key(ProdcategoryId) References  ProductCategory(ProdcategoryId),
Foreign Key(SupplierId) References Supplier(SuppId)
)



create table Inventory(
ProdId int,
ProdCode as 
INVT_QOH int,
ProdUnit varchar(50),
ProdPriceUnit decimal(10,2),
ProdTotalPrice decimal(10,2),
INVT_INDATEAT DATETIME default(GETDATE()),
INVT_UPDATEAT DATETIME,
INVT_ISACTIVE
Primary Key(ProdId),
Foreign Key(ProdId) References Product(Prodid)
)



create table RequisitionForm(
ReqId int identity,
ReqTotalItem int,
ReqStatus varchar(20) default 'Pending',
ReqCreatedAt date default current_timestamp,
ReqUpdated date,
ReqIsActive int default 1,
AccId int,
Primary Key(ReqId),
Foreign Key(AccId) References Account(AccId)
)


create table RequisitionItem(
ReqId int,
ProdId int,
ReqQuantity int,
ReqUnit varchar(20),
Primary Key(ReqId,ProdId),
Foreign Key(ReqId) References  RequisitionForm(ReqId),
Foreign Key(ProdId) References Product(ProdId)
)


create table CanvassForm(
CanvassId int identity,
CanvassTotalItem int,
CanvassStatus varchar(20) default 'Pending',
CanvassCreatedAt date default current_timestamp,
CanvassUpdated date,
CanvassIsActive int default 1,
ReqId int,
Primary Key(CanvassId),
Foreign Key(ReqId) References RequisitionForm(ReqId)
)



create table CanvassItem(
CanvassId int,
ProdId int,
Quantity int,
Unit varchar(20),
Primary Key(CanvassId,ProdId),
Foreign Key(CanvassId) References CanvassForm(CanvassId),
Foreign Key(ProdId) References Product(ProdId)
)


create table QoutationForm(
QouteId int identity,
SuppName varchar(50),
SuppPhone varchar(50),
SuppCity varchar(50),
SuppMunicipality varchar(50),
SuppBarangay varchar(50),
SuppZipcode varchar(50),
QouteStatus varchar(20) default 'Pending',
QouteCreatedAt date default current_timestamp,
QouteUpdatedAt date,
QouteSubtotal Decimal(10,2),
QouteDiscount Decimal(10,2),
QouteTotal Decimal(10,2),
QouteIsDelete int default 0,
CanvassId int,
Primary Key(QouteId),
Foreign Key(CanvassId) References CanvassForm(CanvassId),
)


create table QoutationItem(
QouteId int,
ProdId int,
QouteQuantity int,
QouteUnit varchar(20),
QoutePricePerUnit Decimal(10,2),
Primary Key(QouteId,ProdId),
Foreign Key(QouteId) References QoutationForm(QouteId),
Foreign Key(ProdId) References Product(ProdId)
)



create table PurchaseOrderForm(
PurId int identity,
ReqId int,
SuppId int,
QouteId int, 
PurStatus varchar(50) default 'Pending',
PurCreateAt date,
PurUpdatedAt date,
PurIsDelete int default 0,
Primary Key(PurId),
Foreign Key(ReqId) References RequisitionForm(ReqId),
Foreign Key(SuppId) References Supplier(SuppId),
Foreign Key(QouteId) References QoutationForm(QouteId),
)


create table PurchaseOrderItem(
PurId int,
ProdId int,
ProdQuantityReceived int,
Foreign Key(PurId) References PurchaseOrderForm(PurId),
Foreign Key(ProdId) References Product(ProdId)
)




create table example2
(
prodid int,
pname varchar(20),
primary key(prodid)
)



create table example1
(
id int identity primary key,
fname varchar(20)
)

create table example3
(
id int,
prodid int,
primary key(id,prodid),
foreign key(id) references example1(id),
foreign key(prodid) references example2(prodid)
)

create table example4
(
id int,
prodid int,
primary key(id,prodid),
foreign key(id,prodid) references example3(id,prodid)
)

drop table example1


select * from example1


select * from example2

select * from example3

SET IDENTITY_INSERT example1 ON;

insert into example2
	select id,fname
	from example1;


insert into example3 values(1,1)
insert into example3 values(1,2)
insert into example3 values(1,3)

insert into example2(fname) values('c')
insert into example1(fname) values('d')