using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.Services;

namespace DeliveryApp.Infrastructure.Implementation.Services
{
    public sealed class SqlServerPublicCodeGenerator : IPublicCodeGenerator // مسؤول عن توليد الأكواد بأستخدام SQL Server Sequences
    {
        private const string UserSequence = "shared.user_public_seq";
        private const string OrderSequence = "shared.order_public_seq";
        private const string MerchantSequence = "shared.merchant_public_seq";

        private readonly ApplicationDbContext _context;

        public SqlServerPublicCodeGenerator(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<PublicCode> GenerateUserCodeAsync(CancellationToken ct = default)
            => GenerateAsync(UserSequence, "U", ct);

        public Task<PublicCode> GenerateOrderCodeAsync(CancellationToken ct = default)
            => GenerateAsync(OrderSequence, "ORD", ct);

        public Task<PublicCode> GenerateMerchantCodeAsync(CancellationToken ct = default)
            => GenerateAsync(MerchantSequence, "MER", ct);

        private async Task<PublicCode> GenerateAsync(string sequenceName, string prefix, CancellationToken ct) // مولد الكود
        {
            // الأتصال مع القاعدة
            var connection = _context.Database.GetDbConnection();
            var openedHere = false;

            // إذا مو متصل بيفتح الأتصال
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(ct);
                openedHere = true;
            }

            // توليد الكود
            try
            { 
                using var command = CreateSequenceCommand(connection, sequenceName);

                var currentTransaction = _context.Database.CurrentTransaction;
                if (currentTransaction is not null)
                {
                    command.Transaction = currentTransaction.GetDbTransaction();
                }

                var result = await command.ExecuteScalarAsync(ct);

                if (result is null || result == DBNull.Value)
                {
                    throw new InvalidOperationException($"Failed to get next value from sequence '{sequenceName}'.");
                }

                var number = Convert.ToInt64(result);

                return PublicCode.Create(prefix, number);
            }
            finally // عند الأنتهاء شو ما صار سكر الأتصال
            {
                if (openedHere)
                {
                    await connection.CloseAsync();
                }
            }
        }

        // تعليمات SQL  
        // لإنشاء Squence
        private static DbCommand CreateSequenceCommand(DbConnection connection, string sequenceName) 
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT NEXT VALUE FOR {sequenceName};";
            return command;
        }
    }
}