using FluentValidation;
using MicrobUy_API.Dtos.Enums;
using MicrobUy_API.Models;
using System.Reflection.Metadata;

namespace MicrobUy_API.Dtos
{
    public class CreateInstanceRequestDto
    {
        public string Nombre { get; set; }
        public string Dominio { get; set; }
        public string Logo { get; set; }
        public bool Activo { get; set; }
        public string Tematica { get; set; }
        public EsquemaColoresEnum EsquemaColores { get; set; }
        public PrivacidadEnum Privacidad { get; set; }
    }


    public class CreateInstanceRequestValidator : AbstractValidator<CreateInstanceRequestDto>
    {
        public CreateInstanceRequestValidator()
        {
            RuleFor(instance => instance.Nombre).NotNull().NotEmpty().WithMessage("El nombre de la instancia es requerido");
            RuleFor(instance => instance.Dominio).NotNull().NotEmpty().WithMessage("La URL de la instancia es requerida");


        }
    }
}