using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
	// DB 관계 모델링
	// 1:1
	// 1:다
	// 다:다

	// Entity 클래스 이름 = 테이블 이름 = Item	
	[Table("Item")]
	public class Item
	{
		// 이름Id -> PK
		public int ItemId { get; set; }
		public int TemplateId { get; set; } // 101 = 집행검
		public DateTime CreateDate { get; set; }

		// 다른 클래스 참조 -> FK (Navigational Property)
		[ForeignKey("OwnerId")]
		public Player Owner { get; set; }
	}

	// Entity 클래스 이름 = 테이블 이름 = Player	
	[Table("Player")]
	public class Player
	{
		// 이름Id -> PK
		public int PlayerId { get; set; }
		public string Name { get; set; }

		public Item Item { get; set; }
		public Guild Guild { get; set; }
	}

	[Table("Guild")]
	public class Guild
	{
		public int GuildId { get; set; }
		public string GuildName { get; set; }
		public ICollection<Player> Members { get; set; }
	}

	public class GuildDto
	{
		public string Name { get; set; }
		public int MemberCount { get; set; }
	}
}
