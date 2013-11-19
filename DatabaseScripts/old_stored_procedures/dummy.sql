insert into Customer values('zelinliu@me.com', 'password0', 'Sullivan', 'Zelin', 'Liu', '1234 56th AVE, Seattle, WA, 78901', 'zelinliu@me.com', 'grocery', 1);
insert into Customer values('scottd@scanmylist.com', '1234', 'Scott', '', 'Dyer', '6543 21th Ave, Seattle, WA, 98765', 'scottd@scanmylist.com', 'Consulting', 0);

insert into Customer values('turbo', '1234', 'Chris', '', 'Hannemann', '6543 21th Ave, Seattle, WA, 98765', 'turboenergychris@gmail.com', 'Drinks', 0);

insert into Supplier values('supplier1', 'grocery', 'address1', 1);
insert into Supplier values('supplier2', 'grocery', 'address2', 1);
insert into Supplier values('supplier3', 'general', 'address3', 1);
insert into Supplier values('supplier4', 'general', 'address4', 1);
insert into Supplier values('supplier5', 'general', 'address5', 1);
insert into Supplier values('supplier6', 'general', 'address6', 1);
insert into Supplier values('supplier7', 'general', 'address7', 1);

insert into Product values('0', 'p0', 'd0', null);
insert into Product values('1', 'p1', 'd1', null);
insert into Product values('2', 'p2', 'd2', null);
insert into Product values('3', 'p3', 'd3', null);
insert into Product values('4', 'p4', 'd4', null);
insert into Product values('5', 'p5', 'd5', null);
insert into Product values('11111111', 'Sorghum Cereal', 'Nutrition Information', null);
insert into Product values('22222222', 'Beans & Pulses', 'Nutrition Information', null);
insert into Product values('33333333', 'Corn Soya Blend', 'Nutrition Information', null);
insert into Product values('44444444', 'Cooking Oil', 'Nutrition Information', null);
insert into Product values('55555555', 'Salt', 'Nutrition Information', null);
insert into Product values('66666666', 'Sugar', 'Nutrition Information', null);
insert into Product values('77777777', 'Clean Water', 'Nutrition Information', null);
insert into Product values('88888888', 'Plumpy Nut', 'Nutrition Information', null);

insert into Product values('TBED_MIETSB11', '1 Gallon Turbo Energy Drink BIB', '1 Gallon Turbo Energy Drink BIB', null);
insert into Inventory values('TBED_MIETSB11', 1, 0, 0, '');
insert into Product values('TBED_MIETSB31', '3 Gallon Turbo Energy Drink BIB', '3 Gallon Turbo Energy Drink BIB', null);
insert into Inventory values('TBED_MIETSB31', 1, 0, 0, '');
insert into Product values('TBED_MIETLB11', '1 Gallon Diet Turbo Energy Drink BIB', '1 Gallon Diet Turbo Energy Drink BIB', null);
insert into Inventory values('TBED_MIETLB11', 1, 0, 0, '');
insert into Product values('TBED_MIETHB31', '3 Gallon Dragon Energy Drink BIB', '3 Gallon Dragon Energy Drink BIB', null);
insert into Inventory values('TBED_MIETHB31', 1, 0, 0, '');
insert into Product values('TBED_99TRRTDT', '24 pk Turbo Energy Drink BTLS', '24 pk Turbo Energy Drink BTLS', null);
insert into Inventory values('TBED_99TRRTDT', 1, 0, 0, '');
insert into Product values('TBED_99TDRTDT', '24 pk Diet Turbo Energy Drink BTLS', '24 pk Diet Turbo Energy Drink BTLS', null);
insert into Inventory values('TBED_99TDRTDT', 1, 0, 0, '');
insert into Product values('TBED_MIESTB31', '3 Gallon Energy Shot Energy Drink', '3 Gallon Energy Shot Energy Drink', null);
insert into Inventory values('TBED_MIESTB31', 1, 0, 0, '');
insert into Product values('TBED_MIEOTB31', '3 Gallon Orange Turbo Energy Drink', '3 Gallon Orange Turbo Energy Drink', null);
insert into Inventory values('TBED_MIEOTB31', 1, 0, 0, '');
insert into Product values('TBED_MIETBB8T', '2 Gallon Turbo Blue', '2 Gallon Turbo Blue', null);
insert into Inventory values('TBED_MIETBB8T', 1, 0, 0, '');
insert into Product values('TBED_MIEBDB8T', '2 Gallon Diet Turbo Blue', '2 Gallon Diet Turbo Blue', null);
insert into Inventory values('TBED_MIEBDB8T', 1, 0, 0, '');
insert into Product values('TBED_MIETHB31', '3 Gallon Dragon Energy Drink BIB', '3 Gallon Dragon Energy Drink BIB', null);
insert into Inventory values('TBED_MIETHB31', 1, 0, 0, '');
insert into Product values('TBED_MIEOTB31', '3 Gallon Orange Turbo Energy Drink', '3 Gallon Orange Turbo  Energy Drink', null);
insert into Inventory values('TBED_MIEOTB31', 1, 0, 0, '');

insert into supplies values(4, 'TBED_MIETSB11', 1);
insert into supplies values(4, 'TBED_MIETSB31', 2);
insert into supplies values(4, 'TBED_MIETLB11', 3);
insert into supplies values(4, 'TBED_MIETHB31', 4);
insert into supplies values(4, 'TBED_99TRRTDT', 5);
insert into supplies values(4, 'TBED_99TDRTDT', 6);
insert into supplies values(4, 'TBED_MIESTB31', 7);
insert into supplies values(4, 'TBED_MIEOTB31', 8);
insert into supplies values(4, 'TBED_MIETBB8T', 9);
insert into supplies values(4, 'TBED_MIEBDB8T', 10);

insert into supplies values(3, 'TBED_MIETSB11', 1);
insert into supplies values(3, 'TBED_MIETSB31', 2);
insert into supplies values(3, 'TBED_MIETLB11', 3);
insert into supplies values(3, 'TBED_MIETHB31', 4);
insert into supplies values(3, 'TBED_99TRRTDT', 5);
insert into supplies values(3, 'TBED_99TDRTDT', 6);
insert into supplies values(3, 'TBED_MIESTB31', 7);
insert into supplies values(3, 'TBED_MIEOTB31', 8);
insert into supplies values(3, 'TBED_MIETBB8T', 9);
insert into supplies values(3, 'TBED_MIEBDB8T', 10);

insert into supplies values(2, 'TBED_MIETSB11', 1);
insert into supplies values(2, 'TBED_MIETSB31', 2);
insert into supplies values(2, 'TBED_MIETLB11', 3);
insert into supplies values(2, 'TBED_MIETHB31', 4);
insert into supplies values(2, 'TBED_99TRRTDT', 5);
insert into supplies values(2, 'TBED_99TDRTDT', 6);
insert into supplies values(2, 'TBED_MIESTB31', 7);
insert into supplies values(2, 'TBED_MIEOTB31', 8);
insert into supplies values(2, 'TBED_MIETBB8T', 9);
insert into supplies values(2, 'TBED_MIEBDB8T', 10);

insert into supplies values(1, 'TBED_MIETSB11', 1);
insert into supplies values(1, 'TBED_MIETSB31', 2);
insert into supplies values(1, 'TBED_MIETLB11', 3);
insert into supplies values(1, 'TBED_MIETHB31', 4);
insert into supplies values(1, 'TBED_99TRRTDT', 5);
insert into supplies values(1, 'TBED_99TDRTDT', 6);
insert into supplies values(1, 'TBED_MIESTB31', 7);
insert into supplies values(1, 'TBED_MIEOTB31', 8);
insert into supplies values(1, 'TBED_MIETBB8T', 9);
insert into supplies values(1, 'TBED_MIEBDB8T', 10);

insert into OrderList values('Order 1', 1, 0, 0, CONVERT(BIGINT, LEFT(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(30),CURRENT_TIMESTAMP,126),'-',''),':',''),'T',''),14)));
insert into OrderList values('Order 2', 1, 0, 1, CONVERT(BIGINT, LEFT(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(30),CURRENT_TIMESTAMP,126),'-',''),':',''),'T',''),14)));
insert into OrderList values('Order 3', 1, 1, 0, CONVERT(BIGINT, LEFT(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(30),CURRENT_TIMESTAMP,126),'-',''),':',''),'T',''),14)));
insert into OrderList values('Order 4', 1, 1, 1, CONVERT(BIGINT, LEFT(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(30),CURRENT_TIMESTAMP,126),'-',''),':',''),'T',''),14)));
insert into OrderList values('Order 5', 1, 0, 0, CONVERT(BIGINT, LEFT(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(30),CURRENT_TIMESTAMP,126),'-',''),':',''),'T',''),14)));

insert into Contain values(1,'TBED_MIETSB11',1,5);
insert into Contain values(1,'TBED_99TDRTDT',2,10);
insert into Contain values(1,'TBED_MIEBDB8T',3,25);
insert into Contain values(1,'TBED_MIETSB31',1,30);
insert into Contain values(1,'TBED_MIETLB11',2,45);

insert into Contain values(2,'TBED_MIETSB11',3,10);
insert into Contain values(2,'TBED_99TDRTDT',1,10);
insert into Contain values(2,'TBED_MIEBDB8T',2,10);
insert into Contain values(2,'TBED_MIETSB31',3,10);
insert into Contain values(2,'TBED_MIETLB11',1,10);
insert into Contain values(2,'TBED_99TRRTDT',2,10);