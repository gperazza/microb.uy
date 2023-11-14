using FluentValidation;
using MicrobUy_API.Dtos.Enums;

namespace MicrobUy_API.Dtos
{
    public class CreateInstanceRequestDto
    {
        public string Nombre { get; set; }
        public string Dominio { get; set; }
        public string Logo { get; set; }
        public bool Activo { get; set; }
        public CreateTematicaRequestDto Tematica { get; set; }
        public string Description { get; set; }
        public EsquemaColoresEnum EsquemaColores { get; set; }
        public PrivacidadEnum Privacidad { get; set; }
    }

    public class CreateInstanceRequestValidator : AbstractValidator<CreateInstanceRequestDto>
    {
        public CreateInstanceRequestValidator()
        {
            RuleFor(instance => instance.Nombre).NotNull().NotEmpty().WithMessage("El nombre de la instancia es requerido");
            RuleFor(instance => instance.Dominio).NotNull().NotEmpty().WithMessage("El dominio de la instancia es requerida");
        }
    }
}