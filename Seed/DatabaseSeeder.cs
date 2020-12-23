using System;
using dotNet_base.Components.Extensions;

namespace dotNet_base.Seed
{
    public class DatabaseSeeder : SeederExtension
    {
        protected override object[] Run()
        {
            return null;
        }

        protected override void RunWithDbContext<T>(T dbContext)
        {
        }

        protected override Type GetModelType()
        {
            return GetType();
        }
    }
}