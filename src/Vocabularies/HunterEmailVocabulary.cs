using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Hunter.Vocabularies
{
    public class HunterEmailVocabulary : SimpleVocabulary
    {
        public HunterEmailVocabulary()
        {
            this.VocabularyName = "Hunter Email";
            this.KeyPrefix      = "hunter.email";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Infrastructure.User;

            AddGroup("Details", group =>
            {
                this.Value       = group.Add(new VocabularyKey("value", VocabularyKeyDataType.Email));
                this.Type        = group.Add(new VocabularyKey("type"));
                this.Confidence  = group.Add(new VocabularyKey("confidence"));
                this.FirstName   = group.Add(new VocabularyKey("firstName", VocabularyKeyDataType.PersonName));
                this.LastName    = group.Add(new VocabularyKey("lastName", VocabularyKeyDataType.PersonName));
                this.Position    = group.Add(new VocabularyKey("position"));
                this.Seniority   = group.Add(new VocabularyKey("seniority"));
                this.Department  = group.Add(new VocabularyKey("department"));
                this.Linkedin    = group.Add(new VocabularyKey("linkedIn"));
                this.Twitter     = group.Add(new VocabularyKey("twitter"));
                this.PhoneNumber = group.Add(new VocabularyKey("phoneNumber", VocabularyKeyDataType.PhoneNumber));
            });

            this.AddMapping(FirstName, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.FirstName);
            this.AddMapping(LastName, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.LastName);
            this.AddMapping(Position, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.JobTitle);
            this.AddMapping(PhoneNumber, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.PhoneNumber);
            this.AddMapping(Value, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.Email);
            this.AddMapping(Twitter, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.SocialTwitter);
            this.AddMapping(Linkedin, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.SocialLinkedIn);
        }

        public VocabularyKey Value { get; set; }
        public VocabularyKey Type { get; set; }
        public VocabularyKey Confidence { get; set; }
        public VocabularyKey FirstName { get; set; }
        public VocabularyKey LastName { get; set; }
        public VocabularyKey Position { get; set; }
        public VocabularyKey Seniority { get; set; }
        public VocabularyKey Department { get; internal set; }
        public VocabularyKey Linkedin { get; internal set; }
        public VocabularyKey Twitter { get; internal set; }
        public VocabularyKey PhoneNumber { get; internal set; }
    } 
}
     
