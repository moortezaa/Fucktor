using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Business
{
    public class AppIdentityErrorDescriber : IdentityErrorDescriber
	{
		private readonly IStringLocalizer<AppIdentityErrorDescriber> _localizer;

		public AppIdentityErrorDescriber(IStringLocalizer<AppIdentityErrorDescriber> localizer)
		{
			_localizer = localizer;
		}

		public override IdentityError ConcurrencyFailure() => new()
		{
			Code = nameof(ConcurrencyFailure),
			Description = _localizer["Optimistic concurrency failure, object has been modified."]
		};
		public override IdentityError DefaultError() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["An unknown failure has occurred."]
		};
		public override IdentityError DuplicateEmail(string email) => new()
		{
			Code = nameof(DuplicateEmail),
			Description = string.Format(_localizer["Email {0} is already taken."], email)
		};
		public override IdentityError DuplicateRoleName(string role) => new()
		{
			Code = nameof(DuplicateRoleName),
			Description = string.Format(_localizer["Role {0} is already taken."], role)
		};
		public override IdentityError DuplicateUserName(string userName) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["UserName {0} is already taken."], userName)
		};
		public override IdentityError InvalidEmail(string? email) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["Email {0} is Invalid."], email)
		};
		public override IdentityError InvalidRoleName(string? role) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["Role {0} is Invalid."], role)
		};
		public override IdentityError InvalidToken() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["Ivalid Token."]
		};
		public override IdentityError InvalidUserName(string? userName) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["UserName {0} is Invalid."], userName)
		};
		public override IdentityError LoginAlreadyAssociated() => new()
        {
			Code = nameof(DefaultError),
			Description = _localizer["A user with this login already exists."]
		};
		public override IdentityError PasswordMismatch() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["The password and confirmation password do not match."]
		};
		public override IdentityError PasswordRequiresDigit() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["Passwords must have at least one digit ('0'-'9')."]
		};
		public override IdentityError PasswordRequiresLower() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["Passwords must have at least one lowercase ('a'-'z')."]
		};
		public override IdentityError PasswordRequiresNonAlphanumeric() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["Passwords must have at least one non alphanumeric character."]
		};
		public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["Passwords must have at least one {0}."], (char)uniqueChars)
		};
		public override IdentityError PasswordRequiresUpper() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["Passwords must have at least one uppercase ('A'-'Z')."]
		};
		public override IdentityError PasswordTooShort(int length) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["Passwords must be at least {0} characters."], length)
		};
		public override IdentityError RecoveryCodeRedemptionFailed() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["Invalid Recovery code."]
		};
		public override IdentityError UserAlreadyHasPassword() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["User already has a password set."]
		};
		public override IdentityError UserAlreadyInRole(string role) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["User already in role {0}."], role)
		};
		public override IdentityError UserLockoutNotEnabled() => new()
		{
			Code = nameof(DefaultError),
			Description = _localizer["Lockout is not enabled for this user."]
		};
		public override IdentityError UserNotInRole(string role) => new()
		{
			Code = nameof(DefaultError),
			Description = string.Format(_localizer["User is not in role {0}."], role)
		};
	}
}
