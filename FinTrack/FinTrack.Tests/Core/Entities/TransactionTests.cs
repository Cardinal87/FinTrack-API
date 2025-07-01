

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FluentAssertions;

namespace FinTrack.Tests.Core.Entities
{
    public class TransactionTests
    {
        [Fact]
        public void CreateTransactions_SameAccountIds_ThrowInvalidTransactionException()
        {
            var guid = Guid.NewGuid();

            var createTransaction = () => new Transaction(100, guid, guid, DateTime.UtcNow);

            createTransaction.Should().Throw<InvalidTransactionException>();
        }

        [Fact]
        public void CreateTransacions_AllWithInvalidAmount_AllThrowInvalidTransactionException()
        {
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var time = DateTime.UtcNow;

            var zeroAmoun = () => new Transaction(0, fromId, toId, time);
            var negativeAmount = () => new Transaction(-100, fromId, toId, time);

            zeroAmoun.Should().Throw<InvalidTransactionException>();
            negativeAmount.Should().Throw<InvalidTransactionException>();
        }

        [Fact]
        public void CreateTransacions_WithMixedTime_AcceptTwoValid()
        {
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var amount = 100;
            var now = () => new Transaction(amount, fromId, toId, DateTime.UtcNow);
            var earlier = () => new Transaction(amount, fromId, toId, DateTime.UtcNow.AddDays(-2));

            var later = () => new Transaction(amount, fromId, toId, DateTime.UtcNow.AddDays(1));

            now.Should().NotThrow();
            earlier.Should().NotThrow();
            later.Should().Throw<InvalidTransactionException>();

        }


        [Fact]
        public void CreateTransaction_ValidData_Success()
        {
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var time = DateTime.UtcNow;
            var amount = 100;
            var validTransaction =  new Transaction(amount, fromId, toId, time);

            validTransaction.Amount.Should().Be(amount);
            validTransaction.Date.Should().Be(time);
            validTransaction.FromAccountId.Should().Be(fromId);
            validTransaction.ToAccountId.Should().Be(toId);
        }
    }
}
