using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NeverFoundry.Wiki.Sample.Data;
using NeverFoundry.Wiki.Sample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MVCSample.Controllers
{
    [Route("api/[controller]")]
    public class Fido2Controller : Controller
    {
        private readonly Fido2Storage _fido2Storage;
        private readonly Fido2 _lib;
        private readonly IOptions<Fido2Configuration> _optionsFido2Configuration;
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly UserManager<WikiUser> _userManager;

        public Fido2Controller(
            Fido2Storage fido2Storage,
            IOptions<Fido2Configuration> optionsFido2Configuration,
            SignInManager<WikiUser> signInManager,
            UserManager<WikiUser> userManager)
        {
            _fido2Storage = fido2Storage;
            _optionsFido2Configuration = optionsFido2Configuration;
            _signInManager = signInManager;
            _userManager = userManager;

            _lib = new Fido2(_optionsFido2Configuration.Value);
        }

        [HttpPost("pwassertionOptions")]
        public async Task<ActionResult> AssertionOptionsPost([FromForm] string username, [FromForm] string userVerification)
        {
            try
            {
                var existingCredentials = new List<PublicKeyCredentialDescriptor>();

                if (!string.IsNullOrEmpty(username))
                {
                    var identityUser = await _userManager.FindByNameAsync(username).ConfigureAwait(false);
                    var user = new Fido2User
                    {
                        DisplayName = identityUser.UserName,
                        Name = identityUser.UserName,
                        Id = Encoding.UTF8.GetBytes(identityUser.UserName) // byte representation of userID is required
                    };

                    if (user is null)
                    {
                        throw new ArgumentException("Username was not registered");
                    }

                    // 2. Get registered credentials from database
                    var items = await _fido2Storage.GetCredentialsByUsernameAsync(identityUser.UserName).ConfigureAwait(false);
                    existingCredentials = items.Where(x => x.Descriptor != null).Select(c => c.Descriptor).OfType<PublicKeyCredentialDescriptor>().ToList();
                }

                var exts = new AuthenticationExtensionsClientInputs()
                {
                    SimpleTransactionAuthorization = "FIDO",
                    GenericTransactionAuthorization = new TxAuthGenericArg { ContentType = "text/plain", Content = new byte[] { 0x46, 0x49, 0x44, 0x4F } },
                    UserVerificationIndex = true,
                    Location = true,
                    UserVerificationMethod = true
                };

                // 3. Create options
                var uv = string.IsNullOrEmpty(userVerification) ? UserVerificationRequirement.Discouraged : userVerification.ToEnum<UserVerificationRequirement>();
                var options = _lib.GetAssertionOptions(
                    existingCredentials,
                    uv,
                    exts
                );

                // 4. Temporarily store options, session/in-memory cache/redis/db
                HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

                // 5. Return options to client
                return Json(options);
            }
            catch (Exception e)
            {
                return Json(new AssertionOptions { Status = "error", ErrorMessage = FormatException(e) });
            }
        }

        [HttpPost("pwmakeAssertion")]
        public async Task<JsonResult> MakeAssertion([FromBody] AuthenticatorAssertionRawResponse clientResponse)
        {
            try
            {
                // 1. Get the assertion options we sent the client
                var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
                var options = AssertionOptions.FromJson(jsonOptions);

                // 2. Get registered credential from database
                var creds = await _fido2Storage.GetCredentialById(clientResponse.Id).ConfigureAwait(false);
                if (creds is null)
                {
                    throw new Exception("Unknown credentials");
                }

                // 3. Get credential counter from database
                var storedCounter = creds.SignatureCounter;

                // 4. Create callback to check if userhandle owns the credentialId
                async Task<bool> callback(IsUserHandleOwnerOfCredentialIdParams args)
                {
                    var storedCreds = await _fido2Storage.GetCredentialsByUserHandleAsync(args.UserHandle).ConfigureAwait(false);
                    return storedCreds.Exists(c => c.Descriptor?.Id.SequenceEqual(args.CredentialId) == true);
                }

                // 5. Make the assertion
                var res = await _lib.MakeAssertionAsync(clientResponse, options, creds.PublicKey, storedCounter, callback).ConfigureAwait(false);

                // 6. Store the updated counter
                await _fido2Storage.UpdateCounter(res.CredentialId, res.Counter).ConfigureAwait(false);

                var identityUser = await _userManager.FindByNameAsync(creds.Username).ConfigureAwait(false);
                if (identityUser is null)
                {
                    throw new InvalidOperationException($"Unable to load user.");
                }

                await _signInManager.SignInAsync(identityUser, isPersistent: false).ConfigureAwait(false);

                // 7. return OK to client
                return Json(res);
            }
            catch (Exception e)
            {
                return Json(new AssertionVerificationResult { Status = "error", ErrorMessage = FormatException(e) });
            }
        }

        [HttpPost("pwmakeCredentialOptions")]
        public async Task<JsonResult> MakeCredentialOptions(
            [FromForm] string username,
            [FromForm] string displayName,
            [FromForm] string attType,
            [FromForm] string authType,
            [FromForm] bool requireResidentKey,
            [FromForm] string userVerification)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    username = $"{displayName} ({Guid.NewGuid()})";
                }

                var user = new Fido2User
                {
                    DisplayName = displayName,
                    Name = username,
                    Id = Encoding.UTF8.GetBytes(username) // byte representation of userID is required
                };

                // 2. Get user existing keys by username
                var items = await _fido2Storage.GetCredentialsByUsernameAsync(username).ConfigureAwait(false);
                var existingKeys = new List<PublicKeyCredentialDescriptor>();
                foreach (var publicKeyCredentialDescriptor in items.Where(x => x.Descriptor != null))
                {
                    existingKeys.Add(publicKeyCredentialDescriptor.Descriptor!);
                }

                // 3. Create options
                var authenticatorSelection = new AuthenticatorSelection
                {
                    RequireResidentKey = requireResidentKey,
                    UserVerification = userVerification.ToEnum<UserVerificationRequirement>()
                };

                if (!string.IsNullOrEmpty(authType))
                {
                    authenticatorSelection.AuthenticatorAttachment = authType.ToEnum<AuthenticatorAttachment>();
                }

                var exts = new AuthenticationExtensionsClientInputs()
                {
                    Extensions = true,
                    UserVerificationIndex = true,
                    Location = true,
                    UserVerificationMethod = true,
                    BiometricAuthenticatorPerformanceBounds = new AuthenticatorBiometricPerfBounds { FAR = float.MaxValue, FRR = float.MaxValue }
                };

                var options = _lib.RequestNewCredential(user, existingKeys, authenticatorSelection, attType.ToEnum<AttestationConveyancePreference>(), exts);

                // 4. Temporarily store options, session/in-memory cache/redis/db
                HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

                // 5. return options to client
                return Json(options);
            }
            catch (Exception e)
            {
                return Json(new CredentialCreateOptions { Status = "error", ErrorMessage = FormatException(e) });
            }
        }

        [HttpPost("pwmakeCredential")]
        public async Task<JsonResult> MakeCredential([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
        {
            try
            {
                // 1. get the options we sent the client
                var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
                var options = CredentialCreateOptions.FromJson(jsonOptions);

                // 2. Create callback so that lib can verify credential id is unique to this user
                async Task<bool> callback(IsCredentialIdUniqueToUserParams args)
                {
                    var users = await _fido2Storage.GetUsersByCredentialIdAsync(args.CredentialId).ConfigureAwait(false);
                    return users.Count == 0;
                }

                // 2. Verify and make the credentials
                var success = await _lib.MakeNewCredentialAsync(attestationResponse, options, callback).ConfigureAwait(false);

                // 3. Store the credentials in db
                await _fido2Storage.AddCredentialToUser(options.User, new FidoStoredCredential
                {
                    Username = options.User.Name,
                    Descriptor = new PublicKeyCredentialDescriptor(success.Result.CredentialId),
                    PublicKey = success.Result.PublicKey,
                    UserHandle = success.Result.User.Id,
                    SignatureCounter = success.Result.Counter,
                    CredType = success.Result.CredType,
                    RegDate = DateTime.Now,
                    AaGuid = success.Result.Aaguid
                }).ConfigureAwait(false);

                // 4. return "ok" to the client

                var user = await GetOrCreateUser(options.User.Name, options.User.DisplayName).ConfigureAwait(false);
                // await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    return Json(new Fido2.CredentialMakeResult { Status = "error", ErrorMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'." });
                }

                return Json(success);
            }
            catch (Exception e)
            {
                return Json(new Fido2.CredentialMakeResult { Status = "error", ErrorMessage = FormatException(e) });
            }
        }

        private async Task<WikiUser?> GetOrCreateUser(string userEmail, string displayName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail).ConfigureAwait(false);
            if (user is null)
            {
                user = new WikiUser { UserName = displayName, Email = userEmail, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user).ConfigureAwait(false);
                return result.Succeeded ? user : null;
            }
            else
            {
                return user;
            }
        }

        private string FormatException(Exception e)
            => e.Message + (e.InnerException != null ? $" ({e.InnerException.Message})" : string.Empty);
    }
}
