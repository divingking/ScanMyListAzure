create table Customer(
	id int primary key identity, 
	login varchar(50), 
	password varchar(50), 
	fname varchar(30), 
	mname varchar(30), 
	lname varchar(30),
	addr varchar(255), 
	email varchar(100), 
	business varchar(20), 
	logged_on bit, 
	unique(login)
);
create table Supplier(
	id int primary key identity, 
	name varchar(50), 
	business varchar(20), 
	addr varchar(255), 
	cid int, 
	foreign key (cid) references Customer(id)
);
create table OrderList(
	id int primary key identity, 
	title varchar(100), 
	cid int, 
	sent bit, 
	scan_in bit, 
	date bigint, 
	foreign key (cid) references Customer(id)
);
create table Product(
	upc varchar(13) primary key, 
	name varchar(100), 
	detail varchar(255), 
	thumb varchar(512)
);

create table Inventory(
	upc varchar(13), 
	cid int, 
	lead_time int, 
	quantity int, 
	location varchar(20), 
	foreign key (cid) references Customer(id)
);
create table Contain(
	oid int, 
	upc varchar(13), 
	supplier int,
	quantity int, 
	foreign key (oid) references OrderList(id), 
	foreign key (upc) references Product(upc), 
	foreign key (supplier) references Supplier(id)
);
create table Supplies(
	supplier int, 
	upc varchar(13), 
	price float, 
	foreign key (supplier) references Supplier(id), 
	foreign key (upc) references Product(upc)
);

create index BusinessType on Customer(business);
create clustered index OrderContains on Contain(oid);
create index SuppliesSuppliers on Supplies(supplier);
create clustered index SuppliesProducts on Supplies(upc);
create clustered index InventoryCid on Inventory(cid);