using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMO_EFCore
{
    internal static class Extensions
    {

        public static IQueryable<GuildDto> MapGuildDto(this IQueryable<Guild> guild)
        {
            return guild.Select(g => new GuildDto()
            {
                Name = g.GuildName,
                MemberCount = g.Members.Count
            });
        }
    }
}
