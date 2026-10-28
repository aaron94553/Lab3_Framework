using NUnit.Framework;

using MMABooksBusiness;
using MMABooksProps;
using MMABooksDB;

using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;

using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        [SetUp]
        public void TestResetDatabase()
        {
            ProductDB db = new ProductDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetProductData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewProductConstructor()
        {
            // not in Data Store - no id
            Product p = new Product();
            Assert.AreEqual(0, p.ProductID);
            Assert.AreEqual(string.Empty, p.ProductCode);
            Assert.AreEqual(string.Empty, p.Description);
            Assert.AreEqual(0m, p.UnitPrice);
            Assert.AreEqual(0, p.OnHandQuantity);
            Assert.IsTrue(p.IsNew);
            Assert.IsFalse(p.IsValid);
        }

        [Test]
        public void TestRetrieveFromDataStoreContructor()
        {
            Product p = new Product(1);
            Assert.AreEqual(1, p.ProductID);
            Assert.AreEqual("A4CS", p.ProductCode);
            Assert.IsTrue(p.Description.Length > 0);
            Assert.IsFalse(p.IsNew);
            Assert.IsTrue(p.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            Product p = new Product();
            p.ProductCode = "TEST";
            p.Description = "A brand new product";
            p.UnitPrice = 25.75m;
            p.OnHandQuantity = 50;
            p.Save();
            Product p2 = new Product(p.ProductID);
            Assert.AreEqual(p2.ProductCode, p.ProductCode);
            Assert.AreEqual(p2.Description, p.Description);
            Assert.AreEqual(p2.UnitPrice, p.UnitPrice);
            Assert.AreEqual(p2.OnHandQuantity, p.OnHandQuantity);
        }

        [Test]
        public void TestUpdate()
        {
            Product p = new Product(1);
            p.Description = "Edited description";
            p.UnitPrice = 60m;
            p.Save();

            Product p2 = new Product(1);
            Assert.AreEqual(p2.Description, p.Description);
            Assert.AreEqual(p2.UnitPrice, p.UnitPrice);
        }

        [Test]
        public void TestDelete()
        {
            Product p = new Product(16);
            p.Delete();
            p.Save();
            Assert.Throws<Exception>(() => new Product(16));
        }

        [Test]
        public void TestGetList()
        {
            Product p = new Product();
            List<Product> products = (List<Product>)p.GetList();
            Assert.AreEqual(16, products.Count);
            Assert.AreEqual("A4CS", products[0].ProductCode);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            Product p = new Product();
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            Product p = new Product();
            Assert.Throws<Exception>(() => p.Save());
            p.ProductCode = "TEST";
            Assert.Throws<Exception>(() => p.Save());
            p.Description = "Missing price and quantity";
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            Product p = new Product();
            Assert.Throws<ArgumentOutOfRangeException>(() => p.ProductCode = "WAYTOOLONGCODE");
            Assert.Throws<ArgumentOutOfRangeException>(() => p.UnitPrice = -1m);
            Assert.Throws<ArgumentOutOfRangeException>(() => p.OnHandQuantity = -5);
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            Product p1 = new Product(1);
            Product p2 = new Product(1);

            p1.Description = "Updated first";
            p1.Save();

            p2.Description = "Updated second";
            Assert.Throws<Exception>(() => p2.Save());
        }

        [Test]
        public void TestInvalidSetDoesNotChangeProperty()
        {
            Product p = new Product(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => p.UnitPrice = -1m);
            Assert.AreEqual(56.50m, p.UnitPrice);
            Assert.IsFalse(p.IsDirty);
        }

        [Test]
        public void TestIsDirty()
        {
            Product p = new Product(1);
            Assert.IsFalse(p.IsDirty);
            p.OnHandQuantity = 10;
            Assert.IsTrue(p.IsDirty);
            p.Save();
            Assert.IsFalse(p.IsDirty);
        }

        [Test]
        public void TestUndoChanges()
        {
            Product p = new Product(1);
            string originalDescription = p.Description;
            p.Description = "Changed description";
            p.UndoChanges();
            Assert.AreEqual(originalDescription, p.Description);
            Assert.IsFalse(p.IsDirty);
        }

        [Test]
        public void TestIsDeleted()
        {
            Product p = new Product(16);
            Assert.IsFalse(p.IsDeleted);
            p.Delete();
            Assert.IsTrue(p.IsDeleted);
            p.UndoChanges();
            Assert.IsFalse(p.IsDeleted);
        }

        [Test]
        public void TestToString()
        {
            Product p = new Product(1);
            Assert.IsTrue(p.ToString().Contains(p.ProductCode));
            Assert.IsTrue(p.ToString().Contains("ASP.NET"));
        }
    }
}
