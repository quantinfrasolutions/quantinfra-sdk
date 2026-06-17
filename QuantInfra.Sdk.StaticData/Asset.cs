namespace QuantInfra.Sdk.StaticData;

public class Asset
{		
	public int AssetId { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
	public AssetType AssetType { get; init; }
}