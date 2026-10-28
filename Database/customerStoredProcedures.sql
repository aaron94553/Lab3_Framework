-- Customer stored procedures (modeled on the state stored procedures)
-- usp_CustomerCreate already exists in stateStoredProcedures.sql, so every procedure is dropped first.

DROP PROCEDURE IF EXISTS usp_CustomerSelect;
DROP PROCEDURE IF EXISTS usp_CustomerSelectAll;
DROP PROCEDURE IF EXISTS usp_CustomerCreate;
DROP PROCEDURE IF EXISTS usp_CustomerUpdate;
DROP PROCEDURE IF EXISTS usp_CustomerDelete;

-- 1. Select one customer
DELIMITER //
CREATE PROCEDURE usp_CustomerSelect (in custId int)
BEGIN
	Select * from customers where customerid = custId;
END //
DELIMITER ;

-- 2. Select all customers
DELIMITER //
CREATE PROCEDURE usp_CustomerSelectAll ()
BEGIN
	Select * from customers order by name;
END //
DELIMITER ;

-- 3. Create a customer (returns the new id in custId)
DELIMITER //
CREATE PROCEDURE usp_CustomerCreate (out custId int, in name_p varchar(100), in address_p varchar(50), in city_p varchar(20), in state_p varchar(2), in zipcode_p varchar(15))
BEGIN
	Insert into customers (name, address, city, state, zipcode, concurrencyid)
    Values (name_p, address_p, city_p, state_p, zipcode_p, 1);
    Select LAST_INSERT_ID() into custId;
END //
DELIMITER ;

-- 4. Update a customer (only if nobody else changed it first)
DELIMITER //
CREATE PROCEDURE usp_CustomerUpdate (in custId int, in name_p varchar(100), in address_p varchar(50), in city_p varchar(20), in state_p varchar(2), in zipcode_p varchar(15), in conCurrId int)
BEGIN
	Update customers
    Set name = name_p, address = address_p, city = city_p, state = state_p, zipcode = zipcode_p,
        concurrencyid = (concurrencyid + 1)
    Where customerid = custId and concurrencyid = conCurrId;
END //
DELIMITER ;

-- 5. Delete a customer (only if nobody else changed it first)
DELIMITER //
CREATE PROCEDURE usp_CustomerDelete (in custId int, in conCurrId int)
BEGIN
	Delete from customers where customerid = custId and concurrencyid = conCurrId;
END //
DELIMITER ;
