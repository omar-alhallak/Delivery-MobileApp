using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities;

namespace DeliveryApp.Application.Abstractions.Persistence
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByIDAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Driver driver, CancellationToken ct = default);
    }
}