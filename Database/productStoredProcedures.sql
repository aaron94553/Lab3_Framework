-- Product stored procedures (modeled on the state stored procedures)
USE MMABooksFramework;

DROP PROCEDURE IF EXISTS usp_ProductSelect;
DROP PROCEDURE IF EXISTS usp_ProductSelectAll;
DROP PROCEDURE IF EXISTS usp_ProductCreate;
DROP PROCEDURE IF EXISTS usp_ProductUpdate;
DROP PROCEDURE IF EXISTS usp_ProductDelete;

-- 1. Select one product
DELIMITER //
CREATE PROCEDURE usp_ProductSelect (in prodId int)
BEGIN
	Select * from products where productid = prodId;
END //
DELIMITER ;

-- 2. Select all products
DELIMITER //
CREATE PROCEDURE usp_ProductSelectAll ()
BEGIN
	Select * from products order by productcode;
END //
DELIMITER ;

-- 3. Create a product (returns the new id in prodId)
DELIMITER //
CREATE PROCEDURE usp_ProductCreate (out prodId int, in code_p varchar(10), in description_p varchar(50), in unitprice_p decimal(10,4), in onhand_p int)
BEGIN
	Insert into products (productcode, description, unitprice, onhandquantity, concurrencyid)
    Values (code_p, description_p, unitprice_p, onhand_p, 1);
    Select LAST_INSERT_ID() into prodId;
END //
DELIMITER ;

-- 4. Update a product (only if nobody else changed it first)
DELIMITER //
CREATE PROCEDURE usp_ProductUpdate (in prodId int, in code_p varchar(10), in description_p varchar(50), in unitprice_p decimal(10,4), in onhand_p int, in conCurrId int)
BEGIN
	Update products
    Set productcode = code_p, description = description_p, unitprice = unitprice_p, onhandquantity = onhand_p,
        concurrencyid = (concurrencyid + 1)
    Where productid = prodId and concurrencyid = conCurrId;
END //
DELIMITER ;

-- 5. Delete a product (only if nobody else changed it first)
DELIMITER //
CREATE PROCEDURE usp_ProductDelete (in prodId int, in conCurrId int)
BEGIN
	Delete from products where productid = prodId and concurrencyid = conCurrId;
END //
DELIMITER ;
