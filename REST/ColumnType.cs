using GraphQL;
using GraphQL.Types;
using DatabaseLayer.Models;
using DatabaseLayer.Repositories;
using DatabaseLayer.IRepositories;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI
{
    public class ColumnType : ObjectGraphType<Column>
    {
        public ColumnType()
        {
            Field(x => x.Id).Description("The ID of the Column.");
            Field(x => x.Name).Description("The name of the Column.");
            // Добавьте другие поля, как необходимо
        }
    }

    public class ColumnQuery : ObjectGraphType
    {
        public ColumnQuery(IColumnRepository columnRepository)
        {
            Field<ListGraphType<ColumnType>>(
                "columns",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "databaseName" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "tableName" }
                ),
                resolve: context =>
                {
                    var databaseName = context.GetArgument<string>("databaseName");
                    var tableName = context.GetArgument<string>("tableName");
                    return columnRepository.FindAllTableColumns(databaseName, tableName);
                }
            );

            // Добавьте другие поля запроса, как необходимо
        }
    }

    public class ColumnMutation : ObjectGraphType
    {
        public ColumnMutation(IColumnRepository columnRepository)
        {
            Field<ColumnType>(
                "addColumn",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "databaseName" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "tableName" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "columnName" },
                    new QueryArgument<NonNullGraphType<YourCustomTypeGraphType>> { Name = "columnType" }
                ),
                resolve: context =>
                {
                    var databaseName = context.GetArgument<string>("databaseName");
                    var tableName = context.GetArgument<string>("tableName");
                    var columnName = context.GetArgument<string>("columnName");
                    var columnType = context.GetArgument<DatabaseLayer.Models.Type>("columnType");
                    var added = columnRepository.AddColumn(databaseName, tableName, columnName, columnType);
                    if (!added) context.Errors.Add(new ExecutionError("Failed to add the column."));
                    return added ? columnRepository.FindColumnByName(databaseName, tableName, columnName) : null;
                }
            );

            // Добавьте другие мутации, как необходимо
        }
    }



    public class ColumnSchema : Schema
    {
        public ColumnSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<ColumnQuery>();
            Mutation = provider.GetRequiredService<ColumnMutation>();
        }
    }
}