namespace CluedIn.ExternalSearch.Providers.Hunter.Vocabularies
{
    public static class HunterVocabulary
    {
        static HunterVocabulary()
        {
            Organization = new HunterOrganizationVocabulary();
            Email = new HunterEmailVocabulary();
            EmailVerifier = new HunterEmailVerifierVocabulary();
            EmailCount = new HunterEmailCountVocabulary();
        }

        public static HunterOrganizationVocabulary Organization { get; private set; }
        public static HunterEmailVocabulary Email { get; private set; }
        public static HunterEmailVerifierVocabulary EmailVerifier { get; private set; }
        public static HunterEmailCountVocabulary EmailCount { get; private set; }

    }
}