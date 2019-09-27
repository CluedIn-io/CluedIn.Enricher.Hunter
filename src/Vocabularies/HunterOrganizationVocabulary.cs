using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Hunter.Vocabularies
{
    public class HunterOrganizationVocabulary : SimpleVocabulary
    {
        public HunterOrganizationVocabulary()
        {
            this.VocabularyName = "Hunter Organization";
            this.KeyPrefix      = "hunter.organization";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Organization;

            AddGroup("Details", group =>
            {
                this.Organization = group.Add(new VocabularyKey("organization", VocabularyKeyDataType.OrganizationName));
                this.Domain       = group.Add(new VocabularyKey("domain"));
                this.Webmail      = group.Add(new VocabularyKey("webmail", VocabularyKeyDataType.Boolean));
                this.Disposable   = group.Add(new VocabularyKey("disposable"));
                this.Pattern      = group.Add(new VocabularyKey("pattern"));
            });

            this.AddMapping(Organization, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName);
            this.AddMapping(Domain, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames);
        }

        public VocabularyKey Domain { get; set; }
        public VocabularyKey Disposable { get; set; }
        public VocabularyKey Webmail { get; set; }
        public VocabularyKey Pattern { get; set; }
        public VocabularyKey Organization { get; set; }
    }
}
