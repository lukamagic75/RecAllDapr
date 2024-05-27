namespace RecAll.Contrib.MaskedTextItem.Api.Models;

public class MaskedTextItem
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public string Content { get; set; }

    public string MaskedContent { get; set; }

    public string UserIdentityGuid { get; set; }

    public bool IsDeleted { get; set; }
}