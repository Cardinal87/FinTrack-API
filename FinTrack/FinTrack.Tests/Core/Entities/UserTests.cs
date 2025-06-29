
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FluentAssertions;
using System.Xml.Linq;

namespace FinTrack.Tests.Core.Entities
{
    public class UserTests
    {

        [Fact]
        public void EmailValidation_WithMixedEmails_AcceptsSingleValid()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var name = "test_user";
            var phone = "+79998887766";

            var validEmail = () => new User("user.name+tag@example.co.uk", phone, name, hash);

            var invalidEmail1 = () => new User("@example.com", phone, name, hash);
            var invalidEmail2 = () => new User("user@.com", phone, name, hash);
            var invalidEmail3 = () => new User("user@exa mple.com", phone, name, hash);
            var invalidEmail4 = () => new User("user@example..com", phone, name, hash);
            var invalidEmail5 = () => new User("user@example", phone, name, hash);
            var invalidEmail6 = () => new User("user name@example.com", phone, name, hash);
            var invalidEmail7 = () => new User("user@example_com.com", phone, name, hash);
            var invalidEmail8 = () => new User("user@example.com.", phone, name, hash);


            validEmail.Should().NotThrow();
            invalidEmail1.Should().Throw<ArgumentException>();
            invalidEmail2.Should().Throw<ArgumentException>();
            invalidEmail3.Should().Throw<ArgumentException>();
            invalidEmail4.Should().Throw<ArgumentException>();
            invalidEmail5.Should().Throw<ArgumentException>();
            invalidEmail6.Should().Throw<ArgumentException>();
            invalidEmail7.Should().Throw<ArgumentException>();
            invalidEmail8.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void PhoneValidation_WithMixedPhone_AcceptsSingleValid()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var name = "test_user";
            var email = "test@email.com";

            var validPhone = () => new User(email, "+79123456789", name, hash);

            var invalidPhone1 = () => new User(email, "+7", name, hash);
            var invalidPhone2 = () => new User(email, "+7912 345 6789", name, hash);
            var invalidPhone3 = () => new User(email, "+7abc1234567", name, hash);

            validPhone.Should().NotThrow();
            invalidPhone1.Should().Throw<ArgumentException>();
            invalidPhone2.Should().Throw<ArgumentException>();
            invalidPhone3.Should().Throw<ArgumentException>();

        }


        [Fact]
        public void HashValidation_WithMixedHash_AcceptsSingleValid()
        {
            var name = "test_user";
            var email = "test@email.com";
            var phone = "+79998887766";


            var validHash = () => new User(email, phone, name, "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01");

            var invalidHash1 = () => new User(email, phone, name, "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb0");
            var invalidHash2 = () => new User(email, phone, name, "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb011");
            var invalidHash3 = () => new User(email, phone, name, "");

            validHash.Should().NotThrow();
            invalidHash1.Should().Throw<ArgumentException>();
            invalidHash2.Should().Throw<ArgumentException>();
            invalidHash3.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NameValidation_WithMixedNames_AcceptsSingleValid()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var phone = "+79998887766";

            var validName = () => new User(email, phone, "username", hash);

            var invalidName = () => new User(email, phone, "", hash);

            validName.Should().NotThrow();
            invalidName.Should().Throw<ArgumentException>();

        }

        [Fact]
        public void AddAccount_WithMixedAccounts_AcceptsSingleValid()
        {
            var user = new User("test@email.com",
                                "+79998887766",
                                "test_user",
                                "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01");

            var valid_account = new Account(user.Id);
            var invalid_account = new Account(Guid.NewGuid());

            var add_valid_account = () => user.AddAccount(valid_account);
            var add_invalid_account = () => user.AddAccount(invalid_account);

            add_valid_account.Should().NotThrow();
            add_invalid_account.Should().Throw<AccountOwnershipException>();
            user.Accounts.Should().HaveCount(1);
            user.Accounts.Should().Contain(valid_account);



        }

        [Fact]
        public void DeleteAccout_WithMixesGuid_AcceptSingleValid()
        {
            var user = new User("test@email.com",
                                "+79998887766",
                                "test_user",
                                "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01");

            var valid_account = new Account(user.Id);
            user.AddAccount(valid_account);

            var delete_existing = () => user.DeleteAccount(valid_account.Id);
            var invalid_delete = () => user.DeleteAccount(Guid.NewGuid());

            delete_existing.Should().NotThrow();
            invalid_delete.Should().Throw<KeyNotFoundException>();
            user.Accounts.Should().HaveCount(0);
        }
    }
}
