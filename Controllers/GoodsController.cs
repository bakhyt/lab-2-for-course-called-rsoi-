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

namespace MvcApplication1.Controllers
{
    public class GoodsController : ApiController
    {
        UsersContext context = new UsersContext();

        public Good Get(int id)
        {
            return context.Goods.Where(x => x.ID == id).FirstOrDefault();
        }
        public ICollection<Good> Get(int page, int size)
        {
            return context.Goods.OrderBy(x => x.ID).Skip(page)
                  .Take(size).ToList();
        }
        /*
                public HttpResponseMessage Put(Topic model)
                {
                    if (String.IsNullOrEmpty(model.Title))
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);


                    return new HttpResponseMessage(HttpStatusCode.Created);
                }

                public string Delete(int id)
                {
                    return String.Format("Топик {0} удален!", id);
                }*/
    }
}