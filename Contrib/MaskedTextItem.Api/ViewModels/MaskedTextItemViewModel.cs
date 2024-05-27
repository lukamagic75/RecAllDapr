namespace RecAll.Contrib.MaskedTextItem.Api.ViewModels;

public class MaskedTextItemViewModel {
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public string Content { get; set; }
    
    public string MaskedContent { get; set; }
}