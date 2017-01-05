using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using ApiControllerTutorial.Models;
using System.Collections.ObjectModel;
using System.Net;
using MvcApplication1.Models;
using AttributeRouting.Web.Mvc;
using AttributeRouting;
using ModelLib;

namespace MvcApplication1.Controllers
{
    public class GoodsController : ApiController
    {
        UsersContext context = new UsersContext();

        public string Get(int id)
        {
            MyOAuth.CheckAccessToken();

            Good good = context.Goods.Where(x => x.ID == id).FirstOrDefault();
            if (good != null)
            {
                return string.Format("Название: {0}; Цена: ", good.Title, good.Cost);
            }
            return "Нет данных";
        }

        public ICollection<Good> Get(int page, int size)
        {
            return context.Goods.OrderBy(x => x.ID).Skip((page - 1) * size)
                  .Take(size).ToList();
        }
    }
    public class DeliveriesController : ApiController
    {
        public ICollection<Delivery> Get(int page, int size)
        {
            using (UsersContext context = new UsersContext())
            {
                return context.Deliveries.OrderBy(x => x.ID).Skip((page - 1) * size)
                      .Take(size).ToList();
            }
        }
    }
    public class CustomersController : ApiController
    {
        public ICollection<Customer> Get(int page, int size)
        {
            MyOAuth.CheckAccessToken();
            using (UsersContext context = new UsersContext())
            {
                return context.Customers.OrderBy(x => x.ID).Skip((page - 1) * size)
                      .Take(size).ToList();
            }
        }
    }
    public class OrdersController : ApiController
    {
        UsersContext context = new UsersContext();

        public ICollection<Order> Get(int page, int size)
        {
            MyOAuth.CheckAccessToken();
            return context.Orders.OrderBy(x => x.ID).Skip((page - 1) * size)
                  .Take(size).ToList();
        }

        public int Post()
        {
            MyOAuth.CheckAccessToken();

            int maxid = 0;
            if (context.Orders.Count() > 0)
            {
                maxid = context.Orders.Max(p => p == null ? 0 : p.ID);
            }
            Order order = new Order()
            {
                ID = Convert.ToInt32(maxid + 1)
            };
            context.Orders.Add(order);
            context.SaveChanges();
            return order.ID;
        }
        //[Route("~/api/orders/{orderId}/good/{goodId}/customer/{customerId}/delivery/{deliveryId}")]
        public Order Post(int orderId, int goodId, int customerId, int deliveryId)
        {
            MyOAuth.CheckAccessToken();

            Order order = context.Orders.Where(x => x.ID == orderId).First();
            if (order != null)
            {
                order.GoodId = goodId;
                order.CustomerId = customerId;
                order.DeliveryId = deliveryId;
            }
            context.SaveChanges();
            return order;
        }
        public string Get(int id)
        {
            string res = "";

            var myorder = (from order in context.Orders
                           from good in context.Goods
                           from customer in context.Customers
                           from delivery in context.Deliveries
                           where order.GoodId == good.ID && order.CustomerId == customer.ID && order.DeliveryId == delivery.ID
                           && order.ID == id
                           select new
                          {
                              ID = order.ID,
                              goodName = good.Title,
                              customerName = customer.Title,
                              deliveryName = delivery.Title
                          }).FirstOrDefault();
            res = string.Format("Номер заказа: {0}; Товар: {1}; Покупатель: {2}; Доставка: {3}", myorder.ID, myorder.goodName
                , myorder.customerName, myorder.deliveryName);

            return res;
        }
        [HttpDelete]
        public string Delete(int orderId)
        {
            MyOAuth.CheckAccessToken();

            Order order = context.Orders.Where(x => x.ID == orderId).First();
            context.Orders.Remove(order);
            context.SaveChanges();
            return "Удалено - " + orderId;
        }
    }
}