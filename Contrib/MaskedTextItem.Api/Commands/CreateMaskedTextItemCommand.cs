using System.ComponentModel.DataAnnotations;

namespace RecAll.Contrib.MaskedTextItem.Api.Commands;

public class CreateMaskedTextItemCommand {
    [Required] public string Content { get; set; }
    [Required] public string MaskedContent { get; set; }
}