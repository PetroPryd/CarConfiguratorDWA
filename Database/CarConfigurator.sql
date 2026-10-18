create table ApplicationUser (
	IdApplicationUser int primary key identity,
	Name nvarchar(255) not null unique,
	Username nvarchar(50) not null unique,
	PwdHash nvarchar(255) not null,
	PwdSalt nvarchar(255) not null,
	FirstName nvarchar(255),
	LastName nvarchar(255),
	Email nvarchar(255) not null unique,
	Phone nvarchar(50),
	Role nvarchar(20) not null check (Role in ('Admin', 'User'))
)
go

create table ComponentType (
	IdComponentType int primary key identity,
	Name nvarchar(255) not null unique,
	Description nvarchar(max)
)
go

create table CarModel (
	IdCarModel int primary key identity,
	Name nvarchar(255) not null unique,
	Manufacturer nvarchar(255) not null,
	BasePrice decimal(18, 2) not null check (BasePrice >= 0),
	Year int not null check (Year between 1900 and 2100),
	Description nvarchar(max),
	ImageUrl nvarchar(500)
)
go

create table CarComponent (
	IdCarComponent int primary key identity,
	Name nvarchar(255) not null unique,
	Description nvarchar(max),
	Price decimal(18, 2) not null check (Price >= 0),
	ImageUrl nvarchar(500),
	StockQuantity int not null check (StockQuantity >= 0),
	IsAvailable bit not null,
	ComponentTypeId int foreign key references ComponentType(IdComponentType) not null
)
go

-- M-to-N bridge: CarModel <-> CarComponent
create table CarModelCarComponent (
	CarModelId int foreign key references CarModel(IdCarModel) not null,
	CarComponentId int foreign key references CarComponent(IdCarComponent) not null,
	primary key (CarModelId, CarComponentId)
)
go

-- self-referencing M-to-N: component compatibility rules
create table ComponentCompatibility (
	ComponentId int foreign key references CarComponent(IdCarComponent) not null,
	CompatibleWithId int foreign key references CarComponent(IdCarComponent) not null,
	IsCompatible bit not null,
	primary key (ComponentId, CompatibleWithId)
)
go

create table UserConfiguration (
	IdUserConfiguration int primary key identity,
	Name nvarchar(255) not null,
	UserId int foreign key references ApplicationUser(IdApplicationUser) not null,
	CarModelId int foreign key references CarModel(IdCarModel) not null,
	TotalPrice decimal(18, 2) not null check (TotalPrice >= 0),
	Notes nvarchar(max),
	CreatedAt datetime not null
)
go

-- bridge: UserConfiguration <-> CarComponent (selected components in a saved build)
create table UserConfigurationComponent (
	UserConfigurationId int foreign key references UserConfiguration(IdUserConfiguration) not null,
	CarComponentId int foreign key references CarComponent(IdCarComponent) not null,
	primary key (UserConfigurationId, CarComponentId)
)
go

create table Image (
	IdImage int primary key identity,
	Name nvarchar(255) not null,
	Url nvarchar(500) not null,
	CarComponentId int foreign key references CarComponent(IdCarComponent),
	CarModelId int foreign key references CarModel(IdCarModel)
)
go

create table Log (
	IdLog int primary key identity,
	Timestamp datetime not null,
	Level nvarchar(20) not null check (Level in ('Info', 'Warning', 'Error', 'Debug')),
	Message nvarchar(max) not null
)
go


-- component types
insert into ComponentType (Name, Description) values
('Engine',           'Engine and powertrain components'),
('Color',            'Exterior paint colors'),
('Wheels',           'Wheel sets and rims'),
('Interior',         'Interior trims and upholstery'),
('EquipmentPackage', 'Equipment and option packages'),
('Feature',          'Individual features and add-ons')
go

-- car models
insert into CarModel (Name, Manufacturer, BasePrice, Year, Description, ImageUrl) values
('911 Carrera',    'Porsche',      115000.00, 2026, 'Iconic sports car',                      '/uploads/cars/porsche-911-carrera.webp'),
('M3 Competition', 'BMW',           78500.00, 2026, 'Performance sport sedan',                '/uploads/cars/bmw-m3.jpg'),
('RS6 Avant',      'Audi',         121000.00, 2026, 'High-performance wagon',                 '/uploads/cars/audi-rs6.jpg'),
('AMG GT',         'Mercedes-Benz',142000.00, 2026, 'Grand touring coupe',                    '/uploads/cars/mercedes-amg-gt.jpg'),
-- JDM
('370Z',           'Nissan',        35000.00, 2020, 'Iconic Japanese sports coupe',           '/uploads/cars/nissan-370z.png'),
('GT-R R35',       'Nissan',        95000.00, 2024, 'Legendary supercar killer',              '/uploads/cars/nissan-gtr-r35.jpg'),
('Skyline R34',    'Nissan',        60000.00, 1999, 'The most iconic JDM car ever made',      '/uploads/cars/nissan-skyline-r34.jpg'),
('Supra A90',      'Toyota',        55000.00, 2024, 'Reborn legend with BMW inline-6',        '/uploads/cars/toyota-supra-a90.jpg'),
('RX-7 FD',        'Mazda',         30000.00, 1997, 'Lightweight rotary-powered sports car',  '/uploads/cars/mazda-rx7.jpg'),
('NSX Type S',     'Honda',        165000.00, 2022, 'Hybrid supercar, made in Ohio',          '/uploads/cars/honda-nsx.jpg'),
('WRX STI',        'Subaru',        40000.00, 2021, 'Rally-bred all-wheel drive legend',      '/uploads/cars/subaru-wrx-sti.jpg')
go

-- car components (primary entity)
insert into CarComponent (Name, Description, Price, StockQuantity, IsAvailable, ComponentTypeId) values
-- engines
('V8 Twin-Turbo 4.0L',     '503 HP twin-turbocharged V8 engine',          25000.00, 12, 1, 1),
('Inline-6 3.0L Turbo',    '382 HP inline-six turbo engine',              12000.00, 25, 1, 1),
('V6 Hybrid 3.0L',         '450 HP V6 hybrid powertrain',                 18500.00, 15, 1, 1),
-- colors
('Midnight Black Metallic', 'Premium metallic black paint',                1500.00, 50, 1, 2),
('Arctic White Pearl',      'Pearl white premium paint',                   1800.00, 45, 1, 2),
('Racing Red',              'Vibrant racing red paint',                    1200.00, 40, 1, 2),
('Ocean Blue Metallic',     'Deep ocean blue metallic',                    1600.00, 35, 1, 2),
-- wheels
('19" Alloy Sport Wheels',  'Lightweight 19-inch alloy wheels',            2500.00, 30, 1, 3),
('20" Forged Performance',  'Forged 20-inch performance wheels',           4500.00, 20, 1, 3),
('21" Carbon Fiber Wheels', 'Premium 21-inch carbon fiber wheels',         8000.00,  8, 1, 3),
-- interiors
('Leather Sport Black',     'Premium leather sport seats in black',        3500.00, 20, 1, 4),
('Alcantara Premium',       'Alcantara and leather premium interior',      4200.00, 15, 1, 4),
('Carbon Fiber Trim',       'Full carbon fiber interior trim package',     3800.00, 12, 1, 4),
-- equipment packages
('Premium Sound System',    'Bang & Olufsen 1500W audio system',           4500.00, 25, 1, 5),
('Driver Assistance Pack',  'Adaptive cruise, lane assist, auto-park',     3200.00, 30, 1, 5),
('Cold Weather Package',    'Heated seats, steering wheel, washer nozzles',1800.00, 40, 1, 5),
-- features
('Panoramic Sunroof',       'Full-length panoramic glass sunroof',         2200.00, 22, 1, 6),
('Head-Up Display',         'Color head-up display with navigation',       1500.00, 28, 1, 6),
('LED Matrix Headlights',   'Adaptive LED matrix headlight system',        2800.00, 18, 1, 6)
go

-- M-to-N: which components are available per car model
insert into CarModelCarComponent (CarModelId, CarComponentId) values
-- Porsche 911 Carrera
(1,1),(1,3),(1,4),(1,5),(1,6),(1,8),(1,9),(1,10),(1,11),(1,12),(1,13),(1,16),(1,17),(1,18),(1,19),
-- BMW M3 Competition
(2,1),(2,2),(2,4),(2,5),(2,6),(2,7),(2,8),(2,9),(2,11),(2,12),(2,13),(2,14),(2,15),(2,16),(2,17),(2,18),(2,19),
-- Audi RS6 Avant
(3,1),(3,4),(3,5),(3,7),(3,8),(3,9),(3,11),(3,12),(3,13),(3,14),(3,15),(3,16),(3,17),(3,18),(3,19),
-- Mercedes-AMG GT
(4,1),(4,4),(4,5),(4,6),(4,8),(4,9),(4,10),(4,11),(4,12),(4,13),(4,14),(4,16),(4,17),(4,18),(4,19),
-- Nissan 370Z
(5,3),(5,4),(5,5),(5,6),(5,7),(5,8),(5,9),(5,11),(5,12),(5,15),(5,16),(5,17),(5,18),(5,19),
-- Nissan GT-R R35
(6,1),(6,4),(6,5),(6,6),(6,7),(6,8),(6,9),(6,10),(6,11),(6,12),(6,13),(6,14),(6,15),(6,16),(6,17),(6,18),(6,19),
-- Nissan Skyline R34
(7,2),(7,4),(7,5),(7,6),(7,7),(7,8),(7,9),(7,11),(7,12),(7,15),(7,16),(7,17),(7,18),(7,19),
-- Toyota Supra A90
(8,2),(8,4),(8,5),(8,6),(8,7),(8,8),(8,9),(8,11),(8,12),(8,13),(8,14),(8,15),(8,16),(8,17),(8,18),(8,19),
-- Mazda RX-7 FD
(9,3),(9,4),(9,5),(9,6),(9,7),(9,8),(9,11),(9,12),(9,15),(9,17),(9,18),(9,19),
-- Honda NSX Type S
(10,3),(10,4),(10,5),(10,6),(10,7),(10,8),(10,9),(10,10),(10,11),(10,12),(10,13),(10,14),(10,15),(10,16),(10,17),(10,18),(10,19),
-- Subaru WRX STI
(11,2),(11,4),(11,5),(11,6),(11,7),(11,8),(11,9),(11,11),(11,12),(11,14),(11,15),(11,16),(11,17),(11,18),(11,19)
go

-- incompatibility rules (IsCompatible = 0 means the pair conflicts)
-- for example: 21" carbon fiber wheels conflict with cold weather package
insert into ComponentCompatibility (ComponentId, CompatibleWithId, IsCompatible) values
-- V6 Hybrid mutually exclusive with V8 and Inline-6
(3,1,0),(1,3,0),
(3,2,0),(2,3,0),
-- 21" Carbon Fiber Wheels conflict with Cold Weather Package
(10,16,0),(16,10,0)
go

-- admin user (password: Admin123!)
insert into ApplicationUser (Name, Username, PwdHash, PwdSalt, FirstName, LastName, Email, Phone, Role)
	values(
		'Administrator',
		'admin',
		'xPhPA3+EFqDhBQe2is/Oj3kZrIgeHHTjM1an/F7JOsM=',
		'u53l6fynm3yqhZ96/OKkoA==',
		'System',
		'Administrator',
		'admin@carconfig.com',
		'0000000000',
		'Admin')
go

-- regular user (password: User123!)
insert into ApplicationUser (Name, Username, PwdHash, PwdSalt, FirstName, LastName, Email, Phone, Role)
	values(
		'John Doe',
		'user1',
		'FCvITiC0MDgcpceqK9+ebrLzwoNTSqnLRMle+ZDb6DM=',
		'usAKGgVwOGVhsEeguVUKPw==',
		'John',
		'Doe',
		'user1@carconfig.com',
		'0101010101',
		'User')
go

-- sample saved configuration for the demo
insert into UserConfiguration (Name, UserId, CarModelId, TotalPrice, Notes, CreatedAt) values
	('My Dream BMW M3', 2, 2, 92000.00, 'Configured with premium options', getdate())
go

-- Inline-6(2), Black paint(4), 19" wheels(8), Leather interior(11), Sound system(14)
insert into UserConfigurationComponent (UserConfigurationId, CarComponentId) values
	(1,2),(1,4),(1,8),(1,11),(1,14)
go

-- initial log entries
insert into Log (Timestamp, Level, Message) values
	(getdate(), 'Info', 'Database initialized successfully.'),
	(getdate(), 'Info', 'Seed data loaded.')
go