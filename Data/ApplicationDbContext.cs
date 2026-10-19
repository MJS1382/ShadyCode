using Microsoft.EntityFrameworkCore;
using PersonalBlog.Models;

namespace PersonalBlog.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(x => x.UserName)
            .IsUnique();

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(x => x.Email)
            .IsUnique();

        //Explicitly mapped the post-owner relationship
        modelBuilder.Entity<Post>()
            .HasOne(post => post.Author)
            .WithMany(user => user.Posts)
            .HasForeignKey(post => post.ApplicationUserId);

        //Explicitly mapped the comment-owner relationship
        modelBuilder.Entity<Comment>()
            .HasOne(comment => comment.User)
            .WithMany()
            .HasForeignKey(comment => comment.ApplicationUserId);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(user => user.LikedPosts)
            .WithMany(post => post.LikedByUsers)
            .UsingEntity<Dictionary<string, object>>(
                "PostLikes",
                postLikes => postLikes
                    .HasOne<Post>()
                    .WithMany()
                    .HasForeignKey("PostId"),
                postLikes => postLikes
                    .HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey("UserId"),
                postLikes => postLikes.ToTable("PostLikes"));

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(user => user.LikedComments)
            .WithMany(comment => comment.LikedByUsers)
            .UsingEntity<Dictionary<string, object>>(
                "CommentLikes",
                commentLikes => commentLikes
                    .HasOne<Comment>()
                    .WithMany()
                    .HasForeignKey("CommentId"),
                commentLikes => commentLikes
                    .HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey("UserID"),
                commentLikes => commentLikes.ToTable("CommentLikes"));
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ProjectMetadata> ProjectsMetadata { get; set; }
}