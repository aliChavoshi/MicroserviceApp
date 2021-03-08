using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories.Base;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>,IOrderRepository
    {
        public OrderRepository(OrderContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrderByUserName(string username)
        {
            return await Context.Order.Where(x => x.UserName == username).ToListAsync();
        }
    }
}