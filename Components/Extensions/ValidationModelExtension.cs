using System.Collections.Generic;

namespace dotNet_base.Components.Extensions
{
    public class ValidationModelExtension
    {
        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var property in GetType().GetProperties()) {
                dictionary[property.Name] = property.GetValue(this);
            }

            return dictionary;
        }
    }
}