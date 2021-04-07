using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyInvites.Models
{
    public static class Repository
    {
        private static readonly List<GuestResponse> _responses = new List<GuestResponse>();

        public static IEnumerable<GuestResponse> Responses => _responses;

        public static void AddResponse(GuestResponse guestResponse)
        {
            _responses.Add(guestResponse);
        }
    }
}
