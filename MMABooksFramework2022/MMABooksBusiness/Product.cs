using System;

using MMABooksTools;
using MMABooksProps;
using MMABooksDB;

using System.Collections.Generic;

namespace MMABooksBusiness
{
    /// <summary>
    /// Business object for a product. Validates property values and saves changes through ProductDB.
    /// </summary>
    public class Product : BaseBusiness
    {
        /// <summary>
        /// Read-only. The database assigns the id when the product is created.
        /// </summary>
        public int ProductID
        {
            get
            {
                return ((ProductProps)mProps).ProductID;
            }
        }

        /// <summary>
        /// Product code. Required. Must be 1-10 characters or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public string ProductCode
        {
            get
            {
                return ((ProductProps)mProps).ProductCode;
            }

            set
            {
                if (!(value == ((ProductProps)mProps).ProductCode))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 10)
                    {
                        mRules.RuleBroken("ProductCode", false);
                        ((ProductProps)mProps).ProductCode = value;
                        mIsDirty = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("ProductCode must be between 1 and 10 characters long.");
                    }
                }
            }
        }

        /// <summary>
        /// Product description. Required. Must be 1-50 characters or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public string Description
        {
            get
            {
                return ((ProductProps)mProps).Description;
            }

            set
            {
                if (!(value == ((ProductProps)mProps).Description))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 50)
                    {
                        mRules.RuleBroken("Description", false);
                        ((ProductProps)mProps).Description = value;
                        mIsDirty = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Description must be between 1 and 50 characters long.");
                    }
                }
            }
        }

        // For the numbers, the rule is cleared even when the value doesn't change,
        // so setting a price or quantity of 0 still counts as "set".
        /// <summary>
        /// Price per unit. Required. Cannot be negative or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public decimal UnitPrice
        {
            get
            {
                return ((ProductProps)mProps).UnitPrice;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("UnitPrice cannot be negative.");
                }
                mRules.RuleBroken("UnitPrice", false);
                if (!(value == ((ProductProps)mProps).UnitPrice))
                {
                    ((ProductProps)mProps).UnitPrice = value;
                    mIsDirty = true;
                }
            }
        }

        /// <summary>
        /// Quantity in stock. Required. Cannot be negative or an ArgumentOutOfRangeException is thrown.
        /// </summary>
        public int OnHandQuantity
        {
            get
            {
                return ((ProductProps)mProps).OnHandQuantity;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("OnHandQuantity cannot be negative.");
                }
                mRules.RuleBroken("OnHandQuantity", false);
                if (!(value == ((ProductProps)mProps).OnHandQuantity))
                {
                    ((ProductProps)mProps).OnHandQuantity = value;
                    mIsDirty = true;
                }
            }
        }

        /// <summary>
        /// Returns a List of every product in the database.
        /// </summary>
        public override object GetList()
        {
            List<Product> products = new List<Product>();
            List<ProductProps> props = new List<ProductProps>();

            props = (List<ProductProps>)mdbReadable.RetrieveAll();
            foreach (ProductProps prop in props)
            {
                Product p = new Product(prop);
                products.Add(p);
            }

            return products;
        }

        /// <summary>
        /// Sets default property values for a new product. No defaults are needed.
        /// </summary>
        protected override void SetDefaultProperties()
        {
        }

        /// <summary>
        /// Sets required fields for a record.
        /// </summary>
        protected override void SetRequiredRules()
        {
            mRules.RuleBroken("ProductCode", true);
            mRules.RuleBroken("Description", true);
            mRules.RuleBroken("UnitPrice", true);
            mRules.RuleBroken("OnHandQuantity", true);
        }

        /// <summary>
        /// Instantiates mProps and mOldProps as new Props objects.
        /// Instantiates mbdReadable and mdbWriteable as new DB objects.
        /// </summary>
        protected override void SetUp()
        {
            mProps = new ProductProps();
            mOldProps = new ProductProps();

            mdbReadable = new ProductDB();
            mdbWriteable = new ProductDB();
        }

        #region constructors
        /// <summary>
        /// Default constructor - assumes a new record that is not in the database.
        /// </summary>
        public Product() : base()
        {
        }

        /// <summary>
        /// Use this constructor when the product is already in the database.
        /// </summary>
        /// <param name="key">ProductID of a record in the database.</param>
        public Product(int key)
            : base(key)
        {
        }

        /// <summary>
        /// Creates a product from a props object. Used by GetList.
        /// </summary>
        private Product(ProductProps props)
            : base(props)
        {
        }
        #endregion
    }
}
