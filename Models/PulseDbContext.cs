using Microsoft.EntityFrameworkCore;

namespace Pulse.Models
{
	public class PulseDbContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<AutoReply> AutoReplies { get; set; }
		public DbSet<Blacklist> Blacklists { get; set; }
		public DbSet<Contact> Contacts { get; set; }
		public DbSet<Conversation> Conversations { get; set; }
		public DbSet<Device> Devices { get; set; }
		public DbSet<Draft> Drafts { get; set; }
		public DbSet<Folder> Folders { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<ScheduledMessage> ScheduledMessages { get; set; }
		public DbSet<Template> Templates { get; set; }

		private static bool _ensureCreated = false;
		public PulseDbContext()
		{
			if (!_ensureCreated)
			{
				_ensureCreated = true;
				Database.EnsureCreated();
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=pulse.db");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			#region AccountModel

			modelBuilder.Entity<Account>().HasKey(d => d.AccountId);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.AutoReplies)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Blacklists)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Contacts)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Conversations)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Devices)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Drafts)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Folders)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Messages)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.ScheduledMessages)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Account>()
				.HasMany(g => g.Templates)
				.WithOne(s => s.Account)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			#endregion

			#region ConversationBody

			modelBuilder.Entity<Conversation>().HasKey(b => b.DeviceId);

			modelBuilder.Entity<Conversation>()
				.HasMany(c => c.Drafts)
				.WithOne(s => s.ConversationBody)
				.HasForeignKey(s => s.DeviceConversationId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Conversation>()
				.HasMany(c => c.Messages)
				.WithOne(s => s.ConversationBody)
				.HasForeignKey(s => s.DeviceConversationId)
				.OnDelete(DeleteBehavior.Cascade);

			#endregion

		}
	}
}
