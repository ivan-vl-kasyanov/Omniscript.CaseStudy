using System;
using System.Collections.Generic;
using System.Linq;

using Omniscript.CaseStudy.Server.DataAccess.Clients;
using Omniscript.CaseStudy.Server.DataAccess.DAO;
using Omniscript.CaseStudy.Server.Models.Customer.CreateCustomer;
using Omniscript.CaseStudy.Server.Models.Entities;

using SQLite;

namespace Omniscript.CaseStudy.Server.DataAccess.Repositories
{
    /// <summary>
    /// Customer data repository.
    /// </summary>
    public sealed class CustomerRepository
    {
        private readonly SQLiteConnection _connection;

        /// <summary>
        /// Constructor of the customer data repository.
        /// </summary>
        /// <param name="databaseClient">Database client instance.</param>
        public CustomerRepository(DatabaseClient databaseClient)
        {
            if (databaseClient?.Connection == null)
            {
                var exceptionMessage = $"{nameof(databaseClient.Connection)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }
            _connection = databaseClient.Connection;
        }

        /// <summary>
        /// Returns requested customers.
        /// </summary>
        /// <param name="skip">Amount of skipped customers.</param>
        /// <param name="take">Amount of taken customers.</param>
        /// <returns>Total customers count and requested customers.</returns>
        public (int CustomersTotalCount, IEnumerable<CustomerModel> Customers) GetCustomers(
            int skip,
            int take)
        {
            var customersTotalCount = _connection
                .Table<CustomerDao>()
                .Count();

            var customersRaw = _connection
                .Table<CustomerDao>()
                .OrderByDescending(customer => customer.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList();
            var customerIds = customersRaw
                .Select(customer => customer.Id)
                .ToHashSet();
            var addressesRaw = _connection
                .Table<AddressDao>()
                .Where(address => customerIds.Contains(address.CustomerId))
                .ToDictionary(
                    key => key.CustomerId,
                    value => value);

            var customers = customersRaw
                .Select(customerRaw =>
                {
                    var addressRaw = addressesRaw[customerRaw.Id];
                    var address = new AddressModel(
                        addressRaw.Country,
                        addressRaw.City,
                        addressRaw.Street);
                    var customer = new CustomerModel(
                        customerRaw.Id,
                        customerRaw.Email,
                        address,
                        customerRaw.CreatedAt,
                        customerRaw.IsArchived,
                        customerRaw.PurchasedAt
                    );

                    return customer;
                });

            return (customersTotalCount, customers);
        }

        /// <summary>
        /// Creates new customer entity.
        /// </summary>
        /// <param name="newCustomer">New customer entity.</param>
        public void CreateCustomer(NewCustomerModel newCustomer)
        {
            var isEmailAlreadyInUse = _connection
                .Table<CustomerDao>()
                .Any(customer => customer.Email == newCustomer.Email.ToLowerInvariant());
            if (isEmailAlreadyInUse)
            {
                var exceptionMessage = $"New customer's email \"{newCustomer.Email.ToLowerInvariant()}\" is already in use.";

                throw new ArgumentException(exceptionMessage);
            }

            var newCustomerRaw = new CustomerDao()
            {
                Email = newCustomer.Email.ToLowerInvariant(),
                IsArchived = false,
                PurchasedAt = null,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _connection.Insert(newCustomerRaw);

            var newAddressRaw = new AddressDao()
            {
                CustomerId = newCustomerRaw.Id,
                Country = newCustomer.Address.Country,
                City = newCustomer.Address.City,
                Street = newCustomer.Address.Street,
            };
            _connection.Insert(newAddressRaw);
        }

        /// <summary>
        /// Updates customer's address.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <param name="newAddress">Customer's new address instance.</param>
        public void UpdateAddress(
            long customerId,
            AddressModel newAddress)
        {
            var isAddressWithIdExists = _connection
                .Table<AddressDao>()
                .Any(address => address.CustomerId == customerId);
            if (!isAddressWithIdExists)
            {
                var exceptionMessage = $"Cannot find address affiliated with customer ID:{customerId}.";

                throw new ArgumentException(exceptionMessage);
            }

            var newAddressRaw = new AddressDao()
            {
                CustomerId = customerId,
                Country = newAddress.Country,
                City = newAddress.City,
                Street = newAddress.Street,
            };
            _connection.Update(newAddressRaw);
        }

        /// <summary>
        /// Updates customer's purchase info.
        /// </summary>
        /// <param name="email">E-mail address.</param>
        /// <param name="orderCreatedAt">Order creation timestamp.</param>
        public void UpdateCustomerPurchase(
            string email,
            DateTimeOffset orderCreatedAt)
        {
            var isCustomerWithEmailExists = _connection
                .Table<CustomerDao>()
                .Any(customer => customer.Email == email.ToLowerInvariant());
            if (isCustomerWithEmailExists)
            {
                var exceptionMessage = $"Cannot find customer with email \"{email.ToLowerInvariant()}\".";

                throw new ArgumentException(exceptionMessage);
            }

            var customer = _connection
                .Table<CustomerDao>()
                .Single(cstmr => cstmr.Email == email.ToLowerInvariant());
            customer.PurchasedAt = orderCreatedAt.ToUniversalTime();
            _connection.Update(customer);
        }
    }
}