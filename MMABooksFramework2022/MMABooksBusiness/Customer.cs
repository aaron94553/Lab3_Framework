using System;

using MMABooksTools;
using MMABooksProps;
using MMABooksDB;

using System.Collections.Generic;

namespace MMABooksBusiness
{
    /// <summary>
    /// Business object for a customer. Validates property values and saves changes through CustomerDB.
    /// </summary>
    public class Customer : BaseBusiness
    {
        /// <summary>
        /// Read-only. The database assigns the id when the customer is created.
        /// </summary>
        public int CustomerID
        {
            get
            {
                return ((CustomerProps)mProps).CustomerID;
            }
        }

        /// <summary>
        /// Customer name. Required. Must be 1-100 characters or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public string Name
        {
            get
            {
                return ((CustomerProps)mProps).Name;
            }

            set
            {
                if (!(value == ((CustomerProps)mProps).Name))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 100)
                    {
                        mRules.RuleBroken("Name", false);
                        ((CustomerProps)mProps).Name = value;
                        mIsDirty = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Name must be between 1 and 100 characters long.");
                    }
                }
            }
        }

        /// <summary>
        /// Street address. Required. Must be 1-50 characters or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public string Address
        {
            get
            {
                return ((CustomerProps)mProps).Address;
            }

            set
            {
                if (!(value == ((CustomerProps)mProps).Address))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 50)
                    {
                        mRules.RuleBroken("Address", false);
                        ((CustomerProps)mProps).Address = value;
                        mIsDirty = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Address must be between 1 and 50 characters long.");
                    }
                }
            }
        }

        /// <summary>
        /// City. Required. Must be 1-20 characters or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public string City
        {
            get
            {
                return ((CustomerProps)mProps).City;
            }

            set
            {
                if (!(value == ((CustomerProps)mProps).City))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 20)
                    {
                        mRules.RuleBroken("City", false);
                        ((CustomerProps)mProps).City = value;
                        mIsDirty = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("City must be between 1 and 20 characters long.");
                    }
                }
            }
        }

        /// <summary>
        /// Two-letter state code. Required. Must be exactly 2 characters or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public string State
        {
            get
            {
                return ((CustomerProps)mProps).State;
            }

            set
            {
                if (!(value == ((CustomerProps)mProps).State))
                {
                    if (value.Trim().Length == 2)
                    {
                        mRules.RuleBroken("State", false);
                        ((CustomerProps)mProps).State = value;
                        mIsDirty = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("State must be exactly 2 characters long.");
                    }
                }
            }
        }

        /// <summary>
        /// Zip code. Required. Must be 1-15 characters or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public string ZipCode
        {
            get
            {
                return ((CustomerProps)mProps).ZipCode;
            }

            set
            {
                if (!(value == ((CustomerProps)mProps).ZipCode))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 15)
                    {
                        mRules.RuleBroken("ZipCode", false);
                        ((CustomerProps)mProps).ZipCode = value;
                        mIsDirty = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("ZipCode must be between 1 and 15 characters long.");
                    }
                }
            }
        }

        /// <summary>
        /// Returns a List of every customer in the database.
        /// </summary>
        public override object GetList()
        {
            List<Customer> customers = new List<Customer>();
            List<CustomerProps> props = new List<CustomerProps>();

            props = (List<CustomerProps>)mdbReadable.RetrieveAll();
            foreach (CustomerProps prop in props)
            {
                Customer c = new Customer(prop);
                customers.Add(c);
            }

            return customers;
        }

        /// <summary>
        /// Sets default property values for a new customer. No defaults are needed.
        /// </summary>
        protected override void SetDefaultProperties()
        {
        }

        /// <summary>
        /// Sets required fields for a record.
        /// </summary>
        protected override void SetRequiredRules()
        {
            mRules.RuleBroken("Name", true);
            mRules.RuleBroken("Address", true);
            mRules.RuleBroken("City", true);
            mRules.RuleBroken("State", true);
            mRules.RuleBroken("ZipCode", true);
        }

        /// <summary>
        /// Instantiates mProps and mOldProps as new Props objects.
        /// Instantiates mbdReadable and mdbWriteable as new DB objects.
        /// </summary>
        protected override void SetUp()
        {
            mProps = new CustomerProps();
            mOldProps = new CustomerProps();

            mdbReadable = new CustomerDB();
            mdbWriteable = new CustomerDB();
        }

        #region constructors
        /// <summary>
        /// Default constructor - assumes a new record that is not in the database.
        /// </summary>
        public Customer() : base()
        {
        }

        /// <summary>
        /// Use this constructor when the customer is already in the database.
        /// </summary>
        /// <param name="key">CustomerID of a record in the database.</param>
        public Customer(int key)
            : base(key)
        {
        }

        /// <summary>
        /// Creates a customer from a props object. Used by GetList.
        /// </summary>
        private Customer(CustomerProps props)
            : base(props)
        {
        }
        #endregion
    }
}
