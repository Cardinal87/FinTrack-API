
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FluentAssertions;
using FinTrack.API.Core.Common;

namespace FinTrack.Tests.Core.Entities
{
    public class UserTests
    {

        [Fact]
        public void EmailValidation_WithMixedEmails_AcceptsSingleValid()
        {
            var hash = "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";
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
            var invalidEmail9 = () => new User("", phone, name, hash);
            var invalidEmail10 = () => new User(" ", phone, name, hash);


            validEmail.Should().NotThrow();
            invalidEmail1.Should().Throw<ArgumentException>();
            invalidEmail2.Should().Throw<ArgumentException>();
            invalidEmail3.Should().Throw<ArgumentException>();
            invalidEmail4.Should().Throw<ArgumentException>();
            invalidEmail5.Should().Throw<ArgumentException>();
            invalidEmail6.Should().Throw<ArgumentException>();
            invalidEmail7.Should().Throw<ArgumentException>();
            invalidEmail8.Should().Throw<ArgumentException>();
            invalidEmail9.Should().Throw<ArgumentException>();
            invalidEmail10.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void PhoneValidation_WithMixedPhone_AcceptsSingleValid()
        {
            var hash = "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";
            var name = "test_user";
            var email = "test@email.com";

            var validPhone = () => new User(email, "+79123456789", name, hash);

            var invalidPhone1 = () => new User(email, "+7", name, hash);
            var invalidPhone2 = () => new User(email, "+7912 345 6789", name, hash);
            var invalidPhone3 = () => new User(email, "+7abc1234567", name, hash);
            var invalidPhone4 = () => new User(email, "", name, hash);

            validPhone.Should().NotThrow();
            invalidPhone1.Should().Throw<ArgumentException>();
            invalidPhone2.Should().Throw<ArgumentException>();
            invalidPhone3.Should().Throw<ArgumentException>();
            invalidPhone4.Should().Throw<ArgumentException>();

        }


        [Fact]
        public void HashValidation_WithMixedHash_AcceptsOnlyValid()
        {
            var name = "test_user";
            var email = "test@email.com";
            var phone = "+79998887766";


            var validHash1 = () => new User(email,
                                            phone,
                                            name,
                                            "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=");
            var validHash2 = () => new User(email,
                                            phone,
                                            name,
                                            "MD5.50.zrIHcuDJ0kDHXrJrDjer7g==.X03MO1qnZdYdgyfeuILPmQ==");

            var invalidHash1 = () => new User(email, phone, name, "X03MO1qnZdYdgyfeuILPmQ==");
            var invalidHash2 = () => new User(email, phone, name, "zrIHcuDJ0kDHXrJrDjer7g==.X03MO1qnZdYdgyfeuILPmQ==");
            var invalidHash3 = () => new User(email, phone, name, ".50.zrIHcuDJ0kDHXrJrDjer7g==.X03MO1qnZdYdgyfeuILPmQ==");
            var invalidHash4 = () => new User(email, phone, name, "MD5$.50.zrIHcuDJ0kDHXrJrDjer7#==.X03MO1qnZdYdgyfeuILPm@==");
            var invalidHash5 = () => new User(email, phone, name, "");

            validHash1.Should().NotThrow();
            validHash2.Should().NotThrow();

            invalidHash1.Should().Throw<ArgumentException>();
            invalidHash2.Should().Throw<ArgumentException>();
            invalidHash3.Should().Throw<ArgumentException>();
            invalidHash4.Should().Throw<ArgumentException>();
            invalidHash5.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NameValidation_WithMixedNames_AcceptsSingleValid()
        {
            var hash = "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";
            var email = "test@email.com";
            var phone = "+79998887766";

            var validName = () => new User(email, phone, "username", hash);

            var invalidName1 = () => new User(email, phone, "", hash);
            var invalidName2 = () => new User(email, phone, " ", hash);

            validName.Should().NotThrow();
            invalidName1.Should().Throw<ArgumentException>();
            invalidName2.Should().Throw<ArgumentException>();

        }

        [Fact]
        public void AddAccount_WithMixedAccounts_AcceptsSingleValid()
        {
            var user = new User("test@email.com",
                                "+79998887766",
                                "test_user",
                                "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=");

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
                                "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=");

            var valid_account = new Account(user.Id);
            user.AddAccount(valid_account);

            var delete_existing = () => user.DeleteAccount(valid_account.Id);
            var invalid_delete = () => user.DeleteAccount(Guid.NewGuid());

            delete_existing.Should().NotThrow();
            invalid_delete.Should().Throw<KeyNotFoundException>();
            user.Accounts.Should().HaveCount(0);
        }

        [Fact]
        public void RoleValidation_WithMixedValues_AcceptsOnlyValid()
        {
            var user = new User("test@email.com",
                                "+79998887766",
                                "test_user",
                                "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=");

            var addValidRole1 = () => user.AssignRole(UserRoles.User);
            var addValidRole2 = () => user.AssignRole(UserRoles.Admin);

            var addInvalidRole1 = () => user.AssignRole("IncorrectRole");
            var addInvalidRole2 = () => user.AssignRole("");
            var addInvalidRole3 = () => user.AssignRole(" ");

            addValidRole1.Should().NotThrow();
            addValidRole2.Should().NotThrow();
            addInvalidRole1.Should().Throw<DomainException>();
            addInvalidRole2.Should().Throw<DomainException>();
            addInvalidRole3.Should().Throw<DomainException>();

        }
    }
}
