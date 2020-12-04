using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace dotNet_base.Components.Extensions
{
    public interface IPaginationExtension
    {
        protected void NormalizePagination(ref int page, ref int pageSize)
        {
            if (page <= 0) {
                page = 1;
            }

            if (pageSize == 0 || pageSize < -1) {
                pageSize = 10;
            }
        }

        public async Task<dynamic> UsePagination(IQueryable<object> query, int page, int pageSize)
        {
            NormalizePagination(ref page, ref pageSize);
            var count = query.Count();
            if (pageSize != -1) {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }
            else {
                pageSize = count == 0 ? 1 : count;
            }

            return new {
                list = await query.ToListAsync(),
                page_number = (int) Math.Ceiling(count / (double) pageSize),
                total = count
            };
        }
    }
}