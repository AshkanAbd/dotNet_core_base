using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SoftDeletes.ModelTools;

namespace dotNet_base.Components.Extensions
{
    public class DbContextExtension : SoftDeletes.Core.DbContext
    {
        protected DbContextExtension()
        {
        }

        public DbContextExtension(DbContextOptions options) : base(options)
        {
        }

        public void DetachEntities(List<object> shouldDetach = null, List<object> except = null)
        {
            var states = new List<EntityState> {
                EntityState.Added,
                EntityState.Modified,
                EntityState.Deleted,
                EntityState.Unchanged,
            };
            var entitiesList = ChangeTracker.Entries()
                .Where(x => {
                    var result = states.Contains(x.State);
                    if (shouldDetach != null) {
                        result &= shouldDetach.Contains(x.Entity);
                    }

                    if (except != null) {
                        result &= !except.Contains(x.Entity);
                    }

                    return result;
                }).ToList();
            entitiesList.ForEach(x => x.State = EntityState.Detached);
        }
    }
}