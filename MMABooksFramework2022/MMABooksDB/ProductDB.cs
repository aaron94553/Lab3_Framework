using System;
using System.Collections.Generic;

using MMABooksTools;
using MMABooksProps;

using System.Data;

// *** I use an "alias" for the ado.net classes throughout my code
// When I switch to an oracle database, I ONLY have to change the actual classes here
using DBBase = MMABooksTools.BaseSQLDB;
using DBConnection = MySql.Data.MySqlClient.MySqlConnection;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using DBParameter = MySql.Data.MySqlClient.MySqlParameter;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using DBDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
using DBDbType = MySql.Data.MySqlClient.MySqlDbType;

namespace MMABooksDB
{
    /// <summary>
    /// Performs CRUD operations on the Products table through stored procedures.
    /// </summary>
    public class ProductDB : DBBase, IReadDB, IWriteDB
    {
        /// <summary>
        /// Creates a ProductDB that uses the connection string from mySqlSettings.json.
        /// </summary>
        public ProductDB() : base() { }
        /// <summary>
        /// Creates a ProductDB that uses an existing connection.
        /// </summary>
        public ProductDB(DBConnection cn) : base(cn) { }

        /// <summary>
        /// Inserts a new product and sets its ProductID and ConcurrencyID. Returns the updated props.
        /// </summary>
        public IBaseProps Create(IBaseProps p)
        {
            int rowsAffected = 0;
            ProductProps props = (ProductProps)p;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_ProductCreate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("prodId", DBDbType.Int32);
            command.Parameters.Add("code_p", DBDbType.VarChar);
            command.Parameters.Add("description_p", DBDbType.VarChar);
            command.Parameters.Add("unitprice_p", DBDbType.Decimal);
            command.Parameters.Add("onhand_p", DBDbType.Int32);
            command.Parameters[0].Direction = ParameterDirection.Output;
            command.Parameters["code_p"].Value = props.ProductCode;
            command.Parameters["description_p"].Value = props.Description;
            command.Parameters["unitprice_p"].Value = props.UnitPrice;
            command.Parameters["onhand_p"].Value = props.OnHandQuantity;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ProductID = Convert.ToInt32(command.Parameters[0].Value);
                    props.ConcurrencyID = 1;
                    return props;
                }
                else
                    throw new Exception("Unable to insert record. " + props.GetState());
            }
            catch (Exception e)
            {
                // log this error
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }

        /// <summary>
        /// Deletes a product. Throws an exception if the record was changed by another user (ConcurrencyID mismatch).
        /// </summary>
        public bool Delete(IBaseProps p)
        {
            ProductProps props = (ProductProps)p;
            int rowsAffected = 0;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_ProductDelete";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("prodId", DBDbType.Int32);
            command.Parameters.Add("conCurrId", DBDbType.Int32);
            command.Parameters["prodId"].Value = props.ProductID;
            command.Parameters["conCurrId"].Value = props.ConcurrencyID;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    return true;
                }
                else
                {
                    string message = "Record cannot be deleted. It has been edited by another user.";
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {
                // log this exception
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }

        /// <summary>
        /// Retrieves one product by ProductID. Throws an exception if the record does not exist.
        /// </summary>
        public IBaseProps Retrieve(object key)
        {
            DBDataReader data = null;
            ProductProps props = new ProductProps();
            DBCommand command = new DBCommand();

            command.CommandText = "usp_ProductSelect";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("prodId", DBDbType.Int32);
            command.Parameters["prodId"].Value = (int)key;

            try
            {
                data = RunProcedure(command);
                if (!data.IsClosed)
                {
                    if (data.Read())
                    {
                        props.SetState(data);
                    }
                    else
                        throw new Exception("Record does not exist in the database.");
                }
                return props;
            }
            catch (Exception e)
            {
                // log this exception
                throw;
            }
            finally
            {
                if (data != null)
                {
                    if (!data.IsClosed)
                        data.Close();
                }
            }
        }

        /// <summary>
        /// Retrieves every product. Returns a List of ProductProps.
        /// </summary>
        public object RetrieveAll()
        {
            List<ProductProps> list = new List<ProductProps>();
            DBDataReader reader = null;
            ProductProps props;

            try
            {
                reader = RunProcedure("usp_ProductSelectAll");
                if (!reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        props = new ProductProps();
                        props.SetState(reader);
                        list.Add(props);
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                // log this exception
                throw;
            }
            finally
            {
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Updates a product and increments its ConcurrencyID. Throws an exception if the record was changed by another user.
        /// </summary>
        public bool Update(IBaseProps p)
        {
            int rowsAffected = 0;
            ProductProps props = (ProductProps)p;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_ProductUpdate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("prodId", DBDbType.Int32);
            command.Parameters.Add("code_p", DBDbType.VarChar);
            command.Parameters.Add("description_p", DBDbType.VarChar);
            command.Parameters.Add("unitprice_p", DBDbType.Decimal);
            command.Parameters.Add("onhand_p", DBDbType.Int32);
            command.Parameters.Add("conCurrId", DBDbType.Int32);
            command.Parameters["prodId"].Value = props.ProductID;
            command.Parameters["code_p"].Value = props.ProductCode;
            command.Parameters["description_p"].Value = props.Description;
            command.Parameters["unitprice_p"].Value = props.UnitPrice;
            command.Parameters["onhand_p"].Value = props.OnHandQuantity;
            command.Parameters["conCurrId"].Value = props.ConcurrencyID;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ConcurrencyID++;
                    return true;
                }
                else
                {
                    string message = "Record cannot be updated. It has been edited by another user.";
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {
                // log this exception
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }
    }
}
