using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.Hunter.Models;
using CluedIn.ExternalSearch.Providers.Hunter.Vocabularies;

using RestSharp;

namespace CluedIn.ExternalSearch.Providers.Hunter
{
    /// <summary>The hunter graph external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class HunterExternalSearchProvider : ExternalSearchProviderBase
    {
        private RestClient _client = new RestClient("https://api.hunter.io/v2");
        private readonly int limit = 1;

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        public HunterExternalSearchProvider()
            : base(HunterConstants.Id, EntityType.Organization)
        {
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            var existingResults = request.GetQueryResults<HunterResponse<DomainSearch>>(this).ToList();

            Func<string, bool> nameFilter = value => OrganizationFilters.NameFilter(context, value) || existingResults.Any(r => string.Equals(r.Data.Data.Organization, value, StringComparison.InvariantCultureIgnoreCase));

            var entityType = request.EntityMetaData.EntityType;
            var organizationName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());
            var emailDomainNames = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, new HashSet<string>());

            var firstName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.FirstName, new HashSet<string>());
            var lastName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.LastName, new HashSet<string>());
            var email = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInPerson.Email, new HashSet<string>());

            organizationName.AddRange(request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.Organization, new HashSet<string>()));
            firstName.AddRange(request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.FirstName, new HashSet<string>()));
            lastName.AddRange(request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.LastName, new HashSet<string>()));
            var fullName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.FullName, new HashSet<string>());
            email.AddRange(request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.Email, new HashSet<string>()));

            if (firstName.FirstOrDefault() != null)
                if (lastName.FirstOrDefault() != null)
                    fullName.Add(string.Format("{0} {1}", firstName.FirstOrDefault(), lastName.FirstOrDefault()));

            foreach (var value in email)
                emailDomainNames.Add(new MailAddress(value).Host);

            if (request.EntityMetaData.Uri != null)
                emailDomainNames.Add(request.EntityMetaData.Uri.Host);

            if (entityType == EntityType.Organization)
            {
                organizationName.Add(request.EntityMetaData.Name);
                organizationName.Add(request.EntityMetaData.DisplayName);
            }
            else
            {
                fullName.Add(request.EntityMetaData.Name);
                fullName.Add(request.EntityMetaData.DisplayName);
            }

            if (organizationName.Any())
            {
                foreach (var value in organizationName.Distinct())
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);

                    if (fullName.Any())
                    {
                        foreach (var name in fullName)
                        {
                            if (string.IsNullOrEmpty(name))
                                continue;
                            var queryParameters = new Dictionary<string, string>();
                            queryParameters.Add(ExternalSearchQueryParameter.Name.ToString(), value);
                            queryParameters.Add("fullName", name);
                            yield return new ExternalSearchQuery(this, entityType, queryParameters);
                        }
                    }
                    if (email.Any())
                    {
                        foreach (var name in email)
                        {
                            if (string.IsNullOrEmpty(name))
                                continue;
                            var queryParameters = new Dictionary<string, string>();
                            queryParameters.Add(ExternalSearchQueryParameter.Name.ToString(), value);
                            queryParameters.Add(ExternalSearchQueryParameter.Identifier.ToString(), name);
                            yield return new ExternalSearchQuery(this, entityType, queryParameters);
                        }
                    }
                }
            }
            if (emailDomainNames.Any())
            {
                foreach (var value in emailDomainNames.Distinct())
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Domain, value);

                    if (fullName.Any())
                    {
                        foreach (var name in fullName)
                        {
                            if (string.IsNullOrEmpty(name))
                                continue;
                            var queryParameters = new Dictionary<string, string>();
                            queryParameters.Add(ExternalSearchQueryParameter.Name.ToString(), value);
                            queryParameters.Add("fullName", name);
                            yield return new ExternalSearchQuery(this, entityType, queryParameters);
                        }
                    }
                    if (email.Any())
                    {
                        foreach (var name in email)
                        {
                            if (string.IsNullOrEmpty(name))
                                continue;
                            var queryParameters = new Dictionary<string, string>();
                            queryParameters.Add(ExternalSearchQueryParameter.Name.ToString(), value);
                            queryParameters.Add(ExternalSearchQueryParameter.Identifier.ToString(), name);
                            yield return new ExternalSearchQuery(this, entityType, queryParameters);
                        }
                    }
                }
            }
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            string organizationName = null;
            string domain = null;

            if (query.QueryParameters.Keys.Contains(ExternalSearchQueryParameter.Name.ToString()))
                organizationName = query.QueryParameters.GetValue(ExternalSearchQueryParameter.Name.ToString()).FirstOrDefault();
            if (query.QueryParameters.Keys.Contains(ExternalSearchQueryParameter.Domain.ToString()))
                domain = query.QueryParameters.GetValue(ExternalSearchQueryParameter.Domain.ToString()).FirstOrDefault();

            //_client.AddDefaultParameter("api_key", "60a0e7dafeba7bb2d86bdf4c7c2c2a9222547379");
            _client.AddDefaultParameter("api_key", "08909df3dd777fb9d83c726566d9e13c45c29086");
            _client.AddDefaultParameter("limit", limit.ToString());
            if (!string.IsNullOrEmpty(organizationName))
                _client.AddDefaultParameter("company", organizationName);
            if (!string.IsNullOrEmpty(domain))
                _client.AddDefaultParameter("domain", domain);

            var request = new RestRequest("domain-search", Method.GET);
            request.AddQueryParameter("offset", 0.ToString());
            var response = _client.Execute<HunterResponse<DomainSearch>>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data?.Data?.Domain != null)
                {
                    response.Data.Meta.Params.Company = organizationName;
                    response.Data.Meta.Params.Domain = domain;
                    yield return new ExternalSearchQueryResult<HunterResponse<DomainSearch>>(query, response.Data);
                }
            }
            else if (response.ErrorException != null)
                throw new AggregateException(response.ErrorException.Message, response.ErrorException);
            else
                throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
            //TODO
            //if (response.Data.Data.Emails != null)
            //{
            //    foreach (var value in response.Data.Data.Emails)
            //    {
            //        if (value.Sources != null)
            //            if (value.Sources.Any())
            //                yield return new ExternalSearchQueryResult<HunterResponse<Email>>(query, new HunterResponse<Email>() { Data = value, Meta = response.Data.Meta });
            //    }
            //}

            foreach (var result in ExecuteEmailCountSearch(query))
                yield return result;
        }


        private IEnumerable<IExternalSearchQueryResult> ExecuteEmailVerifierSearch(IExternalSearchQuery query)
        {
            string email = null;
            if (query.QueryParameters.Keys.Contains(ExternalSearchQueryParameter.Identifier.ToString()))
                email = query.QueryParameters.GetValue(ExternalSearchQueryParameter.Identifier.ToString()).FirstOrDefault();

            if (!string.IsNullOrEmpty(email))
            {
                var request = new RestRequest("email-verifier", Method.GET);
                request.AddQueryParameter("email", email);

                var response = _client.Execute<HunterResponse<Email>>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Data?.Data?.Value != null)
                        yield return new ExternalSearchQueryResult<HunterResponse<Email>>(query, response.Data);
                }
                else if (response.ErrorException != null)
                    throw new AggregateException(response.ErrorException.Message, response.ErrorException);
                else
                    throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
            }
        }

        private IEnumerable<IExternalSearchQueryResult> ExecuteEmailCountSearch(IExternalSearchQuery query)
        {
            var request = new RestRequest("email-count", Method.GET);

            var response = _client.Execute<HunterResponse<EmailCount>>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data?.Data != null)
                    yield return new ExternalSearchQueryResult<HunterResponse<EmailCount>>(query, response.Data);
            }
            else if (response.ErrorException != null)
                throw new AggregateException(response.ErrorException.Message, response.ErrorException);
            else
                throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);

        }

        private IEnumerable<IExternalSearchQueryResult> ExecuteEmailFinderSearch(IExternalSearchQuery query)
        {
            string fullName = null;

            if (query.QueryParameters.Keys.Contains("fullName"))
                fullName = query.QueryParameters.GetValue("fullName").FirstOrDefault();

            if (!string.IsNullOrEmpty(fullName))
            {
                long offset = 0;
                long count = 0;
                bool hasMore = true;
                while (hasMore)
                {
                    var request = new RestRequest("email-finder", Method.GET);
                    request.AddQueryParameter("full_name", fullName);
                    request.AddQueryParameter("offset", offset.ToString());

                    var response = _client.Execute<HunterResponse<Email>>(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (response.Data?.Data?.Value != null)
                            yield return new ExternalSearchQueryResult<HunterResponse<Email>>(query, response.Data);
                        count += limit;
                        if (response.Data.Meta.Results <= count)
                            hasMore = false;
                    }
                    else if (response.ErrorException != null)
                        throw new AggregateException(response.ErrorException.Message, response.ErrorException);
                    else
                        throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
                }
            }
        }

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            if (result.As<HunterResponse<DomainSearch>>() != null)
            {
                var resultItem = result.As<HunterResponse<DomainSearch>>();

                var code = this.GetOriginEntityCode(resultItem);

                var clue = new Clue(code, context.Organization);

                this.PopulateMetadata(clue.Data.EntityData, resultItem);

                yield return clue;
            }
            if (result.As<HunterResponse<EmailVerifier>>() != null)
            {
                var resultItem = result.As<HunterResponse<EmailVerifier>>();

                var code = this.GetOriginEntityCode(resultItem);

                var clue = new Clue(code, context.Organization);

                this.PopulateMetadata(clue.Data.EntityData, resultItem);

                yield return clue;
            }
            if (result.As<HunterResponse<EmailCount>>() != null)
            {
                var resultItem = result.As<HunterResponse<EmailCount>>();

                var code = this.GetOriginEntityCode(resultItem);

                var clue = new Clue(code, context.Organization);

                this.PopulateMetadata(clue.Data.EntityData, resultItem);

                yield return clue;
            }
            if (result.As<HunterResponse<Email>>() != null)
            {
                var resultItem = result.As<HunterResponse<Email>>();

                var code = this.GetOriginEntityCode(resultItem);

                var clue = new Clue(code, context.Organization);

                this.PopulateMetadata(clue.Data.EntityData, resultItem);

                yield return clue;
            }
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItemDomainSearch = result.As<HunterResponse<DomainSearch>>();

            if (resultItemDomainSearch == null)
            {
                var resultItemEmailCount = result.As<HunterResponse<EmailCount>>();

                if (resultItemEmailCount == null)
                {
                    var resultItemEmailVerifier = result.As<HunterResponse<EmailVerifier>>();

                    //        if (resultItemEmailVerifier == null)
                    //        {
                    var resultItemEmail = result.As<HunterResponse<Email>>();

                    if (resultItemEmail != null)
                        return this.CreateMetadata(resultItemEmail);
                    //    }
                    //    return this.CreateMetadata(resultItemEmailVerifier);
                }
                return this.CreateMetadata(resultItemEmailCount);
            }
            return this.CreateMetadata(resultItemDomainSearch);
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<HunterResponse<DomainSearch>> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<HunterResponse<EmailCount>> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<HunterResponse<EmailVerifier>> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<HunterResponse<Email>> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<HunterResponse<DomainSearch>> resultItem)
        {
            return new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultItem.Data.Meta.Params.Company ?? resultItem.Data.Meta.Params.Domain);
        }

        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<HunterResponse<EmailVerifier>> resultItem)
        {
            return new EntityCode(EntityType.Infrastructure.User, this.GetCodeOrigin(), resultItem.Data.Data.Email);
        }

        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<HunterResponse<Email>> resultItem)
        {
            return new EntityCode(EntityType.Infrastructure.User, this.GetCodeOrigin(), resultItem.Data.Data.Value);
        }

        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<HunterResponse<EmailCount>> resultItem)
        {
            return new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultItem.Data.Meta.Params.Company ?? resultItem.Data.Meta.Params.Domain);
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("hunter");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<HunterResponse<DomainSearch>> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);

            metadata.EntityType = EntityType.Organization;
            metadata.OriginEntityCode = code;
            metadata.Name = resultItem.Data.Meta.Params.Company;

            metadata.Codes.Add(code);


            metadata.Properties[HunterVocabulary.Organization.Organization] = resultItem.Data.Meta.Params.Company;
            metadata.Properties[HunterVocabulary.Organization.Domain] = resultItem.Data.Meta.Params.Domain;
            metadata.Properties[HunterVocabulary.Organization.Webmail] = resultItem.Data.Data.Webmail.PrintIfAvailable();
        }

        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<HunterResponse<EmailCount>> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);

            metadata.EntityType = EntityType.Organization;
            metadata.OriginEntityCode = code;
            metadata.Name = resultItem.Data.Meta.Params.Company;

            metadata.Codes.Add(code);
            metadata.Properties[HunterVocabulary.EmailCount.Total] = resultItem.Data.Data.Total.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailCount.GenericEmails] = resultItem.Data.Data.GenericEmails.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailCount.PersonalEmails] = resultItem.Data.Data.PersonalEmails.PrintIfAvailable();

            if (resultItem.Data.Data.Department != null)
            {
                metadata.Properties[HunterVocabulary.EmailCount.Department.Executive] = resultItem.Data.Data.Department.Executive.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.It] = resultItem.Data.Data.Department.It.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Finance] = resultItem.Data.Data.Department.Finance.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Management] = resultItem.Data.Data.Department.Management.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Sales] = resultItem.Data.Data.Department.Sales.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Legal] = resultItem.Data.Data.Department.Legal.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Support] = resultItem.Data.Data.Department.Support.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Hr] = resultItem.Data.Data.Department.Hr.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Marketing] = resultItem.Data.Data.Department.Marketing.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Department.Communication] = resultItem.Data.Data.Department.Communication.PrintIfAvailable();
            }

            if (resultItem.Data.Data.Seniority != null)
            {
                metadata.Properties[HunterVocabulary.EmailCount.Executive] = resultItem.Data.Data.Seniority.Executive.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Senior] = resultItem.Data.Data.Seniority.Senior.PrintIfAvailable();
                metadata.Properties[HunterVocabulary.EmailCount.Junior] = resultItem.Data.Data.Seniority.Junior.PrintIfAvailable();
            }
        }

        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<HunterResponse<Email>> email)
        {
            var code = this.GetOriginEntityCode(email);

            metadata.EntityType = EntityType.Infrastructure.User;
            metadata.OriginEntityCode = code;
            metadata.Codes.Add(code);
            metadata.Description = email.Data.Data.Position;

            if (!string.IsNullOrEmpty(email.Data.Data.FirstName))
                if (!string.IsNullOrEmpty(email.Data.Data.LastName))
                    metadata.Name = string.Format("{0} {1}", email.Data.Data.FirstName, email.Data.Data.LastName);
            if (string.IsNullOrEmpty(metadata.Name))
                metadata.Name = email.Data.Data.LastName;
            if (string.IsNullOrEmpty(metadata.Name))
                metadata.Name = email.Data.Data.FirstName;
            if (string.IsNullOrEmpty(metadata.Name))
                metadata.Name = email.Data.Data.Value;

            if (!string.IsNullOrEmpty(email.Data.Data.Twitter))
                metadata.Aliases.Add(email.Data.Data.Twitter);

            if (!string.IsNullOrEmpty(email.Data.Data.Value))
                metadata.Aliases.Add(email.Data.Data.Value);

            if (email.Data.Data.PhoneNumber != null)
                if (!string.IsNullOrEmpty(email.Data.Data.PhoneNumber.ToString()))
                    metadata.Aliases.Add(email.Data.Data.PhoneNumber.ToString());

            if (email.Data.Data.Linkedin != null)
                if (!string.IsNullOrEmpty(email.Data.Data.Linkedin.ToString()))
                    metadata.Aliases.Add(email.Data.Data.Linkedin.ToString());

            metadata.Properties[HunterVocabulary.Email.FirstName] = email.Data.Data.FirstName.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.Email.LastName] = email.Data.Data.LastName.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.Email.Value] = email.Data.Data.Value.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.Email.Twitter] = email.Data.Data.Twitter.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.Email.Linkedin] = email.Data.Data.Linkedin.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.Email.Position] = email.Data.Data.Position.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.Email.PhoneNumber] = email.Data.Data.PhoneNumber.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.Email.Confidence] = email.Data.Data.Confidence.PrintIfAvailable();
        }

        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<HunterResponse<EmailVerifier>> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);

            metadata.EntityType = EntityType.Infrastructure.User;
            metadata.OriginEntityCode = code;
            metadata.Name = resultItem.Data.Data.Email;

            metadata.Codes.Add(code);

            metadata.Properties[HunterVocabulary.EmailVerifier.Result] = resultItem.Data.Data.Result.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.Score] = resultItem.Data.Data.Score.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.Email] = resultItem.Data.Data.Email.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.Regexp] = resultItem.Data.Data.Regexp.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.Gibberish] = resultItem.Data.Data.Gibberish.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.Disposable] = resultItem.Data.Data.Disposable.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.Webmail] = resultItem.Data.Data.Webmail.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.MxRecords] = resultItem.Data.Data.MxRecords.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.SmtpServer] = resultItem.Data.Data.SmtpServer.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.SmtpCheck] = resultItem.Data.Data.SmtpCheck.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.AcceptAll] = resultItem.Data.Data.AcceptAll.PrintIfAvailable();
            metadata.Properties[HunterVocabulary.EmailVerifier.Block] = resultItem.Data.Data.Block.PrintIfAvailable();

        }

        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }
    }
}