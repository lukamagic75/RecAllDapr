using System.ComponentModel.DataAnnotations;

namespace RecAll.Contrib.TextItem.Api.Commands;

public class CreateTextItemCommand {
    [Required] public string Content { get; set; }
}