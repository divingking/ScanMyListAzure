

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
    password varchar(20), 
    business int, 
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
create table InventoryHistory (
    business int, 
    date bigint, 
    upc varchar(20), 
    location varchar(20), 
    quantity int, 
    lead_time int, 
    foreign key (business) references Business(id), 
    foreign key (upc) references Product(upc)
); 
create table Supplies (
    business int, 
    upc varchar(20), 
    price float, 
    foreign key (business) references Business(id), 
    foreign key (upc) references Product(upc)
); 
create table Customer (
    business int, 
    customer int, 
    upc varchar(20), 
    price float, 
    foreign key (business) references Business(id), 
    foreign key (customer) references Business(id), 
    foreign key (upc) references Product(upc)
); 
create table OrderList (
    id int primary key identity, 
    title varchar(50), 
    date bigint, 
    sent bit, 
    scan_in bit, 
    business int, 
    foreign key (business) references Business(id)
); 

create table Consists (
    oid int, 
    upc varchar(20), 
    supplier int, 
    customer int, 
    quantity int, 
    foreign key (oid) references OrderList(id), 
    foreign key (upc) references Product(upc), 
    foreign key (supplier) references Business(id), 
    foreign key (customer) references Business(id)
); 

create index BusinessCategory on Business(category);
create clustered index OrderContains on Consists(oid);
create index SuppliesSuppliers on Supplies(business);
create clustered index SuppliesProducts on Supplies(upc);
create clustered index InventoryCid on Inventory(business);