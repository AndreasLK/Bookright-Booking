namespace Domain.Value_Objects.Ids
{
        public record CampaignId(Guid Value) : StronglyTypedId<Guid>(Value);
}
