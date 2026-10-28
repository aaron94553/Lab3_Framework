using System;

using MMABooksTools;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MMABooksProps
{
    /// <summary>
    /// Holds the field values for one customer record. Used to move data between the database class and the business class.
    /// </summary>
    [Serializable()]
    public class CustomerProps : IBaseProps
    {
        #region Auto-implemented Properties
        public int CustomerID { get; set; } = 0;

        public string Name { get; set; } = "";

        public string Address { get; set; } = "";

        public string City { get; set; } = "";

        public string State { get; set; } = "";

        public string ZipCode { get; set; } = "";

        /// <summary>
        /// ConcurrencyID. Don't manipulate directly.
        /// </summary>
        public int ConcurrencyID { get; set; } = 0;
        #endregion

        /// <summary>
        /// Makes a field-by-field copy of this CustomerProps object.
        /// </summary>
        public object Clone()
        {
            CustomerProps p = new CustomerProps();
            p.CustomerID = this.CustomerID;
            p.Name = this.Name;
            p.Address = this.Address;
            p.City = this.City;
            p.State = this.State;
            p.ZipCode = this.ZipCode;
            p.ConcurrencyID = this.ConcurrencyID;
            return p;
        }

        /// <summary>
        /// Serializes this object to a JSON string.
        /// </summary>
        public string GetState()
        {
            string jsonString;
            jsonString = JsonSerializer.Serialize(this);
            return jsonString;
        }

        /// <summary>
        /// Sets every field from a JSON string created by GetState.
        /// </summary>
        public void SetState(string jsonString)
        {
            CustomerProps p = JsonSerializer.Deserialize<CustomerProps>(jsonString);
            this.CustomerID = p.CustomerID;
            this.Name = p.Name;
            this.Address = p.Address;
            this.City = p.City;
            this.State = p.State;
            this.ZipCode = p.ZipCode;
            this.ConcurrencyID = p.ConcurrencyID;
        }

        /// <summary>
        /// Sets every field from the current row of a data reader.
        /// </summary>
        public void SetState(DBDataReader dr)
        {
            this.CustomerID = (Int32)dr["CustomerID"];
            this.Name = (string)dr["Name"];
            this.Address = (string)dr["Address"];
            this.City = (string)dr["City"];
            this.State = ((string)dr["State"]).Trim();
            this.ZipCode = ((string)dr["ZipCode"]).Trim();
            this.ConcurrencyID = (Int32)dr["ConcurrencyID"];
        }
    }
}
