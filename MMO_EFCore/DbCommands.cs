using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMO_EFCore
{
	// 오늘의 주제 : State (상태)
	// 0) Detached (No Tracking ! 추적되지 않는 상태. SaveChanges를 해도 존재도 모름)
	// 1) Unchanged (DB에 있고, 딱히 수정사항도 없었음. SaveChanges를 해도 아무 것도 X)
	// 2) Deleted (DB에는 아직 있지만, 삭제되어야 함. SaveChanges로 DB에 적용)
	// 3) Modified (DB에 있고, 클라에서 수정된 상태. SaveChanges로 DB에 적용)
	// 4) Added (DB에는 아직 없음. SaveChanges로 DB에 적용)

	public class DbCommands
	{
		// 초기화 시간이 좀 걸림
		public static void InitializeDB(bool forceReset = false)
		{
			using (AppDbContext db = new AppDbContext())
			{
				if (!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
					return;

				db.Database.EnsureDeleted();
				db.Database.EnsureCreated();

				CreateTestData(db);
				Console.WriteLine("DB Initialized");
			}
		}

		public static void CreateTestData(AppDbContext db)
		{
			var rookiss = new Player() { Name = "Rookiss" };
			var faker = new Player() { Name = "Faker" };
			var deft = new Player() { Name = "Deft" };

			List<Item> items = new List<Item>()
			{
				new Item()
				{
					TemplateId = 101,
					CreateDate = DateTime.Now,
					Owner = rookiss
				},
				new EventItem()
				{
					TemplateId = 102,
					CreateDate = DateTime.Now,
					Owner = faker,
					DestroyDate = DateTime.Now
				},
				new Item()
				{
					TemplateId = 103,
					CreateDate = DateTime.Now,
					Owner = deft
				}
			};

			// Test Owned Type
			items[0].Option = new ItemOption() { Dex = 1, Hp = 2, Str = 3 };

			items[2].Detail = new ItemDetail()
			{ 
				Description = "This is good item"
			};
			

			Guild guild = new Guild()
			{
				GuildName = "T1",
				Members = new List<Player>() { rookiss, faker, deft }
			};

			db.Items.AddRange(items);
			db.Guilds.Add(guild);
			
			db.SaveChanges();
		}

		public static void ShowItems()
		{
			using (AppDbContext db = new AppDbContext())
			{
				foreach (var item in db.Items.Include(i => i.Owner).Include(i => i.Detail).IgnoreQueryFilters().ToList())
				{
					if (item.SoftDeleted)
					{
						Console.WriteLine($"DELETED - ItemId({item.ItemId}) TemplateId({item.TemplateId})");
					}
					else
					{
						// Test Owned Type
						if (item.Option != null)
							Console.WriteLine("STR " + item.Option.Str);

						// Test TPH
						//item.Type == ItemType.EventItem
						EventItem eventItem = item as EventItem;
						if (eventItem != null)
							Console.WriteLine("DestroyDate: " + eventItem.DestroyDate);

						// Test Table Splitting
						if (item.Detail != null)
							Console.WriteLine(item.Detail.Description);

						if (item.Owner == null)
							Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) Owner(0)");
						else
							Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) OwnerId({item.Owner.PlayerId}) Owner({item.Owner.Name})");
					}
				}
			}
		}

		public static void ShowGuild()
		{
			using (AppDbContext db = new AppDbContext())
			{
				foreach (var guild in db.Guilds.Include(g => g.Members).ToList())
				{
					Console.WriteLine($"GuildId({guild.GuildId}) GuildName({guild.GuildName}) MemberCount({guild.Members.Count})");
				}
			}
		}
	}
}
