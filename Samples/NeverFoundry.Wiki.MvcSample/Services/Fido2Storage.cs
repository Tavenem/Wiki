using Fido2NetLib;
using Microsoft.EntityFrameworkCore;
using NeverFoundry.Wiki.Sample.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Sample.Services
{
    public class Fido2Storage
    {
        private readonly IdentityDbContext _identityDbContext;

        public Fido2Storage(IdentityDbContext identityDbContext) => _identityDbContext = identityDbContext;

        public Task<List<FidoStoredCredential>> GetCredentialsByUsernameAsync(string username)
            => _identityDbContext.FidoStoredCredential.AsQueryable().Where(x => x.Username == username).ToListAsync();

        public async Task RemoveCredentialsByUsername(string username)
        {
            var item = await _identityDbContext.FidoStoredCredential.AsQueryable()
                .Where(c => c.Username == username).FirstOrDefaultAsync().ConfigureAwait(false);
            if (item != null)
            {
                _identityDbContext.FidoStoredCredential.Remove(item);
                await _identityDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<FidoStoredCredential> GetCredentialById(byte[] id)
        {
            var credentialIdString = Base64Url.Encode(id);
            //byte[] credentialIdStringByte = Base64Url.Decode(credentialIdString);

            return await _identityDbContext.FidoStoredCredential.AsQueryable()
                .Where(c => !string.IsNullOrEmpty(c.DescriptorJson) && c.DescriptorJson.Contains(credentialIdString)).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public Task<List<FidoStoredCredential>> GetCredentialsByUserHandleAsync(byte[] userHandle)
            => Task.FromResult(_identityDbContext.FidoStoredCredential.AsQueryable()
                .Where(c => c.UserHandle != null && c.UserHandle.SequenceEqual(userHandle)).ToList());

        public async Task UpdateCounter(byte[] credentialId, uint counter)
        {
            var credentialIdString = Base64Url.Encode(credentialId);
            //byte[] credentialIdStringByte = Base64Url.Decode(credentialIdString);

            var cred = await _identityDbContext.FidoStoredCredential.AsQueryable()
                .Where(c => !string.IsNullOrEmpty(c.DescriptorJson) && c.DescriptorJson.Contains(credentialIdString)).FirstOrDefaultAsync().ConfigureAwait(false);

            cred.SignatureCounter = counter;
            await _identityDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddCredentialToUser(Fido2User user, FidoStoredCredential credential)
        {
            credential.UserId = user.Id;
            _identityDbContext.FidoStoredCredential.Add(credential);
            await _identityDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<Fido2User>> GetUsersByCredentialIdAsync(byte[] credentialId)
        {
            var credentialIdString = Base64Url.Encode(credentialId);
            //byte[] credentialIdStringByte = Base64Url.Decode(credentialIdString);

            var cred = await _identityDbContext.FidoStoredCredential.AsQueryable()
                .Where(c => !string.IsNullOrEmpty(c.DescriptorJson) && c.DescriptorJson.Contains(credentialIdString)).FirstOrDefaultAsync().ConfigureAwait(false);

            if (cred?.UserId is null)
            {
                return new List<Fido2User>();
            }

            return await _identityDbContext.Users.AsQueryable()
                    .Where(u => Encoding.UTF8.GetBytes(u.UserName)
                    .SequenceEqual(cred.UserId))
                    .Select(u => new Fido2User
                    {
                        DisplayName = u.UserName,
                        Name = u.UserName,
                        Id = Encoding.UTF8.GetBytes(u.UserName) // byte representation of userID is required
                    }).ToListAsync().ConfigureAwait(false);
        }
    }
}
