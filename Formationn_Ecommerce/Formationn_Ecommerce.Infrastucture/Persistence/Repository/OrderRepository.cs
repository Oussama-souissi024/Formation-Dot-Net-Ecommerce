using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Core.Entities.Cart;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;
using Formationn_Ecommerce.Entities.Orders;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.TeleTrust;

namespace Formationn_Ecommerce.Infrastucture.Persistence.Repository
{
    public class OrderRepository : Repository<OrderHeader>, IOrderRepository
    {

        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
      
        public async Task<OrderHeader> AddOrderHeaderAsync(OrderHeader orderHeader)
        {
            await _context.OrderHeaders.AddAsync(orderHeader);
            await _context.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<IEnumerable<OrderDetails>> AddOrderDetailsAsync(IEnumerable<OrderDetails>  orderDetailsList)
        {
            await _context.OrderDetails.AddRangeAsync(orderDetailsList);
            await _context.SaveChangesAsync();
            return orderDetailsList;
        }

        public  IEnumerable<OrderHeader?> GetAllAsync()
        {
            return  _context.OrderHeaders.ToList();
        }

        public IEnumerable<OrderHeader?> GetAllByUserIdAsync(string UserId)
        {
            // Amélioration : appliquer le filtre AVANT d'appeler ToList() et inclure les détails associés
            return _context.OrderHeaders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.UserId == UserId)
                .ToList();
        }

        public async Task<OrderHeader?> GetByIdAsync(Guid orderHeaderId)
        {
            return await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderHeaderId);
        }

        public async Task<OrderHeader?> GetByIdWithDetailsAsync(Guid orderHeaderId)
        {
            // Utiliser Include pour charger explicitement les OrderDetails et leurs produits associés
            return await _context.OrderHeaders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderHeaderId);
        }

        public async Task<bool?> UpdateStatusAsync(Guid orderHeaderId, string newStatus)
        {
            var orderFromDb = await GetByIdAsync(orderHeaderId);
            try
            {
                orderFromDb.Status = newStatus;
                _context.OrderHeaders.Update(orderFromDb);
                await SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<OrderHeader?> UpdateOrderHeaderAsync(OrderHeader orderHeader)
        {
            try
            {
                _context.OrderHeaders.Update(orderHeader);
                await SaveChangesAsync();
                return orderHeader;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Task<OrderDetails> AddAsync(OrderDetails entity)
        {
            throw new NotImplementedException();
        }

        Task<OrderDetails> IRepository<OrderDetails>.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<OrderDetails>> IRepository<OrderDetails>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task Update(OrderDetails entity)
        {
            throw new NotImplementedException();
        }

        public Task Remove(OrderDetails entity)
        {
            throw new NotImplementedException();
        }
    }
}
