using System.Data;
using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Application.interfaces;

public sealed class SqlServerPublicCodeGenerator : IPublicCodeGenerator // يأنشئ الكود من سيكونسي السيرفر
{
    private readonly DbContext Context;

    public SqlServerPublicCodeGenerator(DbContext context)
    {
        Context = context;
    }

    public Task<PublicCode> GenerateUserCodeAsync(CancellationToken ct = default)
        => GenerateAsync("user_public_seq", "U", ct);

    public Task<PublicCode> GenerateOrderCodeAsync(CancellationToken ct = default)
        => GenerateAsync("order_public_seq", "ORD", ct);

    public Task<PublicCode> GenerateMerchantCodeAsync(CancellationToken ct = default)
        => GenerateAsync("merchant_public_seq", "MER", ct);

    private async Task<PublicCode> GenerateAsync(string sequenceName, string prefix, CancellationToken ct)
    {
        var connection = Context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT NEXT VALUE FOR {sequenceName};";

        var result = await command.ExecuteScalarAsync(ct);

        var number = Convert.ToInt64(result);

        return PublicCode.Create(prefix, number);
    }
}