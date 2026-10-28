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
    public class CustomerTests
    {
        [SetUp]
        public void TestResetDatabase()
        {
            CustomerDB db = new CustomerDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewCustomerConstructor()
        {
            // not in Data Store - no id
            Customer c = new Customer();
            Assert.AreEqual(0, c.CustomerID);
            Assert.AreEqual(string.Empty, c.Name);
            Assert.AreEqual(string.Empty, c.Address);
            Assert.AreEqual(string.Empty, c.City);
            Assert.AreEqual(string.Empty, c.State);
            Assert.AreEqual(string.Empty, c.ZipCode);
            Assert.IsTrue(c.IsNew);
            Assert.IsFalse(c.IsValid);
        }

        [Test]
        public void TestRetrieveFromDataStoreContructor()
        {
            // retrieves from Data Store
            Customer c = new Customer(1);
            Assert.AreEqual(1, c.CustomerID);
            Assert.AreEqual("Molunguri, A", c.Name);
            Assert.IsFalse(c.IsNew);
            Assert.IsTrue(c.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            Customer c = new Customer();
            c.Name = "Mouse, Mickey";
            c.Address = "101 Main Street";
            c.City = "Orlando";
            c.State = "FL";
            c.ZipCode = "10001";
            c.Save();
            Customer c2 = new Customer(c.CustomerID);
            Assert.AreEqual(c2.Name, c.Name);
            Assert.AreEqual(c2.Address, c.Address);
            Assert.AreEqual(c2.City, c.City);
            Assert.AreEqual(c2.State, c.State);
            Assert.AreEqual(c2.ZipCode, c.ZipCode);
        }

        [Test]
        public void TestUpdate()
        {
            Customer c = new Customer(1);
            c.Name = "Edited Name";
            c.City = "Klamath Falls";
            c.Save();

            Customer c2 = new Customer(1);
            Assert.AreEqual(c2.Name, c.Name);
            Assert.AreEqual(c2.City, c.City);
        }

        [Test]
        public void TestDelete()
        {
            Customer c = new Customer(2);
            c.Delete();
            c.Save();
            Assert.Throws<Exception>(() => new Customer(2));
        }

        [Test]
        public void TestGetList()
        {
            Customer c = new Customer();
            List<Customer> customers = (List<Customer>)c.GetList();
            Assert.AreEqual(696, customers.Count);
            Assert.IsTrue(customers[0].Name.Length > 0);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            // not in Data Store - all properties must be provided
            Customer c = new Customer();
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            Customer c = new Customer();
            Assert.Throws<Exception>(() => c.Save());
            c.Name = "Mouse, Mickey";
            Assert.Throws<Exception>(() => c.Save());
            c.State = "FL";
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            Customer c = new Customer();
            Assert.Throws<ArgumentOutOfRangeException>(() => c.State = "???");
            Assert.Throws<ArgumentOutOfRangeException>(() => c.City = "This city name is way too long");
            Assert.Throws<ArgumentOutOfRangeException>(() => c.Name = new string('X', 101));
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            Customer c1 = new Customer(1);
            Customer c2 = new Customer(1);

            c1.Name = "Updated first";
            c1.Save();

            c2.Name = "Updated second";
            Assert.Throws<Exception>(() => c2.Save());
        }
    }
}
