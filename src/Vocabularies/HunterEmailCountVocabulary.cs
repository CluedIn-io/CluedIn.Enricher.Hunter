using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Hunter.Vocabularies
{
    public class HunterEmailCountVocabulary : SimpleVocabulary
    {
        public HunterEmailCountVocabulary()
        {
            this.VocabularyName = "Hunter Email Count";
            this.KeyPrefix      = "hunter.emailCount";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Organization;

            AddGroup("Details", group =>
            {
                this.Total          = group.Add(new VocabularyKey("total", VocabularyKeyDataType.Integer));
                this.PersonalEmails = group.Add(new VocabularyKey("personalEmails", VocabularyKeyDataType.Integer));
                this.GenericEmails  = group.Add(new VocabularyKey("genericEmails", VocabularyKeyDataType.Integer));
            });

            AddGroup("Seniority", group =>
            {
                this.Junior = group.Add(new VocabularyKey("junior", VocabularyKeyDataType.Integer));
                this.Senior = group.Add(new VocabularyKey("senior", VocabularyKeyDataType.Integer));
                this.Executive = group.Add(new VocabularyKey("executive", VocabularyKeyDataType.Integer));
            });

            AddGroup("Department", group =>
            {
                this.Department = group.Add(new HunterDepartmentVocabulary().AsCompositeKey("department"));
            });
        }

        public VocabularyKey Total { get; set; }
        public VocabularyKey PersonalEmails { get; set; }

        public VocabularyKey GenericEmails { get; set; }
        public VocabularyKey Junior { get; set; }
        public VocabularyKey Senior { get; set; }
        public VocabularyKey Executive { get; set; }

        public HunterDepartmentVocabulary Department { get; internal set; }
    } 
}
     
