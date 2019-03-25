﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  .Net Core Plugin Manager is distributed under the GNU General Public License version 3 and  
 *  is also available under alternative licenses negotiated directly with Simon Carter.  
 *  If you obtained Service Manager under the GPL, then the GPL applies to all loadable 
 *  Service Manager modules used on your system as well. The GPL (version 3) is 
 *  available at https://opensource.org/licenses/GPL-3.0
 *
 *  This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *  See the GNU General Public License for more details.
 *
 *  The Original Code was created by Simon Carter (s1cart3r@gmail.com)
 *
 *  Copyright (c) 2018 - 2019 Simon Carter.  All Rights Reserved.
 *
 *  Product:  Login Plugin
 *  
 *  File: MockAccountProvider.cs
 *
 *  Purpose:  Mock IAccountProvider for tesing purpose
 *
 *  Date        Name                Reason
 *  09/12/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Globalization;

using Middleware;
using Middleware.Accounts;
using Middleware.Accounts.Invoices;
using Middleware.Accounts.Orders;

namespace AspNetCore.PluginManager.DemoWebsite.Classes
{
    public class MockAccountProvider : IAccountProvider
    {
        #region Private Static Members

        private static List<DeliveryAddress> _deliveryAddresses;

        private static Marketing _marketing;

        private static List<Order> _orders;

        private static List<Invoice> _invoices;

        #endregion Private Static Members

        #region Change Password

        public bool ChangePassword(in long userId, in string newPassword)
        {
            return newPassword.Equals("Pa$$w0rd");
        }

        #endregion Change Password

        #region Address Options

        public AddressOptions GetAddressOptions(in AddressOption addressOption)
        {
            return AddressOptions.AddressLine1Mandatory | AddressOptions.AddressLine1Show |
                AddressOptions.AddressLine2Show |
                AddressOptions.CityMandatory | AddressOptions.CityShow |
                AddressOptions.PostCodeMandatory | AddressOptions.PostCodeShow |
                AddressOptions.TelephoneShow;
        }

        #endregion Address Options

        #region User Contact Details

        public bool GetUserAccountDetails(in Int64 userId, out string firstName, out string lastName, out string email, out bool emailConfirmed,
            out string telephone, out bool telephoneConfirmed)
        {
            firstName = "Fred";
            lastName = "Bloggs";
            email = "fred@bloggs.com";
            emailConfirmed = true;
            telephone = "0044 121 345 6789";
            telephoneConfirmed = false;

            return true;
        }

        public bool SetUserAccountDetails(in Int64 userId, in string firstName, in string lastName, in string email, in string telephone)
        {
            return firstName == "Fred" && lastName == "Bloggs";
        }

        public bool ConfirmEmailAddress(in Int64 userId, in string confirmationCode)
        {
            return confirmationCode.Equals("NewEmail");
        }

        public bool ConfirmTelephoneNumber(in Int64 userId, in string confirmationCode)
        {
            return confirmationCode.Equals("NewTelephone");
        }

        #endregion User Contact Details

        #region Create Account

        public bool CreateAccount(in string email, in string firstName, in string surname, in string password,
            in string telephone, in string businessName, in string addressLine1, in string addressLine2,
            in string addressLine3, in string city, in string county, in string postcode, in string countryCode,
            out long userId)
        {
            userId = 2;


            return true;
        }

        #endregion Create Account

        #region Billing Address

        public bool SetBillingAddress(in long userId, in Address billingAddress)
        {
            return billingAddress.AddressLine1 == "Mike St";
        }

        public Address GetBillingAddress(in long userId)
        {
            return new Address(String.Empty, "Mike St", String.Empty, String.Empty, "London", String.Empty, "L1 1AA", "GB");
        }

        #endregion Billing Address

        #region Delivery Address

        public bool SetDeliveryAddress(in long userId, in DeliveryAddress deliveryAddress)
        {
            return true;
        }

        public List<DeliveryAddress> GetDeliveryAddresses(in long userId)
        {
            if (_deliveryAddresses == null)
                _deliveryAddresses = new List<DeliveryAddress>()
                    {
                        new DeliveryAddress(1, String.Empty, "1 Mike St", String.Empty, String.Empty, "London", String.Empty, "L1 1AA", "GB", 5.99m),
                        new DeliveryAddress(2, String.Empty, "29 5th Avenue", String.Empty, String.Empty, "New York", String.Empty, "49210", "US", 5.99m),
                    };

            return _deliveryAddresses;
        }

        public bool AddDeliveryAddress(in Int64 userId, in DeliveryAddress deliveryAddress)
        {
            _deliveryAddresses.Add(deliveryAddress);
            return true;
        }

        public DeliveryAddress GetDeliveryAddress(in Int64 userId, in int deliveryAddressId)
        {
            foreach (DeliveryAddress address in GetDeliveryAddresses(userId))
            {
                if (address.AddressId == deliveryAddressId)
                    return address;
            }

            return null;
        }

        public bool DeleteDeliveryAddress(in long userId, in DeliveryAddress deliveryAddress)
        {
            if (deliveryAddress.AddressId == 1)
                return false;

            _deliveryAddresses.Remove(deliveryAddress);

            return true;
        }

        #endregion Delivery Address

        #region Marketing Preferences

        public MarketingOptions GetMarketingOptions()
        {
            return MarketingOptions.ShowEmail |
                MarketingOptions.ShowPostal |
                MarketingOptions.ShowSMS |
                MarketingOptions.ShowTelephone;
        }

        public Marketing GetMarketingPreferences(in Int64 userId)
        {
            if (_marketing == null)
                _marketing = new Marketing(true, true, false, false);

            return _marketing;
        }

        public bool SetMarketingPreferences(in Int64 userId, in Marketing marketing)
        {
            _marketing = marketing;
            return true;
        }

        #endregion Marketing Preferences

        #region Orders

        public List<Order> OrdersGet(in Int64 userId)
        {
            if (_orders == null)
            {
                _orders = new List<Order>()
                {
                    new Order(1, DateTime.Now.AddDays(-10), 4.99m, new CultureInfo("en-US"), ProcessStatus.Dispatched,
                        GetDeliveryAddresses(userId)[0], new List<OrderItem>()
                        {
                            new OrderItem(1, "The shining ones by David Eddings", 14.99m, 20, 1m, ItemStatus.Dispatched, DiscountType.Value, 0),
                            new OrderItem(2, "Domes of Fire by David Eddings", 12.99m, 20, 2m, ItemStatus.BackOrder, DiscountType.PercentageSubTotal, 10),
                            new OrderItem(3, "The hidden city by David Eddings", 12.99m, 20, 1m, ItemStatus.OnHold, DiscountType.PercentageTotal, 10),
                            new OrderItem(4, "Bookmark", 0.99m, 20, 1m, ItemStatus.Dispatched, DiscountType.None, 0)
                        }),

                    new Order(2, DateTime.Now.AddDays(-8), 6.99m, new CultureInfo("en-GB"), ProcessStatus.Dispatched,
                        GetDeliveryAddresses(userId)[0], new List<OrderItem>()
                        {
                            new OrderItem(5, "Mug, shiny white", 6.99m, 20, 6m, ItemStatus.Dispatched, DiscountType.Value, 15),
                            new OrderItem(6, "Dinner Plate", 7.99m, 20, 6m, ItemStatus.Dispatched, DiscountType.PercentageSubTotal, 10),
                            new OrderItem(7, "Cereal bowl", 5.99m, 20, 6m, ItemStatus.Dispatched, DiscountType.PercentageTotal, 10)
                        })
                };
            }

            return _orders;
        }

        #endregion Orders

        #region Invoices

        public List<Invoice> InvoicesGet(in Int64 userId)
        {
            if (_invoices == null)
            {
                _invoices = new List<Invoice>()
                {
                    new Invoice(123, DateTime.Now.AddDays(-10), 4.99m, new CultureInfo("en-US"), ProcessStatus.Dispatched,
                        GetDeliveryAddresses(userId)[0], new List<InvoiceItem>()
                        {
                            new InvoiceItem(1, "The shining ones by David Eddings", 14.99m, 20, 1m, ItemStatus.Dispatched, DiscountType.Value, 0),
                            new InvoiceItem(4, "Bookmark", 0.99m, 20, 1m, ItemStatus.Dispatched, DiscountType.None, 0)
                        }),

                    new Invoice(234, DateTime.Now.AddDays(-8), 6.99m, new CultureInfo("en-GB"), ProcessStatus.Dispatched,
                        GetDeliveryAddresses(userId)[0], new List<InvoiceItem>()
                        {
                            new InvoiceItem(5, "Mug, shiny white", 6.99m, 20, 6m, ItemStatus.Dispatched, DiscountType.Value, 15),
                            new InvoiceItem(6, "Dinner Plate", 7.99m, 20, 6m, ItemStatus.Dispatched, DiscountType.PercentageSubTotal, 10),
                            new InvoiceItem(7, "Cereal bowl", 5.99m, 20, 6m, ItemStatus.Dispatched, DiscountType.PercentageTotal, 10)
                        })
                };
            }

            return _invoices;
        }

        #endregion Invoices
    }
}
