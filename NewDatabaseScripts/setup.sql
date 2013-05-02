

create table Business (
    id int primary key identity, 
    name varchar(100), 
    address varchar(255), 
    zip int, 
    email varchar(255), 
    category varchar(50)
); 
create table Product (
    upc varchar(20) primary key, 
    name varchar(100), 
    detail varchar(255)
); 

create table Account (
    login varchar(255), 
    password varchar(512), 
    business int, 
	UUID varchar(50), 
	session_id varchar(64), 
	tier int, 
    foreign key (business) references Business(id)
); 
create table Inventory (
    business int, 
    upc varchar(20), 
    location varchar(20), 
    quantity int, 
    lead_time int, 
    foreign key (business) references Business(id), 
    foreign key (upc) references Product(upc)
); 
create table Supplies (
    business int, 
	customer int, 
    upc varchar(20), 
    price float, 
    foreign key (business) references Business(id), 
	foreign key (customer) references Business(id), 
    foreign key (upc) references Product(upc)
); 
create table Category (
	id int primary key, 
	name varchar(10)
); 
create table RecordStatus (
	id int primary key, 
	status varchar(6)
);
create table Record (
    id int primary key identity, 
    title varchar(50), 
    date bigint, 
    category int, 
	status int, 
    business int, 
    foreign key (business) references Business(id), 
	foreign key (category) references Category(id), 
	foreign key (status) references RecordStatus(id)
); 

create table Consists (
    rid int, 
    upc varchar(20), 
    supplier int, 
    customer int, 
    quantity int, 
    foreign key (rid) references Record(id), 
    foreign key (upc) references Product(upc), 
    foreign key (supplier) references Business(id), 
    foreign key (customer) references Business(id)
); 

create index BusinessCategory on Business(category);
create clustered index OrderContains on Consists(rid);
create index SuppliesSuppliers on Supplies(business);
create index SuppliesCustomers on Supplies(customer);
create clustered index SuppliesProducts on Supplies(upc);
create clustered index InventoryCid on Inventory(business);

insert into Category values (0, 'Order');
insert into Category values (1, 'Receipt');
insert into Category values (2, 'Change');

insert into RecordStatus values (0, 'saved');
insert into RecordStatus values (1, 'sent');
insert into RecordStatus values (2, 'closed');

insert into Business values ('dummy', NULL, 0, NULL, NULL);