﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nexmo.Api.Test.Integration
{
    [TestClass]
    public class AccountTest
	{
        [TestMethod]
		public void should_get_account_balance()
        {
            var balance = Account.GetBalance();
            Assert.AreEqual(.43d, balance.value);
        }

        [TestMethod]
		public void should_get_pricing()
        {
            var pricing = Account.GetPricing("US");
            var verizon = pricing.networks.Single(n => n.code == "310004");
            Assert.AreEqual("Verizon Wireless", verizon.network);
        }

        [TestMethod]
        public void should_get_prefix_pricing()
        {
            var pricing = Account.GetPrefixPricing("1","sms");
            Assert.AreEqual("US", pricing.country);
        }

        [TestMethod]
        public void should_set_settings()
        {
            // TODO: password complexity requirements:
            //{"max-outbound-request":0,"max-inbound-request":0,"error-code":"420","error-code-label":"8 char"}
            //{"max-outbound-request":0,"max-inbound-request":0,"error-code":"420","error-code-label":"alphanumeric"}
            //{"max-outbound-request":0,"max-inbound-request":0,"error-code":"420","error-code-label":"Please use at least one digit."}
            //{"max-outbound-request":0,"max-inbound-request":0,"error-code":"420","error-code-label":"Please use at least one upper case character."}

            var response = Account.SetSettings("newSecret1", "https://mo.test.com", "https://dr.test.com");
            Assert.AreEqual("newSecret1", response.apiSecret);
            Assert.AreEqual("https://mo.test.com", response.moCallbackUrl);
            Assert.AreEqual("https://dr.test.com", response.drCallbackUrl);

            // callback urls are checked for a response
        }

        [TestMethod]
        public void should_get_numbers()
        {
            var response = Account.GetNumbers();
            Assert.AreEqual(1, response.count);
            Assert.AreEqual(Configuration.Instance.Settings["nexmo_number"], response.numbers[0].msisdn);
        }
    }
}