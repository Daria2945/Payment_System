using System;
using System.Security.Cryptography;
using System.Text;

namespace Payment_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int orderId = 123;
            int orderAmount = 12000;
            Order newOrder = new Order(orderId, orderAmount);

            MD5HashSystem md5HashSystem = new MD5HashSystem();
            SHA1HashSystem sha1HashSystem = new SHA1HashSystem();

            PaymentSystem1 paymentSystem1 = new PaymentSystem1(md5HashSystem);
            PaymentSystem2 paymentSystem2 = new PaymentSystem2(md5HashSystem);
            PaymentSystem3 paymentSystem3 = new PaymentSystem3(sha1HashSystem);

            Console.WriteLine(paymentSystem1.GetPayingLink(newOrder));
            Console.WriteLine(paymentSystem2.GetPayingLink(newOrder));
            Console.WriteLine(paymentSystem3.GetPayingLink(newOrder));
        }
    }

    public class Order
    {
        public Order(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }

        public int Id { get; private set; }

        public int Amount { get; private set; }
    }

    public interface IPaymentSystem
    {
        string GetPayingLink(Order order);
    }

    public interface IHashSystem
    {
        string GetHashForValue(string value);
    }

    public class MD5HashSystem : IHashSystem
    {
        public string GetHashForValue(string value) =>
            Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(value)));
    }

    public class SHA1HashSystem : IHashSystem
    {
        public string GetHashForValue(string value) =>
            Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(value)));
    }

    public class PaymentSystem1 : IPaymentSystem
    {
        private readonly IHashSystem _hashSystem;

        private readonly string _urlLinkStart = "pay.system1.ru/order?";
        private readonly string _amountText = "amount=";
        private readonly string _currencyText = "RUB";
        private readonly string _hashText = "hash=";
        private readonly char _ampersand = '&';

        public PaymentSystem1(IHashSystem hashSystem)
        {
            _hashSystem = hashSystem ?? throw new ArgumentNullException(nameof(hashSystem));
        }

        public string GetPayingLink(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var hashValue = _hashSystem.GetHashForValue(order.Id.ToString());

            return $"{_urlLinkStart}{_amountText}{order.Amount}{_currencyText}{_ampersand}{_hashText}{hashValue}";
        }
    }

    public class PaymentSystem2 : IPaymentSystem
    {
        private readonly IHashSystem _hashSystem;

        private readonly string _urlLinkStart = "order.system2.ru/pay?";
        private readonly string _hashText = "hash=";

        public PaymentSystem2(IHashSystem hashSystem)
        {
            _hashSystem = hashSystem ?? throw new ArgumentNullException(nameof(hashSystem));
        }

        public string GetPayingLink(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var hashValue = _hashSystem.GetHashForValue(order.Id.ToString() + order.Amount);

            return $"{_urlLinkStart}{_hashText}{hashValue}";
        }
    }

    public class PaymentSystem3 : IPaymentSystem
    {
        private readonly IHashSystem _hashSystem;

        private readonly string _urlLinkStart = "system3.com/pay?";
        private readonly string _amountText = "amount=";
        private readonly string _currencyText = "currency=RUB";
        private readonly string _hashText = "hash=";
        private readonly string _secretKeyToSystem = "secret_key";
        private readonly char _ampersand = '&';

        public PaymentSystem3(IHashSystem hashSystem)
        {
            _hashSystem = hashSystem ?? throw new ArgumentNullException(nameof(hashSystem));
        }

        public string GetPayingLink(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var hashValue = _hashSystem.GetHashForValue(order.Amount.ToString() + order.Id + _secretKeyToSystem);

            return $"{_urlLinkStart}{_amountText}{order.Amount}{_ampersand}{_currencyText}{_ampersand}{_hashText}{hashValue}";
        }
    }
}