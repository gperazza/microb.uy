using FluentValidation;
using MicrobUy_API.Data;
using MicrobUy_API.Models;
using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Dtos
{
    public class UserRegistrationRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string Biography { get; set; }
        public string Occupation { get; set; }
        public string City { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsSanctioned { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }

    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequestDto>
    {
        private readonly TenantAplicationDbContext _context;

        public UserRegistrationRequestValidator(TenantAplicationDbContext context)
        {
            _context = context;

            RuleFor(user => user.FirstName).NotNull().NotEmpty().WithMessage("El nombre es requerido");
            RuleFor(user => user.LastName).NotNull().NotEmpty().WithMessage("El apellido es requerido");
            RuleFor(user => user.Email).NotNull().NotEmpty().WithMessage("El email es requerido");
            RuleFor(user => user.Username).NotNull().NotEmpty().WithMessage("El username es requerido");
            RuleFor(user => user.Password).NotNull().NotEmpty().WithMessage("El password es requerido");
            RuleFor(user => user.ConfirmPassword).NotNull().NotEmpty().WithMessage("El confirmpassword es requerido");
            RuleFor(user => user.Role).NotNull().NotEmpty().WithMessage("El rol es requerido");
            RuleFor(user => user.Password).Equal(user => user.ConfirmPassword).When(user => !String.IsNullOrWhiteSpace(user.Password)).WithMessage("The password and confirmation password do not match.");

            RuleFor(x => x.Email)
               .Must(e =>
               {
                   var validEmail = _context.User.FirstOrDefault(x => x.Email == e);
                   return validEmail == null;
               })
               .WithErrorCode("AlreadyExists")
               .WithMessage("El email ya esta siendo utilizado para esta instancia");

            RuleFor(x => x.Username)
               .Must(us =>
                {
                    var ValidUserName = _context.User.FirstOrDefault(x => x.UserName == us.ToLower());
                    return ValidUserName == null;

            })
            .WithErrorCode("AlreadyExists")
            .WithMessage("El username ya esta siendo utilizado para esta instancia");
        }
    }
}

