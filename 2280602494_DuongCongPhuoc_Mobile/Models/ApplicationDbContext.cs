using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using _2280602494_DuongCongPhuoc_Mobile.Models; // Ensure Models are accessible


namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>

    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Existing DbSets
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<WeddingTask> Tasks { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<GlobalMenuCatalog> GlobalMenuCatalogs { get; set; }
        public DbSet<GlobalMenuItem> GlobalMenuItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<EventVendor> EventVendors { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<WeddingTaskCategory> WeddingTaskCategories { get; set; }
        public DbSet<EventTimeline> EventTimelines { get; set; }
        public DbSet<ServicePackage> ServicePackages { get; set; }
        public DbSet<ServicePackageItem> ServicePackageItems { get; set; }
        public DbSet<PaymentMilestone> PaymentMilestones { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);
            builder.HasDefaultSchema("identity");
            
            // ========== EVENT CONFIGURATION ==========
            builder.Entity<Event>()
                .HasOne(e => e.EventCategory)
                .WithMany()
                .HasForeignKey(e => e.EventCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Event>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Event>()
                .Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Planning");
            
            builder.Entity<Event>()
                .Property(e => e.Budget)
                .HasColumnType("decimal(18,2)");
            
            // ========== REMINDER CONFIGURATION ==========
            builder.Entity<Reminder>()
                .HasOne(r => r.Event)
                .WithMany()
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // ========== GUEST CONFIGURATION ==========
            builder.Entity<Guest>()
                .HasOne(g => g.Event)
                .WithMany(e => e.Guests)
                .HasForeignKey(g => g.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Guest>()
                .Property(g => g.FullName)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Entity<Guest>()
                .Property(g => g.RSVPStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            
            // ========== MENU CONFIGURATION ==========
            builder.Entity<Menu>()
                .HasOne(m => m.Event)
                .WithMany(e => e.Menus)
                .HasForeignKey(m => m.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Menu>()
                .Property(m => m.MealType)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Entity<Menu>()
                .Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            // ========== MENU ITEM CONFIGURATION ==========
            builder.Entity<MenuItem>()
                .HasOne(mi => mi.Menu)
                .WithMany(m => m.MenuItems)
                .HasForeignKey(mi => mi.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<MenuItem>()
                .Property(mi => mi.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Entity<MenuItem>()
                .Property(mi => mi.UnitPrice)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<MenuItem>()
                .Property(mi => mi.TotalPrice)
                .HasColumnType("decimal(18,2)");
            
            // ========== GLOBAL MENU ITEM CONFIGURATION ==========
            builder.Entity<GlobalMenuItem>()
                .Property(gm => gm.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Entity<GlobalMenuItem>()
                .Property(gm => gm.UnitPrice)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<GlobalMenuItem>()
                .Property(gm => gm.TotalPrice)
                .HasColumnType("decimal(18,2)");
            
            // ========== GLOBAL MENU ITEM CONFIGURATION ==========
            builder.Entity<GlobalMenuItem>()
                .Property(gm => gm.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Entity<GlobalMenuItem>()
                .Property(gm => gm.UnitPrice)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<GlobalMenuItem>()
                .Property(gm => gm.TotalPrice)
                .HasColumnType("decimal(18,2)");

            // ========== BUDGET CONFIGURATION ==========
            builder.Entity<Budget>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Budgets)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Budget>()
                .Property(b => b.Category)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Entity<Budget>()
                .Property(b => b.BudgetedAmount)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<Budget>()
                .Property(b => b.ActualAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
            
            // ========== EXPENSE CONFIGURATION ==========
            // Using NoAction to avoid multiple cascade paths (Events -> Budgets -> Expenses and Events -> Expenses)
            builder.Entity<Expense>()
                .HasOne(ex => ex.Event)
                .WithMany(e => e.Expenses)
                .HasForeignKey(ex => ex.EventId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Expense>()
                .HasOne(ex => ex.Budget)
                .WithMany(b => b.Expenses)
                .HasForeignKey(ex => ex.BudgetId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Expense>()
                .HasOne(ex => ex.Vendor)
                .WithMany(v => v.Expenses)
                .HasForeignKey(ex => ex.VendorId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Expense>()
                .Property(ex => ex.Description)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Entity<Expense>()
                .Property(ex => ex.Amount)
                .HasColumnType("decimal(18,2)");
            
            // ========== TASK CONFIGURATION ==========
            builder.Entity<WeddingTask>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tasks)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<WeddingTask>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Self-referencing for sub-tasks
            builder.Entity<WeddingTask>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<WeddingTask>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Entity<WeddingTask>()
                .Property(t => t.Priority)
                .HasMaxLength(20)
                .HasDefaultValue("Normal");
            
            builder.Entity<WeddingTask>()
                .Property(t => t.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            
            // ========== VENDOR CONFIGURATION ==========
            builder.Entity<Vendor>()
                .Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Entity<Vendor>()
                .Property(v => v.VendorType)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Entity<Vendor>()
                .Property(v => v.Rating)
                .HasColumnType("decimal(3,2)");
            
            // ========== EVENT VENDOR CONFIGURATION ==========
            builder.Entity<EventVendor>()
                .HasOne(ev => ev.Event)
                .WithMany(e => e.EventVendors)
                .HasForeignKey(ev => ev.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<EventVendor>()
                .HasOne(ev => ev.Vendor)
                .WithMany(v => v.EventVendors)
                .HasForeignKey(ev => ev.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<EventVendor>()
                .Property(ev => ev.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            
            builder.Entity<EventVendor>()
                .Property(ev => ev.ContractAmount)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<EventVendor>()
                .Property(ev => ev.DepositAmount)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<EventVendor>()
                .Property(ev => ev.BalanceAmount)
                .HasColumnType("decimal(18,2)");

            // ========== PAYMENT MILESTONE CONFIGURATION ==========
            builder.Entity<PaymentMilestone>()
                .HasOne(pm => pm.EventVendor)
                .WithMany() // Assuming we don't need a navigation property back from EventVendor for now, or we can add it later
                .HasForeignKey(pm => pm.EventVendorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PaymentMilestone>()
                .Property(pm => pm.Amount)
                .HasColumnType("decimal(18,2)");

            // ========== TIMELINE CONFIGURATION ==========
            // Note: EventTimeline does not have a navigation property back from Event/Vendor in the simplified model provided earlier, 
            // but we can configure the constraints.
            // Assuming EventTimelines table has FK to Events and Vendors.
            
            // We need to ensure the table is created.
            builder.Entity<EventTimeline>().ToTable("EventTimelines");
            
            // Indexes
            builder.Entity<EventTimeline>()
                .HasIndex(t => t.EventId)
                .HasDatabaseName("IX_EventTimelines_EventId");
            
            // ========== INDEXES FOR PERFORMANCE ==========
            builder.Entity<Event>()
                .HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Events_UserId");
            
            builder.Entity<Event>()
                .HasIndex(e => e.StartTime)
                .HasDatabaseName("IX_Events_StartTime");
            
            builder.Entity<Guest>()
                .HasIndex(g => g.EventId)
                .HasDatabaseName("IX_Guests_EventId");
            
            builder.Entity<WeddingTask>()
                .HasIndex(t => t.EventId)
                .HasDatabaseName("IX_Tasks_EventId");
            
            builder.Entity<WeddingTask>()
                .HasIndex(t => t.AssignedToUserId)
                .HasDatabaseName("IX_Tasks_AssignedToUserId");
            
            builder.Entity<Expense>()
                .HasIndex(ex => ex.EventId)
                .HasDatabaseName("IX_Expenses_EventId");
        }
    }
}
