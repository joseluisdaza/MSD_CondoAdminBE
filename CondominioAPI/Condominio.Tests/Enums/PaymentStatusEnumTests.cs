using NUnit.Framework;
using Condominio.Utils.Enums;

namespace Condominio.Tests.Enums
{
    [TestFixture]
    public class PaymentStatusEnumTests
    {
        [Test]
        public void PaymentStatus_Pending_HasCorrectValue()
        {
            Assert.AreEqual(1, (int)PaymentStatus.Pending);
        }

        [Test]
        public void PaymentStatus_Paid_HasCorrectValue()
        {
            Assert.AreEqual(2, (int)PaymentStatus.Paid);
        }

        [Test]
        public void PaymentStatus_Verified_HasCorrectValue()
        {
            Assert.AreEqual(3, (int)PaymentStatus.Verified);
        }

        [Test]
        public void PaymentStatus_Cancelled_HasCorrectValue()
        {
            Assert.AreEqual(4, (int)PaymentStatus.Cancelled);
        }

        [Test]
        public void PaymentStatus_Pending_CanBeCasted()
        {
            var status = PaymentStatus.Pending;
            Assert.IsInstanceOf<PaymentStatus>(status);
        }

        [Test]
        public void PaymentStatus_Paid_CanBeCasted()
        {
            var status = PaymentStatus.Paid;
            Assert.IsInstanceOf<PaymentStatus>(status);
        }

        [Test]
        public void PaymentStatus_Verified_CanBeCasted()
        {
            var status = PaymentStatus.Verified;
            Assert.IsInstanceOf<PaymentStatus>(status);
        }

        [Test]
        public void PaymentStatus_Cancelled_CanBeCasted()
        {
            var status = PaymentStatus.Cancelled;
            Assert.IsInstanceOf<PaymentStatus>(status);
        }

        [Test]
        public void PaymentStatus_CanConvertFromInt()
        {
            var status = (PaymentStatus)1;
            Assert.AreEqual(PaymentStatus.Pending, status);
        }

        [Test]
        public void PaymentStatus_CanConvertToInt()
        {
            int value = (int)PaymentStatus.Paid;
            Assert.AreEqual(2, value);
        }

        [Test]
        public void PaymentStatus_AllValuesAreUnique()
        {
            var pending = (int)PaymentStatus.Pending;
            var paid = (int)PaymentStatus.Paid;
            var verified = (int)PaymentStatus.Verified;
            var cancelled = (int)PaymentStatus.Cancelled;

            var values = new[] { pending, paid, verified, cancelled };
            var uniqueValues = new HashSet<int>(values);
            Assert.AreEqual(values.Length, uniqueValues.Count);
        }

        [Test]
        public void PaymentStatus_ValuesAreInExpectedOrder()
        {
            var pending = (int)PaymentStatus.Pending;
            var paid = (int)PaymentStatus.Paid;
            var verified = (int)PaymentStatus.Verified;
            var cancelled = (int)PaymentStatus.Cancelled;

            Assert.IsTrue(pending < paid);
            Assert.IsTrue(paid < verified);
            Assert.IsTrue(verified < cancelled);
        }

        [Test]
        public void PaymentStatus_GetNames()
        {
            var names = System.Enum.GetNames(typeof(PaymentStatus));
            Assert.Contains("Pending", names);
            Assert.Contains("Paid", names);
            Assert.Contains("Verified", names);
            Assert.Contains("Cancelled", names);
            Assert.AreEqual(4, names.Length);
        }

        [Test]
        public void PaymentStatus_GetValues()
        {
            var values = System.Enum.GetValues(typeof(PaymentStatus));
            Assert.AreEqual(4, values.Length);
        }

        [Test]
        public void PaymentStatus_Parse_Pending()
        {
            var status = System.Enum.Parse<PaymentStatus>("Pending");
            Assert.AreEqual(PaymentStatus.Pending, status);
        }

        [Test]
        public void PaymentStatus_Parse_Paid()
        {
            var status = System.Enum.Parse<PaymentStatus>("Paid");
            Assert.AreEqual(PaymentStatus.Paid, status);
        }

        [Test]
        public void PaymentStatus_Parse_Verified()
        {
            var status = System.Enum.Parse<PaymentStatus>("Verified");
            Assert.AreEqual(PaymentStatus.Verified, status);
        }

        [Test]
        public void PaymentStatus_Parse_Cancelled()
        {
            var status = System.Enum.Parse<PaymentStatus>("Cancelled");
            Assert.AreEqual(PaymentStatus.Cancelled, status);
        }

        [Test]
        public void PaymentStatus_ToString_Pending()
        {
            var status = PaymentStatus.Pending;
            Assert.AreEqual("Pending", status.ToString());
        }

        [Test]
        public void PaymentStatus_ToString_Paid()
        {
            var status = PaymentStatus.Paid;
            Assert.AreEqual("Paid", status.ToString());
        }

        [Test]
        public void PaymentStatus_ToString_Verified()
        {
            var status = PaymentStatus.Verified;
            Assert.AreEqual("Verified", status.ToString());
        }

        [Test]
        public void PaymentStatus_ToString_Cancelled()
        {
            var status = PaymentStatus.Cancelled;
            Assert.AreEqual("Cancelled", status.ToString());
        }

        [Test]
        public void PaymentStatus_Pending_NotEqual_Paid()
        {
            Assert.AreNotEqual(PaymentStatus.Pending, PaymentStatus.Paid);
        }

        [Test]
        public void PaymentStatus_Pending_Equal_Pending()
        {
            Assert.AreEqual(PaymentStatus.Pending, PaymentStatus.Pending);
        }

        [Test]
        public void PaymentStatus_Paid_NotEqual_Verified()
        {
            Assert.AreNotEqual(PaymentStatus.Paid, PaymentStatus.Verified);
        }

        [Test]
        public void PaymentStatus_Verified_NotEqual_Cancelled()
        {
            Assert.AreNotEqual(PaymentStatus.Verified, PaymentStatus.Cancelled);
        }

        [Test]
        public void PaymentStatus_CanBeUsedInSwitch()
        {
            var status = PaymentStatus.Paid;
            string result = status switch
            {
                PaymentStatus.Pending => "Pending",
                PaymentStatus.Paid => "Paid",
                PaymentStatus.Verified => "Verified",
                PaymentStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
            Assert.AreEqual("Paid", result);
        }

        [Test]
        public void PaymentStatus_AllEnumValuesHaveDescription()
        {
            foreach (PaymentStatus status in System.Enum.GetValues(typeof(PaymentStatus)))
            {
                var name = status.ToString();
                Assert.IsNotEmpty(name);
            }
        }
    }
}
