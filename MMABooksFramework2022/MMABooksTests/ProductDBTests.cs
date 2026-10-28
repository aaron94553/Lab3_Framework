using NUnit.Framework;

using MMABooksProps;
using MMABooksDB;

using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;

using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace MMABooksTests
{
    public class ProductDBTests
    {
        ProductDB db;

        [SetUp]
        public void ResetData()
        {
            db = new ProductDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetProductData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestRetrieve()
        {
            ProductProps p = (ProductProps)db.Retrieve(1);
            Assert.AreEqual(1, p.ProductID);
            Assert.AreEqual("A4CS", p.ProductCode);
            Assert.AreEqual(56.50m, p.UnitPrice);
        }

        [Test]
        public void TestRetrieveAll()
        {
            List<ProductProps> list = (List<ProductProps>)db.RetrieveAll();
            Assert.AreEqual(16, list.Count);
        }

        [Test]
        public void TestDelete()
        {
            ProductProps p = (ProductProps)db.Retrieve(16);
            Assert.True(db.Delete(p));
            Assert.Throws<Exception>(() => db.Retrieve(16));
        }

        [Test]
        public void TestUpdate()
        {
            ProductProps p = (ProductProps)db.Retrieve(1);
            p.Description = "Updated description";
            p.OnHandQuantity = 10;
            Assert.True(db.Update(p));
            p = (ProductProps)db.Retrieve(1);
            Assert.AreEqual("Updated description", p.Description);
            Assert.AreEqual(10, p.OnHandQuantity);
        }

        [Test]
        public void TestUpdateFieldTooLong()
        {
            ProductProps p = (ProductProps)db.Retrieve(1);
            p.Description = "This description is definitely longer than fifty characters";
            Assert.Throws<MySqlException>(() => db.Update(p));
        }

        [Test]
        public void TestCreate()
        {
            ProductProps p = new ProductProps();
            p.ProductCode = "TEST";
            p.Description = "A brand new product";
            p.UnitPrice = 25.75m;
            p.OnHandQuantity = 50;
            db.Create(p);
            ProductProps p2 = (ProductProps)db.Retrieve(p.ProductID);
            Assert.AreEqual(p.ProductCode, p2.ProductCode);
            Assert.AreEqual(p.Description, p2.Description);
            Assert.AreEqual(p.UnitPrice, p2.UnitPrice);
            Assert.AreEqual(p.OnHandQuantity, p2.OnHandQuantity);
        }

        [Test]
        public void TestCreateCodeTooLong()
        {
            ProductProps p = new ProductProps();
            p.ProductCode = "WAYTOOLONGCODE";
            p.Description = "Bad product";
            p.UnitPrice = 1m;
            p.OnHandQuantity = 1;
            Assert.Throws<MySqlException>(() => db.Create(p));
        }
    }
}
