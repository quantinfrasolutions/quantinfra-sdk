namespace QuantInfra.Sdk.StaticData;

public class Asset
{		
	public Asset() { }

	public Asset(Asset asset)
	{
		AssetId = asset.AssetId;
		Name = asset.Name;
		Description = asset.Description;
		AssetType = asset.AssetType;
	}
	
	public int AssetId { get; init; }
	public AssetType AssetType { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
}