

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FluentAssertions;

namespace FinTrack.Tests.Core.Entities
{
    public class AccountTests
    {
        [Fact]
        public void TopUpBalance_WithMixedValues_AcceptsSingleValid() 
        {
            var account = new Account(Guid.NewGuid());

            var valid_top_up = () => account.TopUp(500);

            var top_up_zero_amount = () => account.TopUp(0);
            var top_up_negative_amout = () => account.TopUp(-100);

            valid_top_up.Should().NotThrow();
            top_up_negative_amout.Should().Throw<ArgumentException>();
            top_up_zero_amount.Should().Throw<ArgumentException>();
            account.Balance.Should().Be(500);
        }

        [Fact]
        public void DebitBalance_WithMixedValues_AcceptsSingleValid()
        {
            var account = new Account(Guid.NewGuid());
            account.TopUp(1000);

            var valid_debit = () => account.Debit(500);

            var debit_greater_than_current_balance = () => account.Debit(2000);
            var debit_zero = () => account.Debit(0);
            var debit_negative = () => account.Debit(-500);

            valid_debit.Should().NotThrow();
            debit_greater_than_current_balance.Should().Throw<InsufficientFundsException>();
            debit_negative.Should().Throw<ArgumentException>();
            debit_zero.Should().Throw<ArgumentException>();
            account.Balance.Should().Be(500);
        }

        [Fact]
        public void AddOutgoingTransaction_WithMixedTransactions_AcceptsSingleValid()
        {
            var account = new Account(Guid.NewGuid());

            var valid_transaction = new Transaction(100, account.Id, Guid.NewGuid(), DateTime.UtcNow);
            var invalid_transaction = new Transaction(100, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

            var add_valid_transaction = () => account.AddOutgoingTransaction(valid_transaction);
            var add_invalid_transaction = () => account.AddOutgoingTransaction(invalid_transaction);

            add_valid_transaction.Should().NotThrow();
            add_invalid_transaction.Should().Throw<TransactionOwnershipException>();
            account.OutgoingTransactions.Should().HaveCount(1);
            account.OutgoingTransactions.Should().Contain(valid_transaction);
        }

        [Fact]
        public void AddIncomingTransaction_WithMixedTransactions_AcceptsSingleValid()
        {
            var account = new Account(Guid.NewGuid());

            var valid_transaction = new Transaction(100, Guid.NewGuid(), account.Id, DateTime.UtcNow);
            var invalid_transaction = new Transaction(100, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

            var add_valid_transaction = () => account.AddIncomingTransaction(valid_transaction);
            var add_invalid_transaction = () => account.AddIncomingTransaction(invalid_transaction);

            add_valid_transaction.Should().NotThrow();
            add_invalid_transaction.Should().Throw<TransactionOwnershipException>();
            account.IncomingTransactions.Should().HaveCount(1);
            account.IncomingTransactions.Should().Contain(valid_transaction);
        }
        [Fact]
        public void CheckInitialData_ValidAccount_Success()
        {
            var userId = Guid.NewGuid();

            var account = new Account(userId);

            account.Balance.Should().Be(0);
            account.UserId.Should().Be(userId);
            account.OutgoingTransactions.Should().HaveCount(0);
            account.IncomingTransactions.Should().HaveCount(0);
        }
    }
}
