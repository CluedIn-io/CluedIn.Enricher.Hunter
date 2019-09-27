using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Hunter.Vocabularies
{
    public class HunterEmailVerifierVocabulary : SimpleVocabulary
    {
        public HunterEmailVerifierVocabulary()
        {
            this.VocabularyName = "Hunter Email Verifier";
            this.KeyPrefix      = "hunter.emailVerifier";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Infrastructure.User;

            AddGroup("Details", group =>
            {
                this.Result     = group.Add(new VocabularyKey("result"));
                this.Score      = group.Add(new VocabularyKey("score", VocabularyKeyDataType.Integer));
                this.Email      = group.Add(new VocabularyKey("email", VocabularyKeyDataType.Email));
                this.Regexp     = group.Add(new VocabularyKey("regexp", VocabularyKeyDataType.Boolean));
                this.Gibberish  = group.Add(new VocabularyKey("gibberish", VocabularyKeyDataType.Boolean));
                this.Disposable = group.Add(new VocabularyKey("disposable", VocabularyKeyDataType.Boolean));
                this.Webmail    = group.Add(new VocabularyKey("webmail", VocabularyKeyDataType.Boolean));
                this.MxRecords  = group.Add(new VocabularyKey("mxRecords", VocabularyKeyDataType.Boolean));
                this.SmtpServer = group.Add(new VocabularyKey("smtpServer", VocabularyKeyDataType.Boolean));
                this.SmtpCheck  = group.Add(new VocabularyKey("smtpCheck", VocabularyKeyDataType.Boolean));
                this.AcceptAll  = group.Add(new VocabularyKey("acceptAll", VocabularyKeyDataType.Boolean));
                this.Block      = group.Add(new VocabularyKey("block", VocabularyKeyDataType.Boolean));
            });

            this.AddMapping(Email, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.Email);
        }

        public VocabularyKey Result { get; set; }
        public VocabularyKey Score { get; set; }
        public VocabularyKey Email { get; set; }
        public VocabularyKey Regexp { get; set; }
        public VocabularyKey Gibberish { get; set; }
        public VocabularyKey Disposable { get; set; }
        public VocabularyKey Webmail { get; set; }
        public VocabularyKey MxRecords { get; internal set; }
        public VocabularyKey SmtpServer { get; internal set; }
        public VocabularyKey SmtpCheck { get; internal set; }
        public VocabularyKey AcceptAll { get; internal set; }
        public VocabularyKey Block { get; internal set; }

    }
}
     
