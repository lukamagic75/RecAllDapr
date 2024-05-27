using System.ComponentModel.DataAnnotations;

namespace RecAll.Contrib.MaskedTextItem.Api.Commands;

public class UpdateMaskedTextItemCommand {
    [Required] public int Id { get; set; }
    [Required] public string Content { get; set; }
    [Required] public string MaskedContent { get; set; }
}