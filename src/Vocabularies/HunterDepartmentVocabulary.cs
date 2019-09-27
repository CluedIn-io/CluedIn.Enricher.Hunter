using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Hunter.Vocabularies
{
    public class HunterDepartmentVocabulary : SimpleVocabulary
    {
        public HunterDepartmentVocabulary()
        {
            this.Executive      = this.Add(new VocabularyKey("executive", VocabularyKeyDataType.Integer));
            this.It             = this.Add(new VocabularyKey("it", VocabularyKeyDataType.Integer));
            this.Finance        = this.Add(new VocabularyKey("finance", VocabularyKeyDataType.Integer));
            this.Management     = this.Add(new VocabularyKey("management", VocabularyKeyDataType.Integer));
            this.Sales          = this.Add(new VocabularyKey("sales", VocabularyKeyDataType.Integer));
            this.Legal          = this.Add(new VocabularyKey("legal", VocabularyKeyDataType.Integer));
            this.Support        = this.Add(new VocabularyKey("support", VocabularyKeyDataType.Integer));
            this.Hr             = this.Add(new VocabularyKey("hr", VocabularyKeyDataType.Integer));
            this.Marketing      = this.Add(new VocabularyKey("marketing", VocabularyKeyDataType.Integer));
            this.Communication  = this.Add(new VocabularyKey("communication", VocabularyKeyDataType.Integer));
        }

        public VocabularyKey Executive { get; set; }
        public VocabularyKey It { get; set; }
        public VocabularyKey Finance { get; set; }
        public VocabularyKey Management { get; set; }
        public VocabularyKey Sales { get; set; }
        public VocabularyKey Legal { get; set; }
        public VocabularyKey Support { get; set; }
        public VocabularyKey Hr { get; set; }
        public VocabularyKey Marketing { get; set; }
        public VocabularyKey Communication { get; set; }
    }
}

