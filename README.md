Here are the scripts for the database (MSSQL):

1. CREATE DATABASE WeatherBotDB;

2. CREATE TABLE Users(
	UserId BIGINT PRIMARY KEY,
	FirstName NVARCHAR(100) NULL,
	LastName NVARCHAR(100) NULL);

3. CREATE TABLE WeatherHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,       
    UserId BIGINT NOT NULL,                   
    City NVARCHAR(100) NULL,        
    Temperature DECIMAL(5,2) NULL,      
    Humidity DECIMAL(5,2) NULL,         
    WindSpeed DECIMAL(5,2) NULL,        
    Condition NVARCHAR(100) NULL,       
    
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
