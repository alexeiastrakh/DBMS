using GraphQL.Types;
using DatabaseLayer.Models;
using System;

namespace WebAPI
{
    public class YourCustomTypeGraphType : EnumerationGraphType
    {
        public YourCustomTypeGraphType()
        {
            Name = "Type";
            Description = "The type of the column in the database.";

            // Добавьте значения перечисления
            foreach (var type in Enum.GetValues(typeof(DatabaseLayer.Models.Type)))
            {
                Add(new EnumValueDefinition(type.ToString(), type));
            }
        }
    }
}