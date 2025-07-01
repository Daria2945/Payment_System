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

            PaymentSystem1 paymentSystem1 = new PaymentSystem1();
            PaymentSystem2 paymentSystem2 = new PaymentSystem2();
            PaymentSystem3 paymentSystem3 = new PaymentSystem3();

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
        string GetHashForValue(int value);
    }

    public class PaymentSystem1 : IPaymentSystem, IHashSystem
    {
        private readonly string _urlLinkStart = "pay.system1.ru/order?";
        private readonly string _amountText = "amount=";
        private readonly string _currencyText = "RUB";
        private readonly string _hashText = "hash=";
        private readonly char _ampersand = '&';

        public string GetHashForValue(int value) =>
            Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(value.ToString())));

        public string GetPayingLink(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var hashId = GetHashForValue(order.Id);

            return $"{_urlLinkStart}{_amountText}{order.Amount}{_currencyText}{_ampersand}{_hashText}{hashId}";
        }
    }

    public class PaymentSystem2 : IPaymentSystem, IHashSystem
    {
        private readonly string _urlLinkStart = "order.system2.ru/pay?";
        private readonly string _hashText = "hash=";

        public string GetHashForValue(int value) =>
            Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(value.ToString())));

        public string GetPayingLink(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var hashId = GetHashForValue(order.Id);

            return $"{_urlLinkStart}{_hashText}{hashId} {order.Amount}";
        }
    }

    public class PaymentSystem3 : IPaymentSystem, IHashSystem
    {
        private readonly string _urlLinkStart = "system3.com/pay?";
        private readonly string _amountText = "amount=";
        private readonly string _currencyText = "currency=RUB";
        private readonly string _hashText = "hash=";
        private readonly string _secretKeyToSystem = "secret_key";
        private readonly char _ampersand = '&';

        public string GetHashForValue(int value) =>
            Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(value.ToString())));

        public string GetPayingLink(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var hashAmount = GetHashForValue(order.Amount);

            return $"{_urlLinkStart}{_amountText}{order.Amount}{_ampersand}{_currencyText}{_ampersand}{_hashText}{hashAmount} {order.Id} {_secretKeyToSystem}";
        }
    }
}