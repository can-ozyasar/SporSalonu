using Microsoft.AspNetCore.Identity;

namespace OZ_SporSalonu.Services
{
    public class TurkceIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() 
            => new IdentityError { Code = nameof(DefaultError), Description = "Bir hata oluştu." };

        public override IdentityError ConcurrencyFailure() 
            => new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Veri başka bir kullanıcı tarafından değiştirildi." };

        public override IdentityError PasswordTooShort(int length) 
            => new IdentityError { Code = nameof(PasswordTooShort), Description = $"Şifre en az {length} karakter olmalıdır." };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) 
            => new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = $"Şifre en az {uniqueChars} farklı karakter içermelidir." };

        public override IdentityError PasswordRequiresNonAlphanumeric() 
            => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Şifre en az bir sembol (!, *, - vs.) içermelidir." };

        public override IdentityError PasswordRequiresDigit() 
            => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Şifre en az bir rakam (0-9) içermelidir." };

        public override IdentityError PasswordRequiresLower() 
            => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Şifre en az bir küçük harf ('a'-'z') içermelidir." };

        public override IdentityError PasswordRequiresUpper() 
            => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Şifre en az bir büyük harf ('A'-'Z') içermelidir." };

        public override IdentityError DuplicateUserName(string userName) 
            => new IdentityError { Code = nameof(DuplicateUserName), Description = $"'{userName}' kullanıcı adı zaten alınmış." };

        public override IdentityError DuplicateEmail(string email) 
            => new IdentityError { Code = nameof(DuplicateEmail), Description = $"'{email}' e-posta adresi zaten kayıtlı." };

        public override IdentityError InvalidUserName(string userName) 
            => new IdentityError { Code = nameof(InvalidUserName), Description = $"Kullanıcı adı '{userName}' geçersiz karakterler içeriyor." };

        public override IdentityError InvalidEmail(string email) 
            => new IdentityError { Code = nameof(InvalidEmail), Description = $"'{email}' geçersiz bir e-posta adresi." };
    }
}